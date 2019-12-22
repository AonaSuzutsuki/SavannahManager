using _7dtd_svmanager_fix_mvvm.Settings.Models;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Windows;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Settings.Views;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.Settings.ViewModels
{
    public class SettingWindowViewModel : ViewModelBase
    {
        SettingModel model;
        public SettingWindowViewModel(WindowService windowService, SettingModel model) : base(windowService, model)
        {
            this.model = model;

            #region Event Initialize
            GetSvFilePathBtClick = new DelegateCommand(GetSvFilePathBt_Click);
            GetConfFilePathBtClick = new DelegateCommand(GetConfFilePathBt_Click);
            GetAdminFilePathBtClick = new DelegateCommand(GetAdminFilePathBt_Click);

            KeyEditBtClick = new DelegateCommand(KeyEditBt_Click);

            GetBackupDirBtClick = new DelegateCommand(GetBackupDirBt_Click);

            SaveBtClick = new DelegateCommand(SaveBt_Click);
            #endregion

            #region Property Initialize
            ExeFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ExeFilePath);
            ConfigFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ConfigFilePath);
            AdminFilePathText = model.ToReactivePropertyAsSynchronized(m => m.AdminFilePath);

            IsLogGetterChecked = model.ToReactivePropertyAsSynchronized(m => m.IsLogGetter);
            ConsoleLengthText = model.ToReactivePropertyAsSynchronized(m => m.ConsoleLengthText);

            IsBetaModeChecked = model.ToReactivePropertyAsSynchronized(m => m.IsBetaMode);
            IsAutoUpdateChecked = model.ToReactivePropertyAsSynchronized(m => m.IsAutoUpdate);
            BackupDirPath = model.ToReactivePropertyAsSynchronized(m => m.BackupDirPath);
            #endregion
        }

        #region Properties
        public ReactiveProperty<string> ExeFilePathText { get; set; }
        public ReactiveProperty<string> ConfigFilePathText { get; set; }
        public ReactiveProperty<string> AdminFilePathText { get; set; }

        public ReactiveProperty<bool> IsLogGetterChecked { get; set; }
        public ReactiveProperty<string> ConsoleLengthText { get; set; }

        public ReactiveProperty<bool> IsBetaModeChecked { get; set; }
        public ReactiveProperty<bool> IsAutoUpdateChecked { get; set; }
        public ReactiveProperty<string> BackupDirPath { get; set; }
        #endregion

        #region Event Properties
        public ICommand GetSvFilePathBtClick { get; set; }
        public ICommand GetConfFilePathBtClick { get; set; }
        public ICommand GetAdminFilePathBtClick { get; set; }

        public ICommand KeyEditBtClick { get; set; }

        public ICommand GetBackupDirBtClick { get; set; }

        public ICommand SaveBtClick { get; set; }
        #endregion

        #region Event Methods
        private void GetSvFilePathBt_Click()
        {
            model.GetServerFilePath();
        }
        private void GetConfFilePathBt_Click()
        {
            model.GetConfFilePath();
        }
        private void GetAdminFilePathBt_Click()
        {
            model.GetAdminFilePath();
        }

        private void KeyEditBt_Click()
        {
            var shortcutManager = model.ShortcutKeyManager;
            var keyConfModel = new KeyConfigModel(shortcutManager);
            var vm = new KeyConfigViewModel(new WindowService(), keyConfModel);
            WindowManageService.ShowDialog<KeyConfig>(vm);
        }

        private void GetBackupDirBt_Click()
        {
            model.GetBackupDirPath();
        }

        private void SaveBt_Click()
        {
            model.Save();
            WindowManageService.Close();
        }
        #endregion
    }
}
