using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _7dtd_svmanager_fix_mvvm.Models.LogViewer.Versions.a20;
using _7dtd_svmanager_fix_mvvm.Views.Update;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;
using CommonStyleLib.Models.Errors;
using SvManagerLibrary.Chat;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer
{
    public class LogViewerModel : ModelBase
    {
        private LogFileInfo _currentFileInfo;
        private ObservableCollection<RichTextItem> _richLogDetailItems = new();
        private readonly Dictionary<string, LogCacheItem> _cache = new();
        private ObservableCollection<ChatInfo> _chatInfos = new();
        private ObservableCollection<PlayerItemInfo> _playerItemInfos = new();

        public ObservableCollection<LogFileInfo> LogFileList { get; set; } = new();

        public LogFileInfo CurrentFileInfo
        {
            get => _currentFileInfo;
            set => SetProperty(ref _currentFileInfo, value);
        }

        public ObservableCollection<RichTextItem> RichLogDetailItems
        {
            get => _richLogDetailItems;
            set => SetProperty(ref _richLogDetailItems, value);
        }

        public ObservableCollection<ChatInfo> ChatInfos
        {
            get => _chatInfos;
            set => SetProperty(ref _chatInfos, value);
        }

        public ObservableCollection<PlayerItemInfo> PlayerItemInfos
        {
            get => _playerItemInfos;
            set => SetProperty(ref _playerItemInfos, value);
        }

        public void Load()
        {
            if (!Directory.Exists("logs"))
                return;

            var files = Directory.GetFiles("logs", "*.log").ToList();
            files.Reverse();

            foreach (var file in files)
            {
                LogFileList.Add(new LogFileInfo(file));
            }
        }

        public int GetFileIndex(LogFileInfo info)
        {
            return LogFileList.IndexOf(info);
        }

        public async Task AnalyzeLogFile(int index)
        {
            if (index < 0 && index >= LogFileList.Count)
                return;

            _currentFileInfo?.ClearCache();

            var logFileInfo = LogFileList[index];
            _currentFileInfo = logFileInfo;

            await AnalyzeCurrentLogFile();
        }

        public async Task AnalyzeCurrentLogFile()
        {
            var logFileInfo = _currentFileInfo;
            if (logFileInfo == null)
                return;

            lock (_cache)
            {
                if (_cache.ContainsKey(logFileInfo.FullPath))
                {
                    var cache = _cache[logFileInfo.FullPath];
                    RichLogDetailItems = cache.RichTextItems;
                    ChatInfos = cache.ChatItems;
                    PlayerItemInfos = cache.PlayerItems;

                    return;
                }
            }
            
            await Task.Factory.StartNew(() =>
            {
                var logAnalyzer = logFileInfo.LogAnalyzer;
                logAnalyzer.Analyze(logFileInfo);
                
                RichLogDetailItems = new ObservableCollection<RichTextItem>(logAnalyzer.LogRichTextList);
                ChatInfos = new ObservableCollection<ChatInfo>(logAnalyzer.ChatList);

                var playerItemList = new List<PlayerItemInfo>();
                foreach (var playerItems in logAnalyzer.PlayerInfos)
                {
                    playerItemList.AddRange(playerItems.Value);
                }

                PlayerItemInfos = new ObservableCollection<PlayerItemInfo>(playerItemList.OrderBy(x => x.Date));

                lock (_cache)
                {
                    _cache.Put(logFileInfo.FullPath, new LogCacheItem
                    {
                        RichTextItems = RichLogDetailItems,
                        ChatItems = ChatInfos,
                        PlayerItems = PlayerItemInfos
                    });
                }
            });
        }

        public void RemoveCurrentLogCache()
        {
            var logFileInfo = _currentFileInfo;
            if (logFileInfo == null)
                return;

            lock(_cache)
            {
                _cache.Remove(logFileInfo.FullPath);
            }
        }

        public void SetLogAnalyzer(string version)
        {
            _currentFileInfo.ChangeLogAnalyzer(version);
        }

        public async Task ChangeFilter(string filter)
        {
            var fileInfo = _currentFileInfo;

            if (fileInfo == null)
                return;

            lock(_cache)
            {
                if (!_cache.ContainsKey(fileInfo.FullPath))
                    return;
            }

            if (string.IsNullOrEmpty(filter))
            {
                lock (_cache)
                {
                    var cache = _cache[fileInfo.FullPath];
                    RichLogDetailItems = cache.RichTextItems;
                    return;
                }
            }
            
            await Task.Factory.StartNew(() =>
            {
                var list = fileInfo.LogAnalyzer.Analyze(fileInfo, filter);

                RichLogDetailItems = new ObservableCollection<RichTextItem>(list);
            });
        }
        
    }
}
