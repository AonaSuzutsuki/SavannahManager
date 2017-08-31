using KimamaLib;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using CommonLib.ExMessageBox;
using System.Diagnostics;
using SvManagerLibrary.Config;
using SvManagerLibrary.Telnet;
using System.Threading;
using System.Windows.Media;
using System.Collections.ObjectModel;
using _7dtd_svmanager_fix_mvvm.Settings;
using CommonLib.Models;
using LanguageEx;
using SvManagerLibrary.Chat;
using SvManagerLibrary.Player;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SvManagerLibrary.Time;
using KimamaLib.Extension;
using Log;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class CheckValue : ICheckValue, ICheckedValue
    {
        public string TelnetPort { get; set; }
        public string ControlPanelPort { get; set; }
        public string ServerPort { get; set; }
        public string TelnetEnabled { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
    interface ICheckValue
    {
        string TelnetPort { get; }
        string ControlPanelPort { get; }
        string ServerPort { get; }
        string TelnetEnabled { get; }
        string Password { get; }
    }
    interface ICheckedValue
    {
        string Password { get; }
        int Port { get; }
    }

    public class MainWindowModel : ModelBase, IMainWindowTelnet
    {
        public MainWindowModel(Window view)
        {
            this.view = view;
        }

        #region AppendedLogTextEvent
        public class AppendedLogTextEventArgs
        {
            public int MaxLength { get; set; }
            public string AppendedLogText { get; set; }
        }
        public delegate void AppendedLogTextEventHandler(object sender, AppendedLogTextEventArgs e);
        public event AppendedLogTextEventHandler AppendConsoleText;
        protected virtual void OnAppendConsoleText(AppendedLogTextEventArgs e)
        {
            AppendConsoleText?.Invoke(this, e);
        }
        #endregion

        #region PropertiesForViewModel
        private bool startBTEnabled = true;
        public bool StartBTEnabled
        {
            get => startBTEnabled;
            set => SetProperty(ref startBTEnabled, value);
        }
        private bool telnetBTIsEnabled = true;
        public bool TelnetBTIsEnabled
        {
            get => telnetBTIsEnabled;
            set => SetProperty(ref telnetBTIsEnabled, value);
        }
        private string telnetBTLabel;
        public string TelnetBTLabel
        {
            get => telnetBTLabel;
            set => SetProperty(ref telnetBTLabel, value);
        }

        private ObservableCollection<ViewModels.UserDetail> usersList;
        public ObservableCollection<ViewModels.UserDetail> UsersList
        {
            get => usersList;
            set => SetProperty(ref usersList, value);
        }


        private string chatLogText;
        public string ChatLogText
        {
            get => chatLogText;
            set => SetProperty(ref chatLogText, value);
        }
        private string chatInputText;
        public string ChatInputText
        {
            get => chatInputText;
            set => SetProperty(ref chatInputText, value);
        }

        private bool connectionPanelIsEnabled = true;
        public bool ConnectionPanelIsEnabled
        {
            get => connectionPanelIsEnabled;
            set => SetProperty(ref connectionPanelIsEnabled, value);
        }
        private bool localMode = true;
        public bool LocalMode
        {
            get => localMode;
            set
            {
                SetProperty(ref localMode, value);
                ConnectionPanelIsEnabled = !value;
                StartBTEnabled = value;
            }
        }
        private bool localModeEnabled = true;
        public bool LocalModeEnabled
        {
            get => localModeEnabled;
            set => SetProperty(ref localModeEnabled, value);
        }
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }
        private string portText = string.Empty;
        public string PortText
        {
            get => portText;
            set
            {
                SetProperty(ref portText, value);
                if (int.TryParse(value, out int port))
                {
                    this.port = port;
                }
            }
        }
        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private string timeDayText = "0";
        public string TimeDayText
        {
            get => timeDayText;
            set => SetProperty(ref timeDayText, value);
        }
        private string timeHourText = "0";
        public string TimeHourText
        {
            get => timeHourText;
            set => SetProperty(ref timeHourText, value);
        }
        private string timeMinuteText = "0";
        public string TimeMinuteText
        {
            get => timeMinuteText;
            set => SetProperty(ref timeMinuteText, value);
        }

        private string bottomNewsLabel;
        public string BottomNewsLabel
        {
            get => bottomNewsLabel;
            set => SetProperty(ref bottomNewsLabel, value);
        }
        #endregion

        #region Properties
        private bool isConnected;
        private bool IsConnected
        {
            get => isConnected && RowConnected;
            set => isConnected = value;
        }
        private bool RowConnected
        {
            get
            {
                if (telnet == null)
                    return false;
                return telnet.Connected;
            }
        }
        public bool IsFailed { get; private set; } = false;
        public bool IsTelnetLoading { get; protected set; } = false;

        private SettingLoader setting;
        public SettingLoader Setting { get => setting; }

        private ShortcutKeyManager shortcutKeyManager;
        public ShortcutKeyManager ShortcutKeyManager { get => shortcutKeyManager; }

        private string address = string.Empty;
        private int port = 0;
        private string password = string.Empty;

        private string ExeFilePath
        {
            get => setting.ExeFilePath;
        }
        private string ConfigFilePath
        {
            get => setting.ConfigFilePath;
        }
        private string AdminFilePath
        {
            get => setting.AdminFilePath;
        }

        private int consoleTextLength;
        #endregion

        #region Fiels
        private Window view;

        private LogStream logStream = new LogStream();
        private TelnetClient telnet = new TelnetClient();
        private ChatInfoArray chatArray = new ChatInfoArray();

        public static Dictionary<int, ViewModels.UserDetail> playersDictionary = new Dictionary<int, ViewModels.UserDetail>();
        public static List<int> connectedIds = new List<int>();

        private Thread logThread;

        private bool isSended = false;
        private bool isServerForceStop = false;
        private bool isStop;
        #endregion

        public override void Activated()
        {
            base.Activated();

            if (!IsTelnetLoading)
                AroundBorderColor = CommonLib.StaticData.ActivatedBorderColor;
            else
                AroundBorderColor = CommonLib.StaticData.ActivatedBorderColor2;
        }

        public void Initialize()
        {
            setting = SettingLoader.Setting = new SettingLoader(StaticData.SettingFilePath);
            shortcutKeyManager = new ShortcutKeyManager(StaticData.AppDirectoryPath + @"\KeyConfig.xml",
                StaticData.AppDirectoryPath + @"\Settings\KeyConfig\" + LangResources.Resources.KeyConfigPath);

            Width = setting.Width;
            Height = setting.Height;
            int screenWidth = (int)SystemParameters.WorkArea.Width;
            int screenHeight = (int)SystemParameters.WorkArea.Height;
            Top = (screenHeight - Height) / 2;
            Left = (screenWidth - Width) / 2;

            Address = setting.Address;
            PortText = setting.Port.ToString();
            Password = setting.Password;

            logStream.IsLogGetter = setting.IsLogGetter;
            LocalMode = setting.LocalMode;
            StartBTEnabled = LocalMode;

            consoleTextLength = setting.ConsoleTextLength;

            if (setting.IsFirstBoot)
            {
                var setUp = new Setup.Views.InitializeWindow(setting);
                setUp.ShowDialog();
            }

            if (setting.IsAutoUpdate)
            {
                //    Task tasks = Task.Factory.StartNew(() =>
                //    {
                //        bool isUpdate = false;
                //        string versionUrl = SharedData.UpdUrls.VersionUrl;
                //        var updator = new Updator.Updator(versionUrl, "version.txt", SharedData.UpdUrls.BetaAlpha);
                //        var updInfo = updator.Check();
                //        isUpdate = updInfo.isUpdate;
                //        updator.Delete();

                //        if (isUpdate)
                //        {
                //            View.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                //            {
                //                var dialogResult = ExMessageBoxBase.Show(this, "アップデートがあります。今すぐアップデートを行いますか？", "アップデートがあります。",
                //                    ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
                //                if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
                //                {
                //                    UpdatorForm.UpdForm updFrm = new UpdatorForm.UpdForm();
                //                    updFrm.ShowDialog();
                //                }
                //            }));
                //        }
                //    });
            }

            AddUser(new PlayerInfo()
            {
                Id = "171",
                Name = "test",
                Level = "1",
                Coord = "0",
                Deaths = "0",
                Health = "0",
                PlayerKills = "0",
                Score = "0",
                SteamId = "0",
                ZombieKills = "0"
            });
        }
        public void SettingsSave()
        {
            setting.Width = (int)width;
            setting.Height = (int)height;
            setting.Address = address;
            setting.LocalMode = localMode;
            setting.Port = port;
            setting.Password = password;
        }
        public void ChangeCulture(string cultureName)
        {
            ResourceService.Current.ChangeCulture(cultureName);
            setting.CultureName = ResourceService.Current.Culture;
        }

        public void RefreshLabels()
        {
            if (IsConnected)
            {
                TelnetBTLabel = LangResources.Resources.UI_DisconnectFromTelnet;
            }
            else
            {
                TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
            }

            if (IsTelnetLoading)
            {
                BottomNewsLabel = LangResources.Resources.UI_WaitingServer;
            }
            else if (IsFailed)
            {
                BottomNewsLabel = LangResources.Resources.Failed_Connecting;
            }
            else
            {
                BottomNewsLabel = LangResources.Resources.UI_ReadyComplete;
            }
        }
        private void FeedColorChange(SolidColorBrush color)
        {
            for (double _i = 1; _i >= 0; _i -= 0.1)
            {
                AroundBorderOpacity = _i;
                Thread.Sleep(5);
            }
            AroundBorderColor = color;
            for (double _i = 0; _i <= 1; _i += 0.1)
            {
                AroundBorderOpacity = _i;
                Thread.Sleep(5);
            }
        }

        public void ServerStart()
        {
            if (!FileExistCheck()) return;

            string address = "127.0.0.1";
            int port = 0;
            string password = string.Empty;

            ICheckedValue checkedValues = GetConfigInfo();
            if (checkedValues == null) { return; }
            password = checkedValues.Password;
            port = checkedValues.Port;

            if (IsConnected)
            {
                ExMessageBoxBase.Show(LangResources.Resources.AlreadyConnected, LangResources.CommonResources.Error
                       , ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            var serverProcessManager = new ServerProcessManager(ExeFilePath, ConfigFilePath);
            Action<string> processFailedAction = (message) => ExMessageBoxBase.Show(message, LangResources.CommonResources.Error
                        , ExMessageBoxBase.MessageType.Exclamation);
            if (!serverProcessManager.ProcessStart(processFailedAction))
                return;

            StartBTEnabled = false;
            TelnetBTIsEnabled = false;
            LocalModeEnabled = false;

            BottomNewsLabel = LangResources.Resources.UI_WaitingServer;
            base.SetBorderColor(CommonLib.StaticData.ActivatedBorderColor2);

            Task tasks = Task.Factory.StartNew(() =>
            {
                IsTelnetLoading = true;
                while (true)
                {
                    if (isServerForceStop)
                    {
                        TelnetBTIsEnabled = true;
                        TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
                        StartBTEnabled = true;
                        LocalModeEnabled = true;
                        ConnectionPanelIsEnabled = !LocalMode;
                        BottomNewsLabel = LangResources.Resources.UI_ReadyComplete;
                        AroundBorderColor = CommonLib.StaticData.ActivatedBorderColor;

                        IsTelnetLoading = false;
                        isServerForceStop = false;
                        break;
                    }

                    if (telnet.Connect(address, port))
                    {
                        IsConnected = true;
                        IsTelnetLoading = false;

                        TelnetBTIsEnabled = true;
                        TelnetBTLabel = LangResources.Resources.UI_DisconnectFromTelnet;
                        LocalModeEnabled = false;
                        ConnectionPanelIsEnabled = false;
                        BottomNewsLabel = LangResources.Resources.UI_FinishedLaunching;
                        base.SetBorderColor(CommonLib.StaticData.ActivatedBorderColor);

                        telnet.Write(TelnetClient.CR);
                        AppendConsoleLog(SocTelnetSend(password));

                        logStream.MakeStream(StaticData.LogDirectoryPath);

                        LaunchThread();

                        break;
                    }

                    Thread.Sleep(2000);
                }
            });
        }
        public void ServerStop(Action shutdownOpener)
        {
            if (IsTelnetLoading)
            {
                isServerForceStop = true;
                return;
            }

            if (IsConnected)
            {
                SocTelnetSend("shutdown");
                isStop = true;

                TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
                StartBTEnabled = true;
            }
            else
            {
                shutdownOpener();
            }
        }

        private bool FileExistCheck()
        {
            try
            {
                FileInfo fi = new FileInfo(ExeFilePath);
                if (!fi.Exists)
                {
                    ExMessageBoxBase.Show(string.Format(LangResources.Resources.Not_Found_0, "7DaysToDieServer.exe"), LangResources.CommonResources.Error
                        , ExMessageBoxBase.MessageType.Exclamation);
                    return false;
                }
            }
            catch (ArgumentException)
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_Is_Invalid, LangResources.Resources.ServerFilePath), LangResources.CommonResources.Error
                           , ExMessageBoxBase.MessageType.Exclamation);
                return false;
            }

            try
            {
                FileInfo fi = new FileInfo(ConfigFilePath);
                if (!fi.Exists)
                {
                    ExMessageBoxBase.Show(string.Format(LangResources.Resources.Not_Found_0, LangResources.Resources.ConfigFilePath), LangResources.CommonResources.Error
                           , ExMessageBoxBase.MessageType.Exclamation);
                    return false;
                }
            }
            catch (ArgumentException)
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_Is_Invalid, "7DaysToDieServer.exe"), LangResources.CommonResources.Error
                           , ExMessageBoxBase.MessageType.Exclamation);
                return false;
            }
            return true;
        }
        private ICheckedValue GetConfigInfo()
        {
            ConfigLoader configLoader = new ConfigLoader(ConfigFilePath);
            var portConfig = configLoader.GetValue("TelnetPort");
            string strPort = portConfig == null ? string.Empty : portConfig.Value;
            var cpPortConfig = configLoader.GetValue("ControlPanelPort");
            string cpPort = cpPortConfig == null ? string.Empty : cpPortConfig.Value;
            var svPortConfig = configLoader.GetValue("ServerPort");
            string svPort = svPortConfig == null ? string.Empty : svPortConfig.Value;

            var passConfig = configLoader.GetValue("TelnetPassword");
            string password = passConfig == null ? "CHANGEME" : passConfig.Value;
            string telnetEnabledString = configLoader.GetValue("TelnetEnabled").Value;

            ICheckValue checkValues = new CheckValue()
            {
                TelnetPort = strPort,
                ControlPanelPort = cpPort,
                ServerPort = svPort,
                TelnetEnabled = telnetEnabledString,
                Password = password
            };
            ICheckedValue checkedValues = CheckRightfulness(checkValues);

            return checkedValues;
        }
        private ICheckedValue CheckRightfulness(ICheckValue checkValues)
        {
            var checkedValues = new CheckValue();

            //TelnetEnabledチェック
            if (!bool.TryParse(checkValues.TelnetEnabled, out bool telnetEnabled))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_Is_Invalid, "TelnetEnabled"), LangResources.CommonResources.Error
                       , ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }
            if (!telnetEnabled)
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_1, "TelnetEnabled", "False"), LangResources.CommonResources.Error
                       , ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }

            // ポート被りチェック
            if (checkValues.TelnetPort.Equals(checkValues.ControlPanelPort))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources.Same_Port_as_0_has_been_used_in_1, "TelnetEnabled", "ControlPanelPort")
                    , LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }
            if (checkValues.TelnetPort.Equals(checkValues.ServerPort))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources.Same_Port_as_0_has_been_used_in_1, "TelnetEnabled", "ServerPort")
                    , LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }

            // Telnetポート変換チェック
            if (!int.TryParse(checkValues.TelnetPort, out int port))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_Is_Invalid, LangResources.Resources.Port), LangResources.CommonResources.Error
                                   , ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }
            checkedValues.Port = port;
            checkedValues.Password = checkValues.Password;
            return checkedValues;
        }

        public void TelnetConnectOrDisconnect()
        {
            if (!IsConnected)
            {
                TelnetConnect();
            }
            else
            {
                TelnetDisconnect();
            }
        }
        private void TelnetConnect()
        {
            string address = Address;
            int port = this.port;
            string password = Password;

            if (LocalMode)
            {
                address = "127.0.0.1";

                if (!File.Exists(ConfigFilePath))
                {
                    ExMessageBoxBase.Show(string.Format(LangResources.Resources.Not_Found_0, LangResources.Resources.ConfigFile), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                    return;
                }

                ICheckedValue checkedValues = GetConfigInfo();
                if (checkedValues == null) { return; }

                password = checkedValues.Password;
                port = checkedValues.Port;
            }
            
            IsFailed = false;
            if (telnet.Connect(address, port))
            {
                IsConnected = true;
                telnet.Write(TelnetClient.CR);
                AppendConsoleLog(SocTelnetSend(password));
            }
            else
            {
                BottomNewsLabel = LangResources.Resources.Failed_Connecting;

                IsFailed = true;

                Task tasks = Task.Factory.StartNew(() =>
                {
                    FeedColorChange(CommonLib.StaticData.ActivatedBorderColor2);
                    FeedColorChange(CommonLib.StaticData.ActivatedBorderColor);
                });

                return;
            }

            LaunchThread();

            logStream.MakeStream(StaticData.LogDirectoryPath);
            TelnetBTLabel = LangResources.Resources.UI_DisconnectFromTelnet;
            LocalModeEnabled = false;
            ConnectionPanelIsEnabled = false;
            StartBTEnabled = false;
        }
        private void TelnetDisconnect()
        {
            StopThread();

            try
            {
                SocTelnetSendNRT("exit");
            }
            catch (System.Net.Sockets.SocketException)
            {

            }
            telnet.Dispose();
            TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
            IsConnected = false;

            ConnectionPanelIsEnabled = !LocalMode;
            LocalModeEnabled = true;
            StartBTEnabled = LocalMode;

            PlayerClean();

            logStream.StreamDisposer();
        }

        private void LaunchThread()
        {
            logThread = new Thread(LogRead)
            {
                Priority = ThreadPriority.Highest
            };
            logThread.Start();
        }
        private void StopThread()
        {
            if (logThread != null)
                if (logThread.IsAlive)
                    logThread.Abort();
        }

        private void TelnetDispose()
        {
            telnet.Dispose();

            isStop = false;
            IsConnected = false;
            PlayerClean();

            TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
            LocalModeEnabled = true;
            ConnectionPanelIsEnabled = !LocalMode;
            StartBTEnabled = LocalMode;

            logStream.StreamDisposer();

            StopThread();
        }
        private void LogRead()
        {
            while (true)
            {
                if (LogLockCheck())
                {
                    Thread.Sleep(10);
                    continue;
                }

                try
                {
                    if (IsConnected)
                    {
                        string log = telnet.Read().Trim('\0');

                        if (isStop)
                            SocTelnetSendDirect("");

                        logStream.WriteSteam(log);
                        
                        AppendConsoleLog(log);

                        if (log.IndexOf("Chat") > -1)
                        {
                            AddChatText(log);
                        }
                        if (log.IndexOf("INF Created player with id=") > -1)
                        {
                            view.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                            {
                                PlayerRefresh();
                            }));
                        }
                        if (log.IndexOf("INF Player disconnected") > -1)
                        {
                            RemoveUser(log);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (System.Net.Sockets.SocketException)
                {
                    break;
                }

                Thread.Sleep(10);
            }

            TelnetDispose();
        }
        private void LogLock()
        {
            isSended = true;
        }
        private void LogUnlock()
        {
            isSended = false;
        }
        private bool LogLockCheck()
        {
            return isSended;
        }

        //
        // チャット
        //
        private void AddChatText(string text)
        {
            chatArray.Add(text);
            ChatInfo cData = chatArray.GetLast();
            ChatLogText += string.Format("{0}: {1}\r\n", cData.Name, cData.Message);
        }
        public void SendChat(string text)
        {
            if (CheckConnected())
                Chat.SendChat(telnet, text);
        }

        //
        // プレイヤー取得
        //
        public void PlayerRefresh()
        {
            if (!CheckConnected())
                return;

            LogLock();
            connectedIds.Clear();
            var playerInfoArray = Player.SetPlayerInfo(telnet);
            foreach (PlayerInfo uDetail in playerInfoArray)
                AddUser(uDetail);

            LogUnlock();
        }
        private void AddUser(PlayerInfo playerInfo)
        {
            int id = playerInfo.Id.ToInt();
            var pDict = playersDictionary;
            var keys = connectedIds;
            if (!pDict.ContainsKey(id))
            {
                var uDetail = new ViewModels.UserDetail()
                {
                    ID = playerInfo.Id,
                    Level = playerInfo.Level,
                    Name = playerInfo.Name,
                    Health = playerInfo.Health,
                    ZombieKills = playerInfo.ZombieKills,
                    PlayerKills = playerInfo.PlayerKills,
                    Death = playerInfo.Deaths,
                    Score = playerInfo.Score,
                    Coord = playerInfo.Coord,
                    SteamID = playerInfo.SteamId,

                };
                pDict.Add(id, uDetail);
                keys.Add(id);
            }

            var listdatas = new ObservableCollection<ViewModels.UserDetail>(pDict.Values);
            UsersList = listdatas;
        }
        private void RemoveUser(string log)
        {
            var pDict = playersDictionary;
            var keys = connectedIds;

            StringReader sr = new StringReader(log);
            while (sr.Peek() > -1)
            {
                //2017-04-20T00:01:57 11679.923 INF Player disconnected: EntityID=171, PlayerID='76561198010715714', OwnerID='76561198010715714', PlayerName='Aona Suzutsuki'
                const string expression = "(?<date>.*?) (?<number>.*?) INF Player disconnected: EntityID=(?<entityid>.*?), PlayerID='(?<steamid>.*?)', OwnerID='(?<ownerid>.*?)', PlayerName='(?<name>.*?)'$";
                var reg = new Regex(expression);

                var match = reg.Match(sr.ReadLine());
                if (match.Success == true)
                {
                    int id = match.Groups["entityid"].Value.ToInt();
                    pDict.Remove(id);
                    keys.Remove(id);
                }
            }
            var listdatas = new ObservableCollection<ViewModels.UserDetail>(pDict.Values);
            UsersList = listdatas;
        }
        private void PlayerClean()
        {
            playersDictionary.Clear();
            UsersList = null;
        }
        public void ShowPlayerInfo(int index)
        {
            var playerInfo = UsersList[index];
            string msg = playerInfo.ToString();
            CommonLib.ExMessageBox.ExMessageBoxBase.Show(msg, "Player Info", ExMessageBoxBase.MessageType.Asterisk);
        }

        // Time
        public void SetTimeToTextBox()
        {
            if (!CheckConnected())
                return;

            LogLock();
            var timeInfo = Time.GetTimeFromTelnet(telnet);

            TimeDayText = timeInfo.Day.ToString();
            TimeHourText = timeInfo.Hour.ToString();
            TimeMinuteText = timeInfo.Minute.ToString();
            LogUnlock();
        }
        public void SetTimeToGame()
        {
            if (!CheckConnected())
                return;

            var timeInfo = new TimeInfo()
            {
                Day = TimeDayText.ToInt(),
                Hour = TimeHourText.ToInt(),
                Minute = TimeMinuteText.ToInt()

            };
            Time.SendTime(telnet, timeInfo);
        }


        private void AppendConsoleLog(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                OnAppendConsoleText(new AppendedLogTextEventArgs()
                {
                    AppendedLogText = text,
                    MaxLength = consoleTextLength
                });
            }
        }

        public void SendCommand(string cmd)
        {
            SocTelnetSendNRT(cmd);
        }
        private bool CheckConnected(bool isAlert = false)
        {
            if (!IsConnected)
            {
                view.Dispatcher.Invoke(() =>
                {
                    ExMessageBoxBase.Show(LangResources.Resources.HasnotBeConnected, LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                });
                return false;
            }
            return true;
        }
        private void SocTelnetSendDirect(string cmd)
        {
            telnet.Write(cmd);
            telnet.Write(TelnetClient.CRLF);
        }
        private string SocTelnetSend(string cmd)
        {
            if (!CheckConnected())
                return null;

            SocTelnetSendDirect(cmd);
            string log = string.Empty;

            LogLock();
            Thread.Sleep(100);
            log = telnet.Read().TrimEnd('\0');
            log += telnet.Read().TrimEnd('\0');
            LogUnlock();

            return log;
        }
        public bool SocTelnetSendNRT(string cmd)
        {
            if (!CheckConnected())
                return false;

            SocTelnetSendDirect(cmd);
            return true;
        }



        /*
         * Player Command
         */
        public void AddAdmin(int index)
        {
            var playerInfo = UsersList[index];
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var add = new PlayerController.Views.Pages.Add(this, name);
            var playerBase = new PlayerController.Views.PlayerBase("Add", add);
            playerBase.ShowDialog();
        }
    }
}
