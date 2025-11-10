using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BlazorWinForms.Bridge;
using BlazorWinForms.Interop;

namespace BlazorWinForms.Components;

/// <summary>
/// Controller for Blazor components in hybrid apps using composition.
/// Alternative to inheriting from HybridComponentBase.
/// </summary>
public sealed class HybridComponentController : IDisposable
{
    private readonly RequestService _requestService;
    private readonly EventBridgeService _eventBridge;
    private readonly IJSRuntime _jsRuntime;
    private readonly List<IDisposable> _subscriptions = new();
    private bool _initialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="HybridComponentController"/> class.
    /// Inject these services via dependency injection in your component.
    /// </summary>
    /// <param name="requestService">The request service for sending requests to the WinForms host.</param>
    /// <param name="eventBridge">The event bridge service for receiving events from the WinForms host.</param>
    /// <param name="jsRuntime">The JavaScript runtime for custom JavaScript interop.</param>
    public HybridComponentController(
        RequestService requestService,
        EventBridgeService eventBridge,
        IJSRuntime jsRuntime)
    {
        _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
        _eventBridge = eventBridge ?? throw new ArgumentNullException(nameof(eventBridge));
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    /// <summary>
    /// Initializes the controller. Call this in OnAfterRenderAsync(firstRender).
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _initialized = true;
        await _eventBridge.InitializeAsync();
    }

    /// <summary>
    /// Subscribe to an event from WinForms. Auto-disposed on controller disposal.
    /// </summary>
    public void SubscribeToEvent<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        var subscription = _eventBridge.On(handler);
        _subscriptions.Add(subscription);
    }

    /// <summary>
    /// Subscribe to an event from WinForms (async). Auto-disposed on controller disposal.
    /// </summary>
    public void SubscribeToEvent<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        var subscription = _eventBridge.On(handler);
        _subscriptions.Add(subscription);
    }

    /// <summary>
    /// Send a request to WinForms.
    /// </summary>
    public Task<Result<TResult>> SendRequest<TResult>(IRequest<TResult> request)
    {
        return _requestService.SendAsync(request);
    }

    /// <summary>
    /// Gets the IJSRuntime for custom JavaScript interop.
    /// </summary>
    public IJSRuntime JSRuntime => _jsRuntime;

    /// <summary>
    /// Disposes the controller and cleans up all event subscriptions.
    /// Call this method when the component is disposed to prevent memory leaks.
    /// </summary>
    public void Dispose()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription?.Dispose();
        }
        _subscriptions.Clear();
    }
}
