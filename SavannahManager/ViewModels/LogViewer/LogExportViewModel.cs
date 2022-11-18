using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer;
using CommonCoreLib;
using CommonStyleLib.File;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using CommonStyleLib.Views.Behaviors.ListBoxBehavior;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer
{
    public class LogExportViewModel : ViewModelBase
    {
        private readonly LogExportModel _model;

        public ReactiveProperty<ObservableCollection<MoveableListBoxItem>> ColumnItems { get; set; }
        public ReactiveProperty<string> PreviewText { get; set; }

        public ICommand DropCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public LogExportViewModel(IWindowService windowService, LogExportModel model) : base(windowService, model)
        {
            ColumnItems = model.ObserveProperty(m => m.ColumnItems).ToReactiveProperty().AddTo(CompositeDisposable);
            PreviewText = model.ObserveProperty(m => m.PreviewText).ToReactiveProperty().AddTo(CompositeDisposable);

            DropCommand = new DelegateCommand<DropArguments>(DropItem);
            SaveCommand = new DelegateCommand(Save);

            _model = model;
        }

        public void DropItem(DropArguments arguments)
        {
            _model.DisplayPreview();
        }

        public void Save()
        {
            // 2021-12-20- 14-12-37.log
            var fileName = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            var prefix = _model.Mode;
            var saveFile =
                FileSelector.GetFilePath(AppInfo.GetAppPath(), "TSV File (*.tsv)|*.tsv|All File (*.*)|*.*", $"{prefix}-{fileName}.tsv", FileSelector.FileSelectorType.Write);

            _model.SaveFile(saveFile);
        }
    }
}
