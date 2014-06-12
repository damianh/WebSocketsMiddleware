namespace WebSocketsMiddleware
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Owin.Hosting;
    using Owin;
    using WebSocketMiddleware;
    using Xunit;

    public class WebSocketsMiddlewareTests
    {
        [Fact]
        public async Task Can_send_and_receive_message()
        {
            using (WebApp.Start("http://localhost:5000/", app => app.UseWebSockets(WebSocketReverseMessage)))
            {
                string response = await SendWebsocketMessage("ws://localhost:5000/", "Hello");

                response.Should().Be("olleH");
            }
        }

        [Fact]
        public void When_path_does_not_match_than_can_not_send_message()
        {
            using (WebApp.Start("http://localhost:5000/", app => app.UseWebSockets(WebSocketReverseMessage)))
            {
                Func<Task> act = () => SendWebsocketMessage("ws://localhost:5000/path", "Hello");
                
                act.ShouldThrow<WebSocketException>();
            }
        }

        [Fact]
        public async Task When_path_matches_than_can_send_message()
        {
            using (WebApp.Start("http://localhost:5000/", app => app.UseWebSockets("/path", WebSocketEcho)))
            {
                string response = await SendWebsocketMessage("ws://localhost:5000/path", "Hello");

                response.Should().Be("Hello");
            }
        }

        [Fact]
        public async Task Should_have_request_env()
        {
            using (WebApp.Start("http://localhost:5000/", app => app.UseWebSockets(WebSocketCheckEnv)))
            {
                string response = await SendWebsocketMessage("ws://localhost:5000", string.Empty);

                response.Should().Be("true");
            }
        }

        [Fact]
        public async Task Can_have_http_and_websocket_at_same_uri()
        {
            using (WebApp.Start("http://localhost:5000/",
                app => app
                    .UseWebSockets(WebSocketEcho)
                    .Use(async (context, _)  =>
                    {
                        await context.Response.WriteAsync("http request");
                    })))
            {
                string websocketResponse = await SendWebsocketMessage("ws://localhost:5000/", "ws request");
                websocketResponse.Should().Be("ws request");

                using (var httpclient = new HttpClient())
                {
                    var httpResponse = await httpclient.GetAsync("http://localhost:5000/");
                    (await httpResponse.Content.ReadAsStringAsync()).Should().Be("http request");
                }
            }
        }

        private static async Task<string> SendWebsocketMessage(string uri, string message)
        {
            using (var clientWebSocket = new ClientWebSocket())
            {
                await clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await
                    clientWebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, false,
                        CancellationToken.None);

                var buffer = new byte[1024];
                WebSocketReceiveResult result =
                    await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                return Encoding.UTF8.GetString(buffer, 0, result.Count);
            }
        }

        private static async Task WebSocketReverseMessage(IWebSocketContext webSocketContext)
        {
            var buffer = new byte[1024];
            IWebSocketReceiveResult received = await webSocketContext.Receive(new ArraySegment<byte>(buffer));

            while (!webSocketContext.ClientClosed)
            {
                byte[] reversedMessage = Encoding.UTF8.GetBytes(String.Concat(Encoding.UTF8.GetString(buffer, 0, received.Count).Reverse()));
                await webSocketContext.Send(
                    new ArraySegment<byte>(reversedMessage, 0, reversedMessage.Length),
                    received.MessageType,
                    received.EndOfMessage);

                received = await webSocketContext.Receive(new ArraySegment<byte>(buffer));
            }

            await webSocketContext.Close();
        }

        private static async Task WebSocketEcho(IWebSocketContext webSocketContext)
        {
            var buffer = new byte[1024];
            IWebSocketReceiveResult received = await webSocketContext.Receive(new ArraySegment<byte>(buffer));

            while (!webSocketContext.ClientClosed)
            {
                await webSocketContext.Send(
                    new ArraySegment<byte>(buffer, 0, received.Count),
                    received.MessageType,
                    received.EndOfMessage);

                received = await webSocketContext.Receive(new ArraySegment<byte>(buffer));
            }

            await webSocketContext.Close();
        }

        private static async Task WebSocketCheckEnv(IWebSocketContext webSocketContext)
        {
            var buffer = new byte[1024];
            IWebSocketReceiveResult received = await webSocketContext.Receive(new ArraySegment<byte>(buffer));

            while (!webSocketContext.ClientClosed)
            {
                bool haveEnv = webSocketContext.Environment != null;
                byte[] bytes = Encoding.UTF8.GetBytes(haveEnv.ToString());

                await webSocketContext.Send(
                    new ArraySegment<byte>(bytes, 0, bytes.Length),
                    received.MessageType,
                    received.EndOfMessage);

                received = await webSocketContext.Receive(new ArraySegment<byte>(buffer));
            }

            await webSocketContext.Close();
        }
    }
}