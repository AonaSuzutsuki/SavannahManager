using System.Collections.Generic;
using System.Text.RegularExpressions;
using SvManagerLibrary.AnalyzerPlan.Console;
using SvManagerLibrary.Chat;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer.Versions.a20;

public class Formal1Point3Analyzer : AbstractLogAnalyzer, ILogAnalyzer
{
    public const string AnalyzerName = "1.3";

    public string Name => AnalyzerName;

    protected override void AnalyzePlayer(Dictionary<string, List<PlayerItemInfo>> playerDict, string line)
    {
        var spawnedRegex = new Regex("^(?<date>[0-9a-zA-Z-:]+) (?<tick>[0-9.]+) INF PlayerSpawnedInWorld \\(reason:(?<reason>.*), position: (?<position>[0-9, -]+)\\): EntityID=(?<id>[0-9-]+), PltfmId='(?<steamId>[a-zA-Z0-9_]+)', CrossId='[a-zA-Z0-9_]+', OwnerID='[a-zA-Z0-9_]+', PlayerName='(?<name>.*)', ClientNumber='(?<clientNumber>.*)'$");
        var disconnectedRegex = new Regex("^(?<date>[0-9a-zA-Z-:]+) (?<tick>[0-9.]+) INF Player disconnected: EntityID=(?<id>[0-9]+), PltfmId='(?<steamId>[a-zA-Z0-9_]+)', CrossId='[a-zA-Z0-9_]+', OwnerID='[a-zA-Z0-9_]+', PlayerName='(?<name>.*)', ClientNumber='(?<clientNumber>.*)'$");

        var spawnedMatch = spawnedRegex.Match(line);
        var disconnectedMatch = disconnectedRegex.Match(line);

        if (spawnedMatch.Success)
        {
            var id = spawnedMatch.Groups["id"].Value;
            var players = GetPlayerList(playerDict, id);

            players.Add(new PlayerItemInfo
            {
                InOut = PlayerItemInfo.InOutType.In,
                Date = spawnedMatch.Groups["date"].Value,
                Id = id,
                Name = spawnedMatch.Groups["name"].Value,
                SteamId = spawnedMatch.Groups["steamId"].Value,
                IpAddress = spawnedMatch.Groups["ip"].Value,
                Position = spawnedMatch.Groups["position"].Value
            });
        }
        else if (disconnectedMatch.Success)
        {
            var id = disconnectedMatch.Groups["id"].Value;
            var players = GetPlayerList(playerDict, id);

            players.Add(new PlayerItemInfo
            {
                InOut = PlayerItemInfo.InOutType.Out,
                Date = disconnectedMatch.Groups["date"].Value,
                Id = id,
                Name = disconnectedMatch.Groups["name"].Value,
                SteamId = disconnectedMatch.Groups["steamId"].Value
            });
        }
    }

    protected override ChatInfo AnalyzeChat(string line)
    {
        return ChatInfoConverter.ConvertChat(line, new OnePointTreeConsoleAnalyzer());
    }
}