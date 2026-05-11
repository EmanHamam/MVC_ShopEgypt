using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ShopEgypt.Application.Extensions
{
    public static class SessionExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static void SetObject<T>(this ISession session, string key, T value)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var json = JsonSerializer.Serialize(value, _jsonOptions);
            var bytes = Encoding.UTF8.GetBytes(json);

            session.Set(key, bytes);
        }

        public static T? GetObject<T>(this ISession session, string key)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            if (!session.TryGetValue(key, out var bytes) || bytes is null || bytes.Length == 0)
                return default;

            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }

        public static void RemoveObject(this ISession session, string key)
        {
            ArgumentNullException.ThrowIfNull(session);
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            session.Remove(key);
        }
    }

}
