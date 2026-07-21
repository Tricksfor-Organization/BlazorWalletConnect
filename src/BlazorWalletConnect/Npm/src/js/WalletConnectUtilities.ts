export interface WalletAccount {
    address?: string
    addresses: string[]
    isConnected: boolean
    isConnecting: boolean
    isDisconnected: boolean
    isReconnecting: boolean
    status: 'connected' | 'connecting' | 'disconnected' | 'reconnecting'
    chainId: number
}

interface EthersLogLike {
    address: string
    topics: readonly string[]
    data: string
    blockNumber: number
    transactionHash: string
    transactionIndex: number
    blockHash: string
    index: number
    removed: boolean
}

interface EthersTransactionReceiptLike {
    to: string | null
    from: string
    contractAddress: string | null
    hash: string
    index: number
    blockHash: string
    blockNumber: number
    logsBloom: string
    logs: readonly EthersLogLike[]
    gasUsed: bigint
    blobGasUsed: bigint | null
    cumulativeGasUsed: bigint
    gasPrice: bigint
    blobGasPrice: bigint | null
    type: number
    status: number | null
    root?: string | null
}

const supportedChainIds = new Set([1, 56, 137, 42161, 10, 43114])

export function assertSupportedChainId(chainId: number): void {
    if (!supportedChainIds.has(chainId)) {
        throw new Error(`Unsupported chain ID: ${chainId}.`)
    }
}

export function normalizeChainId(chainId: string | number | undefined): number | undefined {
    if (typeof chainId === 'number') {
        return chainId
    }
    if (!chainId) {
        return undefined
    }

    const normalized = chainId.includes(':') ? chainId.split(':').at(-1) : chainId
    const parsed = Number(normalized)
    return Number.isSafeInteger(parsed) ? parsed : undefined
}

export function accountsEqual(current: WalletAccount, previous: WalletAccount | undefined): boolean {
    return previous !== undefined
        && current.address === previous.address
        && current.isConnected === previous.isConnected
        && current.status === previous.status
        && current.chainId === previous.chainId
        && current.addresses.join(',') === previous.addresses.join(',')
}

export function unsubscribeAll(...subscriptions: Array<(() => void) | undefined>): void {
    for (const unsubscribe of subscriptions) {
        unsubscribe?.()
    }
}

export function bigIntegerReplacer(_key: string, value: unknown): unknown {
    return typeof value === 'bigint' ? value.toString() : value
}

export function serializeTransactionReceipt(receipt: EthersTransactionReceiptLike): string {
    return JSON.stringify({
        transactionHash: receipt.hash,
        transactionIndex: toHexQuantity(receipt.index),
        blockHash: receipt.blockHash,
        blockNumber: toHexQuantity(receipt.blockNumber),
        from: receipt.from,
        to: receipt.to,
        cumulativeGasUsed: toHexQuantity(receipt.cumulativeGasUsed),
        gasUsed: toHexQuantity(receipt.gasUsed),
        effectiveGasPrice: toHexQuantity(receipt.gasPrice),
        blobGasUsed: toHexQuantity(receipt.blobGasUsed),
        blobGasPrice: toHexQuantity(receipt.blobGasPrice),
        contractAddress: receipt.contractAddress,
        logs: receipt.logs.map(log => ({
            address: log.address,
            topics: log.topics,
            data: log.data,
            blockNumber: toHexQuantity(log.blockNumber),
            transactionHash: log.transactionHash,
            transactionIndex: toHexQuantity(log.transactionIndex),
            blockHash: log.blockHash,
            logIndex: toHexQuantity(log.index),
            removed: log.removed
        })),
        logsBloom: receipt.logsBloom,
        status: toHexQuantity(receipt.status),
        root: receipt.root,
        type: toHexQuantity(receipt.type)
    })
}

export function serializeError(error: unknown): string {
    if (error instanceof Error) {
        return JSON.stringify(error.message)
    }
    return JSON.stringify(String(error))
}

function toHexQuantity(value: number | bigint | null | undefined): string | null | undefined {
    if (value === null) {
        return null
    }
    if (value === undefined) {
        return undefined
    }
    return `0x${BigInt(value).toString(16)}`
}
