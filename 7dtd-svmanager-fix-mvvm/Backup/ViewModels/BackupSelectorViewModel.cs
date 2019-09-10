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

            BackupBtClicked = new DelegateCommand(BackupBt_Clicked);
        }

        #region Fields

        private BackupSelectorModel model;

        #endregion

        #region Properties

        public ReadOnlyReactiveCollection<string> BackupList { get; set; }
        public ReadOnlyReactiveCollection<BackupItem> BackupFileList { get; set; }

        public ReactiveProperty<int> BackupProgressValue { get; set; }
        public ReactiveProperty<string> ProgressLabel { get; set; }

        #endregion

        #region Event Properties

        public ICommand BackupBtClicked { get; set; }

        #endregion

        #region Event Methods

        public void BackupBt_Clicked()
        {
            model.Backup();
        }

        #endregion
    }
}
