using System.Collections.Generic;

namespace FScruiser.Util
{
    public static class DictionaryExtentions
    {
        public static TValue GetValueOrDefault<TValue, TKey>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            if (@this.TryGetValue(key, out TValue value))
            { return value; }
            else { return default(TValue); }
        }
    }
}