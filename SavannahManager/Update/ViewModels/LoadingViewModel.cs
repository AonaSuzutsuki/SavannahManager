using _7dtd_svmanager_fix_mvvm.Update.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.Update.ViewModels
{
    public class LoadingViewModel : ViewModelBase
    {
        private readonly LoadingModel _model;
        public LoadingViewModel(WindowService windowService, LoadingModel model) : base(windowService, model)
        {
            _model = model;
        }
    }
}
