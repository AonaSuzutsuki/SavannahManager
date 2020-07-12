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
using _7dtd_svmanager_fix_mvvm.Update.Views;
using UpdateLib.Http;
using UpdateLib.Update;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class UpdFormModel : ModelBase
    {
        #region Fiels
        private UpdateManager updateManager;

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


        public async Task Initialize()
        {
            updateManager = new UpdateManager();
            await updateManager.Initialize();

            CurrentVersion = updateManager.CurrentVersion;
            LatestVersion = updateManager.LatestVersion;

            CanUpdate = updateManager.IsUpdate;

            VersionList.AddAll(updateManager.GetVersions());
            if (VersionList.Count > 0)
                VersionListSelectedIndex = 0;
            ShowDetails(0);
        }

        public void ShowDetails(int index)
        {
            if (index < 0 || index >= VersionList.Count)
                return;
            var version = VersionList[index];
            var detail = updateManager.Updates.Get(version);
            RichDetailText = new ObservableCollection<RichTextItem>(detail);
        }

        public async Task Update()
        {
            if (updateManager.IsUpdUpdate)
            {
                try
                {
                    await updateManager.ApplyUpdUpdate(Path.GetDirectoryName(ConstantValues.UpdaterFilePath) + "/");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            int id = System.Diagnostics.Process.GetCurrentProcess().Id;
            var p = new System.Diagnostics.Process
            {
                StartInfo = updateManager.GetUpdaterInfo(id)
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
