using System.Collections.ObjectModel;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using SvManagerLibrary.Chat;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer;

public class LogCacheItem
{
    public ObservableCollection<RichTextItem> RichTextItems { get; set; }
    public ObservableCollection<ChatInfo> ChatItems { get; set; }
    public ObservableCollection<PlayerItemInfo> PlayerItems { get; set; }
}