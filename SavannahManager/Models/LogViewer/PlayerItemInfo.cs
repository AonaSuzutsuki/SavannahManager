using System.Collections;
using System.Collections.Generic;

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

    public Dictionary<string, string> GetMap()
    {
        return new Dictionary<string, string>
        {
            { nameof(Date), Date },
            { nameof(InOut), InOut.ToString() },
            { nameof(Name), Name },
            { nameof(Id), Id },
            { nameof(SteamId), SteamId },
            { nameof(IpAddress), IpAddress },
            { nameof(Position), Position },
        };
    }

    public static IEnumerable<string> Names()
    {
        return new List<string>
        {
            nameof(Date),
            nameof(InOut),
            nameof(Name),
            nameof(Id),
            nameof(SteamId),
            nameof(IpAddress),
            nameof(Position)
        };
    }
}