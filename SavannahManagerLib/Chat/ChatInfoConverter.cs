using System.IO;
using System.Text.RegularExpressions;

namespace SvManagerLibrary.Chat
{
    public class ChatInfoConverter
    {
        public static ChatInfo ConvertChat(string text)
        {
            //2019-01-19T16:14:21 140.048 INF Chat (from '-non-player-', entity id '-1', to 'Global'): 'Server': test
            var chatData = new ChatInfo();
            const string expression = "^([0-9a-zA-Z:-]+) ([0-9.]+?) INF Chat \\(.*\\): '(?<name>.*)': (?<chat>.*)$"; // "^(?<date>.*?) (.*?) INF Chat: '(?<name>.*?)': (?<chat>.*?)$";
            var reg = new Regex(expression);
            var sr = new StringReader(text);

            while (sr.Peek() > -1)
            {
                var match = reg.Match(sr.ReadLine() ?? string.Empty);
                if (match.Success)
                {
                    chatData.Name = match.Groups["name"].Value;
                    chatData.Message = match.Groups["chat"].Value;
                }
            }

            return chatData;
        }
    }
}
