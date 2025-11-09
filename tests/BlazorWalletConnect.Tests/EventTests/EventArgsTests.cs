using BlazorWalletConnect.Events;
using BlazorWalletConnect.Models;
using Nethereum.RPC.Eth.DTOs;

namespace BlazorWalletConnect.Tests.EventTests;

[TestFixture]
public class AccountChangedEventArgsTests
{
    [Test]
    public void AccountChangedEventArgs_ShouldStoreCurrentAccount()
    {
        // Arrange
        var currentAccount = new AccountDto(
            "0xNew",
            ["0xNew"],
            true,
            false,
            false,
            false,
            "connected",
            1
        );

        // Act
        var eventArgs = new AccountChangedEventArgs
        {
            currentAccount = currentAccount,
            prevAccount = null
        };

        // Assert
        eventArgs.currentAccount.Should().NotBeNull();
        eventArgs.currentAccount!.Address.Should().Be("0xNew");
        eventArgs.currentAccount.IsConnected.Should().BeTrue();
    }

    [Test]
    public void AccountChangedEventArgs_ShouldStorePreviousAccount()
    {
        // Arrange
        var prevAccount = new AccountDto(
            "0xOld",
            ["0xOld"],
            false,
            false,
            true,
            false,
            "disconnected",
            1
        );

        // Act
        var eventArgs = new AccountChangedEventArgs
        {
            currentAccount = null,
            prevAccount = prevAccount
        };

        // Assert
        eventArgs.prevAccount.Should().NotBeNull();
        eventArgs.prevAccount!.Address.Should().Be("0xOld");
        eventArgs.prevAccount.IsDisconnected.Should().BeTrue();
    }

    [Test]
    public void AccountChangedEventArgs_ShouldStoreBothAccounts()
    {
        // Arrange
        var currentAccount = new AccountDto("0xNew", ["0xNew"], true, false, false, false, "connected", 1);
        var prevAccount = new AccountDto("0xOld", ["0xOld"], false, false, true, false, "disconnected", 1);

        // Act
        var eventArgs = new AccountChangedEventArgs
        {
            currentAccount = currentAccount,
            prevAccount = prevAccount
        };

        // Assert
        eventArgs.currentAccount.Should().NotBeNull();
        eventArgs.prevAccount.Should().NotBeNull();
        eventArgs.currentAccount!.Address.Should().NotBe(eventArgs.prevAccount!.Address);
    }

    [Test]
    public void AccountChangedEventArgs_WithNullAccounts_ShouldWork()
    {
        // Act
        var eventArgs = new AccountChangedEventArgs
        {
            currentAccount = null,
            prevAccount = null
        };

        // Assert
        eventArgs.currentAccount.Should().BeNull();
        eventArgs.prevAccount.Should().BeNull();
    }

    [Test]
    public void AccountChangedEventArgs_ShouldInheritFromEventArgs()
    {
        // Arrange & Act
        var eventArgs = new AccountChangedEventArgs
        {
            currentAccount = null,
            prevAccount = null
        };

        // Assert
        eventArgs.Should().BeAssignableTo<EventArgs>();
    }
}

[TestFixture]
public class ChainIdChangedEventArgsTests
{
    [Test]
    public void ChainIdChangedEventArgs_ShouldStoreCurrentChainId()
    {
        // Arrange & Act
        var eventArgs = new ChainIdChangedEventArgs
        {
            currentChainId = 137,
            prevChainId = 1
        };

        // Assert
        eventArgs.currentChainId.Should().Be(137);
    }

    [Test]
    public void ChainIdChangedEventArgs_ShouldStorePreviousChainId()
    {
        // Arrange & Act
        var eventArgs = new ChainIdChangedEventArgs
        {
            currentChainId = 56,
            prevChainId = 1
        };

        // Assert
        eventArgs.prevChainId.Should().Be(1);
    }

    [Test]
    public void ChainIdChangedEventArgs_WithNullableChainIds_ShouldWork()
    {
        // Arrange & Act
        var eventArgs = new ChainIdChangedEventArgs
        {
            currentChainId = null,
            prevChainId = null
        };

        // Assert
        eventArgs.currentChainId.Should().BeNull();
        eventArgs.prevChainId.Should().BeNull();
    }

    [Test]
    public void ChainIdChangedEventArgs_WithOnlyCurrentChainId_ShouldWork()
    {
        // Arrange & Act
        var eventArgs = new ChainIdChangedEventArgs
        {
            currentChainId = 1,
            prevChainId = null
        };

        // Assert
        eventArgs.currentChainId.Should().Be(1);
        eventArgs.prevChainId.Should().BeNull();
    }

