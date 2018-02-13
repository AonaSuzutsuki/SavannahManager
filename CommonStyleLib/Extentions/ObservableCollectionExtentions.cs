using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommonLib.Extentions
{
    public static class ObservableCollectionExtentions
    {
        public static void AddAll<TValue>(this ObservableCollection<TValue> collection, IEnumerable<TValue> list)
        {
            foreach (var elem in list)
                collection.Add(elem);
        }
    }
}
