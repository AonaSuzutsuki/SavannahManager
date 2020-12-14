using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using _7dtd_svmanager_fix_mvvm.Setup.Models;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class FinishPageViewModel : NavigationPageViewModelBase
    {
        public FinishPageViewModel(NavigationWindowService<InitializeData> bindableValue, FinishPageModel model) : base(bindableValue?.NavigationValue)
        {
            _model = model;

            BindableValue.CloseAction = Close;
        }

        #region Fields

        private FinishPageModel _model;

        #endregion

        public void Close()
        {
            _model.ApplySetting();
        }

        public override void RefreshValues()
        {
            BindableValue.NextBtVisibility = Visibility.Visible;
            BindableValue.BackBtVisibility = Visibility.Visible;
            BindableValue.CancelBtVisibility = Visibility.Collapsed;
            BindableValue.CloseBtVisibility = Visibility.Visible;
        }
    }
}
