import { Web3Modal, createWeb3Modal, defaultWagmiConfig } from '@web3modal/wagmi'
import { polygon, mainnet, arbitrum, optimism, bsc, Chain } from 'viem/chains'
import {
    reconnect, disconnect, Config, getAccount, getBalance, GetAccountReturnType,
    sendTransaction, SendTransactionErrorType, SendTransactionParameters, SendTransactionReturnType,
    waitForTransactionReceipt, WaitForTransactionReceiptReturnType, WaitForTransactionReceiptErrorType,
    prepareTransactionRequest, type PrepareTransactionRequestReturnType, signMessage, SignMessageErrorType,
    watchAccount, watchChainId, switchChain,
    readContract, ReadContractReturnType
} from '@wagmi/core'
import { SignableMessage, erc721Abi, createPublicClient, http } from 'viem'

let modal: Web3Modal
let configured = false
let walletConfig: Config
let account: GetAccountReturnType
interface CustomChain {
    chainId: number,
    rpcUrl: string|null
}
let clientChainIds: [CustomChain]

export async function configure(options: any, dotNetInterop: any) {
    if (configured) {
        return;
    }
    let { projectId, name, description, url, termsConditionsUrl, privacyPolicyUrl, themeMode, backgroundColor, accentColor,
        enableEmail, chainIds } = JSON.parse(options);

    // 2. Create wagmiConfig
    const metadata = {
        name: name,
        description: description,
        url: url, // origin must match your domain & subdomain.
        icons: ['https://avatars.githubusercontent.com/u/37784886']
    }

    let chains: [Chain] = chainIds.map((s: CustomChain) => { 
        if (s.chainId === mainnet.id) return mainnet
        else if (s.chainId === polygon.id) return polygon
        else if (s.chainId === arbitrum.id) return arbitrum
        else if (s.chainId === optimism.id) return optimism
        else if (s.chainId === bsc.id) return bsc
        else throw new Error('ChainId not found.');
    })

    for (const item of chainIds) {
        if (clientChainIds === undefined)
            clientChainIds = [ { chainId:  item.chainId, rpcUrl: item.rpcUrl }]
        else
            clientChainIds.push(item)
    }

    const config = defaultWagmiConfig({
        chains,
        projectId,
        metadata,
        enableEmail: enableEmail
        //...wagmiOptions // Optional - Override createConfig parameters
    })

    walletConfig = config

    reconnect(config)

    // 3. Create modal
    modal = createWeb3Modal({
        wagmiConfig: config,
        projectId,
        enableAnalytics: true, // Optional - defaults to your Cloud configuration
        enableOnramp: true, // Optional - false as default
        termsConditionsUrl: termsConditionsUrl,
        defaultChain: chains[0],
        privacyPolicyUrl: privacyPolicyUrl,
        themeMode: themeMode,
        themeVariables: {
            '--w3m-color-mix': backgroundColor,
            '--w3m-accent': accentColor
        }
    })

    watchAccount(walletConfig, {
        onChange: (currenctAccount, prevAccount) => {
            account = getAccount(walletConfig)
            dotNetInterop.invokeMethodAsync('OnAccountChanged', JSON.stringify(currenctAccount, connectorReplacer), JSON.stringify(prevAccount, connectorReplacer));
        }
    })

    watchChainId(walletConfig, {
        onChange: (currenctChainId, prevChainId) => {
            account = getAccount(walletConfig)
            dotNetInterop.invokeMethodAsync('OnChainIdChanged', currenctChainId, prevChainId);
        }
    })

    configured = true;
}

export async function disconnectWallet() {
    if (!configured) {
        throw new Error("Attempting to disconnect before we have configured.");
    }
    await disconnect(walletConfig)
}

export async function getWalletAccount() {
    if (!configured) {
        throw new Error("Attempting to disconnect before we have configured.");
    }
    account = getAccount(walletConfig)
    return JSON.stringify(account, connectorReplacer)
}

export async function getWalletMainBalance() {
    if (!configured) {
        throw new Error("Attempting to disconnect before we have configured.");
    }

    await validateAccount()

    let balance = await getBalance(walletConfig, {
        address: account.address!,
        chainId: account.chainId
    })
    return JSON.stringify(balance, bigIntegerReplacer)
}

