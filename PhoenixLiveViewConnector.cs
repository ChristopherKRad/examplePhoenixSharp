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
        var jsonMessageSerializer = new JsonMessageSerializer();
        var socketOptions = new Socket.Options(jsonMessageSerializer);

        socket = new Socket("ws://bitter-resonance-4714.fly.dev", null, websocketFactory, socketOptions);

        socket.OnOpen += OnSocketOpen;
        socket.OnMessage += OnSocketMessage;
        socket.OnError += OnSocketError;

        socket.Connect();
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
