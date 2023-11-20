using System;
using UnityEngine;
using Phoenix;
using WebSocketSharp;
using PhoenixTests.WebSocketImpl;

public class WebSocketClient : MonoBehaviour
{
    [Serializable]
    public class WebsocketData
    {
        public string url;
    }

    private Socket socket;
    [SerializeField] string stringURL = "ws://bitter-resonance-4714.fly.dev/socket";

    void Start()
    {
        var websocketFactory = new WebsocketSharpFactory();
        var socketAddress = stringURL;
        var jsonMessageSerializer = new JsonMessageSerializer();
        var socketOptions = new Socket.Options(jsonMessageSerializer);

        socket = new Socket(socketAddress, null, websocketFactory, socketOptions);

        socket.OnOpen += OnSocketOpen;
        socket.OnMessage += OnSocketMessage;
        socket.OnError += OnSocketError;

        socket.Connect();

        var roomChannel = socket.Channel("room:123", null);
    
    // Add event listeners for the channel
    roomChannel.On(Message.InBoundEvent.Error, message => Debug.LogError("Error: " + message));
    roomChannel.On("after_join", message => Debug.Log("After Join: " + message));
    // Other event listeners as needed

    // Join the channel
    roomChannel.Join()
        .Receive(ReplyStatus.Ok, reply => Debug.Log("Joined channel successfully"))
        .Receive(ReplyStatus.Error, reply => Debug.LogError("Error joining channel"));

    }


    private void OnSocketOpen()
    {
        Debug.Log("WebSocket connected.");
    }

private void OnSocketMessage(Message message)
{
    try
    {
        // Check the type of inbound event
        switch (message.Event)
        {
            case "phx_error":
                Debug.LogError("Error event received");
                break;

            case "phx_close":
                Debug.Log("Close event received");
                break;

            case "phx_reply":
                var jsonData = message.Payload.Unbox<string>();
                WebsocketData data = JsonUtility.FromJson<WebsocketData>(jsonData);
                Debug.Log("Reply received: URL - " + data.url);
                break;

            default:
                Debug.Log("Unknown event received: " + message.Event);
                break;
        }
    }
    catch (Exception ex)
    {
        Debug.LogError("Error processing message: " + ex.Message);
    }
}

    private void OnSocketError(string message)
    {
        Debug.LogError("WebSocket error: " + message);
    }

    void OnDestroy()
    {
        if (socket != null)
        {
            socket.Disconnect();
        }
    }
}
