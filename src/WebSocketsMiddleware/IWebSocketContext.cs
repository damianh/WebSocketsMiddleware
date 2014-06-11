namespace WebSocketMiddleware
{
    using System;
    using System.Threading.Tasks;

    public interface IWebSocketContext
    {
        Task Send(ArraySegment<byte> data, int messageType, bool endOfMessage);

        Task<IWebSocketReceiveResult> Receive(ArraySegment<byte> buffer);

        Task Close();

        bool ClientClosed { get; }
    }
}