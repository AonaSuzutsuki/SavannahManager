using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _7dtd_svmanager_fix_mvvm.Backup.Models.Image;
using _7dtd_svmanager_fix_mvvm.Models;
using BackupLib.Backup;
using BackupLib.CommonPath;
using CommonCoreLib.CommonPath;
using CommonExtensionLib.Extensions;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Backup.Models
{
    public class BackupItem : PathMapItem
    {
        public ImageSource ImageSource { get; set; }

        public string DateString
        {
            get
            {
                if (Date != DateTime.MinValue)
                    return Date.ToString("yyyy/MM/dd");
                return string.Empty;
            }
        }

        public static IEnumerable<BackupItem> ToBackupItem(IEnumerable<PathMapItem> pathMapItems)
        {
            var list = new List<BackupItem>();
            foreach (var pathMapItem in pathMapItems)
            {
                ImageSource imageSource = ImageLoader.LoadFromResource("_7dtd_svmanager_fix_mvvm.Backup.Images.no-image.png");
                if (pathMapItem.ItemType == PathMapItemType.File)
                    imageSource = ImageLoader.LoadFromResource("_7dtd_svmanager_fix_mvvm.Backup.Images.FileIcon.png");
                if (pathMapItem.ItemType == PathMapItemType.Directory)
                    imageSource = ImageLoader.LoadFromResource("_7dtd_svmanager_fix_mvvm.Backup.Images.DirectoryIcon.png");

                list.Add(new BackupItem
                {
                    ImageSource = imageSource,
                    Name = pathMapItem.Name,
                    Parent = pathMapItem.Parent,
                    FullPath = pathMapItem.FullPath,
                    ItemType = pathMapItem.ItemType,
                    Files = pathMapItem.Files,
                    Date = pathMapItem.Date
                });
            }

            return list;
        }
    }

    public class BackupSelectorModel : ModelBase
    {
        #region Constants
        #endregion

        #region Properties
        public ObservableCollection<string> BackupList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<BackupItem> BackupFileList { get; set; } = new ObservableCollection<BackupItem>();

        public bool CanRestore
        {
            get => _canRestore;
            set => SetProperty(ref _canRestore, value);
        }

        public bool CanDeleteAll
        {
            get => _canDeleteAll;
            set => SetProperty(ref _canDeleteAll, value);
        }

        public bool ForwardBtIsEnabled
        {
            get => _forwardBtIsEnabled;
            set => SetProperty(ref _forwardBtIsEnabled, value);
        }

        public bool BackBtIsEnabled
        {
            get => _backBtIsEnabled;
            set => SetProperty(ref _backBtIsEnabled, value);
        }

        public string PathText
        {
            get => _pathText;
            set => SetProperty(ref _pathText, value);
        }

        public int BackupProgressValue
        {
            get => _backupProgressValue;
            set => SetProperty(ref _backupProgressValue, value);
        }

        public string ProgressLabel
        {
            get => _progressLabel;
            set => SetProperty(ref _progressLabel, value);
        }
        #endregion

        #region Fields

        private TimeBackup2 _timeBackup;

        private readonly string _backupDirPath;
        private readonly string _restoreDirPath;

        private string _sevenDaysSavePath;

        private bool _canRestore;
        private bool _canDeleteAll;
        private bool _forwardBtIsEnabled;
        private bool _backBtIsEnabled;
        private string _pathText;
        private int _backupProgressValue;
        private string _progressLabel;

        private Stack<PathMapItem> _forwardStack = new Stack<PathMapItem>();
        private PathMapItem _current;
        #endregion


        public BackupSelectorModel(SettingLoader settingLoader)
        {
            _backupDirPath = settingLoader.BackupDirPath;
            _restoreDirPath = settingLoader.RestoreDirPath;

            Initialize();
        }

        public void Initialize()
        {
            var userDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            _sevenDaysSavePath = $"{userDir}/7DaysToDie";
            _timeBackup = new TimeBackup2(_backupDirPath, _sevenDaysSavePath);
            _timeBackup.BackupCompleted += TimeBackupOnBackupCompleted;
            _timeBackup.BackupProgress += TimeBackupOnBackupProgress;
            _timeBackup.BackupStarted += TimeBackupOnBackupStarted;
            _timeBackup.RestoreProgress += TimeBackupOnRestoreProgress;

            InitializeBackupList();
        }

        private void InitializeBackupList()
        {
            BackupList.Clear();
            var trace = _timeBackup.TraceBackup();
            foreach (var backupDate in trace)
            {
                BackupList.Add(backupDate);
            }
        }

        private void TimeBackupOnRestoreProgress(object sender, TimeBackup.BackupProgressEventArgs eventArgs)
        {
            BackupProgressValue = eventArgs.Percentage;
        }

        private void TimeBackupOnBackupStarted(object sender, TimeBackup.BackupCompletedEventArgs eventArgs)
        {
            switch (eventArgs.CompletedMode)
            {
                case TimeBackup.Mode.Restore:
                    ProgressLabel = "Restore...";
                    break;
                case TimeBackup.Mode.Backup:
                    ProgressLabel = "Backup...";
                    break;
                case TimeBackup.Mode.ComputeHash:
                    ProgressLabel = "Analyze...";
                    break;
            }
        }

        private void TimeBackupOnBackupProgress(object sender, TimeBackup.BackupProgressEventArgs eventArgs)
        {
            BackupProgressValue = eventArgs.Percentage;
        }

        private void TimeBackupOnBackupCompleted(object sender, TimeBackup.BackupCompletedEventArgs eventArgs)
        {
            switch (eventArgs.CompletedMode)
            {
                case TimeBackup.Mode.NothingToBackup:
                    ProgressLabel = "Nothing to backup.";
                    break;
                case TimeBackup.Mode.Backup:
                    ProgressLabel = "Completed to backup.";
                    break;
                case TimeBackup.Mode.Restore:
                    ProgressLabel = "Completed to restore.";
                    break;
            }
        }

        public void MenuOpened()
        {
            CanDeleteAll = new List<string>(_timeBackup.TraceBackup()).Count > 0;
        }

        public void Restore()
        {
            if (!_timeBackup.CanRestore)
                return;

            var result = ExMessageBoxBase.Show("Are you sure to start restoring?", "Start to restore",
                ExMessageBoxBase.MessageType.Exclamation, ExMessageBoxBase.ButtonType.YesNo);
            if (result == ExMessageBoxBase.DialogResult.Yes)
                Task.Factory.StartNew(() => _timeBackup.Restore(_restoreDirPath));
        }

        public void Backup()
        {
            Task.Factory.StartNew(_timeBackup.Backup).ContinueWith((t) => InitializeBackupList()).ContinueWith(t =>
            {
                if (t.Exception == null)
                    return;
                foreach (var exceptionInnerException in t.Exception.InnerExceptions)
                    App.ShowAndWriteException(exceptionInnerException);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void SelectBackup(int index)
        {
            if (index > -1)
            {
                _timeBackup.SelectBackup(index);
                _forwardStack = new Stack<PathMapItem>();
                ForwardBtIsEnabled = _forwardStack.Count > 0;
            }
            CanRestore = _timeBackup.CanRestore;
        }

        public void NewDirectoryChange()
        {
            _forwardStack = new Stack<PathMapItem>();
            ForwardBtIsEnabled = false;
        }

        public void DirectoryForward()
        {
            var pathMapItem = _forwardStack.Pop();
            DirectoryChange(pathMapItem);
            ForwardBtIsEnabled = _forwardStack.Count > 0;
        }
        
        public void DirectoryBack()
        {
            if (_current?.Parent != null)
            {
                _forwardStack.Push(_current);
                DirectoryChange(_current.Parent);
            }
            ForwardBtIsEnabled = _forwardStack.Count > 0;
        }

        public void DrawBackup()
        {
            var root = _timeBackup.GetFileAndDirectories();
            if (root != null && root.Files.Count > 0)
                DirectoryChange(root);
        }

        public void DirectoryChange(PathMapItem pathMapItem)
        {
            if (pathMapItem != null &&
                (pathMapItem.ItemType == PathMapItem.PathMapItemType.Directory || pathMapItem.ItemType == PathMapItem.PathMapItemType.Root))
            {
                BackupFileList.Clear();
                if (pathMapItem.Files.Count > 0)
                    BackupFileList.AddRange(BackupItem.ToBackupItem(pathMapItem.Files));

                _current = pathMapItem;
                BackBtIsEnabled = pathMapItem.Parent != null;
                PathText = pathMapItem.ToString();
            }
        }

        public void Delete()
        {
            if (!_timeBackup.CanRestore)
                return;

            _timeBackup.DeleteBackup();
            InitializeBackupList();

            BackupFileList.Clear();
            CanRestore = _timeBackup.CanRestore;
        }

        public void DeleteAll()
        {
            if (!Directory.Exists(_backupDirPath))
                return;

            var dirs = Directory.GetDirectories(_backupDirPath);
            foreach (var dir in dirs)
            {
                Directory.Delete(dir, true);
            }

            Initialize();
        }
    }
}
