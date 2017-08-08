using System.Collections.Generic;
using System.Linq;

namespace CommonLib.Extentions
{
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
