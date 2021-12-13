using CommonExtensionLib.Extensions;
using SvManagerLibrary.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SvManagerLibrary.Player
{
    /// <summary>
    /// Provides a converter of PlayerInfo.
    /// </summary>
    public class PlayerInfoConverter
    {

        private string text = "";
        private Dictionary<string, string> _properties = new Dictionary<string,string>();

        public IReadOnlyDictionary<string, string> Properties { get; }

        public PlayerInfoConverter(string text)
        {
            _properties = new Dictionary<string, string>();
            Properties = _properties;
            this.text = text;
        }

        public void Analyze()
        {
            var number = "0";
            var id = "0";
            var name = "0";

            const string firstExpression = "(?<number>[0-9]+\\.) id=(?<identity>.*?), (?<name>.*?), ";
            var firstRegex = new Regex(firstExpression);
            var firstMatch = firstRegex.Match(text);
            if (firstMatch.Success)
            {
                number = firstMatch.Groups["number"].Value;
                id = firstMatch.Groups["identity"].Value;
                name = firstMatch.Groups["name"].Value;

                _properties.Add("id", id);
                _properties.Add("name", name);

                firstRegex.Replace(text, "");
            }

            const string secondExpression = "(, )*(?<name>[a-zA-Z]+)=(?<value>(\\([0-9., -]+\\))|([0-9a-zA-Z_\\.:]+))";
            var secondRegex = new Regex(secondExpression);
            foreach (Match secondMatch in secondRegex.Matches(text))
            {
                var propName = secondMatch.Groups["name"].Value;
                var propValue = secondMatch.Groups["value"].Value;
                _properties.Put(propName, propValue);
            }
        }

        /// <summary>
        /// Convert a text of 7dtd telnet log to a PlayerInfo object.
        /// </summary>
        /// <param name="text">7dtd telnet log.</param>
        /// <returns>PlayerInfo object.</returns>
        public static PlayerInfo ConvertPlayerDetail(string text)
        {
            var converter = new PlayerInfoConverter(text);
            converter.Analyze();

            var player = new PlayerInfo()
            {
                Id = converter.Properties.Get("id"),
                Level = converter.Properties.Get("level"),
                Name = converter.Properties.Get("name"),
                Health = converter.Properties.Get("health"),
                ZombieKills = converter.Properties.Get("zombies"),
                PlayerKills = converter.Properties.Get("players"),
                Deaths = converter.Properties.Get("deaths"),
                Score = converter.Properties.Get("score"),
                Coord = converter.Properties.Get("pos"),
                SteamId = converter.Properties.Get("pltfmid") ?? converter.Properties.Get("steamid"),
            };

            return player;



            // 0. id=171, Aona Suzutsuki, pos=(741.9, 45.1, 951.5), rot=(-11.2, -406.4, 0.0), remote=True, health=92, deaths=0, zombies=4, players=0, score=0, level=1, pltfmid=Steam_76561198010715714, crossid=EOS_0002bfc568d6401ca9de387e0ae914c9, ip=192.168.0.81, ping=0
            // Total of 1 in the game
            //const string expression = "(?<number>.*?) id=(?<identity>.*?), (?<name>.*?), pos=(?<position>.*?), rot=(?<rotate>.*?), remote=(?<remote>.*?), health=(?<health>.*?), deaths=(?<deaths>.*?), zombies=(?<zombies>.*?), players=(?<players>.*?), score=(?<scores>.*?), level=(?<level>.*?), steamid=(?<steamid>.*?), ip=(?<ip>.*?), ping=(?<ping>.*?)$";
            //var reg = new Regex(expression);

            //var match = reg.Match(text);
            //if (match.Success)
            //{
            //    uDetail = new PlayerInfo()
            //    {
            //        Id = match.Groups["identity"].Value,
            //        Level = match.Groups["level"].Value,
            //        Name = match.Groups["name"].Value,
            //        Health = match.Groups["health"].Value,
            //        ZombieKills = match.Groups["zombies"].Value,
            //        PlayerKills = match.Groups["players"].Value,
            //        Deaths = match.Groups["deaths"].Value,
            //        Score = match.Groups["scores"].Value,
            //        Coord = match.Groups["position"].Value,
            //        SteamId = match.Groups["steamid"].Value,
            //    };
            //}

            //return uDetail;
        }
    }
}
