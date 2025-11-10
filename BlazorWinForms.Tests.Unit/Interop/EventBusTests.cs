using BlazorWinForms.Interop;
using FluentAssertions;
using Xunit;

namespace BlazorWinForms.Tests.Unit.Interop;

// Test event
public record TestEvent(string Message) : IEvent;

// Test handler with tracking - must be top-level for EventBus discovery
public class TestEventHandler : IEventHandler<TestEvent>
{
    public static TestEvent? LastHandledEvent { get; private set; }
    public static int HandleCallCount { get; private set; }

    public static void Reset()
    {
        LastHandledEvent = null;
        HandleCallCount = 0;
    }

    public void Handle(TestEvent @event)
    {
        LastHandledEvent = @event;
        HandleCallCount++;
    }

    public Task HandleAsync(TestEvent @event, CancellationToken cancellationToken = default)
    {
        Handle(@event);
        return Task.CompletedTask;
    }
}

/// <summary>
/// Tests for the EventBus event publishing system.
/// </summary>
public class EventBusTests
{
    [Fact]
    public async Task PublishAsync_CallsLocalHandlers()
    {
        // Arrange
        TestEventHandler.Reset();
        var eventBus = new EventBus(relay: null, typeof(TestEventHandler).Assembly);
        var testEvent = new TestEvent("test message");

        // Act
        await eventBus.PublishAsync(testEvent);

        // Assert
        TestEventHandler.LastHandledEvent.Should().NotBeNull();
        TestEventHandler.LastHandledEvent!.Message.Should().Be("test message");
        TestEventHandler.HandleCallCount.Should().Be(1);
    }
}
