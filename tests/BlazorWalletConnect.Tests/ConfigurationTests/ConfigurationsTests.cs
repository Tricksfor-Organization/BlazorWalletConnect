using BlazorWalletConnect.Models;
using BlazorWalletConnect.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Nethereum.Signer;

namespace BlazorWalletConnect.Tests.ConfigurationTests;

[TestFixture]
public class ConfigurationsTests
{
    private IServiceCollection _services = null!;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        // Add a mock JSRuntime since WalletConnectInterop requires it
        _services.AddScoped<IJSRuntime>(_ => Substitute.For<IJSRuntime>());
    }

    [Test]
    public void AddBlazorWalletConnect_ShouldRegisterIWalletConnectInterop()
    {
        // Arrange & Act
        _services.AddBlazorWalletConnect(options =>
        {
            options.ProjectId = "test-project-id";
            options.Name = "Test App";
            options.Description = "Test Description";
            options.Url = "https://test.com";
            options.TermsConditionsUrl = "https://test.com/terms";
            options.PrivacyPolicyUrl = "https://test.com/privacy";
            options.ThemeMode = "light";
            options.BackgroundColor = "#ffffff";
            options.AccentColor = "#000000";
            options.EnableEmail = true;
            options.Chains = [new ChainDto(Chain.MainNet, "https://mainnet.infura.io")];
        });

        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        var service = serviceProvider.GetService<IWalletConnectInterop>();
        service.Should().NotBeNull();
        service.Should().BeOfType<WalletConnectInterop>();
    }

    [Test]
    public void AddBlazorWalletConnect_WithNullProjectId_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act
        var act = () => _services.AddBlazorWalletConnect(options =>
        {
            options.ProjectId = null!;
            options.Name = "Test App";
            options.Description = "Test Description";
            options.Url = "https://test.com";
            options.TermsConditionsUrl = "https://test.com/terms";
            options.PrivacyPolicyUrl = "https://test.com/privacy";
            options.ThemeMode = "light";
            options.BackgroundColor = "#ffffff";
            options.AccentColor = "#000000";
            options.EnableEmail = true;
            options.Chains = [new ChainDto(Chain.MainNet, "https://mainnet.infura.io")];
        }).BuildServiceProvider().GetService<IWalletConnectInterop>();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("You must provide a project Id to initialise WalletConnect.");
    }

    [Test]
    public void AddBlazorWalletConnect_WithEmptyProjectId_ShouldThrowInvalidOperationException()
    {
        // Arrange & Act
        var act = () => _services.AddBlazorWalletConnect(options =>
        {
            options.ProjectId = string.Empty;
            options.Name = "Test App";
            options.Description = "Test Description";
            options.Url = "https://test.com";
            options.TermsConditionsUrl = "https://test.com/terms";
            options.PrivacyPolicyUrl = "https://test.com/privacy";
            options.ThemeMode = "light";
            options.BackgroundColor = "#ffffff";
            options.AccentColor = "#000000";
            options.EnableEmail = true;
            options.Chains = [new ChainDto(Chain.MainNet, "https://mainnet.infura.io")];
        }).BuildServiceProvider().GetService<IWalletConnectInterop>();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("You must provide a project Id to initialise WalletConnect.");
    }

    [Test]
    public void AddBlazorWalletConnect_ShouldRegisterAsScoped()
    {
        // Arrange & Act
        _services.AddBlazorWalletConnect(options =>
        {
            options.ProjectId = "test-project-id";
            options.Name = "Test App";
            options.Description = "Test Description";
            options.Url = "https://test.com";
            options.TermsConditionsUrl = "https://test.com/terms";
            options.PrivacyPolicyUrl = "https://test.com/privacy";
            options.ThemeMode = "light";
            options.BackgroundColor = "#ffffff";
            options.AccentColor = "#000000";
            options.EnableEmail = true;
            options.Chains = [new ChainDto(Chain.MainNet, "https://mainnet.infura.io")];
        });

        // Assert
        var serviceDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IWalletConnectInterop));
        serviceDescriptor.Should().NotBeNull();
        serviceDescriptor!.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Test]
    public void AddBlazorWalletConnect_WithNullConfiguration_ShouldNotThrow()
    {
        // Arrange & Act
        var act = () => _services.AddBlazorWalletConnect(null);

        // Assert - The exception should occur when trying to get the service, not during registration
        act.Should().NotThrow();
    }
}
