# GitHub Copilot Instructions for BlazorWalletConnect

## Project Overview

BlazorWalletConnect is a Blazor WebAssembly library that integrates WalletConnect for Web3 wallet interactions. It supports multiple blockchain networks (Ethereum, Polygon, Arbitrum, Optimism, BSC) and provides comprehensive wallet operations including balance queries, transactions, message signing, and NFT operations.

## Technology Stack

- **Framework**: .NET 9.0, Blazor WebAssembly
- **Language**: C# 12, TypeScript 5.x
- **Package Manager**: NuGet (with Central Package Management)
- **JavaScript**: Web3Modal, Wagmi, Viem
- **Testing**: NUnit 4.4, NSubstitute 5.3, FluentAssertions 8.8, bUnit 1.32
- **Build**: MSBuild, npm/webpack

## Project Structure

```
BlazorWalletConnect/
├── src/BlazorWalletConnect/           # Main library
│   ├── Components/                     # Razor components
│   ├── Events/                         # Event argument classes
│   ├── Helpers/                        # Utility helpers
│   ├── Models/                         # DTOs and models
│   ├── Npm/                           # TypeScript/JavaScript sources
│   └── Services/                       # Core services
├── tests/BlazorWalletConnect.Tests/   # Unit tests
├── demo/BlazorWalletConnectDemo/      # Demo application
└── .github/workflows/                 # CI/CD workflows
```

## Code Style and Conventions

### C# Guidelines

1. **Naming Conventions**
   - Use PascalCase for classes, methods, properties, and public fields
   - Use camelCase for private fields with underscore prefix (`_fieldName`)
   - Use PascalCase for interfaces with 'I' prefix (`IWalletConnectInterop`)
   - Use PascalCase for async methods with 'Async' suffix (`GetAccountAsync`)

2. **Code Organization**
   - Keep classes focused and single-purpose
   - Use meaningful names that describe intent
   - Group related functionality in regions (e.g., `#region Methods`)
   - Place using statements at the top of files

3. **Async/Await Patterns**
   - Always use `async/await` for I/O operations
   - Return `Task` or `Task<T>` for async methods
   - Use `ValueTask` for performance-critical paths
   - Configure await with `ConfigureAwait(false)` when appropriate

4. **Nullable Reference Types**
   - Project uses nullable reference types enabled
   - Use `?` for nullable types
   - Use null-forgiving operator `!` sparingly and only when certain

5. **Dependency Injection**
   - Register services in extension methods
   - Use constructor injection
   - Prefer interface over concrete implementations
   - Use scoped lifetime for services that maintain state

### TypeScript Guidelines

1. **Type Safety**
   - Always define explicit types
   - Avoid `any` type
   - Use interfaces for object shapes
   - Use type guards for runtime checks

2. **Async Operations**
   - Use async/await for asynchronous operations
   - Handle errors with try-catch blocks
   - Return promises for asynchronous functions

3. **Function Naming**
   - Use camelCase for function names
   - Export functions that will be called from C#
   - Keep functions small and focused

## Coding Patterns

### Service Pattern

```csharp
public class WalletConnectInterop : IWalletConnectInterop
{
    private readonly IJSRuntime _jsRuntime;
    private readonly WalletConnectOptions _options;
    
    public WalletConnectInterop(IJSRuntime jsRuntime, IOptions<WalletConnectOptions> options)
    {
        _jsRuntime = jsRuntime;
        _options = options.Value;
    }
    
    public async Task<AccountDto?> GetAccountAsync()
    {
        await EnsureConfiguredAsync();
        var module = await GetModuleAsync();
        var result = await module.InvokeAsync<string>("getWalletAccount");
        return JsonSerializer.Deserialize<AccountDto>(result);
    }
}
```

### Event Handling Pattern

```csharp
public event EventHandler<AccountChangedEventArgs>? AccountChanged;

private void RaiseAccountChanged(AccountDto? current, AccountDto? previous)
{
    var e = new AccountChangedEventArgs
    {
        currentAccount = current,
        prevAccount = previous
    };
    AccountChanged?.Invoke(this, e);
}
```

### JSInterop Pattern

```csharp
// C# side
var module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
    "import", "./_content/BlazorWalletConnect/main.bundle.js");
var result = await module.InvokeAsync<string>("functionName", param1, param2);

// TypeScript side
export async function functionName(param1: string, param2: number) {
    // Implementation
    return JSON.stringify(result);
}
```

## Testing Guidelines

### Unit Test Structure

1. **Test Organization**
   - Use AAA pattern (Arrange-Act-Assert)
   - One test class per production class
   - Group related tests in nested classes or folders
   - Use descriptive test names: `Method_Scenario_ExpectedBehavior`

2. **Test Fixtures**
   ```csharp
   [TestFixture]
   public class WalletConnectInteropTests
   {
       private IJSRuntime _jsRuntime;
       private WalletConnectInterop _sut; // System Under Test
       
       [SetUp]
       public void SetUp()
       {
           _jsRuntime = Substitute.For<IJSRuntime>();
           _sut = new WalletConnectInterop(_jsRuntime, options);
       }
       
       [TearDown]
       public void TearDown()
       {
           _sut?.Dispose();
       }
   }
   ```

3. **Mocking with NSubstitute**
   ```csharp
   _jsModule.InvokeAsync<string>("method", Arg.Any<object[]>())
       .Returns(jsonResponse);
   ```

4. **Assertions with FluentAssertions**
   ```csharp
   result.Should().NotBeNull();
   result.Address.Should().Be(expectedAddress);
   result.IsConnected.Should().BeTrue();
   ```

## Component Development

### Razor Component Pattern

