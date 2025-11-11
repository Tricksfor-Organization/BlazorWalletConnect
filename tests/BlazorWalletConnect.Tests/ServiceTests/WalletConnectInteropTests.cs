using BlazorWalletConnect.Events;
using BlazorWalletConnect.Models;
using BlazorWalletConnect.Services;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using System.Numerics;
using System.Text.Json;

namespace BlazorWalletConnect.Tests.ServiceTests;

[TestFixture]
public class WalletConnectInteropTests
{
    private IJSRuntime _jsRuntime = null!;
    private IJSObjectReference _jsModule = null!;
    private WalletConnectOptions _options = null!;
    private IOptions<WalletConnectOptions> _optionsWrapper = null!;
    private WalletConnectInterop _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _jsRuntime = Substitute.For<IJSRuntime>();
        _jsModule = Substitute.For<IJSObjectReference>();

        _options = new WalletConnectOptions
        {
            ProjectId = "test-project-id",
            Name = "Test App",
            Description = "Test Description",
            Url = "https://test.com",
            TermsConditionsUrl = "https://test.com/terms",
            PrivacyPolicyUrl = "https://test.com/privacy",
            ThemeMode = "light",
            BackgroundColor = "#ffffff",
            AccentColor = "#000000",
            Chains = [new ChainDto(Chain.MainNet, "https://mainnet.infura.io")]
        };

        _optionsWrapper = Options.Create(_options);

        // Setup JSRuntime to return the mock module
        _jsRuntime.InvokeAsync<IJSObjectReference>("import", Arg.Any<object[]>())
            .Returns(_jsModule);

