using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _7dtd_svmanager_fix_mvvm.Backup.Models.Image;
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
                    Date = pathMapItem.Date,
                });
            }

            return list;
        }
    }

    public class BackupSelectorModel : ModelBase
    {
        #region Properties
        public ObservableCollection<string> BackupList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<BackupItem> BackupFileList { get; set; } = new ObservableCollection<BackupItem>();

        public bool CanRestore
        {
            get => canRestore;
            set => SetProperty(ref canRestore, value);
        }

        public bool ForwardBtIsEnabled
        {
            get => forwardBtIsEnabled;
            set => SetProperty(ref forwardBtIsEnabled, value);
        }

        public bool BackBtIsEnabled
        {
            get => backBtIsEnabled;
            set => SetProperty(ref backBtIsEnabled, value);
        }

        public string PathText
        {
            get => pathText;
            set => SetProperty(ref pathText, value);
        }

        public int BackupProgressValue
        {
            get => backupProgressValue;
            set => SetProperty(ref backupProgressValue, value);
        }

        public string ProgressLabel
        {
            get => progressLabel;
            set => SetProperty(ref progressLabel, value);
        }
        #endregion

        #region Fields

        private string sevenDaysSavePath;

        private bool canRestore;
        private bool forwardBtIsEnabled;
        private bool backBtIsEnabled;
        private string pathText;
        private int backupProgressValue;
        private string progressLabel;

        private Stack<PathMapItem> forwardStack = new Stack<PathMapItem>();
        private PathMapItem current;
        #endregion

        private readonly TimeBackup timeBackup;

        public BackupSelectorModel()
        {
            var userDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            sevenDaysSavePath = $"{userDir}/7DaysToDie";
            timeBackup = new TimeBackup("backup", sevenDaysSavePath);
            timeBackup.BackupCompleted += TimeBackupOnBackupCompleted;
            timeBackup.BackupProgress += TimeBackupOnBackupProgress;
            timeBackup.BackupStarted += TimeBackupOnBackupStarted;
            timeBackup.RestoreProgress += TimeBackupOnRestoreProgress;

            InitializeBackupList();
        }

        private void InitializeBackupList()
        {
            BackupList.Clear();
            var trace = timeBackup.TraceBackup();
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

        public void Restore()
        {
            if (!timeBackup.CanRestore)
                return;

            var result = ExMessageBoxBase.Show("Are you sure to start restoring?", "Start to restore",
                ExMessageBoxBase.MessageType.Exclamation, ExMessageBoxBase.ButtonType.YesNo);
            if (result == ExMessageBoxBase.DialogResult.Yes)
                Task.Factory.StartNew(() => timeBackup.Restore());
        }

        public void Backup()
        {
            Task.Factory.StartNew(timeBackup.Backup).ContinueWith((t) => InitializeBackupList());
        }

        public void SelectBackup(int index)
        {
            if (index > -1)
            {
                timeBackup.SelectBackup(index);
                forwardStack = new Stack<PathMapItem>();
                ForwardBtIsEnabled = forwardStack.Count > 0;
            }
            CanRestore = timeBackup.CanRestore;
        }

        public void NewDirectoryChange()
        {
            forwardStack = new Stack<PathMapItem>();
            ForwardBtIsEnabled = false;
        }

        public void DirectoryForward()
        {
            var pathMapItem = forwardStack.Pop();
            DirectoryChange(pathMapItem);
            ForwardBtIsEnabled = forwardStack.Count > 0;
        }
        
        public void DirectoryBack()
        {
            if (current?.Parent != null)
            {
                forwardStack.Push(current);
                DirectoryChange(current.Parent);
            }
            ForwardBtIsEnabled = forwardStack.Count > 0;
        }

        public void DrawBackup()
        {
            var root = timeBackup.GetFileAndDirectories();
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
                    BackupFileList.AddAll(BackupItem.ToBackupItem(pathMapItem.Files));

                current = pathMapItem;
                BackBtIsEnabled = pathMapItem.Parent != null;
                PathText = pathMapItem.ToString();
            }
        }
    }
}
