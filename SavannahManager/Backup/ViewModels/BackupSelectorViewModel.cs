﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Backup.Models;
using BackupLib.CommonPath;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Backup.ViewModels
{
    public class BackupSelectorViewModel : ViewModelBase
    {
        public BackupSelectorViewModel(WindowService windowService, BackupSelectorModel model) : base(windowService, model)
        {
            _model = model;

            BackupList = model.BackupList.ToReadOnlyReactiveCollection();
            BackupFileList = model.BackupFileList.ToReadOnlyReactiveCollection();
            BackupProgressValue = model.ObserveProperty(m => m.BackupProgressValue).ToReactiveProperty();
            ProgressLabel = model.ObserveProperty(m => m.ProgressLabel).ToReactiveProperty();
            RestoreBtEnabled = model.ObserveProperty(m => m.CanRestore).ToReactiveProperty();
            ForwardBtIsEnabled = model.ObserveProperty(m => m.ForwardBtIsEnabled).ToReactiveProperty();
            BackBtIsEnabled = model.ObserveProperty(m => m.BackBtIsEnabled).ToReactiveProperty();
            PathText = model.ToReactivePropertyAsSynchronized(m => m.PathText);
            DeleteBtEnabled = model.ObserveProperty(m => m.CanRestore).ToReactiveProperty();
            DeleteAllBtEnabled = model.ObserveProperty(m => m.CanDeleteAll).ToReactiveProperty();

            RestoreCommand = new DelegateCommand(RestoreBt_Clicked);
            BackupCommand = new DelegateCommand(BackupBt_Clicked);
            DeleteBackupCommand = new DelegateCommand(DeleteBt_Clicked);
            DeleteAllBackupCommand = new DelegateCommand(DeleteAllBt_Clicked);
            BackupListContextMenuOpened = new DelegateCommand(BackupListContextMenu_Opened);
            BackupListSelectionChanged = new DelegateCommand<int?>(BackupList_SelectionChanged);
            ForwardPageCommand = new DelegateCommand(ForwardBt_Clicked);
            BackPageCommand = new DelegateCommand(BackBt_Clicked);
            BackupFileListMouseDoubleClick = new DelegateCommand<PathMapItem>(BackupFileList_MouseDoubleClick);
        }

        #region Fields

        private readonly BackupSelectorModel _model;

        #endregion

        #region Properties

        public ReadOnlyReactiveCollection<string> BackupList { get; set; }
        public ReadOnlyReactiveCollection<BackupItem> BackupFileList { get; set; }

        public ReactiveProperty<bool> RestoreBtEnabled { get; set; }
        public ReactiveProperty<bool> ForwardBtIsEnabled { get; set; }
        public ReactiveProperty<bool> BackBtIsEnabled { get; set; }
        public ReactiveProperty<string> PathText { get; set; }
        public ReactiveProperty<int> BackupProgressValue { get; set; }
        public ReactiveProperty<string> ProgressLabel { get; set; }

        public ReactiveProperty<bool> DeleteBtEnabled { get; set; }
        public ReactiveProperty<bool> DeleteAllBtEnabled { get; set; }

        #endregion

        #region Event Properties

        public ICommand RestoreCommand { get; set; }
        public ICommand BackupCommand { get; set; }
        public ICommand DeleteBackupCommand { get; set; }
        public ICommand DeleteAllBackupCommand { get; set; }

        public ICommand BackupListContextMenuOpened { get; set; }

        public ICommand BackupListSelectionChanged { get; set; }

        public ICommand ForwardPageCommand { get; set; }
        public ICommand BackPageCommand { get; set; }
        public ICommand BackupFileListMouseDoubleClick { get; set; }


        #endregion

        #region Event Methods

        public void RestoreBt_Clicked()
        {
            _model.Restore();
        }
        public void BackupBt_Clicked()
        {
            _model.Backup();
        }

        public void DeleteBt_Clicked()
        {
            _model.Delete();
        }
        public void DeleteAllBt_Clicked()
        {
            _model.DeleteAll();
        }

        public void BackupListContextMenu_Opened()
        {
            _model.MenuOpened();
        }

        public void BackupList_SelectionChanged(int? index)
        {
            if (index == null)
                return;

            var value = index.Value;
            _model.SelectBackup(value);
            _model.DrawBackup();
        }

        public void ForwardBt_Clicked()
        {
            _model.DirectoryForward();
        }

        public void BackBt_Clicked()
        {
            _model.DirectoryBack();
        }

        public void BackupFileList_MouseDoubleClick(PathMapItem pathMapItem)
        {
            _model.NewDirectoryChange();
            _model.DirectoryChange(pathMapItem);
        }

        #endregion
    }
}
