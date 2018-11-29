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
            
            Task task = Task.Factory.StartNew(UpdateFunc);
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

                updateInfo = new UpdateInfo(pid, cmds[2], cmds[3]);
            }
            else
            {
                ExMessageBoxBase.Show(LangResources.Resources.Cannot_start_directly, LangResources.Resources.Warning, ExMessageBoxBase.MessageType.Exclamation, ExMessageBoxBase.ButtonType.OK);
                Environment.Exit(0);
            }
        }

        private void UpdateFunc()
        {
            if (updateInfo == null)
                return;

            CanClose = false;
            int pid = updateInfo.Pid;
            string filename = updateInfo.FileName;
            string zipPath = string.Format("{0}\\{1}.zip", ConstantValues.AppDirectoryPath, filename);
            string url = updateInfo.Url;

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

            StatusLabel = LangResources.Resources.Start_Update;
            ProgressValue = 0;
            ProgressIndeterminate = false;
            StatusLabel = LangResources.Resources.Downloading_the_update_file;

            Uri u = new Uri(url);
            var downloadClient = new System.Net.WebClient();
            downloadClient.DownloadProgressChanged += DownloadClient_DownloadProgressChanged;
            downloadClient.DownloadFileCompleted += DownloadClient_DownloadFileCompleted;
            downloadClient.DownloadFileAsync(u, zipPath);

            while (true)
            {
                if (finished)
                {
                    break;
                }
                System.Threading.Thread.Sleep(500);
            }

            if (cancelled)
            {
                CanExit(0);
                return;
            }
            else if (errored)
            {
                CanExit(1);
                return;
            }

            StatusLabel = LangResources.Resources.Extracting_the_update_file;
            ProgressValue = 0;

            Unzip(filename + ".zip");
            var fi = new FileInfo(zipPath);
            if (fi.Exists)
                fi.Delete();

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
