using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Setup
{
    public class CanChangedEventArgs : EventArgs
    {
        public CanChangedEventArgs(bool canChanged)
        {
            CanChanged = canChanged;
        }
        public bool CanChanged { get; }
    }
    public delegate void CanChangedEventHandler(object sender, CanChangedEventArgs e);
}
