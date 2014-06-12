namespace WebSocketMiddleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a Web Socket context.
    /// </summary>
    public interface IWebSocketContext
    {
        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="endOfMessage">if set to <c>true</c> [end of message].</param>
        /// <returns></returns>
        Task Send(ArraySegment<byte> data, int messageType, bool endOfMessage);

        /// <summary>
        /// Receives data into the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        Task<IWebSocketReceiveResult> Receive(ArraySegment<byte> buffer);

        /// <summary>
        /// Closes this websocket connection.
        /// </summary>
        /// <returns></returns>
        Task Close();

        /// <summary>
        /// Gets a value indicating whether [client closed] the websocket connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [client closed]; otherwise, <c>false</c>.
        /// </value>
        bool ClientClosed { get; }

        /// <summary>
        /// The OWIN environment.
        /// </summary>
        IDictionary<string, object> Environment { get; }
    }
}