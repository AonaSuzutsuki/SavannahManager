using _7dtd_svmanager_fix_mvvm.Views;
using CommonStyleLib.ViewModels;
using LanguageEx;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using _7dtd_svmanager_fix_mvvm.Backup.Models;
using _7dtd_svmanager_fix_mvvm.Backup.ViewModels;
using _7dtd_svmanager_fix_mvvm.Backup.Views;
using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages;
using _7dtd_svmanager_fix_mvvm.Settings.Models;
using _7dtd_svmanager_fix_mvvm.Settings.ViewModels;
using _7dtd_svmanager_fix_mvvm.Settings.Views;
using _7dtd_svmanager_fix_mvvm.Setup.ViewModels;
using CommonStyleLib.Views;
using _7dtd_svmanager_fix_mvvm.Setup.Views;
using _7dtd_svmanager_fix_mvvm.Update.Models;
using _7dtd_svmanager_fix_mvvm.Update.ViewModels;
using _7dtd_svmanager_fix_mvvm.Update.Views;
using CommonStyleLib.ExMessageBox;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class UserDetail
    {
        public string ID { get; set; }
        public string Level { get; set; }
        public string Name { get; set; }
        public string Health { get; set; }
        public string ZombieKills { get; set; }
        public string PlayerKills { get; set; }
        public string Death { get; set; }
        public string Score { get; set; }
        public string Coord { set; get; }
        public string SteamId { get; set; }

        public override string ToString()
        {
            return $"{ID} {Level} {Name} {Health} {ZombieKills} {PlayerKills} {Death} {Score} {Coord} {SteamId}\r\n";
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(WindowService windowService, Models.MainWindowModel model, MainWindow view) : base(windowService, model)
        {
            model.AppendConsoleText += Model_AppendConsoleText;
            this.view = view;
            this.model = model;

            #region Event Initialize
            //Loaded = new DelegateCommand(MainWindow_Loaded);
            Closing = new DelegateCommand(MainWindow_Closing);
            KeyDown = new DelegateCommand<KeyEventArgs>(MainWindow_KeyDown);

            MenuSettingsBTClick = new DelegateCommand(MenuSettingsBT_Click);
            MenuFirstSettingsBTClick = new DelegateCommand(MenuFirstSettingsBT_Click);
            MenuLangJapaneseBTClick = new DelegateCommand(MenuLangJapaneseBT_Click);
            MenuLangEnglishBTClick = new DelegateCommand(MenuLangEnglishBT_Click);
            MenuConfigEditorBTClick = new DelegateCommand(MenuConfigEditorBT_Click);
            MenuBackupEditorBTClick = new DelegateCommand(MenuBackupEditorBT_Click);
            MenuCheckUpdateBTClick = new DelegateCommand(MenuCheckUpdateBT_Click);
            MenuVersionInfoClick = new DelegateCommand(MenuVersionInfo_Click);

            StartBTClick = new DelegateCommand(StartBT_Click);
            StopBTClick = new DelegateCommand(StopBT_Click);
            TelnetBTClick = new DelegateCommand(TelnetBT_Click);
            CommandListBTClick = new DelegateCommand(CommandListBT_Click);

            PlayerListRefreshBtClick = new DelegateCommand(PlayerListRefreshBt_Click);

            PlayerContextMenuOpened = new DelegateCommand(PlayerContextMenu_Opened);
            AdminAddBTClick = new DelegateCommand(AdminAddBT_Click);
            AdminRemoveBTClick = new DelegateCommand(AdminRemoveBT_Click);
            WhiteListAddBTClick = new DelegateCommand(WhiteListAddBT_Click);
            WhiteListRemoveBTClick = new DelegateCommand(WhiteListRemoveBT_Click);
            KickBTClick = new DelegateCommand(KickBT_Click);
            BanBTClick = new DelegateCommand(BanBT_Click);
            KillBTClick = new DelegateCommand(KillBT_Click);
            WatchPlayerInfoBTClick = new DelegateCommand(WatchPlayerInfoBT_Click);

            ChatTextBoxEnterDown = new DelegateCommand<string>(ChatTextBoxEnter_Down);

            ConsoleTextBoxMouseEnter = new DelegateCommand(ConsoleTextBoxMouse_Enter);
            ConsoleTextBoxMouseLeave = new DelegateCommand(ConsoleTextBoxMouse_Leave);
            DeleteLogBTClick = new DelegateCommand(DeleteLogBT_Click);

            CmdTextBoxEnterDown = new DelegateCommand<string>(CmdTextBox_EnterDown);

            GetTimeBTClick = new DelegateCommand(GetTimeBT_Click);
            SetTimeBTClick = new DelegateCommand(SetTimeBT_Click);
            SaveWorldBTClick = new DelegateCommand(SaveWorldBT_Click);

            GetIpClicked = new DelegateCommand(GetIp_Clicked);
            CheckPortClicked = new DelegateCommand(CheckPort_Clicked);
            #endregion

            #region Property Initialize

            IsBeta = model.ObserveProperty(m => m.IsBeta).ToReactiveProperty();

            StartBTEnabled = model.ToReactivePropertyAsSynchronized(m => m.StartBtEnabled);
            TelnetBTIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.TelnetBtIsEnabled);
            TelnetBTLabel = model.ToReactivePropertyAsSynchronized(m => m.TelnetBtLabel);

            UsersList = model.ToReactivePropertyAsSynchronized(m => m.UsersList);
            
            ChatLogText = model.ObserveProperty(m => m.ChatLogText).ToReactiveProperty();
            ChatInputText = model.ToReactivePropertyAsSynchronized(m => m.ChatInputText);
            
            ConnectionPanelIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.ConnectionPanelIsEnabled);
            LocalModeChecked = model.ToReactivePropertyAsSynchronized(m => m.LocalMode);
            LocalModeEnabled = model.ToReactivePropertyAsSynchronized(m => m.LocalModeEnabled);
            TelnetAddressText = model.ToReactivePropertyAsSynchronized(m => m.Address);
            TelnetPortText = model.ToReactivePropertyAsSynchronized(m => m.PortText);
            TelnetPasswordText = model.ToReactivePropertyAsSynchronized(m => m.Password);

            TimeDayText = model.ToReactivePropertyAsSynchronized(m => m.TimeDayText);
            TimeHourText = model.ToReactivePropertyAsSynchronized(m => m.TimeHourText);
            TimeMinuteText = model.ToReactivePropertyAsSynchronized(m => m.TimeMinuteText);

            BottomNewsLabel = model.ToReactivePropertyAsSynchronized(m => m.BottomNewsLabel);
            #endregion

            DoLoaded();
        }

        #region Fields
        private readonly MainWindow view;
        private readonly Models.MainWindowModel model;
        private StringBuilder consoleLog = new StringBuilder();

        private bool consoleIsFocus = false;
        #endregion

        #region EventProperties
        public ICommand MenuSettingsBTClick { get; set; }
        public ICommand MenuFirstSettingsBTClick { get; set; }
        public ICommand MenuLangJapaneseBTClick { get; set; }
        public ICommand MenuLangEnglishBTClick { get; set; }
        public ICommand MenuConfigEditorBTClick { get; set; }
        public ICommand MenuBackupEditorBTClick { get; set; }
        public ICommand MenuCheckUpdateBTClick { get; set; }
        public ICommand MenuVersionInfoClick { get; set; }

        public ICommand StartBTClick { get; set; }
        public ICommand StopBTClick { get; set; }
        public ICommand TelnetBTClick { get; set; }
        public ICommand CommandListBTClick { get; set; }

        public ICommand PlayerListRefreshBtClick { get; set; }
        
        public ICommand PlayerContextMenuOpened { get; set; }
        public ICommand AdminAddBTClick { get; set; }
        public ICommand AdminRemoveBTClick { get; set; }
        public ICommand WhiteListAddBTClick { get; set; }
        public ICommand WhiteListRemoveBTClick { get; set; }
        public ICommand KickBTClick { get; set; }
        public ICommand BanBTClick { get; set; }
        public ICommand KillBTClick { get; set; }
        public ICommand WatchPlayerInfoBTClick { get; set; }

        public ICommand ChatTextBoxEnterDown { get; set; }

        public ICommand ConsoleTextBoxMouseEnter { get; set; }
        public ICommand ConsoleTextBoxMouseLeave { get; set; }
        public ICommand DeleteLogBTClick { get; set; }

        public ICommand CmdTextBoxEnterDown { get; set; }

        public ICommand GetTimeBTClick { get; set; }
        public ICommand SetTimeBTClick { get; set; }
        public ICommand SaveWorldBTClick { get; set; }

        public ICommand GetIpClicked { get; set; }
        public ICommand CheckPortClicked { get; set; }
        #endregion

        #region Properties
        public ReactiveProperty<bool> IsBeta { get; set; }

        public TextBox ConsoleTextBox { get; set; }
        
        public ReactiveProperty<bool> StartBTEnabled { get; set; }
        public ReactiveProperty<bool> TelnetBTIsEnabled { get; set; }
        public ReactiveProperty<string> TelnetBTLabel { get; set; }
        
        public ReactiveProperty<ObservableCollection<UserDetail>> UsersList { get; set; }
        private int usersListSelectedIndex = -1;
        public int UsersListSelectedIndex
        {
            get => usersListSelectedIndex;
            set => SetProperty(ref usersListSelectedIndex, value);
        }

        private bool adminContextEnabled;
        public bool AdminContextEnabled
        {
            get => adminContextEnabled;
            set => SetProperty(ref adminContextEnabled, value);
        }
        private bool whitelistContextEnabled;
        public bool WhitelistContextEnabled
        {
            get => whitelistContextEnabled;
            set => SetProperty(ref whitelistContextEnabled, value);
        }
        private bool kickContextEnabled;
        public bool KickContextEnabled
        {
            get => kickContextEnabled;
            set => SetProperty(ref kickContextEnabled, value);
        }
        private bool banContextEnabled;
        public bool BanContextEnabled
        {
            get => banContextEnabled;
            set => SetProperty(ref banContextEnabled, value);
        }
        private bool watchPlayerInfoContextEnabled;
        public bool WatchPlayerInfoContextEnabled
        {
            get => watchPlayerInfoContextEnabled;
            set => SetProperty(ref watchPlayerInfoContextEnabled, value);
        }
        
        public ReactiveProperty<string> ChatLogText { get; set; }
        public ReactiveProperty<string> ChatInputText { get; set; }

        private string consoleLogText;
        public string ConsoleLogText
        {
            get => consoleLogText;
            set => SetProperty(ref consoleLogText, value);
        }
        private string cmdText;
        public string CmdText
        {
            get => cmdText;
            set => SetProperty(ref cmdText, value);
        }
        
        public ReactiveProperty<bool> ConnectionPanelIsEnabled { get; set; }
        public ReactiveProperty<bool> LocalModeChecked { get; set; }
        public ReactiveProperty<bool> LocalModeEnabled { get; set; }
        public ReactiveProperty<string> TelnetAddressText { get; set; }
        public ReactiveProperty<string> TelnetPortText { get; set; }
        public ReactiveProperty<string> TelnetPasswordText { get; set; }
        
        public ReactiveProperty<string> TimeDayText { get; set; }
        public ReactiveProperty<string> TimeHourText { get; set; }
        public ReactiveProperty<string> TimeMinuteText { get; set; }
        
        public ReactiveProperty<string> BottomNewsLabel { get; set; }
        #endregion

        #region EventMethods
        protected override void MainWindow_Loaded()
        {
            model.Initialize();

            model.RefreshLabels();

            if (model.Setting.IsFirstBoot)
                MenuFirstSettingsBT_Click();

            var task = model.CheckUpdate();
            task.ContinueWith(continueTask =>
            {
                var dialogResult = continueTask.Result;
                if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
                {
                    WindowManageService.Dispatch(() =>
                    {
                        var updFormModel = new UpdFormModel();
                        var vm = new UpdFormViewModel(new WindowService(), updFormModel);
                        WindowManageService.Show<UpdForm>(vm);
                    });
                }
            });
        }
        protected override void MainWindow_Closing()
        {
            model.SettingsSave();
            model.Dispose();
        }
        protected override void MainWindow_KeyDown(KeyEventArgs e)
        {
            model.PushShortcutKey(e.Key);
        }

        private void MenuSettingsBT_Click()
        {
            var setting = model.Setting;
            var keyManager = model.ShortcutKeyManager;

            var settingModel = new SettingModel(setting, keyManager);
            var vm = new SettingWindowViewModel(new WindowService(), settingModel);
            WindowManageService.ShowDialog<SettingWindow>(vm);
            model.IsBeta = setting.IsBetaMode;
        }
        private void MenuFirstSettingsBT_Click()
        {
            var setting = model.Setting;
            WindowManageService.ShowDialog<InitializeWindow>(window =>
            {
                var initModel = new Setup.Models.InitializeWindowModel(setting, window.MainFrame.NavigationService);
                return new InitializeWindowViewModel(new WindowService(), initModel);
            });
        }
        private void MenuLangJapaneseBT_Click()
        {
            model.ChangeCulture(ResourceService.Japanese);
            model.RefreshLabels();
        }
        private void MenuLangEnglishBT_Click()
        {
            model.ChangeCulture(ResourceService.English);
            model.RefreshLabels();
        }
        private void MenuConfigEditorBT_Click()
        {
            model.RunConfigEditor();
        }

        private void MenuBackupEditorBT_Click()
        {
            var setting = model.Setting;
            var backupModel = new BackupSelectorModel(setting);
            var vm = new BackupSelectorViewModel(new WindowService(), backupModel);
            WindowManageService.Show<BackupSelector>(vm);
        }
        private void MenuCheckUpdateBT_Click()
        {
            var updFormModel = new UpdFormModel();
            var vm = new UpdFormViewModel(new WindowService(), updFormModel);
            WindowManageService.Show<UpdForm>(vm);
        }
        private void MenuVersionInfo_Click()
        {
            var model = new Models.VersionInfoModel();
            var vm = new VersionInfoViewModel(new WindowService(), model);
            WindowManageService.ShowDialog<VersionInfo>(vm);
        }

        private void StartBT_Click()
        {
            model.ServerStart();
        }
        private void StopBT_Click()
        {
            var isForceShutdown = model.ServerStop();
            if (!isForceShutdown)
                return;

            var forceShutdownerModel = new ForceShutdownerModel();
            var vm = new ForceShutdownerViewModel(new WindowService(), forceShutdownerModel);
            WindowManageService.Show<ForceShutdowner>(vm);
        }
        private void TelnetBT_Click()
        {
            model.TelnetConnectOrDisconnect();
        }
        private void CommandListBT_Click()
        {

        }

        private void PlayerListRefreshBt_Click()
        {
            model.PlayerRefresh();
        }

        private void PlayerContextMenu_Opened()
        {
            int index = UsersListSelectedIndex;
            if (index < 0)
            {
                AdminContextEnabled = false;
                WhitelistContextEnabled = false;
                KickContextEnabled = false;
                BanContextEnabled = false;
                WatchPlayerInfoContextEnabled = false;
            }
            else
            {
                AdminContextEnabled = true;
                WhitelistContextEnabled = true;
                KickContextEnabled = true;
                BanContextEnabled = true;
                WatchPlayerInfoContextEnabled = true;
            }
        }
        private void AdminAddBT_Click()
        {
            var playerInfo = model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var playerBaseModel = new PlayerBaseModel();
            var adminAdd = new AdminAdd(model, AddType.Type.Admin, name);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = adminAdd;
                window.Navigate();
                return new PlayerBaseViewModel(new WindowService(), playerBaseModel)
                {
                    WindowTitle = "Add"
                };
            });
        }
        private void AdminRemoveBT_Click()
        {
            model.RemoveAdmin(UsersListSelectedIndex);
        }
        private void WhiteListAddBT_Click()
        {
            var playerInfo = model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var playerBaseModel = new PlayerBaseModel();
            var whitelistAdd = new AdminAdd(model, AddType.Type.Whitelist, name);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = whitelistAdd;
                window.Navigate();
                return new PlayerBaseViewModel(new WindowService(), playerBaseModel)
                {
                    WindowTitle = "Whitelist"
                };
            });
        }
        private void WhiteListRemoveBT_Click()
        {
            model.RemoveWhitelist(UsersListSelectedIndex);
        }
        private void KickBT_Click()
        {
            var playerInfo = model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var playerBaseModel = new PlayerBaseModel();
            var kick = new Kick(model, name);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = kick;
                window.Navigate();
                return new PlayerBaseViewModel(new WindowService(), playerBaseModel)
                {
                    WindowTitle = "Kick"
                };
            });
        }
        private void BanBT_Click()
        {
            var playerInfo = model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var playerBaseModel = new PlayerBaseModel();
            var ban = new Ban(model, name);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = ban;
                window.Navigate();
                return new PlayerBaseViewModel(new WindowService(), playerBaseModel)
                {
                    WindowTitle = "Ban"
                };
            });
        }
        private void KillBT_Click()
        {

        }
        private void WatchPlayerInfoBT_Click()
        {
            model.ShowPlayerInfo(UsersListSelectedIndex);
        }
        
        private void ChatTextBoxEnter_Down(string e)
        {
            model.SendChat(e, () => view.ChatTextBox.Text = "");
        }

        private void ConsoleTextBoxMouse_Enter()
        {
            consoleIsFocus = true;
        }
        private void ConsoleTextBoxMouse_Leave()
        {
            consoleIsFocus = false;
        }
        private void DeleteLogBT_Click()
        {
            consoleLog.Clear();
            ConsoleLogText = "";
        }

        private void CmdTextBox_EnterDown(string e)
        {
            model.SendCommand(e);
            CmdText = string.Empty;
        }

        private void GetTimeBT_Click()
        {
            model.SetTimeToTextBox();
        }
        private void SetTimeBT_Click()
        {
            model.SetTimeToGame();
        }
        private void SaveWorldBT_Click()
        {
            model.SendCommand("saveworld");
        }

        private void GetIp_Clicked()
        {
            var ipAddressGetterModel = new IpAddressGetterModel();
            var vm = new IpAddressGetterViewModel(new WindowService(), ipAddressGetterModel);
            WindowManageService.Show<IpAddressGetter>(vm);
        }
        private void CheckPort_Clicked()
        {
            var portCheckModel = new PortCheckModel();
            var vm = new PortCheckViewModel(new WindowService(), portCheckModel);
            WindowManageService.Show<PortCheck>(vm);
        }


        private void Model_AppendConsoleText(object sender, Models.MainWindowModel.AppendedLogTextEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.AppendedLogText))
            {
                view.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    AppendConsoleText(e.AppendedLogText, e.MaxLength);
                }));
            }
        }
        #endregion

        #region Methods
        private void AppendConsoleText(string text, int maxLength)
        {
            if (consoleLog == null)
            {
                consoleLog = new StringBuilder(maxLength * 2);
            }
            consoleLog.Append(text);
            if (consoleLog.Length > maxLength)
            {
                consoleLog.Remove(0, consoleLog.Length - maxLength);
            }

            ConsoleLogText = consoleLog.ToString();

            if (!consoleIsFocus)
            {
                if (!consoleIsFocus)
                {
                    ConsoleTextBox?.Select(ConsoleLogText.Length, 0);
                    ConsoleTextBox?.ScrollToEnd();
                }
            }
        }
        #endregion
    }
}
