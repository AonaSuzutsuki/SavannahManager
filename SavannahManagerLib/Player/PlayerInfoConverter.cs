using System.Text.RegularExpressions;

namespace SvManagerLibrary.Player
{
    public class PlayerInfoConverter
    {
        public static PlayerInfo ConvertPlayerDetail(string text)
        {
            PlayerInfo uDetail = null;

            const string expression = "(?<number>.*?) id=(?<identity>.*?), (?<name>.*?), pos=(?<position>.*?), rot=(?<rotate>.*?), remote=(?<remote>.*?), health=(?<health>.*?), deaths=(?<deaths>.*?), zombies=(?<zombies>.*?), players=(?<players>.*?), score=(?<scores>.*?), level=(?<level>.*?), steamid=(?<steamid>.*?), ip=(?<ip>.*?), ping=(?<ping>.*?)$";
            var reg = new Regex(expression);

            var match = reg.Match(text);
            if (match.Success)
            {
                uDetail = new PlayerInfo()
                {
                    Id = match.Groups["identity"].Value,
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
