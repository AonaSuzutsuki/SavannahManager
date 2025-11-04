using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.LangResources;
using _7dtd_svmanager_fix_mvvm.ViewModels;
using _7dtd_svmanager_fix_mvvm.Models.Config;
using CommonStyleLib.Models;
using CommonStyleLib.ExMessageBox;
using SvManagerLibrary.Time;
using SvManagerLibrary.Telnet;
using SvManagerLibrary.Chat;
using SvManagerLibrary.Player;
using CommonExtensionLib.Extensions;
using System.Linq;
using System.Text;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using _7dtd_svmanager_fix_mvvm.Models.Ssh;
using _7dtd_svmanager_fix_mvvm.Models.Update;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using CommonStyleLib.Models.Errors;
using Renci.SshNet.Common;
using _7dtd_svmanager_fix_mvvm.Models.AutoRestart;
using _7dtd_svmanager_fix_mvvm.Models.Scheduled;
using SvManagerLibrary.AnalyzerPlan.Console;

namespace _7dtd_svmanager_fix_mvvm.Models.WindowModel
{
    public class MainWindowModel : ModelBase, IMainWindowTelnet, IMainWindowServerStart, IRelease, ISettingMainWindowModel
    {
        #region AppendedLogTextEvent
        public class AppendedLogTextEventArgs
        {
            public int MaxLength { get; set; }
            public string AppendedLogText { get; set; }
        }
        private readonly Subject<AppendedLogTextEventArgs> _consoleTextAppended = new Subject<AppendedLogTextEventArgs>();
        public IObservable<AppendedLogTextEventArgs> ConsoleTextAppended => _consoleTextAppended;
        #endregion

        #region ErrorEvent

        public class MessageBoxOccurredEventArgs
        {
            public string Message { get; set; }
            public string Title { get; set; }
            public ExMessageBoxBase.MessageType MessageType { get; set; } = ExMessageBoxBase.MessageType.Exclamation;
            public ExMessageBoxBase.ButtonType ButtonType { get; set; } = ExMessageBoxBase.ButtonType.OK;

            public Action<ExMessageBoxBase.DialogResult> CallBack { get; set; }
        }
        private readonly Subject<MessageBoxOccurredEventArgs> _messageBoxOccurred = new Subject<MessageBoxOccurredEventArgs>();
        public IObservable<MessageBoxOccurredEventArgs> MessageBoxOccurred => _messageBoxOccurred;
        
        #endregion

        #region PropertiesForViewModel
        private bool _startBtEnabled = true;
        public bool StartBtEnabled
        {
            get => _startBtEnabled;
            set => SetProperty(ref _startBtEnabled, value);
        }
        private bool _telnetBtIsEnabled = true;
        public bool TelnetBtIsEnabled
        {
            get => _telnetBtIsEnabled;
            set => SetProperty(ref _telnetBtIsEnabled, value);
        }
        private string _telnetBtLabel;
        public string TelnetBtLabel
        {
            get => _telnetBtLabel;
            set => SetProperty(ref _telnetBtLabel, value);
        }

        private ObservableCollection<UserDetail> _usersList;
        public ObservableCollection<UserDetail> UsersList
        {
            get => _usersList;
            set => SetProperty(ref _usersList, value);
        }

        private string _chatInputText;
        public string ChatInputText
        {
            get => _chatInputText;
            set => SetProperty(ref _chatInputText, value);
        }

        private bool _connectionPanelIsEnabled = true;
        public bool ConnectionPanelIsEnabled
        {
            get => _connectionPanelIsEnabled;
            set => SetProperty(ref _connectionPanelIsEnabled, value);
        }

        private string _autoRestartText = "AutoRestart Disabled";
        public string AutoRestartText
        {
            get => _autoRestartText;
            set => SetProperty(ref _autoRestartText, value);
        }

        private string _commandRunnerButtonText = ConstantValues.DisabledCommandRunnerContent;
        public string CommandRunnerButtonText
        {
            get => _commandRunnerButtonText;
            set => SetProperty(ref _commandRunnerButtonText, value);
        }

        private bool _isBeta;

        public bool IsBeta
        {
            get => _isBeta;
            set => SetProperty(ref _isBeta, value);
        }

        private bool _localMode = true;
        public bool LocalMode
        {
            get => _localMode;
            set
            {
                SetProperty(ref _localMode, value);
                ConnectionPanelIsEnabled = !value;
            }
        }
        private bool _localModeEnabled = true;
        public bool LocalModeEnabled
        {
            get => _localModeEnabled;
            set => SetProperty(ref _localModeEnabled, value);
        }
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }
        private string _portText = string.Empty;
        public string PortText
        {
            get => _portText;
            set
            {
                SetProperty(ref _portText, value);
                if (int.TryParse(value, out var port))
                {
                    _port = port;
                }
            }
        }
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _timeDayText = "0";
        public string TimeDayText
        {
            get => _timeDayText;
            set => SetProperty(ref _timeDayText, value);
        }
        private string _timeHourText = "0";
        public string TimeHourText
        {
            get => _timeHourText;
            set => SetProperty(ref _timeHourText, value);
        }
        private string _timeMinuteText = "0";
        public string TimeMinuteText
        {
            get => _timeMinuteText;
            set => SetProperty(ref _timeMinuteText, value);
        }

