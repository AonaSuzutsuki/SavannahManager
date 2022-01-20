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
using _7dtd_svmanager_fix_mvvm.Views.Update;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer
{
    public class LogFileInfo
    {
        public string Name => Info.Name;
        public string FullPath => Info.FullName;
        public FileInfo Info { get; }

        public LogFileInfo(string path)
        {
            Info = new FileInfo(path);
        }
    }

    public class LogViewerModel : ModelBase
    {
        private ObservableCollection<RichTextItem> _richLogDetailItems = new();
        private readonly Dictionary<string, ObservableCollection<RichTextItem>> _cache = new();

        public ObservableCollection<LogFileInfo> LogFileList { get; set; } = new();

        public ObservableCollection<RichTextItem> RichLogDetailItems
        {
            get => _richLogDetailItems;
            set => SetProperty(ref _richLogDetailItems, value);
        }

        public void Load()
        {
            var files = CommonCoreLib.File.DirectorySearcher.GetAllFiles("logs", "*.log").ToList();
            files.Reverse();

            foreach (var file in files)
            {
                LogFileList.Add(new LogFileInfo(file));
            }
        }

        public async Task AnalyzeLogFile(int index)
        {
            if (index < 0 && index >= LogFileList.Count)
                return;

            var logFileInfo = LogFileList[index];
            if (_cache.ContainsKey(logFileInfo.FullPath))
            {
                RichLogDetailItems = _cache[logFileInfo.FullPath];
                return;
            }

            await Task.Factory.StartNew(() =>
            {
                var list = new List<RichTextItem>(LogFileList.Count);
                using var stream = new FileStream(logFileInfo.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var streamReader = new StreamReader(stream);
                while (streamReader.Peek() > -1)
                {
                    var line = streamReader.ReadLine();
                    if (line == null)
                        continue;

                    var paragraph = AnalyzeLine(line);
                    list.Add(paragraph);
                }

                RichLogDetailItems = new ObservableCollection<RichTextItem>(list);
                _cache.Add(logFileInfo.FullPath, RichLogDetailItems);
            });
        }

        private RichTextItem AnalyzeLine(string line)
        {
            var item = AnalyzeComment(line);
            if (item != null)
                return item;

            var commonItem = AnalyzeCommonText(line);
            if (commonItem != null)
                return commonItem;

            var text = CreateTextItem(line);

            return text;
        }

        private RichTextItem CreateTextItem(string line)
        {
            var paragraph = new RichTextItem
            {
                TextType = RichTextType.Paragraph
            };
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Text = line
            });

            return paragraph;
        }

        private RichTextItem AnalyzeCommonText(string line)
        {
            var expression = @"^(?<Time>[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2} [0-9]+\.[0-9]+) (?<InfoType>[a-zA-Z]+) (?<Message>.*)$";
            var regex = new Regex(expression);
            var match = regex.Match(line);
            if (!match.Success)
                return null;

            var spaceItem = new RichTextItem
            {
                TextType = RichTextType.Text,
                Text = " "
            };
            var time = match.Groups["Time"].Value;
            var infoType = match.Groups["InfoType"].Value;
            var message = match.Groups["Message"].Value;

            var paragraph = new RichTextItem
            {
                TextType = RichTextType.Paragraph
            };
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Foreground = Color.FromRgb(64, 155, 81),
                Text = time
            });
            paragraph.AddChildren(spaceItem);
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Foreground = Color.FromRgb(69, 157, 255),
                Text = infoType
            });
            paragraph.AddChildren(spaceItem);
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Text = message
            });
            return paragraph;
        }

        private RichTextItem AnalyzeComment(string line)
        {
            var expression = @"^\*\*\* (.*)$";
            var regex = new Regex(expression);
            var match = regex.Match(line);
            if (!match.Success)
                return null;

            var paragraph = new RichTextItem
            {
                TextType = RichTextType.Paragraph
            };
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Foreground = Color.FromRgb(64, 155, 81),
                Text = line
            });
            return paragraph;
        }
    }
}
