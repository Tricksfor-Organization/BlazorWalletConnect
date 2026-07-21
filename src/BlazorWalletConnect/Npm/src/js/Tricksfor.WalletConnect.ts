import { createAppKit, type AppKit, type CreateAppKit } from '@reown/appkit'
import { EthersAdapter } from '@reown/appkit-adapter-ethers'
import {
    arbitrum,
    avalanche,
    bsc,
    mainnet,
    optimism,
    polygon,
    type AppKitNetwork
} from '@reown/appkit/networks'
import {
    BrowserProvider,
    Contract,
    JsonRpcProvider,
    type Eip1193Provider,
    type EventLog,
    type Provider,
    type TransactionRequest
} from 'ethers'
import {
    accountsEqual,
    assertSupportedChainId,
    bigIntegerReplacer,
    normalizeChainId,
    serializeError,
    serializeTransactionReceipt,
    unsubscribeAll,
    type WalletAccount
} from './WalletConnectUtilities'

interface DotNetInterop {
    invokeMethodAsync(methodName: string, ...args: unknown[]): Promise<unknown>
}

interface CustomChain {
    chainId: number
    rpcUrl: string | null
}

interface WalletConnectConfiguration {
    projectId: string
    name: string
    description: string
    url: string
    termsConditionsUrl: string
    privacyPolicyUrl: string
    themeMode: string
    backgroundColor: string
    accentColor: string
    colorMixStrength: number
    chainIds: CustomChain[]
}

interface SerializedTransaction {
    to?: string
    value?: string
    data?: string
}

const erc20Abi = [
    'function balanceOf(address owner) view returns (uint256)',
    'function decimals() view returns (uint8)',
    'function symbol() view returns (string)'
]

const erc721Abi = [
    'function balanceOf(address owner) view returns (uint256)',
    'function ownerOf(uint256 tokenId) view returns (address)',
    'function tokenOfOwnerByIndex(address owner, uint256 index) view returns (uint256)',
    'event Transfer(address indexed from, address indexed to, uint256 indexed tokenId)'
]

const supportedNetworks = new Map<number, AppKitNetwork>([
    [mainnet.id, mainnet],
    [polygon.id, polygon],
    [arbitrum.id, arbitrum],
    [optimism.id, optimism],
    [bsc.id, bsc],
    [avalanche.id, avalanche]
])

let configured = false
let appKit: AppKit | undefined
let configuredChains = new Map<number, { network: AppKitNetwork, rpcUrl?: string }>()
let unsubscribeAccount: (() => void) | undefined
let unsubscribeNetwork: (() => void) | undefined
let previousAccount: WalletAccount | undefined
let previousChainId: number | undefined

export async function configure(options: string, dotNetInterop: DotNetInterop): Promise<void> {
    if (configured) {
        return
    }

    const configuration = JSON.parse(options) as WalletConnectConfiguration
    const networks = configuration.chainIds.map(({ chainId }) => getSupportedNetwork(chainId))

    if (networks.length === 0) {
        throw new Error('At least one supported chain must be configured.')
    }

    configuredChains = new Map(configuration.chainIds.map(chain => [
        chain.chainId,
        {
            network: getSupportedNetwork(chain.chainId),
            rpcUrl: chain.rpcUrl?.trim() || undefined
        }
    ]))

    const customRpcUrls = Object.fromEntries(
        configuration.chainIds
            .filter(chain => Boolean(chain.rpcUrl?.trim()))
            .map(chain => [`eip155:${chain.chainId}`, [{ url: chain.rpcUrl!.trim() }]])
    )

    const appKitOptions: CreateAppKit = {
        adapters: [new EthersAdapter()],
        networks: networks as [AppKitNetwork, ...AppKitNetwork[]],
        projectId: configuration.projectId,
        metadata: {
            name: configuration.name,
            description: configuration.description,
            url: configuration.url,
            icons: ['https://avatars.githubusercontent.com/u/37784886']
        },
        features: {
            analytics: true,
            onramp: true
        },
        termsConditionsUrl: configuration.termsConditionsUrl,
        privacyPolicyUrl: configuration.privacyPolicyUrl,
        enableReconnect: true,
        enableNetworkSwitch: true,
        customRpcUrls,
        themeVariables: {
            '--apkt-color-mix': configuration.backgroundColor,
            '--apkt-color-mix-strength': configuration.colorMixStrength,
            '--apkt-accent': configuration.accentColor
        }
    }

    const themeMode = configuration.themeMode.trim().toLowerCase()
    if (themeMode !== 'auto') {
        appKitOptions.themeMode = themeMode as 'dark' | 'light'
    }

    appKit = createAppKit(appKitOptions)
    configured = true
    previousAccount = createWalletAccount()
    previousChainId = previousAccount.chainId || undefined

    unsubscribeAccount = appKit.subscribeAccount(() => {
        const currentAccount = createWalletAccount()

        if (!accountsEqual(currentAccount, previousAccount)) {
            const previous = previousAccount
            previousAccount = currentAccount
            void dotNetInterop.invokeMethodAsync(
                'OnAccountChanged',
                JSON.stringify(currentAccount),
                previous ? JSON.stringify(previous) : null)
        }
    }, 'eip155')

    unsubscribeNetwork = appKit.subscribeNetwork(({ chainId }) => {
        const currentChainId = normalizeChainId(chainId)
        if (currentChainId === undefined || currentChainId === previousChainId) {
            return
        }

        const oldChainId = previousChainId
        previousChainId = currentChainId
        void dotNetInterop.invokeMethodAsync('OnChainIdChanged', currentChainId, oldChainId)
    })
}

