using KimamaLib;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using CommonStyleLib.ExMessageBox;
using System.Diagnostics;
using SvManagerLibrary.Config;
using SvManagerLibrary.Telnet;
using System.Threading;
using System.Windows.Media;
using System.Collections.ObjectModel;
using _7dtd_svmanager_fix_mvvm.Settings;
using CommonStyleLib.Models;

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

    public class MainWindowModel : ModelBase
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
        public bool IsConnected
        {
            get;
            set;
        }
        //public bool IsTelnetLoading { get; private set; } = false;
        public bool IsFailed { get; private set; } = false;
        
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

        private bool isLogGetter;

        private int consoleTextLength;
        #endregion

        #region Fiels
        private Window view;

        private string appPath = AppInfo.GetAppPath();
        private TelnetClient telnet = new TelnetClient();

        private Thread logThread;

        private bool isSended = false;
        private bool isServerStarted = false;
        private bool isServerForceStop = false;
        private bool isStop;
        #endregion

        public void Initialize()
        {
            setting = SettingLoader.Setting = new SettingLoader(appPath + @"\settings.ini");
            shortcutKeyManager = new ShortcutKeyManager(AppInfo.GetAppPath() + @"\KeyConfig.xml",
                AppInfo.GetAppPath() + @"\Settings\KeyConfig\" + LangResources.Resources.KeyConfigPath);

            Width = setting.Width;
            Height = setting.Height;
            int ScreenWidth = (int)SystemParameters.WorkArea.Width;
            int ScreenHeight = (int)SystemParameters.WorkArea.Height;
            Top = (ScreenHeight - Height) / 2;
            Left = (ScreenWidth - Width) / 2;

            //exeFilePath = setting.ExeFilePath;
            //adminFilePath = setting.AdminFilePath;
            //configFilePath = setting.ConfigFilePath;

            Address = setting.Address;
            PortText = setting.Port.ToString();
            Password = setting.Password;

            isLogGetter = setting.IsLogGetter;
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
                //view.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                //{
                    AroundBorderOpacity = _i;
                    Thread.Sleep(5);
                //}));
            }
            //view.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            //{
                AroundBorderColor = color;
            //}));
            for (double _i = 0; _i <= 1; _i += 0.1)
            {
                //view.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                //{
                    AroundBorderOpacity = _i;
                    Thread.Sleep(5);
                //}));
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

            Process p = new Process();
            p.StartInfo.FileName = ExeFilePath;
            p.StartInfo.Arguments = @"-quit -batchmode -nographics -configfile=""" + ConfigFilePath + @""" -dedicated";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.RedirectStandardInput = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(ExeFilePath);

            try
            {
                p.Start();
            }
            catch (Win32Exception ex)
            {
                ExMessageBoxBase.Show(ex.Message, LangResources.CommonResources.Error
                        , ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            isServerStarted = true;
            StartBTEnabled = false;
            TelnetBTIsEnabled = false;
            LocalModeEnabled = false;

            BottomNewsLabel = LangResources.Resources.UI_WaitingServer;
            if (!IsDeactivated)
            {
                AroundBorderColor = CommonStyleLib.StaticData.ActivatedBorderColor2;
            }

            Task tasks = Task.Factory.StartNew(() =>
            {
                IsTelnetLoading = true;
                while (true)
                {
                    if (isServerForceStop)
                    {
                        view.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        {
                            TelnetBTIsEnabled = true;
                            TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
                            StartBTEnabled = true;
                            LocalModeEnabled = true;
                            ConnectionPanelIsEnabled = !LocalMode;

                            BottomNewsLabel = LangResources.Resources.UI_ReadyComplete;

                            AroundBorderColor = CommonStyleLib.StaticData.ActivatedBorderColor;
                        }));

                        IsTelnetLoading = false;
                        isServerStarted = false;
                        isServerForceStop = false;

                        break;
                    }

                    if (telnet.Connect(address, port))
                    {
                        IsConnected = true;
                        IsTelnetLoading = false;

                        view.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        {
                            TelnetBTIsEnabled = true;
                            TelnetBTLabel = LangResources.Resources.UI_DisconnectFromTelnet;
                            LocalModeEnabled = false;
                            ConnectionPanelIsEnabled = false;

                            BottomNewsLabel = LangResources.Resources.UI_FinishedLaunching;

                            if (!IsDeactivated)
                            {
                                AroundBorderColor = CommonStyleLib.StaticData.ActivatedBorderColor;
                            }
                        }));
                        
                        telnet.Write(TelnetClient.CR);
                        AppendConsoleLog(SocTelnetSend(password));

                        if (isLogGetter)
                        {
                            Log.LogStream.MakeStream(appPath + @"\logs\");
                        }

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
            string password = passConfig == null ? string.Empty : passConfig.Value;
            string telnetEnabledString = configLoader.GetValue("TelnetEnabled").Value;
            configLoader.Dispose();

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
            int port = 0;
            if (!int.TryParse(checkValues.TelnetPort, out port))
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

            //
            // 空文字チェック
            //

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
                    FeedColorChange(CommonStyleLib.StaticData.ActivatedBorderColor2);
                    FeedColorChange(CommonStyleLib.StaticData.ActivatedBorderColor);
                });

                return;
            }

            LaunchThread();

            if (isLogGetter)
            {
                Log.LogStream.MakeStream(appPath + @"\logs\");
            }
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
            isServerStarted = false;
            IsConnected = false;

            ConnectionPanelIsEnabled = !LocalMode;
            LocalModeEnabled = true;
            StartBTEnabled = LocalMode;

            //PlayerClean();
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

        private void LogRead()
        {
            while (true)
            {
                if (isSended)
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
                        {
                            SocTelnetSend("");
                        }

                        if (isLogGetter)
                        {
                            Log.LogStream.WriteSteam(log);
                        }

                        if (!string.IsNullOrEmpty(log))
                        {
                            AppendConsoleLog(log);
                        }

                        //if (!string.IsNullOrEmpty(log) && !ConsoleIsFocus)
                        //{
                        //    this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        //    {
                        //        ConsoleTextBox.Select(ConsoleTextBox.Text.Length, 0);
                        //        ConsoleTextBox.ScrollToEnd();
                        //    }));
                        //}

                        //if (log.IndexOf("Chat") > -1)
                        //{
                        //    //Data.chats.Add(log);
                        //    SetMainChats(log);
                        //}
                        //if (log.IndexOf("INF Created player with id=") > -1)
                        //{
                        //    this.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                        //    {
                        //        Refresh();
                        //    }));
                        //}
                        //if (log.IndexOf("INF Player disconnected") > -1)
                        //{
                        //    RemoveUser(log);
                        //}
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

            telnet.Dispose();
            isStop = false;
            //PlayerClean();
            TelnetBTLabel = LangResources.Resources.UI_ConnectWithTelnet;
            LocalModeEnabled = true;
            ConnectionPanelIsEnabled = !LocalMode;
            isServerStarted = false;
            IsConnected = false;

            if (LocalMode)
            {
                StartBTEnabled = true;
            }

            Log.LogStream.StreamDisposer();

            StopThread();
        }

        public void SendCommand(string cmd)
        {
            SocTelnetSendNRT(cmd);
        }
        
        private void AppendConsoleLog(string text)
        {
            OnAppendConsoleText(new AppendedLogTextEventArgs()
            {
                AppendedLogText = text,
                MaxLength = consoleTextLength
            });
        }

        private string SocTelnetSend(string cmd, bool stop = false)
        {
            if (!IsConnected)
            {
                ExMessageBoxBase.Show(LangResources.Resources.HasnotBeConnected, LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return null;
            }

            telnet.Write(cmd);
            telnet.Write(TelnetClient.CRLF);
            string log = string.Empty;
            if (stop)
            {
                isSended = true;
                Thread.Sleep(100);
                log = telnet.Read().TrimEnd('\0');
                log += telnet.Read().TrimEnd('\0');
                isSended = false;
            }
            return log;
        }
        private void SocTelnetSendNRT(string cmd)
        {
            if (!IsConnected)
            {
                ExMessageBoxBase.Show(LangResources.Resources.HasnotBeConnected, LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }

            telnet.Write(cmd);
            telnet.Write(TelnetClient.CRLF);
        }
    }
}
