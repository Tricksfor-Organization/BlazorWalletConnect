using BlazorWalletConnect.Components;
using BlazorWalletConnect.Services;
using Bunit;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorWalletConnect.Tests.ComponentTests;

[TestFixture]
public class WalletConnectButtonTests
{
    [Test]
    public void ButtonColorAndCssClass_ShouldBeAppliedToAppKitButtonHost()
    {
        using var context = CreateContext();

        var component = context.Render<WalletConnectButton>(parameters => parameters
            .Add(button => button.Label, "Connect")
            .Add(button => button.ButtonColor, " #22c55e ")
            .Add(button => button.CssClass, "wallet-connect"));

        var button = component.Find("appkit-button");
        button.ClassList.Should().Contain("wallet-connect");
        button.GetAttribute("style").Should()
            .Be("--apkt-tokens-core-backgroundAccentPrimary: #22c55e;");
    }

    [Test]
    public void ButtonColorAndCssClass_WithoutLabel_ShouldBeAppliedToConnectButtonHost()
    {
        using var context = CreateContext();

        var component = context.Render<WalletConnectButton>(parameters => parameters
            .Add(button => button.ButtonColor, "rgb(34 197 94)")
            .Add(button => button.CssClass, "wallet-connect"));

        var button = component.Find("appkit-connect-button");
        button.ClassList.Should().Contain("wallet-connect");
        button.GetAttribute("style").Should()
            .Be("--apkt-tokens-core-backgroundAccentPrimary: rgb(34 197 94);");
    }

    [Test]
    public void ButtonColorAndCssClass_WhenNotSpecified_ShouldBeOmitted()
    {
        using var context = CreateContext();

        var component = context.Render<WalletConnectButton>(parameters => parameters
            .Add(button => button.Label, "Connect"));

        var button = component.Find("appkit-button");
        button.HasAttribute("class").Should().BeFalse();
        button.HasAttribute("style").Should().BeFalse();
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        var interop = Substitute.For<IWalletConnectInterop>();
        interop.ConfigureAsync().Returns(Task.CompletedTask);
        context.Services.AddSingleton(interop);
        return context;
    }
}