export async function dispose(): Promise<void> {
    unsubscribeAll(unsubscribeAccount, unsubscribeNetwork)
    unsubscribeAccount = undefined
    unsubscribeNetwork = undefined
    previousAccount = undefined
    previousChainId = undefined
    configuredChains.clear()
    appKit = undefined
    configured = false
}

export async function disconnectWallet(): Promise<void> {
    const modal = getAppKit('Attempting to disconnect before we have configured.')
    await modal.disconnect('eip155')
}

export async function getWalletAccount(): Promise<string> {
    getAppKit('Attempting to get account before we have configured.')
    return JSON.stringify(createWalletAccount())
}

export async function getWalletMainBalance(): Promise<string> {
    const account = validateAccount()
    const provider = getReadProvider(account.chainId)
    const balance = await provider.getBalance(account.address)
    const network = getConfiguredChain(account.chainId).network

    return JSON.stringify({
        decimals: network.nativeCurrency.decimals,
        symbol: network.nativeCurrency.symbol,
        value: balance.toString()
    })
}

export async function getBalanceOfErc20Token(tokenAddress: string): Promise<string> {
    const account = validateAccount()
    const contract = new Contract(tokenAddress, erc20Abi, getReadProvider(account.chainId))
    const [balance, decimals, symbol] = await Promise.all([
        contract.balanceOf(account.address) as Promise<bigint>,
        contract.decimals() as Promise<bigint>,
        contract.symbol() as Promise<string>
    ])

    return JSON.stringify({
        decimals: Number(decimals),
        symbol,
        value: balance.toString()
    })
}

export async function SendTransaction(input: string, dotNetInterop: DotNetInterop): Promise<string> {
    validateAccount()

    try {
        const signer = await getSigner()
        const parsedTransaction = JSON.parse(input) as SerializedTransaction
        const transaction: TransactionRequest = {
            to: parsedTransaction.to || undefined,
            value: parsedTransaction.value || undefined,
            data: parsedTransaction.data || undefined
        }
        const response = await signer.sendTransaction(transaction)

        void response.wait(1).then(receipt => {
            if (receipt) {
                return dotNetInterop.invokeMethodAsync(
                    'OnTransactionConfirmed',
                    serializeTransactionReceipt(receipt))
            }
            return undefined
        }).catch(() => undefined)

        return JSON.stringify(response.hash)
    }
    catch (error) {
        return serializeError(error)
    }
}

export async function SignMessage(message: string): Promise<string> {
    validateAccount()

    try {
        const signature = await (await getSigner()).signMessage(message)
        return JSON.stringify(signature)
    }
    catch (error) {
        return serializeError(error)
    }
}

export async function getBalanceOfErc721Token(contractAddress: string): Promise<string> {
    const account = validateAccount()
    const contract = new Contract(contractAddress, erc721Abi, getReadProvider(account.chainId))
    const balance = await contract.balanceOf(account.address) as bigint
    return JSON.stringify(balance, bigIntegerReplacer)
}

export async function getTokenOfOwnerByIndex(contractAddress: string, index: string | number): Promise<string> {
    const account = validateAccount()
    const contract = new Contract(contractAddress, erc721Abi, getReadProvider(account.chainId))
    const tokenId = await contract.tokenOfOwnerByIndex(account.address, BigInt(index)) as bigint
    return JSON.stringify(tokenId, bigIntegerReplacer)
}

