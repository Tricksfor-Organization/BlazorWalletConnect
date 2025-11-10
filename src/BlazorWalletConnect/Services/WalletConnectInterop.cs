using BlazorWalletConnect.Events;
using BlazorWalletConnect.Models;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using System.Text.Json;

namespace BlazorWalletConnect.Services
{
    public class WalletConnectInterop : IWalletConnectInterop
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly WalletConnectOptions _options;
        private readonly DotNetObjectReference<WalletConnectInterop> _jsRef;
        private bool _configured;
        private bool _disposed;
        private IJSObjectReference? _module;

        public WalletConnectInterop(IJSRuntime jsRuntime, IOptions<WalletConnectOptions> options)
        {
            _options = options.Value;
            _jsRuntime = jsRuntime;
            _jsRef = DotNetObjectReference.Create(this);
        }

        public async Task ConfigureAsync()
        {
            if (!_configured)
            {
                var module = await GetModuleAsync();

                await module.InvokeVoidAsync("configure", JsonSerializer.Serialize(_options), _jsRef);
                _configured = true;
            }
        }

        public async Task DisconnectAsync()
        {
            var module = await EnsureConfiguredAsync();
            await module.InvokeVoidAsync("disconnectWallet");
        }

        public async Task<AccountDto?> GetAccountAsync()
        {
            var module = await EnsureConfiguredAsync();
            var stringResult = await module.InvokeAsync<string>("getWalletAccount");
            return JsonSerializer.Deserialize<AccountDto>(stringResult);
        }

        public async Task<BalanceDto?> GetBalanceAsync()
        {
            var module = await EnsureConfiguredAsync();
            var stringResult = await module.InvokeAsync<string>("getWalletMainBalance");
            var internalDto = JsonSerializer.Deserialize<WalletConnectBalanceDto>(stringResult);
            
            if (internalDto is null)
                return null;

            return new BalanceDto(
                internalDto.Decimals,
                internalDto.Symbol,
                BigInteger.Parse(internalDto.Value)
            );
        }

        public async Task<BalanceDto?> GetBalanceAsync(string erc20TokenAddress)
        {
            var module = await EnsureConfiguredAsync();
            var stringResult = await module.InvokeAsync<string>("getBalanceOfErc20Token", erc20TokenAddress);
            var internalDto = JsonSerializer.Deserialize<WalletConnectBalanceDto>(stringResult);
            
            if (internalDto is null)
                return null;

            return new BalanceDto(
                internalDto.Decimals,
                internalDto.Symbol,
                BigInteger.Parse(internalDto.Value)
            );
        }

        public async Task<BigInteger?> GetBalanceOfAsync(string erc721ContractAddress)
        {
            var module = await EnsureConfiguredAsync();
            var stringResult = await module.InvokeAsync<string>("getBalanceOfErc721Token", erc721ContractAddress);
            var deserializedString = JsonSerializer.Deserialize<string>(stringResult);
            return deserializedString != null ? BigInteger.Parse(deserializedString) : null;
        }

        public async Task<BigInteger?> GetTokenOfOwnerByIndexAsync(string erc721ContractAddress, BigInteger index)
        {
            var module = await EnsureConfiguredAsync();
            var stringResult = await module.InvokeAsync<string>("getTokenOfOwnerByIndex", erc721ContractAddress, (long)index);
            var deserializedString = JsonSerializer.Deserialize<string>(stringResult);
            return deserializedString != null ? BigInteger.Parse(deserializedString) : null;
        }

        public async Task<List<BigInteger>?> GetStakedTokensAsync(string erc721ContractAddress, string erc721StakeContractAddress)
        {
            var module = await EnsureConfiguredAsync();
            var stringResult = await module.InvokeAsync<string>("getStakedTokens", erc721ContractAddress, erc721StakeContractAddress);
            var deserializedStrings = JsonSerializer.Deserialize<List<string>>(stringResult);
            return deserializedStrings?.Select(s => BigInteger.Parse(s)).ToList();
        }

