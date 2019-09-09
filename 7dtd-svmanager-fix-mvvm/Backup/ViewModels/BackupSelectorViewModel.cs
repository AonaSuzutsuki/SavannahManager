using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using _7dtd_svmanager_fix_mvvm.Backup.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.Backup.ViewModels
{
    public class BackupSelectorViewModel : ViewModelBase
    {
        public BackupSelectorViewModel(Window view, BackupSelectorModel modelBase) : base(view, modelBase)
        {
        }
    }
}
