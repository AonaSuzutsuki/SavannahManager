using CommonLib.Models;
using CommonLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Updater.Models;
using Reactive.Bindings.Extensions;

namespace Updater.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        MainWindowModel model;
        public MainWindowViewModel(Window view, MainWindowModel model) : base(view, model)
        {
            this.model = model;
            
            StatusLabel = model.ToReactivePropertyAsSynchronized(m => m.StatusLabel);
            ProgressValue = model.ToReactivePropertyAsSynchronized(m => m.ProgressValue);
            ProgressMaximum = model.ToReactivePropertyAsSynchronized(m => m.ProgressMaximum);
            ProgressIndeterminate = model.ToReactivePropertyAsSynchronized(m => m.ProgressIndeterminate);
            ProgressLabel = model.ToReactivePropertyAsSynchronized(m => m.ProgressLabel);
            CloseBtEnabled = model.ToReactivePropertyAsSynchronized(m => m.CanClose);

            Loaded = new DelegateCommand(MainWindow_Loaded);
            Closed = new DelegateCommand(MainWindow_Closed);

            if (Loaded != null && Loaded.CanExecute(null))
            {
                Loaded?.Execute(null);
            }
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
        public ICommand Loaded { get; set; }
        public ICommand Closed { get; set; }
        #endregion

        #region EventMethods
        public void MainWindow_Loaded()
        {
            model.Update();
        }

        public void MainWindow_Closed()
        {
            model.Close();
        }
        #endregion
    }
}
