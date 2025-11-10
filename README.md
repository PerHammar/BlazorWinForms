# WinForms + Blazor Hybrid Application

> A production-ready framework for building hybrid desktop applications combining Windows Forms with Blazor WebView2, featuring strongly-typed interop and full compile-time type safety.

[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Windows](https://img.shields.io/badge/platform-Windows-blue.svg)](https://www.microsoft.com/windows)

## 📋 Table of Contents

- [Overview](#overview)
- [BlazorWinForms.Sdk Library](#blazorwinformssdk-library)
- [Features](#features)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [How It Works](#how-it-works)
- [Usage Examples](#usage-examples)
- [Project Structure](#project-structure)
- [API Reference](#api-reference)
- [Best Practices](#best-practices)
- [Performance](#performance)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

## Overview

This project demonstrates **BlazorWinForms.Sdk**, a reusable library for building **hybrid desktop applications** that combine the native power of Windows Forms with the modern UI capabilities of Blazor, all while maintaining full compile-time type safety across the host-guest boundary.

**What's Included:**
- 📦 **BlazorWinForms.Sdk**: Production-ready, NuGet-ready library for WinForms + Blazor integration
- 🎨 **WinFormsBlazor Demo**: Reference implementation showcasing the library with theming and multiple forms

### Why This Approach?

**Traditional Challenges:**
- ❌ String-based message passing loses type safety
- ❌ JavaScript interop requires runtime type checking
- ❌ Refactoring breaks communication at runtime
- ❌ No IntelliSense across boundaries

**Our Solution:**
- ✅ Full C# type safety from WinForms to Blazor
- ✅ IntelliSense everywhere
- ✅ Compile-time error checking
- ✅ Automatic handler discovery
- ✅ Clean, maintainable architecture

## BlazorWinForms.Sdk Library

**BlazorWinForms.Sdk** is a production-ready library that provides:

### Library Features

- 🎯 **Dual Integration Patterns**: Choose **Inheritance** (`HybridFormBase`) or **Composition** (`HybridFormController`)
- 🔌 **Dual Component Patterns**: Choose **Inheritance** (`HybridComponentBase`) or **Composition** (`HybridComponentController`)
- 🔒 **Complete Type Safety**: All interop is compile-time checked using `IRequest<TResult>` and `IEvent`
- 🔍 **Auto-Discovery**: Handlers automatically registered via reflection
- 📦 **Modern .NET**: Built for .NET 9.0
- ⚙️ **Configurable**: Namespace and component discovery can be customized
- 🧹 **Clean**: Zero dependencies on demo-specific features (like theming)

### Quick Start with Library

```csharp
// 1. Reference the library
<ItemGroup>
  <ProjectReference Include="path/to/BlazorWinForms.Sdk/BlazorWinForms.Sdk.csproj" />
</ItemGroup>

// 2. Create a hybrid form (Inheritance pattern)
using BlazorWinForms.Forms;

public partial class MainForm : HybridFormBase
{
    public MainForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        InitializeHybridForm();  // That's it!
    }

    protected override string GetComponentNamespace() => "MyApp.Web.Pages";
}

// 3. Define requests/events in your app
using BlazorWinForms.Interop;

public record GetData(string Key) : IRequest<string>;
public record DataChanged(string Value) : IEvent;

// 4. Implement handlers - auto-discovered!
public class GetDataHandler : IRequestHandler<GetData, string>
{
    public Task<string> HandleAsync(GetData request, CancellationToken ct)
        => Task.FromResult($"Value for {request.Key}");
}
```

See the **WinFormsBlazor** demo project for complete examples including theming, multiple forms, and advanced patterns.

## Features

### 🎨 Modern UI
- **Blazor Components**: Modern, reactive web UI components
- **4 Built-in Themes**: Light, Light Blue, Dark, Dark Purple
- **Windows 11 Effects**: Native Mica and Acrylic backdrops
- **Responsive Design**: Adapts to different window sizes

### 🔗 Strongly-Typed Interop
- **Type-Safe Requests**: Blazor → WinForms communication with compile-time checking
- **Type-Safe Events**: WinForms → Blazor pub/sub with strong typing
- **IntelliSense Support**: Full autocomplete for all operations
- **Refactor-Safe**: Rename properties without breaking at runtime

### 🏗️ Clean Architecture
- **Separation of Concerns**: Clear boundaries between layers
- **SOLID Principles**: Dependency injection, interface segregation
- **Flexible Integration**: Choose **Inheritance** (`HybridFormBase`) or **Composition** (`HybridFormController`)
- **DRY Principle**: Zero code duplication between patterns
- **Auto-Discovery**: Handlers automatically registered via reflection

### 🚀 Production Ready
- **Error Handling**: Comprehensive error handling with `Result<T>` wrapper
- **Async/Await**: Fully asynchronous communication
- **Cancellation Support**: Proper cancellation token support
- **Memory Management**: Automatic subscription disposal

## Architecture

### High-Level Overview

```
┌─────────────────────────────────────────────────────────────┐
│                     WinForms Host                           │
│  ┌──────────────┐         ┌──────────────┐                  │
│  │              │         │              │                  │
│  │ MainForm     │         │  Theming     │                  │
│  │              │         │  Manager     │                  │
│  └──────┬───────┘         └──────────────┘                  │
│         │                                                   │
│         │  ┌─────────────────────────────────┐              │
│         └──│    RequestDispatcher            │              │
│            │  (Handles incoming requests)    │              │
│            └─────────────┬───────────────────┘              │
│                          │                                  │
│         ┌────────────────┴──────────────┐                   │
│         │                               │                   │
│    ┌────▼─────┐                  ┌──────▼──────┐            │
│    │ Request  │                  │   Request   │            │
│    │ Handler  │                  │   Handler   │            │
│    │    #1    │                  │     #2      │            │
│    └──────────┘                  └─────────────┘            │
│                                                             │
│         ┌─────────────────────────────────┐                 │
│         │       EventBus                  │                 │
│         │  (Publishes events to Blazor)   │                 │
│         └─────────────┬───────────────────┘                 │
└───────────────────────┼─────────────────────────────────────┘
                        │
                        │ WebView2 Boundary
                        │ (JSON over JavaScript)
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                    Blazor Guest                             │
│         ┌─────────────────────────────────┐                 │
│         │   EventBridgeService            │                 │
│         │  (Receives events from host)    │                 │
│         └─────────────┬───────────────────┘                 │
│                       │                                     │
│         ┌─────────────▼───────────────┐                     │
│         │   Blazor Components         │                     │
│         │  (UI + Business Logic)      │                     │
│         └─────────────┬───────────────┘                     │
│                       │                                     │
│         ┌─────────────▼───────────────┐                     │
│         │    RequestService           │                     │
│         │  (Sends requests to host)   │                     │
│         └─────────────────────────────┘                     │
└─────────────────────────────────────────────────────────────┘
```

### Communication Patterns

#### 1. Request/Response (Blazor → WinForms)

**Use Case**: Blazor needs data or wants to trigger an action in WinForms

```csharp
// 1. Define the request
public record GetUserSettings() : IRequest<UserSettings>;

// 2. Implement the handler (WinForms side)
public class GetUserSettingsHandler : IRequestHandler<GetUserSettings, UserSettings>
{
    public Task<UserSettings> HandleAsync(GetUserSettings request, CancellationToken ct)
    {
        return Task.FromResult(new UserSettings(
            Theme: ThemeManager.Current.Name,
            NotificationsEnabled: true
        ));
    }
}

// 3. Send from Blazor
var result = await RequestService.SendAsync(new GetUserSettings());
if (result.Success && result.Data != null)
{
    // Use the data
    Console.WriteLine($"Current theme: {result.Data.Theme}");
}
```

**Flow:**
```
Blazor Component
  → RequestService.SendAsync(request)
    → JavaScript: appBridge.send(json, typeName)
      → WebView2 Host Object (COM)
        → AppBridge.Send(json, typeName)
          → RequestDispatcher.SendAsync(request)
            → Handler.HandleAsync(request)
              → Result<T> serialized to JSON
                → Returns to Blazor
```

#### 2. Pub/Sub Events (WinForms → Blazor)

**Use Case**: WinForms needs to notify Blazor about state changes

```csharp
// 1. Define the event
public record ThemeChanged(string ThemeName) : IEvent;

// 2. Publish from WinForms
await EventBus.PublishAsync(new ThemeChanged("Dark"));

// 3. Subscribe in Blazor
protected override async Task OnInitializedAsync()
{
    await EventBridge.InitializeAsync();

    SubscribeToEvent<ThemeChanged>(async e =>
    {
        Console.WriteLine($"Theme changed to: {e.ThemeName}");
        await ApplyTheme(e.ThemeName);
        StateHasChanged();
    });
}
```

**Flow:**
```
WinForms Code
  → EventBus.PublishAsync(event)
    → WebViewEventRelay.SendAsync(event)
      → CoreWebView2.PostWebMessageAsString(json)
        → JavaScript: chrome.webview.message
          → EventBridge.receiveMessage(message)
            → EventBridgeService.OnHostEvent(json)
              → Registered handlers invoked
```

## Getting Started

### Prerequisites

- **[.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)** or later
- **Windows 10/11** (WinForms and WebView2 are Windows-only)
- **WebView2 Runtime** (pre-installed on Windows 11, [download for Windows 10](https://developer.microsoft.com/microsoft-edge/webview2/))

### Installation

```bash
# Clone the repository
git clone https://github.com/yourusername/WinFormsBlazor.git
cd WinFormsBlazor

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

### Quick Start: Create Your First Hybrid Form

**Option 1: Inheritance Pattern** (less code, convention-based)

```csharp
// 1. Create a form inheriting from HybridFormBase
public partial class MyForm : HybridFormBase
{
    public MyForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        Text = "My Hybrid App";
        Size = new Size(1200, 800);

        InitializeHybridForm(); // Convention: MyForm → My.razor
    }
}
```

**Option 2: Composition Pattern** (explicit, flexible)

```csharp
// 1. Create a regular Form using composition
public partial class MyForm : Form
{
    private readonly HybridFormController _hybrid;

    public MyForm()
    {
        InitializeComponent();

        _hybrid = new HybridFormController(this)
            .WithComponent<Web.Pages.My>()
            .Initialize();

        Text = "My Hybrid App";
        Size = new Size(1200, 800);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _hybrid?.Dispose();
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
```

**2. Create a Blazor component (same for both patterns)**

```razor
@page "/"
@namespace MyApp.Web.Pages
@inherits HybridComponentBase

<h1>Hello from Blazor!</h1>
<button @onclick="RequestData">Get Data from WinForms</button>

@code {
    private async Task RequestData()
    {
        var result = await SendRequest(new GetMyData());
        // Use the data...
    }
}
```

## How It Works

### The Magic: Type-Safe Cross-Boundary Communication

#### Problem: The WebView2 Boundary

WebView2 creates a **process boundary** between WinForms (.NET) and Blazor (also .NET, but running in a Chromium context). Traditional approaches lose type safety at this boundary.

#### Our Solution: Strongly-Typed JSON Bridge

We serialize **strongly-typed C# objects** to JSON, send them across the boundary, then deserialize them back to **the same strongly-typed C# objects**.

**Key Innovation**: We preserve the **assembly-qualified type name** with the JSON payload:

```csharp
// On the Blazor side:
var json = JsonSerializer.Serialize(request, request.GetType());
var typeName = request.GetType().AssemblyQualifiedName; // Key!

// Send both to WinForms
await jsRuntime.InvokeAsync("appBridge.send", json, typeName);

// On the WinForms side:
var requestType = Type.GetType(typeName); // Restore the exact type!
var request = JsonSerializer.Deserialize(json, requestType);
```

This allows us to:
1. **Compile-time checking** on both sides
2. **IntelliSense** for all properties
3. **Refactor safety** - rename a property, all usages update

### Automatic Handler Discovery

Handlers are discovered **automatically** at startup via reflection:

```csharp
public class RequestDispatcher
{
    private readonly Dictionary<Type, object> _handlers = new();

    public RequestDispatcher(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            // Find all classes implementing IRequestHandler<,>
            var handlerTypes = assembly.GetTypes()
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                             i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

            foreach (var handlerType in handlerTypes)
            {
                var requestType = /* extract from interface */;
                _handlers[requestType] = Activator.CreateInstance(handlerType);
            }
        }
    }
}
```

**Benefits**:
- No manual registration needed
- Just implement the interface
- Handlers are found automatically

### The Base Class Pattern

Instead of repeating setup code, inherit from `HybridFormBase`:

```csharp
public abstract class HybridFormBase : Form
{
    protected RequestDispatcher RequestDispatcher { get; private set; }
    protected EventBus EventBus { get; private set; }
    protected BlazorWebView BlazorWebView { get; private set; }

    protected void InitializeHybridForm()
    {
        // 1. Create request dispatcher
        RequestDispatcher = new RequestDispatcher(GetType().Assembly);

        // 2. Setup Blazor WebView
        BlazorWebView = new BlazorWebView { /* config */ };

        // 3. Configure DI services
        services.AddScoped<RequestService>();
        services.AddScoped<EventBridgeService>();

        // 4. Inject host object for JavaScript interop
        webView.AddHostObjectToScript("appBridge", new AppBridge(RequestDispatcher));

        // 5. Create event bus with relay
        EventBus = new EventBus(new WebViewEventRelay(webView));
    }
}
```

**Saves ~100 lines of boilerplate per form!**

### Windows 11 Visual Effects

We use P/Invoke to apply native Windows 11 effects:

```csharp
public static class Win11Effects
{
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    public static void SetBackdropType(IntPtr handle, BackdropType type)
    {
        int backdropValue = (int)type;
        DwmSetWindowAttribute(handle, 38, ref backdropValue, sizeof(int));
    }

    public enum BackdropType
    {
        None = 1,
        Mica = 2,      // Subtle, translucent material
        Acrylic = 3,   // Frosted glass effect
        Tabbed = 4
    }
}
```

## Usage Examples

### Example 1: Requesting Data from WinForms

```csharp
// Define the request
public record GetWeatherData(string City) : IRequest<WeatherInfo>;

// Implement the handler
public class GetWeatherDataHandler : IRequestHandler<GetWeatherData, WeatherInfo>
{
    public async Task<WeatherInfo> HandleAsync(GetWeatherData request, CancellationToken ct)
    {
        // Call your weather API, database, etc.
        var data = await _weatherService.GetWeatherAsync(request.City);
        return new WeatherInfo(data.Temperature, data.Conditions);
    }
}

// Use from Blazor
@code {
    private async Task LoadWeather()
    {
        var result = await SendRequest(new GetWeatherData("Seattle"));

        if (result.Success && result.Data != null)
        {
            temperature = result.Data.Temperature;
            conditions = result.Data.Conditions;
            StateHasChanged();
        }
        else
        {
            // Handle error
            errorMessage = result.Error;
        }
    }
}
```

### Example 2: Opening a Native File Dialog

```csharp
// Request
public record OpenFileDialog(string Filter) : IRequest<string?>;

// Handler
public class OpenFileDialogHandler : IRequestHandler<OpenFileDialog, string?>
{
    public Task<string?> HandleAsync(OpenFileDialog request, CancellationToken ct)
    {
        using var dialog = new System.Windows.Forms.OpenFileDialog
        {
            Filter = request.Filter
        };

        return Task.FromResult(
            dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null
        );
    }
}

// Blazor usage
var result = await SendRequest(new OpenFileDialog("Text Files|*.txt|All Files|*.*"));
if (result.Success && result.Data != null)
{
    selectedFile = result.Data;
}
```

### Example 3: Real-Time Progress Updates

```csharp
// Event
public record ProgressUpdated(int Percentage, string Message) : IEvent;

// WinForms publishes during long operation
private async Task ProcessLargeFile()
{
    for (int i = 0; i <= 100; i += 10)
    {
        // Do work...
        await Task.Delay(500);

        // Notify Blazor
        await EventBus.PublishAsync(new ProgressUpdated(i, $"Processing... {i}%"));
    }
}

// Blazor subscribes and updates UI
@code {
    private int progress = 0;
    private string statusMessage = "";

    protected override async Task OnInitializedAsync()
    {
        await EventBridge.InitializeAsync();

        SubscribeToEvent<ProgressUpdated>(e =>
        {
            progress = e.Percentage;
            statusMessage = e.Message;
            StateHasChanged();
        });
    }
}
```

## Project Structure

```
WinFormsBlazor/
├── BlazorWinForms.Sdk/         # Reusable SDK library
│   ├── Interop/                # Core interop infrastructure
│   │   ├── Interfaces.cs      # IRequest<T>, IEvent, handler interfaces
│   │   ├── RequestDispatcher.cs # Routes requests to handlers
│   │   ├── EventBus.cs        # Publishes events to subscribers
│   │   └── Result.cs          # Result<T> wrapper for error handling
│   │
│   ├── Bridge/                 # Communication bridge layer
│   │   ├── AppBridge.cs       # Host object exposed to JavaScript
│   │   ├── RequestService.cs  # Blazor service for sending requests
│   │   ├── EventBridgeService.cs # Blazor service for receiving events
│   │   ├── WebViewEventRelay.cs  # Relays events to WebView2
│   │   └── BridgeJavaScript.cs   # JavaScript interop code
│   │
│   └── Forms/                  # Form integration
│       ├── HybridFormBase.cs  # Inheritance pattern
│       ├── HybridFormController.cs # Composition pattern
│       ├── HybridComponentBase.cs  # Blazor component base
│       └── HybridComponentController.cs
│
└── WinFormsBlazor.Demo/        # Demo application
    ├── Requests/               # Request definitions
    │   ├── GetUserSettings.cs
    │   ├── ChangeTheme.cs
    │   └── Handlers/          # Request handlers
    │       ├── GetUserSettingsHandler.cs
    │       └── ChangeThemeHandler.cs
    │
    ├── Events/                 # Event definitions
    │   ├── ThemeChanged.cs
    │   └── FormButtonClicked.cs
    │
    ├── Theming/                # Theme management
    │   ├── Theme.cs           # Theme data model
    │   ├── ThemeManager.cs    # Centralized theme state
    │   └── Win11Effects.cs    # P/Invoke for Windows 11 effects
    │
    ├── Web/                    # Blazor components
    │   ├── Pages/
    │   │   ├── Main.razor     # Main page component
    │   │   └── Second.razor   # Secondary page
    │   ├── Shared/
    │   │   └── MainLayout.razor # Layout component
    │   └── wwwroot/           # Static web assets
    │       ├── css/
    │       ├── js/
    │       └── index.html
    │
    ├── Examples/               # Example code
    │   └── TypeSafetyDemo.cs  # Type safety demonstrations
    │
    ├── FormController.cs       # Centralized form management
    ├── MainForm.cs             # Main application form
    ├── SecondForm.cs           # Secondary demo form
    └── Program.cs              # Application entry point
```

## API Reference

### Core Interfaces

#### `IRequest<TResult>`
Marker interface for requests that return `TResult`.

```csharp
public interface IRequest<TResult> { }
```

**Example:**
```csharp
public record GetUserName(int UserId) : IRequest<string>;
```

#### `IRequestHandler<TRequest, TResult>`
Interface for handling requests.

```csharp
public interface IRequestHandler<in TRequest, TResult>
    where TRequest : IRequest<TResult>
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
```

**Example:**
```csharp
public class GetUserNameHandler : IRequestHandler<GetUserName, string>
{
    public Task<string> HandleAsync(GetUserName request, CancellationToken ct)
    {
        var name = _userRepository.GetName(request.UserId);
        return Task.FromResult(name);
    }
}
```

#### `IEvent`
Marker interface for events.

```csharp
public interface IEvent { }
```

**Example:**
```csharp
public record UserLoggedIn(string Username, DateTime Timestamp) : IEvent;
```

#### `IEventHandler<TEvent>`
Interface for handling events.

```csharp
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    void Handle(TEvent @event) { }
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
```

### RequestDispatcher

Routes requests to appropriate handlers.

```csharp
public class RequestDispatcher
{
    // Constructor - discovers handlers from assemblies
    public RequestDispatcher(params Assembly[]? assemblies);

    // Send a request and get a result
    public Task<Result<TResult>> SendAsync<TResult>(
        IRequest<TResult> request,
        CancellationToken cancellationToken = default);
}
```

### EventBus

Publishes events to subscribers and relays to WebView.

```csharp
public class EventBus
{
    // Constructor
    public EventBus(WebViewEventRelay? relay = null, params Assembly[]? assemblies);

    // Publish an event
    public Task PublishAsync<TEvent>(
        TEvent @event,
        CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}
```

### Result<T>

Wraps operation results with success/error handling.

```csharp
public record Result<T>(bool Success, string? Error, T? Data)
{
    public static Result<T> Ok(T data);
    public static Result<T> Fail(string error);
}
```

**Usage:**
```csharp
var result = await SendRequest(new GetData());
if (result.Success)
{
    // Use result.Data
}
else
{
    // Handle result.Error
}
```

### HybridComponentBase

Base class for Blazor components in hybrid apps.

```csharp
public abstract class HybridComponentBase : ComponentBase, IDisposable
{
    [Inject] protected RequestService RequestService { get; set; }
    [Inject] protected EventBridgeService EventBridge { get; set; }

    // Send a request to WinForms
    protected Task<Result<TResult>> SendRequest<TResult>(IRequest<TResult> request);

    // Subscribe to an event (auto-disposed)
    protected void SubscribeToEvent<TEvent>(Action<TEvent> handler)
        where TEvent : IEvent;

    // Subscribe to an event with async handler
    protected void SubscribeToEvent<TEvent>(Func<TEvent, Task> handler)
        where TEvent : IEvent;

    // Called after initialization
    protected virtual Task OnHybridInitializedAsync();
}
```

### HybridFormBase

Base class for hybrid forms using **inheritance pattern**.

```csharp
public abstract class HybridFormBase : Form
{
    protected RequestDispatcher? RequestDispatcher { get; }
    protected EventBus? EventBus { get; }
    protected BlazorWebView? BlazorWebView { get; }

    // Initialize the form (call after InitializeComponent)
    protected void InitializeHybridForm();

    // Override to configure services
    protected virtual void ConfigureServices(ServiceCollection services);

    // Override for WebView2 initialization
    protected virtual void OnWebView2Ready(CoreWebView2 webView);
    protected virtual void OnWebView2InitializationFailed(Exception? exception);

    // Override for custom event handling
    protected virtual void OnCustomEvent();
}
```

**Example:**
```csharp
public partial class MyForm : HybridFormBase
{
    public MyForm()
    {
        InitializeComponent();
        if (DesignMode) return;

        InitializeHybridForm(); // Convention: MyForm → My.razor
    }

    protected override void ConfigureServices(ServiceCollection services)
    {
        services.AddSingleton<IMyService, MyService>();
    }
}
```

### HybridFormController

Controller for hybrid forms using **composition pattern**.

```csharp
public sealed class HybridFormController : IDisposable
{
    public RequestDispatcher? RequestDispatcher { get; }
    public EventBus? EventBus { get; }
    public BlazorWebView? BlazorWebView { get; }

    // Constructor with configuration callback
    public HybridFormController(Form form, Action<HybridFormConfiguration>? configure = null);

    // Initialize (must call after construction)
    public void Initialize();
}
```

**Example:**
```csharp
public partial class MyForm : Form
{
    private readonly HybridFormController _hybrid;

    public MyForm()
    {
        InitializeComponent();

        _hybrid = new HybridFormController(this, config =>
        {
            config.WithComponent<Web.Pages.Index>();
            config.ConfigureServices(services =>
            {
                services.AddSingleton<IMyService, MyService>();
            });
        });

        _hybrid.Initialize();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _hybrid?.Dispose();
        base.Dispose(disposing);
    }
}
```

## Best Practices

### 1. Choosing Between Inheritance and Composition

**Use Inheritance (`HybridFormBase`)** when:
- Starting a new project from scratch
- You control the form hierarchy
- You want the least amount of code
- Convention-based discovery fits your needs
- You prefer virtual method overrides

**Use Composition (`HybridFormController`)** when:
- Working with existing forms (already inheriting from another base)
- You need explicit control over configuration
- Your codebase avoids deep inheritance hierarchies
- You prefer fluent/builder patterns
- You want maximum flexibility

**Both patterns:**
- Share the same core logic (DRY)
- Provide identical functionality
- Support the same customization hooks
- Are fully production-ready

### 2. Request Naming Conventions

- **Queries** (read data): Start with `Get`, `Find`, `Search`, `List`
  ```csharp
  GetUserSettings, FindDocuments, SearchProducts
  ```

- **Commands** (modify state): Use verbs
  ```csharp
  CreateUser, UpdateSettings, DeleteItem, ChangeTheme
  ```

### 3. Error Handling

Always check `Result.Success` before using data:

```csharp
var result = await SendRequest(new GetData());
if (!result.Success)
{
    logger.LogError($"Request failed: {result.Error}");
    ShowErrorMessage(result.Error);
    return;
}

// Safe to use result.Data
ProcessData(result.Data);
```

### 4. Event Subscriptions

Always use `SubscribeToEvent` in `HybridComponentBase` for automatic disposal:

```csharp
// ✅ Good - auto-disposed
protected override async Task OnHybridInitializedAsync()
{
    SubscribeToEvent<MyEvent>(HandleMyEvent);
}

// ❌ Bad - manual disposal required
var subscription = EventBridge.On<MyEvent>(HandleMyEvent);
```

### 5. Cancellation Tokens

Pass cancellation tokens for long-running operations:

```csharp
public async Task<LargeData> HandleAsync(
    GetLargeData request,
    CancellationToken cancellationToken)
{
    var data = await _repository.GetDataAsync(cancellationToken);
    cancellationToken.ThrowIfCancellationRequested();
    return ProcessData(data);
}
```

### 6. Form Initialization

Always check `DesignMode` to prevent designer crashes:

```csharp
public MyForm()
{
    InitializeComponent();

    // ✅ Good - prevents designer issues
    if (DesignMode) return;

    InitializeHybridForm();
}
```

## Performance

### Benchmarks

Typical operation times on a modern PC:

| Operation | Time |
|-----------|------|
| Simple request (GetUserSettings) | < 1ms |
| Complex request with DB query | 5-20ms |
| Event publish (local handlers) | < 1ms |
| Event relay to Blazor | 2-5ms |
| Handler discovery (startup) | 50-100ms |

### Optimization Tips

**1. Minimize Cross-Boundary Calls**

```csharp
// ❌ Bad - multiple round-trips
var theme = await SendRequest(new GetTheme());
var settings = await SendRequest(new GetSettings());
var user = await SendRequest(new GetUser());

// ✅ Good - single request
var data = await SendRequest(new GetAllAppData());
```

**2. Batch Event Updates**

```csharp
// ❌ Bad - many small events
for (int i = 0; i < 100; i++)
{
    await EventBus.PublishAsync(new ProgressChanged(i));
}

// ✅ Good - throttle updates
for (int i = 0; i < 100; i++)
{
    if (i % 10 == 0) // Every 10%
        await EventBus.PublishAsync(new ProgressChanged(i));
}
```

**3. Use Records for DTOs**

Records are optimized for this pattern:

```csharp
// ✅ Efficient, immutable, value-based equality
public record UserData(string Name, int Age, string Email);
```

## Troubleshooting

### Issue: "No handler registered for X"

**Cause**: Handler not discovered by reflection

**Solutions**:
1. Ensure handler implements `IRequestHandler<,>` correctly
2. Ensure handler has a parameterless constructor
3. Ensure handler is in the application assembly (automatically registered)

```csharp
// ✅ Correct
public class MyHandler : IRequestHandler<MyRequest, string>
{
    public MyHandler() { } // Parameterless constructor

    public Task<string> HandleAsync(MyRequest request, CancellationToken ct)
    {
        return Task.FromResult("Result");
    }
}
```

### Issue: WebView2 not loading

**Cause**: WebView2 Runtime not installed or initialization failure

**Solutions**:
1. Install [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/)
2. Check `OnWebView2InitializationFailed` for error details
3. Ensure `wwwroot/index.html` exists in the output directory

### Issue: Type not found during deserialization

**Cause**: Type moved or renamed after compilation

**Solutions**:
1. Clean and rebuild: `dotnet clean && dotnet build`
2. Ensure both projects are up-to-date
3. Check that namespaces match

### Issue: Events not received in Blazor

**Cause**: EventBridge not initialized

**Solution**:
```csharp
protected override async Task OnInitializedAsync()
{
    // ✅ Must call this!
    await EventBridge.InitializeAsync();

    SubscribeToEvent<MyEvent>(HandleEvent);
}
```

### Issue: Designer fails to load forms

**Cause**: Missing DesignMode check

**Solution**:
```csharp
public MyForm()
{
    InitializeComponent();

    // ✅ Essential for designer support
    if (DesignMode) return;

    InitializeHybridForm();
}
```

## Contributing

Contributions are welcome! Please follow these guidelines:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Follow** the existing code style
4. **Add** tests if applicable
5. **Update** documentation
6. **Commit** your changes (`git commit -m 'Add amazing feature'`)
7. **Push** to the branch (`git push origin feature/amazing-feature`)
8. **Open** a Pull Request

### Code Style

- Use C# 12 features (records, pattern matching, etc.)
- Follow [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Add XML documentation comments for public APIs
- Use meaningful variable names

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Acknowledgments

- Built with [.NET 9.0](https://dotnet.microsoft.com/)
- Uses [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) for modern UI
- Powered by [WebView2](https://developer.microsoft.com/microsoft-edge/webview2/)
- Inspired by clean architecture principles and CQRS patterns

## Support

- **Issues**: Report bugs or request features via GitHub Issues
- **Discussions**: Share ideas and ask questions via GitHub Discussions

---

**Made with ❤️ for the .NET community**

