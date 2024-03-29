﻿using Microsoft.AspNetCore.Http;
using TypeSerialization;

namespace ServiceProviderEndpoint;

internal static class HttpExtensions
{
    public static bool IsJson(this HttpRequest value)
    {
        return value.ContentType?.StartsWith("application/json", StringComparison.InvariantCultureIgnoreCase) == true;
    }

    public static IHeaderDictionary AddNoCache(this IHeaderDictionary headers)
    {
        foreach (var kvp in NoCacheHeaders)
            headers[kvp.Key] = kvp.Value;

        return headers;
    }

    static readonly Dictionary<string, string> NoCacheHeaders = new()
    {
        { "Cache-Control", "no-cache, no-store, must-revalidate" },
        { "Pragma", "no-cache" },
        { "Expires", "0" },
    };
}
