using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController.Pages;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController.Pages
{
    public class AdminAddViewModel : PageViewModelBase
    {
        private readonly AdminAddModel _model;
        public AdminAddViewModel(AdminAddModel model)
        {
            _model = model;

            NameText = model.ToReactivePropertyAsSynchronized(m => m.Name).AddTo(CompositeDisposable);
            PermissionText = model.ToReactivePropertyAsSynchronized(m => m.Permission).AddTo(CompositeDisposable);

            AddAdminCommand = new DelegateCommand(model.AddAdmin);
            CancelCommand = new DelegateCommand(model.Cancel);
        }

        public ReactiveProperty<string> NameText { get; set; }
        public ReactiveProperty<int> PermissionText { get; set; }

        #region EventProperties
        public ICommand AddAdminCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region EventMethods
        #endregion
    }
}
