using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Settings.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Reactive.Bindings;

namespace _7dtd_svmanager_fix_mvvm.Settings.ViewModels
{
    public class PermissionEditorViewModel : ViewModelBase
    {
        public PermissionEditorViewModel(IWindowService windowService, PermissionEditorModel model) : base(windowService, model)
        {

            CommandPermissions = model.CommandPermissions.ToReadOnlyReactiveCollection(m => m);
            AdminPermissions = model.AdminPermissions.ToReadOnlyReactiveCollection(m => m);
            AdminGroupPermissions = model.AdminGroupPermissions.ToReadOnlyReactiveCollection(m => m);
            WhitelistPermissions = model.WhitelistPermissions.ToReadOnlyReactiveCollection(m => m);
            WhitelistGroupPermissions = model.WhitelistGroupPermissions.ToReadOnlyReactiveCollection(m => m);
            BlacklistPermissions = model.BlacklistPermissions.ToReadOnlyReactiveCollection(m => m);
        }

        #region Fields



        #endregion

        #region Properties

        public ReadOnlyReactiveCollection<PermissionInfo> CommandPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfo> AdminPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfo> AdminGroupPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfo> WhitelistPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfo> WhitelistGroupPermissions { get; set; }
        public ReadOnlyReactiveCollection<BlackListPermissionInfo> BlacklistPermissions { get; set; }

        #endregion

        #region Event Properties



        #endregion

        #region Event Methods



        #endregion
    }
}
