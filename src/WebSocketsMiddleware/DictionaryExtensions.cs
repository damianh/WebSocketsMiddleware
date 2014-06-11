namespace WebSocketMiddleware
{
    using System.Collections.Generic;

    internal static class DictionaryExtensions
    {
        internal static T Get<T>(this IDictionary<string, object> dictionary, string key)
            where T : class
        {
            object value;
            return dictionary.TryGetValue(key, out value) ? value as T : null;
        }
    }
}