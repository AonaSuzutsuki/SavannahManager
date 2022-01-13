using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController.Pages;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController.Pages
{
    public class KickViewModel : PageViewModelBase
    {
        public KickViewModel(KickModel model)
        {
            _model = model;

            NameText = model.ToReactivePropertyAsSynchronized(m => m.Name).AddTo(CompositeDisposable);

            KickPlayerCommanded = new DelegateCommand(KickBt_Clicked);
            CancelCommand = new DelegateCommand(model.Cancel);
        }

        private readonly KickModel _model;
        private string _reasonText;

        public ReactiveProperty<string> NameText { get; set; }

        public string ReasonText
        {
            get => _reasonText;
            set => SetProperty(ref _reasonText, value);
        }

        #region EventProperties
        public ICommand KickPlayerCommanded { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region EventMethods
        private void KickBt_Clicked()
        {
            _model.Kick(ReasonText);
        }
        #endregion
    }
}
