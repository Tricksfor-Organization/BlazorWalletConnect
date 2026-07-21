using BlazorWalletConnect.Models;
using BlazorWalletConnect.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWalletConnect
{
    public static class Configurations
    {
        public static IServiceCollection AddBlazorWalletConnect(this IServiceCollection services, Action<WalletConnectOptions>? configure)
        {
            services.Configure<WalletConnectOptions>(options =>
            {
                configure?.Invoke(options);
                if (string.IsNullOrEmpty(options.ProjectId))
                {
                    throw new InvalidOperationException("You must provide a project Id to initialise WalletConnect.");
                }

                if (!string.Equals(options.ThemeMode, "auto", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(options.ThemeMode, "light", StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(options.ThemeMode, "dark", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("ThemeMode must be Auto, Light, or Dark.");
                }

                if (options.ColorMixStrength is < 0 or > 100)
                {
                    throw new InvalidOperationException("ColorMixStrength must be between 0 and 100.");
                }
            });

            services.AddScoped<IWalletConnectInterop, WalletConnectInterop>();

            return services;
        }
    }
}
