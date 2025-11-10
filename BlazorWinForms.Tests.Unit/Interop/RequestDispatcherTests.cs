using BlazorWinForms.Interop;
using FluentAssertions;
using Xunit;

namespace BlazorWinForms.Tests.Unit.Interop;

public class RequestDispatcherTests
{
    // Test request and handler
    public record TestRequest(string Value) : IRequest<string>;

    public class TestRequestHandler : IRequestHandler<TestRequest, string>
    {
        public Task<string> HandleAsync(TestRequest request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult($"Handled: {request.Value}");
        }
    }

    [Fact]
    public async Task SendAsync_WithRegisteredHandler_ReturnsSuccess()
    {
        // Arrange
        var dispatcher = new RequestDispatcher(typeof(TestRequestHandler).Assembly);
        var request = new TestRequest("test");

        // Act
        var result = await dispatcher.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().Be("Handled: test");
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task SendAsync_WithUnregisteredHandler_ReturnsFailure()
    {
        // Arrange
        var dispatcher = new RequestDispatcher(); // Empty, no handlers
        var request = new TestRequest("test");

        // Act
        var result = await dispatcher.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("No handler registered");
    }

    [Fact]
    public async Task SendAsync_HandlerThrowsException_ReturnsFailureWithError()
    {
        // Arrange
        var dispatcher = new RequestDispatcher(typeof(ThrowingHandler).Assembly);
        var request = new ThrowingRequest();

        // Act
        var result = await dispatcher.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("Test exception");
    }

    // Helper for exception test
    public record ThrowingRequest : IRequest<string>;

    public class ThrowingHandler : IRequestHandler<ThrowingRequest, string>
    {
        public Task<string> HandleAsync(ThrowingRequest request, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Test exception");
        }
    }
}
