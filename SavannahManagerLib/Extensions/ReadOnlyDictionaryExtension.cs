using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.Extensions
{
    public static class ReadOnlyDictionaryExtension
    {
        public static TV Get<TK, TV>(this IReadOnlyDictionary<TK, TV> dict, TK key, TV defaultValue = default)
        {
            if (dict.ContainsKey(key))
                return dict[key];
            return defaultValue;
        }
    }
}
