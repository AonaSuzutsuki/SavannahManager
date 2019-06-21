using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SvManagerLibrary.Chat
{
    public static class ChatInfoListExtension
    {
        public static void Add(this List<ChatInfo> list, string log)
        {
            list.Add(ChatInfoConverter.ConvertChat(log));
        }
        public static void AddMultiLine(this List<ChatInfo> list, string log)
        {
            using (var sr = new StringReader(log))
            {
                while (sr.Peek() > 0)
                {
                    list.Add(sr.ReadLine());
                }
            }
        }
        public static ChatInfo GetLast(this List<ChatInfo> list)
        {
            var count = list.Count;
            if (count > 0)
                return list[count - 1];
            return null;
        }
    }
}
