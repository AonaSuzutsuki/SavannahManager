using _7dtd_svmanager_fix_mvvm.PlayerController.Models;
using CommonStyleLib.ViewModels;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels
{
    public class PlayerBaseViewModel : ViewModelBase
    {
        private PlayerBaseModel model;
        public PlayerBaseViewModel(WindowService windowService, PlayerBaseModel model) : base(windowService, model)
        {
            this.model = model;
        }


        #region EventProperties
        #endregion

        #region Properties
        private string windowTitle = default;
        public string WindowTitle
        {
            get => windowTitle;
            set => SetProperty(ref windowTitle, value);
        }
        #endregion
    }
}
