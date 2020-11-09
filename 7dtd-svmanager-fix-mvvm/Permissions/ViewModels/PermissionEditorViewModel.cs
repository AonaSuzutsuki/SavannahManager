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
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Permissions.ViewModels
{
    public abstract class PermissionBaseViewModel : BindableBase
    {
        public PermissionBase.PermissionItemType ItemType { get; set; }

        public string Name { get; set; }

        public ReactiveProperty<string> SteamId { get; set; }
        public int? Permission { get; set; } = 0;

        public Action AddDummyAction { get; set; }
        public IWindowService WindowManageService { get; }
        public ICommand TextChangedCommand { get; set; }


        public Func<string> GetSteamIdFunc { get; set; }
        public ICommand GetSteamIdCommand { get; set; }

        protected PermissionBaseViewModel(PermissionBase permissionBase, IWindowService windowService)
        {
            SteamId = permissionBase.ToReactivePropertyAsSynchronized(m => m.SteamId);
            WindowManageService = windowService;
            AddDummyAction = permissionBase.AddDummyAction;
            ItemType = permissionBase.ItemType;
            Name = permissionBase.Name;
            Permission = permissionBase.Permission;

            TextChangedCommand = new DelegateCommand(() =>
            {
                if (ItemType == PermissionBase.PermissionItemType.Dummy)
                {
                    ItemType = PermissionBase.PermissionItemType.Real;
                    AddDummyAction();
                }
            });

            GetSteamIdCommand = new DelegateCommand(() =>
            {
                permissionBase.SteamId = GetSteamIdFunc();
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
        public int? DefaultPermission { get; set; }
        public int? ModeratorPermission { get; set; }


        public AdminPermissionInfoViewModel(AdminPermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
            DefaultPermission = permissionBase.DefaultPermission;
            ModeratorPermission = permissionBase.ModeratorPermission;
        }
    }

    public class BlackListPermissionInfoViewModel : PermissionBaseViewModel
    {
        public ReactiveProperty<string> UnBanDate { get; set; }
        public string Reason { get; set; }

        public ICommand SetDateTimeCommand { get; set; }
        public BlackListPermissionInfoViewModel(BlackListPermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
            UnBanDate = permissionBase.ToReactivePropertyAsSynchronized(m => m.UnBanDate);
            Reason = permissionBase.Reason;

            SetDateTimeCommand = new DelegateCommand(() =>
            {
                var model = new UnBanDateSettingModel();
                windowService.ShowDialog<UnBanDateSetting>(setting =>
                {
                    var vm = new UnBanDateSettingViewModel(new WindowService(), model);
                    var dateTime = UnBanDateSettingModel.ConvertStringToDateTime(permissionBase.UnBanDate);
                    if (dateTime.HasValue)
                    {
                        setting.SetDisplayDate(dateTime.Value);
                        model.SelectedDate = dateTime;
                        model.HourText = dateTime.Value.Hour;
                        model.MinuteText = dateTime.Value.Minute;
                        model.SecondText = dateTime.Value.Second;
                    }
                    return vm;
                });

                permissionBase.UnBanDate = model.ConvertToString();
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
                => new AdminPermissionInfoViewModel(m, WindowManageService)
                {
                    GetSteamIdFunc = GetSteamId
                });
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

        public string GetSteamId()
        {
            return "";
        }

        #endregion
    }
}
