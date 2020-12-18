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
        private UpdateManager _updateManager;

        private ObservableCollection<string> _versionList = new ObservableCollection<string>();
        private int _versionListSelectedIndex = -1;

        private bool _canUpdate;
        private bool _canCancel = true;

        private ObservableCollection<RichTextItem> _richDetailText = new ObservableCollection<RichTextItem>();

        private string _detailText;
        private string _currentVersion;
        private string _latestVersion;
        #endregion

        #region Properties
        public ObservableCollection<string> VersionList
        {
            get => _versionList;
            set => SetProperty(ref _versionList, value);
        }
        public int VersionListSelectedIndex
        {
            get => _versionListSelectedIndex;
            set => SetProperty(ref _versionListSelectedIndex, value);
        }

        public bool CanUpdate
        {
            get => _canUpdate;
            set => SetProperty(ref _canUpdate, value);
        }
        public bool CanCancel
        {
            get => _canCancel;
            set => SetProperty(ref _canCancel, value);
        }


        public ObservableCollection<RichTextItem> RichDetailText
        {
            get => _richDetailText;
            set => SetProperty(ref _richDetailText, value);
        }

        public string DetailText
        {
            get => _detailText;
            set => SetProperty(ref _detailText, value);
        }
        public string CurrentVersion
        {
            get => _currentVersion;
            set => SetProperty(ref _currentVersion, value);
        }
        public string LatestVersion
        {
            get => _latestVersion;
            set => SetProperty(ref _latestVersion, value);
        }
        #endregion


        public async Task Initialize()
        {
            _updateManager = new UpdateManager();
            await _updateManager.Initialize();

            CurrentVersion = _updateManager.CurrentVersion;
            LatestVersion = _updateManager.LatestVersion;

            CanUpdate = _updateManager.IsUpdate;

            VersionList.AddAll(_updateManager.GetVersions());
            if (VersionList.Count > 0)
                VersionListSelectedIndex = 0;
            ShowDetails(0);
        }

        public void ShowDetails(int index)
        {
            if (index < 0 || index >= VersionList.Count)
                return;
            var version = VersionList[index];
            var detail = _updateManager.Updates.Get(version);
            RichDetailText = new ObservableCollection<RichTextItem>(detail);
        }

        public async Task Update()
        {
            if (_updateManager.IsUpdUpdate)
            {
                try
                {
                    await _updateManager.ApplyUpdUpdate(Path.GetDirectoryName(ConstantValues.UpdaterFilePath) + "/");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            var id = System.Diagnostics.Process.GetCurrentProcess().Id;
            var p = new System.Diagnostics.Process
            {
                StartInfo = _updateManager.GetUpdaterInfo(id)
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
