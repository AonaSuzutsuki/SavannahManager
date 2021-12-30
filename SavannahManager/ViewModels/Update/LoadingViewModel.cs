using _7dtd_svmanager_fix_mvvm.Models.Update;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Update
{
    public class LoadingViewModel : ViewModelBase
    {
        private readonly LoadingModel _model;
        public LoadingViewModel(WindowService windowService, LoadingModel model) : base(windowService, model)
        {
            _model = model;
        }
    }
}
