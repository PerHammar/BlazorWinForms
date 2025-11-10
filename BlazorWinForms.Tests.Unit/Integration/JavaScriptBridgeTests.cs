using BlazorWinForms.Bridge;
using FluentAssertions;
using Xunit;

namespace BlazorWinForms.Tests.Unit.Integration;

/// <summary>
/// Tests that verify the JavaScript bridge code is correctly generated.
/// </summary>
public class JavaScriptBridgeTests
{
    [Fact]
    public void BridgeJavaScript_IsNotEmpty()
    {
        // Arrange & Act
        var jsCode = BridgeJavaScript.Code;

        // Assert
        jsCode.Should().NotBeNullOrWhiteSpace();
        jsCode.Length.Should().BeGreaterThan(100, "bridge code should contain substantial JavaScript");
    }
}
