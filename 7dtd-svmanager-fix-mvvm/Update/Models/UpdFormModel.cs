using CommonStyleLib.ExMessageBox;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UpdateLib.Http;
using UpdateLib.Update;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class UpdFormModel : ModelBase
    {
        #region Fiels
        private UpdateLink updLink = new UpdateLink();
        private UpdateClient updateClient;

        private ObservableCollection<string> versionList = new ObservableCollection<string>();
        private int versionListSelectedIndex = -1;

        private bool canUpdate = false;
        private bool canCancel = true;

        private ObservableCollection<RichTextItem> richDetailText = new ObservableCollection<RichTextItem>();

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
        public int VersionListSelectedIndex
        {
            get => versionListSelectedIndex;
            set => SetProperty(ref versionListSelectedIndex, value);
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


        public ObservableCollection<RichTextItem> RichDetailText
        {
            get => richDetailText;
            set => SetProperty(ref richDetailText, value);
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
        }

        public async Task Initialize()
        {
            CurrentVersion = ConstantValues.Version;

            var webClient = new UpdateWebClient
            {
                BaseUrl = "http://kimamalab.azurewebsites.net/updates/SavannahManager4/"
            };
            updateClient = new UpdateClient(webClient)
            {
                DetailVersionInfoDownloadUrlPath = $"details/{LangResources.UpdResources.Updater_XMLNAME}"
            };
            

            CanCancel = false;
            try
            {
                LatestVersion = await updateClient.GetVersion("main");
                CanUpdate = LatestVersion != CurrentVersion;
                var versionDetails = await updateClient.GetVersionInfo();
                VersionList.AddAll(versionDetails.Keys);
                CanCancel = true;

                if (VersionList.Count > 0)
                    VersionListSelectedIndex = 0;
                await ShowDetails(0);
            }
            catch (NotEqualsHashException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public async Task ShowDetails(int index)
        {
            if (index < 0 || index >= VersionList.Count)
                return;
            var version = VersionList[index];
            var detail = (await updateClient.GetVersionInfo())[version];
            DetailText = detail;
        }

        public async Task Update()
        {
            var updVersion = CommonCoreLib.CommonFile.Version.GetVersion(ConstantValues.UpdaterFilePath);
            var latestUpdVersion = await updateClient.GetVersion("updater");

            if (updVersion != latestUpdVersion)
            {
                try
                {
                    var updData = await updateClient.DownloadUpdateFile();
                    using var ms = new MemoryStream(updData.Length);
                    ms.Write(updData, 0, updData.Length);
                    ms.Seek(0, SeekOrigin.Begin);
                    using var zip = new UpdateLib.Archive.Zip(ms, ConstantValues.AppDirectoryPath + "/");
                    zip.Extract();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }

            int id = System.Diagnostics.Process.GetCurrentProcess().Id;
            var p = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = ConstantValues.UpdaterFilePath,
                    Arguments = $"{id} SavannahManager2.exe \"{updateClient.WebClient.BaseUrl}\" \"{updateClient.MainDownloadUrlPath}\""
                }
            };
            //42428 SavannahManager2.exe "http://kimamalab.azurewebsites.net/updates/SavannahManager3/SavannahManager.zip"

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
