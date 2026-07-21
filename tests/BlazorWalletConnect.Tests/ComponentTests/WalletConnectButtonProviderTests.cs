using BlazorWalletConnect.Components;
using BlazorWalletConnect.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWalletConnect.Tests.ComponentTests;

[TestFixture]
public class WalletConnectButtonProviderTests
{
    [Test]
    public void Provider_ShouldRenderOnlyTheConsumerButton()
    {
        using var context = new BunitContext();
        var interop = Substitute.For<IWalletConnectInterop>();
        context.Services.AddSingleton(interop);

        var component = context.Render<WalletConnectButtonProvider>(parameters => parameters
            .AddChildContent("<appkit-button id=\"consumer-button\"></appkit-button>"));

        component.WaitForState(() => component.FindAll("appkit-button").Count == 1);
        component.FindAll("appkit-button").Should().ContainSingle();
        component.Find("appkit-button").Id.Should().Be("consumer-button");
    }
}
