using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using UpdateLib.Http;
using UpdateLib.Update;

namespace Updater.Models
{
    public class MainWindowModel : ModelBase
    {
        #region Fields
        UpdateInfo updateInfo;

        private int exitCode = 1;
        private bool finished = false;
        private bool errored = false;
        private bool cancelled = false;
        
        private string statusLabel;
        private double progressValue;
        private double progressMaximum;
        private bool progressIndeterminate = false;
        private string progressLabel;
        private bool canClose = true;
        #endregion

        #region Properties
        public string StatusLabel
        {
            get => statusLabel;
            set => SetProperty(ref statusLabel, value);
        }
        public double ProgressValue
        {
            get => progressValue;
            set => SetProperty(ref progressValue, value);
        }
        public double ProgressMaximum
        {
            get => progressMaximum;
            set => SetProperty(ref progressMaximum, value);
        }
        public bool ProgressIndeterminate
        {
            get => progressIndeterminate;
            set
            {
                SetProperty(ref progressIndeterminate, value);
            }
        }
        public string ProgressLabel
        {
            get => progressLabel;
            set => SetProperty(ref progressLabel, value);
        }
        public bool CanClose
        {
            get => canClose;
            set => SetProperty(ref canClose, value);
        }
        #endregion

        public MainWindowModel()
        {
        }

        public void Close()
        {
            try
            {
                if (exitCode == 0)
                {
                    Process.Start(System.IO.Path.GetDirectoryName(ConstantValues.AppDirectoryPath) + @"\" + updateInfo.FileName);
                }
            }
            catch
            {
                Environment.Exit(exitCode);
            }
        }

        public void Update()
        {
            Initialize();
            
            var task = UpdateFunc();
        }

        public void Initialize()
        {
            int pid = 0;
            string[] cmds = System.Environment.GetCommandLineArgs();
            if (cmds.Length > 1)
            {

                if (!int.TryParse(cmds[1], out pid))
                {
                    ExMessageBoxBase.Show(LangResources.Resources.Invaild_Argument, LangResources.Resources.Error, ExMessageBoxBase.MessageType.Beep, ExMessageBoxBase.ButtonType.OK);
                    Environment.Exit(0);
                }


                var updWebClient = new UpdateWebClient { BaseUrl = cmds[3] };
                updWebClient.DownloadProgressChanged += (sender, e) =>
                {
                    ProgressValue = (int)e.ProgressPercentage;
                    ProgressLabel = e.ProgressPercentage + "%";
                };
                var updateClient = new UpdateClient(updWebClient) { MainDownloadUrlPath = cmds[4] };
                updateClient.DownloadProgressChanged += UpdateClientOnDownloadProgressChanged;
                updateClient.DownloadCompleted += UpdateClientOnDownloadCompleted;

                updateInfo = new UpdateInfo(pid, cmds[2], updateClient);
            }
            else
            {
                ExMessageBoxBase.Show(LangResources.Resources.Cannot_start_directly, LangResources.Resources.Warning, ExMessageBoxBase.MessageType.Exclamation, ExMessageBoxBase.ButtonType.OK);
                Environment.Exit(0);
            }
        }

        private void UpdateClientOnDownloadCompleted(object sender, UpdateClient.DownloadCompletedEventArgs e)
        {
        }

        private void UpdateClientOnDownloadProgressChanged(object sender, UpdateClient.DownloadProgressEventArgs e)
        {
            ProgressValue = (int)e.Percentage;
            ProgressLabel = e.Percentage + "%";
        }

        private async Task UpdateFunc()
        {
            if (updateInfo == null)
                return;

            CanClose = false;
            var pid = updateInfo.Pid;
            var filename = updateInfo.FileName;
            var updateClient = updateInfo.Client;

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
                    return;
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

                StatusLabel = LangResources.Resources.Extracting_the_update_file;
                ProgressValue = 0;

                using var zip = new UpdateLib.Archive.Zip(ms, Path.GetDirectoryName(ConstantValues.AppDirectoryPath));
                zip.Extracted += (sender, args) => ProgressValue++;
                ProgressMaximum = zip.Count;
                await Task.Factory.StartNew(zip.Extract);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            

            StatusLabel = LangResources.Resources.Finished_updating;
            ProgressValue = ProgressMaximum;

            CanExit(0);
        }

        private void CanExit(int exitCode)
        {
            CanClose = true;
            this.exitCode = exitCode;
        }

        private void Unzip(string zipFileName)
        {
            using (Archive.Zip archive = new Archive.Zip(ConstantValues.AppDirectoryPath + @"\" + zipFileName, System.IO.Path.GetDirectoryName(ConstantValues.AppDirectoryPath)))
            {
                ProgressMaximum = archive.Count;
                archive.Extracted += Archive_Extracted;
                archive.Extract();
            }
        }
        private void Archive_Extracted(object sender, EventArgs e)
        {
            ProgressValue += 1;
        }

        private void DownloadClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                cancelled = true;
            }
            else if (e.Error != null)
            {
                ExMessageBoxBase.Show(e.Error.Message, LangResources.Resources.Error, ExMessageBoxBase.MessageType.Exclamation, ExMessageBoxBase.ButtonType.OK);

                errored = true;
            }

            finished = true;
        }

        private void DownloadClient_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            ProgressValue = (int)e.ProgressPercentage;
            ProgressLabel = e.ProgressPercentage.ToString() + "%";
        }
    }
}
