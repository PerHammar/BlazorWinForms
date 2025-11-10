using BlazorWinForms.Bridge;
using BlazorWinForms.Interop;
using FluentAssertions;
using Microsoft.JSInterop;
using NSubstitute;
using System.Text.Json;
using Xunit;

namespace BlazorWinForms.Tests.Unit.Integration;

/// <summary>
/// End-to-end integration tests that verify the complete request/response flow
/// from Blazor through JavaScript to WinForms and back.
/// </summary>
public class EndToEndFlowTests
{
    public record EchoRequest(string Message) : IRequest<string>;

    public class EchoRequestHandler : IRequestHandler<EchoRequest, string>
    {
        public Task<string> HandleAsync(EchoRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult($"Echo: {request.Message}");
        }
    }

    public record ThrowingRequest : IRequest<string>;

    public class ThrowingRequestHandler : IRequestHandler<ThrowingRequest, string>
    {
        public Task<string> HandleAsync(ThrowingRequest request, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Test exception from handler");
        }
    }

    private static IJSRuntime CreateMockedJSRuntime(AppBridge appBridge)
    {
        var jsRuntime = Substitute.For<IJSRuntime>();

        jsRuntime.InvokeAsync<string>(
            Arg.Is("appBridge.send"),
            Arg.Any<CancellationToken>(),
            Arg.Any<object[]>())
            .Returns(callInfo =>
            {
                var args = callInfo.ArgAt<object[]>(2);
                var json = args[0] as string;
                var typeName = args[1] as string;
                return new ValueTask<string>(appBridge.Send(json!, typeName!));
            });

        return jsRuntime;
    }

    [Fact]
    public async Task BlazorToWinForms_FullRequestFlow_Works()
    {
        // This test simulates the complete flow:
        // 1. Blazor RequestService serializes request
        // 2. JavaScript bridge receives it (mocked)
        // 3. AppBridge deserializes and dispatches
        // 4. Handler processes it
        // 5. Result is serialized back
        // 6. RequestService deserializes result

        // Arrange
        var dispatcher = new RequestDispatcher(typeof(EchoRequestHandler).Assembly);
        var appBridge = new AppBridge(dispatcher);
        var jsRuntime = CreateMockedJSRuntime(appBridge);
        var requestService = new RequestService(jsRuntime);
        var request = new EchoRequest("Hello from Blazor");

        // Act
        var result = await requestService.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().Be("Echo: Hello from Blazor");
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task BlazorToWinForms_WithNoHandler_ReturnsError()
    {
        // Arrange
        var dispatcher = new RequestDispatcher(); // Empty - no handlers
        var appBridge = new AppBridge(dispatcher);
        var jsRuntime = CreateMockedJSRuntime(appBridge);
        var requestService = new RequestService(jsRuntime);
        var request = new EchoRequest("Test");

        // Act
        var result = await requestService.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("No handler");
    }

    [Fact]
    public async Task BlazorToWinForms_WithException_ReturnsErrorResult()
    {
        // Arrange
        var dispatcher = new RequestDispatcher(typeof(ThrowingRequestHandler).Assembly);
        var appBridge = new AppBridge(dispatcher);
        var jsRuntime = CreateMockedJSRuntime(appBridge);
        var requestService = new RequestService(jsRuntime);
        var request = new ThrowingRequest();

        // Act
        var result = await requestService.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Test exception");
    }

    [Fact]
    public async Task AppBridge_WithInvalidJson_ReturnsFailureResult()
    {
        // Arrange
        var dispatcher = new RequestDispatcher(typeof(EchoRequestHandler).Assembly);
        var appBridge = new AppBridge(dispatcher);
        var invalidJson = "{ this is not valid json }";
        var typeName = typeof(EchoRequest).AssemblyQualifiedName!;

        // Act
        var resultJson = await appBridge.Send(invalidJson, typeName);

        // Assert
        var result = JsonSerializer.Deserialize<Result<string>>(resultJson);
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AppBridge_WithUnknownType_ReturnsFailureResult()
    {
        // Arrange
        var dispatcher = new RequestDispatcher(typeof(EchoRequestHandler).Assembly);
        var appBridge = new AppBridge(dispatcher);
        var requestJson = "{\"Message\":\"test\"}";
        var invalidTypeName = "NonExistent.Type, NonExistent";

        // Act
        var resultJson = await appBridge.Send(requestJson, invalidTypeName);

        // Assert
        var result = JsonSerializer.Deserialize<Result<object>>(resultJson);
        result.Should().NotBeNull();
        result!.Success.Should().BeFalse();
        result.Error.Should().NotBeNullOrEmpty();
    }
}