```razor
@inject IWalletConnectInterop WalletConnect
@implements IDisposable

<div class="wallet-component">
    @if (account?.IsConnected == true)
    {
        <p>Connected: @account.Address</p>
    }
    else
    {
        <button @onclick="Connect">Connect Wallet</button>
    }
</div>

@code {
    private AccountDto? account;
    
    protected override async Task OnInitializedAsync()
    {
        WalletConnect.AccountChanged += OnAccountChanged;
        await WalletConnect.ConfigureAsync();
    }
    
    private async Task Connect()
    {
        // Connection logic
    }
    
    private void OnAccountChanged(object? sender, AccountChangedEventArgs e)
    {
        account = e.currentAccount;
        StateHasChanged();
    }
    
    public void Dispose()
    {
        WalletConnect.AccountChanged -= OnAccountChanged;
    }
}
```

## Package Management

### Central Package Management (CPM)

- All package versions are defined in `Directory.Packages.props`
- Use `<PackageReference Include="PackageName" />` without version in .csproj
- Update versions centrally in one place

### Adding New Packages

1. Add to `Directory.Packages.props`:
   ```xml
   <PackageVersion Include="NewPackage" Version="1.0.0" />
   ```

2. Reference in project:
   ```xml
   <PackageReference Include="NewPackage" />
   ```

## Build and Deployment

### Local Development

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run demo
cd demo/BlazorWalletConnectDemo
dotnet run
```

### NuGet Package

- Package ID: `Tricksfor.BlazorWalletConnect`
- Automatically built on `GeneratePackageOnBuild`
- Version managed in `.csproj`
- Published to NuGet.org

## Common Tasks

### Adding New Wallet Operation

1. Add method to `IWalletConnectInterop` interface
2. Implement in `WalletConnectInterop` class
3. Add corresponding TypeScript function in `main.ts`
4. Create unit tests
5. Update documentation

### Adding New Event

1. Create event args class in `Events/` folder
2. Add event to `IWalletConnectInterop`
3. Implement event raising in `WalletConnectInterop`
4. Add JSInvokable callback method
5. Wire up in TypeScript
6. Create unit tests

### Adding New Model/DTO

1. Create record or class in `Models/` folder
2. Use JSON serialization attributes
3. Add validation if needed
4. Create unit tests for serialization

## Error Handling

### C# Exception Handling

```csharp
public async Task<string> SendTransactionAsync(TransactionInput input)
{
    try
    {
        var result = await _module.InvokeAsync<string>("SendTransaction", input);
        return JsonSerializer.Deserialize<string>(result);
    }
    catch (JSException jsEx)
    {
        throw new InvalidOperationException($"Transaction failed: {jsEx.Message}", jsEx);
    }
}
```

### TypeScript Error Handling

```typescript
export async function sendTransaction(input: string) {
    try {
        const transaction = JSON.parse(input);
        const result = await sendTransaction(config, transaction);
        return JSON.stringify(result);
    }
    catch (e) {
        const error = e as SendTransactionErrorType;
        return JSON.stringify({ error: error.message });
    }
}
```

## Performance Considerations

1. **Minimize JSInterop Calls**
   - Batch operations when possible
   - Cache module references
   - Use ValueTask for hot paths

2. **Async Best Practices**
   - Don't block async methods
   - Use ConfigureAwait appropriately
   - Avoid async void except for event handlers

3. **Memory Management**
   - Dispose IJSObjectReference instances
   - Unsubscribe from events in Dispose
   - Use DotNetObjectReference.Create carefully

## Documentation Standards

1. **XML Documentation Comments**
   ```csharp
   /// <summary>
   /// Gets the current wallet account information.
   /// </summary>
   /// <returns>The account details or null if not connected.</returns>
   public async Task<AccountDto?> GetAccountAsync()
   ```

2. **README Updates**
   - Update for new features
   - Include code examples
   - Keep API documentation current

3. **Changelog**
   - Document breaking changes
   - Note new features
   - List bug fixes

## Security Considerations

1. **Input Validation**
   - Validate addresses (checksum)
   - Sanitize user input
   - Validate transaction parameters

2. **Error Messages**
   - Don't expose sensitive information
   - Log securely
   - Use appropriate error types

3. **Dependencies**
   - Keep packages updated
   - Review security advisories
   - Use dependabot

## Contribution Workflow

1. Create feature branch from `develop`
2. Write code following conventions
3. Add/update tests (maintain coverage)
4. Update documentation
5. Run tests locally
6. Create pull request
7. Address review comments
8. Merge after approval

## Resources

- [WalletConnect Documentation](https://docs.walletconnect.com/)
- [Web3Modal Documentation](https://docs.web3modal.com/)
- [Nethereum Documentation](https://docs.nethereum.com/)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)
- [NUnit Documentation](https://docs.nunit.org/)

## Quick Reference

### Important Files
- `Configurations.cs` - Service registration
- `WalletConnectInterop.cs` - Main service implementation
- `main.ts` - TypeScript/JavaScript interop
- `Directory.Packages.props` - Package versions
- `ci.yml` - CI/CD pipeline

### Key Namespaces
- `BlazorWalletConnect` - Root namespace
- `BlazorWalletConnect.Services` - Core services
- `BlazorWalletConnect.Models` - DTOs and models
- `BlazorWalletConnect.Events` - Event arguments
- `BlazorWalletConnect.Components` - Razor components
- `BlazorWalletConnect.Helpers` - Utility helpers

### Common Commands
```bash
dotnet restore                    # Restore packages
dotnet build                      # Build solution
dotnet test                       # Run tests
dotnet pack                       # Create NuGet package
dotnet format                     # Format code
```

---

**Note**: Always ensure code is well-tested, documented, and follows the established patterns before committing.
