using System;
using System.Net.WebSockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    private ClientWebSocket webSocket;

    async void Start()
    {
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://localhost:8080");

        try
        {
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("✅ WebSocket Connected!");

            // 메시지 수신 루프 시작
            ReceiveLoop();
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Connection Error: {e.Message}");
        }
    }

    async void ReceiveLoop()
    {
        var buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Debug.Log($"📩 Server says: {msg}");
        }
    }

    public async void SendMessage(string message)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    private void OnApplicationQuit()
    {
        webSocket?.Dispose();
    }
}
