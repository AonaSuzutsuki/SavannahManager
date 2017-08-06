using System.Text.RegularExpressions;

namespace SvManagerLibrary.Player
{
    public class PlayerInfoConverter
    {
        public static PlayerInfo ConvertPlayerDetail(string text)
        {
            PlayerInfo uDetail = null;

            // 0. id=171, Aona Suzutsuki, pos=(-157.3, 61.1, -115.8), rot=(0.0, 350.2, 0.0), remote=True, health=32, deaths=18, zombies=0, players=0, score=0, level=1, steamid=76561198010715714, ip=192.168.1.45, ping=2
            const string expression = "(?<number>.*?) id=(?<entityid>.*?), (?<name>.*?), pos=(?<position>.*?), rot=(?<rotate>.*?), remote=(?<remote>.*?), health=(?<health>.*?), deaths=(?<deaths>.*?), zombies=(?<zombies>.*?), players=(?<players>.*?), score=(?<scores>.*?), level=(?<level>.*?), steamid=(?<steamid>.*?), ip=(?<ip>.*?), ping=(?<ping>.*?)$";
            var reg = new Regex(expression);

            var items = new string[9];
            var match = reg.Match(text);
            if (match.Success == true)
            {
                uDetail = new PlayerInfo()
                {
                    Id = match.Groups["entityid"].Value,
                    Level = match.Groups["level"].Value,
                    Name = match.Groups["name"].Value,
                    Health = match.Groups["health"].Value,
                    ZombieKills = match.Groups["zombies"].Value,
                    PlayerKills = match.Groups["players"].Value,
                    Deaths = match.Groups["deaths"].Value,
                    Score = match.Groups["scores"].Value,
                    Coord = match.Groups["position"].Value,
                    SteamId = match.Groups["steamid"].Value,
                };
            }

            return uDetail;
        }
    }
}
