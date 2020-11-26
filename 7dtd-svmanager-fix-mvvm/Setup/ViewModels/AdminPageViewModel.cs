using System.Windows;
using _7dtd_svmanager_fix_mvvm.Setup.Models;
using _7dtd_svmanager_fix_mvvm.Setup.Views;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Windows.Input;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class AdminPageViewModel : NavigationPageViewModelBase
    {
        private readonly AdminPageModel model;
        public AdminPageViewModel(NavigationWindowService<InitializeData> bindableValue, AdminPageModel model) : base(bindableValue?.NavigationValue)
        {
            this.model = model;

            GetPathCommand = new DelegateCommand(GetPathBt_Click);
            AutoSearchCommand = new DelegateCommand(AutoSearchBt_Click);

            ServerConfigPathText = model.ToReactivePropertyAsSynchronized(m => m.ServerConfigPathText);
        }

        public ReactiveProperty<string> ServerConfigPathText { get; set; }

        public ICommand GetPathCommand { get; set; }
        public ICommand AutoSearchCommand { get; set; }

        public void GetPathBt_Click()
        {
            model.SelectAndGetFilePath();
        }
        public void AutoSearchBt_Click()
        {
            model.AutoSearchAndGetFilePath();
        }

        public override void RefreshValues()
        {
            BindableValue.NextBtVisibility = Visibility.Visible;
            BindableValue.BackBtVisibility = Visibility.Visible;
            BindableValue.CancelBtVisibility = Visibility.Visible;
            BindableValue.CloseBtVisibility = Visibility.Collapsed;
        }
    }
}
