using System.IO;
using System.Text.RegularExpressions;

namespace SvManagerLibrary.Chat
{
    /// <summary>
    /// Provides a converter of ChatInfo.
    /// </summary>
    public class ChatInfoConverter
    {
        /// <summary>
        /// Convert a text of 7dtd telnet log to a ChatInfo object.
        /// </summary>
        /// <param name="text">7dtd telnet log.</param>
        /// <returns>ChatInfo object.</returns>
        public static ChatInfo ConvertChat(string text)
        {
            //2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': test
            // "^(?<date>.*?) (.*?) INF Chat: '(?<name>.*?)': (?<chat>.*?)$";
            const string expression = "^(?<date>[0-9a-zA-Z:-]+) ([0-9.]+?) INF Chat \\(from '(?<steamId>[a-zA-Z0-9_-]+)', entity id '(?<id>[0-9-]+)'.*\\): '(?<name>.*)': (?<chat>.*)$";
            var reg = new Regex(expression);
            var sr = new StringReader(text);

            while (sr.Peek() > -1)
            {
                var match = reg.Match(sr.ReadLine() ?? string.Empty);
                if (match.Success)
                {
                    var chatData = new ChatInfo
                    {
                        Name = match.Groups["name"].Value,
                        Message = match.Groups["chat"].Value,
                        Date = match.Groups["date"].Value,
                        Id = match.Groups["id"].Value,
                        SteamId = match.Groups["steamId"].Value
                    };
                    return chatData;
                }
            }

            return null;
        }
    }
}
