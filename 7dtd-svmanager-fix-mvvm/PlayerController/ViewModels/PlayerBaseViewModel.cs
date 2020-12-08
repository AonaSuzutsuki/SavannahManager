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
        public PlayerBaseViewModel(WindowService windowService, PlayerBaseModel model) : base(windowService, model)
        {
            _model = model;
        }

        private readonly PlayerBaseModel _model;
        private string _windowTitle;

        #region EventProperties
        #endregion

        #region Properties
        public string WindowTitle
        {
            get => _windowTitle;
            set => SetProperty(ref _windowTitle, value);
        }
        #endregion
    }
}