    [Test]
    public void ChainIdChangedEventArgs_ShouldInheritFromEventArgs()
    {
        // Arrange & Act
        var eventArgs = new ChainIdChangedEventArgs
        {
            currentChainId = 1,
            prevChainId = 1
        };

        // Assert
        eventArgs.Should().BeAssignableTo<EventArgs>();
    }

    [Test]
    public void ChainIdChangedEventArgs_WithDifferentChains_ShouldStoreBoth()
    {
        // Arrange & Act
        var eventArgs = new ChainIdChangedEventArgs
        {
            currentChainId = 137, // Polygon
            prevChainId = 1       // Ethereum Mainnet
        };

        // Assert
        eventArgs.currentChainId.Should().NotBe(eventArgs.prevChainId);
        eventArgs.currentChainId.Should().Be(137);
        eventArgs.prevChainId.Should().Be(1);
    }
}

[TestFixture]
public class TransactionConfirmedEventArgsTests
{
    [Test]
    public void TransactionConfirmedEventArgs_ShouldStoreTransactionReceipt()
    {
        // Arrange
        var receipt = new TransactionReceipt
        {
            TransactionHash = "0x1234567890abcdef",
            Status = new Nethereum.Hex.HexTypes.HexBigInteger(1),
            BlockNumber = new Nethereum.Hex.HexTypes.HexBigInteger(12345)
        };

        // Act
        var eventArgs = new TransactionConfirmedEventArgs
        {
            TransactionReceipt = receipt
        };

        // Assert
        eventArgs.TransactionReceipt.Should().NotBeNull();
        eventArgs.TransactionReceipt.TransactionHash.Should().Be("0x1234567890abcdef");
        eventArgs.TransactionReceipt.Status.Value.Should().Be(1);
    }

    [Test]
    public void TransactionConfirmedEventArgs_ShouldInheritFromEventArgs()
    {
        // Arrange
        var receipt = new TransactionReceipt
        {
            TransactionHash = "0xHash"
        };

        // Act
        var eventArgs = new TransactionConfirmedEventArgs
        {
            TransactionReceipt = receipt
        };

        // Assert
        eventArgs.Should().BeAssignableTo<EventArgs>();
    }

    [Test]
    public void TransactionConfirmedEventArgs_WithSuccessfulTransaction_ShouldHaveStatusOne()
    {
        // Arrange
        var receipt = new TransactionReceipt
        {
            TransactionHash = "0xSuccessHash",
            Status = new Nethereum.Hex.HexTypes.HexBigInteger(1),
            GasUsed = new Nethereum.Hex.HexTypes.HexBigInteger(21000)
        };

        // Act
        var eventArgs = new TransactionConfirmedEventArgs
        {
            TransactionReceipt = receipt
        };

        // Assert
        eventArgs.TransactionReceipt.Status.Value.Should().Be(1);
        eventArgs.TransactionReceipt.TransactionHash.Should().Be("0xSuccessHash");
    }

    [Test]
    public void TransactionConfirmedEventArgs_WithFailedTransaction_ShouldHaveStatusZero()
    {
        // Arrange
        var receipt = new TransactionReceipt
        {
            TransactionHash = "0xFailedHash",
            Status = new Nethereum.Hex.HexTypes.HexBigInteger(0)
        };

        // Act
        var eventArgs = new TransactionConfirmedEventArgs
        {
            TransactionReceipt = receipt
        };

        // Assert
        eventArgs.TransactionReceipt.Status.Value.Should().Be(0);
    }

    [Test]
    public void TransactionConfirmedEventArgs_WithCompleteReceipt_ShouldStoreAllProperties()
    {
        // Arrange
        var receipt = new TransactionReceipt
        {
            TransactionHash = "0xCompleteHash",
            BlockHash = "0xBlockHash",
            BlockNumber = new Nethereum.Hex.HexTypes.HexBigInteger(100),
            From = "0xFrom",
            To = "0xTo",
            GasUsed = new Nethereum.Hex.HexTypes.HexBigInteger(50000),
            Status = new Nethereum.Hex.HexTypes.HexBigInteger(1)
        };

        // Act
        var eventArgs = new TransactionConfirmedEventArgs
        {
            TransactionReceipt = receipt
        };

        // Assert
        eventArgs.TransactionReceipt.Should().NotBeNull();
        eventArgs.TransactionReceipt.TransactionHash.Should().Be("0xCompleteHash");
        eventArgs.TransactionReceipt.BlockHash.Should().Be("0xBlockHash");
        eventArgs.TransactionReceipt.From.Should().Be("0xFrom");
        eventArgs.TransactionReceipt.To.Should().Be("0xTo");
    }
}
