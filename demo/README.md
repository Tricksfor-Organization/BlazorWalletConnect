# BlazorWalletConnect Demo

This folder contains a sample Blazor PWA application demonstrating the usage of BlazorWalletConnect.

## 🌐 Live Demo

**Try it live:** [blazorwalletconnect-demo.pages.dev](https://blazorwalletconnect-demo.pages.dev)

The demo is automatically deployed to Cloudflare Pages whenever a new release is published.

## Quick Start

```bash
cd BlazorWalletConnectDemo
dotnet run
```

Then open your browser to `https://localhost:5001`

## Important

Before running, configure your WalletConnect Project ID:
- Get a free Project ID from https://cloud.walletconnect.com/
- Set your Project ID using [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) or by adding it to `appsettings.json` (see lines 23-27 in `Program.cs`)

## More Information

See the [full README](BlazorWalletConnectDemo/README.md) in the project folder for detailed setup and usage instructions.
