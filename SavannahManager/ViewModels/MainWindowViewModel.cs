using _7dtd_svmanager_fix_mvvm.Views;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using _7dtd_svmanager_fix_mvvm.LangResources;
using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.Models.AutoRestart;
using _7dtd_svmanager_fix_mvvm.Models.Backup;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer;
using _7dtd_svmanager_fix_mvvm.Models.Permissions;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController;
using _7dtd_svmanager_fix_mvvm.Models.Settings;
using _7dtd_svmanager_fix_mvvm.Models.Setup;
using _7dtd_svmanager_fix_mvvm.Models.Update;
using CommonStyleLib.Views;
using CommonExtensionLib.Extensions;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;
using SvManagerLibrary.Player;
using SvManagerLibrary.Telnet;
using _7dtd_svmanager_fix_mvvm.Models.WindowModel;
using _7dtd_svmanager_fix_mvvm.ViewModels.Backup;
using _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer;
using _7dtd_svmanager_fix_mvvm.ViewModels.Permissions;
using _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController;
using _7dtd_svmanager_fix_mvvm.ViewModels.Settings;
using _7dtd_svmanager_fix_mvvm.ViewModels.Setup;
using _7dtd_svmanager_fix_mvvm.ViewModels.Update;
using _7dtd_svmanager_fix_mvvm.Views.Backup;
using _7dtd_svmanager_fix_mvvm.Views.LogViewer;
using _7dtd_svmanager_fix_mvvm.Views.Permissions;
using _7dtd_svmanager_fix_mvvm.Views.PlayerController;
using _7dtd_svmanager_fix_mvvm.Views.PlayerController.Pages;
using _7dtd_svmanager_fix_mvvm.Views.Settings;
using _7dtd_svmanager_fix_mvvm.Views.Setup;
using _7dtd_svmanager_fix_mvvm.Views.Update;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using CommonNavigationControlLib.Navigation.ViewModels;
using CommonNavigationControlLib.Navigation.Views;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController.Pages;
using _7dtd_svmanager_fix_mvvm.ViewModels.AutoRestart;
using _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController.Pages;
using _7dtd_svmanager_fix_mvvm.Views.AutoRestart;
using SavannahManagerStyleLib.ViewModels.Encryption;
using SavannahManagerStyleLib.Views.Encryption;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class UserDetail
    {
        public string Id { get; set; }
        public string Level { get; set; }
        public string Name { get; set; }
        public string Health { get; set; }
        public string ZombieKills { get; set; }
        public string PlayerKills { get; set; }
        public string Death { get; set; }
        public string Score { get; set; }
        public string Coord { set; get; }
        public string SteamId { get; set; }

        public PlayerInfo ToPlayerInfo()
        {
            return new PlayerInfo
            {
                Id = Id,
                Level = Level,
                Name = Name,
                Health = Health,
                ZombieKills = ZombieKills,
                PlayerKills = PlayerKills,
                Deaths = Death,
                Score = Score,
                Coord = Coord,
                SteamId = SteamId
            };
        }

        public override string ToString()
        {
            return $"{Id} {Level} {Name} {Health} {ZombieKills} {PlayerKills} {Death} {Score} {Coord} {SteamId}\r\n";
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(MainWindowService windowService, MainWindowModel model) : base(windowService, model)
        {
            model.ConsoleTextAppended.Subscribe(Model_AppendConsoleText);
            model.ErrorOccurred.Subscribe((message) =>
            {
                if (message.IsAsync)
                {
                    windowService.MessageBoxDispatchShow(message.ErrorMessage,
                        LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                }
                else {
                    windowService.MessageBoxShow(message.ErrorMessage,
                        LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                }
            });
            model.MessageBoxOccurred.Subscribe(args =>
            {
                var dialogResult = windowService.MessageBoxShow(args.Message, args.Title, args.MessageType, args.ButtonType);
                args.CallBack(dialogResult);
            });
            model.TelnetStarted.Subscribe(Telnet_Started);
            model.TelnetFinished.Subscribe(Telnet_Finished);
            model.TelnetRead.Subscribe(TelnetReadEvent);

            _mainWindowService = windowService;
            _model = model;

            #region Event Initialize
            //Loaded = new DelegateCommand(MainWindow_Loaded);
            Closing = new DelegateCommand(MainWindow_Closing);
            KeyDown = new DelegateCommand<KeyEventArgs>(MainWindow_KeyDown);

            OpenSettingCommand = new DelegateCommand(OpenMenuSettings);
            OpenFirstSettingsCommand = new DelegateCommand(OpenMenuFirstSettings);
            SetLangJapaneseCommand = new DelegateCommand(SetMenuLangJapanese);
            SetLangEnglishCommand = new DelegateCommand(SetMenuLangEnglish);
            OpenConfigEditorCommand = new DelegateCommand(OpenConfigEditor);
            OpenXmlEditorCommand = new DelegateCommand(OpenMenuXmlEditor);
            OpenLogViewerCommand = new DelegateCommand(OpenLogViewer);
            OpenCheckUpdateCommand = new DelegateCommand(OpenMenuCheckUpdate);
            OpenVersionInfoCommand = new DelegateCommand(OpenMenuVersionInfo);

            StartServerCommand = new DelegateCommand(StartServer);
            StopServerCommand = new DelegateCommand(StopServer);
            ConnectTelnetCommand = new DelegateCommand(ConnectTelnet);
            AutoRestartCommand = new DelegateCommand(AutoRestart);
            OpenCommandListCommand = new DelegateCommand(OpenCommandList);

            PlayerListRefreshCommand = new DelegateCommand(PlayerListRefresh);

            PlayerContextMenuOpened = new DelegateCommand(PlayerContextMenu_Opened);
            AddAdminCommand = new DelegateCommand(AdminAddPlayer);
            RemoveAdminCommand = new DelegateCommand(AdminRemovePlayer);
            AddWhiteListCommand = new DelegateCommand(WhiteListAddPlayer);
            RemoveWhiteListCommand = new DelegateCommand(WhiteListRemovePlayer);
            KickPlayerCommand = new DelegateCommand(KickPlayer);
            BanPlayerCommand = new DelegateCommand(BanPlayer);
            KillPlayerCommand = new DelegateCommand(KillPlayer);
            ShowPlayerInfoCommand = new DelegateCommand(WatchPlayerInfo);

            ChatTextBoxEnterDown = new DelegateCommand<string>(ChatTextBoxEnter_Down);
            ChatLogMouseEnterCommand = new DelegateCommand(ChatLogMouseEnter);
            ChatLogMouseLeaveCommand = new DelegateCommand(ChatLogMouseLeave);

            ConsoleTextBoxMouseEnter = new DelegateCommand(ConsoleTextBoxMouse_Enter);
            ConsoleTextBoxMouseLeave = new DelegateCommand(ConsoleTextBoxMouse_Leave);
            DeleteLogCommand = new DelegateCommand(DeleteLog);

            CmdTextBoxEnterDown = new DelegateCommand<string>(SendCmd);
            SetCmdHistoryCommand = new DelegateCommand<KeyBinding>(SetCmdHistory);

            OpenTelnetWaitTimeCalculatorCommand = new DelegateCommand(OpenTelnetWaitTimeCalculator);
            GetTimeCommand = new DelegateCommand(GetTime);
            SetTimeCommand = new DelegateCommand(SetTime);
            SaveWorldCommand = new DelegateCommand(SaveWorld);
            OpenPermissionEditorCommand = new DelegateCommand(OpenPermissionEditor);

            OpenGetIpCommand = new DelegateCommand(OpenGetIp);
            CheckPortCommand = new DelegateCommand(OpenCheckPort);
            #endregion

            #region Property Initialize

            IsBeta = model.ObserveProperty(m => m.IsBeta).ToReactiveProperty().AddTo(CompositeDisposable);

            StartBtEnabled = model.ToReactivePropertyAsSynchronized(m => m.StartBtEnabled).AddTo(CompositeDisposable);
            TelnetBtIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.TelnetBtIsEnabled).AddTo(CompositeDisposable);
            TelnetBtLabel = model.ToReactivePropertyAsSynchronized(m => m.TelnetBtLabel).AddTo(CompositeDisposable);
            AutoRestartText = model.ObserveProperty(m => m.AutoRestartText).ToReactiveProperty().AddTo(CompositeDisposable);

            UsersList = model.ToReactivePropertyAsSynchronized(m => m.UsersList).AddTo(CompositeDisposable);

            IsConsoleLogTextWrapping = model.ToReactivePropertyAsSynchronized(m => m.IsConsoleLogTextWrapping).AddTo(CompositeDisposable);
            ChatLogText = new ReactiveProperty<string>();
            ChatInputText = model.ToReactivePropertyAsSynchronized(m => m.ChatInputText).AddTo(CompositeDisposable);
            
            ConnectionPanelIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.ConnectionPanelIsEnabled).AddTo(CompositeDisposable);
            LocalModeChecked = model.ToReactivePropertyAsSynchronized(m => m.LocalMode).AddTo(CompositeDisposable);
            LocalModeEnabled = model.ToReactivePropertyAsSynchronized(m => m.LocalModeEnabled).AddTo(CompositeDisposable);
            TelnetAddressText = model.ToReactivePropertyAsSynchronized(m => m.Address).AddTo(CompositeDisposable);
            TelnetPortText = model.ToReactivePropertyAsSynchronized(m => m.PortText).AddTo(CompositeDisposable);
            TelnetPasswordText = model.ToReactivePropertyAsSynchronized(m => m.Password).AddTo(CompositeDisposable);

            TimeDayText = model.ToReactivePropertyAsSynchronized(m => m.TimeDayText).AddTo(CompositeDisposable);
            TimeHourText = model.ToReactivePropertyAsSynchronized(m => m.TimeHourText).AddTo(CompositeDisposable);
            TimeMinuteText = model.ToReactivePropertyAsSynchronized(m => m.TimeMinuteText).AddTo(CompositeDisposable);

            BottomNewsLabel = model.ToReactivePropertyAsSynchronized(m => m.BottomNewsLabel).AddTo(CompositeDisposable);
            BottomDebugLabel = new ReactiveProperty<string>();

            SshAddressText = model.ToReactivePropertyAsSynchronized(m => m.SshAddressText).AddTo(CompositeDisposable);
            SshPortText = model.ToReactivePropertyAsSynchronized(m => m.SshPortText).AddTo(CompositeDisposable);
            SshUserNameText = model.ToReactivePropertyAsSynchronized(m => m.SshUserNameText).AddTo(CompositeDisposable);
            SshPasswordText = model.ToReactivePropertyAsSynchronized(m => m.SshPasswordText).AddTo(CompositeDisposable);
            SshExeFileDirectoryText = model.ToReactivePropertyAsSynchronized(m => m.SshExeFileDirectoryText).AddTo(CompositeDisposable);
            SshShellScriptFileName = model.ToReactivePropertyAsSynchronized(m => m.SshShellScriptFileName).AddTo(CompositeDisposable);
            SshArgumentText = model.ToReactivePropertyAsSynchronized(m => m.SshArgument).AddTo(CompositeDisposable);
            SshAuthMode = model.ToReactivePropertyAsSynchronized(m => m.SshAuthMode).AddTo(CompositeDisposable);
            SshKeyPathText = model.ToReactivePropertyAsSynchronized(m => m.SshKeyPathText).AddTo(CompositeDisposable);
            SshPassPhraseText = model.ToReactivePropertyAsSynchronized(m => m.SshPassPhraseText).AddTo(CompositeDisposable);

            #endregion

            model.InitializeWindow();
        }

        #region Fields
        private readonly MainWindowService _mainWindowService;
        private readonly MainWindowModel _model;
        private StringBuilder _consoleLog = new();
        private StringBuilder _chatLog = new();

        private bool _chatLogIsFocus;
        private bool _consoleIsFocus;

        private int _usersListSelectedIndex = -1;
        private bool _adminContextEnabled;
        private bool _whitelistContextEnabled;
        private bool _kickContextEnabled;
        private bool _banContextEnabled;
        private bool _watchPlayerInfoContextEnabled;
        private string _consoleLogText;
        private string _cmdText;
        #endregion

        #region EventProperties
        public ICommand OpenSettingCommand { get; set; }
        public ICommand OpenFirstSettingsCommand { get; set; }
        public ICommand SetLangJapaneseCommand { get; set; }
        public ICommand SetLangEnglishCommand { get; set; }
        public ICommand OpenConfigEditorCommand { get; set; }
        public ICommand OpenXmlEditorCommand { get; set; }
        public ICommand OpenLogViewerCommand { get; set; }
        public ICommand OpenCheckUpdateCommand { get; set; }
        public ICommand OpenVersionInfoCommand { get; set; }

        public ICommand StartServerCommand { get; set; }
        public ICommand StopServerCommand { get; set; }
        public ICommand ConnectTelnetCommand { get; set; }
        public ICommand AutoRestartCommand { get; set; }
        public ICommand OpenCommandListCommand { get; set; }

        public ICommand PlayerListRefreshCommand { get; set; }
        
        public ICommand PlayerContextMenuOpened { get; set; }
        public ICommand AddAdminCommand { get; set; }
        public ICommand RemoveAdminCommand { get; set; }
        public ICommand AddWhiteListCommand { get; set; }
        public ICommand RemoveWhiteListCommand { get; set; }
        public ICommand KickPlayerCommand { get; set; }
        public ICommand BanPlayerCommand { get; set; }
        public ICommand KillPlayerCommand { get; set; }
        public ICommand ShowPlayerInfoCommand { get; set; }

        public ICommand ChatTextBoxEnterDown { get; set; }
        public ICommand ChatLogMouseEnterCommand { get; set; }
        public ICommand ChatLogMouseLeaveCommand { get; set; }

        public ICommand SetCmdHistoryCommand { get; set; }

        public ICommand ConsoleTextBoxMouseEnter { get; set; }
        public ICommand ConsoleTextBoxMouseLeave { get; set; }
        public ICommand DeleteLogCommand { get; set; }

        public ICommand CmdTextBoxEnterDown { get; set; }

        public ICommand OpenTelnetWaitTimeCalculatorCommand { get; set; }
        public ICommand GetTimeCommand { get; set; }
        public ICommand SetTimeCommand { get; set; }
        public ICommand SaveWorldCommand { get; set; }
        public ICommand OpenPermissionEditorCommand { get; set; }

        public ICommand OpenGetIpCommand { get; set; }
        public ICommand CheckPortCommand { get; set; }
        #endregion

        #region Properties
        public ReactiveProperty<bool> IsBeta { get; set; }

        public TextBox ConsoleTextBox { get; set; }
        
        public ReactiveProperty<bool> StartBtEnabled { get; set; }
        public ReactiveProperty<bool> TelnetBtIsEnabled { get; set; }
        public ReactiveProperty<string> TelnetBtLabel { get; set; }
        public ReactiveProperty<string> AutoRestartText { get; set; }
        
        public ReactiveProperty<ObservableCollection<UserDetail>> UsersList { get; set; }
        public int UsersListSelectedIndex
        {
            get => _usersListSelectedIndex;
            set => SetProperty(ref _usersListSelectedIndex, value);
        }

        public bool AdminContextEnabled
        {
            get => _adminContextEnabled;
            set => SetProperty(ref _adminContextEnabled, value);
        }
        public bool WhitelistContextEnabled
        {
            get => _whitelistContextEnabled;
            set => SetProperty(ref _whitelistContextEnabled, value);
        }
        public bool KickContextEnabled
        {
            get => _kickContextEnabled;
            set => SetProperty(ref _kickContextEnabled, value);
        }
        public bool BanContextEnabled
        {
            get => _banContextEnabled;
            set => SetProperty(ref _banContextEnabled, value);
        }
        public bool WatchPlayerInfoContextEnabled
        {
            get => _watchPlayerInfoContextEnabled;
            set => SetProperty(ref _watchPlayerInfoContextEnabled, value);
        }
        
        public ReactiveProperty<string> ChatLogText { get; set; }
        public ReactiveProperty<string> ChatInputText { get; set; }

        public ReactiveProperty<bool> IsConsoleLogTextWrapping { get; set; }

        public string ConsoleLogText
        {
            get => _consoleLogText;
            set => SetProperty(ref _consoleLogText, value);
        }
        public string CmdText
        {
            get => _cmdText;
            set => SetProperty(ref _cmdText, value);
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
        public ReactiveProperty<string> BottomDebugLabel { get; set; }

        public ReactiveProperty<string> SshAddressText { get; set; }
        public ReactiveProperty<string> SshPortText { get; set; }
        public ReactiveProperty<string> SshUserNameText { get; set; }
        public ReactiveProperty<string> SshPasswordText { get; set; }
        public ReactiveProperty<string> SshExeFileDirectoryText { get; set; }
        public ReactiveProperty<string> SshShellScriptFileName { get; set; }
        public ReactiveProperty<string> SshArgumentText { get; set; }
        public ReactiveProperty<AuthMode> SshAuthMode { get; set; }
        public ReactiveProperty<string> SshKeyPathText { get; set; }
        public ReactiveProperty<string> SshPassPhraseText { get; set; }

        #endregion

        #region EventMethods
        protected override void MainWindow_Loaded()
        {
            _model.Initialize();
            
            if (_model.Setting.IsEncryptPassword)
            {
                string password;
                string salt;
                do
                {
                    const int inputWidth = InputWindowViewModel.DefaultWidth;
                    const int inputHeight = InputWindowViewModel.DefaultHeight;
                    var (left, top) = MainWindowModel.CalculateCenterTop(_model, inputWidth, inputHeight);
                    var inputViewModel = new InputWindowViewModel(new WindowService(), new ModelBase
                    {
                        Width = inputWidth,
                        Height = inputHeight,
                        Top = top,
                        Left = left
                    })
                    {
                        Title =
                        {
                            Value = "Password Dialog"
                        },
                        Message =
                        {
                            Value = "Input password for decryption."
                        }
                    };

                    WindowManageService.ShowDialog<InputWindow>(inputViewModel);
                    password = inputViewModel.IsCancel ? null : inputViewModel.InputText.Value;
                    salt = inputViewModel.IsCancel ? null : inputViewModel.InputSaltText.Value;
                } while (!_model.InitializeEncryptionData(password, salt));
            }
            else
            {
                _model.InitializeEncryptionData();
            }


            _model.RefreshLabels();

            if (_model.Setting.IsFirstBoot)
                OpenMenuFirstSettings();

            CheckUpdate().ContinueWith(t =>
            {
                if (t.Exception == null)
                    return;
                foreach (var exceptionInnerException in t.Exception.InnerExceptions)
                    App.ShowAndWriteException(exceptionInnerException);
            }, TaskContinuationOptions.OnlyOnFaulted);

#if DEBUG
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    BottomDebugLabel.Value = (GC.GetTotalMemory(false) / 1024 / 1024).ToString() + "MB";
                    await Task.Delay(1000);
                }
            });
#endif
        }

        protected override void MainWindow_Closing()
        {
            _model.SettingsSave();
            _model.Dispose();
        }
        protected override void MainWindow_KeyDown(KeyEventArgs e)
        {
            _model.PushShortcutKey(e.Key);
        }

        private void OpenMenuSettings()
        {
            var setting = _model.Setting;
            var keyManager = _model.ShortcutKeyManager;

            var settingModel = new SettingModel(setting, keyManager, _model);
            var vm = new SettingWindowViewModel(new WindowService(), settingModel);
            WindowManageService.ShowDialog<SettingWindow>(vm);
            _model.IsBeta = setting.IsBetaMode;
        }
        private void OpenMenuFirstSettings()
        {
            WindowManageService.ShowDialog<NavigationBase>(window =>
            {
                var model = new ModelBase();
                var service = new NavigationWindowService<InitializeData>
                {
                    Owner = window,
                    Navigation = window.NavigationControl,
                    Share = new InitializeData
                    {
                        Setting = _model.Setting,
                        ServerConfigFilePath = _model.Setting.ConfigFilePath,
                        ServerFilePath = _model.Setting.ExeFilePath,
                        ServerAdminConfigFilePath = _model.Setting.AdminFilePath
                    },
                    Pages = new List<NavigationPageInfo>
                    {
                        new(typeof(FirstPage)),
                        new(typeof(ExecutablePage)),
                        new(typeof(ConfigPage)),
                        new(typeof(AdminPage)),
                        new(typeof(FinishPage))
                    }
                };
                service.NavigationValue.WindowTitle = LangResources.SetupResource.UI_NameLabel;
                service.Initialize();
                var vm = new NavigationBaseViewModel<InitializeData>(service, model)
                {
                    CloseAction = (initData) =>
                    {
                        var di = ExMessageBoxBase.Show(LangResources.SetupResource.Dialog_ShowAgainText,
                            LangResources.SetupResource.Dialog_ShowAgainTitle,
                            ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
                        if (di == ExMessageBoxBase.DialogResult.No)
                            initData.Setting.IsFirstBoot = false;
                    }
                };

                return vm;
            });
        }
        private void SetMenuLangJapanese()
        {
            _model.ChangeCulture(ResourceService.Japanese);
            _model.RefreshLabels();
        }
        private void SetMenuLangEnglish()
        {
            _model.ChangeCulture(ResourceService.English);
            _model.RefreshLabels();
        }
        private void OpenConfigEditor()
        {
            _model.RunConfigEditor();
        }

        private void OpenMenuXmlEditor()
        {
            _model.RunXmlEditor();
        }

        private void OpenLogViewer()
        {
            var model = new LogViewerModel();
            var vm = new LogViewerViewModel(new WindowService(), model);
            WindowManageService.Show<Views.LogViewer.LogViewer>(vm);
        }
        private void OpenMenuCheckUpdate()
        {
            var updFormModel = new UpdFormModel();
            var vm = new UpdFormViewModel(new WindowService(), updFormModel);
            WindowManageService.ShowNonOwner<UpdForm>(vm);
        }
        private void OpenMenuVersionInfo()
        {
            var versionInfoModel = new VersionInfoModel();
            var vm = new VersionInfoViewModel(new WindowService(), versionInfoModel);
            WindowManageService.ShowDialog<VersionInfo>(vm);
        }

        private void StartServer()
        {
            _ = _model.ServerStartForButton();
        }

        private void StopServer()
        {
            var isForceShutdown = _model.ServerStop();
            if (!isForceShutdown)
                return;

            var forceShutdownerModel = new ForceShutdownerModel();
            var vm = new ForceShutdownerViewModel(new WindowService(), forceShutdownerModel);
            WindowManageService.ShowNonOwnerOnly<ForceShutdowner>(vm);
        }

        private void ConnectTelnet()
        {
            _model.SwitchTelnetConnection();
        }

        public void AutoRestart()
        {
            if (!_model.AutoRestartEnabled)
            {
                if (_model.Setting.RebootingWaitMode == 0)
                {
                    _model.StartAutoRestartCoolTime();
                }
                else
                {
                    _model.StartAutoRestartProcessWaiter(() =>
                    {
                        using var model = new ProcessSelectorModel();
                        var vm = new ProcessSelectorViewModel(new WindowService(), model);
                        WindowManageService.Dispatch(() =>
                        {
                            WindowManageService.ShowDialog<ProcessSelector>(vm);
                        });

                        return model.ProcessId;
                    });
                }
            }
            else
            {
                _model.StopRequestAutoRestart();
            }
        }

        private void OpenCommandList()
        {

        }

        private void PlayerListRefresh()
        {
            _model.PlayerRefresh();
        }

        private void PlayerContextMenu_Opened()
        {
            var index = UsersListSelectedIndex;
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
        private void AdminAddPlayer()
        {
            var playerInfo = _model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.Id) ? string.Empty : playerInfo.Id;

            var playerBaseModel = new PlayerBaseModel();
            var adminModel = new AdminAddModel(_model, new AddType(AddType.Type.Admin))
            {
                Name = name
            };
            var adminViewModel = new AdminAddViewModel(adminModel);
            var adminAdd = new AdminAdd(adminViewModel, adminModel);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = adminAdd;
                window.AssignEnded();
                window.Navigate();
                var vm = new PlayerBaseViewModel(new PlayerBaseWindowService(adminViewModel), playerBaseModel)
                {
                    WindowTitle = "Add"
                };
                return vm;
            });
        }
        private void AdminRemovePlayer()
        {
            _model.RemoveAdmin(UsersListSelectedIndex);
        }
        private void WhiteListAddPlayer()
        {
            var playerInfo = _model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.Id) ? string.Empty : playerInfo.Id;

            var playerBaseModel = new PlayerBaseModel();
            var whitelistModel = new AdminAddModel(_model, new AddType(AddType.Type.Whitelist))
            {
                Name = name
            };
            var whitelistViewModel = new AdminAddViewModel(whitelistModel);
            var whitelistAdd = new AdminAdd(whitelistViewModel, whitelistModel);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = whitelistAdd;
                window.AssignEnded();
                window.Navigate();
                return new PlayerBaseViewModel(new PlayerBaseWindowService(whitelistViewModel), playerBaseModel)
                {
                    WindowTitle = "Whitelist"
                };
            });
        }
        private void WhiteListRemovePlayer()
        {
            _model.RemoveWhitelist(UsersListSelectedIndex);
        }
        private void KickPlayer()
        {
            var playerInfo = _model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.Id) ? string.Empty : playerInfo.Id;

            var playerBaseModel = new PlayerBaseModel();
            var kickModel = new KickModel(_model)
            {
                Name = name
            };
            var kickViewModel = new KickViewModel(kickModel);
            var kick = new Kick(kickViewModel, kickModel);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = kick;
                window.AssignEnded();
                window.Navigate();
                return new PlayerBaseViewModel(new PlayerBaseWindowService(kickViewModel), playerBaseModel)
                {
                    WindowTitle = "Kick"
                };
            });
        }
        private void BanPlayer()
        {
            var playerInfo = _model.GetUserDetail(UsersListSelectedIndex);
            var name = string.IsNullOrEmpty(playerInfo.Id) ? string.Empty : playerInfo.Id;

            var playerBaseModel = new PlayerBaseModel();
            var banModel = new BanModel(_model)
            {
                Name = name
            };
            var banViewModel = new BanViewModel(banModel);
            var ban = new Ban(banViewModel, banModel);
            WindowManageService.ShowDialog<PlayerBase>(window =>
            {
                window.Page = ban;
                window.AssignEnded();
                window.Navigate();
                return new PlayerBaseViewModel(new PlayerBaseWindowService(banViewModel), playerBaseModel)
                {
                    WindowTitle = "Ban"
                };
            });
        }
        private void KillPlayer()
        {

        }
        private void WatchPlayerInfo()
        {
            var playerInfo = _model.GetSelectedPlayerInfo(UsersListSelectedIndex);
            var playerInfoModel = new PlayerInfoModel();
            var vm = new PlayerInfoViewModel(new WindowService(), playerInfoModel);
            vm.SetPlayer(playerInfo);
            WindowManageService.ShowDialog<PlayerInfoView>(vm);
        }
        
        private void ChatTextBoxEnter_Down(string e)
        {
            _model.SendChat(e, () => _model.ChatInputText = "");
        }

        private void ChatLogMouseEnter()
        {
            _chatLogIsFocus = true;
        }
        private void ChatLogMouseLeave()
        {
            _chatLogIsFocus = false;
        }

        private void ConsoleTextBoxMouse_Enter()
        {
            _consoleIsFocus = true;
        }
        private void ConsoleTextBoxMouse_Leave()
        {
            _consoleIsFocus = false;
        }
        private void DeleteLog()
        {
            _consoleLog.Clear();
            ConsoleLogText = "";
        }

        private void SendCmd(string e)
        {
            _model.SendCommand(e);
            CmdText = string.Empty;
        }

        private void SetCmdHistory(KeyBinding e)
        {
            var key = e.Key;
            var cmd = "";
            switch (key)
            {
                case Key.Up:
                    cmd = _model.GetPreviousCommand();
                    break;
                case Key.Down:
                    cmd = _model.GetNextCommand();
                    break;
            }

            CmdText = cmd;
            _mainWindowService.Select(_mainWindowService.CmdTextBox, CmdText.Length, 0);
        }

        private void OpenTelnetWaitTimeCalculator()
        {
            if (!_model.CheckConnected())
                return;

            var model = new TelnetWaitTimeCalculatorModel(_model.Telnet, _model.Setting);
            var vm = new TelnetWaitTimeCalculatorViewModel(new WindowService(), model);
            WindowManageService.ShowDialog<TelnetWaitTimeCalculator>(vm);
        }
        private void GetTime()
        {
            _model.SetTimeToTextBox();
        }
        private void SetTime()
        {
            _model.SetTimeToGame();
        }
        private void SaveWorld()
        {
            _model.SendCommand("saveworld", false);
        }

        private void OpenPermissionEditor()
        {
            var adminFilePath = _model.Setting.AdminFilePath;
            if (string.IsNullOrEmpty(adminFilePath))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "AdminFilePath"),
                    LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }
            var vm = new PermissionEditorViewModel(new WindowService(), new PermissionEditorModel(adminFilePath));
            WindowManageService.ShowNonOwner<PermissionEditor>(vm);
        }

        private void OpenGetIp()
        {
            var ipAddressGetterModel = new IpAddressGetterModel();
            var vm = new IpAddressGetterViewModel(new WindowService(), ipAddressGetterModel);
            WindowManageService.Show<IpAddressGetter>(vm);
        }
        private void OpenCheckPort()
        {
            var portCheckModel = new PortCheckModel();
            var vm = new PortCheckViewModel(new WindowService(), portCheckModel);
            WindowManageService.Show<PortCheck>(vm);
        }


        private void Model_AppendConsoleText(MainWindowModel.AppendedLogTextEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.AppendedLogText))
            {
                WindowManageService.Dispatch(DispatcherPriority.Background, () =>
                {
                    AppendConsoleText(e.AppendedLogText, e.MaxLength);
                });
            }
        }
        #endregion

        #region Methods
        private async Task CheckUpdate()
        {
            var availableUpdate = await _model.CheckUpdate();
            if (availableUpdate)
            {
                var dialogResult = _mainWindowService.MessageBoxShow(LangResources.Resources.UI_DoUpdateAlertMessage,
                    LangResources.Resources.UI_DoUpdateAlertTitle, ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
                if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
                {
                    var updFormModel = new UpdFormModel();
                    await updFormModel.Initialize();

                    WindowManageService.Dispatch(() =>
                    {
                        var vm = new UpdFormViewModel(new WindowService(), updFormModel, true);
                        WindowManageService.Show<UpdForm>(vm);
                    });
                }
            }
        }

        private void AppendConsoleText(string text, int maxLength)
        {
            if (_consoleLog == null)
            {
                _consoleLog = new StringBuilder(maxLength * 2);
            }
            _consoleLog.Append(text);
            if (_consoleLog.Length > maxLength)
            {
                _consoleLog.Remove(0, _consoleLog.Length - maxLength);
            }

            ConsoleLogText = _consoleLog.ToString();

            if (!_consoleIsFocus)
            {
                _mainWindowService.Select(_mainWindowService.ConsoleTextBox, ConsoleLogText.Length, 0);
                _mainWindowService.ScrollToEnd(_mainWindowService.ConsoleTextBox);
            }
        }

        private void AppendChatText(string text, int maxLength)
        {
            if (_chatLog == null)
            {
                _chatLog = new StringBuilder(maxLength * 2);
            }
            _chatLog.Append(text);
            if (_chatLog.Length > maxLength)
            {
                _chatLog.Remove(0, _chatLog.Length - maxLength);
            }

            ChatLogText.Value = _chatLog.ToString();

            if (!_chatLogIsFocus)
            {
                WindowManageService.Dispatch(DispatcherPriority.Background, () =>
                {
                    _mainWindowService.Select(_mainWindowService.ChatLogText, ConsoleLogText.Length, 0);
                    _mainWindowService.ScrollToEnd(_mainWindowService.ChatLogText);
                });
            }
        }
        #endregion


        private void Telnet_Started(TelnetClient.TelnetReadEventArgs e)
        {
            _model.PlayerClean();
        }

        private void Telnet_Finished(TelnetClient.TelnetReadEventArgs e)
        {
            _model.PlayerClean();
            _model.TelnetFinish();
            _model.StopAutoRestart(false);
        }

        private void TelnetReadEvent(TelnetClient.TelnetReadEventArgs e)
        {
            var log = "{0}".FormatString(e.Log);

            _model.AppendConsoleLog(log);

            if (log.IndexOf("Chat", StringComparison.Ordinal) > -1)
            {
                var chat = _model.GetChatText(log);
                AppendChatText(chat, 16384);
            }
            if (log.IndexOf("INF Created player with id=", StringComparison.Ordinal) > -1)
            {
                WindowManageService.Dispatch(DispatcherPriority.Background, _model.PlayerRefresh);
            }
            if (log.IndexOf("INF Player disconnected", StringComparison.Ordinal) > -1)
            {
                _model.RemoveUser(log);
            }
        }
    }
}
