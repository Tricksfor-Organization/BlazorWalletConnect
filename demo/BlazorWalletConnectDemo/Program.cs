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
    options.Url = "https://localhost:5001";
    options.TermsConditionsUrl = "https://example.com/terms";
    options.PrivacyPolicyUrl = "https://example.com/privacy";
    options.ThemeMode = "dark";
    options.BackgroundColor = "#1a1a1a";
    options.AccentColor = "#3b82f6";
    options.EnableEmail = true;
    options.Chains = new List<BlazorWalletConnect.Models.ChainDto>
    {
        new(Chain.MainNet, "https://mainnet.infura.io/v3/YOUR_INFURA_KEY"),
        new(Chain.Polygon, "https://polygon-rpc.com")
    };
});

await builder.Build().RunAsync();