export async function getBalanceOfErc20Token(tokenAddress: '0x${string}') {
    if (!configured) {
        throw new Error("Attempting to disconnect before we have configured.");
    }

    await validateAccount()

    let balance = await getBalance(walletConfig, {
        address: account.address!,
        chainId: account.chainId,
        token: tokenAddress
    })
    return JSON.stringify(balance, bigIntegerReplacer)
}

export async function SendTransaction(input: string, dotNetInterop: any) {
    if (!configured) {
        throw new Error("Attempting to send transaction before we have configured.");
    }

    await validateAccount()

    try {
        const parsedTransaction: SendTransactionParameters = JSON.parse(input)
        delete parsedTransaction.gas

        const preparedTransaction: PrepareTransactionRequestReturnType = await prepareTransactionRequest(walletConfig, {
            to: parsedTransaction.to as `0x${string}`,
            value: parsedTransaction.value,
            chainId: account.chainId,
            data: parsedTransaction.data
        })
       
        const transactionHash: SendTransactionReturnType = await sendTransaction(walletConfig, {
            to: (preparedTransaction.to ?? parsedTransaction.to) as `0x${string}`,
            value: preparedTransaction.value,
            gas: preparedTransaction.gas,
            chainId: account.chainId,
            data: preparedTransaction.data
        });

        setTimeout(async () => {
            try {
                const transactionReciept: WaitForTransactionReceiptReturnType = await waitForTransactionReceipt(walletConfig, {
                    confirmations: 1,
                    hash: transactionHash,
                    chainId: account.chainId
                })

                dotNetInterop.invokeMethodAsync("OnTransactionConfirmed", JSON.stringify(transactionReciept, transactionRecieptReplacer));
            }
            catch (e) {
                const error = e as WaitForTransactionReceiptErrorType

                if (error.name === 'TimeoutError')
                    return JSON.stringify(error.details)
                return error.message
            }
        }, 0);

        return JSON.stringify(transactionHash) 
    }
    catch (e) {
        const error = e as SendTransactionErrorType

        if (error.name === 'TransactionExecutionError') {
            return JSON.stringify(error.details)
        }
        if (error.name === 'ConnectorAccountNotFoundError')
            return JSON.stringify(error.message)
        if (error.name === 'ConnectorNotConnectedError')
            return JSON.stringify(error.message)
        if (error.name === 'WagmiCoreError')
            return JSON.stringify(error.message)
        if (error.name === 'Error')
            return JSON.stringify(error.message)
        
        return JSON.stringify(error)
    }
}

export async function SignMessage(message: SignableMessage) {
    if (!configured) {
        throw new Error("Attempting to sign message before we have configured.");
    }

    await validateAccount()

    try {
        const result = await signMessage(walletConfig, {
            message: message,
            account: account.address
        });

        return JSON.stringify(result);
    }
    catch (e) {
        const error = e as SignMessageErrorType
        if (error.name === 'Error')
            return JSON.stringify(error.message)
        if (error.name === 'TimeoutError')
            return JSON.stringify(error.details)
        return JSON.stringify(error.message)
    }
}

export async function getBalanceOfErc721Token(contractAddress: '0x${string}') {
    if (!configured) {
        throw new Error("Attempting to disconnect before we have configured.");
    }

    await validateAccount()

    const balance: ReadContractReturnType = await readContract(walletConfig, {
        address: contractAddress,
        chainId: account.chainId,
        functionName: 'balanceOf',
        abi: erc721Abi,
        args: [account.address!]
    })
    return JSON.stringify(balance, bigIntegerReplacer)
}

