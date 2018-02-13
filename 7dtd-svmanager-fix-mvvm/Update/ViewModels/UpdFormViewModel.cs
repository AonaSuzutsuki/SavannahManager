using _7dtd_svmanager_fix_mvvm.Update.Models;
using CommonLib.Models;
using CommonLib.ViewModels;
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
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Update.ViewModels
{
    public class UpdFormViewModel : ViewModelBase
    {
        private UpdFormModel model;
        public UpdFormViewModel(Window view, UpdFormModel model) : base(view, model)
        {
            this.model = model;

            VersionListView = model.ToReactivePropertyAsSynchronized(m => m.VersionList);
            VersionListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.VersionListSelectedIndex);
            UpdateBTIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanUpdate);
            CancelBTIsEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanCancel);
            DetailText = model.ToReactivePropertyAsSynchronized(m => m.DetailText);
            CurrentVersion = model.ToReactivePropertyAsSynchronized(m => m.CurrentVersion);
            LatestVersion = model.ToReactivePropertyAsSynchronized(m => m.LatestVersion);

            VersionList_SelectionChanged = new DelegateCommand<int?>(VersionListSelectionChanged);
            UpdateBtClick = new DelegateCommand(UpdateBt_Clicked);
        }

        #region Properties
        public ReactiveProperty<ObservableCollection<string>> VersionListView { get; set; }
        public ReactiveProperty<int> VersionListSelectedIndex { get; set; }

        public ReactiveProperty<bool> UpdateBTIsEnabled { get; set; }
        public ReactiveProperty<bool> CancelBTIsEnabled { get; set; }

        public ReactiveProperty<string> DetailText { get; set; }
        public ReactiveProperty<string> CurrentVersion { get; set; }
        public ReactiveProperty<string> LatestVersion { get; set; }
        #endregion

        #region EventProperties
        public ICommand VersionList_SelectionChanged { get; }
        public ICommand UpdateBtClick { get; }
        #endregion

        #region EventMethods
        private void VersionListSelectionChanged(int? arg)
        {
            if (arg != null)
            {
                int index = arg.Value;
                model.ShowDetails(index);
            }
        }

        private void UpdateBt_Clicked()
        {
            model.Update();
        }
        #endregion
    }
}
