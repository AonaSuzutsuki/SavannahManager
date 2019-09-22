using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using LanguageEx;
using Log;
using _7dtd_svmanager_fix_mvvm.Views;
using _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages;
using _7dtd_svmanager_fix_mvvm.Settings;
using CommonStyleLib.Models;
using CommonStyleLib.ExMessageBox;
using SvManagerLibrary.Time;
using SvManagerLibrary.Config;
using SvManagerLibrary.Telnet;
using SvManagerLibrary.Chat;
using SvManagerLibrary.Player;
using CommonExtensionLib.Extensions;

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

    public class MainWindowModel : ModelBase, IMainWindowTelnet, IDisposable
    {
        public MainWindowModel(Window view)
        {
            this.view = view;

            InitializeTelnet();
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
        private bool startBtEnabled = true;
        public bool StartBtEnabled
        {
            get => startBtEnabled;
            set => SetProperty(ref startBtEnabled, value);
        }
        private bool telnetBtIsEnabled = true;
        public bool TelnetBtIsEnabled
        {
            get => telnetBtIsEnabled;
            set => SetProperty(ref telnetBtIsEnabled, value);
        }
        private string telnetBtLabel;
        public string TelnetBtLabel
        {
            get => telnetBtLabel;
            set => SetProperty(ref telnetBtLabel, value);
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

        private bool isBeta = false;

        public bool IsBeta
        {
            get => isBeta;
            set => SetProperty(ref isBeta, value);
        }

        private bool localMode = true;
        public bool LocalMode
        {
            get => localMode;
            set
            {
                SetProperty(ref localMode, value);
                ConnectionPanelIsEnabled = !value;
                StartBtEnabled = value;
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
        private bool RowConnected => telnet != null && telnet.Connected;
        public bool IsFailed { get; private set; } = false;
        public bool IsTelnetLoading { get; protected set; } = false;
        public SettingLoader Setting { get; private set; }
        public ShortcutKeyManager ShortcutKeyManager { get; private set; }

        private string ExeFilePath => Setting.ExeFilePath;
        private string ConfigFilePath => Setting.ConfigFilePath;
        private string AdminFilePath => Setting.AdminFilePath;

        private int consoleTextLength;
        #endregion

        #region Fiels
        private readonly Window view;

        private string address = string.Empty;
        private int port = 0;
        private string password = string.Empty;

        private readonly LogStream logStream = new LogStream();
        private TelnetClient telnet;
        private readonly List<ChatInfo> chatArray = new List<ChatInfo>();

        private readonly Dictionary<int, ViewModels.UserDetail> playersDictionary = new Dictionary<int, ViewModels.UserDetail>();
        private readonly List<int> connectedIds = new List<int>();

        private bool isServerForceStop = false;
        #endregion

        public void InitializeTelnet()
        {
            if (telnet == null)
            {
                telnet = new TelnetClient();
                telnet.ReadEvent += TelnetReadEvent;
                telnet.Finished += Telnet_Finished;
                telnet.Started += Telnet_Started;
            }
        }

        private void Telnet_Started(object sender, TelnetClient.TelnetReadEventArgs e)
        {
            PlayerClean();
        }

        private void Telnet_Finished(object sender, TelnetClient.TelnetReadEventArgs e)
        {
            PlayerClean();
        }

        private void TelnetReadEvent(object sender, TelnetClient.TelnetReadEventArgs e)
        {
            var log = "{0}".FormatString(e.Log);

            //if (isStop)
            //    SocTelnetSendDirect("");

            logStream.WriteSteam(log);

            AppendConsoleLog(log);

            if (log.IndexOf("Chat", StringComparison.Ordinal) > -1)
            {
                AddChatText(log);
            }
            if (log.IndexOf("INF Created player with id=", StringComparison.Ordinal) > -1)
            {
                view.Dispatcher?.Invoke(DispatcherPriority.Background, new Action(PlayerRefresh));
            }
            if (log.IndexOf("INF Player disconnected", StringComparison.Ordinal) > -1)
            {
                RemoveUser(log);
            }
        }

        public override void Activated()
        {
            base.Activated();

            if (!IsTelnetLoading)
                AroundBorderColor = CommonStyleLib.ConstantValues.ActivatedBorderColor;
            else
                AroundBorderColor = CommonStyleLib.ConstantValues.ActivatedBorderColor2;
        }

        public async void Initialize()
        {
            Setting = SettingLoader.SettingInstance;
            ShortcutKeyManager = new ShortcutKeyManager(ConstantValues.AppDirectoryPath + @"\KeyConfig.xml",
                ConstantValues.AppDirectoryPath + @"\Settings\KeyConfig\" + LangResources.Resources.KeyConfigPath);

            Width = Setting.Width;
            Height = Setting.Height;
            int screenWidth = (int)SystemParameters.WorkArea.Width;
            int screenHeight = (int)SystemParameters.WorkArea.Height;
            Top = (screenHeight - Height) / 2;
            Left = (screenWidth - Width) / 2;

            Address = Setting.Address;
            PortText = Setting.Port.ToString();
            Password = Setting.Password;

            logStream.IsLogGetter = Setting.IsLogGetter;
            LocalMode = Setting.LocalMode;
            StartBtEnabled = LocalMode;

            consoleTextLength = Setting.ConsoleTextLength;

            IsBeta = Setting.IsBetaMode;

            if (Setting.IsFirstBoot)
            {
                var setUp = new Setup.Views.InitializeWindow(Setting);
                setUp.ShowDialog();
            }

            if (Setting.IsAutoUpdate)
            {
                (bool, string) avalableUpd = await Update.Models.UpdateManager.CheckUpdateAsync(new Update.Models.UpdateLink().VersionUrl, ConstantValues.Version);
                if (avalableUpd.Item1)
                {
                    var dialogResult = ExMessageBoxBase.Show("アップデートがあります。今すぐアップデートを行いますか？", "アップデートがあります。",
                            ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
                    if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
                    {
                        var updForm = new Update.Views.UpdForm();
                        updForm.Show();
                    }
                }
            }
        }
        public void SettingsSave()
        {
            Setting.Width = (int)width;
            Setting.Height = (int)height;
            Setting.Address = address;
            Setting.LocalMode = localMode;
            Setting.Port = port;
            Setting.Password = password;
        }
        public void ChangeCulture(string cultureName)
        {
            ResourceService.Current.ChangeCulture(cultureName);
            Setting.CultureName = ResourceService.Current.Culture;
        }

        public void RefreshLabels()
        {
            if (IsConnected)
            {
                TelnetBtLabel = LangResources.Resources.UI_DisconnectFromTelnet;
            }
            else
            {
                TelnetBtLabel = LangResources.Resources.UI_ConnectWithTelnet;
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

            StartBtEnabled = false;
            TelnetBtIsEnabled = false;
            LocalModeEnabled = false;

            BottomNewsLabel = LangResources.Resources.UI_WaitingServer;
            base.SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor2);

            Task tasks = Task.Factory.StartNew(() =>
            {
                IsTelnetLoading = true;
                while (true)
                {
                    if (isServerForceStop)
                    {
                        TelnetBtIsEnabled = true;
                        TelnetBtLabel = LangResources.Resources.UI_ConnectWithTelnet;
                        StartBtEnabled = true;
                        LocalModeEnabled = true;
                        ConnectionPanelIsEnabled = !LocalMode;
                        BottomNewsLabel = LangResources.Resources.UI_ReadyComplete;
                        AroundBorderColor = CommonStyleLib.ConstantValues.ActivatedBorderColor;

                        IsTelnetLoading = false;
                        isServerForceStop = false;
                        break;
                    }

                    if (telnet.Connect(address, port))
                    {
                        IsConnected = true;
                        IsTelnetLoading = false;

                        TelnetBtIsEnabled = true;
                        TelnetBtLabel = LangResources.Resources.UI_DisconnectFromTelnet;
                        LocalModeEnabled = false;
                        ConnectionPanelIsEnabled = false;
                        BottomNewsLabel = LangResources.Resources.UI_FinishedLaunching;
                        base.SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor);

                        telnet.Write(TelnetClient.Cr);
                        AppendConsoleLog(SocTelnetSend(password));

                        logStream.MakeStream(ConstantValues.LogDirectoryPath);

                        break;
                    }

                    Thread.Sleep(2000);
                }
            });
        }
        public void ServerStop()
        {
            if (IsTelnetLoading)
            {
                isServerForceStop = true;
                return;
            }

            if (IsConnected)
            {
                SocTelnetSend("shutdown");

                TelnetBtLabel = LangResources.Resources.UI_ConnectWithTelnet;
                StartBtEnabled = true;
            }
            else
            {
                var fs = new ForceShutdowner();
                fs.ShowDialog();
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
            var address = Address;
            var port = this.port;
            var password = Password;

            if (LocalMode)
            {
                address = "127.0.0.1";

                if (!File.Exists(ConfigFilePath))
                {
                    ExMessageBoxBase.Show(string.Format(LangResources.Resources.Not_Found_0, LangResources.Resources.ConfigFile), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                    return;
                }

                ICheckedValue checkedValues = GetConfigInfo();
                if (checkedValues == null)
                    return;

                password = checkedValues.Password;
                port = checkedValues.Port;
            }
            
            IsFailed = false;
            if (telnet.Connect(address, port))
            {
                IsConnected = true;
                telnet.Write(TelnetClient.Cr);
                AppendConsoleLog(SocTelnetSend(password));
            }
            else
            {
                BottomNewsLabel = LangResources.Resources.Failed_Connecting;

                IsFailed = true;

                _ = Task.Factory.StartNew(() =>
                {
                    FeedColorChange(CommonStyleLib.ConstantValues.ActivatedBorderColor2);
                    FeedColorChange(CommonStyleLib.ConstantValues.ActivatedBorderColor);
                });

                return;
            }

            logStream.MakeStream(ConstantValues.LogDirectoryPath);
            TelnetBtLabel = LangResources.Resources.UI_DisconnectFromTelnet;
            LocalModeEnabled = false;
            ConnectionPanelIsEnabled = false;
            StartBtEnabled = false;
        }
        private void TelnetDisconnect()
        {
            try
            {
                SocTelnetSendNRT("exit");
            }
            catch (System.Net.Sockets.SocketException)
            {

            }
            telnet.Dispose();
            TelnetBtLabel = LangResources.Resources.UI_ConnectWithTelnet;
            IsConnected = false;

            ConnectionPanelIsEnabled = !LocalMode;
            LocalModeEnabled = true;
            StartBtEnabled = LocalMode;

            PlayerClean();

            logStream.StreamDisposer();
        }

        //private void TelnetDispose()
        //{
        //    telnet.Dispose();

        //    isStop = false;
        //    IsConnected = false;
        //    PlayerClean();

        //    TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
        //    LocalModeEnabled = true;
        //    ConnectionPanelIsEnabled = !LocalMode;
        //    StartBTEnabled = LocalMode;

        //    logStream.StreamDisposer();
        //}

        //
        // チャット
        //
        private void AddChatText(string text)
        {
            chatArray.AddMultiLine(text);
            var cData = chatArray.GetLast();
            if (cData != null)
                ChatLogText += string.Format("{0}: {1}\r\n", cData.Name, cData.Message);
        }
        public void SendChat(string text, Action act)
        {
            if (CheckConnected())
                Chat.SendChat(telnet, text);
            act();
        }

        //
        // プレイヤー取得
        //
        public void PlayerRefresh()
        {
            if (!CheckConnected())
                return;

            connectedIds.Clear();
            playersDictionary.Clear();
            var playerInfoArray = Player.SetPlayerInfo(telnet);
            foreach (PlayerInfo uDetail in playerInfoArray)
                AddUser(uDetail);
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
                    SteamId = playerInfo.SteamId,

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
            ExMessageBoxBase.Show(msg, "Player Info", ExMessageBoxBase.MessageType.Asterisk);
        }

        // Time
        public void SetTimeToTextBox()
        {
            if (!CheckConnected())
                return;

            var timeInfo = Time.GetTimeFromTelnet(telnet);

            TimeDayText = timeInfo.Day.ToString();
            TimeHourText = timeInfo.Hour.ToString();
            TimeMinuteText = timeInfo.Minute.ToString();
        }
        public void SetTimeToGame()
        {
            if (!CheckConnected())
                return;

            var dialogResult = ExMessageBoxBase.Show(LangResources.Resources.DoYouChangeTime, LangResources.Resources.Warning, ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
            if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
            {
                var timeInfo = new TimeInfo()
                {
                    Day = TimeDayText.ToInt(),
                    Hour = TimeHourText.ToInt(),
                    Minute = TimeMinuteText.ToInt()

                };
                Time.SendTime(telnet, timeInfo);
            }
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

        /*
         * Common Telnet Methods
         */
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
            telnet.Write(TelnetClient.Crlf);
        }
        private string SocTelnetSend(string cmd)
        {
            if (!CheckConnected())
                return null;

            SocTelnetSendDirect(cmd);
            string log = string.Empty;

            Thread.Sleep(100);
            log = telnet.Read().TrimEnd('\0');
            log += telnet.Read().TrimEnd('\0');

            return log;
        }

        public bool SocTelnetSendNRT(string cmd)
        {
            if (!CheckConnected())
                return false;

            SocTelnetSendDirect(cmd);
            return true;
        }

        public void OpenGetIpAddress()
        {
            var ipAddressGetter = new IpAddressGetter();
            ipAddressGetter.Show();
        }
        public void OpenPortCheck()
        {
            var portCheck = new PortCheck();
            portCheck.Show();
        }



        /*
         * Player Command
         */
        public void AddAdmin(int index)
        {
            var playerInfo = UsersList[index];
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var adminAdd = new AdminAdd(this, AddType.Type.Admin, name);
            var playerBase = new PlayerController.Views.PlayerBase("Add", adminAdd);
            playerBase.ShowDialog();
        }
        public void RemoveAdmin(int index)
        {
            var playerId = UsersList[index].ID;
            if (string.IsNullOrEmpty(playerId))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            SocTelnetSendNRT("admin remove " + playerId);
        }
        public void AddWhitelist(int index)
        {
            var playerInfo = UsersList[index];
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var whitelistAdd = new AdminAdd(this, AddType.Type.Whitelist, name);
            var playerBase = new PlayerController.Views.PlayerBase("Whitelist", whitelistAdd);
            playerBase.ShowDialog();
        }
        public void RemoveWhitelist(int index)
        {
            var playerId = UsersList[index].ID;
            if (string.IsNullOrEmpty(playerId))
            {
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            SocTelnetSendNRT("whitelist remove " + playerId);
        }
        public void AddBan(int index)
        {
            var playerInfo = UsersList[index];
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var ban = new Ban(this, name);
            var playerBase = new PlayerController.Views.PlayerBase("Ban", ban);
            playerBase.ShowDialog();
        }
        public void Kick(int index)
        {
            var playerInfo = UsersList[index];
            var name = string.IsNullOrEmpty(playerInfo.ID) ? string.Empty : playerInfo.ID;

            var kick = new Kick(this, name);
            var playerBase = new PlayerController.Views.PlayerBase("Kick", kick);
            playerBase.ShowDialog();
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
                Process.Start(fi.FullName, cfgArg);
            else
                ExMessageBoxBase.Show(string.Format(LangResources.Resources._0_is_not_found, LangResources.Resources.ConfigEditor),
                    LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
        }

        public void ShowBackupEditor()
        {
            var backup = new Backup.Views.BackupSelector();
            backup.Show();
        }
        public void ShowSettings()
        {
            var setWin = new Settings.Views.SettingWindow(Setting, ShortcutKeyManager);
            setWin.ShowDialog();
            IsBeta = Setting.IsBetaMode;
        }
        public void ShowInitialize()
        {
            var initializeWindow = new Setup.Views.InitializeWindow(Setting);
            initializeWindow.ShowDialog();
        }
        public void ShowUpdForm()
        {
            var updForm = new Update.Views.UpdForm();
            updForm.Show();
        }
        public void ShowVersionForm()
        {
            var verInfo = new VersionInfo();
            verInfo.ShowDialog();
        }


        /*
         * Shortcut Key
         */
         public void PushShortcutKey(Key key)
        {
            if (ShortcutKeyManager.IsPushed("StartServerKey", Keyboard.Modifiers, key))
            {
                if (!IsConnected)
                    ServerStart();
            }
            else if (ShortcutKeyManager.IsPushed("StopServerKey", Keyboard.Modifiers, key))
            {
                ServerStop();
            }
            else if (ShortcutKeyManager.IsPushed("ConTelnetKey", Keyboard.Modifiers, key))
            {
                if (!IsConnected)
                    TelnetConnect();
            }
            else if (ShortcutKeyManager.IsPushed("DisConTelnetKey", Keyboard.Modifiers, key))
            {
                if (IsConnected)
                {
                    TelnetDisconnect();
                }
            }
        }



        public void Dispose()
        {
            try
            {
                telnet?.Dispose();
            }
            catch { }
        }
    }
}
