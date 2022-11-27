using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommonExtensionLib.Extensions;
using UpdateLib.Http;
using UpdateLib.Update;

namespace Updater.Models
{
    public enum UpdateMode
    {
        Undefined,
        Update,
        Clean
    }

    public static class UpdateModeConverter
    {
        public static UpdateMode Convert(string text)
        {
            return text switch
            {
                "update" => UpdateMode.Update,
                "clean" => UpdateMode.Clean,
                _ => UpdateMode.Undefined
            };
        }
    }

    public class MainWindowModel : ModelBase, IDisposable
    {
        #region Fields
        UpdateInfo _updateInfo;

        public int ExitCode { get; set; } = 1;
        
        private string _statusLabel;
        private double _progressValue;
        private double _progressMaximum;
        private bool _progressIndeterminate;
        private string _progressLabel;
        private bool _canClose = true;
        
        private readonly StreamWriter _writer;
        #endregion

        #region Properties
        public string StatusLabel
        {
            get => _statusLabel;
            set => SetProperty(ref _statusLabel, value);
        }
        public double ProgressValue
        {
            get => _progressValue;
            set => SetProperty(ref _progressValue, value);
        }
        public double ProgressMaximum
        {
            get => _progressMaximum;
            set => SetProperty(ref _progressMaximum, value);
        }
        public bool ProgressIndeterminate
        {
            get => _progressIndeterminate;
            set
            {
                SetProperty(ref _progressIndeterminate, value);
            }
        }
        public string ProgressLabel
        {
            get => _progressLabel;
            set => SetProperty(ref _progressLabel, value);
        }
        public bool CanClose
        {
            get => _canClose;
            set => SetProperty(ref _canClose, value);
        }
        #endregion

        public MainWindowModel()
        {
            if (File.Exists("debug.xml"))
            {
                var reader = new SavannahXmlLib.XmlWrapper.SavannahXmlReader("debug.xml");
                var enabledText = reader.GetValue("/debug/property[@name='enabled']");
                bool.TryParse(enabledText, out var enabled);
                if (!enabled)
                    return;

                var logPath = reader.GetValue("/debug/property[@name='logPath']");
                _writer = new StreamWriter(logPath, true, Encoding.UTF8);
            }
        }

