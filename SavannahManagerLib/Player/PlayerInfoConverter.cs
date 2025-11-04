using CommonExtensionLib.Extensions;
using SvManagerLibrary.AnalyzerPlan.Console;
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
        private IConsoleAnalyzer _analyzerPlan;

        public IReadOnlyDictionary<string, string> Properties { get; }

        public PlayerInfoConverter(string text, IConsoleAnalyzer analyzerPlan)
        {
            _properties = new Dictionary<string, string>();
            Properties = _properties;
            this.text = text;
            _analyzerPlan = analyzerPlan;
        }

        public void Analyze()
        {
            var expressions = _analyzerPlan.GetPlayerExpression();
            var firstExpression = expressions.first;
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

            var secondExpression = expressions.second;
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
        /// <param name="analyzerPlan"></param>
        /// <returns>PlayerInfo object.</returns>
        public static PlayerInfo ConvertPlayerDetail(string text, IConsoleAnalyzer analyzerPlan)
        {
            var converter = new PlayerInfoConverter(text, analyzerPlan);
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
