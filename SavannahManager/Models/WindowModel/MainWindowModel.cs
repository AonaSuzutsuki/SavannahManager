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
using _7dtd_svmanager_fix_mvvm.Settings;
using _7dtd_svmanager_fix_mvvm.Update.Models;
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
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using _7dtd_svmanager_fix_mvvm.Models.Ssh;

namespace _7dtd_svmanager_fix_mvvm.Models.WindowModel
{
    public class MainWindowModel : ModelBase, IMainWindowTelnet, IMainWindowServerStart, IRelease
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

        private readonly Subject<string> _errorOccurred = new Subject<string>();
        public IObservable<string> ErrorOccurred => _errorOccurred;

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


        private string _chatLogText;
        public string ChatLogText
        {
            get => _chatLogText;
            set => SetProperty(ref _chatLogText, value);
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
                StartBtEnabled = value;
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
        private string _sshConfigFileNameText;

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
        public string SshConfigFileNameText
        {
            get => _sshConfigFileNameText;
            set => SetProperty(ref _sshConfigFileNameText, value);
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
        private bool RowConnected => Telnet != null && Telnet.Connected;

        public bool AutoRestartEnabled { get; set; }

        public bool IsFailed { get; private set; }
        public bool IsTelnetLoading { get; protected set; }
        public SettingLoader Setting { get; }
        public ShortcutKeyManager ShortcutKeyManager { get; }

        private string ExeFilePath => Setting.ExeFilePath;
        private string ConfigFilePath => Setting.ConfigFilePath;

        public int ConsoleTextLength { get; private set; }

        public TelnetClient Telnet { get; private set; }
        #endregion

        #region Fiels
        private string _address = string.Empty;
        private int _port;
        private string _password = string.Empty;

        private readonly List<ChatInfo> _chatArray = new List<ChatInfo>();

        private readonly Dictionary<int, UserDetail> _playersDictionary = new Dictionary<int, UserDetail>();
        private readonly List<int> _connectedIds = new List<int>();

        private bool _isServerForceStop;

        private bool _isLogGetter;
        private LogStream _loggingStream;

        private AutoRestart _autoRestart;
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
            Setting = SettingLoader.SettingInstance;
            ShortcutKeyManager = new ShortcutKeyManager(ConstantValues.AppDirectoryPath + @"\KeyConfig.xml",
                ConstantValues.AppDirectoryPath + @"\Settings\KeyConfig\" + Resources.KeyConfigPath);
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

        public void Initialize()
        {
            Address = Setting.Address;
            PortText = Setting.Port.ToString();
            Password = Setting.Password;

            IsConsoleLogTextWrapping = Setting.IsConsoleLogTextWrapping;
            ConsoleTextLength = Setting.ConsoleTextLength;

            _isLogGetter = Setting.IsLogGetter;
            LocalMode = Setting.LocalMode;
            StartBtEnabled = LocalMode;

            IsBeta = Setting.IsBetaMode;

            SshAddressText = Setting.SshAddress;
            SshPortText = Setting.SshPort.ToString();
            SshUserNameText = Setting.SshUserName;
            SshExeFileDirectoryText = Setting.SshExeFileDirectory;
            SshConfigFileNameText = Setting.SshConfigFileName;

            Setting.ApplyCulture();
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
            Setting.Width = (int)width;
            Setting.Height = (int)height;
            Setting.Address = _address;
            Setting.LocalMode = _localMode;
            Setting.Port = _port;
            Setting.Password = _password;
            Setting.IsConsoleLogTextWrapping = IsConsoleLogTextWrapping;

            Setting.SshAddress = _sshAddressText;
            int.TryParse(_sshPortText, out var sshPort);
            Setting.SshPort = sshPort;
            Setting.SshUserName = _sshUserNameText;
            Setting.SshExeFileDirectory = _sshExeFileDirectoryText;
            Setting.SshConfigFileName = _sshConfigFileNameText;
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



        public async Task ServerStart()
        {
            if (!FileExistCheck()) return;

            var checkedValues = ConfigChecker.GetConfigInfo(ConfigFilePath);
            if (checkedValues.IsFailed)
            {
                _errorOccurred.OnNext(checkedValues.Message);
                return;
            }

            const string localAddress = "127.0.0.1";
            var localPassword = checkedValues.Password;
            var localPort = checkedValues.Port;

            if (IsConnected)
            {
                _errorOccurred.OnNext(Resources.AlreadyConnected);
                return;
            }

            var serverProcessManager = new ServerProcessManager(ExeFilePath, ConfigFilePath);
            void ProcessFailedAction(string message) => _errorOccurred.OnNext(message);
            if (!serverProcessManager.ProcessStart(ProcessFailedAction))
                return;

            StartBtEnabled = false;
            TelnetBtIsEnabled = false;
            LocalModeEnabled = false;

            BottomNewsLabel = Resources.UI_WaitingServer;
            SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor2);

            await ConnectTelnetForServerStart(localAddress, localPort, localPassword);
        }
        public async Task ServerStartWithSsh()
        {
            if (IsConnected)
            {
                _errorOccurred.OnNext(Resources.AlreadyConnected);
                return;
            }

            StartBtEnabled = false;
            TelnetBtIsEnabled = false;
            LocalModeEnabled = false;

            BottomNewsLabel = Resources.UI_WaitingServer;
            SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor2);

            try
            {
                using var sshManager = new SshServerManager(SshAddressText);
                sshManager.Connect(SshUserNameText, SshPasswordText);
                sshManager.StartServer($"cd {SshExeFileDirectoryText} " +
                                       $"&& ./startserver.sh -configfile={SshConfigFileNameText}");
                await Task.Delay(500);
            }
            catch (Renci.SshNet.Common.SshAuthenticationException)
            {
                _errorOccurred.OnNext("failed to authenticate on ssh.");
            }
            catch (System.Net.Sockets.SocketException)
            {
                _errorOccurred.OnNext("failed to connect ssh.");
            }

            await ConnectTelnetForServerStart(Address, _port, Password);
        }

        private async Task ConnectTelnetForServerStart(string address, int port, string password)
        {
            Telnet = GenerateTelnetClient(this);
            await Task.Factory.StartNew(() =>
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

                    if (Telnet.Connect(address, port))
                    {
                        IsConnected = true;
                        IsTelnetLoading = false;

                        TelnetBtIsEnabled = true;
                        TelnetBtLabel = Resources.UI_DisconnectFromTelnet;
                        LocalModeEnabled = false;
                        ConnectionPanelIsEnabled = false;
                        BottomNewsLabel = Resources.UI_FinishedLaunching;
                        SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor);

                        Telnet.Write(TelnetClient.Cr);
                        AppendConsoleLog(SocTelnetSend(password));

                        MakeLogStream();

                        break;
                    }

                    Thread.Sleep(2000);
                }
            });
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

