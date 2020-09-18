using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class KeyConfigDictionary : Dictionary<string, IList<string>>
    {
        public int MinValueCount { get; private set; } = 0;

        public new void Add(string key, IList<string> values)
        {
            base.Add(key, values);

            if (MinValueCount <= 0 || MinValueCount > values.Count)
                MinValueCount = values.Count;
        }
    }
}
