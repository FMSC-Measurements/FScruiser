using System.Collections.Generic;

namespace FScruiser.Util
{
    public static class IDictionaryExtentions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key)
        {
            if (@this.TryGetValue(key, out TValue value))
            { return value; }
            else { return default(TValue); }
        }

        public static void SetValue<TKey, TValue>(this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if(@this.ContainsKey(key))
            {
                @this[key] = value;
            }
            else
            {
                @this.Add(key, value);
            }
        }
    }
}