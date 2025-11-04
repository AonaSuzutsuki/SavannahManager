using System.IO;
using System.Text.RegularExpressions;
using SvManagerLibrary.AnalyzerPlan.Console;

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
        /// <param name="analyzerPlan"></param>
        /// <returns>ChatInfo object.</returns>
        public static ChatInfo ConvertChat(string text, IConsoleAnalyzer analyzerPlan)
        {
            //2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': test
            //2025-02-22T00:55:15 2223.454 INF Chat (from '-non-player-', entity id '-1', to 'Global'): test
            // "^(?<date>.*?) (.*?) INF Chat: '(?<name>.*?)': (?<chat>.*?)$";

            // 2024-07-21T15:44:58 26527.982 INF Chat (from 'Steam_76561198010715714', entity id '171', to 'Global'): kusa
            // 2024-07-17T16:56:21 7943.935 INF Chat (from '-non-player-', entity id '-1', to 'Global'): gomi
            var expression = analyzerPlan.GetChatExpression();
            var reg = new Regex(expression);
            var sr = new StringReader(text);

            while (sr.Peek() > -1)
            {
                var match = reg.Match(sr.ReadLine() ?? string.Empty);
                if (match.Success)
                {
                    var name = match.Groups["name"].Value;
                    if (string.IsNullOrEmpty(name))
                        name = "Server";

                    var chatData = new ChatInfo
                    {
                        Name = name,
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
