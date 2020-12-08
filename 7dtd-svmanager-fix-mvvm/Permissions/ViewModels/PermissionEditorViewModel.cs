using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Permissions.Models;
using _7dtd_svmanager_fix_mvvm.Permissions.Views;
using CommonStyleLib.File;
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
        public ReactiveProperty<string> Name { get; set; }

        public ReactiveProperty<string> SteamId { get; set; }
        public ReactiveProperty<string> Permission { get; set; }

        public IWindowService WindowManageService { get; }
        public ICommand TextChangedCommand { get; set; }
        public ICommand LostFocusCommand { get; set; }


        public Func<string, string> GetSteamIdFunc { get; set; }
        public Action TextBoxChangedAction { get; set; }
        public ICommand GetSteamIdCommand { get; set; }

        private readonly PermissionBase _permissionBase;

        protected PermissionBaseViewModel(PermissionBase permissionBase, IWindowService windowService)
        {
            _permissionBase = permissionBase;

            SteamId = permissionBase.ToReactivePropertyAsSynchronized(m => m.SteamId);
            WindowManageService = windowService;
            Name = permissionBase.ToReactivePropertyAsSynchronized(m => m.Name);
            Permission = permissionBase.ToReactivePropertyAsSynchronized(m => m.Permission);

            TextChangedCommand = new DelegateCommand(() =>
            {
                if (permissionBase.ItemType == PermissionBase.PermissionItemType.Dummy)
                {
                    permissionBase.ItemType = PermissionBase.PermissionItemType.Real;
                    permissionBase.AddDummyAction?.Invoke();
                }
                TextBoxChangedAction?.Invoke();
            });

            LostFocusCommand = new DelegateCommand(LostFocus);

            GetSteamIdCommand = new DelegateCommand(() =>
            {
                permissionBase.SteamId = GetSteamIdFunc(permissionBase.SteamId);
            });
        }

        protected virtual void LostFocus()
        {
            if (_permissionBase.CanRemove())
                _permissionBase.RemoveAction?.Invoke();
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
        public ReactiveProperty<string> DefaultPermission { get; set; }
        public ReactiveProperty<string> ModeratorPermission { get; set; }


        public AdminPermissionInfoViewModel(AdminPermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
        }

        public AdminPermissionInfoViewModel(AdminGroupPermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
            DefaultPermission = permissionBase.ToReactivePropertyAsSynchronized(m => m.DefaultPermission);
            ModeratorPermission = permissionBase.ToReactivePropertyAsSynchronized(m => m.ModeratorPermission);
        }
    }

    public class WhitelistPermissionInfoViewModel : PermissionBaseViewModel
    {
        public WhitelistPermissionInfoViewModel(WhitelistPermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
        }
        public WhitelistPermissionInfoViewModel(WhitelistGroupPermissionInfo permissionBase, IWindowService windowService) : base(permissionBase, windowService)
        {
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
            _model = model;

            IsEdited = model.ObserveProperty(m => m.IsEdited).ToReactiveProperty();
            CanSave = model.ObserveProperty(m => m.CanSave).ToReactiveProperty();

            CommandPermissions = model.CommandPermissions.ToReadOnlyReactiveCollection(m
                => new PermissionInfoViewModel(m, WindowManageService));
            AdminPermissions = model.AdminPermissions.ToReadOnlyReactiveCollection(m
                => new AdminPermissionInfoViewModel(m, WindowManageService)
                {
                    GetSteamIdFunc = GetSteamId,
                    TextBoxChangedAction = Edited
                });
            AdminGroupPermissions = model.AdminGroupPermissions.ToReadOnlyReactiveCollection(m
                => new AdminPermissionInfoViewModel(m, WindowManageService)
                {
                    GetSteamIdFunc = GetSteamGroupId,
                    TextBoxChangedAction = Edited
                });
            WhitelistPermissions = model.WhitelistPermissions.ToReadOnlyReactiveCollection(m
                => new WhitelistPermissionInfoViewModel(m, WindowManageService)
                {
                    GetSteamIdFunc = GetSteamId,
                    TextBoxChangedAction = Edited
                });
            WhitelistGroupPermissions = model.WhitelistGroupPermissions.ToReadOnlyReactiveCollection(m
                => new WhitelistPermissionInfoViewModel(m, WindowManageService)
                {
                    GetSteamIdFunc = GetSteamGroupId,
                    TextBoxChangedAction = Edited
                });
            BlacklistPermissions = model.BlacklistPermissions.ToReadOnlyReactiveCollection(m
                => new BlackListPermissionInfoViewModel(m, WindowManageService)
                {
                    GetSteamIdFunc = GetSteamId,
                    TextBoxChangedAction = Edited
                });

            NewFileCommand = new DelegateCommand(NewFile);
            OpenFileCommand = new DelegateCommand(OpenFile);
            SaveFileCommand = new DelegateCommand(SaveFile);
            SaveAsFileCommand = new DelegateCommand(SaveAsFile);
        }

        #region Fields

        private readonly PermissionEditorModel _model;

        #endregion

        #region Properties

        public ReactiveProperty<bool> IsEdited { get; set; }
        public ReactiveProperty<bool> CanSave { get; set; }

        public ReadOnlyReactiveCollection<PermissionInfoViewModel> CommandPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfoViewModel> AdminPermissions { get; set; }
        public ReadOnlyReactiveCollection<AdminPermissionInfoViewModel> AdminGroupPermissions { get; set; }
        public ReadOnlyReactiveCollection<WhitelistPermissionInfoViewModel> WhitelistPermissions { get; set; }
        public ReadOnlyReactiveCollection<WhitelistPermissionInfoViewModel> WhitelistGroupPermissions { get; set; }
        public ReadOnlyReactiveCollection<BlackListPermissionInfoViewModel> BlacklistPermissions { get; set; }

        #endregion

        #region Event Properties

        public ICommand NewFileCommand { get; set; }
        public ICommand OpenFileCommand { get; set; }
        public ICommand SaveFileCommand { get; set; }
        public ICommand SaveAsFileCommand { get; set; }

        #endregion

        #region Event Methods

        public void NewFile()
        {
            _model.NewFile();
        }

        public void OpenFile()
        {
            var dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\7DaysToDie\\Saves";
            var filePath = FileSelector.GetFilePath(dirPath,
                LangResources.SettingsResources.Filter_XmlFile,"serveradmin.xml", FileSelector.FileSelectorType.Read);
            _model.OpenFile(filePath);
        }

        public void SaveFile()
        {
            _model.Save();
        }

        public void SaveAsFile()
        {
            var dirPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\7DaysToDie\\Saves";
            var filePath = FileSelector.GetFilePath(dirPath,
                LangResources.SettingsResources.Filter_XmlFile, "serveradmin.xml", FileSelector.FileSelectorType.Write);
            _model.Save(filePath);
        }

        public void Edited()
        {
            _model.IsEdited = true;
        }

        public string GetSteamId(string currentId)
        {
            var steamIdModel = new GetProfileSteamIdModel();
            var vm = new GetSteamIdViewModel(new WindowService(), steamIdModel);
            WindowManageService.ShowDialogNonOwner<GetSteamId>(vm);

            return steamIdModel.IsWritten ? steamIdModel.Steam64Id : currentId;
        }

        public string GetSteamGroupId(string currentId)
        {
            var steamIdModel = new GetGroupSteamIdModel();
            var vm = new GetSteamIdViewModel(new WindowService(), steamIdModel);
            WindowManageService.ShowDialogNonOwner<GetSteamId>(vm);

            return steamIdModel.IsWritten ? steamIdModel.Steam64Id : currentId;
        }

        #endregion
    }
}
