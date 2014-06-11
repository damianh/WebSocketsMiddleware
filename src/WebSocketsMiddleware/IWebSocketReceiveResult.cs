namespace WebSocketMiddleware
{
    public interface IWebSocketReceiveResult
    {
        int MessageType { get; }

        bool EndOfMessage { get; }

        int Count { get; }
    }
}