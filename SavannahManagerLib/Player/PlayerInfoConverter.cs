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
            const string firstExpression = "(?<number>[0-9]+\\.) id=(?<identity>.*?), (?<name>.*?), ";
            var firstRegex = new Regex(firstExpression);
            var firstMatch = firstRegex.Match(text);
            if (firstMatch.Success)
            {
                var number = firstMatch.Groups["number"].Value;
                var id = firstMatch.Groups["identity"].Value;
                var name = firstMatch.Groups["name"].Value;

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

            if (_properties.ContainsKey("pltfmid"))
            {
                var value = _properties.Get("pltfmid");
                var steamId = value.Replace("Steam_", "");
                _properties.Put("pltfmid", steamId);
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

            if (converter.Properties.Count <= 0)
                return null;

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
                            // a20-                                 // -a19
                SteamId = converter.Properties.Get("pltfmid") ?? converter.Properties.Get("steamid"),
            };

            return player;
        }
    }
}
