using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SvManagerLibrary.Chat
{
    /// <summary>
    /// Extension methods of a ChatInfo list object.
    /// </summary>
    public static class ChatInfoListExtension
    {
        /// <summary>
        /// Convert the text log to ChatInfo object and then add it.
        /// </summary>
        /// <param name="list">The list to add.</param>
        /// <param name="log">The text log to be added.</param>
        public static void Add(this List<ChatInfo> list, string log)
        {
            var elem = ChatInfoConverter.ConvertChat(log);
            if (!ChatInfo.IsNullOrEmpty(elem))
                list.Add(elem);
        }

        /// <summary>
        /// Convert the text log included breakline to ChatInfo object and then add it.
        /// </summary>
        /// <param name="list">The list to add.</param>
        /// <param name="log">The text log included breakline to be added.</param>
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
