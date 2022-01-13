using System;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController
{
    public class PlayerBaseWindowService : WindowService
    {
        public IDisposable Page { get; }

        public PlayerBaseWindowService(IDisposable page)
        {
            Page = page;
        }
    }

    public class PlayerBaseViewModel : ViewModelBase
    {
        public PlayerBaseViewModel(PlayerBaseWindowService windowService, PlayerBaseModel model) : base(windowService, model)
        {
            _playerBaseWindowService = windowService;
            _model = model;
        }

        private readonly PlayerBaseWindowService _playerBaseWindowService;
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

        protected override void MainWindow_Closing()
        {
            base.MainWindow_Closing();

            _playerBaseWindowService.Page?.Dispose();
        }
    }
}
