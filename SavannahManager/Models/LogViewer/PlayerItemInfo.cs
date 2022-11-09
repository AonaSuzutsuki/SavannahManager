namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer;

public class PlayerItemInfo
{
    public enum InOutType
    {
        In,
        Out
    }

    public InOutType InOut { get; set; }
    public string Date { get; set; }
    public string Name { get; set; }
    public string Id { get; set; }
    public string SteamId { get; set; }
    public string IpAddress { get; set; }
    public string Position { get; set; }
}