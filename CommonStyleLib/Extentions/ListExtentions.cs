using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonLib.Extentions
{
    public static class ListExtentions
    {
        public static void ForEachInIndex<TValue>(this List<TValue> list, Action<int, TValue> act)
        {
            foreach (var item in list.Select((v, i) => new { v, i }))
                act?.Invoke(item.i, item.v);
        }
    }
}
