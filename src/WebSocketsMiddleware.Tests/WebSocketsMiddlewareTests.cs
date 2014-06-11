namespace WebSocketsMiddleware
{
    using System;
    using System.Linq;
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
                var clientWebSocket = new ClientWebSocket();
                await clientWebSocket.ConnectAsync(new Uri("ws://localhost:5000/"), CancellationToken.None);
                byte[] bytes = Encoding.UTF8.GetBytes("Hello");
                await clientWebSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, false, CancellationToken.None);

                var buffer = new byte[1024];
                WebSocketReceiveResult result =
                    await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string s = Encoding.UTF8.GetString(buffer, 0, result.Count);

                s.Should().Be("olleH");
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
    }
}