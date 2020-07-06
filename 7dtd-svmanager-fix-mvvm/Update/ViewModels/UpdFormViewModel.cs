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
        private UpdFormModel model;
        public UpdFormViewModel(WindowService windowService, UpdFormModel model, bool isAsync = false) : base(windowService, model)
        {
            this.model = model;

            if (!isAsync)
                DoLoaded();

            VersionListView = model.ToReactivePropertyAsSynchronized(m => m.VersionList);
            VersionListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.VersionListSelectedIndex);
            UpdateBtIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanUpdate);
            CancelBtIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanCancel);
            RichDetailText = model.ToReactivePropertyAsSynchronized(m => m.RichDetailText);
            DetailText = model.ToReactivePropertyAsSynchronized(m => m.DetailText);
            CurrentVersion = model.ToReactivePropertyAsSynchronized(m => m.CurrentVersion);
            LatestVersion = model.ToReactivePropertyAsSynchronized(m => m.LatestVersion);

            VersionListSelectionChanged = new DelegateCommand<int?>(VersionList_SelectionChanged);
            UpdateBtClick = new DelegateCommand(UpdateBt_Clicked);
            DoLoaded();
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
        public ICommand UpdateBtClick { get; }
        #endregion

        #region EventMethods

        protected override void MainWindow_Loaded()
        {
            var loadingModel = new LoadingModel();
            var windowService = new WindowService();
            var vm = new LoadingViewModel(windowService, loadingModel);
            WindowManageService.Show<Loading>(vm);

            var task = model.Initialize();

            windowService.Close();
        }

        private void VersionList_SelectionChanged(int? arg)
        {
            if (arg != null)
            {
                int index = arg.Value;
                var task = model.ShowDetails(index);
            }
        }

        private void UpdateBt_Clicked()
        {
            var task = model.Update();
        }
        #endregion
    }
}
