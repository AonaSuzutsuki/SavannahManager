using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Setup.Models;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class FirstPageViewModel : NavigationPageViewModelBase
    {
        public FirstPageViewModel(NavigationWindowService<InitializeData> bindableValue) : base(bindableValue?.NavigationValue)
        {
        }
    }
}
