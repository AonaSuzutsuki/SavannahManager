using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Update;
using _7dtd_svmanager_fix_mvvm.Views.Update;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Update
{
    public class UpdFormViewModel : ViewModelBase
    {
        private readonly UpdFormModel _model;
        public UpdFormViewModel(WindowService windowService, UpdFormModel model, bool isAsync = false) : base(windowService, model)
        {
            _model = model;

            if (isAsync)
            {
                Loaded = new DelegateCommand(() => { });
            }

            VersionListView = model.ToReactivePropertyAsSynchronized(m => m.VersionList).AddTo(CompositeDisposable);
            VersionListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.VersionListSelectedIndex).AddTo(CompositeDisposable);
            UpdateBtIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanUpdate).AddTo(CompositeDisposable);
            CancelBtIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanCancel).AddTo(CompositeDisposable);
            RichDetailText = model.ToReactivePropertyAsSynchronized(m => m.RichDetailText).AddTo(CompositeDisposable);
            DetailText = model.ToReactivePropertyAsSynchronized(m => m.DetailText).AddTo(CompositeDisposable);
            CurrentVersion = model.ToReactivePropertyAsSynchronized(m => m.CurrentVersion).AddTo(CompositeDisposable);
            LatestVersion = model.ToReactivePropertyAsSynchronized(m => m.LatestVersion).AddTo(CompositeDisposable);

            VersionListSelectionChanged = new DelegateCommand<int?>(VersionList_SelectionChanged);
            DoUpdateCommand = new DelegateCommand(Update);
            DoCleanUpdateCommand = new DelegateCommand(CleanUpdate);
            OpenLinkCommand = new DelegateCommand<string>(OpenLink);
        }

        #region Properties
        public ReactiveProperty<ObservableCollection<string>> VersionListView { get; set; }
        public ReactiveProperty<int> VersionListSelectedIndex { get; set; }

        public ReactiveProperty<bool> UpdateBtIsEnabled { get; set; }
        public ReactiveProperty<bool> CancelBtIsEnabled { get; set; }

        public ReactiveProperty<ObservableCollection<RichTextItem>> RichDetailText { get; set; }
        public ReactiveProperty<string> DetailText { get; set; }
        public ReactiveProperty<string> CurrentVersion { get; set; }
        public ReactiveProperty<string> LatestVersion { get; set; }
        #endregion

        #region EventProperties
        public ICommand VersionListSelectionChanged { get; }
        public ICommand DoCleanUpdateCommand { get; }
        public ICommand DoUpdateCommand { get; }

        public ICommand OpenLinkCommand { get; }
        #endregion

        #region EventMethods

        protected override void MainWindow_Loaded()
        {
            var vm = new LoadingViewModel(new WindowService(), new LoadingModel());
            WindowManageService.Show<Loading>(vm);

            var task = _model.Initialize();
            task.ContinueWith(t =>
            {
                if (t.Exception == null)
                    return;
                foreach (var exceptionInnerException in t.Exception.InnerExceptions)
                    App.ShowAndWriteException(exceptionInnerException);
            }, TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t =>
            {
                WindowManageService.Dispatch(() => vm.MainWindowCloseBtClick.Execute(null));
            });
        }

        private void VersionList_SelectionChanged(int? arg)
        {
            if (arg != null)
            {
                var index = arg.Value;
                _model.ShowDetails(index);
            }
        }

        private void Update()
        {
            _ = CheckUpdate();
        }

        private void CleanUpdate()
        {
            var files = _model.GetCleanFiles().Where(x => !x.Contains("Updater\\"));

            var checkFileModel = new CheckCleanFileModel(files);
            var checkViewModel = new CheckCleanFileViewModel(new WindowService(), checkFileModel);
            WindowManageService.ShowDialog<CheckCleanFile>(checkViewModel);

            if (checkFileModel.CanCleanUpdate)
            {
                var targets = checkFileModel.GetTargetFiles();
                var enumerable = targets.ToList();
                _ = CheckCleanUpdate(enumerable);
            }
        }

        private async Task CheckUpdate()
        {
            var (notice, isConfirm) = await _model.CheckAlert();
            if (string.IsNullOrEmpty(notice))
            {
                await _model.Update();
                return;
            }

            if (isConfirm)
            {
                var dr = WindowManageService.MessageBoxShow(notice, "Notice", ExMessageBoxBase.MessageType.Exclamation,
                    ExMessageBoxBase.ButtonType.YesNo);
                if (dr == ExMessageBoxBase.DialogResult.Yes)
                    await _model.Update();
            }
            else
            {
                WindowManageService.MessageBoxShow(notice, "Notice", ExMessageBoxBase.MessageType.Exclamation);
                await _model.Update();
            }
        }

        private async Task CheckCleanUpdate(IReadOnlyCollection<string> targets)
        {
            var (notice, isConfirm) = await _model.CheckAlert();
            if (string.IsNullOrEmpty(notice))
            {
                await (targets.Any() ? _model.CleanUpdate(targets) : _model.Update());
                return;
            }

            if (isConfirm)
            {
                var dr = WindowManageService.MessageBoxShow(notice, "Notice", ExMessageBoxBase.MessageType.Exclamation,
                    ExMessageBoxBase.ButtonType.YesNo);
                if (dr == ExMessageBoxBase.DialogResult.Yes)
                    await (targets.Any() ? _model.CleanUpdate(targets) : _model.Update());
            }
            else
            {
                WindowManageService.MessageBoxShow(notice, "Notice", ExMessageBoxBase.MessageType.Exclamation);
                await (targets.Any() ? _model.CleanUpdate(targets) : _model.Update());
            }
        }

        public void OpenLink(string url)
        {
            var dialogResult = ExMessageBoxBase.Show("Are you sure open it with default browser?", "Open Browser", ExMessageBoxBase.MessageType.Question,
                ExMessageBoxBase.ButtonType.YesNo);
            if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = url
                });
        }
        #endregion
    }
}
