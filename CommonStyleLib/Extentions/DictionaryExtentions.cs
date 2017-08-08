using System;
using System.Collections.Generic;

namespace CommonLib.Extentions
{
    public static class DictionaryExtentions
    {
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dic, Action<TKey, TValue> act)
        {
            foreach (KeyValuePair<TKey, TValue> val in dic)
                act?.Invoke(val.Key, val.Value);
        }
    }
}