        private string _bottomNewsLabel;
        public string BottomNewsLabel
        {
            get => _bottomNewsLabel;
            set => SetProperty(ref _bottomNewsLabel, value);
        }

        private bool _isConsoleLogTextWrapping;
        public bool IsConsoleLogTextWrapping
        {
            get => _isConsoleLogTextWrapping;
            set => SetProperty(ref _isConsoleLogTextWrapping, value);
        }


        private string _sshAddressText = string.Empty;
        private string _sshPortText = string.Empty;
        private string _sshUserNameText = string.Empty;
        private string _sshPasswordText = string.Empty;
        private string _sshExeFileDirectoryText;
        private string _sshShellScriptFileName;
        private string _sshArgument;
        private AuthMode _sshAuthMode;
        private string _sshKeyPathText;
        private string _sshPassPhraseText;

        public string SshAddressText
        {
            get => _sshAddressText;
            set => SetProperty(ref _sshAddressText, value);
        }
        public string SshPortText
        {
            get => _sshPortText;
            set => SetProperty(ref _sshPortText, value);
        }
        public string SshUserNameText
        {
            get => _sshUserNameText;
            set => SetProperty(ref _sshUserNameText, value);
        }
        public string SshPasswordText
        {
            get => _sshPasswordText;
            set => SetProperty(ref _sshPasswordText, value);
        }
        public string SshExeFileDirectoryText
        {
            get => _sshExeFileDirectoryText;
            set => SetProperty(ref _sshExeFileDirectoryText, value);
        }

        public string SshShellScriptFileName
        {
            get => _sshShellScriptFileName;
            set => SetProperty(ref _sshShellScriptFileName, value);
        }
        public string SshArgument
        {
            get => _sshArgument;
            set => SetProperty(ref _sshArgument, value);
        }

        public AuthMode SshAuthMode
        {
            get => _sshAuthMode;
            set => SetProperty(ref _sshAuthMode, value);
        }
        public string SshKeyPathText
        {
            get => _sshKeyPathText;
            set => SetProperty(ref _sshKeyPathText, value);
        }
        public string SshPassPhraseText
        {
            get => _sshPassPhraseText;
            set => SetProperty(ref _sshPassPhraseText, value);
        }

        private bool _isExecuteScheduledCommand;

        public bool IsExecuteScheduledCommand
        {
            get => _isExecuteScheduledCommand;
            set => SetProperty(ref _isExecuteScheduledCommand, value);
        }

        private ObservableCollection<ScheduledCommandExecutor> _commands = new();
        public ObservableCollection<ScheduledCommandExecutor> Commands
        {
            get => _commands;
            set => SetProperty(ref _commands, value);
        }

        #endregion

        #region Properties

