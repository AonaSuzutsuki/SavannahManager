using _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models.Comparer
{
    public class FileSizeAscComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (x is not LogFileItem logFileItemX || y is not LogFileItem logFileItemY)
                return 0;

            return logFileItemX.FileSize.CompareTo(logFileItemY.FileSize);
        }
    }
}
