using _7dtd_svmanager_fix_mvvm.Update.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _7dtd_svmanager_fix_mvvm.Update.ViewModels
{
    public class LoadingViewModel : ViewModelBase
    {
        LoadingModel model;
        public LoadingViewModel(Window view, LoadingModel model) : base(view, model)
        {
            this.model = model;
        }
    }
}
