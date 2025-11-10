using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.JSInterop;
using BlazorWinForms.Interop;

namespace BlazorWinForms.Bridge;

/// <summary>
/// Blazor-side service for receiving events from the WinForms host.
/// Registers with JavaScript to receive event notifications.
/// </summary>
public class EventBridgeService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private DotNetObjectReference<EventBridgeService>? _selfReference;
    private readonly ConcurrentDictionary<Type, List<Delegate>> _handlers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBridgeService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for invoking JavaScript interop.</param>
    public EventBridgeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initializes the event bridge by registering this service with JavaScript.
    /// Must be called before events can be received from the host.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual async Task InitializeAsync()
    {
        _selfReference = DotNetObjectReference.Create(this);
        await _jsRuntime.InvokeVoidAsync("eventBridge.register", _selfReference);
    }

    /// <summary>
    /// Called from JavaScript when an event is received from the WinForms host.
    /// Deserializes the event and invokes all registered handlers.
    /// </summary>
    /// <param name="json">The JSON payload containing the event name and data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    [JSInvokable]
    public async Task OnHostEvent(string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var typeName = root.GetProperty("name").GetString();
        var payloadJson = root.GetProperty("payload").GetRawText();

        if (string.IsNullOrEmpty(typeName))
            return;

        var eventType = Type.GetType(typeName, throwOnError: false);
        if (eventType == null)
            return;

        var @event = JsonSerializer.Deserialize(payloadJson, eventType) as IEvent;
        if (@event == null)
            return;

        if (_handlers.TryGetValue(eventType, out var handlerList))
        {
            foreach (var handler in handlerList)
            {
                switch (handler)
                {
                    case Action<IEvent> syncHandler:
                        syncHandler(@event);
                        break;
                    case Func<IEvent, Task> asyncHandler:
                        await asyncHandler(@event);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Subscribes to an event with a synchronous handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to subscribe to.</typeparam>
    /// <param name="handler">The handler to invoke when the event is received.</param>
    /// <returns>An <see cref="IDisposable"/> that unsubscribes the handler when disposed.</returns>
    public IDisposable On<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        return AddHandler(typeof(TEvent), (Action<IEvent>)(@event => handler((TEvent)@event)));
    }

    /// <summary>
    /// Subscribes to an event with an asynchronous handler.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to subscribe to.</typeparam>
    /// <param name="handler">The async handler to invoke when the event is received.</param>
    /// <returns>An <see cref="IDisposable"/> that unsubscribes the handler when disposed.</returns>
    public IDisposable On<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        return AddHandler(typeof(TEvent), (Func<IEvent, Task>)(@event => handler((TEvent)@event)));
    }

    private IDisposable AddHandler(Type eventType, Delegate handler)
    {
        _handlers.TryAdd(eventType, new List<Delegate>());
        _handlers[eventType].Add(handler);

        return new Unsubscriber(_handlers, eventType, handler);
    }

    private sealed record Unsubscriber(
        ConcurrentDictionary<Type, List<Delegate>> Handlers,
        Type EventType,
        Delegate Handler) : IDisposable
    {
        public void Dispose()
        {
            if (Handlers.TryGetValue(EventType, out var list))
                list.Remove(Handler);
        }
    }

    /// <summary>
    /// Disposes the event bridge service and cleans up JavaScript interop references.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        _selfReference?.Dispose();
        return ValueTask.CompletedTask;
    }
}
