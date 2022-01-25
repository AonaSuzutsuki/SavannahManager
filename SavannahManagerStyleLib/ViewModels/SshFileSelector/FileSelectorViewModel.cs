using System;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SavannahManagerStyleLib.Models.SshFileSelector;
using SavannahManagerStyleLib.Views.SshFileSelector;

namespace SavannahManagerStyleLib.ViewModels.SshFileSelector
{
    public enum FileSelectorMode
    {
        Open,
        SaveAs
    }

    public class FileSelectorViewModel : ViewModelBase
    {

        #region Fields

        private readonly FileSelectorModel _model;
        private FileSelectorMode _mode = FileSelectorMode.Open;

        #endregion

        #region Properties

        public ReactiveProperty<bool> BackBtIsEnabled { get; set; }
        public ReactiveProperty<bool> ForwardBtIsEnabled { get; set; }

        public ReactiveProperty<string> PathText { get; set; }
        public ReadOnlyReactiveCollection<SftpFileDetailInfo> FileList { get; set; }
        public ReactiveProperty<SftpFileDetailInfo> SelectedFileItem { get; set; }

        public ReactiveProperty<bool> IsLoading { get; set; }

        public ReactiveProperty<string> FileName { get; set; }

        public FileSelectorMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                SaveContent.Value = value switch
                {
                    FileSelectorMode.Open => "Open",
                    FileSelectorMode.SaveAs => "Save As",
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                };
            }
        }
        public ReactiveProperty<string> SaveContent { get; set; }

        #endregion

        #region Event Properties

        public ICommand BackPageCommand { get; set; }
        public ICommand ForwardPageCommand { get; set; }
        public ICommand TraceBackPageCommand { get; set; }

        public ICommand BackupFileListSelectionChangedCommand { get; set; }
        public ICommand BackupFileListMouseDoubleClickCommand { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Properties

        public bool IsNewConnection { get; set; }

        private readonly Subject<string> _fileDoubleClickedSubject = new();
        public IObservable<string> FileDoubleClicked => _fileDoubleClickedSubject;

        #endregion

        public FileSelectorViewModel(IWindowService windowService, FileSelectorModel model) : base(windowService, model)
        {
            _model = model;

            BackBtIsEnabled = model.ObserveProperty(m => m.CanGoBack).ToReactiveProperty().AddTo(CompositeDisposable);
            ForwardBtIsEnabled = model.ObserveProperty(m => m.CanGoForward).ToReactiveProperty().AddTo(CompositeDisposable);
            PathText = model.ToReactivePropertyAsSynchronized(m => m.CurrentDirectory).AddTo(CompositeDisposable); ;
            FileList = model.FileList.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable);
            SelectedFileItem = new ReactiveProperty<SftpFileDetailInfo>();
            IsLoading = new ReactiveProperty<bool>();
            FileName = new ReactiveProperty<string>();
            SaveContent = new ReactiveProperty<string>();

            BackPageCommand = new DelegateCommand(BackPage);
            ForwardPageCommand = new DelegateCommand(ForwardPage);
            TraceBackPageCommand = new DelegateCommand(TraceBackPage);
            BackupFileListSelectionChangedCommand = new DelegateCommand<SftpFileDetailInfo>(BackupFileListSelectionChanged);
            BackupFileListMouseDoubleClickCommand = new DelegateCommand<SftpFileDetailInfo>(BackupFileListMouseDoubleClick);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        protected override void MainWindow_Loaded()
        {
            base.MainWindow_Loaded();

            if (IsNewConnection)
            {
                OpenConnectionWindow().ContinueWith(t =>
                {
                    var result = t.Result;
                    if (!result)
                        WindowManageService.Dispatch(WindowManageService.Close);
                });
            }
        }

        private async Task<bool> OpenConnectionWindow()
        {
            var model = new InputConnectionInfoModel();
            var vm = new InputConnectionInfoViewModel(new WindowService(), model);
            WindowManageService.ShowDialog<InputConnectionInfoView>(vm);

            if (vm.IsCancel)
            {
                return false;
            }

            var item = new
            {
                Address = vm.Address.Value,
                Port = vm.Port.Value,
                Username = vm.Username.Value,
                IsPassword = vm.SshPasswordChecked.Value,
                Password = vm.SshPassword.Value,
                KeyPath = vm.SshKeyPath.Value,
                PassPhrase = vm.SshPassPhrase.Value
            };

            IsLoading.Value = true;
            if (item.IsPassword)
            {
                _model.Open(item.Address, item.Port);
                await _model.Connect(item.Username, item.Password);
            }
            else
            {
                _model.Open(item.Address, item.Port);
                await _model.Connect(item.Username, item.PassPhrase, item.KeyPath);
            }

            IsLoading.Value = false;
            return true;
        }

        public void BackPage()
        {
            _ = _model.DirectoryBack();
        }

        public void ForwardPage()
        {
            _ = _model.DirectoryForward();
        }

        public void TraceBackPage()
        {
            _ = _model.TraceBackPage();
        }

        public void BackupFileListSelectionChanged(SftpFileDetailInfo item)
        {
            if (item == null)
                return;

            var path = item.Path;
            FileName.Value = Path.GetFileName(path);
        }

        public void BackupFileListMouseDoubleClick(SftpFileDetailInfo item)
        {
            if (item == null)
                return;

            var path = item.Path;
            if (item.IsDirectory)
            {
                _model.NewDirectory();
                _ = _model.ChangeDirectory(path);
            }
            else
            {
                Save();
            }
        }

        private void Save()
        {
            if (_mode == FileSelectorMode.Open)
            {
                var selectedItem = SelectedFileItem.Value;

                if (selectedItem == null || selectedItem.IsDirectory)
                    return;

                _model.DoOpenAction(selectedItem.Path);
            }
            else
            {
                if (FileList.Any(x => x.Name == FileName.Value))
                {
                    var dialogResult = WindowManageService.MessageBoxShow("File exists. Are you sure to overwrite?", "File exists.",
                        ExMessageBoxBase.MessageType.Question, ExMessageBoxBase.ButtonType.YesNo);
                    if (dialogResult == ExMessageBoxBase.DialogResult.No)
                        return;
                }

                var fullPath = _model.GetFullPath(FileName.Value);
                _model.DoSaveAction(fullPath);
            }

            WindowManageService.Close();
        }

        public void Cancel()
        {
            WindowManageService.Close();
        }

        public override void Dispose()
        {
            base.Dispose();

            _model.Dispose();
        }
    }
}
