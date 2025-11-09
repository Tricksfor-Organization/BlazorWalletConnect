# BlazorWalletConnect.Tests

This project contains comprehensive unit tests for the BlazorWalletConnect library using NUnit, NSubstitute, and FluentAssertions.

## Testing Stack

- **NUnit 4.4.0** - Testing framework
- **NUnit3TestAdapter 4.6.0** - Test adapter for Visual Studio/VS Code
- **Microsoft.NET.Test.Sdk 17.12.0** - Test platform
- **NSubstitute 5.3.0** - Mocking framework
- **FluentAssertions 8.8.0** - Assertion library
- **bUnit 1.32.7** - Testing library for Blazor components
- **coverlet.collector 6.0.2** - Code coverage tool

## Project Structure

```
BlazorWalletConnect.Tests/
├── ConfigurationTests/
│   └── ConfigurationsTests.cs          # Tests for service registration and DI
├── ServiceTests/
│   └── WalletConnectInteropTests.cs    # Tests for WalletConnectInterop service
├── HelperTests/
│   └── TransactionHelperTests.cs       # Tests for TransactionHelper utility
├── ModelTests/
│   └── ModelTests.cs                   # Tests for DTOs and models
├── EventTests/
│   └── EventArgsTests.cs               # Tests for event argument classes
└── BlazorWalletConnect.Tests.csproj    # Project file with CPM
```

## Test Coverage

### ConfigurationsTests
- Service registration with valid configuration
- Validation of required ProjectId
- Correct service lifetime (Scoped)
- Error handling for invalid configurations

### WalletConnectInteropTests
- JSInterop method invocations with mocked JSRuntime
- Configuration lifecycle
- All wallet operations (connect, disconnect, balance, transactions)
- Event handling (TransactionConfirmed, AccountChanged, ChainIdChanged)
- Resource disposal
- Error handling scenarios

### TransactionHelperTests
- Contract transaction input creation
- Function message encoding
- Address mapping and validation
- Different function message types

### ModelTests
- AccountDto serialization/deserialization
- BalanceDto with BigInteger values
- WalletConnectOptions configuration
- ChainDto for multiple blockchain networks

### EventArgsTests
- AccountChangedEventArgs with account transitions
- ChainIdChangedEventArgs for network switches
- TransactionConfirmedEventArgs with transaction receipts

## Running Tests

### Using .NET CLI

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~ConfigurationsTests"

# Run tests in a specific namespace
dotnet test --filter "FullyQualifiedName~BlazorWalletConnect.Tests.ServiceTests"
```

### Using Visual Studio Code

1. Install the **.NET Core Test Explorer** extension
2. Open the Test Explorer view (Test icon in the Activity Bar)
3. Click "Run All Tests" or run individual tests

### Using Visual Studio

1. Open Test Explorer (Test > Test Explorer)
2. Click "Run All" or run individual tests
3. View test results and coverage

## Test Principles

### Arrange-Act-Assert Pattern
All tests follow the AAA pattern for clarity:
```csharp
[Test]
public void Method_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and mocks
    var service = CreateService();
    
    // Act - Execute the method under test
    var result = service.DoSomething();
    
    // Assert - Verify the outcome
    result.Should().NotBeNull();
}
```

### Mocking with NSubstitute
JSInterop and other dependencies are mocked:
```csharp
var jsRuntime = Substitute.For<IJSRuntime>();
jsRuntime.InvokeAsync<string>("method", Arg.Any<object[]>())
    .Returns("expectedResult");
```

### Fluent Assertions
Readable and expressive assertions:
```csharp
result.Should().NotBeNull();
result.Address.Should().Be("0x123");
result.IsConnected.Should().BeTrue();
```

## Continuous Integration

These tests are designed to run in CI/CD pipelines:
```yaml
- name: Run Tests
  run: dotnet test --no-build --verbosity normal
```

## Code Coverage

To generate code coverage reports:

```bash
# Install ReportGenerator
dotnet tool install -g dotnet-reportgenerator-globaltool

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate HTML report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## Contributing

When adding new features to BlazorWalletConnect:
1. Write tests first (TDD approach)
2. Ensure all tests pass
3. Maintain or improve code coverage
4. Follow existing test patterns and naming conventions

## Notes

- Tests use **Central Package Management (CPM)** defined in `Directory.Packages.props`
- All package versions are centrally managed at the solution level
- Global usings are configured for NUnit, NSubstitute, and FluentAssertions
- Tests are isolated and can run in any order
- Mocks are reset between tests using `[SetUp]` and `[TearDown]` methods
