using System.Reflection;
using BlazorWinForms.Bridge;

namespace BlazorWinForms.Interop;

/// <summary>
/// Central dispatcher for events.
/// Auto-discovers event handlers via reflection and publishes events to all registered handlers.
/// Can optionally relay events to WebView via WebViewEventRelay.
/// </summary>
public sealed class EventBus
{
    private readonly Dictionary<Type, List<object>> _handlers = new();
    private readonly WebViewEventRelay? _relay;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventBus"/> class.
    /// Automatically discovers and registers all event handlers in the specified assemblies.
    /// </summary>
    /// <param name="relay">Optional WebView relay for sending events to Blazor.</param>
    /// <param name="assemblies">Assemblies to scan for event handlers. If null, scans the executing assembly.</param>
    public EventBus(WebViewEventRelay? relay = null, params Assembly[]? assemblies)
    {
        _relay = relay;
        assemblies ??= [Assembly.GetExecutingAssembly()];

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                // Skip abstract classes and interfaces
                if (type.IsAbstract || type.IsInterface)
                    continue;

                var handlerInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType &&
                                        i.GetGenericTypeDefinition() == typeof(IEventHandler<>));

                if (handlerInterface == null)
                    continue;

                var eventType = handlerInterface.GetGenericArguments()[0];

                if (!_handlers.ContainsKey(eventType))
                    _handlers[eventType] = new List<object>();

                try
                {
                    _handlers[eventType].Add(Activator.CreateInstance(type)!);
                }
                catch
                {
                    // Skip types that can't be instantiated
                }
            }
        }
    }

    /// <summary>
    /// Publishes an event to all registered handlers and optionally relays it to Blazor via WebView.
    /// Invokes both synchronous (Handle) and asynchronous (HandleAsync) handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <param name="event">The event instance to publish.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        // Invoke local handlers
        if (_handlers.TryGetValue(typeof(TEvent), out var handlers))
        {
            foreach (var handler in handlers)
            {
                if (handler is IEventHandler<TEvent> typedHandler)
                {
                    // Call HandleAsync which should internally call Handle if needed
                    await typedHandler.HandleAsync(@event, cancellationToken);
                }
            }
        }

        // Relay to WebView if configured
        if (_relay != null && !cancellationToken.IsCancellationRequested)
            await _relay.SendAsync(@event, cancellationToken);
    }
}
