namespace WebSocketMiddleware
{
    /// <summary>
    /// Represents the result of a Web Socket receive.
    /// </summary>
    public interface IWebSocketReceiveResult
    {
        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        int MessageType { get; }

        /// <summary>
        /// Gets a value indicating whether [end of message].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [end of message]; otherwise, <c>false</c>.
        /// </value>
        bool EndOfMessage { get; }

        /// <summary>
        /// Gets the count of bytes received .
        /// </summary>
        /// <value>
        /// The count of bytes received.
        /// </value>
        int Count { get; }
    }
}