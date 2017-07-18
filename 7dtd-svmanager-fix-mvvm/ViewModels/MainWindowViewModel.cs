using _7dtd_svmanager_fix_mvvm.Views;
using CommonStyleLib.ViewModels;
using LanguageEx;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
        public string SteamID { get; set; }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(Window view, Models.MainWindowModel model) : base(view, model)
        {
            model.AppendConsoleText += Model_AppendConsoleText;
            this.model = model;

            #region Event Initialize
            Loaded = new DelegateCommand(MainWindow_Loaded);
            Closing = new DelegateCommand(MainWindow_Closing);
            KeyDown = new DelegateCommand<KeyEventArgs>(MainWindow_KeyDown);

            MenuSettingsBTClick = new DelegateCommand(MenuSettingsBT_Click);
            MenuFirstSettingsBTClick = new DelegateCommand(MenuFirstSettingsBT_Click);
            MenuLangJapaneseBTClick = new DelegateCommand(MenuLangJapaneseBT_Click);
            MenuLangEnglishBTClick = new DelegateCommand(MenuLangEnglishBT_Click);
            MenuConfigEditorBTClick = new DelegateCommand(MenuConfigEditorBT_Click);
            MenuCheckUpdateBTClick = new DelegateCommand(MenuCheckUpdateBT_Click);
            MenuVersionInfoClick = new DelegateCommand(MenuVersionInfo_Click);

            StartBTClick = new DelegateCommand(StartBT_Click);
            StopBTClick = new DelegateCommand(StopBT_Click);
            TelnetBTClick = new DelegateCommand(TelnetBT_Click);
            CommandListBTClick = new DelegateCommand(CommandListBT_Click);

            PlayerListRefreshBTClick = new DelegateCommand(PlayerListRefreshBT_Click);

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
            #endregion

            #region Property Initialize
            StartBTEnabled = model.ToReactivePropertyAsSynchronized(m => m.StartBTEnabled);
            TelnetBTIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.TelnetBTIsEnabled);
            TelnetBTLabel = model.ToReactivePropertyAsSynchronized(m => m.TelnetBTLabel);

            AdminContextEnabled = model.ToReactivePropertyAsSynchronized(m => m.AdminContextEnabled);
            WhitelistContextEnabled = model.ToReactivePropertyAsSynchronized(m => m.WhitelistContextEnabled);
            KickContextEnabled = model.ToReactivePropertyAsSynchronized(m => m.KickContextEnabled);
            BanContextEnabled = model.ToReactivePropertyAsSynchronized(m => m.BanContextEnabled);
            WatchPlayerInfoContextEnabled = model.ToReactivePropertyAsSynchronized(m => m.WatchPlayerInfoContextEnabled);

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
            
            if (Loaded != null && Loaded.CanExecute(null))
            {
                Loaded?.Execute(null);
            }
        }
        
        #region Fields
        Models.MainWindowModel model;
        StringBuilder consoleLog = new StringBuilder();

        bool consoleIsFocus = false;
        #endregion

        #region EventProperties
        public ICommand Loaded { get; set; }
        public ICommand Closing { get; set; }
        public ICommand KeyDown { get; set; }

        public ICommand MenuSettingsBTClick { get; set; }
        public ICommand MenuFirstSettingsBTClick { get; set; }
        public ICommand MenuLangJapaneseBTClick { get; set; }
        public ICommand MenuLangEnglishBTClick { get; set; }
        public ICommand MenuConfigEditorBTClick { get; set; }
        public ICommand MenuCheckUpdateBTClick { get; set; }
        public ICommand MenuVersionInfoClick { get; set; }

        public ICommand StartBTClick { get; set; }
        public ICommand StopBTClick { get; set; }
        public ICommand TelnetBTClick { get; set; }
        public ICommand CommandListBTClick { get; set; }

        public ICommand PlayerListRefreshBTClick { get; set; }
        
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
        #endregion

        #region Properties
        public TextBox ConsoleTextBox { get; set; }
        
        public ReactiveProperty<bool> StartBTEnabled { get; set; }
        public ReactiveProperty<bool> TelnetBTIsEnabled { get; set; }
        public ReactiveProperty<string> TelnetBTLabel { get; set; }
        
        public ReactiveProperty<ObservableCollection<UserDetail>> UsersList { get; set; }
        public ReactiveProperty<int> UsersListSelectedIndex { get; set; }
        
        public ReactiveProperty<bool> AdminContextEnabled { get; set; }
        public ReactiveProperty<bool> WhitelistContextEnabled { get; set; }
        public ReactiveProperty<bool> KickContextEnabled { get; set; }
        public ReactiveProperty<bool> BanContextEnabled { get; set; }
        public ReactiveProperty<bool> WatchPlayerInfoContextEnabled { get; set; }
        
        public ReactiveProperty<string> ChatLogText { get; set; }
        public ReactiveProperty<string> ChatInputText { get; set; }

        private string consoleLogText;
        public string ConsoleLogText
        {
            get => consoleLogText;
            set
            {
                consoleLogText = value;
                OnPropertyChanged(this);
            }
        }
        private string cmdText;
        public string CmdText
        {
            get => cmdText;
            set
            {
                cmdText = value;
                OnPropertyChanged(this);
            }
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
        private void MainWindow_Loaded()
        {
            model.Initialize();
            model.RefreshLabels();
        }
        private void MainWindow_Closing()
        {
            model.SettingsSave();
        }
        private void MainWindow_KeyDown(KeyEventArgs e)
        {
            Console.WriteLine(e.Key);
        }

        private void MenuSettingsBT_Click()
        {
            var setWin = new Settings.Views.SettingWindow(model.Setting, model.ShortcutKeyManager);
            setWin.ShowDialog();
        }
        private void MenuFirstSettingsBT_Click()
        {
            var initializeWindow = new Setup.Views.InitializeWindow(model.Setting);
            initializeWindow.ShowDialog();
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

        }
        private void MenuCheckUpdateBT_Click()
        {

        }
        private void MenuVersionInfo_Click()
        {

        }

        private void StartBT_Click()
        {
            model.ServerStart();
        }
        private void StopBT_Click()
        {
            model.ServerStop(() =>
            {
                ForceShutdowner fs = new ForceShutdowner();
                fs.ShowDialog();
            });
        }
        private void TelnetBT_Click()
        {
            model.TelnetConnectOrDisconnect();
        }
        private void CommandListBT_Click()
        {

        }

        private void PlayerListRefreshBT_Click()
        {

        }

        private void PlayerContextMenu_Opened()
        {

        }
        private void AdminAddBT_Click()
        {

        }
        private void AdminRemoveBT_Click()
        {

        }
        private void WhiteListAddBT_Click()
        {

        }
        private void WhiteListRemoveBT_Click()
        {

        }
        private void KickBT_Click()
        {

        }
        private void BanBT_Click()
        {

        }
        private void KillBT_Click()
        {

        }
        private void WatchPlayerInfoBT_Click()
        {

        }
        
        private void ChatTextBoxEnter_Down(string e)
        {

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

        }

        private void CmdTextBox_EnterDown(string e)
        {
            model.SendCommand(e);
            CmdText = string.Empty;
        }

        private void GetTimeBT_Click()
        {

        }
        private void SetTimeBT_Click()
        {

        }
        private void SaveWorldBT_Click()
        {

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
