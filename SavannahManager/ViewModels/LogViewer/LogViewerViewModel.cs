using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer;
using _7dtd_svmanager_fix_mvvm.Views.LogViewer;
using _7dtd_svmanager_fix_mvvm.Views.Update;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SvManagerLibrary.Chat;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer
{
    public class LogViewerViewModel : ViewModelBase
    {
        private readonly LogViewerModel _model;

        public ReactiveProperty<bool> CanExportPlayer { get; set; }
        public ReactiveProperty<bool> CanExportChat { get; set; }

        public ObservableCollection<string> EncodingItems { get; set; }
        public ReactiveProperty<string> EncodingSelectedItem { get; set; }

        public ReadOnlyCollection<LogFileItem> LogFileList { get; set; }
        public ReactiveProperty<LogFileItem> LogFileSelectedItem { get; set; }
        public ReactiveProperty<bool> IsWordWrapping { get; set; }
        public ReactiveProperty<bool> ProgressBarVisibility { get; set; }
        public ReactiveProperty<ObservableCollection<RichTextItem>> RichLogDetailItems { get; set; }
        public ReactiveProperty<string> LoadedLineLabel { get; set; }

        public ReactiveProperty<string> FilterText { get; set; }
        public ReactiveProperty<ObservableCollection<PlayerItemInfo>> PlayerItems { get; set; }
        public ReactiveProperty<ObservableCollection<ChatInfo>> ChatItems { get; set; }

        public ICommand ExportPlayerCommand { get; set; }
        public ICommand ExportChatCommand { get; set; }

        public ICommand LogFileListSelectionChangedCommand { get; set; }
        public ICommand TextChangedCommand { get; set; }
        public ICommand ScrollEndedCommand { get; set; }

        public ICommand ApplyFilterCommand { get; set; }

        public LogViewerViewModel(IWindowService windowService, LogViewerModel model) : base(windowService, model)
        {
            _model = model;

            CanExportPlayer = new ReactiveProperty<bool>();
            CanExportChat = new ReactiveProperty<bool>();

            EncodingItems = new ObservableCollection<string>(LogFileInfo.EncodingNames);

            LogFileList = model.LogFileList.ToReadOnlyReactiveCollection(m => new LogFileItem(m)).AddTo(CompositeDisposable);
            LogFileSelectedItem = new ReactiveProperty<LogFileItem>();
            LogFileSelectedItem.PropertyChanged += (sender, args) =>
            {
                if (EncodingSelectedItem != null)
                    EncodingSelectedItem.Value = LogFileSelectedItem.Value.EncodingName;
            };

            EncodingSelectedItem = new ReactiveProperty<string>();
            EncodingSelectedItem.PropertyChanged += (sender, args) =>
            {
                if (_model.CurrentFileInfo == null)
                    return;

                if (_model.CurrentFileInfo.EncodingName != EncodingSelectedItem.Value)
                {
                    if (ProgressBarVisibility != null)
                        ProgressBarVisibility.Value = true;
                    _model.CurrentFileInfo.EncodingName = EncodingSelectedItem.Value;
                    _model.RemoveCurrentLogCache();
                    _ = _model.AnalyzeCurrentLogFile();
                }
            };

            RichLogDetailItems = model.ObserveProperty(m => m.RichLogDetailItems).ToReactiveProperty().AddTo(CompositeDisposable);
            ProgressBarVisibility = new ReactiveProperty<bool>();
            IsWordWrapping = new ReactiveProperty<bool>();
            LoadedLineLabel = new ReactiveProperty<string>();

            FilterText = new ReactiveProperty<string>();
            PlayerItems = model.ObserveProperty(m => m.PlayerItemInfos).ToReactiveProperty().AddTo(CompositeDisposable);
            PlayerItems.PropertyChanged += (sender, args) => CanExportPlayer.Value = _model.PlayerItemInfos.Any();
            ChatItems = model.ObserveProperty(m => m.ChatInfos).ToReactiveProperty().AddTo(CompositeDisposable);
            ChatItems.PropertyChanged += (sender, args) => CanExportChat.Value = _model.ChatInfos.Any();

            ExportPlayerCommand = new DelegateCommand(ExportPlayer);
            ExportChatCommand = new DelegateCommand(ExportChat);
            LogFileListSelectionChangedCommand = new DelegateCommand<int?>(LogFileListSelectionChanged);
            TextChangedCommand = new DelegateCommand<BindableRichTextBox>(TextChanged);
            ScrollEndedCommand = new DelegateCommand<BindableRichTextBox>(ReachEndLogText);
            ApplyFilterCommand = new DelegateCommand(ApplyFilter);

            model.Load();
        }

        public void ExportPlayer()
        {
            var players = (from x in _model.PlayerItemInfos select x.GetMap()).ToList();
            var model = new LogExportModel(PlayerItemInfo.Names(), players, "players");
            var vm = new LogExportViewModel(new WindowService(), model);
            WindowManageService.ShowDialog<LogExport>(vm);
        }

        public void ExportChat()
        {
            var chats = (from x in _model.ChatInfos select x.GetMap()).ToList();
            var model = new LogExportModel(ChatInfo.Names(), chats, "chats");
            var vm = new LogExportViewModel(new WindowService(), model);
            WindowManageService.ShowDialog<LogExport>(vm);
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

        public void ApplyFilter()
        {
            _ = _model.ChangeFilter(FilterText.Value);
        }
    }
}
