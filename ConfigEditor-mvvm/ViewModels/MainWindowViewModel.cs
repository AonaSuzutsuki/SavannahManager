using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;

namespace ConfigEditor_mvvm.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(Window view, ModelBase modelBase) : base(view, modelBase)
        {
        }
    }
}
