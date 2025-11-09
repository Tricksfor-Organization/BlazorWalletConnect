# BlazorWalletConnect Demo

A Progressive Web App (PWA) demonstrating the integration of BlazorWalletConnect library in a Blazor WebAssembly application.

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- A WalletConnect Project ID (free from [WalletConnect Cloud](https://cloud.walletconnect.com/))
- (Optional) An Infura API key for custom RPC endpoints

### Setup Instructions

1. **Get a WalletConnect Project ID**
   - Visit [WalletConnect Cloud](https://cloud.walletconnect.com/)
   - Create a free account
   - Create a new project
   - Copy your Project ID

2. **Configure the Application**
   
   Set your WalletConnect Project ID using user secrets (recommended):
   ```bash
   cd demo/BlazorWalletConnectDemo
   dotnet user-secrets set "WalletConnect:ProjectId" "YOUR_PROJECT_ID"
   ```
   
   Alternatively, you can set it in `appsettings.Development.json` (not recommended for production):
   ```json
   {
     "WalletConnect": {
       "ProjectId": "YOUR_PROJECT_ID"
     }
   }
   ```
   
   - (Optional) Replace `YOUR_INFURA_KEY` in `Program.cs` with your Infura API key or use public RPC endpoints

3. **Run the Application**
   ```bash
   cd demo/BlazorWalletConnectDemo
   dotnet run
   ```

4. **Access the App**
   - Open your browser and navigate to `https://localhost:5001`
   - Click the "Connect Wallet" button to connect your Web3 wallet

## 📱 Features

This demo showcases the following features of BlazorWalletConnect:

- ✅ Easy wallet connection with WalletConnect protocol
- ✅ Support for multiple blockchain networks (Ethereum Mainnet, Sepolia, Polygon)
- ✅ Account balance display
- ✅ Chain switching support
- ✅ Responsive design for mobile and desktop
- ✅ PWA support for offline functionality

## 🔧 Usage

### Adding the WalletConnect Button

Simply add the component to your Razor page:

```razor
<WalletConnectButton Lable="Connect Wallet" ShowBalance="true" />
```

### Configuration

Configure BlazorWalletConnect in your `Program.cs`:

```csharp
builder.Services.AddBlazorWalletConnect(options =>
{
    options.ProjectId = "YOUR_PROJECT_ID";
    options.Name = "Your App Name";
    options.Description = "Your app description";
    options.Url = "https://your-app-url.com";
    options.TermsConditionsUrl = "https://your-app-url.com/terms";
    options.PrivacyPolicyUrl = "https://your-app-url.com/privacy";
    options.ThemeMode = "dark"; // or "light"
    options.BackgroundColor = "#1a1a1a";
    options.AccentColor = "#3b82f6";
    options.EnableEmail = true;
    options.Chains = new List<BlazorWalletConnect.Models.ChainDto>
    {
        new(Chain.MainNet, "YOUR_RPC_URL"),
        new(Chain.Sepolia, "YOUR_RPC_URL"),
        new(Chain.Polygon, "YOUR_RPC_URL")
    };
});
```

## 🌐 Supported Networks

The demo is configured to support:

- Ethereum Mainnet
- Ethereum Sepolia (testnet)
- Polygon

You can add more networks by adding them to the `Chains` list in the configuration.

## 📦 Project Structure

```
BlazorWalletConnectDemo/
├── Pages/
│   └── Home.razor          # Main page with WalletConnect button
├── Shared/
│   └── MainLayout.razor    # Application layout
├── wwwroot/
│   ├── css/
│   │   └── app.css        # Application styles
│   ├── manifest.json      # PWA manifest
│   ├── service-worker.js  # Service worker for PWA
│   └── index.html         # Main HTML file
├── App.razor              # App component
├── Program.cs             # Application entry point
└── _Imports.razor         # Global imports
```

## 🔗 Resources

- [BlazorWalletConnect GitHub](https://github.com/Tricksfor-Organization/BlazorWalletConnect)
- [WalletConnect Documentation](https://docs.walletconnect.com/)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)

## 📝 License

This demo application is part of the BlazorWalletConnect project and follows the same license.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
