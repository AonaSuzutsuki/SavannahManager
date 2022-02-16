using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Settings;
using _7dtd_svmanager_fix_mvvm.Models.WindowModel;
using _7dtd_svmanager_fix_mvvm.Views.Settings;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SavannahManagerStyleLib.ViewModels.Encryption;
using SavannahManagerStyleLib.Views.Encryption;

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

            ResetPasswordCommand = new DelegateCommand(ResetPassword);

            KeyEditCommand = new DelegateCommand(KeyEditBt_Click);

            GetBackupDirCommand = new DelegateCommand(GetBackupDirBt_Click);
            GetRestoreDirCommand = new DelegateCommand(GetRestoreDirBt_Click);

            SaveBtCommand = new DelegateCommand(SaveBt_Click);
            #endregion

            #region Property Initialize
            ExeFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ExeFilePath).AddTo(CompositeDisposable);
            ConfigFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ConfigFilePath).AddTo(CompositeDisposable);
            AdminFilePathText = model.ToReactivePropertyAsSynchronized(m => m.AdminFilePath).AddTo(CompositeDisposable);

            IsLogGetterChecked = model.ToReactivePropertyAsSynchronized(m => m.IsLogGetter).AddTo(CompositeDisposable);
            ConsoleLengthText = model.ToReactivePropertyAsSynchronized(m => m.ConsoleLengthText).AddTo(CompositeDisposable);

            TelnetWaitTime = model.ToReactivePropertyAsSynchronized(m => m.TelnetWaitTime).AddTo(CompositeDisposable);

            IsBetaModeChecked = model.ToReactivePropertyAsSynchronized(m => m.IsBetaMode).AddTo(CompositeDisposable);
            IsAutoUpdateChecked = model.ToReactivePropertyAsSynchronized(m => m.IsAutoUpdate).AddTo(CompositeDisposable);
            IsEncryptPassword = model.ToReactivePropertyAsSynchronized(m => m.IsEncryptPassword).AddTo(CompositeDisposable);

            AutoRestartIntervalTime = model.ToReactivePropertyAsSynchronized(m => m.IntervalTime).AddTo(CompositeDisposable);
            AutoRestartIntervalTimeMode = model.ToReactivePropertyAsSynchronized(m => m.IntervalTimeSelectedIndex).AddTo(CompositeDisposable);
            IsAutoRestartSendMessage = model.ToReactivePropertyAsSynchronized(m => m.IsAutoRestartSendMessage).AddTo(CompositeDisposable);
            AutoRestartSendingMessageStartTime =
                model.ToReactivePropertyAsSynchronized(m => m.AutoRestartSendingMessageStartTime).AddTo(CompositeDisposable);
            AutoRestartSendingMessageStartTimeMode =
                model.ToReactivePropertyAsSynchronized(m => m.AutoRestartSendingMessageStartTimeMode).AddTo(CompositeDisposable);
            AutoRestartSendingMessageIntervalTime =
                model.ToReactivePropertyAsSynchronized(m => m.AutoRestartSendingMessageIntervalTime).AddTo(CompositeDisposable);
            AutoRestartSendingMessageIntervalTimeMode =
                model.ToReactivePropertyAsSynchronized(m => m.AutoRestartSendingMessageIntervalTimeMode).AddTo(CompositeDisposable);
            AutoRestartSendingMessageFormat =
                model.ToReactivePropertyAsSynchronized(m => m.AutoRestartSendingMessageFormat).AddTo(CompositeDisposable);

            BackupDirPath = model.ToReactivePropertyAsSynchronized(m => m.BackupDirPath).AddTo(CompositeDisposable);
            RestoreDirPath = model.ToReactivePropertyAsSynchronized(m => m.RestoreDirPath).AddTo(CompositeDisposable);
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

        public ReactiveProperty<int> AutoRestartIntervalTime { get; set; }
        public ReactiveProperty<int> AutoRestartIntervalTimeMode { get; set; }
        public ReactiveProperty<bool> IsAutoRestartSendMessage { get; set; }
        public ReactiveProperty<int> AutoRestartSendingMessageStartTime { get; set; }
        public ReactiveProperty<int> AutoRestartSendingMessageStartTimeMode { get; set; }
        public ReactiveProperty<int> AutoRestartSendingMessageIntervalTime { get; set; }
        public ReactiveProperty<int> AutoRestartSendingMessageIntervalTimeMode { get; set; }
        public ReactiveProperty<string> AutoRestartSendingMessageFormat { get; set; }

        public ReactiveProperty<string> BackupDirPath { get; set; }
        public ReactiveProperty<string> RestoreDirPath { get; set; }
        #endregion

        #region Event Properties
        public ICommand GetSvFilePathCommand { get; set; }
        public ICommand GetConfFilePathCommand { get; set; }
        public ICommand GetAdminFilePathCommand { get; set; }

        public ICommand HelpHyperlinkCommand { get; set; }

        public ICommand ResetPasswordCommand { get; set; }

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

        private void ResetPassword()
        {
            if (_model.IsEncryptPassword)
            {
                const int inputWidth = InputWindowViewModel.DefaultWidth;
                const int inputHeight = InputWindowViewModel.DefaultHeight;
                var (left, top) = MainWindowModel.CalculateCenterTop(_model, inputWidth, inputHeight);
                var inputViewModel = new InputWindowViewModel(new WindowService(), new ModelBase
                {
                    Width = inputWidth,
                    Height = inputHeight,
                    Top = top,
                    Left = left
                })
                {
                    Title =
                    {
                        Value = "Password Dialog"
                    },
                    Message =
                    {
                        Value = "Input password for encryption."
                    }
                };
                WindowManageService.ShowDialog<InputWindow>(inputViewModel);
                if (!inputViewModel.IsCancel)
                {
                    _model.EnabledEncryptionData(inputViewModel.InputText.Value, inputViewModel.InputSaltText.Value);
                }
            }
        }
        private void SaveBt_Click()
        {
            if (_model.IsRequirePassword)
                ResetPassword();
            _model.Save();

            WindowManageService.Close();
        }
        #endregion
    }
}
