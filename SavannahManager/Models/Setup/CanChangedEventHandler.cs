using System;

namespace _7dtd_svmanager_fix_mvvm.Models.Setup
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
