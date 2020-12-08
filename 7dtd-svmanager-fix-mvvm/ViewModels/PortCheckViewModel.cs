using _7dtd_svmanager_fix_mvvm.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class PortCheckViewModel : ViewModelBase
    {
        public PortCheckViewModel(WindowService windowService, PortCheckModel model) : base(windowService, model)
        {
            this.model = model;

            #region EventInitialize
            AutoSetCommand = new DelegateCommand(AutoSet_Clicked);
            CheckPortCommand = new DelegateCommand(Check_Clicked);
            #endregion

            #region PropertyInitialize
            ExternalIpAddress = model.ToReactivePropertyAsSynchronized(m => m.ExternalIpAddress);
            Port = model.ToReactivePropertyAsSynchronized(m => m.PortText);
            StatusLabel = model.ToReactivePropertyAsSynchronized(m => m.StatusLabel);
            #endregion
        }

        #region Fields
        private PortCheckModel model;
        #endregion

        #region Properties
        public ReactiveProperty<string> ExternalIpAddress { get; set; }
        public ReactiveProperty<string> Port { get; set; }
        public ReactiveProperty<string> StatusLabel { get; set; }
        #endregion

        #region EventProperties
        public ICommand AutoSetCommand { get; set; }
        public ICommand CheckPortCommand { get; set; }
        #endregion

        #region EventMethods
        private void AutoSet_Clicked()
        {
            _ = model.AutoSetIpAddress();
        }
        private void Check_Clicked()
        {
            _ = model.CheckPort();
        }
        #endregion
    }
}
