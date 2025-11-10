using System.Reflection;

namespace BlazorWinForms.Interop;

/// <summary>
/// Central dispatcher for requests from Blazor to WinForms host.
/// Auto-discovers request handlers via reflection and routes requests to appropriate handlers.
/// Handles both queries (read operations) and commands (write operations).
/// </summary>
public class RequestDispatcher
{
    private readonly Dictionary<Type, object> _handlers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestDispatcher"/> class.
    /// Automatically discovers and registers all request handlers in the specified assemblies.
    /// </summary>
    /// <param name="assemblies">Assemblies to scan for request handlers. If null, scans the executing assembly.</param>
    public RequestDispatcher(params Assembly[]? assemblies)
    {
        assemblies ??= [Assembly.GetExecutingAssembly()];

        foreach (var assembly in assemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                var handlerInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType &&
                                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

                if (handlerInterface == null)
                    continue;

                var requestType = handlerInterface.GetGenericArguments()[0];
                _handlers[requestType] = Activator.CreateInstance(type)!;
            }
        }
    }

    /// <summary>
    /// Sends a request to the appropriate handler and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of result expected from the request.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> containing the response or error message.</returns>
    public async Task<Result<TResult>> SendAsync<TResult>(
        IRequest<TResult> request,
        CancellationToken cancellationToken = default)
    {
        if (!_handlers.TryGetValue(request.GetType(), out var handler))
            return Result<TResult>.Fail($"No handler registered for {request.GetType().Name}");

        try
        {
            dynamic typedHandler = handler;
            TResult result = await typedHandler.HandleAsync((dynamic)request, cancellationToken);
            return Result<TResult>.Ok(result);
        }
        catch (Exception ex)
        {
            return Result<TResult>.Fail(ex.Message);
        }
    }
}
