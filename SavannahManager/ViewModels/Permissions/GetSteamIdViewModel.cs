using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Permissions;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Permissions
{
    public class GetSteamIdViewModel : ViewModelBase
    {
        public GetSteamIdViewModel(IWindowService windowService, AbstractGetSteamIdModel model) : base(windowService, model)
        {
            _model = model;

            SteamId = model.ObserveProperty(m => m.Steam64Id).ToReactiveProperty();
            ApplyBtEnabled = model.ObserveProperty(m => m.CanWrite).ToReactiveProperty();

            AnalyzeCommand = new DelegateCommand(Analyze);
            ApplyCommand = new DelegateCommand(Apply);
        }

        private readonly AbstractGetSteamIdModel _model;
        private bool _analyzeBtEnabled;
        private string _urlText;

        public ReactiveProperty<string> SteamId { get; set; }
        public ReactiveProperty<bool> ApplyBtEnabled { get; set; }

        public bool AnalyzeBtEnabled
        {
            get => _analyzeBtEnabled;
            set => SetProperty(ref _analyzeBtEnabled, value);
        }

        public string UrlText
        {
            get => _urlText;
            set
            {
                _urlText = value;
                AnalyzeBtEnabled = !string.IsNullOrEmpty(value);
            }
        }

        public ICommand AnalyzeCommand { get; set; }
        public ICommand ApplyCommand { get; set; }

        public void Analyze()
        {
            _ = _model.Analyze(UrlText).ContinueWith(task =>
            {
                var exceptions = task.Exception?.InnerExceptions;
                if (exceptions == null)
                    return;
                foreach (var exception in exceptions)
                    App.ShowAndWriteException(exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Apply()
        {
            _model.IsWritten = true;
            WindowManageService.Close();
        }
    }
}
