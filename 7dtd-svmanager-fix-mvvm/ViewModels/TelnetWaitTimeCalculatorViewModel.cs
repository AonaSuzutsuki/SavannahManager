using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class TelnetWaitTimeCalculatorViewModel : ViewModelBase
    {
        public TelnetWaitTimeCalculatorViewModel(IWindowService windowService, TelnetWaitTimeCalculatorModel model) : base(windowService, model)
        {
        }
    }
}
