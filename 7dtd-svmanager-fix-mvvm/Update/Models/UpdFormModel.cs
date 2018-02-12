using CommonLib.ExMessageBox;
using CommonLib.Extentions;
using CommonLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class UpdFormModel : ModelBase
    {
        #region Fiels
        private UpdateLink updLink = UpdateLink.GetInstance;
        private UpdateManager updManager = null;

        private ObservableCollection<string> versionList = new ObservableCollection<string>();

        private bool canUpdate = false;
        private bool canCancel = true;

        private string detailText;
        private string currentVersion;
        private string latestVersion;
        #endregion

        #region Properties
        public ObservableCollection<string> VersionList
        {
            get => versionList;
            set => SetProperty(ref versionList, value);
        }

        public bool CanUpdate
        {
            get => canUpdate;
            set => SetProperty(ref canUpdate, value);
        }
        public bool CanCancel
        {
            get => canCancel;
            set => SetProperty(ref canCancel, value);
        }
        
        public string DetailText
        {
            get => detailText;
            set => SetProperty(ref detailText, value);
        }
        public string CurrentVersion
        {
            get => currentVersion;
            set => SetProperty(ref currentVersion, value);
        }
        public string LatestVersion
        {
            get => latestVersion;
            set => SetProperty(ref latestVersion, value);
        }
        #endregion



        public UpdFormModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            CurrentVersion = CommonLib.File.Version.GetVersion();

            CanCancel = false;
            updManager = new UpdateManager(updLink, ConstantValues.UpdatorFilePath);
            CanUpdate = updManager.IsUpdate;
            LatestVersion = updManager.Version;
            VersionList.AddAll(updManager.Updates.Keys);
            CanCancel = true;
        }

        public void ShowDetails(int index)
        {
            var version = VersionList[index];
            var detail = updManager.Updates[version];
            DetailText = detail;
        }

        public void Update()
        {
            if (updManager.IsUpdUpdate)
            {
                Task tasks = Task.Factory.StartNew(() =>
                {
                    string upzippath = ConstantValues.AppDirectoryPath + @"\update.zip";
                    using (var wc = new System.Net.WebClient())
                    {
                        wc.DownloadFile(updLink.UpPath, upzippath);
                    }
                    var fi = new FileInfo(upzippath);
                    if (fi.Exists)
                    {
                        Archive.Zip.Extract(fi.FullName, ConstantValues.AppDirectoryPath);
                        fi.Delete();
                    }
                });
                tasks.Wait();
            }

            int id = System.Diagnostics.Process.GetCurrentProcess().Id;
            var p = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ConstantValues.AppDirectoryPath + @"\update.exe",
                    Arguments = id.ToString() + " " + "SavannahManager2.exe" + " " + @"""" + updLink.MainPath + @""""
                }
            };

            try
            {
                p.Start();
            }
            catch (System.ComponentModel.Win32Exception)
            {
                ExMessageBoxBase.Show(string.Format(LangResources.UpdResources._0_is_not_found, LangResources.UpdResources.Updater)
                    , LangResources.UpdResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                return;
            }
            Application.Current.Shutdown();
        }
    }
}
