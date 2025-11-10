namespace BlazorWinForms.Bridge;

/// <summary>
/// Contains the JavaScript code for the event and request bridges.
/// This code is automatically injected into the WebView2 at runtime.
/// </summary>
public static class BridgeJavaScript
{
    /// <summary>
    /// JavaScript code that sets up the event bridge and request bridge for communication
    /// between the WinForms host and Blazor guest.
    /// </summary>
    public const string Code = """
        console.log('[BridgeJS] Script executing, checking environment...');
        console.log('[BridgeJS] window.chrome:', !!window.chrome);
        console.log('[BridgeJS] window.chrome.webview:', !!window.chrome?.webview);
        console.log('[BridgeJS] hostObjects:', !!window.chrome?.webview?.hostObjects);
        console.log('[BridgeJS] hostObjects.sync:', !!window.chrome?.webview?.hostObjects?.sync);
        console.log('[BridgeJS] hostObjects.sync.appBridge:', !!window.chrome?.webview?.hostObjects?.sync?.appBridge);

        // Event bridge for receiving events from the WinForms host
        window.eventBridge = {
            dotNetReference: null,

            register: function (dotNetRef) {
                this.dotNetReference = dotNetRef;
            },

            // This function is called by the host via PostWebMessageAsJson
            receiveMessage: function (message) {
                if (this.dotNetReference && message.type === 'event') {
                    // Create the JSON string that EventBridgeService.OnHostEvent expects
                    const eventData = JSON.stringify({
                        name: message.name,
                        payload: message.payload
                    });

                    // Call the .NET method with the JSON string as a single argument
                    this.dotNetReference.invokeMethodAsync('OnHostEvent', eventData)
                        .catch(err => console.error('[EventBridge] Error:', err));
                }
            }
        };

        // Request bridge for sending requests to the WinForms host
        window.appBridge = {
            send: async function (json, typeName) {
                console.log('[appBridge] send() called with:', { json, typeName });

                // Check if host object is available
                if (!window.chrome?.webview?.hostObjects?.sync?.appBridge) {
                    console.error('[appBridge] Host object not available!', {
                        hasChrome: !!window.chrome,
                        hasWebview: !!window.chrome?.webview,
                        hasHostObjects: !!window.chrome?.webview?.hostObjects,
                        hasSync: !!window.chrome?.webview?.hostObjects?.sync,
                        hasAppBridge: !!window.chrome?.webview?.hostObjects?.sync?.appBridge
                    });
                    return JSON.stringify({ Success: false, Error: 'Host object not available' });
                }

                try {
                    console.log('[appBridge] Calling host object Send method...');
                    // IMPORTANT: Use .sync proxy to access host objects synchronously
                    // Async access causes "Element not found (0x80070490)" error
                    const result = window.chrome.webview.hostObjects.sync.appBridge.Send(json, typeName);
                    console.log('[appBridge] Host returned:', result);
                    return result;
                } catch (error) {
                    console.error('[appBridge] Error calling host:', error);
                    return JSON.stringify({ Success: false, Error: error.message });
                }
            }
        };

        console.log('[BridgeJS] Bridges created successfully');

        // Listen for messages from WebView2
        if (window.chrome && window.chrome.webview) {
            window.chrome.webview.addEventListener('message', event => {
                // Ignore Blazor's internal messages (they start with __bwv:)
                if (typeof event.data === 'string' && event.data.startsWith('__bwv:')) {
                    return; // Let Blazor handle its own messages
                }

                // Parse string data if needed (PostWebMessageAsString sends strings)
                const data = typeof event.data === 'string' ? JSON.parse(event.data) : event.data;
                window.eventBridge.receiveMessage(data);
            });
        }
        """;
}
