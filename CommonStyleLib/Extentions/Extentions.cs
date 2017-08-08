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

    public static class ArrayExtentions
    {
        /// <summary>
        /// 比較元にだけ存在する要素を取り出す
        /// </summary>
        /// <typeparam name="T">抽出した要素配列</typeparam>
        /// <param name="ary1">比較元配列</param>
        /// <param name="ary2">比較配列</param>
        /// <returns></returns>
        public static T[] ArrayExcept<T>(this T[] ary1, T[] ary2)
        {
            //HashSetに変換する
            HashSet<T> hs1 = new HashSet<T>(ary1);
            HashSet<T> hs2 = new HashSet<T>(ary2);

            // h1にのみ存在する要素を取得
            var query1 = new HashSet<T>(hs1.Intersect(hs2));
            var query2 = new HashSet<T>(hs1.Except(query1));

            //配列に変換する
            T[] resultArray = new T[query2.Count];
            query2.CopyTo(resultArray, 0);
            return resultArray;
        }
    }
}
