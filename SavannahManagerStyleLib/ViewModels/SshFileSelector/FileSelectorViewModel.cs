using System;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows.Controls;
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
    public class FileSelectorWindowService : WindowService
    {
        public Action PathTextBoxScrollToEnd { get; set; }
    }

    public class FileSelectorViewModel : ViewModelBase
    {

        #region Fields

        private readonly FileSelectorWindowService _fileSelectorWindowService;
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
        
        public ConnectionInformation ConnectionInformation { get; set; }

        public bool IsCancel { get; private set; } = true;

        #endregion

        #region Event Properties

        public ICommand BackPageCommand { get; set; }
        public ICommand ForwardPageCommand { get; set; }
        public ICommand TraceBackPageCommand { get; set; }
        public ICommand PathTextEnterDownCommand { get; set; }

        public ICommand BackupFileListSelectionChangedCommand { get; set; }
        public ICommand BackupFileListMouseDoubleClickCommand { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ICommand RenameCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand MakeDirectoryCommand { get; set; }

        #endregion

        public FileSelectorViewModel(FileSelectorWindowService windowService, FileSelectorModel model) : base(windowService, model)
        {
            windowService.OwnerChanged.Subscribe(window =>
            {
                if (window is not FileSelectorView view)
                    return;

                windowService.PathTextBoxScrollToEnd = view.PathTextBoxScrollToEnd;
            });

            _fileSelectorWindowService = windowService;
            _model = model;

            BackBtIsEnabled = model.ObserveProperty(m => m.CanGoBack).ToReactiveProperty().AddTo(CompositeDisposable);
            ForwardBtIsEnabled = model.ObserveProperty(m => m.CanGoForward).ToReactiveProperty().AddTo(CompositeDisposable);
            PathText = model.ToReactivePropertyAsSynchronized(m => m.CurrentDirectory).AddTo(CompositeDisposable);
            FileList = model.FileList.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable);
            SelectedFileItem = new ReactiveProperty<SftpFileDetailInfo>();
            IsLoading = new ReactiveProperty<bool>();
            FileName = new ReactiveProperty<string>();
            SaveContent = new ReactiveProperty<string>();

            BackPageCommand = new DelegateCommand(BackPage);
            ForwardPageCommand = new DelegateCommand(ForwardPage);
            TraceBackPageCommand = new DelegateCommand(TraceBackPage);
            PathTextEnterDownCommand = new DelegateCommand<string>(async (path) => await PathTextEnterDown(path));
            BackupFileListSelectionChangedCommand = new DelegateCommand<SftpFileDetailInfo>(BackupFileListSelectionChanged);
            BackupFileListMouseDoubleClickCommand = new DelegateCommand<SftpFileDetailInfo>(BackupFileListMouseDoubleClick);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);

            RenameCommand = new DelegateCommand(Rename);
            DeleteCommand = new DelegateCommand(Delete);
            MakeDirectoryCommand = new DelegateCommand(MakeDirectory);

            PathText.PropertyChanged += (_, _) =>
            {
                _fileSelectorWindowService.PathTextBoxScrollToEnd();
            };

            model.ErrorOccurred.Subscribe(e =>
            {
                WindowManageService.MessageBoxShow(e.Message, "Error", ExMessageBoxBase.MessageType.Hand);
                WindowManageService.Close();
            });
            model.CaughtOccurred.Subscribe(e =>
            {
                WindowManageService.MessageBoxShow(e.Message, "Error", ExMessageBoxBase.MessageType.Hand);
            });
        }

        protected override void MainWindow_Loaded()
        {
            base.MainWindow_Loaded();

            var task = OpenConnectionWindow();
            task.ContinueWith(t =>
            {
                var result = t.Result;
                if (!result)
                    WindowManageService.Dispatch(WindowManageService.Close);
                
                IsCancel = false;
            });
            task.ContinueWith(t =>
            {
                var e = t.Exception?.InnerException;
                if (e == null)
                    return;

                WindowManageService.MessageBoxDispatchShow(e.Message, "Error", ExMessageBoxBase.MessageType.Hand);
                WindowManageService.Dispatch(WindowManageService.Close);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        private async Task<bool> OpenConnectionWindow()
        {
            var model = new InputConnectionInfoModel();
            var vm = new InputConnectionInfoViewModel(new WindowService(), model);
            vm.SetConnectionInformation(ConnectionInformation);
            WindowManageService.ShowDialog<InputConnectionInfoView>(vm);

            if (vm.IsCancel)
            {
                return false;
            }

            ConnectionInformation = new ConnectionInformation
            {
                Address = vm.Address.Value,
                Port = vm.Port.Value,
                Username = vm.Username.Value,
                IsPassword = vm.SshPasswordChecked.Value,
                Password = vm.SshPassword.Value,
                KeyPath = vm.SshKeyPath.Value,
                PassPhrase = vm.SshPassPhrase.Value,
                DefaultWorkingDirectory = vm.WorkingDirectory.Value
            };

            IsLoading.Value = true;
            if (ConnectionInformation.IsPassword)
            {
                _model.Open(ConnectionInformation.Address, ConnectionInformation.Port);
                await _model.Connect(ConnectionInformation.Username, ConnectionInformation.Password, ConnectionInformation.DefaultWorkingDirectory);
            }
            else
            {
                _model.Open(ConnectionInformation.Address, ConnectionInformation.Port);
                await _model.Connect(ConnectionInformation.Username, ConnectionInformation.PassPhrase,
                    ConnectionInformation.KeyPath, ConnectionInformation.DefaultWorkingDirectory);
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

        public async Task PathTextEnterDown(string text)
        {
            await _model.ChangeDirectory(text);
        }

        public void BackupFileListSelectionChanged(SftpFileDetailInfo item)
        {
            if (item == null)
                return;

            var path = item.FullPath;
            FileName.Value = Path.GetFileName(path);

            var editModeItems = FileList.Where(x => x.IsEditMode);
            foreach (var info in editModeItems)
            {
                info.IsEditMode = false;
            }
        }

        public void BackupFileListMouseDoubleClick(SftpFileDetailInfo item)
        {
            if (item == null || item.IsEditMode)
                return;

            var path = item.FullPath;
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

        public void Rename()
        {
            var item = SelectedFileItem.Value;
            if (item == null)
                return;

            item.IsEditMode = true;
        }

        public void Delete()
        {
            var item = SelectedFileItem.Value;
            if (item == null)
                return;

            var dialogResult = WindowManageService.MessageBoxShow("Are you sure to delete?", "Delete",
                ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
            if (dialogResult == ExMessageBoxBase.DialogResult.No)
                return;

            _ = _model.Delete(item.FullPath);
        }

        public void MakeDirectory()
        {
            _model.MakeDirectory();
        }

        private void Save()
        {
            var selectedItem = SelectedFileItem.Value;
            if (_model.DirectoryMode == FileDirectoryMode.Directory)
            {
                _model.DoOpenAction(selectedItem == null ? $"{PathText.Value}/{FileName.Value}" : selectedItem.FullPath);
            }
            else
            {
                if (selectedItem == null)
                    return;

                if (selectedItem.IsDirectory)
                {
                    _ = _model.ChangeDirectory(selectedItem.FullPath);
                    return;
                }

                if (_mode == FileSelectorMode.Open)
                {
                    _model.DoOpenAction(selectedItem.FullPath);
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
