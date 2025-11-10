using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorWinForms.Bridge;
using BlazorWinForms.Interop;

namespace BlazorWinForms.Components;

/// <summary>
/// Base class for Blazor components in hybrid apps.
/// Automatically handles RequestService, EventBridge initialization, and cleanup.
/// </summary>
public abstract class HybridComponentBase : ComponentBase, IDisposable
{
    /// <summary>
    /// Gets the request service for sending requests to the WinForms host.
    /// Automatically injected by dependency injection.
    /// </summary>
    [Inject] protected RequestService RequestService { get; set; } = null!;

    /// <summary>
    /// Gets the event bridge service for receiving events from the WinForms host.
    /// Automatically injected by dependency injection.
    /// </summary>
    [Inject] protected EventBridgeService EventBridge { get; set; } = null!;

    /// <summary>
    /// Gets the JavaScript runtime for custom JavaScript interop.
    /// Automatically injected by dependency injection.
    /// </summary>
    [Inject] protected IJSRuntime JSRuntime { get; set; } = null!;

    private readonly List<IDisposable> _subscriptions = new();
    private bool _initialized = false;

    /// <summary>
    /// Called after the component has been rendered. On the first render, initializes the event bridge.
    /// Override this method to perform custom initialization after rendering.
    /// </summary>
    /// <param name="firstRender">True if this is the first time the component is being rendered.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender || _initialized)
            return;

        _initialized = true;
        await EventBridge.InitializeAsync();
        await OnHybridInitializedAsync();
    }

    /// <summary>
    /// Called once after the component is initialized and EventBridge is ready.
    /// Override this to subscribe to events and perform initialization.
    /// </summary>
    protected virtual Task OnHybridInitializedAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Subscribe to an event from WinForms. Auto-disposed on component disposal.
    /// </summary>
    protected void SubscribeToEvent<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        var subscription = EventBridge.On(handler);
        _subscriptions.Add(subscription);
    }

    /// <summary>
    /// Subscribe to an event from WinForms (async). Auto-disposed on component disposal.
    /// </summary>
    protected void SubscribeToEvent<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        var subscription = EventBridge.On(handler);
        _subscriptions.Add(subscription);
    }

    /// <summary>
    /// Send a request to WinForms.
    /// </summary>
    protected Task<Result<TResult>> SendRequest<TResult>(IRequest<TResult> request)
    {
        return RequestService.SendAsync(request);
    }

    /// <summary>
    /// Disposes the component and cleans up all event subscriptions.
    /// Override this method to perform custom cleanup, but be sure to call base.Dispose().
    /// </summary>
    public virtual void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription?.Dispose();
        }
        _subscriptions.Clear();
    }
}
