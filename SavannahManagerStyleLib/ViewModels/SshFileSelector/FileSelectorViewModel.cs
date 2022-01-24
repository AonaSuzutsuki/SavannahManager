using System.Windows.Input;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SavannahManagerStyleLib.Models.SshFileSelector;

namespace SavannahManagerStyleLib.ViewModels.SshFileSelector
{
    public class FileSelectorViewModel : ViewModelBase
    {

        #region Fields

        private readonly FileSelectorModel _model;

        #endregion

        #region Properties

        public ReactiveProperty<bool> BackBtIsEnabled { get; set; }
        public ReactiveProperty<bool> ForwardBtIsEnabled { get; set; }

        public ReactiveProperty<string> PathText { get; set; }
        public ReadOnlyReactiveCollection<SftpFileDetailInfo> FileList { get; set; }

        #endregion

        #region Event Properties

        public ICommand BackPageCommand { get; set; }
        public ICommand ForwardPageCommand { get; set; }
        public ICommand TraceBackPageCommand { get; set; }

        public ICommand BackupFileListMouseDoubleClickCommand { get; set; }

        #endregion

        public FileSelectorViewModel(IWindowService windowService, FileSelectorModel model) : base(windowService, model)
        {
            _model = model;

            BackBtIsEnabled = model.ObserveProperty(m => m.CanGoBack).ToReactiveProperty().AddTo(CompositeDisposable);
            ForwardBtIsEnabled = model.ObserveProperty(m => m.CanGoForward).ToReactiveProperty().AddTo(CompositeDisposable);
            PathText = model.ToReactivePropertyAsSynchronized(m => m.CurrentDirectory).AddTo(CompositeDisposable); ;
            FileList = model.FileList.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable); ;

            BackPageCommand = new DelegateCommand(BackPage);
            ForwardPageCommand = new DelegateCommand(ForwardPage);
            TraceBackPageCommand = new DelegateCommand(TraceBackPage);
            BackupFileListMouseDoubleClickCommand = new DelegateCommand<SftpFileDetailInfo>(BackupFileListMouseDoubleClick);

            model.Open();
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
        }

        public override void Dispose()
        {
            base.Dispose();

            _model.Dispose();
        }
    }
}
