using BlazorWalletConnect.Models;
using Nethereum.Signer;
using System.Numerics;
using System.Text.Json;

namespace BlazorWalletConnect.Tests.ModelTests;

[TestFixture]
public class AccountDtoTests
{
    [Test]
    public void AccountDto_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var account = new AccountDto(
            Address: "0x1234567890123456789012345678901234567890",
            Addresses: ["0x1234567890123456789012345678901234567890", "0xAnotherAddress"],
            IsConnected: true,
            IsConnecting: false,
            IsDisconnected: false,
            IsReconnecting: false,
            Status: "connected",
            ChainId: 1
        );

        // Act
        var json = JsonSerializer.Serialize(account);
        var deserialized = JsonSerializer.Deserialize<AccountDto>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Address.Should().Be(account.Address);
        deserialized.Addresses.Should().HaveCount(2);
        deserialized.IsConnected.Should().BeTrue();
        deserialized.ChainId.Should().Be(1);
    }

    [Test]
    public void AccountDto_WithNullAddress_ShouldWorkCorrectly()
    {
        // Arrange
        var account = new AccountDto(
            Address: null,
            Addresses: [],
            IsConnected: false,
            IsConnecting: false,
            IsDisconnected: true,
            IsReconnecting: false,
            Status: "disconnected",
            ChainId: 0
        );

        // Act & Assert
        account.Address.Should().BeNull();
        account.IsDisconnected.Should().BeTrue();
    }

    [Test]
    public void AccountDto_Properties_ShouldBeAccessible()
    {
        // Arrange & Act
        var account = new AccountDto(
            "0xAddress",
            ["0xAddress"],
            true,
            false,
            false,
            false,
            "connected",
            137
        );

        // Assert
        account.Address.Should().Be("0xAddress");
        account.Addresses.Should().ContainSingle();
        account.IsConnected.Should().BeTrue();
        account.IsConnecting.Should().BeFalse();
        account.IsDisconnected.Should().BeFalse();
        account.IsReconnecting.Should().BeFalse();
        account.Status.Should().Be("connected");
        account.ChainId.Should().Be(137);
    }
}

[TestFixture]
public class BalanceDtoTests
{
    [Test]
    public void BalanceDto_ShouldCreateWithCorrectValues()
    {
        // Arrange & Act
        var balance = new BalanceDto(
            Decimals: 18,
            Symbol: "ETH",
            Value: BigInteger.Parse("1500000000000000000")
        );

        // Assert
        balance.Decimals.Should().Be(18);
        balance.Symbol.Should().Be("ETH");
        balance.Value.Should().Be(BigInteger.Parse("1500000000000000000"));
    }

    [Test]
    public void BalanceDto_WithLargeValue_ShouldHandleCorrectly()
    {
        // Arrange
        var largeValue = BigInteger.Parse("1000000000000000000000000");

        // Act
        var balance = new BalanceDto(18, "TKN", largeValue);

        // Assert
        balance.Value.Should().Be(largeValue);
    }

    [Test]
    public void BalanceDto_Properties_ShouldBeAccessible()
    {
        // Arrange & Act
        var balance = new BalanceDto(6, "USDC", new BigInteger(100500000));

        // Assert
        balance.Decimals.Should().Be(6);
        balance.Symbol.Should().Be("USDC");
        balance.Value.Should().Be(new BigInteger(100500000));
    }
}

[TestFixture]
public class WalletConnectOptionsTests
{
    [Test]
    public void WalletConnectOptions_ShouldSerializeCorrectly()
    {
        // Arrange
        var options = new WalletConnectOptions
        {
            ProjectId = "test-project-id",
            Name = "Test App",
            Description = "Test Description",
            Url = "https://test.com",
            TermsConditionsUrl = "https://test.com/terms",
            PrivacyPolicyUrl = "https://test.com/privacy",
            ThemeMode = "dark",
            BackgroundColor = "#000000",
            AccentColor = "#ffffff",
            Chains = [new ChainDto(Chain.MainNet, "https://mainnet.infura.io")]
        };

        // Act
        var json = JsonSerializer.Serialize(options);
        var deserialized = JsonSerializer.Deserialize<WalletConnectOptions>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.ProjectId.Should().Be(options.ProjectId);
        deserialized.Name.Should().Be(options.Name);
        deserialized.ThemeMode.Should().Be("dark");
        deserialized.Chains.Should().HaveCount(1);
    }

