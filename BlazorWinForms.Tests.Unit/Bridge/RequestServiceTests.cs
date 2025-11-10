using BlazorWinForms.Bridge;
using BlazorWinForms.Interop;
using FluentAssertions;
using Microsoft.JSInterop;
using NSubstitute;
using System.Text.Json;
using Xunit;

namespace BlazorWinForms.Tests.Unit.Bridge;

public class RequestServiceTests
{
    public record TestRequest(string Value) : IRequest<string>;

    [Fact]
    public async Task SendAsync_Success_ReturnsSuccessResult()
    {
        // Arrange
        var jsRuntime = Substitute.For<IJSRuntime>();
        var expectedResult = Result<string>.Ok("success");
        var resultJson = JsonSerializer.Serialize(expectedResult);

        jsRuntime.InvokeAsync<string>(
            Arg.Is("appBridge.send"),
            Arg.Any<CancellationToken>(),
            Arg.Any<object[]>())
            .Returns(new ValueTask<string>(resultJson));

        var service = new RequestService(jsRuntime);
        var request = new TestRequest("test");

        // Act
        var result = await service.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().Be("success");
    }

    [Fact]
    public async Task SendAsync_JavaScriptError_ReturnsFailureResult()
    {
        // Arrange
        var jsRuntime = Substitute.For<IJSRuntime>();
        jsRuntime.InvokeAsync<string>(
            Arg.Is("appBridge.send"),
            Arg.Any<CancellationToken>(),
            Arg.Any<object[]>())
            .Returns(ValueTask.FromException<string>(new JSException("JavaScript error")));

        var service = new RequestService(jsRuntime);
        var request = new TestRequest("test");

        // Act
        var result = await service.SendAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Error.Should().Contain("JavaScript error");
    }
}
