namespace BlazorWinForms.Interop;

/// <summary>
/// Marker interface for requests that return a result of type TResult.
/// Requests represent both queries (read operations) and commands (write operations)
/// sent from Blazor to the WinForms host.
/// </summary>
public interface IRequest<TResult> { }

/// <summary>
/// Handler interface for processing requests.
/// Implement this interface to handle requests sent from Blazor to the WinForms host.
/// </summary>
public interface IRequestHandler<in TRequest, TResult> where TRequest : IRequest<TResult>
{
    /// <summary>
    /// Handles a request asynchronously and returns the result.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The result of the request.</returns>
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Marker interface for events.
/// Events represent things that have happened in the system and are published
/// from the WinForms host to Blazor components.
/// </summary>
public interface IEvent { }

/// <summary>
/// Handler interface for processing events.
/// Supports both synchronous (Handle) and asynchronous (HandleAsync) processing.
/// </summary>
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    /// <summary>
    /// Handles an event synchronously. Override this for synchronous event processing.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    void Handle(TEvent @event) { }

    /// <summary>
    /// Handles an event asynchronously. Override this for asynchronous event processing.
    /// </summary>
    /// <param name="event">The event to handle.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default) => Task.CompletedTask;
}
