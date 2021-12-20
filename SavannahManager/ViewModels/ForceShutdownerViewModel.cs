using System.Windows;
using System.Collections.ObjectModel;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;
using System.Windows.Input;
using CommonStyleLib.ExMessageBox;
using Prism.Commands;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using _7dtd_svmanager_fix_mvvm.Models.WindowModel;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class ForceShutdownerViewModel : ViewModelBase
    {
        ForceShutdownerModel model;
        public ForceShutdownerViewModel(WindowService windowService, ForceShutdownerModel model) : base(windowService, model)
        {
            this.model = model;

            Loaded = new DelegateCommand(Window_Loaded);
            ForceShutdownCommand = new DelegateCommand(ShutdownBt_Click);
            SelectionChanged = new DelegateCommand<int?>(Selection_Changed);

            ProcessData = new ReadOnlyObservableCollection<ProcessTab>(model.ProcessData);
            ShutdownBtIsEnabled = model.ObserveProperty(m => m.ProcessSelected).ToReactiveProperty();
        }

        #region EventProperties
        public ICommand ForceShutdownCommand { get; }
        public ICommand SelectionChanged { get; }
        #endregion

        #region Properties
        public ReadOnlyObservableCollection<ProcessTab> ProcessData { get; }
        public int ProcessListSelectedIndex { get; set; } = -1;

        public ReactiveProperty<bool> ShutdownBtIsEnabled { get; set; }
        #endregion

        #region Event Methods
        private void Window_Loaded()
        {
            model.Refresh();
        }
        private void ShutdownBt_Click()
        {
            var dialogResult = WindowManageService.MessageBoxShow(LangResources.ForceShutdownerResources.UI_Message,
                LangResources.ForceShutdownerResources.UI_Title, ExMessageBoxBase.MessageType.Asterisk, ExMessageBoxBase.ButtonType.YesNo);
            if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
            {
                model.KillProcess(ProcessListSelectedIndex);
                model.Refresh();
            }
        }

        private void Selection_Changed(int? index)
        {
            var value = index ?? -1;
            model.ProcessSelected = value > -1;
        }
        #endregion
    }
}
