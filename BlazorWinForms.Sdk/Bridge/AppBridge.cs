using System.Runtime.InteropServices;
using System.Text.Json;
using BlazorWinForms.Interop;

namespace BlazorWinForms.Bridge;

/// <summary>
/// Host-side bridge exposed to JavaScript for receiving requests from Blazor.
/// This object is injected into the WebView2 context and accessible from JavaScript.
/// </summary>
[ClassInterface(ClassInterfaceType.AutoDual)]
[ComVisible(true)]
public class AppBridge
{
    private readonly RequestDispatcher _requestDispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppBridge"/> class.
    /// </summary>
    /// <param name="requestDispatcher">The request dispatcher to route incoming requests.</param>
    public AppBridge(RequestDispatcher requestDispatcher)
    {
        _requestDispatcher = requestDispatcher;
    }

    /// <summary>
    /// Receives a request from JavaScript, deserializes it, dispatches it to the appropriate handler,
    /// and returns the serialized result.
    /// </summary>
    /// <param name="json">The JSON-serialized request.</param>
    /// <param name="typeName">The assembly-qualified type name of the request.</param>
    /// <returns>A JSON-serialized <see cref="Result{T}"/> containing the response or error.</returns>
    public async Task<string> Send(string json, string typeName)
    {
        try
        {
            var requestType = Type.GetType(typeName);
            if (requestType == null)
                return JsonSerializer.Serialize(new { Success = false, Error = $"Type not found: {typeName}" });

            var request = JsonSerializer.Deserialize(json, requestType);
            if (request == null)
                return JsonSerializer.Serialize(new { Success = false, Error = "Failed to deserialize request" });

            var requestInterface = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (requestInterface == null)
                return JsonSerializer.Serialize(new { Success = false, Error = "Invalid request type" });

            var resultType = requestInterface.GetGenericArguments()[0];

            var sendMethod = typeof(RequestDispatcher)
                .GetMethod(nameof(RequestDispatcher.SendAsync))!
                .MakeGenericMethod(resultType);

            var task = (Task)sendMethod.Invoke(_requestDispatcher, new[] { request, CancellationToken.None })!;
            await task;

            var resultProperty = task.GetType().GetProperty("Result")!;
            var result = resultProperty.GetValue(task)!;

            return JsonSerializer.Serialize(result);
        }
        catch (Exception ex)
        {
            return JsonSerializer.Serialize(new { Success = false, Error = ex.Message });
        }
    }
}
