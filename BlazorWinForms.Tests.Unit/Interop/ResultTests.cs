using BlazorWinForms.Interop;
using FluentAssertions;
using Xunit;

namespace BlazorWinForms.Tests.Unit.Interop;

/// <summary>
/// Tests for the Result wrapper type.
/// </summary>
public class ResultTests
{
    [Fact]
    public void Ok_CreatesSuccessResult()
    {
        // Act
        var result = Result<string>.Ok("test data");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().Be("test data");
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Fail_CreatesFailureResult()
    {
        // Act
        var result = Result<string>.Fail("error message");

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Data.Should().BeNull();
        result.Error.Should().Be("error message");
    }
}
