using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using _7dtd_svmanager_fix_mvvm.Models;
using System.Collections.ObjectModel;
using Reactive.Bindings.Extensions;
using Reactive.Bindings;
using System.Windows.Input;
using Prism.Commands;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class ForceShutdownerViewModel : ViewModelBase
    {
        ForceShutdownerModel model;
        public ForceShutdownerViewModel(Window view, ForceShutdownerModel model) : base(view, model)
        {
            this.model = model;
            base.view = view;

            Loaded = new DelegateCommand(Window_Loaded);
            ShutdownBtClick = new DelegateCommand(ShutdownBt_Click);

            ProcessData = new ReadOnlyObservableCollection<ProcessTab>(model.ProcessData);
            ProcessListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.CurrentIndex);
        }

        #region EventProperties
        public ICommand Loaded { get; }
        public ICommand ShutdownBtClick { get; }
        #endregion

        #region Properties
        public ReadOnlyObservableCollection<ProcessTab> ProcessData { get; }
        public ReactiveProperty<int> ProcessListSelectedIndex { get; set; }
        #endregion

        #region Event Methods
        private void Window_Loaded()
        {
            model.Refresh();
        }
        private void ShutdownBt_Click()
        {
            model.KillProcess();
        }
        #endregion
    }
}
