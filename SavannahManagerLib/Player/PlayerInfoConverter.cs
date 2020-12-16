using System.Text.RegularExpressions;

namespace SvManagerLibrary.Player
{
    /// <summary>
    /// Provides a converter of PlayerInfo.
    /// </summary>
    public class PlayerInfoConverter
    {
        /// <summary>
        /// Convert a text of 7dtd telnet log to a PlayerInfo object.
        /// </summary>
        /// <param name="text">7dtd telnet log.</param>
        /// <returns>PlayerInfo object.</returns>
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
