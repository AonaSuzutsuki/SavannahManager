using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class KeyConfigDictionary : Dictionary<string, List<string>>
    {
        public int MaxValueCount { get; private set; }

        public new void Add(string key, List<string> values)
        {
            base.Add(key, values);

            if (MaxValueCount < values.Count)
                MaxValueCount = values.Count;
        }
    }
}
