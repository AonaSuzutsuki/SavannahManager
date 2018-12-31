using _7dtd_svmanager_fix_mvvm.PlayerController.Models.Pages;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels.Pages
{
    public class AdminAddViewModel : BindableBase
    {
        private AdminAddModel model;
        public AdminAddViewModel(AdminAddModel model)
        {
            this.model = model;

            NameText = model.ToReactivePropertyAsSynchronized(m => m.Name);
            PermissionText = model.ToReactivePropertyAsSynchronized(m => m.Permission);

            AddBtClicked = new DelegateCommand(model.AddAdmin);
            CancelBtClicked = new DelegateCommand(model.Cancel);
        }

        public ReactiveProperty<string> NameText { get; set; }
        public ReactiveProperty<int> PermissionText { get; set; }

        #region EventProperties
        public ICommand AddBtClicked { get; }
        public ICommand CancelBtClicked { get; }
        #endregion

        #region EventMethods
        #endregion
    }
}
