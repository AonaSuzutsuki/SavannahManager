using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer;
using _7dtd_svmanager_fix_mvvm.Views.Update;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer
{

    public class LogViewerViewModel : ViewModelBase, IDisposable
    {
        private readonly LogViewerModel _model;
        private readonly CompositeDisposable _compositeDisposable = new();

        public Action<IDisposable> ClosedAction { get; set; }

        public ReadOnlyCollection<string> LogFileList { get; set; }
        public ReactiveProperty<ObservableCollection<RichTextItem>> RichLogDetailItems { get; set; }

        public ICommand LogFileListSelectionChangedCommand { get; set; }

        public LogViewerViewModel(IWindowService windowService, LogViewerModel model) : base(windowService, model)
        {
            _model = model;

            LogFileList = model.LogFileList.ToReadOnlyReactiveCollection(m => m.Name).AddTo(_compositeDisposable);
            RichLogDetailItems = model.ObserveProperty(m => m.RichLogDetailItems).ToReactiveProperty();

            LogFileListSelectionChangedCommand = new DelegateCommand<int?>(LogFileListSelectionChanged);

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

            _model.AnalyzeLogFile(index.Value);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
