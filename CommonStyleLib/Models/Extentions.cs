using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Models
{
    public static class Extentions
    {
        public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dic, Action<TKey, TValue> act)
        {
            foreach (KeyValuePair<TKey, TValue> val in dic)
                act?.Invoke(val.Key, val.Value);
        }

        public static void ForEachInIndex<TValue>(this List<TValue> list, Action<int, TValue> act)
        {
            foreach (var item in list.Select((v, i) => new { v, i }))
                act?.Invoke(item.i, item.v);
        }

        public static void AddAll<TValue>(this ObservableCollection<TValue> collection, List<TValue> list)
        {
            list.ForEach((val) => collection.Add(val));
        }
    }
}
