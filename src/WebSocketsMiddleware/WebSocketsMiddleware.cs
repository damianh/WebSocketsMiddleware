namespace WebSocketMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using AppFunc = System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>;
    using MidFunc = System.Func<
        System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>,
        System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>
        >;

    // http://owin.org/extensions/owin-WebSocket-Extension-v0.4.0.htm
    using WebSocketAccept = 
        System.Action<System.Collections.Generic.IDictionary<string, object>, // options
        System.Func<System.Collections.Generic.IDictionary<string, object>, System.Threading.Tasks.Task>>; // callback
    using WebSocketClose = 
            System.Func<int /* closeStatus */,
                string /* closeDescription */,
                System.Threading.CancellationToken /* cancel */,
                System.Threading.Tasks.Task>;
    using WebSocketReceive = 
        System.Func<System.ArraySegment<byte> /* data */,
            System.Threading.CancellationToken /* cancel */,
            System.Threading.Tasks.Task<
                System.Tuple<
                    int /* messageType */,
                    bool /* endOfMessage */,
                    int /* count */>>>;
    using WebSocketSend =
        System.Func<System.ArraySegment<byte> /* data */,
            int /* messageType */,
            bool /* endOfMessage */,
            System.Threading.CancellationToken /* cancel */,
            System.Threading.Tasks.Task>;
    using WebSocketReceiveResult = System.Tuple<int, // type
        bool, // end of message?
        int>; // count

    /// <summary>
    /// Represents an owin middleware that can handle web socket requests. 
    /// </summary>
    public static class WebSocketsMiddleware
    {
        private const string OwinRequestPathKey = "owin.RequestPath";
        private const string WebSocketAcceptKey = "websocket.Accept";
        private const string WebSocketSendKey = "websocket.SendAsync";
        private const string WebSocketReceiveKey = "websocket.ReceiveAsync";
        private const string WebSocketCloseKey = "websocket.CloseAsync";
        private const string WebSocketCallCancelledKey = "websocket.CallCancelled";

        /// <summary>
        /// Use the websocket middleware in an owin pipeline. 
        /// </summary>
        /// <param name="onAccept">A delagete that is invoked when a websocket request has been received.</param>
        /// <returns>An owin middleware delegate.</returns>
        public static MidFunc UseWebsockets(Func<IWebSocketContext, Task> onAccept)
        {
            return UseWebsockets("/", onAccept);
        }

        /// <summary>
        /// Use the websocket middleware in an owin pipeline. 
        /// </summary>
        /// <param name="path">The path to handle websocket requests.</param>
        /// <param name="onAccept">A delagete that is invoked when a websocket request has been received.</param>
        /// <returns>An owin middleware delegate.</returns>
        public static MidFunc UseWebsockets(string path, Func<IWebSocketContext, Task> onAccept)
        {
            return
                next =>
                env =>
                {
                    var requestPath = env.Get<string>(OwinRequestPathKey);
                    if (requestPath != path)
                    {
                        return next(env);
                    }

                    var accept = env.Get<WebSocketAccept>(WebSocketAcceptKey);
                    if (accept == null)
                    {
                        return next(env);
                    }

                    accept(null, webSocketContext => onAccept(new WebSocketContext(webSocketContext)));

                    return Task.FromResult<object>(null);
                };
        }

        private class WebSocketContext : IWebSocketContext
        {
            private readonly WebSocketSend _send;
            private readonly WebSocketReceive _receive;
            private readonly WebSocketClose _close;
            private readonly CancellationToken _callCancelled;
            private readonly IDictionary<string, object> _env;
            private const string WebSocketClientCloseStatusKey = "websocket.ClientCloseStatus";
            private const string WebSocketClientCloseDescriptionKey = "websocket.ClientCloseDescription";

            public WebSocketContext(IDictionary<string, object> env)
            {
                _env = env;
                _send = (WebSocketSend)env[WebSocketSendKey];
                _receive = (WebSocketReceive)env[WebSocketReceiveKey];
                _close = (WebSocketClose)env[WebSocketCloseKey];
                _callCancelled = (CancellationToken)env[WebSocketCallCancelledKey];
            }

            public Task Close()
            {
                return _close((int)_env[WebSocketClientCloseStatusKey], (string)_env[WebSocketClientCloseDescriptionKey], _callCancelled);
            }

            public async Task<IWebSocketReceiveResult> Receive(ArraySegment<byte> buffer)
            {
                Tuple<int, bool, int> receiveResult = await _receive(buffer, _callCancelled);
                return new ReceiveResult(receiveResult.Item1, receiveResult.Item2, receiveResult.Item3);
            }

            public Task Send(ArraySegment<byte> data, int messageType, bool endOfMessage)
            {
                return _send(data, messageType, endOfMessage, _callCancelled);
            }

            public bool ClientClosed
            {
                get
                {
                    object status;
                    return _env.TryGetValue(WebSocketClientCloseStatusKey, out status) && (int)status == 0;
                }
            }

            private class ReceiveResult : IWebSocketReceiveResult
            {
                private readonly int _messageType;
                private readonly bool _endOfMessage;
                private readonly int _count;

                public ReceiveResult(int messageType, bool endOfMessage, int count)
                {
                    _messageType = messageType;
                    _endOfMessage = endOfMessage;
                    _count = count;
                }

                public int MessageType
                {
                    get { return _messageType; }
                }

                public bool EndOfMessage
                {
                    get { return _endOfMessage; }
                }

                public int Count
                {
                    get { return _count; }
                }
            }
        }
    }
}