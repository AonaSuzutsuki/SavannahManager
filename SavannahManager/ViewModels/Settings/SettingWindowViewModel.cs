using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Settings;
using _7dtd_svmanager_fix_mvvm.Models.WindowModel;
using _7dtd_svmanager_fix_mvvm.Views.Settings;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Settings
{
    public class SettingWindowViewModel : ViewModelBase
    {
        private readonly SettingModel _model;
        public SettingWindowViewModel(WindowService windowService, SettingModel model) : base(windowService, model)
        {
            _model = model;

            #region Event Initialize
            GetSvFilePathCommand = new DelegateCommand(GetSvFilePathBt_Click);
            GetConfFilePathCommand = new DelegateCommand(GetConfFilePathBt_Click);
            GetAdminFilePathCommand = new DelegateCommand(GetAdminFilePathBt_Click);
            HelpHyperlinkCommand = new DelegateCommand<string>(OpenHelpHyperlink);

            KeyEditCommand = new DelegateCommand(KeyEditBt_Click);

            GetBackupDirCommand = new DelegateCommand(GetBackupDirBt_Click);
            GetRestoreDirCommand = new DelegateCommand(GetRestoreDirBt_Click);

            SaveBtCommand = new DelegateCommand(SaveBt_Click);
            #endregion

            #region Property Initialize
            ExeFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ExeFilePath);
            ConfigFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ConfigFilePath);
            AdminFilePathText = model.ToReactivePropertyAsSynchronized(m => m.AdminFilePath);

            IsLogGetterChecked = model.ToReactivePropertyAsSynchronized(m => m.IsLogGetter);
            ConsoleLengthText = model.ToReactivePropertyAsSynchronized(m => m.ConsoleLengthText);

            TelnetWaitTime = model.ToReactivePropertyAsSynchronized(m => m.TelnetWaitTime);

            IsBetaModeChecked = model.ToReactivePropertyAsSynchronized(m => m.IsBetaMode);
            IsAutoUpdateChecked = model.ToReactivePropertyAsSynchronized(m => m.IsAutoUpdate);
            IsEncryptPassword = model.ToReactivePropertyAsSynchronized(m => m.IsEncryptPassword);

            BackupDirPath = model.ToReactivePropertyAsSynchronized(m => m.BackupDirPath);
            RestoreDirPath = model.ToReactivePropertyAsSynchronized(m => m.RestoreDirPath);
            #endregion
        }

        #region Properties
        public ReactiveProperty<string> ExeFilePathText { get; set; }
        public ReactiveProperty<string> ConfigFilePathText { get; set; }
        public ReactiveProperty<string> AdminFilePathText { get; set; }

        public ReactiveProperty<bool> IsLogGetterChecked { get; set; }
        public ReactiveProperty<string> ConsoleLengthText { get; set; }

        public ReactiveProperty<int> TelnetWaitTime { get; set; }

        public ReactiveProperty<bool> IsBetaModeChecked { get; set; }
        public ReactiveProperty<bool> IsAutoUpdateChecked { get; set; }
        public ReactiveProperty<bool> IsEncryptPassword { get; set; }

        public ReactiveProperty<string> BackupDirPath { get; set; }
        public ReactiveProperty<string> RestoreDirPath { get; set; }
        #endregion

        #region Event Properties
        public ICommand GetSvFilePathCommand { get; set; }
        public ICommand GetConfFilePathCommand { get; set; }
        public ICommand GetAdminFilePathCommand { get; set; }

        public ICommand HelpHyperlinkCommand { get; set; }

        public ICommand KeyEditCommand { get; set; }

        public ICommand GetBackupDirCommand { get; set; }
        public ICommand GetRestoreDirCommand { get; set; }

        public ICommand SaveBtCommand { get; set; }
        #endregion

        #region Event Methods
        private void GetSvFilePathBt_Click()
        {
            _model.GetServerFilePath();
        }
        private void GetConfFilePathBt_Click()
        {
            _model.GetConfFilePath();
        }
        private void GetAdminFilePathBt_Click()
        {
            _model.GetAdminFilePath();
        }

        private void OpenHelpHyperlink(string text)
        {
            ExMessageBoxBase.Show(text, "Help", ExMessageBoxBase.MessageType.Question);
        }

        private void KeyEditBt_Click()
        {
            var shortcutManager = _model.ShortcutKeyManager;
            var keyConfModel = new KeyConfigModel(shortcutManager);
            var vm = new KeyConfigViewModel(new WindowService(), keyConfModel);
            WindowManageService.ShowDialog<KeyConfig>(vm);
        }

        private void GetBackupDirBt_Click()
        {
            _model.GetBackupDirPath();
        }
        private void GetRestoreDirBt_Click()
        {
            _model.GetRestoreDirPath();
        }

        private void SaveBt_Click()
        {
            if (_model.IsEncryptPassword)
            {
                var inputWidth = 300;
                var inputHeight = 200;
                var item = MainWindowModel.CalculateCenterTop(_model, inputWidth, inputHeight);
                var inputViewModel = new InputWindowViewModel(new WindowService(), new InputWindowModel
                {
                    Width = inputWidth,
                    Height = inputHeight,
                    Top = item.top,
                    Left = item.left
                });
                WindowManageService.ShowDialog<InputWindow>(inputViewModel);
                if (!inputViewModel.IsCancel)
                {
                    _model.EnabledEncryptionData(inputViewModel.InputText.Value);
                }
            }
            _model.Save();

            WindowManageService.Close();
        }
        #endregion
    }
}
