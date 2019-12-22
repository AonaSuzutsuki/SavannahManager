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

            PrevBtClick = new DelegateCommand(PrevBt_Click);
            NextBtClick = new DelegateCommand(NextBt_Click);
            ExitBtClick = new DelegateCommand(ExitBt_Click);

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

        public ICommand PrevBtClick { get; set; }
        public ICommand NextBtClick { get; set; }
        public ICommand ExitBtClick { get; set; }

        private void PrevBt_Click()
        {
            model.PreviousPage();
        }
        private void NextBt_Click()
        {
            model.NextPage();
        }
        private void ExitBt_Click()
        {
            model.Save();
            WindowManageService.Close();
        }
    }
}
