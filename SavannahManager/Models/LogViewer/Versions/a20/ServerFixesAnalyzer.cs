using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SvManagerLibrary.AnalyzerPlan.Console;
using SvManagerLibrary.Chat;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer.Versions.a20;

public class ServerFixesAnalyzer : AbstractLogAnalyzer, ILogAnalyzer
{
    public const string AnalyzerName = "ServerFixes";

    public string Name => AnalyzerName;

    protected override void AnalyzePlayer(Dictionary<string, List<PlayerItemInfo>> playerDict, string line)
    {
        var connectedRegex = new Regex("^(?<date>[0-9a-zA-Z-:]+) (?<tick>[0-9.]+) INF Player connected, entityid=(?<id>[0-9]+), name=(?<name>.*), pltfmid=(?<steamId>[a-zA-Z0-9_]+), crossid=[a-zA-Z_0-9]+, steamOwner=[a-zA-Z_0-9]+, ip=(?<ip>[a-zA-Z0-9.:]+)$");
        var spawnedRegex = new Regex("^(?<date>[0-9a-zA-Z-:]+) (?<tick>[0-9.]+) INF PlayerSpawnedInWorld \\(reason:(?<reason>.*), position: (?<position>[0-9, -]+)\\): EntityID=(?<id>[0-9-]+), PltfmId='[a-zA-Z0-9_]+', CrossId='[a-zA-Z0-9_]+', OwnerID='[a-zA-Z0-9_]+', PlayerName='(?<name>.*)', ClientNumber='(?<clientNumber>.*)'$");
        var disconnectedRegex = new Regex("^(?<date>[0-9a-zA-Z-:]+) (?<tick>[0-9.]+) INF Player disconnected: EntityID=(?<id>[0-9]+), PltfmId='(?<steamId>[a-zA-Z0-9_]+)', CrossId='[a-zA-Z0-9_]+', OwnerID='[a-zA-Z0-9_]+', PlayerName='(?<name>.*)', ClientNumber='(?<clientNumber>.*)'$");

        var connectedMatch = connectedRegex.Match(line);
        var spawnedMatch = spawnedRegex.Match(line);
        var disconnectedMatch = disconnectedRegex.Match(line);

        if (connectedMatch.Success)
        {
            var id = connectedMatch.Groups["id"].Value;
            var players = GetPlayerList(playerDict, id);

            players.Add(new PlayerItemInfo
            {
                InOut = PlayerItemInfo.InOutType.In,
                Date = connectedMatch.Groups["date"].Value,
                Id = id,
                Name = connectedMatch.Groups["name"].Value,
                SteamId = connectedMatch.Groups["steamId"].Value,
                IpAddress = connectedMatch.Groups["ip"].Value
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
        else if (spawnedMatch.Success)
        {
            var id = spawnedMatch.Groups["id"].Value;
            var players = GetPlayerList(playerDict, id);
            var player = players.LastOrDefault(x => x.InOut == PlayerItemInfo.InOutType.In);

            if (player != null)
            {
                player.Position = spawnedMatch.Groups["position"].Value;
            }
        }
    }

    protected override ChatInfo AnalyzeChat(string line)
    {
        return ChatInfoConverter.ConvertChat(line, new OnePointTreeConsoleAnalyzer());
    }
}