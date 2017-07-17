using System.IO;
using System.Text.RegularExpressions;

namespace SvManagerLibrary.Chat
{
    public class ChatInfoConverter
    {
        public static ChatInfo ConvertChat(string text)
        {
            var chatData = new ChatInfo();
            const string expression = "^(?<date>.*?) (.*?) INF Chat: '(?<name>.*?)': (?<chat>.*?)$";
            var reg = new Regex(expression);
            var sr = new StringReader(text);

            while (sr.Peek() > -1)
            {
                var match = reg.Match(sr.ReadLine());
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
