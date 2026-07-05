namespace VRT.Resume.Application.Common.Extensions;

public static class DictionaryExtensions
{
    public static TValue? GetValueOrDefault<TKey, TValue>(
        this IReadOnlyDictionary<TKey, TValue> dic, TKey key,
        Func<TValue>? defaultValueGetter = null)
    {
        if (null == dic) return default;

        if (dic.TryGetValue(key, out var value))
            return value;

        return defaultValueGetter == null
            ? default
            : defaultValueGetter();
    }
}