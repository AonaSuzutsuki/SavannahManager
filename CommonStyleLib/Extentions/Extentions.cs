using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public static class ListExtentions
    {
        public static void ForEachInIndex<TValue>(this List<TValue> list, Action<int, TValue> act)
        {
            foreach (var item in list.Select((v, i) => new { v, i }))
                act?.Invoke(item.i, item.v);
        }
    }

    public static class ObservableCollectionExtentions
    {
        public static void AddAll<TValue>(this ObservableCollection<TValue> collection, List<TValue> list)
        {
            list.ForEach((val) => collection.Add(val));
        }
    }
}