        public bool StartAutoRestart()
        {
            if (!IsBeta || !LocalMode || !IsConnected)
            {
                var reason = "";
                if (!IsBeta)
                    reason = "because beta mod not enabled";
                else if (!LocalMode)
                    reason = "because local server mode not enabled";
                else if (!IsConnected)
                    reason = "because telnet are not connected";

                _errorOccurred.OnNext($"Cannot enable auto restart mode {reason}.");
                return false;
            }

            if (_autoRestart != null)
            {
                StopAutoRestart();
            }

            _autoRestart = new AutoRestart(this, new TimeSpan(0, 0, 2, 0));
            var newsLabel = BottomNewsLabel;
            _autoRestart.TimeProgress.Subscribe((ts) =>
            {
                BottomNewsLabel = $"{newsLabel}, AutoRestart: {ts:hh\\:mm\\:ss} remaining.";
                Debug.WriteLine($"AutoRestart: {ts} remaining.");
            }, () => BottomNewsLabel = newsLabel);
            _autoRestart.Start();

            AutoRestartText = "AutoRestart Enabled";
            AutoRestartEnabled = true;

            return true;
        }

        public void StopAutoRestart()
        {
            if (_autoRestart == null || _autoRestart.IsRestarting)
                return;

            _autoRestart?.Dispose();
            _autoRestart = null;
            AutoRestartText = "AutoRestart Disabled";
            AutoRestartEnabled = false;
        }

        private void MakeLogStream()
        {
            if (_isLogGetter)
                _loggingStream = new LogStream(ConstantValues.LogDirectoryPath);
        }

        public void WriteLogStream(string log)
        {
            _loggingStream?.WriteSteam(log);
        }

