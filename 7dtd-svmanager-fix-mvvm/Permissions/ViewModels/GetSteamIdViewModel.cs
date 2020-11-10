using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Permissions.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Permissions.ViewModels
{
    public class GetSteamIdViewModel : ViewModelBase
    {
        public GetSteamIdViewModel(IWindowService windowService, AbstractGetSteamIdModel model) : base(windowService, model)
        {
            this.model = model;

            SteamId = model.ObserveProperty(m => m.Steam64Id).ToReactiveProperty();
            ApplyBtEnabled = model.ObserveProperty(m => m.CanWrite).ToReactiveProperty();

            AnalyzeCommand = new DelegateCommand(Analyze);
            ApplyCommand = new DelegateCommand(Apply);
        }

        private readonly AbstractGetSteamIdModel model;
        private bool analyzeBtEnabled;
        private string urlText;

        public ReactiveProperty<string> SteamId { get; set; }
        public ReactiveProperty<bool> ApplyBtEnabled { get; set; }

        public bool AnalyzeBtEnabled
        {
            get => analyzeBtEnabled;
            set => SetProperty(ref analyzeBtEnabled, value);
        }

        public string UrlText
        {
            get => urlText;
            set
            {
                urlText = value;
                AnalyzeBtEnabled = !string.IsNullOrEmpty(value);
            }
        }

        public ICommand AnalyzeCommand { get; set; }
        public ICommand ApplyCommand { get; set; }

        public void Analyze()
        {
            _ = model.Analyze(UrlText).ContinueWith(task =>
            {
                var exception = task.Exception?.InnerException;
                App.ShowAndWriteException(exception);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void Apply()
        {
            model.IsWritten = true;
            WindowManageService.Close();
        }
    }
}
