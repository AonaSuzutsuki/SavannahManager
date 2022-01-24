using System;
using System.Reactive.Subjects;
using System.Windows.Input;
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
        Save,
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

        public FileSelectorMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                SaveContent.Value = value switch
                {
                    FileSelectorMode.Open => "Open",
                    FileSelectorMode.Save => "Save",
                    FileSelectorMode.SaveAs => "SaveAs",
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

        public ICommand BackupFileListMouseDoubleClickCommand { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion

        #region Properties

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
            SaveContent = new ReactiveProperty<string>();

            BackPageCommand = new DelegateCommand(BackPage);
            ForwardPageCommand = new DelegateCommand(ForwardPage);
            TraceBackPageCommand = new DelegateCommand(TraceBackPage);
            BackupFileListMouseDoubleClickCommand = new DelegateCommand<SftpFileDetailInfo>(BackupFileListMouseDoubleClick);
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }


        public void OpenConnectionWindow()
        {
            var model = new InputConnectionInfoModel();
            var vm = new InputConnectionInfoViewModel(new WindowService(), model);
            WindowManageService.ShowDialog<InputConnectionInfoView>(vm);

            if (vm.IsCancel)
                return;

            if (vm.SshPasswordChecked.Value)
            {
                _model.Open(vm.Address.Value, vm.Port.Value);
                _model.Connect(vm.Username.Value, vm.SshPassword.Value);
            }
            else
            {
                _model.Open(vm.Address.Value, vm.Port.Value);
                _model.Connect(vm.Username.Value, vm.SshPassPhrase.Value, vm.SshKeyPath.Value);
            }
        }

        public void BackPage()
        {
            _model.DirectoryBack();
        }

        public void ForwardPage()
        {
            _model.DirectoryForward();
        }

        public void TraceBackPage()
        {
            _model.TraceBackPage();
        }

        public void BackupFileListMouseDoubleClick(SftpFileDetailInfo item)
        {
            if (item == null)
                return;

            var path = item.Path;
            if (item.IsDirectory)
            {
                _model.NewDirectory();
                _model.ChangeDirectory(path);
            }
            else
            {
                _fileDoubleClickedSubject.OnNext(path);
            }
        }

        private void Save()
        {
            if (_mode == FileSelectorMode.Open)
            {

            }
            else if (_mode == FileSelectorMode.Save)
            {

            }
            else
            {

            }
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
