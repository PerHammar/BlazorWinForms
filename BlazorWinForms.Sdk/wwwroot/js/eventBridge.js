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
        try {
            // IMPORTANT: Use .sync proxy to access host objects synchronously
            // Async access causes "Element not found (0x80070490)" error
            const result = window.chrome.webview.hostObjects.sync.appBridge.Send(json, typeName);
            return result;
        } catch (error) {
            console.error('Bridge error:', error);
            return JSON.stringify({ Success: false, Error: error.message });
        }
    }
};

// Listen for messages from WebView2
if (window.chrome && window.chrome.webview) {
    window.chrome.webview.addEventListener('message', event => {
        window.eventBridge.receiveMessage(event.data);
    });
}
