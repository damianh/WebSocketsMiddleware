namespace Owin
{
    using System;
    using System.Threading.Tasks;
    using WebSocketMiddleware;

    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseWebSockets(this IAppBuilder appBuilder, Func<IWebSocketContext, Task> onAccept)
        {
            appBuilder.Use(WebSocketsMiddleware.UseWebsockets(onAccept));
            return appBuilder;
        }
    }
}
