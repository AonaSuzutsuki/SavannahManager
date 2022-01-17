using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer;
using _7dtd_svmanager_fix_mvvm.Views.Update;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer
{

    public class LogViewerViewModel : ViewModelBase
    {
        private readonly LogViewerModel _model;

        public Action<IDisposable> ClosedAction { get; set; }

        public ReadOnlyCollection<string> LogFileList { get; set; }
        public ReactiveProperty<bool> IsWordWrapping { get; set; }
        public ReactiveProperty<bool> ProgressBarVisibility { get; set; }
        public ReactiveProperty<ObservableCollection<RichTextItem>> RichLogDetailItems { get; set; }
        public ReactiveProperty<string> LoadedLineLabel { get; set; }

        public ICommand LogFileListSelectionChangedCommand { get; set; }
        public ICommand TextChangedCommand { get; set; }
        public ICommand ScrollEndedCommand { get; set; }

        public LogViewerViewModel(IWindowService windowService, LogViewerModel model) : base(windowService, model)
        {
            _model = model;

            LogFileList = model.LogFileList.ToReadOnlyReactiveCollection(m => m.Name).AddTo(CompositeDisposable);
            RichLogDetailItems = model.ObserveProperty(m => m.RichLogDetailItems).ToReactiveProperty().AddTo(CompositeDisposable);
            ProgressBarVisibility = new ReactiveProperty<bool>();
            IsWordWrapping = new ReactiveProperty<bool>();
            LoadedLineLabel = new ReactiveProperty<string>();

            LogFileListSelectionChangedCommand = new DelegateCommand<int?>(LogFileListSelectionChanged);
            TextChangedCommand = new DelegateCommand<BindableRichTextBox>(TextChanged);
            ScrollEndedCommand = new DelegateCommand<BindableRichTextBox>(ReachEndLogText);

            model.Load();
        }

        protected override void MainWindowCloseBt_Click()
        {
            base.MainWindowCloseBt_Click();

            ClosedAction?.Invoke(this);
        }

        public void LogFileListSelectionChanged(int? index)
        {
            if (index == null)
                return;

            ProgressBarVisibility.Value = true;
            _ = _model.AnalyzeLogFile(index.Value);
        }

        public void TextChanged(BindableRichTextBox control)
        {
            ProgressBarVisibility.Value = false;
            LoadedLineLabel.Value = $"{control.CurrentLines}/{control.Lines}";
        }

        public void ReachEndLogText(BindableRichTextBox control)
        {
            control.ShowNext();

            LoadedLineLabel.Value = $"{control.CurrentLines}/{control.Lines}";

            Debug.WriteLine("This is the end {0}/{1}", control.CurrentLines, control.Lines);
        }
    }
}
