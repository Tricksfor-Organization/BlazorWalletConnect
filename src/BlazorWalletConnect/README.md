# Tricksfor.BlazorWalletConnect

A modern Blazor WebAssembly library for integrating WalletConnect into your Web3 applications.

## Features

- 🔗 **Multi-Chain Support** - Ethereum, Polygon, Arbitrum, Optimism, BSC, and more
- 💼 **Wallet Operations** - Connect, disconnect, and manage wallet accounts
- 💰 **Balance Queries** - Get native and ERC-20 token balances
- 📝 **Message Signing** - Sign messages and typed data (EIP-712)
- 🔄 **Transactions** - Send transactions and interact with smart contracts
- 🎨 **NFT Operations** - Transfer ERC-721 and ERC-1155 tokens
- 🌐 **Network Switching** - Switch between different blockchain networks
- ⚡ **Event Handling** - Real-time events for account and network changes

## Installation

```bash
dotnet add package Tricksfor.BlazorWalletConnect
```

### Version Compatibility

Choose the package version that matches your .NET version:

- **.NET 9**: Use version `9.x`
  ```bash
  dotnet add package Tricksfor.BlazorWalletConnect --version 9.*
  ```

- **.NET 10**: Use version `10.x`
  ```bash
  dotnet add package Tricksfor.BlazorWalletConnect --version 10.*
  ```

## Quick Start

### 1. Get WalletConnect Project ID

Visit [WalletConnect Cloud](https://cloud.walletconnect.com/) and create a free project to get your Project ID.

### 2. Configure Services

```csharp
// Program.cs
builder.Services.AddWalletConnect(options =>
{
    options.ProjectId = "YOUR_PROJECT_ID";
    options.Metadata = new AppMetadata
    {
        Name = "My Blazor dApp",
        Description = "Web3 application built with Blazor",
        Url = "https://myapp.com",
        Icons = new[] { "https://myapp.com/icon.png" }
    };
});
```

### 3. Use the Component

```razor
@page "/"
@inject IWalletConnectInterop WalletConnect

<WalletConnectButton />

@if (account?.IsConnected == true)
{
    <p>Connected: @account.Address</p>
    <p>Balance: @balance ETH</p>
}

@code {
    private AccountDto? account;
    private string? balance;

    protected override async Task OnInitializedAsync()
    {
        WalletConnect.AccountChanged += OnAccountChanged;
        await WalletConnect.ConfigureAsync();
        account = await WalletConnect.GetAccountAsync();
        
        if (account?.IsConnected == true)
        {
            var balanceDto = await WalletConnect.GetBalanceAsync();
            balance = balanceDto?.Formatted;
        }
    }

    private async void OnAccountChanged(object? sender, AccountChangedEventArgs e)
    {
        account = e.currentAccount;
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        WalletConnect.AccountChanged -= OnAccountChanged;
    }
}
```

## Key APIs

### Wallet Connection
```csharp
var account = await WalletConnect.GetAccountAsync();
await WalletConnect.DisconnectAsync();
```

### Balance Queries
```csharp
var balance = await WalletConnect.GetBalanceAsync();
var tokenBalance = await WalletConnect.GetTokenBalanceAsync(tokenAddress);
```

### Transactions
```csharp
var txHash = await WalletConnect.SendTransactionAsync(new TransactionInput
{
    To = recipientAddress,
    Value = Web3.Convert.ToWei(0.1m).ToString()
});
```

### Smart Contracts
```csharp
var txHash = await WalletConnect.SendTransactionAsync(
    contractAddress, 
    functionName, 
    parameters
);
```

### Message Signing
```csharp
var signature = await WalletConnect.SignMessageAsync("Hello Web3!");
```

### Network Management
```csharp
await WalletConnect.SwitchChainAsync(chainId);
```

## Events

Subscribe to real-time events:

```csharp
WalletConnect.AccountChanged += (sender, e) => {
    // Handle account change
};

WalletConnect.ChainIdChanged += (sender, e) => {
    // Handle network change
};
```

## Requirements

- .NET 9.*.* or later
- Blazor WebAssembly
- WalletConnect Project ID

## Documentation

For comprehensive documentation, examples, and advanced features, visit the [GitHub repository](https://github.com/Tricksfor-Organization/BlazorWalletConnect).

## Support

- 📫 Report issues: [GitHub Issues](https://github.com/Tricksfor-Organization/BlazorWalletConnect/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/Tricksfor-Organization/BlazorWalletConnect/discussions)

## License

MIT License - see [LICENSE](https://github.com/Tricksfor-Organization/BlazorWalletConnect/blob/main/LICENSE) for details.

---

Built with ❤️ by Tricksfor Organization
