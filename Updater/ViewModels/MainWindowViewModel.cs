using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommonStyleLib.Views;
using Updater.Models;
using Reactive.Bindings.Extensions;

namespace Updater.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        readonly MainWindowModel _model;
        public MainWindowViewModel(WindowService windowService, MainWindowModel model) : base(windowService, model)
        {
            this._model = model;
            
            StatusLabel = model.ToReactivePropertyAsSynchronized(m => m.StatusLabel);
            ProgressValue = model.ToReactivePropertyAsSynchronized(m => m.ProgressValue);
            ProgressMaximum = model.ToReactivePropertyAsSynchronized(m => m.ProgressMaximum);
            ProgressIndeterminate = model.ToReactivePropertyAsSynchronized(m => m.ProgressIndeterminate);
            ProgressLabel = model.ToReactivePropertyAsSynchronized(m => m.ProgressLabel);
            CloseBtEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanClose);

            Loaded = new DelegateCommand(MainWindow_Loaded);
            Closed = new DelegateCommand(MainWindow_Closed);

            DoLoaded();
        }

        #region Properties
        public ReactiveProperty<string> StatusLabel { get; set; }
        public ReactiveProperty<double> ProgressValue { get; set; }
        public ReactiveProperty<double> ProgressMaximum { get; set; }
        public ReactiveProperty<bool> ProgressIndeterminate { get; set; }
        public ReactiveProperty<string> ProgressLabel { get; set; }

        public ReactiveProperty<bool> CloseBtEnabled { get; set; }
        #endregion

        #region EventProperties
        public ICommand Closed { get; set; }
        #endregion

        #region EventMethods
        protected override void MainWindow_Loaded()
        {
            _ = _model.Update().ContinueWith(t =>
            {
                if (t.Exception == null)
                    return;
                foreach (var exceptionInnerException in t.Exception.InnerExceptions)
                {
                    App.ShowAndWriteException(exceptionInnerException);
                    _model.WriteLog($"Throw {exceptionInnerException.Message}");
                    _model.WriteLog($"Exit {_model.ExitCode}");
                }
                _model.Dispose();
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void MainWindow_Closed()
        {
            _model.Close();
        }
        #endregion
    }
}
