# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2025-11-10

### Initial Release

**BlazorWinForms** - A production-ready framework for building hybrid desktop applications combining Windows Forms with Blazor.

#### BlazorWinForms.Sdk Library

**Integration Patterns**:
- `HybridFormBase` - Inheritance-based pattern for rapid development
- `HybridFormController` - Composition-based pattern for maximum flexibility
- `HybridComponentBase` - Blazor component integration via inheritance
- `HybridComponentController` - Blazor component integration via composition

**Core Features**:
- **Type-Safe Communication** - Full compile-time checking for Blazor ↔ WinForms interop
- **Request/Response Pattern** - `IRequest<TResult>` and `IRequestHandler<,>` for Blazor → WinForms
- **Pub/Sub Events** - `IEvent` and `IEventHandler<>` for WinForms → Blazor
- **Auto-Discovery** - Automatic handler registration via reflection
- **Result Wrapper** - Unified error handling across boundaries

**Bridge Infrastructure**:
- `AppBridge` - COM host object for JavaScript interop
- `RequestService` - Blazor service for sending requests to WinForms
- `EventBridgeService` - Blazor service for receiving events from WinForms
- `WebViewEventRelay` - Relays events from WinForms to WebView2

#### Demo Application

**Features**:
- **4 Built-in Themes** - Light, Light Blue, Dark, Dark Purple
- **Windows 11 Effects** - Native Mica and Acrylic backdrops via P/Invoke
- **Centralized Theme Management** - Single subscription updates all open windows automatically
- **Multiple Forms** - MainForm and SecondForm demonstrating different patterns
- **Type Safety Examples** - TypeSafetyDemo.cs showcasing compile-time checking
- **DevTools Support** - F12 keyboard shortcut for debugging

**Architecture**:
- Clean separation between WinForms and Blazor layers
- SOLID principles with dependency injection
- DRY principle - zero code duplication between patterns
- Convention-based component discovery (e.g., MainForm → Main.razor)

#### Technical Specifications

- **Target Framework**: .NET 9.0
- **Platform**: Windows 10/11 (x64)
- **UI Framework**: Blazor WebView (Microsoft.AspNetCore.Components.WebView.WindowsForms)
- **WebView Engine**: WebView2 (Microsoft Edge Chromium)
- **JSON Serialization**: System.Text.Json

#### Documentation

- [README.md](./README.md) - Comprehensive project documentation with quick start guide
- [CLAUDE.md](./CLAUDE.md) - Developer guide with architecture details and best practices

---

## Links

- **Documentation**: [README.md](./README.md)
- **Developer Guide**: [CLAUDE.md](./CLAUDE.md)

---

[1.0.0]: https://github.com/yourusername/WinFormsBlazor/releases/tag/v1.0.0
