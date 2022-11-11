using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using SvManagerLibrary.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer
{
    public interface ILogAnalyzer
    {
        List<RichTextItem> LogRichTextList { get; set; }
        List<ChatInfo> ChatList { get; set; }
        Dictionary<string, List<PlayerItemInfo>> PlayerInfos { get; set; }

        void Analyze(LogFileInfo logFileInfo);
        List<RichTextItem> Analyze(LogFileInfo logFileInfo, string filter);
    }
}
