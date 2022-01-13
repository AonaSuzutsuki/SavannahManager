using System.Windows;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Setup;
using CommonNavigationControlLib.Navigation.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Setup
{
    public class ExecutablePageViewModel : NavigationPageViewModelBase
    {
        public ExecutablePageViewModel(NavigationWindowService<InitializeData> bindableValue, ExecutablePageModel model) : base(bindableValue?.NavigationValue)
        {
            this.model = model;

            GetPathCommand = new DelegateCommand(GetPathBt_Click);
            AutoSearchCommand = new DelegateCommand(AutoSearchBt_Click);

            ServerFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ServerFilePathText).AddTo(CompositeDisposable);
        }

        public ReactiveProperty<string> ServerFilePathText { get; set; }

        public ICommand GetPathCommand { get; set; }
        public ICommand AutoSearchCommand { get; set; }

        private ExecutablePageModel model;

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
