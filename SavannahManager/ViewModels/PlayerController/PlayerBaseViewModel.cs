using _7dtd_svmanager_fix_mvvm.Models.PlayerController;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController
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