export async function getTokenOfOwnerByIndex(contractAddress: '0x${string}', index: bigint) {
    if (!configured) {
        throw new Error("Attempting to disconnect before we have configured.");
    }

    await validateAccount()

    const tokenId: ReadContractReturnType = await readContract(walletConfig, {
        address: contractAddress,
        chainId: account.chainId,
        functionName: 'tokenOfOwnerByIndex',
        abi: [
            {
                "inputs": [
                    {
                        "internalType": "address",
                        "name": "owner",
                        "type": "address"
                    },
                    {
                        "internalType": "uint256",
                        "name": "index",
                        "type": "uint256"
                    }
                ],
                "name": "tokenOfOwnerByIndex",
                "outputs": [
                    {
                        "internalType": "uint256",
                        "name": "",
                        "type": "uint256"
                    }
                ],
                "stateMutability": "view",
                "type": "function"
            }
        ],
        //functionName: 'tokenByIndex',
        //abi: erc721Abi,
        args: [account.address!, index]
    })
    return JSON.stringify(tokenId, bigIntegerReplacer)
}

export async function getOwnerOf(contractAddress: '0x${string}', tokenId: bigint) {
    if (!configured) {
        throw new Error("Attempting to disconnect before we have configured.");
    }

    await validateAccount()

    const owner: ReadContractReturnType = await readContract(walletConfig, {
        address: contractAddress,
        chainId: account.chainId,
        functionName: 'ownerOf',
        abi: erc721Abi,
        args: [tokenId]
    })
    return JSON.stringify(owner)
}

export async function getStakedTokens(contractAddress: '0x${string}', stakeContractAddress: '0x${string}') {
    if (!configured) {
        throw new Error("Attempting to disconnect before we have configured.");
    }
    
    await validateAccount()

    const selectedChain = clientChainIds.find(exp => exp.chainId === account.chainId)

    const publicClient = createPublicClient({
        chain: account.chain,
        transport: selectedChain === null ? http() : http(selectedChain?.rpcUrl!, {
            timeout: 20000
        }),
        batch: {
            multicall: true
        }
    })

    const stakeLogs = await publicClient.getLogs({
        address: contractAddress,
        event: erc721Abi[2],
        args: {
            from: account.address,
            to: stakeContractAddress,
            tokenId: null
        },
        strict: true,
        fromBlock: 'earliest',
        toBlock: 'latest'
    })

    const unStakeLogs = await publicClient.getLogs({
        address: contractAddress,
        event: erc721Abi[2],
        args: {
            from: stakeContractAddress,
            to: account.address,
            tokenId: null
        },
        strict: true,
        fromBlock: 'earliest',
        toBlock: 'latest'
    })

    if (stakeLogs == null || undefined)
        return null

    let distinctTokenIds: Array<bigint> = [];
    for (const item of stakeLogs) {
        if (!distinctTokenIds.includes(item.args.tokenId))
            distinctTokenIds.push(item.args.tokenId)
    }

    let result: Array<bigint> = []

    for (const item of distinctTokenIds) {
        const stakes = stakeLogs.filter(exp => exp.args.tokenId === item).length
        const unstakes = unStakeLogs.filter(exp => exp.args.tokenId === item).length
        if (stakes > unstakes)
            result.push(item)
    }
    
    return JSON.stringify(result, bigIntegerReplacer)
}

export async function switchChainId(chainId: number) {
    if (!configured) {
        throw new Error("Attempting to sign message before we have configured.");
    }

    await validateAccount()

    switchChain(walletConfig, {
        chainId: chainId
    })
}


function connectorReplacer(key:string, value:string) {
    if (key == "connector") {
        return undefined;
    }
    return value;
}

function bigIntegerReplacer(key: string, value: any) {
    if (typeof value === 'bigint') {
        return value.toString();
    }
    return value;
}

function transactionRecieptReplacer(key: string, value: any) {
    if (key === 'status') {
        if (value === 'success')
            return Number(1);
        if (value === 'reverted')
            return Number(0);
        return Number(0);
    }
    if (typeof value === 'bigint') {
        return value.toString();
    }
    return value;
}

async function validateAccount() {
    if (account?.address == undefined)
        account = getAccount(walletConfig)
}


function getErrorResponse(e:any) {
    let response = {
        data: null,
        error: e.reason ?? e.message ?? e,
        success: false
    }
    return JSON.stringify(response);
}

function getSuccessResponse(result:any) {
    let response = {
        data: result,
        error: null,
        success: true
    };
    return JSON.stringify(response);
}