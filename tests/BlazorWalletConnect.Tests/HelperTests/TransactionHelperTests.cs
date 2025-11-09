using BlazorWalletConnect.Helpers;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace BlazorWalletConnect.Tests.HelperTests;

[TestFixture]
public class TransactionHelperTests
{
    [Test]
    public void CreateContractTransactionInput_ShouldSetFromAddress()
    {
        // Arrange
        var request = new TestFunctionMessage
        {
            Recipient = "0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb",
            Amount = 0
        };
        var fromAddress = "0x1234567890123456789012345678901234567890";
        var contractAddress = "0xContractAddress123456789012345678901234";

        // Act
        var result = TransactionHelper.CreateContractTransactionInput(request, fromAddress, contractAddress);

        // Assert
        result.Should().NotBeNull();
        result.From.Should().Be(fromAddress);
    }

    [Test]
    public void CreateContractTransactionInput_ShouldSetToAddress()
    {
        // Arrange
        var request = new TestFunctionMessage
        {
            Recipient = "0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb",
            Amount = 0
        };
        var fromAddress = "0x1234567890123456789012345678901234567890";
        var contractAddress = "0xContractAddress123456789012345678901234";

        // Act
        var result = TransactionHelper.CreateContractTransactionInput(request, fromAddress, contractAddress);

        // Assert
        result.To.Should().Be(contractAddress);
    }

    [Test]
    public void CreateContractTransactionInput_ShouldEncodeFunction()
    {
        // Arrange
        var request = new TestFunctionMessage
        {
            Amount = 100,
            Recipient = "0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb"
        };
        var fromAddress = "0x1234567890123456789012345678901234567890";
        var contractAddress = "0xContractAddress123456789012345678901234";

        // Act
        var result = TransactionHelper.CreateContractTransactionInput(request, fromAddress, contractAddress);

        // Assert
        result.Data.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void CreateContractTransactionInput_WithDifferentFunctionTypes_ShouldWork()
    {
        // Arrange
        var request = new AnotherTestFunctionMessage
        {
            Value = "test value"
        };
        var fromAddress = "0xFrom";
        var contractAddress = "0xContract";

        // Act
        var result = TransactionHelper.CreateContractTransactionInput(request, fromAddress, contractAddress);

        // Assert
        result.Should().NotBeNull();
        result.From.Should().Be(fromAddress);
        result.To.Should().Be(contractAddress);
        result.Data.Should().NotBeNullOrEmpty();
    }

    [Test]
    public void CreateContractTransactionInput_ShouldReturnValidTransactionInput()
    {
        // Arrange
        var request = new TestFunctionMessage
        {
            Recipient = "0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb",
            Amount = 0
        };
        var fromAddress = "0xFrom";
        var contractAddress = "0xContract";

        // Act
        var result = TransactionHelper.CreateContractTransactionInput(request, fromAddress, contractAddress);

        // Assert
        result.Should().BeOfType<TransactionInput>();
        result.From.Should().Be(fromAddress);
        result.To.Should().Be(contractAddress);
    }
}

// Test function messages for testing purposes
[Nethereum.ABI.FunctionEncoding.Attributes.Function("transfer")]
public class TestFunctionMessage : FunctionMessage
{
    [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("address", "recipient", 1)]
    public string? Recipient { get; set; }

    [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("uint256", "amount", 2)]
    public int Amount { get; set; }
}

[Nethereum.ABI.FunctionEncoding.Attributes.Function("setValue")]
public class AnotherTestFunctionMessage : FunctionMessage
{
    [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("string", "value", 1)]
    public string? Value { get; set; }
}
