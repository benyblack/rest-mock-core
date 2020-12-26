using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace RestMockCore
{
    public static class Utility
    {
        public static bool HasAny<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }

        public static void AddRange(this IHeaderDictionary responseHeader, Dictionary<string,string> headers )
        {
            if (headers == null) return;
            foreach (var item in headers)
            {
                responseHeader.Add(item.Key, item.Value);
            }
        }
    }
}
