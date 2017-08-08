using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommonLib.Extentions
{
    public static class ObservableCollectionExtentions
    {
        public static void AddAll<TValue>(this ObservableCollection<TValue> collection, List<TValue> list)
        {
            list.ForEach((val) => collection.Add(val));
        }
    }
}