        //public IMessageBoxWindowService MessageBoxWindowService { get; set; }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected && RowConnected;
            set => _isConnected = value;
        }
        private bool RowConnected => LockFunction(telnet => telnet != null && telnet.Connected);

        public bool AutoRestartEnabled { get; set; }

        public bool IsFailed { get; private set; }
        public bool IsTelnetLoading { get; protected set; }
        public SettingLoader Setting { get; }
        public ShortcutKeyManager ShortcutKeyManager { get; }

        private string ExeFilePath => Setting.ExeFilePath;
        private string ConfigFilePath => Setting.ConfigFilePath;

        public int ConsoleTextLength { get; private set; }

        public TelnetClient Telnet { get; private set; }

        public int CurrentProcessId { get; private set; } = -1;

        public ScheduledCommandRunner CommandRunner => _scheduledCommandRunner;

        #endregion

        #region Fiels
        private string _address = string.Empty;
        private int _port;
        private string _password = string.Empty;

        private readonly Stack<ChatInfo> _chatArray = new();
        private readonly CommandCollector _commandCollector = new();
        private readonly Dictionary<int, UserDetail> _playersDictionary = new();
        private readonly List<int> _connectedIds = new();

        private bool _isServerForceStop;

        private bool _isLogGetter;

        private AbstractAutoRestart _autoRestart;
        private readonly ScheduledCommandRunner _scheduledCommandRunner;

        private readonly IConsoleAnalyzer _analyzerPlan = new OnePointTreeConsoleAnalyzer();
        #endregion

        #region Event

        private Subject<TelnetClient.TelnetReadEventArgs> _telnetStartedSubject = new Subject<TelnetClient.TelnetReadEventArgs>();
        public IObservable<TelnetClient.TelnetReadEventArgs> TelnetStarted => _telnetStartedSubject;

        private Subject<TelnetClient.TelnetReadEventArgs> _telnetFinishedSubject = new Subject<TelnetClient.TelnetReadEventArgs>();
        public IObservable<TelnetClient.TelnetReadEventArgs> TelnetFinished => _telnetFinishedSubject;

        private Subject<TelnetClient.TelnetReadEventArgs> _telnetReadSubject = new Subject<TelnetClient.TelnetReadEventArgs>();
        public IObservable<TelnetClient.TelnetReadEventArgs> TelnetRead => _telnetReadSubject;

        #endregion


        public MainWindowModel()
        {
            Setting = new SettingLoader(ConstantValues.SettingFilePath);
            ShortcutKeyManager = new ShortcutKeyManager(ConstantValues.AppDirectoryPath + @"\KeyConfig.xml",
                ConstantValues.AppDirectoryPath + @"\Settings\KeyConfig\" + Resources.KeyConfigPath);
            _scheduledCommandRunner = new ScheduledCommandRunner(this);
        }

        public override void Activated()
        {
            base.Activated();

            AroundBorderColor = !IsTelnetLoading ? CommonStyleLib.ConstantValues.ActivatedBorderColor
                : CommonStyleLib.ConstantValues.ActivatedBorderColor2;
        }

        public void InitializeWindow()
        {
            Width = Setting.Width;
            Height = Setting.Height;
            var screenWidth = (int)SystemParameters.WorkArea.Width;
            var screenHeight = (int)SystemParameters.WorkArea.Height;
            Top = (screenHeight - Height) / 2;
            Left = (screenWidth - Width) / 2;
        }

        public static (double left, double top) CalculateCenterTop(ModelBase model, double targetWidth, double targetHeight)
        {
            var returnLeft = model.Left + model.Width / 2 - targetWidth / 2;
            var returnTop = model.Top + model.Height / 2 - targetHeight / 2;

            return (returnLeft, returnTop);
        }

        public void Initialize()
        {
            Address = Setting.Address;
            PortText = Setting.Port.ToString();

            IsConsoleLogTextWrapping = Setting.IsConsoleLogTextWrapping;
            ConsoleTextLength = Setting.ConsoleTextLength;

            _isLogGetter = Setting.IsLogGetter;
            LocalMode = Setting.LocalMode;

            IsBeta = Setting.IsBetaMode;

            SshAddressText = Setting.SshAddress;
            SshPortText = Setting.SshPort.ToString();
            SshUserNameText = Setting.SshUserName;
            SshExeFileDirectoryText = Setting.SshExeFileDirectory;
            SshShellScriptFileName = Setting.SshShellScriptFileName;
            SshArgument = Setting.SshArgument;
            SshAuthMode = Setting.SshAuthMode.FromInt();
            SshKeyPathText = Setting.SshKeyPath;

            IsExecuteScheduledCommand = Setting.IsExecuteScheduledCommand;

            Setting.ApplyCulture();

            _ = LoadScheduledCommandsAsync();
        }

        public bool InitializeEncryptionData(string password = null, string salt = null)
        {
            if (password != null)
            {
                salt = string.IsNullOrEmpty(salt) ? null : salt;
                Setting.SetEncryptionPassword(password, salt);
                try
                {
                    Setting.LoadEncryptionData();
                }
                catch
                {
                    ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = "Invalid password." });
                    return false;
                }
            }

            Password = Setting.Password;
            SshPasswordText = Setting.SshPassword;

            return true;
        }

        public async Task<bool> CheckUpdate()
        {
            if (Setting.IsAutoUpdate)
            {
                var availableUpdate = await UpdateManager.CheckCanUpdate(UpdateManager.GetUpdateClient());
                return availableUpdate;
            }
            return false;
        }
        public void SettingsSave()
        {
            Setting.Width = (int)Width;
            Setting.Height = (int)Height;
            Setting.Address = Address;
            Setting.LocalMode = LocalMode;
            Setting.Port = _port;
            Setting.Password = Password;
            Setting.IsConsoleLogTextWrapping = IsConsoleLogTextWrapping;

            Setting.SshAddress = SshAddressText;
            Setting.SshPort = SshPortText.ToInt();
            Setting.SshUserName = SshUserNameText;
            Setting.SshPassword = SshPasswordText;
            Setting.SshExeFileDirectory = SshExeFileDirectoryText;
            Setting.SshShellScriptFileName = SshShellScriptFileName;
            Setting.SshArgument = SshArgument;
            Setting.SshAuthMode = SshAuthMode.ToInt();
            Setting.SshKeyPath = SshKeyPathText;

            Setting.IsExecuteScheduledCommand = IsExecuteScheduledCommand;
        }
        public void ChangeCulture(string cultureName)
        {
            ResourceService.Current.ChangeCulture(cultureName);
            Setting.CultureName = ResourceService.Current.Culture;
        }

        public void RefreshLabels()
        {
            TelnetBtLabel = IsConnected ? Resources.UI_DisconnectFromTelnet : Resources.UI_ConnectWithTelnet;

            if (IsTelnetLoading)
            {
                BottomNewsLabel = Resources.UI_WaitingServer;
            }
            else if (IsFailed)
            {
                BottomNewsLabel = Resources.Failed_Connecting;
            }
            else
            {
                BottomNewsLabel = Resources.UI_ReadyComplete;
            }
        }
        private void FeedColorChange(SolidColorBrush color)
        {
            for (double i = 1; i >= 0; i -= 0.1)
            {
                AroundBorderOpacity = i;
                Thread.Sleep(5);
            }
            AroundBorderColor = color;
            for (double i = 0; i <= 1; i += 0.1)
            {
                AroundBorderOpacity = i;
                Thread.Sleep(5);
            }
        }



        public async Task<bool> ServerStartForButton()
        {
            if (!LocalMode)
            {
                return await ServerStartWithSsh();
            }

            return await ServerStart();
        }

        public async Task<bool> ServerStart(bool isAsync = false)
        {
            if (!FileExistCheck(isAsync)) return false;

            var checkedValues = ConfigChecker.GetConfigInfo(ConfigFilePath);
            if (checkedValues.IsFailed)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = checkedValues.Message, IsAsync = isAsync });
                return false;
            }

            const string localAddress = "127.0.0.1";
            var localPassword = checkedValues.Password;
            var localPort = checkedValues.Port;

            if (IsConnected)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = Resources.AlreadyConnected, IsAsync = isAsync });
                return false;
            }

            using var serverProcessManager = new ServerProcessManager(ExeFilePath, ConfigFilePath);
            void ProcessFailedAction(string message) => ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = message, IsAsync = isAsync });
            if (!serverProcessManager.ProcessStart(ProcessFailedAction))
                return false;

            CurrentProcessId = serverProcessManager.ProcessId;

            StartBtEnabled = false;
            TelnetBtIsEnabled = false;
            LocalModeEnabled = false;

            BottomNewsLabel = Resources.UI_WaitingServer;
            SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor2);

            await ConnectTelnetForServerStart(localAddress, localPort, localPassword);
            return true;
        }

        public async Task<bool> ServerStartWithSsh(bool isAsync = false)
        {
            if (IsConnected)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = Resources.AlreadyConnected, IsAsync = isAsync });
                return false;
            }

            StartBtEnabled = false;
            TelnetBtIsEnabled = false;
            LocalModeEnabled = false;

            BottomNewsLabel = Resources.UI_WaitingServer;
            SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor2);

            try
            {
                using var sshManager = new SshServerManager(SshAddressText);
                if (SshAuthMode == AuthMode.Password)
                    sshManager.Connect(SshUserNameText, SshPasswordText);
                else
                    sshManager.Connect(SshUserNameText, SshPassPhraseText, SshKeyPathText);
                sshManager.StartServer($"cd {SshExeFileDirectoryText} " +
                                       $"&& {SshShellScriptFileName}" +
                                       (string.IsNullOrEmpty(SshArgument) ? "" : $" {SshArgument}"));
                await Task.Delay(500);
            }
            catch (SshAuthenticationException)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = "failed to authenticate on ssh.", IsAsync = isAsync });

                StartBtEnabled = true;
                TelnetBtIsEnabled = true;
                LocalModeEnabled = true;
                BottomNewsLabel = Resources.UI_ReadyComplete;
                return false;
            }
            catch (SshOperationTimeoutException)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = "failed to connect ssh.", IsAsync = isAsync });

                StartBtEnabled = true;
                TelnetBtIsEnabled = true;
                LocalModeEnabled = true;
                BottomNewsLabel = Resources.UI_ReadyComplete;
                return false;
            }

            await ConnectTelnetForServerStart(Address, _port, Password);
            return true;
        }

        private async Task ConnectTelnetForServerStart(string address, int port, string password)
        {
            Telnet = GenerateTelnetClient(this);
            await Task.Factory.StartNew(async () =>
            {
                IsTelnetLoading = true;
                while (true)
                {
                    if (_isServerForceStop)
                    {
                        TelnetBtIsEnabled = true;
                        BottomNewsLabel = Resources.UI_ReadyComplete;
                        AroundBorderColor = CommonStyleLib.ConstantValues.ActivatedBorderColor;

                        IsTelnetLoading = false;
                        _isServerForceStop = false;

                        TelnetFinish();
                        break;
                    }

                    if (LockFunction(telnet => telnet.Connect(address, port)))
                    {
                        IsConnected = true;
                        IsTelnetLoading = false;

                        TelnetBtIsEnabled = true;
                        TelnetBtLabel = Resources.UI_DisconnectFromTelnet;
                        LocalModeEnabled = false;
                        ConnectionPanelIsEnabled = false;
                        BottomNewsLabel = Resources.UI_FinishedLaunching;
                        SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor);

                        LockAction(telnet => telnet.Write(TelnetClient.Cr));
                        AppendConsoleLog(SocTelnetSend(password, true));

                        if (IsExecuteScheduledCommand)
                        {
                            await StartCommandRunner();
                        }

                        break;
                    }

                    Thread.Sleep(2000);
                }
            });
        }

        public async Task StartCommandRunner()
        {
            if (!CheckConnected())
                return;

            await LoadScheduledCommandsAsync();
            _scheduledCommandRunner.Start();

            if (_scheduledCommandRunner.ScheduledCommands.Any())
                CommandRunnerButtonText = ConstantValues.EnabledCommandRunnerContent;
        }

        public void StopCommandRunner()
        {
            _scheduledCommandRunner.Stop();

            CommandRunnerButtonText = ConstantValues.DisabledCommandRunnerContent;
        }

        public async Task LoadScheduledCommandsAsync()
        {
            if (!_scheduledCommandRunner.IsStop)
                return;

            await _scheduledCommandRunner.LoadAsync();
            Commands.Clear();
            Commands.AddRange(_scheduledCommandRunner.ScheduledCommands);
        }

        private void LockAction(Action<TelnetClient> action)
        {
            if (Telnet == null) return;
            lock (Telnet)
            {
                action(Telnet);
            }
        }

        private T LockFunction<T>(Func<TelnetClient, T> func)
        {
            if (Telnet == null) return default;
            lock (Telnet)
            {
                return func(Telnet);
            }
        }

        public bool ServerStop()
        {
            if (IsTelnetLoading)
            {
                _isServerForceStop = true;
                return false;
            }

            if (IsConnected)
            {
                SocTelnetSend("shutdown");

                TelnetBtLabel = Resources.UI_ConnectWithTelnet;
                StartBtEnabled = true;
            }
            else
            {
                return true;
            }

            return false;
        }

        public bool StartAutoRestartCoolTime()
        {
            if (!IsConnected)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs
                {
                    ErrorMessage = "Cannot enable auto restart mode because telnet are not connected."
                });
                return false;
            }

            if (_autoRestart != null)
            {
                StopAutoRestart(true);
            }

            _autoRestart = new AutoRestartCoolTime(new MainWindowServerStart(this, !LocalMode));

            InnerStartAutoRestart();

            return true;
        }

        public bool StartAutoRestartProcessWaiter(Func<int> callback)
        {
            if (!IsConnected)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs
                {
                    ErrorMessage = "Cannot enable auto restart mode because telnet are not connected."
                });
                return false;
            }

            if (!LocalMode)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs
                {
                    ErrorMessage = "Cannot enable auto restart mode because local server mode disabled."
                });
                return false;
            }

            if (_autoRestart != null)
            {
                StopAutoRestart(true);
            }

            var autoRestart = new AutoRestartProcessWaiter(new MainWindowServerStart(this, !LocalMode), callback);
            autoRestart.Initialize();
            if (!autoRestart.IsAttach)
            {
                return false;
            }

            _autoRestart = autoRestart;

            InnerStartAutoRestart();

            return true;
        }
        
        private void InnerStartAutoRestart()
        {
            var newsLabel = BottomNewsLabel;
            _autoRestart.TimeProgress.Subscribe((args) =>
            {
                var ts = args.WaitingTime;
                if (args.EventType == AutoRestartWaitingTimeEventArgs.WaitingType.RestartWait)
                {
                    BottomNewsLabel = $"{newsLabel}, AutoRestart: {ts:d\\.hh\\:mm\\:ss} remaining.";
                }
                else if (args.EventType == AutoRestartWaitingTimeEventArgs.WaitingType.ProcessWait)
                {
                    BottomNewsLabel = $"{newsLabel}, AutoRestart: Waiting to stop server.";
                }
                else if (args.EventType == AutoRestartWaitingTimeEventArgs.WaitingType.ScriptWait)
                {
                    BottomNewsLabel = $"{newsLabel}, Script Cool Time: {ts:d\\.hh\\:mm\\:ss} remaining.";
                }
                else
                {
                    BottomNewsLabel = $"{newsLabel}, Rebooting Cool Time: {ts:d\\.hh\\:mm\\:ss} remaining.";
                }
            }, () => BottomNewsLabel = newsLabel);
            _autoRestart.FewRemaining.Subscribe(ts =>
            {
                var sec = Math.Round(ts.TotalSeconds);
                SocTelnetSend($"say \"{string.Format(Setting.AutoRestartSendingMessageFormat, sec)}\"");
            });
            _autoRestart.ScriptRunning.Subscribe(args =>
            {
                BottomNewsLabel = $"{newsLabel}, AutoRestart: Waiting to run the script.";
            }, () => BottomNewsLabel = newsLabel);
            _autoRestart.Start(() => StopAutoRestart(true));

            AutoRestartText = "AutoRestart Enabled";
            AutoRestartEnabled = true;
        }

        public void StopRequestAutoRestart()
        {
            if (_autoRestart == null)
                return;

            _autoRestart.StopRequest();
        }

        public void StopAutoRestart(bool forceStop)
        {
            if (_autoRestart == null)
                return;

            if (!forceStop && _autoRestart.IsRestarting)
                return;

            _autoRestart?.Dispose();
            _autoRestart = null;
            AutoRestartText = "AutoRestart Disabled";
            AutoRestartEnabled = false;
        }

        private bool FileExistCheck(bool isAsync = false)
        {
            try
            {
                var fi = new FileInfo(ExeFilePath);
                if (!fi.Exists)
                {
                    ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = string.Format(Resources.Not_Found_0, "7DaysToDieServer.exe"), IsAsync = isAsync });
                    return false;
                }
            }
            catch (ArgumentException)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = string.Format(Resources._0_Is_Invalid, Resources.ServerFilePath), IsAsync = isAsync });
                return false;
            }

            try
            {
                var fi = new FileInfo(ConfigFilePath);
                if (!fi.Exists)
                {
                    ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = string.Format(Resources.Not_Found_0, Resources.ConfigFilePath), IsAsync = isAsync });
                    return false;
                }
            }
            catch (ArgumentException)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = string.Format(Resources._0_Is_Invalid, "7DaysToDieServer.exe"), IsAsync = isAsync });
                return false;
            }
            return true;
        }
        
        public void SwitchTelnetConnection()
        {
            if (!IsConnected)
            {
                _ = ConnectTelnet().ContinueWith((task) =>
                {
                    var innerExceptions = task.Exception?.InnerExceptions;
                    if (innerExceptions != null)
                    {
                        foreach (var exception in innerExceptions)
                        {
                            App.ShowAndWriteException(exception);
                        }
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            else
            {
                DisconnectTelnet();
            }
        }
        private async Task ConnectTelnet()
        {
            var localAddress = Address;
            var localPort = _port;
            var localPassword = Password;

            if (LocalMode)
            {
                localAddress = "127.0.0.1";

                if (!File.Exists(ConfigFilePath))
                {
                    ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = string.Format(Resources.Not_Found_0, Resources.ConfigFile) });
                    return;
                }

                var checkedValues = ConfigChecker.GetConfigInfo(ConfigFilePath);
                if (checkedValues.IsFailed)
                {
                    ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = checkedValues.Message });
                    return;
                }

                localPassword = checkedValues.Password;
                localPort = checkedValues.Port;
            }

            StartBtEnabled = false;
            TelnetBtIsEnabled = false;
            Telnet = GenerateTelnetClient(this);
            var connected = await Task.Factory.StartNew(() => LockFunction(telnet => telnet.Connect(localAddress, localPort)));
            TelnetBtIsEnabled = true;
            StartBtEnabled = true;

            IsFailed = false;
            if (connected)
            {
                TelnetBtLabel = Resources.UI_DisconnectFromTelnet;
                LocalModeEnabled = false;
                ConnectionPanelIsEnabled = false;
                StartBtEnabled = false;

                IsConnected = true;
                LockAction(telnet => telnet.Write(TelnetClient.Cr));
                AppendConsoleLog(SocTelnetSend(localPassword));

                if (IsExecuteScheduledCommand)
                {
                    await StartCommandRunner();
                }
            }
            else
            {
                BottomNewsLabel = Resources.Failed_Connecting;

                IsFailed = true;

                _ = Task.Factory.StartNew(() =>
                {
                    FeedColorChange(CommonStyleLib.ConstantValues.ActivatedBorderColor2);
                    FeedColorChange(CommonStyleLib.ConstantValues.ActivatedBorderColor);
                });
                TelnetFinish();
            }
        }
        private void DisconnectTelnet()
        {
            try
            {
                SocTelnetSendNrt("exit");
            }
            catch (System.Net.Sockets.SocketException)
            {

            }

            TelnetFinish();

            PlayerClean();
        }

        private static TelnetClient GenerateTelnetClient(MainWindowModel model)
        {
            var telnet = new TelnetClient
            {
                TelnetEventWaitTime = model.Setting.TelnetWaitTime
            };

            if (model._isLogGetter)
            {
                telnet.LogDirectory = ConstantValues.LogDirectoryPath;
            }

            telnet.Started += (sender, args) => model._telnetStartedSubject?.OnNext(args);
            telnet.Finished += (sender, args) => model._telnetFinishedSubject?.OnNext(args);
            telnet.ReadEvent += (sender, args) => model._telnetReadSubject?.OnNext(args);
            return telnet;
        }

        public void TelnetFinish()
        {
            StopCommandRunner();

            TelnetBtLabel = Resources.UI_ConnectWithTelnet;
            IsConnected = false;

            ConnectionPanelIsEnabled = !LocalMode;
            LocalModeEnabled = true;
            StartBtEnabled = true;

            CurrentProcessId = -1;

            if (Telnet != null)
            {
                lock (Telnet)
                {
                    Telnet?.Dispose();
                    Telnet = null;
                }
            }
        }

        public string GetChatText(string text)
        {
            _chatArray.Clear();
            _chatArray.AddMultiLine(text, _analyzerPlan);

            var sb = new StringBuilder();

            while (_chatArray.Any())
            {
                var data = _chatArray.Pop();
                if (data != null)
                    sb.AppendLine($"{data.Name}: {data.Message}");
            }

            return sb.ToString();
        }
        public void SendChat(string text, Action act)
        {
            if (CheckConnected())
            {
                LockAction(telnet => Chat.SendChat(telnet, text));
            }
            act();
        }

        //
        // プレイヤー取得
        //
        public void PlayerRefresh()
        {
            if (!CheckConnected())
                return;

            _connectedIds.Clear();
            PlayerClean();
            var playerInfoArray = LockFunction((telnet) => Player.GetPlayerInfoList(telnet, _analyzerPlan));
            foreach (var uDetail in playerInfoArray)
                AddUser(uDetail);
        }
        private void AddUser(PlayerInfo playerInfo)
        {
            var id = playerInfo.Id.ToInt();
            var pDict = _playersDictionary;
            var keys = _connectedIds;
            if (!pDict.ContainsKey(id))
            {
                var uDetail = new UserDetail()
                {
                    Id = playerInfo.Id,
                    Level = playerInfo.Level,
                    Name = playerInfo.Name,
                    Health = playerInfo.Health,
                    ZombieKills = playerInfo.ZombieKills,
                    PlayerKills = playerInfo.PlayerKills,
                    Death = playerInfo.Deaths,
                    Score = playerInfo.Score,
                    Coord = playerInfo.Coord,
                    SteamId = playerInfo.SteamId

                };
                pDict.Add(id, uDetail);
                keys.Add(id);
            }

            var details = new ObservableCollection<UserDetail>(pDict.Values);
            UsersList = details;
        }
        public void RemoveUser(string log)
        {
            var pDict = _playersDictionary;
            var keys = _connectedIds;

            var sr = new StringReader(log);
            while (sr.Peek() > -1)
            {
                //2017-04-20T00:01:57 11679.923 INF Player disconnected: EntityID=171, PlayerID='76561198010715714', OwnerID='76561198010715714', PlayerName='Aona Suzutsuki'
                const string expression = "(?<date>.*?) (?<number>.*?) INF Player disconnected: EntityID=(?<entityid>.*?), PlayerID='(?<steamid>.*?)', OwnerID='(?<ownerid>.*?)', PlayerName='(?<name>.*?)'$";
                var reg = new Regex(expression);

                var match = reg.Match(sr.ReadLine() ?? string.Empty);
                if (match.Success)
                {
                    var id = match.Groups["entityid"].Value.ToInt();
                    pDict.Remove(id);
                    keys.Remove(id);
                }
            }
            var collection = new ObservableCollection<UserDetail>(pDict.Values);
            UsersList = collection;
        }
        public void PlayerClean()
        {
            _playersDictionary.Clear();
            UsersList = null;
        }
        public PlayerInfo GetSelectedPlayerInfo(int index)
        {
            var playerInfo = UsersList[index].ToPlayerInfo();
            return playerInfo;
        }

        // Time
        public void SetTimeToTextBox()
        {
            if (!CheckConnected())
                return;

            var timeInfo = LockFunction((telnet) => Time.GetTimeFromTelnet(telnet, _analyzerPlan));

            TimeDayText = timeInfo.Day.ToString();
            TimeHourText = timeInfo.Hour.ToString();
            TimeMinuteText = timeInfo.Minute.ToString();
        }
        public void SetTimeToGame()
        {
            if (!CheckConnected())
                return;
            
            _messageBoxOccurred.OnNext(new MessageBoxOccurredEventArgs
            {
                Message = Resources.DoYouChangeTime,
                Title = Resources.Warning,
                MessageType = ExMessageBoxBase.MessageType.Asterisk,
                ButtonType = ExMessageBoxBase.ButtonType.YesNo,
                CallBack = (dialogResult) =>
                {
                    if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
                    {
                        var timeInfo = new TimeInfo()
                        {
                            Day = TimeDayText.ToInt(),
                            Hour = TimeHourText.ToInt(),
                            Minute = TimeMinuteText.ToInt()

                        };
                        LockAction(telnet => Time.SendTime(telnet, timeInfo));
                    }
                }
            });
        }


        public void AppendConsoleLog(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                _consoleTextAppended.OnNext(new AppendedLogTextEventArgs()
                {
                    AppendedLogText = text,
                    MaxLength = ConsoleTextLength
                });
            }
        }

        /*
         * Common Telnet Methods
         */
        public void SendCommand(string cmd, bool isCollect = true)
        {
#if DEBUG
            if (cmd == "gc")
            {
                GC.Collect(2, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();
                return;
            }
#endif

            if (isCollect && !string.IsNullOrEmpty(cmd))
                _commandCollector.AddCommand(cmd);
            _commandCollector.Reset();
            SocTelnetSendNrt(cmd);
        }

        public string GetPreviousCommand()
        {
            return _commandCollector.GetPreviousCommand();
        }

        public string GetNextCommand()
        {
            return _commandCollector.GetNextCommand();
        }

        public bool CheckConnected(bool isAsync = false, bool isShowError = true)
        {
            if (IsConnected)
                return true;

            if (isShowError)
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = Resources.HasnotBeConnected, IsAsync = isAsync });

            return false;
        }
        private void SocTelnetSendDirect(string cmd)
        {
            LockAction(telnet =>
            {
                telnet.Write(cmd);
                telnet.Write(TelnetClient.Crlf);
            });
        }
        private string SocTelnetSend(string cmd, bool isAsync = false)
        {
            if (!CheckConnected(isAsync))
                return null;

            SocTelnetSendDirect(cmd);

            var log = LockFunction(telnet => telnet.Read().TrimEnd('\0'));
            log += LockFunction(telnet => telnet.Read().TrimEnd('\0'));

            return log;
        }

        public bool SocTelnetSendNrt(string cmd)
        {
            if (!CheckConnected())
                return false;

            SocTelnetSendDirect(cmd);
            return true;
        }

        public bool SocTelnetSendNrtNer(string cmd, bool isAsync = false)
        {
            if (!CheckConnected(isAsync, false))
                return false;

            SocTelnetSendDirect(cmd);
            return true;
        }




        /*
         * Player Command
         */
        public UserDetail GetUserDetail(int index)
        {
            if (index < 0 || index >= UsersList.Count)
                return null;
            return UsersList[index];
        }
        public void RemoveAdmin(int index)
        {
            var playerId = UsersList[index].Id;
            if (string.IsNullOrEmpty(playerId))
            {
                _messageBoxOccurred.OnNext(new MessageBoxOccurredEventArgs
                {
                    Message = string.Format(Resources._0_is_Empty, "ID or Name"),
                    Title = CommonResources.Error
                });
                return;
            }

            SocTelnetSendNrt("admin remove " + playerId);
        }
        public void RemoveWhitelist(int index)
        {
            var playerId = UsersList[index].Id;
            if (string.IsNullOrEmpty(playerId))
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = string.Format(Resources._0_is_Empty, "ID or Name") });
                return;
            }

            SocTelnetSendNrt("whitelist remove " + playerId);
        }


        /*
         * Menu
         */
        public void RunConfigEditor()
        {
            var cfgArg = string.Empty;
            var configFilePath = Setting.ConfigFilePath;
            var isExists = !string.IsNullOrEmpty(configFilePath) && new FileInfo(configFilePath).Exists;

            if (isExists)
                cfgArg = "\"{0}\"".FormatString(configFilePath);

            var fi = new FileInfo(ConstantValues.ConfigEditorFilePath);
            if (fi.Exists)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fi.FullName,
                        Arguments = cfgArg,
                        WorkingDirectory = Path.GetDirectoryName(fi.FullName) ?? ConstantValues.AppDirectoryPath
                    }
                };
                process.Start();
            }
            else
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = string.Format(Resources._0_is_not_found, Resources.ConfigEditor) });
            }
        }

        public void RunXmlEditor()
        {
            var fi = new FileInfo(ConstantValues.XmlEditorFilePath);
            if (fi.Exists)
                Process.Start(fi.FullName);
            else
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs { ErrorMessage = string.Format(Resources._0_is_not_found, ConstantValues.XmlEditorFilePath) });
        }


        /*
         * Shortcut Key
         */
        public void PushShortcutKey(Key key)
        {
            if (ShortcutKeyManager.IsPushed("StartServerKey", Keyboard.Modifiers, key))
            {
                if (!IsConnected)
                    _ = ServerStart();
            }
            else if (ShortcutKeyManager.IsPushed("StopServerKey", Keyboard.Modifiers, key))
            {
                ServerStop();
            }
            else if (ShortcutKeyManager.IsPushed("ConTelnetKey", Keyboard.Modifiers, key))
            {
                if (!IsConnected)
                    _ = ConnectTelnet();
            }
            else if (ShortcutKeyManager.IsPushed("DisConTelnetKey", Keyboard.Modifiers, key))
            {
                if (IsConnected)
                {
                    DisconnectTelnet();
                }
            }
        }


        #region IDisposable
        // Flag: Has Dispose already been called?
        private bool _disposed;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Release()
        {
            if (Telnet != null)
            {
                lock (Telnet)
                {
                    Telnet?.Dispose();
                    Telnet = null;
                }
            }

            if (_autoRestart != null)
            {
                lock (_autoRestart)
                {
                    _autoRestart.Dispose();
                    _autoRestart = null;
                }
            }
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Release();

                if (_telnetFinishedSubject != null)
                {
                    lock (_telnetFinishedSubject)
                    {
                        _telnetFinishedSubject?.Dispose();
                        _telnetFinishedSubject = null;
                    }
                }


                if (_telnetStartedSubject != null)
                {
                    lock (_telnetStartedSubject)
                    {
                        _telnetStartedSubject?.Dispose();
                        _telnetStartedSubject = null;
                    }
                }


                if (_telnetReadSubject != null)
                {
                    lock (_telnetReadSubject)
                    {
                        _telnetReadSubject?.Dispose();
                        _telnetReadSubject = null;
                    }
                }

                Setting.Save();
                Setting.Dispose();
            }

            _disposed = true;
        }
        #endregion
    }
}
