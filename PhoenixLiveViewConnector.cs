using UnityEngine;
using Phoenix;
using WebSocketSharp;
using PhoenixTests.WebSocketImpl;

public class WebSocketClient : MonoBehaviour
{
    private Socket socket;

    void Start()
    {
        var websocketFactory = new WebsocketSharpFactory();
        var socketAddress = "ws://bitter-resonance-4714.fly.dev";
        var jsonMessageSerializer = new JsonMessageSerializer();
        var socketOptions = new Socket.Options(jsonMessageSerializer);

        socket = new Socket(socketAddress, null, websocketFactory, socketOptions);

        socket.OnOpen += OnSocketOpen;
        socket.OnMessage += OnSocketMessage;
        socket.OnError += OnSocketError;

        socket.Connect();

        var roomChannel = socket.Channel("tester:phoenix-sharp", null);
    
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
        var jsonData = message.Payload.Unbox<string>();
        Debug.Log("Message received: " + jsonData);
        // Deserialize jsonData here if necessary
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
