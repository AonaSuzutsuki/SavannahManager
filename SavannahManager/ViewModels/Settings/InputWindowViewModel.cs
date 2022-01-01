using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Settings;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Settings
{
    public class InputWindowViewModel : ViewModelBase
    {
        #region Fields

        

        #endregion

        #region Event Properties

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Properties

        public bool IsCancel { get; set; } = true;

        public ReactiveProperty<string> InputText { get; set; }

        #endregion


        public InputWindowViewModel(IWindowService windowService, InputWindowModel model) : base(windowService, model)
        {
            InputText = new ReactiveProperty<string>("");

            OkCommand = new DelegateCommand(OkClick);
            CancelCommand = new DelegateCommand(CancelClick);
        }

        public void OkClick()
        {
            IsCancel = false;
            MainWindowCloseBt_Click();
        }

        public void CancelClick()
        {
            MainWindowCloseBt_Click();
        }
    }
}