        public void Close()
        {
            try
            {
                if (ExitCode == 0)
                {
                    Directory.SetCurrentDirectory(_updateInfo.ExtractDirectoryPath);
                    Process.Start(_updateInfo.ExtractDirectoryPath + @"\" + _updateInfo.FileName);
                }
            }
            catch
            {
                Environment.Exit(ExitCode);
            }
        }

        public void WriteLog(string log)
        {
            _writer?.WriteLine($"{DateTime.Now} {log}");
            _writer?.Flush();
        }

        public async Task Update()
        {
            var (mode, values) = Initialize();
            
            if (mode == UpdateMode.Update)
            {
                await UpdateFunc();
            }
            else
            {
                await UpdateFunc(values.FirstOrDefault(), CleanFunc);
            }
        }

        public (UpdateMode mode, IEnumerable<string> values) Initialize()
        {
            var commands = Environment.GetCommandLineArgs().Skip(1).ToArray();

            WriteLog($"Start Init: {string.Join(" ", commands)}");

            var parser = new CommonCoreLib.Parser.CommandLineParameterParser(commands);
            parser.AddParameter("pid", 1);
            parser.AddParameter("name", 1);
            parser.AddParameter("base", 1);
            parser.AddParameter("main", 1);
            parser.AddParameter("mode", 1);
            parser.AddParameter("out", 1);

            parser.Parse();

            if (!parser.HasArguments)
            {
                ExMessageBoxBase.Show(LangResources.Resources.Cannot_start_directly, LangResources.Resources.Warning, ExMessageBoxBase.MessageType.Exclamation, ExMessageBoxBase.ButtonType.OK);
                Environment.Exit(0);
            }

            if (!int.TryParse(parser.GetArgumentValue("pid"), out var pid))
            {
                ExMessageBoxBase.Show(LangResources.Resources.Invaild_Argument, LangResources.Resources.Error, ExMessageBoxBase.MessageType.Beep, ExMessageBoxBase.ButtonType.OK);
                Environment.Exit(0);
            }

            var modeText = parser.GetArgumentValue("mode") ?? "update";
            var mode = UpdateModeConverter.Convert(modeText);

            var updWebClient = new UpdateWebClient { BaseUrl = parser.GetArgumentValue("base") };
            updWebClient.DownloadProgressChanged += (sender, e) =>
            {
                ProgressValue = (int)e.ProgressPercentage;
                ProgressLabel = e.ProgressPercentage + "%";
            };
            var updateClient = new UpdateClient(updWebClient) { MainDownloadUrlPath = parser.GetArgumentValue("main") };
            updateClient.DownloadProgressChanged += UpdateClientOnDownloadProgressChanged;
            updateClient.DownloadCompleted += UpdateClientOnDownloadCompleted;

            _updateInfo = new UpdateInfo(pid, parser.GetArgumentValue("name").Replace(".exe", ""), updateClient,
                parser.GetArgumentValue("out") ?? Path.GetDirectoryName(ConstantValues.AppDirectoryPath));


            WriteLog($"Finished Init: {mode} {updWebClient.BaseUrl} {parser.GetArgumentValue("main")}");

            return (mode, parser.GetValues());
        }

        private void UpdateClientOnDownloadCompleted(object sender, UpdateClient.DownloadCompletedEventArgs e)
        {
        }

        private void UpdateClientOnDownloadProgressChanged(object sender, UpdateClient.DownloadProgressEventArgs e)
        {
            ProgressValue = (int)e.Percentage;
            ProgressLabel = e.Percentage + "%";
        }

        private async Task CleanFunc(string listFilePath)
        {
            if (string.IsNullOrEmpty(listFilePath))
                return;

            if (_updateInfo == null)
                return;

            WriteLog("Start Clean");

            CanClose = false;
            var pid = _updateInfo.Pid;
            var filename = _updateInfo.FileName;

            if (pid > 0)
            {
                try
                {
                    ProgressMaximum = 100;
                    ProgressValue = 0;
                    ProgressIndeterminate = false;
                    StatusLabel = LangResources.Resources.Waiting_for_the_Application_end;

                    var p = Process.GetProcessesByName(filename);
                    while (p.Length > 0)
                    {
                        System.Threading.Thread.Sleep(500);
                        p = Process.GetProcessesByName(filename);
                    }
                }
                catch
                {
                    CanExit(1);
                    throw;
                }
            }

            await Task.Factory.StartNew(() =>
            {
                var targetFiles = File.ReadAllLines(listFilePath);

                ProgressMaximum = targetFiles.Length;
                ProgressValue = 0;
                ProgressIndeterminate = false;
                StatusLabel = LangResources.Resources.Cleaning_files;

                foreach (var dllFile in targetFiles.Where(File.Exists))
                {
                    File.Delete(dllFile);
                    ProgressValue++;

                    WriteLog($"Delete {dllFile}");
                }

                StatusLabel = LangResources.Resources.Finished_cleaning;
                ProgressValue = ProgressMaximum;
            });

            WriteLog("Finished Clean");
        }

        private async Task UpdateFunc(string filesListPath = null, Func<string, Task> successDownloadFunc = null)
        {
            if (_updateInfo == null)
                return;

            CanClose = false;
            var pid = _updateInfo.Pid;
            var filename = _updateInfo.FileName;
            var updateClient = _updateInfo.Client;

            if (pid > 0)
            {
                try
                {
                    ProgressMaximum = 100;
                    ProgressValue = 0;
                    ProgressIndeterminate = false;
                    StatusLabel = LangResources.Resources.Waiting_for_the_Application_end;

                    var p = Process.GetProcessesByName(filename);
                    while (p.Length > 0)
                    {
                        Thread.Sleep(500);
                        p = Process.GetProcessesByName(filename);
                    }
                }
                catch
                {
                    CanExit(1);
                    throw;
                }
            }

            StatusLabel = LangResources.Resources.Start_Update;
            ProgressValue = 0;
            ProgressIndeterminate = false;
            StatusLabel = LangResources.Resources.Downloading_the_update_file;

            try
            {
                var data = await updateClient.DownloadMainFile();
                using var ms = new MemoryStream(data.Length);
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);

                if (successDownloadFunc != null)
                {
                    await successDownloadFunc(filesListPath);
                    await Task.Delay(1000);
                }

                WriteLog("Start Update");

                StatusLabel = LangResources.Resources.Extracting_the_update_file;
                ProgressValue = 0;

                using var zip = new UpdateLib.Archive.Zip(ms, _updateInfo.ExtractDirectoryPath);
                zip.Extracted += (sender, args) =>
                {
                    WriteLog($"Update {++ProgressValue}/{args.Total} {args.FilePath}");
                };
                ProgressMaximum = zip.Count;
                await Task.Factory.StartNew(zip.Extract);
            }
            catch (Exception)
            {
                CanExit(1);
                throw;
            }

            if (filesListPath != null && File.Exists(filesListPath))
            {
                File.Delete(filesListPath);
                WriteLog($"Delete {filesListPath}");
            }

            StatusLabel = LangResources.Resources.Finished_updating;
            ProgressValue = ProgressMaximum;

            WriteLog("Finished Update");

            CanExit(0);
        }

        private void CanExit(int exitCode)
        {
            CanClose = true;
            this.ExitCode = exitCode;
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }
    }
}
