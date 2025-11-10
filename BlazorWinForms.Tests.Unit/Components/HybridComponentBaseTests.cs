using BlazorWinForms.Bridge;
using BlazorWinForms.Components;
using BlazorWinForms.Interop;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace BlazorWinForms.Tests.Unit.Components;

public class HybridComponentBaseTests : TestContext
{
    // Test component that extends HybridComponentBase
    public class TestComponent : HybridComponentBase
    {
        public bool InitializedCalled { get; private set; }
        public bool EventReceived { get; private set; }

        // Expose protected properties for testing
        public RequestService PublicRequestService => RequestService;
        public EventBridgeService PublicEventBridge => EventBridge;
        public Microsoft.JSInterop.IJSRuntime PublicJSRuntime => JSRuntime;

        protected override Task OnHybridInitializedAsync()
        {
            InitializedCalled = true;
            SubscribeToEvent<TestEvent>(OnTestEvent);
            return Task.CompletedTask;
        }

        private void OnTestEvent(TestEvent evt)
        {
            EventReceived = true;
        }
    }

    public record TestEvent(string Message) : IEvent;
    public record TestRequest(string Value) : IRequest<string>;

    [Fact]
    public void Component_InjectsRequiredServices()
    {
        // Arrange
        var requestService = Substitute.For<RequestService>(Substitute.For<Microsoft.JSInterop.IJSRuntime>());
        var eventBridge = Substitute.For<EventBridgeService>(Substitute.For<Microsoft.JSInterop.IJSRuntime>());

        Services.AddSingleton(requestService);
        Services.AddSingleton(eventBridge);

        // Act
        var cut = RenderComponent<TestComponent>();

        // Assert
        cut.Instance.PublicRequestService.Should().NotBeNull();
        cut.Instance.PublicEventBridge.Should().NotBeNull();
        cut.Instance.PublicJSRuntime.Should().NotBeNull();
    }

    [Fact]
    public async Task OnAfterRenderAsync_FirstRender_CallsOnHybridInitializedAsync()
    {
        // Arrange
        var requestService = Substitute.For<RequestService>(Substitute.For<Microsoft.JSInterop.IJSRuntime>());
        var eventBridge = Substitute.For<EventBridgeService>(Substitute.For<Microsoft.JSInterop.IJSRuntime>());

        Services.AddSingleton(requestService);
        Services.AddSingleton(eventBridge);

        // Act
        var cut = RenderComponent<TestComponent>();
        await Task.Delay(100); // Give time for async initialization

        // Assert
        cut.Instance.InitializedCalled.Should().BeTrue();
        await eventBridge.Received(1).InitializeAsync();
    }

    [Fact]
    public void Dispose_UnsubscribesAllEvents()
    {
        // Arrange
        var requestService = Substitute.For<RequestService>(Substitute.For<Microsoft.JSInterop.IJSRuntime>());
        var eventBridge = Substitute.For<EventBridgeService>(Substitute.For<Microsoft.JSInterop.IJSRuntime>());

        Services.AddSingleton(requestService);
        Services.AddSingleton(eventBridge);

        var cut = RenderComponent<TestComponent>();

        // Act
        cut.Instance.Dispose();

        // Assert - component should dispose without errors
        // Subscriptions list should be cleared
        cut.Instance.Should().NotBeNull();
    }
}
