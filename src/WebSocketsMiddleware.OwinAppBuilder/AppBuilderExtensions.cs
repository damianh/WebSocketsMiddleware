namespace Owin
{
    using System;
    using System.Threading.Tasks;
    using WebSocketMiddleware;

    /// <summary>
    /// Extension methods on the IAppBuilde
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>
        /// Add web socket middleware to owin pipeline that is built using IAppBuilder.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="onAccept">A delagete that is invoked when a websocket request has been received.</param>
        /// <returns>The application builder.</returns>
        public static IAppBuilder UseWebSockets(this IAppBuilder appBuilder, Func<IWebSocketContext, Task> onAccept)
        {
            return UseWebSockets(appBuilder, @"/", onAccept);
        }

        /// <summary>
        /// Add web socket middleware to owin pipeline that is built using IAppBuilder.
        /// </summary>
        /// <param name="appBuilder">The application builder.</param>
        /// <param name="path">The path to handle websocket requests.</param>
        /// <param name="onAccept">A delagete that is invoked when a websocket request has been received.</param>
        /// <returns>The application builder.</returns>
        public static IAppBuilder UseWebSockets(this IAppBuilder appBuilder, string path, Func<IWebSocketContext, Task> onAccept)
        {
            appBuilder.Use(WebSockets.UseWebsockets(path, onAccept));
            return appBuilder;
        }
    }
}