        public async Task<string> SendTransactionAsync(TransactionInput transactionInput)
        {
            var module = await EnsureConfiguredAsync();
            var stringResult = await module.InvokeAsync<string>("SendTransaction", JsonSerializer.Serialize(transactionInput), _jsRef);
            try
            {
                return JsonSerializer.Deserialize<string>(stringResult) ?? throw new InvalidOperationException("Transaction returned null result");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize transaction result: {stringResult}", ex);
            }
        }

        [JSInvokable]
        public Task OnTransactionConfirmed(string? transactionReceipt)
        {
            if (transactionReceipt is not null)
            {
                try
                {
                    var result = JsonSerializer.Deserialize<TransactionReceipt>(transactionReceipt);
                    if (result is not null)
                    {
                        RaiseTransactionConfirmed(result);
                    }
                }
                catch (JsonException ex)
                {
                    throw new InvalidOperationException($"Failed to deserialize transaction receipt: {transactionReceipt}", ex);
                }
            }

            return Task.CompletedTask;
        }

        public event EventHandler<TransactionConfirmedEventArgs>? TransactionConfirmed;

        private void RaiseTransactionConfirmed(TransactionReceipt transactionReceipt)
        {
            var e = new TransactionConfirmedEventArgs { TransactionReceipt = transactionReceipt };
            TransactionConfirmed?.Invoke(this, e);
        }

        [JSInvokable]
        public Task OnAccountChanged(string? currentAccount, string? prevAccount)
        {
            var current = currentAccount is not null ? JsonSerializer.Deserialize<AccountDto>(currentAccount) : null;
            var previous = prevAccount is not null ? JsonSerializer.Deserialize<AccountDto>(prevAccount) : null;
            RaiseAccountChanged(current, previous);

            return Task.CompletedTask;
        }

        public event EventHandler<AccountChangedEventArgs>? AccountChanged;

        private void RaiseAccountChanged(AccountDto? currentAccount, AccountDto? prevAccount)
        {
            var e = new AccountChangedEventArgs
            {
                currentAccount = currentAccount,
                prevAccount = prevAccount
            };
            AccountChanged?.Invoke(this, e);
        }

        [JSInvokable]
        public Task OnChainIdChanged(int? currentChainId, int? prevChainId)
        {
            RaiseChainIdChanged(currentChainId, prevChainId);

            return Task.CompletedTask;
        }

        public event EventHandler<ChainIdChangedEventArgs>? ChainIdChanged;

        private void RaiseChainIdChanged(int? currentChainId, int? prevChainId)
        {
            var e = new ChainIdChangedEventArgs
            {
                currentChainId = currentChainId,
                prevChainId = prevChainId
            };
            ChainIdChanged?.Invoke(this, e);
        }

        public async Task<string> SignMessageAsync(string message)
        {
            var module = await EnsureConfiguredAsync();
            var stringResult = await module.InvokeAsync<string>("SignMessage", message);
            try
            {
                return JsonSerializer.Deserialize<string>(stringResult) ?? throw new InvalidOperationException("Signature returned null result");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize signature result: {stringResult}", ex);
            }
        }

        public async Task SwitchChainIdAsync(int chainId)
        {
            var module = await EnsureConfiguredAsync();
            await module.InvokeVoidAsync("switchChainId", chainId);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);

            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_module is not null)
            {
                await _module.DisposeAsync().ConfigureAwait(false);
            }

            _jsRef?.Dispose();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _jsRef?.Dispose();
            }

            _disposed = true;
        }

        #region Methods
        private async ValueTask<IJSObjectReference> EnsureConfiguredAsync()
        {
            if (!_configured)
            {
                await ConfigureAsync();
            }
            return await GetModuleAsync();
        }

        private async Task<IJSObjectReference> GetModuleAsync()
        {
            if (_module is null)
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>("import",
                    "./_content/BlazorWalletConnect/main.bundle.js");
            return _module;
        }
        #endregion
    }
}
