using System;
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
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Backup.ViewModels
{
    public class BackupSelectorViewModel : ViewModelBase
    {
        public BackupSelectorViewModel(Window view, BackupSelectorModel model) : base(view, model)
        {
            this.model = model;

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

            RestoreBtClicked = new DelegateCommand(RestoreBt_Clicked);
            BackupBtClicked = new DelegateCommand(BackupBt_Clicked);
            DeleteBtClicked = new DelegateCommand(DeleteBt_Clicked);
            DeleteAllBtClicked = new DelegateCommand(DeleteAllBt_Clicked);
            BackupListContextMenuOpened = new DelegateCommand(BackupListContextMenu_Opened);
            BackupListSelectionChanged = new DelegateCommand<int?>(BackupList_SelectionChanged);
            ForwardBtClicked = new DelegateCommand(ForwardBt_Clicked);
            BackBtClicked = new DelegateCommand(BackBt_Clicked);
            BackupFileListMouseDoubleClick = new DelegateCommand<PathMapItem>(BackupFileList_MouseDoubleClick);
        }

        #region Fields

        private readonly BackupSelectorModel model;

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

        public ICommand RestoreBtClicked { get; set; }
        public ICommand BackupBtClicked { get; set; }
        public ICommand DeleteBtClicked { get; set; }
        public ICommand DeleteAllBtClicked { get; set; }

        public ICommand BackupListContextMenuOpened { get; set; }

        public ICommand BackupListSelectionChanged { get; set; }

        public ICommand ForwardBtClicked { get; set; }
        public ICommand BackBtClicked { get; set; }
        public ICommand BackupFileListMouseDoubleClick { get; set; }


        #endregion

        #region Event Methods

        public void RestoreBt_Clicked()
        {
            model.Restore();
        }
        public void BackupBt_Clicked()
        {
            model.Backup();
        }

        public void DeleteBt_Clicked()
        {
            model.Delete();
        }
        public void DeleteAllBt_Clicked()
        {
            model.DeleteAll();
        }

        public void BackupListContextMenu_Opened()
        {
            model.MenuOpened();
        }

        public void BackupList_SelectionChanged(int? index)
        {
            if (index == null)
                return;

            var value = index.Value;
            model.SelectBackup(value);
            model.DrawBackup();
        }

        public void ForwardBt_Clicked()
        {
            model.DirectoryForward();
        }

        public void BackBt_Clicked()
        {
            model.DirectoryBack();
        }

        public void BackupFileList_MouseDoubleClick(PathMapItem pathMapItem)
        {
            model.NewDirectoryChange();
            model.DirectoryChange(pathMapItem);
        }

        #endregion
    }
}
