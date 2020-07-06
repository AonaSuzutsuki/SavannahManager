using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using LanguageEx;
using Log;
using _7dtd_svmanager_fix_mvvm.Settings;
using _7dtd_svmanager_fix_mvvm.Update.Models;
using _7dtd_svmanager_fix_mvvm.ViewModels;
using CommonStyleLib.Models;
using CommonStyleLib.ExMessageBox;
using SvManagerLibrary.Time;
using SvManagerLibrary.Config;
using SvManagerLibrary.Telnet;
using SvManagerLibrary.Chat;
using SvManagerLibrary.Player;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Views;

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

        public IMessageBoxWindowService MessageBoxWindowService { get; set; }

        private bool isConnected;
        private bool IsConnected
        {
            get => isConnected && RowConnected;
            set => isConnected = value;
        }
        private bool RowConnected => Telnet != null && Telnet.Connected;
        public bool IsFailed { get; private set; } = false;
        public bool IsTelnetLoading { get; protected set; } = false;
        public SettingLoader Setting { get; private set; }
        public ShortcutKeyManager ShortcutKeyManager { get; private set; }

        private string ExeFilePath => Setting.ExeFilePath;
        private string ConfigFilePath => Setting.ConfigFilePath;
        private string AdminFilePath => Setting.AdminFilePath;

        private int consoleTextLength;

        public TelnetClient Telnet { get; private set; }
        public LogStream LoggingStream { get; } = new LogStream();
        #endregion

        #region Fiels
        private string address = string.Empty;
        private int port = 0;
        private string password = string.Empty;

        private readonly List<ChatInfo> chatArray = new List<ChatInfo>();

        private readonly Dictionary<int, ViewModels.UserDetail> playersDictionary = new Dictionary<int, ViewModels.UserDetail>();
        private readonly List<int> connectedIds = new List<int>();

        private bool isServerForceStop = false;

        #endregion


        public MainWindowModel()
        {
            Telnet = new TelnetClient();

            Setting = SettingLoader.SettingInstance;
            ShortcutKeyManager = new ShortcutKeyManager(ConstantValues.AppDirectoryPath + @"\KeyConfig.xml",
                ConstantValues.AppDirectoryPath + @"\Settings\KeyConfig\" + LangResources.Resources.KeyConfigPath);
        }

        public override void Activated()
        {
            base.Activated();

            if (!IsTelnetLoading)
                AroundBorderColor = CommonStyleLib.ConstantValues.ActivatedBorderColor;
            else
                AroundBorderColor = CommonStyleLib.ConstantValues.ActivatedBorderColor2;
        }

        public void InitializeWindow()
        {
            Width = Setting.Width;
            Height = Setting.Height;
            int screenWidth = (int)SystemParameters.WorkArea.Width;
            int screenHeight = (int)SystemParameters.WorkArea.Height;
            Top = (screenHeight - Height) / 2;
            Left = (screenWidth - Width) / 2;
        }

        public void Initialize()
        {
            Address = Setting.Address;
            PortText = Setting.Port.ToString();
            Password = Setting.Password;

            LoggingStream.IsLogGetter = Setting.IsLogGetter;
            LocalMode = Setting.LocalMode;
            StartBtEnabled = LocalMode;

            consoleTextLength = Setting.ConsoleTextLength;

            IsBeta = Setting.IsBetaMode;
        }

        public async Task<ExMessageBoxBase.DialogResult> CheckUpdate()
        {
            if (Setting.IsAutoUpdate)
            {
                var availableUpdate = await UpdateManager.CheckCanUpdate(UpdateManager.GetUpdateClient());
                //var (availableUpdate, _) = await Update.Models.UpdateManager.CheckUpdateAsync(new Update.Models.UpdateLink().VersionUrl, ConstantValues.Version);
                if (availableUpdate)
                {
                    var dialogResult = MessageBoxWindowService.MessageBoxShow("アップデートがあります。今すぐアップデートを行いますか？", "アップデートがあります。",
                        ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
                    return dialogResult;
                }
            }
            return ExMessageBoxBase.DialogResult.No;
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

            var address = "127.0.0.1";
            var port = 0;
            var password = string.Empty;

            var checkedValues = GetConfigInfo();
            if (checkedValues == null) { return; }
            password = checkedValues.Password;
            port = checkedValues.Port;

            if (IsConnected)
            {
                MessageBoxWindowService.MessageBoxShow(LangResources.Resources.AlreadyConnected, LangResources.CommonResources.Error
                       , ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            var serverProcessManager = new ServerProcessManager(ExeFilePath, ConfigFilePath);
            void ProcessFailedAction(string message) => MessageBoxWindowService.MessageBoxShow(message, LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
            if (!serverProcessManager.ProcessStart(ProcessFailedAction))
                return;

            StartBtEnabled = false;
            TelnetBtIsEnabled = false;
            LocalModeEnabled = false;

            BottomNewsLabel = LangResources.Resources.UI_WaitingServer;
            base.SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor2);

            var tasks = Task.Factory.StartNew(() =>
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

                    if (Telnet.Connect(address, port))
                    {
                        IsConnected = true;
                        IsTelnetLoading = false;

                        TelnetBtIsEnabled = true;
                        TelnetBtLabel = LangResources.Resources.UI_DisconnectFromTelnet;
                        LocalModeEnabled = false;
                        ConnectionPanelIsEnabled = false;
                        BottomNewsLabel = LangResources.Resources.UI_FinishedLaunching;
                        base.SetBorderColor(CommonStyleLib.ConstantValues.ActivatedBorderColor);

                        Telnet.Write(TelnetClient.Cr);
                        AppendConsoleLog(SocTelnetSend(password));

                        LoggingStream.MakeStream(ConstantValues.LogDirectoryPath);

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
                isServerForceStop = true;
                return false;
            }

            if (IsConnected)
            {
                SocTelnetSend("shutdown");

                TelnetBtLabel = LangResources.Resources.UI_ConnectWithTelnet;
                StartBtEnabled = true;
            }
            else
            {
                return true;
            }

            return false;
        }

        private bool FileExistCheck()
        {
            try
            {
                FileInfo fi = new FileInfo(ExeFilePath);
                if (!fi.Exists)
                {
                    MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources.Not_Found_0, "7DaysToDieServer.exe"), LangResources.CommonResources.Error
                        , ExMessageBoxBase.MessageType.Exclamation);
                    return false;
                }
            }
            catch (ArgumentException)
            {
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_Is_Invalid, LangResources.Resources.ServerFilePath), LangResources.CommonResources.Error
                           , ExMessageBoxBase.MessageType.Exclamation);
                return false;
            }

            try
            {
                FileInfo fi = new FileInfo(ConfigFilePath);
                if (!fi.Exists)
                {
                    MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources.Not_Found_0, LangResources.Resources.ConfigFilePath), LangResources.CommonResources.Error
                           , ExMessageBoxBase.MessageType.Exclamation);
                    return false;
                }
            }
            catch (ArgumentException)
            {
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_Is_Invalid, "7DaysToDieServer.exe"), LangResources.CommonResources.Error
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
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_Is_Invalid, "TelnetEnabled"), LangResources.CommonResources.Error
                       , ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }
            if (!telnetEnabled)
            {
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_is_1, "TelnetEnabled", "False"), LangResources.CommonResources.Error
                       , ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }

            // ポート被りチェック
            if (checkValues.TelnetPort.Equals(checkValues.ControlPanelPort))
            {
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources.Same_Port_as_0_has_been_used_in_1, "TelnetEnabled", "ControlPanelPort")
                    , LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }
            if (checkValues.TelnetPort.Equals(checkValues.ServerPort))
            {
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources.Same_Port_as_0_has_been_used_in_1, "TelnetEnabled", "ServerPort")
                    , LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }

            // Telnetポート変換チェック
            if (!int.TryParse(checkValues.TelnetPort, out int port))
            {
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_Is_Invalid, LangResources.Resources.Port), LangResources.CommonResources.Error
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
                    MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources.Not_Found_0, LangResources.Resources.ConfigFile), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                    return;
                }

                ICheckedValue checkedValues = GetConfigInfo();
                if (checkedValues == null)
                    return;

                password = checkedValues.Password;
                port = checkedValues.Port;
            }
            
            IsFailed = false;
            if (Telnet.Connect(address, port))
            {
                LoggingStream.MakeStream(ConstantValues.LogDirectoryPath);
                TelnetBtLabel = LangResources.Resources.UI_DisconnectFromTelnet;
                LocalModeEnabled = false;
                ConnectionPanelIsEnabled = false;
                StartBtEnabled = false;

                IsConnected = true;
                Telnet.Write(TelnetClient.Cr);
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
            Telnet.Dispose();

            TelnetFinish();

            PlayerClean();
        }

        public void TelnetFinish()
        {
            TelnetBtLabel = LangResources.Resources.UI_ConnectWithTelnet;
            IsConnected = false;

            ConnectionPanelIsEnabled = !LocalMode;
            LocalModeEnabled = true;
            StartBtEnabled = LocalMode;

            LoggingStream.StreamDisposer();
        }

        //private void TelnetDispose()
        //{
        //    Telnet.Dispose();

        //    isStop = false;
        //    IsConnected = false;
        //    PlayerClean();

        //    TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
        //    LocalModeEnabled = true;
        //    ConnectionPanelIsEnabled = !LocalMode;
        //    StartBTEnabled = LocalMode;

        //    LoggingStream.StreamDisposer();
        //}

        //
        // チャット
        //
        public void AddChatText(string text)
        {
            chatArray.AddMultiLine(text);
            var cData = chatArray.GetLast();
            if (cData != null)
                ChatLogText += string.Format("{0}: {1}\r\n", cData.Name, cData.Message);
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

            connectedIds.Clear();
            playersDictionary.Clear();
            var playerInfoArray = Player.SetPlayerInfo(Telnet);
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
        public void RemoveUser(string log)
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
        public void PlayerClean()
        {
            playersDictionary.Clear();
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

            var dialogResult = MessageBoxWindowService.MessageBoxShow(LangResources.Resources.DoYouChangeTime, LangResources.Resources.Warning, ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
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


        public void AppendConsoleLog(string text)
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
                MessageBoxWindowService.MessageBoxDispatchShow(LangResources.Resources.HasnotBeConnected, LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);

                return false;
            }
            return true;
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
            string log = string.Empty;

            Thread.Sleep(100);
            log = Telnet.Read().TrimEnd('\0');
            log += Telnet.Read().TrimEnd('\0');

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
        public UserDetail GetUserDetail(int index)
        {
            if (index < 0 || index >= UsersList.Count)
                return null;
            return UsersList[index];
        }
        public void RemoveAdmin(int index)
        {
            var playerId = UsersList[index].ID;
            if (string.IsNullOrEmpty(playerId))
            {
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            SocTelnetSendNRT("admin remove " + playerId);
        }
        public void RemoveWhitelist(int index)
        {
            var playerId = UsersList[index].ID;
            if (string.IsNullOrEmpty(playerId))
            {
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_is_Empty, "ID or Name"), LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            SocTelnetSendNRT("whitelist remove " + playerId);
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
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_is_not_found, LangResources.Resources.ConfigEditor),
                    LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
        }

        public void RunXmlEditor()
        {
            var fi = new FileInfo(ConstantValues.XmlEditorFilePath);
            if (fi.Exists)
                Process.Start(fi.FullName);
            else
                MessageBoxWindowService.MessageBoxShow(string.Format(LangResources.Resources._0_is_not_found, ConstantValues.XmlEditorFilePath),
                    LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
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


        #region IDisposable
        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                Telnet?.Dispose();
            }

            disposed = true;
        }
        #endregion
    }
}
