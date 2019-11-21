using System.Windows;
using _7dtd_svmanager_fix_mvvm.Setup.Models;
using Reactive.Bindings;
using System.Windows.Input;
using Prism.Commands;
using Reactive.Bindings.Extensions;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class InitializeWindowViewModel : ViewModelBase
    {
        public InitializeWindowViewModel(WindowService windowService, InitializeWindowModel model) : base(windowService, model)
        {
            this.model = model;

            PrevBTClick = new DelegateCommand(PrevBT_Click);
            NextBTClick = new DelegateCommand(NextBT_Click);
            ExitBTClick = new DelegateCommand(ExitBT_Click);

            PrevBTEnabled = model.ToReactivePropertyAsSynchronized(m => m.PrevBTEnabled);
            NextBTEnabled = model.ToReactivePropertyAsSynchronized(m => m.NextBTEnabled);
            ExitBTVisibility = model.ToReactivePropertyAsSynchronized(m => m.ExitBTVisibility);
            CancelBTVisibility = model.ToReactivePropertyAsSynchronized(m => m.CancelBTVisibility);
        }

        InitializeWindowModel model;

        public ReactiveProperty<bool> PrevBTEnabled { get; set; }
        public ReactiveProperty<bool> NextBTEnabled { get; set; }
        public ReactiveProperty<Visibility> ExitBTVisibility { get; set; }
        public ReactiveProperty<Visibility> CancelBTVisibility { get; set; }

        public ICommand PrevBTClick { get; set; }
        public ICommand NextBTClick { get; set; }
        public ICommand ExitBTClick { get; set; }

        private void PrevBT_Click()
        {
            model.PreviousPage();
        }
        private void NextBT_Click()
        {
            model.NextPage();
        }
        private void ExitBT_Click()
        {
            model.Save();
            WindowService.Close();
        }
    }
}
