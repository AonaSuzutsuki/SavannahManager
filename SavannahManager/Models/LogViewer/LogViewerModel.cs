using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Views.Update;
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

        public ObservableCollection<LogFileInfo> LogFileList { get; set; } = new();

        public ObservableCollection<RichTextItem> RichLogDetailItems
        {
            get => _richLogDetailItems;
            set => SetProperty(ref _richLogDetailItems, value);
        }

        public void Load()
        {
            var files = CommonCoreLib.File.DirectorySearcher.GetAllFiles("logs", "*.log");

            foreach (var file in files)
            {
                LogFileList.Add(new LogFileInfo(file));
            }
        }

        public void AnalyzeLogFile(int index)
        {
            if (index < 0 && index >= LogFileList.Count)
                return;

            var list = new List<RichTextItem>(LogFileList.Count);

            var logFileInfo = LogFileList[index];
            using var stream = new FileStream(logFileInfo.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var streamReader = new StreamReader(stream);
            while (streamReader.Peek() > -1)
            {
                var line = streamReader.ReadLine();
                if (line == null)
                    continue;

                var paragraph = new RichTextItem
                {
                    TextType = RichTextType.Paragraph
                };
                paragraph.AddChildren(new RichTextItem
                {
                    TextType = RichTextType.Text,
                    Text = line
                });
                list.Add(paragraph);
            }

            RichLogDetailItems = new ObservableCollection<RichTextItem>(list);
        }
    }
}