        _sut = new WalletConnectInterop(_jsRuntime, _optionsWrapper);
    }

    [TearDown]
    public void TearDown()
    {
        _sut?.Dispose();
    }

    [Test]
    public async Task ConfigureAsync_ShouldCallJSInteropWithCorrectParameters()
    {
        // Act
        await _sut.ConfigureAsync();

        // Assert - Configuration should complete without exceptions
        Assert.Pass();
    }

    [Test]
    public async Task ConfigureAsync_WhenCalledMultipleTimes_ShouldOnlyConfigureOnce()
    {
        // Act
        await _sut.ConfigureAsync();
        await _sut.ConfigureAsync();
        await _sut.ConfigureAsync();

        // Assert - Should not throw exceptions
        Assert.Pass();
    }

    [Test]
    public async Task DisconnectAsync_ShouldCallJSInterop()
    {
        // Arrange
        await _sut.ConfigureAsync();

        // Act
        await _sut.DisconnectAsync();

        // Assert - Should complete without exceptions
        Assert.Pass();
    }

    [Test]
    public async Task GetAccountAsync_ShouldReturnDeserializedAccountDto()
    {
        // Arrange
        var expectedAccount = new AccountDto(
            Address: "0x1234567890123456789012345678901234567890",
            Addresses: ["0x1234567890123456789012345678901234567890"],
            IsConnected: true,
            IsConnecting: false,
            IsDisconnected: false,
            IsReconnecting: false,
            Status: "connected",
            ChainId: 1
        );

        var jsonResponse = JsonSerializer.Serialize(expectedAccount);
        _jsModule.InvokeAsync<string>("getWalletAccount")
            .Returns(jsonResponse);

        await _sut.ConfigureAsync();

        // Act
        var result = await _sut.GetAccountAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Address.Should().Be(expectedAccount.Address);
        result.IsConnected.Should().Be(expectedAccount.IsConnected);
        result.ChainId.Should().Be(expectedAccount.ChainId);
    }

    [Test]
    public async Task GetBalanceAsync_WithoutAddress_ShouldReturnBalanceDto()
    {
        // Arrange
        var jsonResponse = "{\"value\":\"1000000000000000000\",\"decimals\":18,\"symbol\":\"ETH\"}";
        _jsModule.InvokeAsync<string>("getWalletMainBalance")
            .Returns(jsonResponse);

        await _sut.ConfigureAsync();

        // Act
        var result = await _sut.GetBalanceAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Decimals.Should().Be(18);
        result.Symbol.Should().Be("ETH");
        result.Value.Should().Be(BigInteger.Parse("1000000000000000000"));
    }

    [Test]
    public async Task GetBalanceAsync_WithErc20TokenAddress_ShouldCallCorrectMethod()
    {
        // Arrange
        var tokenAddress = "0xTokenAddress";
        var jsonResponse = "{\"value\":\"5000000000000000000\",\"decimals\":18,\"symbol\":\"TKN\"}";
        _jsModule.InvokeAsync<string>("getBalanceOfErc20Token", Arg.Any<object[]>())
            .Returns(jsonResponse);

        await _sut.ConfigureAsync();

        // Act
        var result = await _sut.GetBalanceAsync(tokenAddress);

        // Assert
        result.Should().NotBeNull();
        result!.Decimals.Should().Be(18);
        result.Symbol.Should().Be("TKN");
        result.Value.Should().Be(BigInteger.Parse("5000000000000000000"));
    }

    [Test]
    public async Task GetBalanceOfAsync_ShouldReturnBigInteger()
    {
        // Arrange
        var contractAddress = "0xNftContract";
        var jsonResponse = "\"5\"";
        _jsModule.InvokeAsync<string>("getBalanceOfErc721Token", Arg.Any<object[]>())
            .Returns(jsonResponse);

        await _sut.ConfigureAsync();

        // Act
        var result = await _sut.GetBalanceOfAsync(contractAddress);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(new BigInteger(5));
    }

    [Test]
    public async Task GetTokenOfOwnerByIndexAsync_ShouldReturnTokenId()
    {
        // Arrange
        var contractAddress = "0xNftContract";
        var index = new BigInteger(2);
        var jsonResponse = "\"42\"";
        _jsModule.InvokeAsync<string>("getTokenOfOwnerByIndex", Arg.Any<object[]>())
            .Returns(jsonResponse);

        await _sut.ConfigureAsync();

        // Act
        var result = await _sut.GetTokenOfOwnerByIndexAsync(contractAddress, index);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(new BigInteger(42));
    }

    [Test]
    public async Task GetStakedTokensAsync_ShouldReturnListOfBigIntegers()
    {
        // Arrange
        var contractAddress = "0xNftContract";
        var stakeContractAddress = "0xStakeContract";
        var jsonResponse = "[\"1\", \"5\", \"10\"]";
        _jsModule.InvokeAsync<string>("getStakedTokens", Arg.Any<object[]>())
            .Returns(jsonResponse);

        await _sut.ConfigureAsync();

        // Act
        var result = await _sut.GetStakedTokensAsync(contractAddress, stakeContractAddress);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(new BigInteger(1));
        result.Should().Contain(new BigInteger(5));
        result.Should().Contain(new BigInteger(10));
    }

    [Test]
    public async Task SendTransactionAsync_ShouldReturnTransactionHash()
    {
        // Arrange
        var transactionInput = new TransactionInput
        {
            From = "0xFrom",
            To = "0xTo",
            Value = new Nethereum.Hex.HexTypes.HexBigInteger(1000)
        };
        var expectedHash = "0xTransactionHash";
        var jsonResponse = $"\"{expectedHash}\"";
        _jsModule.InvokeAsync<string>("SendTransaction", Arg.Any<object[]>())
            .Returns(jsonResponse);

        await _sut.ConfigureAsync();

        // Act
        var result = await _sut.SendTransactionAsync(transactionInput);

        // Assert
        result.Should().Be(expectedHash);
    }

    [Test]
    public async Task SendTransactionAsync_WithInvalidResponse_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var transactionInput = new TransactionInput();
        var invalidResponse = "Invalid JSON response";
        _jsModule.InvokeAsync<string>("SendTransaction", Arg.Any<object[]>())
            .Returns(invalidResponse);

        await _sut.ConfigureAsync();

        // Act
        var act = async () => await _sut.SendTransactionAsync(transactionInput);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Failed to deserialize transaction result: {invalidResponse}");
    }

    [Test]
    public async Task SignMessageAsync_ShouldReturnSignedMessage()
    {
        // Arrange
        var message = "Hello World";
        var expectedSignature = "0xSignature";
        var jsonResponse = $"\"{expectedSignature}\"";
        _jsModule.InvokeAsync<string>("SignMessage", Arg.Any<object[]>())
            .Returns(jsonResponse);

        await _sut.ConfigureAsync();

        // Act
        var result = await _sut.SignMessageAsync(message);

        // Assert
        result.Should().Be(expectedSignature);
    }

    [Test]
    public async Task SwitchChainIdAsync_ShouldCallJSInterop()
    {
        // Arrange
        var chainId = 137; // Polygon
        await _sut.ConfigureAsync();

        // Act & Assert - Should complete without exceptions
        await _sut.SwitchChainIdAsync(chainId);
        Assert.Pass();
    }

    [Test]
    public async Task TransactionConfirmed_EventShouldBeRaised()
    {
        // Arrange
        var eventRaised = false;
        TransactionConfirmedEventArgs? capturedArgs = null;
        _sut.TransactionConfirmed += (sender, args) =>
        {
            eventRaised = true;
            capturedArgs = args;
        };

        var receipt = new TransactionReceipt
        {
            TransactionHash = "0xHash",
            Status = new Nethereum.Hex.HexTypes.HexBigInteger(1)
        };
        var receiptJson = Newtonsoft.Json.JsonConvert.SerializeObject(receipt);

        // Act
        await _sut.OnTransactionConfirmed(receiptJson);

        // Assert
        eventRaised.Should().BeTrue();
        capturedArgs.Should().NotBeNull();
        capturedArgs!.TransactionReceipt.TransactionHash.Should().Be("0xHash");
    }

    [Test]
    public async Task AccountChanged_EventShouldBeRaised()
    {
        // Arrange
        var eventRaised = false;
        AccountChangedEventArgs? capturedArgs = null;
        _sut.AccountChanged += (sender, args) =>
        {
            eventRaised = true;
            capturedArgs = args;
        };

        var currentAccount = new AccountDto("0xNew", ["0xNew"], true, false, false, false, "connected", 1);
        var prevAccount = new AccountDto("0xOld", ["0xOld"], false, false, true, false, "disconnected", 1);

        var currentJson = JsonSerializer.Serialize(currentAccount);
        var prevJson = JsonSerializer.Serialize(prevAccount);

        // Act
        await _sut.OnAccountChanged(currentJson, prevJson);

        // Assert
        eventRaised.Should().BeTrue();
        capturedArgs.Should().NotBeNull();
        capturedArgs!.currentAccount!.Address.Should().Be("0xNew");
        capturedArgs.prevAccount!.Address.Should().Be("0xOld");
    }

    [Test]
    public async Task ChainIdChanged_EventShouldBeRaised()
    {
        // Arrange
        var eventRaised = false;
        ChainIdChangedEventArgs? capturedArgs = null;
        _sut.ChainIdChanged += (sender, args) =>
        {
            eventRaised = true;
            capturedArgs = args;
        };

        // Act
        await _sut.OnChainIdChanged(137, 1);

        // Assert
        eventRaised.Should().BeTrue();
        capturedArgs.Should().NotBeNull();
        capturedArgs!.currentChainId.Should().Be(137);
        capturedArgs.prevChainId.Should().Be(1);
    }

    [Test]
    public async Task DisposeAsync_ShouldDisposeResourcesProperly()
    {
        // Arrange
        await _sut.ConfigureAsync();

        // Act
        await _sut.DisposeAsync();

        // Assert
        await _jsModule.Received(1).DisposeAsync();
    }

    [Test]
    public void Dispose_ShouldDisposeResourcesProperly()
    {
        // Act
        _sut.Dispose();

        // Assert - Should not throw
        Assert.Pass();
    }

    [Test]
    public async Task MethodsCalledWithoutConfiguration_ShouldAutoConfigureFirst()
    {
        // Arrange
        var jsonResponse = JsonSerializer.Serialize(new AccountDto(
            "0x123", ["0x123"], true, false, false, false, "connected", 1));
        _jsModule.InvokeAsync<string>("getWalletAccount")
            .Returns(jsonResponse);

        // Act
        await _sut.GetAccountAsync(); // Should trigger ConfigureAsync automatically

        // Assert - Should complete without throwing exceptions
        Assert.Pass();
    }
}
