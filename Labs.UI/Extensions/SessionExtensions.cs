// Labs.UI/Extensions/SessionExtensions.cs
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Labs.UI.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T item)
        {
            var serializedItem = JsonSerializer.Serialize(item);
            session.SetString(key, serializedItem);
        }

        public static T Get<T>(this ISession session, string key)
        {
            var item = session.GetString(key);
            if (string.IsNullOrEmpty(item))
                return default(T);

            return JsonSerializer.Deserialize<T>(item);
        }
    }
}