"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.configure = configure;
exports.disconnectWallet = disconnectWallet;
exports.getWalletAccount = getWalletAccount;
exports.getWalletMainBalance = getWalletMainBalance;
exports.getBalanceOfErc20Token = getBalanceOfErc20Token;
exports.SendTransaction = SendTransaction;
exports.SignMessage = SignMessage;
exports.getBalanceOfErc721Token = getBalanceOfErc721Token;
exports.getTokenOfOwnerByIndex = getTokenOfOwnerByIndex;
exports.getOwnerOf = getOwnerOf;
exports.getStakedTokens = getStakedTokens;
exports.switchChainId = switchChainId;
const wagmi_1 = require("@web3modal/wagmi");
const chains_1 = require("viem/chains");
const core_1 = require("@wagmi/core");
const viem_1 = require("viem");
let configured = false;
let walletConfig;
let account;
let clientChainIds;
function configure(options, dotNetInterop) {
    return __awaiter(this, void 0, void 0, function* () {
        if (configured) {
            return;
        }
        const { projectId, name, description, url, termsConditionsUrl, privacyPolicyUrl, themeMode, backgroundColor, accentColor, chainIds } = JSON.parse(options);
        // Create wagmiConfig
        const metadata = {
            name,
            description,
            url, // origin must match your domain & subdomain
            icons: ['https://avatars.githubusercontent.com/u/37784886']
        };
        const chains = chainIds.map((s) => {
            if (s.chainId === chains_1.mainnet.id)
                return chains_1.mainnet;
            else if (s.chainId === chains_1.polygon.id)
                return chains_1.polygon;
            else if (s.chainId === chains_1.arbitrum.id)
                return chains_1.arbitrum;
            else if (s.chainId === chains_1.optimism.id)
                return chains_1.optimism;
            else if (s.chainId === chains_1.bsc.id)
                return chains_1.bsc;
            else
                throw new Error('ChainId not found.');
        });
        clientChainIds = chainIds.map((item) => ({
            chainId: item.chainId,
            rpcUrl: item.rpcUrl
        }));
        const config = (0, wagmi_1.defaultWagmiConfig)({
            chains: chains,
            projectId,
            metadata
        });
        walletConfig = config;
        (0, core_1.reconnect)(config);
        // Create modal
        (0, wagmi_1.createWeb3Modal)({
            wagmiConfig: config,
            projectId,
            enableAnalytics: true, // Optional - defaults to your Cloud configuration
            enableOnramp: true, // Optional - false as default
            termsConditionsUrl,
            defaultChain: chains[0],
            privacyPolicyUrl,
            themeMode,
            themeVariables: {
                '--w3m-color-mix': backgroundColor,
                '--w3m-accent': accentColor
            }
        });
        (0, core_1.watchAccount)(walletConfig, {
            onChange: (currentAccount, prevAccount) => {
                account = (0, core_1.getAccount)(walletConfig);
                dotNetInterop.invokeMethodAsync('OnAccountChanged', JSON.stringify(currentAccount, connectorReplacer), JSON.stringify(prevAccount, connectorReplacer));
            }
        });
        (0, core_1.watchChainId)(walletConfig, {
            onChange: (currentChainId, prevChainId) => {
                account = (0, core_1.getAccount)(walletConfig);
                dotNetInterop.invokeMethodAsync('OnChainIdChanged', currentChainId, prevChainId);
            }
        });
        configured = true;
    });
}
function disconnectWallet() {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to disconnect before we have configured.');
        }
        yield (0, core_1.disconnect)(walletConfig);
    });
}
function getWalletAccount() {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to get account before we have configured.');
        }
        account = (0, core_1.getAccount)(walletConfig);
        return JSON.stringify(account, connectorReplacer);
    });
}
function getWalletMainBalance() {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to get balance before we have configured.');
        }
        yield validateAccount();
        const balance = yield (0, core_1.getBalance)(walletConfig, {
            address: account.address,
            chainId: account.chainId
        });
        // Convert BigInt to string manually before JSON.stringify
        const balanceWithStringValue = {
            decimals: balance.decimals,
            symbol: balance.symbol,
            value: balance.value.toString()
        };
        return JSON.stringify(balanceWithStringValue);
    });
}
function getBalanceOfErc20Token(tokenAddress) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to get token balance before we have configured.');
        }
        yield validateAccount();
        const balance = yield (0, core_1.getBalance)(walletConfig, {
            address: account.address,
            chainId: account.chainId,
            token: tokenAddress
        });
        // Convert BigInt to string manually before JSON.stringify
        const balanceWithStringValue = {
            decimals: balance.decimals,
            symbol: balance.symbol,
            value: balance.value.toString()
        };
        return JSON.stringify(balanceWithStringValue);
    });
}
function SendTransaction(input, dotNetInterop) {
    return __awaiter(this, void 0, void 0, function* () {
        var _a;
        if (!configured) {
            throw new Error('Attempting to send transaction before we have configured.');
        }
        yield validateAccount();
        try {
            const parsedTransaction = JSON.parse(input);
            delete parsedTransaction.gas;
            const preparedTransaction = yield (0, core_1.prepareTransactionRequest)(walletConfig, {
                to: parsedTransaction.to,
                value: parsedTransaction.value,
                chainId: account.chainId,
                data: parsedTransaction.data
            });
            const transactionHash = yield (0, core_1.sendTransaction)(walletConfig, {
                to: ((_a = preparedTransaction.to) !== null && _a !== void 0 ? _a : parsedTransaction.to),
                value: preparedTransaction.value,
                gas: preparedTransaction.gas,
                chainId: account.chainId,
                data: preparedTransaction.data
            });
            setTimeout(() => __awaiter(this, void 0, void 0, function* () {
                try {
                    const transactionReceipt = yield (0, core_1.waitForTransactionReceipt)(walletConfig, {
                        confirmations: 1,
                        hash: transactionHash,
                        chainId: account.chainId
                    });
                    dotNetInterop.invokeMethodAsync('OnTransactionConfirmed', JSON.stringify(transactionReceipt, transactionReceiptReplacer));
                }
                catch (e) {
                    const error = e;
                    if (error.name === 'TimeoutError')
                        return JSON.stringify(error.details);
                    return error.message;
                }
            }), 0);
            return JSON.stringify(transactionHash);
        }
        catch (e) {
            const error = e;
            if (error.name === 'TransactionExecutionError') {
                return JSON.stringify(error.details);
            }
            if (error.name === 'ConnectorAccountNotFoundError')
                return JSON.stringify(error.message);
            if (error.name === 'ConnectorNotConnectedError')
                return JSON.stringify(error.message);
            if (error.name === 'WagmiCoreError')
                return JSON.stringify(error.message);
            if (error.name === 'Error')
                return JSON.stringify(error.message);
            return JSON.stringify(error);
        }
    });
}
function SignMessage(message) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to sign message before we have configured.');
        }
        yield validateAccount();
        try {
            const result = yield (0, core_1.signMessage)(walletConfig, {
                message: message,
                account: account.address
            });
            return JSON.stringify(result);
        }
        catch (e) {
            const error = e;
            if (error.name === 'Error')
                return JSON.stringify(error.message);
            if (error.name === 'TimeoutError')
                return JSON.stringify(error.details);
            return JSON.stringify(error.message);
        }
    });
}
function getBalanceOfErc721Token(contractAddress) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to get NFT balance before we have configured.');
        }
        yield validateAccount();
        const balance = yield (0, core_1.readContract)(walletConfig, {
            address: contractAddress,
            chainId: account.chainId,
            functionName: 'balanceOf',
            abi: viem_1.erc721Abi,
            args: [account.address]
        });
        return JSON.stringify(balance, bigIntegerReplacer);
    });
}
function getTokenOfOwnerByIndex(contractAddress, index) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to get token by index before we have configured.');
        }
        yield validateAccount();
        const tokenId = yield (0, core_1.readContract)(walletConfig, {
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
            args: [account.address, index]
        });
        return JSON.stringify(tokenId, bigIntegerReplacer);
    });
}
function getOwnerOf(contractAddress, tokenId) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to get owner before we have configured.');
        }
        yield validateAccount();
        const owner = yield (0, core_1.readContract)(walletConfig, {
            address: contractAddress,
            chainId: account.chainId,
            functionName: 'ownerOf',
            abi: viem_1.erc721Abi,
            args: [tokenId]
        });
        return JSON.stringify(owner);
    });
}
function getStakedTokens(contractAddress, stakeContractAddress) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to get staked tokens before we have configured.');
        }
        yield validateAccount();
        const selectedChain = clientChainIds.find(exp => exp.chainId === account.chainId);
        const publicClient = (0, viem_1.createPublicClient)({
            chain: account.chain,
            transport: selectedChain === null || selectedChain === undefined ? (0, viem_1.http)() : (0, viem_1.http)(selectedChain.rpcUrl, {
                timeout: 20000
            }),
            batch: {
                multicall: true
            }
        });
        const stakeLogs = yield publicClient.getLogs({
            address: contractAddress,
            event: viem_1.erc721Abi[2],
            args: {
                from: account.address,
                to: stakeContractAddress,
                tokenId: null
            },
            strict: true,
            fromBlock: 'earliest',
            toBlock: 'latest'
        });
        const unStakeLogs = yield publicClient.getLogs({
            address: contractAddress,
            event: viem_1.erc721Abi[2],
            args: {
                from: stakeContractAddress,
                to: account.address,
                tokenId: null
            },
            strict: true,
            fromBlock: 'earliest',
            toBlock: 'latest'
        });
        if (stakeLogs === null || stakeLogs === undefined)
            return null;
        const distinctTokenIds = [];
        for (const item of stakeLogs) {
            if (!distinctTokenIds.includes(item.args.tokenId))
                distinctTokenIds.push(item.args.tokenId);
        }
        const result = [];
        for (const item of distinctTokenIds) {
            const stakes = stakeLogs.filter(exp => exp.args.tokenId === item).length;
            const unstakes = unStakeLogs.filter(exp => exp.args.tokenId === item).length;
            if (stakes > unstakes)
                result.push(item);
        }
        return JSON.stringify(result, bigIntegerReplacer);
    });
}
function switchChainId(chainId) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!configured) {
            throw new Error('Attempting to switch chain before we have configured.');
        }
        yield validateAccount();
        (0, core_1.switchChain)(walletConfig, {
            chainId: chainId
        });
    });
}
function connectorReplacer(key, value) {
    if (key === 'connector') {
        return undefined;
    }
    return value;
}
function bigIntegerReplacer(key, value) {
    if (typeof value === 'bigint') {
        return value.toString();
    }
    return value;
}
function transactionReceiptReplacer(key, value) {
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
function validateAccount() {
    return __awaiter(this, void 0, void 0, function* () {
        if ((account === null || account === void 0 ? void 0 : account.address) === undefined)
            account = (0, core_1.getAccount)(walletConfig);
    });
}
//# sourceMappingURL=main.js.map