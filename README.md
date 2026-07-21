# BlazorWalletConnect

[![NuGet](https://img.shields.io/nuget/v/Tricksfor.BlazorWalletConnect.svg)](https://www.nuget.org/packages/Tricksfor.BlazorWalletConnect/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Tricksfor.BlazorWalletConnect.svg)](https://www.nuget.org/packages/Tricksfor.BlazorWalletConnect/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-9_|_10-blue.svg)](https://dotnet.microsoft.com/download)

A powerful Blazor WebAssembly library for integrating WalletConnect into your Web3 applications. Reown AppKit provides the wallet UI and WalletConnect sessions, while Ethers 6 handles EVM interactions on Ethereum, Polygon, Arbitrum, Optimism, BSC, and Avalanche.

## 🌐 Live Demo

Try the live demo: **[blazorwalletconnect-demo.pages.dev](https://blazorwalletconnect-demo.pages.dev)**

The demo showcases all library features including wallet connection, balance queries, transactions, and more. Connect your wallet and explore the capabilities!

## ✨ Features

- 🔌 **Easy WalletConnect Integration** - Connect to 300+ wallets with a few lines of code
- 🌐 **Multi-Chain Support** - Ethereum, Polygon, Arbitrum, Optimism, BSC, and Avalanche
- 💼 **Wallet Operations** - Connect, disconnect, switch networks seamlessly
- 💰 **Balance Queries** - Native tokens and ERC20 token balances
- 📝 **Transaction Management** - Send transactions and track confirmations
- ✍️ **Message Signing** - Sign messages and verify signatures
- 🎨 **NFT Support** - ERC721 operations including balance, ownership, and staking
- 🔔 **Event Handling** - Real-time events for account changes, network switches, and transactions
- 🎭 **Customizable UI** - Theme colors, modes, and branding options
- 🧪 **Fully Tested** - Comprehensive unit test coverage with NUnit

## 📦 Installation

### NuGet Package

```bash
dotnet add package Tricksfor.BlazorWalletConnect
```

### Version Compatibility

**Important:** Choose the package version that matches your .NET version:

- **.NET 9**: Use version `9.x.x`
  ```xml
  <PackageReference Include="Tricksfor.BlazorWalletConnect" Version="9.*" />
  ```

- **.NET 10**: Use version `10.x.x`
  ```xml
  <PackageReference Include="Tricksfor.BlazorWalletConnect" Version="10.*" />
  ```

### Via Package Manager Console

```powershell
# For .NET 9
Install-Package Tricksfor.BlazorWalletConnect -Version 9.*

# For .NET 10
Install-Package Tricksfor.BlazorWalletConnect -Version 10.*
```

## 🚀 Quick Start

### 1. Get Your WalletConnect Project ID

Visit [WalletConnect Cloud](https://cloud.walletconnect.com/) and create a free project to get your Project ID.

### 2. Configure Services

Add BlazorWalletConnect to your `Program.cs`:

```csharp
using BlazorWalletConnect;
using Nethereum.Signer;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddBlazorWalletConnect(options =>
{
    options.ProjectId = "YOUR_PROJECT_ID"; // Required: Get from WalletConnect Cloud
    options.Name = "My DApp";
    options.Description = "My awesome Web3 application";
    options.Url = "https://myapp.com";
    options.TermsConditionsUrl = "https://myapp.com/terms";
    options.PrivacyPolicyUrl = "https://myapp.com/privacy";
    
    // UI Customization
    options.ThemeMode = "dark"; // "light", "dark", or "auto"
    options.BackgroundColor = "#000000";
    options.AccentColor = "#3b82f6";
    options.ColorMixStrength = 40; // 0-100
    
    // Supported Chains
    options.Chains = new List<ChainDto>
    {
        new ChainDto(Chain.MainNet, "https://mainnet.infura.io/v3/YOUR_KEY"),
        new ChainDto(Chain.Polygon, "https://polygon-rpc.com"),
        new ChainDto(Chain.Avalanche, "https://api.avax.network/ext/bc/C/rpc")
    };
});

await builder.Build().RunAsync();
```

### 3. Add the Import

In your `_Imports.razor`:

```razor
@using BlazorWalletConnect
@using BlazorWalletConnect.Components
@using BlazorWalletConnect.Models
@using BlazorWalletConnect.Services
```

### 4. Use the Wallet Connect Button

```razor
@page "/"
@inject IWalletConnectInterop WalletConnect

<WalletConnectButtonProvider>
    <WalletConnectButton />
</WalletConnectButtonProvider>

<button @onclick="GetBalance" disabled="@(!isConnected)">
    Get Balance
</button>

<p>Balance: @balance</p>

@code {
    private bool isConnected;
    private string? balance;

    protected override async Task OnInitializedAsync()
    {
        WalletConnect.AccountChanged += OnAccountChanged;
        await WalletConnect.ConfigureAsync();
    }

    private async void OnAccountChanged(object? sender, AccountChangedEventArgs e)
    {
        isConnected = e.currentAccount?.IsConnected ?? false;
        StateHasChanged();
    }

    private async Task GetBalance()
    {
        var balanceDto = await WalletConnect.GetBalanceAsync();
        balance = $"{balanceDto?.Formatted} {balanceDto?.Symbol}";
    }

    public void Dispose()
    {
        WalletConnect.AccountChanged -= OnAccountChanged;
    }
}
```

## 📚 Documentation

### Core Services

#### IWalletConnectInterop

The main service for interacting with wallets.

```csharp
@inject IWalletConnectInterop WalletConnect
```

### Wallet Connection

```csharp
// Configure the wallet connection
await WalletConnect.ConfigureAsync();

// Get current account information
var account = await WalletConnect.GetAccountAsync();
if (account?.IsConnected == true)
{
    Console.WriteLine($"Connected: {account.Address}");
    Console.WriteLine($"Chain ID: {account.ChainId}");
}

// Disconnect wallet
await WalletConnect.DisconnectAsync();
```

### Balance Queries

```csharp
// Get native token balance (ETH, MATIC, etc.)
var balance = await WalletConnect.GetBalanceAsync();
Console.WriteLine($"{balance.Formatted} {balance.Symbol}");

// Get ERC20 token balance
var tokenBalance = await WalletConnect.GetBalanceAsync("0xTokenAddress");
Console.WriteLine($"{tokenBalance.Formatted} {tokenBalance.Symbol}");
```

### Transactions

```csharp
using Nethereum.RPC.Eth.DTOs;

var transactionInput = new TransactionInput
{
    From = "0xYourAddress",
    To = "0xRecipientAddress",
    Value = new HexBigInteger(1000000000000000000) // 1 ETH in Wei
};

var txHash = await WalletConnect.SendTransactionAsync(transactionInput);
Console.WriteLine($"Transaction Hash: {txHash}");
```

### Smart Contract Interactions

```csharp
using BlazorWalletConnect.Helpers;
using Nethereum.Contracts;

// Define your contract function
[Function("transfer")]
public class TransferFunction : FunctionMessage
{
    [Parameter("address", "to", 1)]
    public string To { get; set; }
    
    [Parameter("uint256", "amount", 2)]
    public BigInteger Amount { get; set; }
}

// Create transaction
var transferFunction = new TransferFunction
{
    To = "0xRecipient",
    Amount = BigInteger.Parse("1000000000000000000") // 1 token
};

var transactionInput = TransactionHelper.CreateContractTransactionInput(
    transferFunction, 
    account.Address, 
    "0xContractAddress"
);

var txHash = await WalletConnect.SendTransactionAsync(transactionInput);
```

### Message Signing

```csharp
var message = "Sign this message to verify your wallet ownership";
var signature = await WalletConnect.SignMessageAsync(message);
Console.WriteLine($"Signature: {signature}");
```

### NFT Operations (ERC721)

```csharp
// Get NFT balance
var nftBalance = await WalletConnect.GetBalanceOfAsync("0xNFTContractAddress");
Console.WriteLine($"You own {nftBalance} NFTs");

// Get token by index
var tokenId = await WalletConnect.GetTokenOfOwnerByIndexAsync("0xNFTContract", 0);
Console.WriteLine($"First NFT Token ID: {tokenId}");

// Get staked tokens
var stakedTokens = await WalletConnect.GetStakedTokensAsync(
    "0xNFTContract", 
    "0xStakingContract"
);
Console.WriteLine($"Staked: {string.Join(", ", stakedTokens)}");
```

### Network Switching

```csharp
// Switch to Polygon
await WalletConnect.SwitchChainIdAsync(137);

// Switch to Ethereum Mainnet
await WalletConnect.SwitchChainIdAsync(1);
```

### Event Handling

```csharp
protected override async Task OnInitializedAsync()
{
    // Account changed event
    WalletConnect.AccountChanged += (sender, e) =>
    {
        var current = e.currentAccount;
        var previous = e.prevAccount;
        Console.WriteLine($"Account changed from {previous?.Address} to {current?.Address}");
        StateHasChanged();
    };
    
    // Chain/Network changed event
    WalletConnect.ChainIdChanged += (sender, e) =>
    {
        Console.WriteLine($"Network changed from {e.prevChainId} to {e.currentChainId}");
        StateHasChanged();
    };
    
    // Transaction confirmed event
    WalletConnect.TransactionConfirmed += (sender, e) =>
    {
        var receipt = e.TransactionReceipt;
        Console.WriteLine($"Transaction confirmed: {receipt.TransactionHash}");
        Console.WriteLine($"Status: {receipt.Status}");
        StateHasChanged();
    };
    
    await WalletConnect.ConfigureAsync();
}

public void Dispose()
{
    WalletConnect.AccountChanged -= OnAccountChanged;
    WalletConnect.ChainIdChanged -= OnChainIdChanged;
    WalletConnect.TransactionConfirmed -= OnTransactionConfirmed;
}
```

## 🎨 Customization

### Theme Options

```csharp
options.ThemeMode = "dark"; // "light", "dark", "auto"
options.BackgroundColor = "#1a1a1a";
options.AccentColor = "#3b82f6";
options.ColorMixStrength = 40; // 0-100
```

### Supported Chains

```csharp
using Nethereum.Signer;

options.Chains = new List<ChainDto>
{
    new ChainDto(Chain.MainNet, "https://mainnet.infura.io/v3/KEY"),
    new ChainDto(Chain.Polygon, "https://polygon-rpc.com"),
    new ChainDto(Chain.Arbitrum, "https://arb1.arbitrum.io/rpc"),
    new ChainDto(Chain.Optimism, "https://mainnet.optimism.io"),
    new ChainDto(Chain.Avalanche, "https://api.avax.network/ext/bc/C/rpc"),
};
```

## 🧪 Testing

The library includes comprehensive unit tests using NUnit, NSubstitute, and FluentAssertions.

```bash
# Run tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

See [tests/BlazorWalletConnect.Tests/README.md](tests/BlazorWalletConnect.Tests/README.md) for more details.

## 🛠️ Development

### Prerequisites

- .NET 9.*.* SDK
- Node.js (for TypeScript compilation)
- npm or yarn

### Build from Source

```bash
# Clone the repository
git clone https://github.com/Tricksfor-Organization/BlazorWalletConnect.git
cd BlazorWalletConnect

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Run the demo
cd demo/BlazorWalletConnectDemo
dotnet run
```

### Deploy Demo to Cloudflare Pages

The demo application can be automatically deployed to Cloudflare Pages:

1. **Quick Setup** - See [.github/workflows/QUICKSTART-CLOUDFLARE.md](.github/workflows/QUICKSTART-CLOUDFLARE.md)
2. **Full Documentation** - See [.github/workflows/README-CLOUDFLARE.md](.github/workflows/README-CLOUDFLARE.md)

The deployment workflow:
- ✅ Automatically triggers when you publish a new release
- ✅ Builds and publishes the Blazor WASM app
- ✅ Configures proper headers and SPA routing
- ✅ Deploys to Cloudflare's global CDN
- ✅ Provides instant worldwide access

## 📋 Requirements

- .NET 9.*.* or later
- Blazor WebAssembly
- Modern web browser with Web3 wallet support

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [WalletConnect](https://walletconnect.com/) - For the Web3 wallet connection protocol
- [Nethereum](https://nethereum.com/) - For Ethereum .NET integration
- [Reown AppKit](https://docs.reown.com/appkit) - For the beautiful wallet connection UI (formerly Web3Modal)

## 🔗 Links

- [NuGet Package](https://www.nuget.org/packages/Tricksfor.BlazorWalletConnect/)
- [GitHub Repository](https://github.com/Tricksfor-Organization/BlazorWalletConnect)
- [Demo Application](https://github.com/Tricksfor-Organization/BlazorWalletConnect/tree/main/demo)
- [WalletConnect Cloud](https://cloud.walletconnect.com/)

---

Made with ❤️ by [Tricksfor Organization](https://github.com/Tricksfor-Organization)
