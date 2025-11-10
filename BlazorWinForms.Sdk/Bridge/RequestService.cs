using System.Text.Json;
using Microsoft.JSInterop;
using BlazorWinForms.Interop;

namespace BlazorWinForms.Bridge;

/// <summary>
/// Blazor-side service for sending requests to the WinForms host.
/// </summary>
public class RequestService
{
    private readonly IJSRuntime _jsRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestService"/> class.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for invoking JavaScript interop.</param>
    public RequestService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Sends a request to the WinForms host and returns the result.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected result.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A <see cref="Result{T}"/> containing the response or error.</returns>
    public async Task<Result<TResult>> SendAsync<TResult>(
        IRequest<TResult> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Serialize using the concrete type, not the interface
            var concreteType = request.GetType();
            var json = JsonSerializer.Serialize(request, concreteType);
            var typeName = concreteType.AssemblyQualifiedName!;

            var resultJson = await _jsRuntime.InvokeAsync<string>(
                "appBridge.send",
                cancellationToken,
                json,
                typeName);

            return JsonSerializer.Deserialize<Result<TResult>>(resultJson)!;
        }
        catch (Exception ex)
        {
            return Result<TResult>.Fail(ex.Message);
        }
    }
}
