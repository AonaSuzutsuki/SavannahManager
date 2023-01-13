using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.AutoRestart;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.AutoRestart
{
    public class ProcessSelectorViewModel : ViewModelBase
    {
        #region Fields

        private readonly ProcessSelectorModel _model;

        #endregion

        #region Properties

        public ReactiveProperty<string> Filter { get; set; }

        public ReadOnlyReactiveCollection<ProcessSelectorInfo> ProcessItems { get; set; }

        public ReactiveProperty<ProcessSelectorInfo> ProcessSelectedItem { get; set; }

        #endregion

        #region Event Properties
        
        public ICommand OkayCommand { get; set; }

        #endregion

        public ProcessSelectorViewModel(IWindowService windowService, ProcessSelectorModel model) : base(windowService, model)
        {
            _model = model;

            Filter = new ReactiveProperty<string>("7DaysToDie");
            Filter.PropertyChanged += (sender, args) =>
            {
                model.FilterProcesses(Filter.Value);
            };

            ProcessItems = model.FilteredProcesses.ToReadOnlyReactiveCollection(x => new ProcessSelectorInfo(x));
            ProcessSelectedItem = new ReactiveProperty<ProcessSelectorInfo>();
            
            OkayCommand = new DelegateCommand(Okay);

            model.Load(Filter.Value);
        }

        public void Okay()
        {
            var item = ProcessSelectedItem.Value;
            if (item == null)
                return;

            _model.SelectProcess(item.ProcessId);
            WindowManageService.Close();
        }
    }
}
