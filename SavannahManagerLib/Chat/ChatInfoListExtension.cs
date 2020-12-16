using System.Collections.Generic;
using System.IO;

namespace SvManagerLibrary.Chat
{
    public static class ChatInfoListExtension
    {
        public static void Add(this List<ChatInfo> list, string log)
        {
            var elem = ChatInfoConverter.ConvertChat(log);
            if (!ChatInfo.IsNullOrEmpty(elem))
                list.Add(elem);
        }
        public static void AddMultiLine(this List<ChatInfo> list, string log)
        {
            using var sr = new StringReader(log);
            while (sr.Peek() > 0)
            {
                list.Add(sr.ReadLine());
            }
        }
    }
}