        private bool FileExistCheck()
        {
            try
            {
                var fi = new FileInfo(ExeFilePath);
                if (!fi.Exists)
                {
                    _errorOccurred.OnNext(string.Format(Resources.Not_Found_0, "7DaysToDieServer.exe"));
                    return false;
                }
            }
            catch (ArgumentException)
            {
                _errorOccurred.OnNext(string.Format(Resources._0_Is_Invalid, Resources.ServerFilePath));
                return false;
            }

            try
            {
                var fi = new FileInfo(ConfigFilePath);
                if (!fi.Exists)
                {
                    _errorOccurred.OnNext(string.Format(Resources.Not_Found_0, Resources.ConfigFilePath));
                    return false;
                }
            }
            catch (ArgumentException)
            {
                _errorOccurred.OnNext(string.Format(Resources._0_Is_Invalid, "7DaysToDieServer.exe"));
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
                    _errorOccurred.OnNext(string.Format(Resources.Not_Found_0, Resources.ConfigFile));
                    return;
                }

                var checkedValues = ConfigChecker.GetConfigInfo(ConfigFilePath);
                if (checkedValues.IsFailed)
                {
                    _errorOccurred.OnNext(checkedValues.Message);
                    return;
                }

                localPassword = checkedValues.Password;
                localPort = checkedValues.Port;
            }

            StartBtEnabled = false;
            TelnetBtIsEnabled = false;
            Telnet = GenerateTelnetClient(this);
            var connected = await Task.Factory.StartNew(() => Telnet.Connect(localAddress, localPort));
            TelnetBtIsEnabled = true;
            if (LocalMode)
                StartBtEnabled = true;

            IsFailed = false;
            if (connected)
            {
                MakeLogStream();
                TelnetBtLabel = Resources.UI_DisconnectFromTelnet;
                LocalModeEnabled = false;
                ConnectionPanelIsEnabled = false;
                StartBtEnabled = false;

                IsConnected = true;
                Telnet.Write(TelnetClient.Cr);
                AppendConsoleLog(SocTelnetSend(localPassword));
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
            telnet.Started += (sender, args) => model._telnetStartedSubject?.OnNext(args);
            telnet.Finished += (sender, args) => model._telnetFinishedSubject?.OnNext(args);
            telnet.ReadEvent += (sender, args) => model._telnetReadSubject?.OnNext(args);
            return telnet;
        }

        public void TelnetFinish()
        {
            TelnetBtLabel = Resources.UI_ConnectWithTelnet;
            IsConnected = false;

            ConnectionPanelIsEnabled = !LocalMode;
            LocalModeEnabled = true;
            StartBtEnabled = LocalMode;

            _loggingStream?.Dispose();
            _loggingStream = null;
            Telnet?.Dispose();
            Telnet = null;
        }

        public void AddChatText(string text)
        {
            _chatArray.AddMultiLine(text);
            var cData = _chatArray.LastOrDefault();
            if (cData != null)
                ChatLogText += $"{cData.Name}: {cData.Message}\r\n";
        }
        public void SendChat(string text, Action act)
        {
            if (CheckConnected())
                Chat.SendChat(Telnet, text);
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
            var playerInfoArray = Player.GetPlayerInfoList(Telnet);
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

            var timeInfo = Time.GetTimeFromTelnet(Telnet);

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
                        Time.SendTime(Telnet, timeInfo);
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
        public void SendCommand(string cmd)
        {
#if DEBUG
            if (cmd == "gc")
            {
                GC.Collect(2, GCCollectionMode.Forced, true);
                GC.WaitForPendingFinalizers();
                return;
            }
#endif

            SocTelnetSendNrt(cmd);
        }
        public bool CheckConnected()
        {
            if (IsConnected)
                return true;

            _errorOccurred.OnNext(Resources.HasnotBeConnected);
            return false;
        }
        private void SocTelnetSendDirect(string cmd)
        {
            Telnet.Write(cmd);
            Telnet.Write(TelnetClient.Crlf);
        }
        private string SocTelnetSend(string cmd)
        {
            if (!CheckConnected())
                return null;

            SocTelnetSendDirect(cmd);
            Thread.Sleep(100);
            var log = Telnet.Read().TrimEnd('\0');
            log += Telnet.Read().TrimEnd('\0');

            return log;
        }

        public bool SocTelnetSendNrt(string cmd)
        {
            if (!CheckConnected())
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
                _errorOccurred.OnNext(string.Format(Resources._0_is_Empty, "ID or Name"));
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
                _errorOccurred.OnNext(string.Format(Resources._0_is_not_found, Resources.ConfigEditor));
            }
        }

        public void RunXmlEditor()
        {
            var fi = new FileInfo(ConstantValues.XmlEditorFilePath);
            if (fi.Exists)
                Process.Start(fi.FullName);
            else
                _errorOccurred.OnNext(string.Format(Resources._0_is_not_found, ConstantValues.XmlEditorFilePath));
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
            }

            _disposed = true;
        }
        #endregion
    }
}