export async function getOwnerOf(contractAddress: string, tokenId: string | number): Promise<string> {
    const account = validateAccount()
    const contract = new Contract(contractAddress, erc721Abi, getReadProvider(account.chainId))
    const owner = await contract.ownerOf(BigInt(tokenId)) as string
    return JSON.stringify(owner)
}

export async function getStakedTokens(contractAddress: string, stakeContractAddress: string): Promise<string> {
    const account = validateAccount()
    const contract = new Contract(contractAddress, erc721Abi, getReadProvider(account.chainId))
    const stakeLogs = await contract.queryFilter(
        contract.filters.Transfer(account.address, stakeContractAddress),
        0,
        'latest')
    const unstakeLogs = await contract.queryFilter(
        contract.filters.Transfer(stakeContractAddress, account.address),
        0,
        'latest')

    const stakeCounts = countTokenTransfers(stakeLogs.filter(isEventLog))
    const unstakeCounts = countTokenTransfers(unstakeLogs.filter(isEventLog))
    const result = [...stakeCounts.entries()]
        .filter(([tokenId, count]) => count > (unstakeCounts.get(tokenId) ?? 0))
        .map(([tokenId]) => BigInt(tokenId))

    return JSON.stringify(result, bigIntegerReplacer)
}

export async function switchChainId(chainId: number): Promise<void> {
    const modal = getAppKit('Attempting to switch chain before we have configured.')
    validateAccount()
    await modal.switchNetwork(getConfiguredChain(chainId).network, { throwOnFailure: true })
}

function getSupportedNetwork(chainId: number): AppKitNetwork {
    assertSupportedChainId(chainId)
    const network = supportedNetworks.get(chainId)
    if (!network) {
        throw new Error(`Network metadata is unavailable for chain ID: ${chainId}.`)
    }
    return network
}

function getConfiguredChain(chainId: number): { network: AppKitNetwork, rpcUrl?: string } {
    const chain = configuredChains.get(chainId)
    if (!chain) {
        throw new Error(`Chain ID ${chainId} is not configured.`)
    }
    return chain
}

function getAppKit(errorMessage: string): AppKit {
    if (!configured || !appKit) {
        throw new Error(errorMessage)
    }
    return appKit
}

function createWalletAccount(): WalletAccount {
    const modal = getAppKit('Attempting to get account before we have configured.')
    const account = modal.getAccount('eip155')
    const status = account?.status ?? 'disconnected'
    const address = account?.address
    const addresses = account?.allAccounts
        .filter(item => item.namespace === 'eip155')
        .map(item => item.address) ?? (address ? [address] : [])

    return {
        address,
        addresses,
        isConnected: status === 'connected',
        isConnecting: status === 'connecting',
        isDisconnected: status === 'disconnected',
        isReconnecting: status === 'reconnecting',
        status,
        chainId: normalizeChainId(modal.getChainId()) ?? 0
    }
}

function validateAccount(): WalletAccount & { address: string } {
    const account = createWalletAccount()
    if (!account.isConnected || !account.address) {
        throw new Error('No wallet account is connected.')
    }
    if (!account.chainId) {
        throw new Error('The connected wallet did not provide a chain ID.')
    }
    getConfiguredChain(account.chainId)
    return account as WalletAccount & { address: string }
}

async function getSigner() {
    const modal = getAppKit('Attempting to access a wallet provider before we have configured.')
    const walletProvider = modal.getWalletProvider()
    if (!walletProvider) {
        throw new Error('No EVM wallet provider is available.')
    }

    const provider = new BrowserProvider(walletProvider as Eip1193Provider)
    return provider.getSigner()
}

function getReadProvider(chainId: number): Provider {
    const chain = getConfiguredChain(chainId)
    const defaultRpcUrl = chain.network.rpcUrls.default.http[0]
    return new JsonRpcProvider(chain.rpcUrl ?? defaultRpcUrl, chainId, { staticNetwork: true })
}

function isEventLog(log: EventLog | unknown): log is EventLog {
    return typeof log === 'object' && log !== null && 'args' in log
}

function countTokenTransfers(logs: EventLog[]): Map<string, number> {
    const counts = new Map<string, number>()
    for (const log of logs) {
        const tokenId = (log.args.tokenId as bigint).toString()
        counts.set(tokenId, (counts.get(tokenId) ?? 0) + 1)
    }
    return counts
}
