using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BackupLib.Backup;
using CommonCoreLib.CommonPath;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Backup.Models
{
    public class BackupItem
    {
        public string Text { get; set; }
    }

    public class BackupSelectorModel : ModelBase
    {
        public ObservableCollection<string> BackupList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<BackupItem> BackupFileList { get; set; }

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


        private int backupProgressValue;
        private string progressLabel;

        private readonly TimeMachine timeMachine;

        public BackupSelectorModel()
        {
            BackupFileList = new ObservableCollection<BackupItem>
            {
                new BackupItem { Text = "1"},
                new BackupItem { Text = "2"},
                new BackupItem { Text = "3"}
            };

            var userDir = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            timeMachine = new TimeMachine("backup", $"{userDir}/7DaysToDie");
            timeMachine.BackupCompleted += TimeMachineOnBackupCompleted;
            timeMachine.BackupProgress += TimeMachineOnBackupProgress;
            timeMachine.BackupStarted += TimeMachineOnBackupStarted;
            timeMachine.RestoreProgress += TimeMachineOnRestoreProgress;

            InitializeBackupList();
        }

        private void InitializeBackupList()
        {
            BackupList.Clear();
            var trace = timeMachine.TraceBackup();
            foreach (var backupDate in trace)
            {
                BackupList.Add(backupDate);
            }
        }

        private void TimeMachineOnRestoreProgress(object sender, TimeMachine.BackupProgressEventArgs eventArgs)
        {

        }

        private void TimeMachineOnBackupStarted(object sender, TimeMachine.BackupCompletedEventArgs eventArgs)
        {
            switch (eventArgs.CompletedMode)
            {
                case TimeMachine.Mode.Backup:
                    ProgressLabel = "Backup...";
                    break;
                case TimeMachine.Mode.ComputeHash:
                    ProgressLabel = "Analyze...";
                    break;
            }
        }

        private void TimeMachineOnBackupProgress(object sender, TimeMachine.BackupProgressEventArgs eventArgs)
        {
            BackupProgressValue = eventArgs.Percentage;
        }

        private void TimeMachineOnBackupCompleted(object sender, TimeMachine.BackupCompletedEventArgs eventArgs)
        {
            switch (eventArgs.CompletedMode)
            {
                case TimeMachine.Mode.NothingToBackup:
                    ProgressLabel = "Nothing to backup.";
                    break;
                case TimeMachine.Mode.Backup:
                    ProgressLabel = "Completed to backup.";
                    break;
            }
        }

        public void Backup()
        {
            Task.Factory.StartNew(timeMachine.Backup).ContinueWith((t) => InitializeBackupList());

        }
    }
}
