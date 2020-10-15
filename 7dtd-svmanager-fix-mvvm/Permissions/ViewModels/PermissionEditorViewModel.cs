using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Permissions.Models;
using _7dtd_svmanager_fix_mvvm.Permissions.Views;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;

namespace _7dtd_svmanager_fix_mvvm.Permissions.ViewModels
{
    public abstract class PermissionBaseViewModel : PermissionBase
    {
        public IWindowService WindowManageService { get; }
        public ICommand TextChangedCommand { get; set; }

        protected PermissionBaseViewModel(PermissionBase permissionBase, IWindowService windowService)
        {
            WindowManageService = windowService;
            AddDummyAction = permissionBase.AddDummyAction;
            ItemType = permissionBase.ItemType;
            Name = permissionBase.Name;
            Permission = permissionBase.Permission;

            TextChangedCommand = new DelegateCommand(() =>
            {
                if (ItemType == PermissionItemType.Dummy)
                {
                    ItemType = PermissionItemType.Real;
                    AddDummyAction();
                }
            });
        }
    }

    public class PermissionInfoViewModel : PermissionBaseViewModel
    {
        public PermissionInfoViewModel(PermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
        }
    }

    public class AdminPermissionInfoViewModel : PermissionBaseViewModel
    {
        public string SteamId { get; set; }
        public int? DefaultPermission { get; set; }
        public int? ModeratorPermission { get; set; }

        public AdminPermissionInfoViewModel(AdminPermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
            SteamId = permissionBase.SteamId;
            DefaultPermission = permissionBase.DefaultPermission;
            ModeratorPermission = permissionBase.ModeratorPermission;
        }
    }

    public class BlackListPermissionInfoViewModel : PermissionBaseViewModel
    {
        public string SteamId { get; set; }
        public string UnBanDate { get; set; }
        public string Reason { get; set; }

        public ICommand SetDateTimeCommand { get; set; }
        public BlackListPermissionInfoViewModel(BlackListPermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
            SteamId = permissionBase.SteamId;
            UnBanDate = permissionBase.UnBanDate;
            Reason = permissionBase.Reason;

            SetDateTimeCommand = new DelegateCommand(() =>
            {
                var model = new ModelBase();
                var vm = new ViewModelBase(new WindowService(), model);
                windowService.ShowDialog<UnBanDateSetting>(vm);
            });
        }
    }

    public class PermissionEditorViewModel : ViewModelBase
    {
        public PermissionEditorViewModel(IWindowService windowService, PermissionEditorModel model) : base(windowService, model)
        {
            CommandPermissions = model.CommandPermissions.ToReadOnlyReactiveCollection(m
                =>new PermissionInfoViewModel(m, WindowManageService));
            AdminPermissions = model.AdminPermissions.ToReadOnlyReactiveCollection(m
                => new AdminPermissionInfoViewModel(m, WindowManageService));
            AdminGroupPermissions = model.AdminGroupPermissions.ToReadOnlyReactiveCollection(m
                => new AdminPermissionInfoViewModel(m, WindowManageService));
            WhitelistPermissions = model.WhitelistPermissions.ToReadOnlyReactiveCollection(m
                => new AdminPermissionInfoViewModel(m, WindowManageService));
            WhitelistGroupPermissions = model.WhitelistGroupPermissions.ToReadOnlyReactiveCollection(m
                => new AdminPermissionInfoViewModel(m, WindowManageService));
            BlacklistPermissions = model.BlacklistPermissions.ToReadOnlyReactiveCollection(m
                => new BlackListPermissionInfoViewModel(m, WindowManageService));
        }

        #region Fields



        #endregion

        #region Properties

        public ReadOnlyReactiveCollection<PermissionInfoViewModel> CommandPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfoViewModel> AdminPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfoViewModel> AdminGroupPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfoViewModel> WhitelistPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfoViewModel> WhitelistGroupPermissions { get; set; }
        public ReadOnlyReactiveCollection<BlackListPermissionInfoViewModel> BlacklistPermissions { get; set; }

        #endregion

        #region Event Properties



        #endregion

        #region Event Methods



        #endregion
    }
}
