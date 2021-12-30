using System.Windows;
using _7dtd_svmanager_fix_mvvm.Models.Setup;
using CommonNavigationControlLib.Navigation.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Setup
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