    [Test]
    public void WalletConnectOptions_WithMultipleChains_ShouldWorkCorrectly()
    {
        // Arrange
        var options = new WalletConnectOptions
        {
            ProjectId = "test-id",
            Name = "App",
            Description = "Desc",
            Url = "https://app.com",
            TermsConditionsUrl = "https://app.com/terms",
            PrivacyPolicyUrl = "https://app.com/privacy",
            ThemeMode = "light",
            BackgroundColor = "#fff",
            AccentColor = "#000",
            Chains =
            [
                new ChainDto(Chain.MainNet, "https://mainnet.infura.io"),
                new ChainDto(Chain.Polygon, "https://polygon-rpc.com"),
                new ChainDto(Chain.Sepolia, null)
            ]
        };

        // Act & Assert
        options.Chains.Should().HaveCount(3);
        options.Chains[0].Chain.Should().Be(Chain.MainNet);
        options.Chains[1].Chain.Should().Be(Chain.Polygon);
        options.Chains[2].RpcUrl.Should().BeNull();
    }

    [Test]
    public void WalletConnectOptions_AllProperties_ShouldBeSettable()
    {
        // Arrange & Act
        var options = new WalletConnectOptions
        {
            ProjectId = "proj-123",
            Name = "My App",
            Description = "My Description",
            Url = "https://myapp.com",
            TermsConditionsUrl = "https://myapp.com/terms",
            PrivacyPolicyUrl = "https://myapp.com/privacy",
            ThemeMode = "auto",
            BackgroundColor = "#f0f0f0",
            AccentColor = "#ff5500",
            Chains =
            [
                new ChainDto(Chain.MainNet, "https://mainnet.infura.io"),
                new ChainDto(Chain.Polygon, "https://polygon-rpc.com"),
                new ChainDto(Chain.Sepolia, null)
            ]
        };

        // Assert
        options.ProjectId.Should().Be("proj-123");
        options.Name.Should().Be("My App");
        options.Description.Should().Be("My Description");
        options.Url.Should().Be("https://myapp.com");
        options.TermsConditionsUrl.Should().Be("https://myapp.com/terms");
        options.PrivacyPolicyUrl.Should().Be("https://myapp.com/privacy");
        options.ThemeMode.Should().Be("auto");
        options.BackgroundColor.Should().Be("#f0f0f0");
        options.AccentColor.Should().Be("#ff5500");
        options.Chains.Count.Should().Be(3);
    }
}

[TestFixture]
public class ChainDtoTests
{
    [Test]
    public void ChainDto_ShouldCreateWithChainAndRpcUrl()
    {
        // Arrange & Act
        var chain = new ChainDto(Chain.MainNet, "https://mainnet.infura.io");

        // Assert
        chain.Chain.Should().Be(Chain.MainNet);
        chain.RpcUrl.Should().Be("https://mainnet.infura.io");
    }

    [Test]
    public void ChainDto_WithNullRpcUrl_ShouldWork()
    {
        // Arrange & Act
        var chain = new ChainDto(Chain.Polygon, null);

        // Assert
        chain.Chain.Should().Be(Chain.Polygon);
        chain.RpcUrl.Should().BeNull();
    }

    [Test]
    public void ChainDto_ShouldSerializeCorrectly()
    {
        // Arrange
        var chain = new ChainDto(Chain.Sepolia, "https://sepolia.infura.io");

        // Act
        var json = JsonSerializer.Serialize(chain);

        // Assert
        json.Should().Contain("chainId");
        json.Should().Contain("rpcUrl");
    }

    [Test]
    public void ChainDto_WithDifferentChains_ShouldWork()
    {
        // Arrange & Act
        var mainnet = new ChainDto(Chain.MainNet, "url1");
        var polygon = new ChainDto(Chain.Polygon, "url2");
        var sepolia = new ChainDto(Chain.Sepolia, "url3");

        // Assert
        mainnet.Chain.Should().Be(Chain.MainNet);
        polygon.Chain.Should().Be(Chain.Polygon);
        sepolia.Chain.Should().Be(Chain.Sepolia);
    }
}
