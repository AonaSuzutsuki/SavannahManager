using System.Collections;
using _7dtd_svmanager_fix_mvvm.ViewModels.AutoRestart;

namespace _7dtd_svmanager_fix_mvvm.Models.Comparer;

public class MemoryDescComparer : IComparer
{
    public int Compare(object x, object y)
    {
        if (x is not ProcessSelectorInfo infoX || y is not ProcessSelectorInfo infoY)
            return 0;

        return infoX.Memory.CompareTo(infoY.Memory) * -1;
    }
}