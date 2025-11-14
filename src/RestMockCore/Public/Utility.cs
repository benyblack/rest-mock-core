namespace RestMockCore;
public static class Utility
{
    public static bool HasAny<T>(this IEnumerable<T> data)
    {
        if (data == null) return false;
        // Fast path for collections that expose Count
        if (data is ICollection<T> collection) return collection.Count > 0;
        // Fallback to enumerator without LINQ allocation
        using var e = data.GetEnumerator();
        return e.MoveNext();
    }

    public static void AddRange(this IHeaderDictionary responseHeader, Dictionary<string, string> headers)
    {
        if (headers == null || headers.Count == 0) return;
        foreach (var item in headers)
        {
            // Assign to indexer to replace or add without throwing on duplicates
            responseHeader[item.Key] = item.Value;
        }
    }
}
