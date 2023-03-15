using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models.Settings;
using _7dtd_svmanager_fix_mvvm.Views.Settings;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Settings
{
    public class ListScheduledCommandViewModel : ViewModelWindowStyleBase
    {
        public ListScheduledCommandViewModel(IWindowService windowService, ListScheduledCommandModel model) : base(windowService, model)
        {
        }
    }
}
