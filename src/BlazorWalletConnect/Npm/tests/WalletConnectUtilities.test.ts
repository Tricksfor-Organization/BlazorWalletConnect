import assert from 'node:assert/strict'
import test from 'node:test'
import {
    accountsEqual,
    assertSupportedChainId,
    bigIntegerReplacer,
    normalizeChainId,
    serializeTransactionReceipt,
    unsubscribeAll,
    type WalletAccount
} from '../src/js/WalletConnectUtilities'

const connectedAccount: WalletAccount = {
    address: '0x1234',
    addresses: ['0x1234'],
    isConnected: true,
    isConnecting: false,
    isDisconnected: false,
    isReconnecting: false,
    status: 'connected',
    chainId: 43114
}

test('accepts Avalanche and rejects unsupported chains', () => {
    assert.doesNotThrow(() => assertSupportedChainId(43114))
    assert.throws(() => assertSupportedChainId(11155111), /Unsupported chain ID: 11155111/)
})

test('normalizes numeric and CAIP EVM chain IDs', () => {
    assert.equal(normalizeChainId(43114), 43114)
    assert.equal(normalizeChainId('eip155:43114'), 43114)
    assert.equal(normalizeChainId('invalid'), undefined)
})

test('detects provider account and chain state changes', () => {
    assert.equal(accountsEqual(connectedAccount, { ...connectedAccount }), true)
    assert.equal(accountsEqual({ ...connectedAccount, chainId: 1 }, connectedAccount), false)
    assert.equal(accountsEqual({ ...connectedAccount, address: '0xabcd' }, connectedAccount), false)
})

test('releases each provider subscription', () => {
    let releases = 0
    unsubscribeAll(() => releases++, undefined, () => releases++)
    assert.equal(releases, 2)
})

test('serializes Ethers bigint values as JSON strings', () => {
    const json = JSON.stringify({ value: 12345678901234567890n }, bigIntegerReplacer)
    assert.equal(json, '{"value":"12345678901234567890"}')
})

test('normalizes an Ethers receipt to the Nethereum JSON-RPC wire shape', () => {
    const json = serializeTransactionReceipt({
        to: '0x2222',
        from: '0x1111',
        contractAddress: null,
        hash: '0xreceipt',
        index: 2,
        blockHash: '0xblock',
        blockNumber: 42,
        logsBloom: '0xbloom',
        logs: [{
            address: '0x3333',
            topics: ['0xtopic'],
            data: '0x',
            blockNumber: 42,
            transactionHash: '0xreceipt',
            transactionIndex: 2,
            blockHash: '0xblock',
            index: 3,
            removed: false
        }],
        gasUsed: 21000n,
        blobGasUsed: null,
        cumulativeGasUsed: 42000n,
        gasPrice: 1000000000n,
        blobGasPrice: null,
        type: 2,
        status: 1,
        root: null
    })
    const receipt = JSON.parse(json)

    assert.equal(receipt.transactionHash, '0xreceipt')
    assert.equal(receipt.hash, undefined)
    assert.equal(receipt.blockNumber, '0x2a')
    assert.equal(receipt.status, '0x1')
    assert.equal(receipt.logs[0].logIndex, '0x3')
})
