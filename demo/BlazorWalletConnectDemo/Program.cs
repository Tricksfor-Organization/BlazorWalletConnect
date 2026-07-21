using BlazorWalletConnect;
using BlazorWalletConnectDemo;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Nethereum.Signer;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Configure BlazorWalletConnect
builder.Services.AddBlazorWalletConnect(options =>
{
    // Read ProjectId from user secrets or appsettings.json
    var projectId = builder.Configuration.GetValue<string>("WalletConnect:ProjectId");
    if (string.IsNullOrEmpty(projectId))
    {
        throw new InvalidOperationException(
            "WalletConnect ProjectId is not configured. " +
            "Please set it using: dotnet user-secrets set \"WalletConnect:ProjectId\" \"YOUR_PROJECT_ID\"");
    }

    options.ProjectId = projectId;
    options.Name = "BlazorWalletConnect Demo";
    options.Description = "A demo application showcasing BlazorWalletConnect integration";
    options.Url = builder.HostEnvironment.BaseAddress;
    options.TermsConditionsUrl = "https://example.com/terms";
    options.PrivacyPolicyUrl = "https://example.com/privacy";
    options.ThemeMode = "dark";
    options.BackgroundColor = "#28363c";
    options.AccentColor = "#3b82f6";
    options.ColorMixStrength = 40;
    options.Chains = new List<BlazorWalletConnect.Models.ChainDto>
    {
        new(Chain.MainNet, "https://ethereum-rpc.publicnode.com"),
        new(Chain.Polygon, "https://polygon-rpc.com"),
        new(Chain.Avalanche, "https://api.avax.network/ext/bc/C/rpc")
    };
});

await builder.Build().RunAsync();
