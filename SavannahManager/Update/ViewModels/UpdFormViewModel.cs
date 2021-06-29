using _7dtd_svmanager_fix_mvvm.Update.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using _7dtd_svmanager_fix_mvvm.Update.Views;
using CommonStyleLib.Views;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Update.ViewModels
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

            VersionListView = model.ToReactivePropertyAsSynchronized(m => m.VersionList);
            VersionListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.VersionListSelectedIndex);
            UpdateBtIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanUpdate);
            CancelBtIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanCancel);
            RichDetailText = model.ToReactivePropertyAsSynchronized(m => m.RichDetailText);
            DetailText = model.ToReactivePropertyAsSynchronized(m => m.DetailText);
            CurrentVersion = model.ToReactivePropertyAsSynchronized(m => m.CurrentVersion);
            LatestVersion = model.ToReactivePropertyAsSynchronized(m => m.LatestVersion);

            VersionListSelectionChanged = new DelegateCommand<int?>(VersionList_SelectionChanged);
            DoUpdateCommand = new DelegateCommand(Update);
            DoCleanUpdateCommand = new DelegateCommand(CleanUpdate);
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
        #endregion

        #region EventMethods

        protected override void MainWindow_Loaded()
        {
            var task = _model.Initialize();
            task.ContinueWith(t =>
            {
                if (t.Exception == null)
                    return;
                foreach (var exceptionInnerException in t.Exception.InnerExceptions)
                    App.ShowAndWriteException(exceptionInnerException);
            }, TaskContinuationOptions.OnlyOnFaulted);
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
            _ = _model.Update();
        }

        private void CleanUpdate()
        {
            var files = _model.GetCleanFiles();

            var checkFileModel = new CheckCleanFileModel(files);
            var checkViewModel = new CheckCleanFileViewModel(new WindowService(), checkFileModel);
            WindowManageService.ShowDialog<CheckCleanFile>(checkViewModel);
        }
        #endregion
    }
}
