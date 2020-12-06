using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class TelnetWaitTimeCalculatorViewModel : ViewModelBase
    {
        public TelnetWaitTimeCalculatorViewModel(IWindowService windowService, TelnetWaitTimeCalculatorModel model) : base(windowService, model)
        {
            this._model = model;

            WaitTime = model.ObserveProperty(m => m.WaitTimeText).ToReactiveProperty();
            RecommendedWaitTime = model.ObserveProperty(m => m.RecommendedWaitTimeText).ToReactiveProperty();

            CalculateWaitTimeCommand = new DelegateCommand(CalculateWaitTime);
            SetToSettingsCommand = new DelegateCommand<string>(SetToSettings);
        }

        private readonly TelnetWaitTimeCalculatorModel _model;

        public ReactiveProperty<string> WaitTime { get; set; }
        public ReactiveProperty<string> RecommendedWaitTime { get; set; }

        public ICommand CalculateWaitTimeCommand { get; set; }
        public ICommand SetToSettingsCommand { get; set; }

        public void CalculateWaitTime()
        {
            _model.CalculateWaitTime();
        }

        public void SetToSettings(string text)
        {
            _model.SetToSettings(text);
        }
    }
}
