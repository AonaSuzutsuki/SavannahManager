using System.Windows;
using _7dtd_svmanager_fix_mvvm.Setup.Models;
using Reactive.Bindings;
using System.Windows.Input;
using Reactive.Bindings.Extensions;
using Prism.Commands;
using _7dtd_svmanager_fix_mvvm.Setup.Views;
using CommonStyleLib.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class Page2ViewModel : NavigationPageViewModelBase
    {
        public Page2ViewModel(NavigationWindowService<InitializeData> bindableValue, Page2Model model) : base(bindableValue?.NavigationValue)
        {
            this.model = model;

            GetPathCommand = new DelegateCommand(GetPathBt_Click);
            AutoSearchCommand = new DelegateCommand(AutoSearchBt_Click);

            ServerFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ServerFilePathText);
        }

        public ReactiveProperty<string> ServerFilePathText { get; set; }

        public ICommand GetPathCommand { get; set; }
        public ICommand AutoSearchCommand { get; set; }

        private Page2Model model;

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
