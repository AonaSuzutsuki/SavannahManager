using System.Collections.Generic;
using System.Linq;
using System.IO;
using SvManagerLibrary.AnalyzerPlan.Console;

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
        /// <param name="analyzerPlan"></param>
        public static List<ChatInfo> Add(this List<ChatInfo> list, string log, IConsoleAnalyzer analyzerPlan)
        {
            var elem = ChatInfoConverter.ConvertChat(log, analyzerPlan);
            if (!ChatInfo.IsNullOrEmpty(elem))
                list.Add(elem);

            return list;
        }

        /// <summary>
        /// Convert the text log included breakline to ChatInfo object and then add it.
        /// </summary>
        /// <param name="list">The list to add.</param>
        /// <param name="log">The text log included breakline to be added.</param>
        /// <param name="analyzerPlan"></param>
        public static List<ChatInfo> AddMultiLine(this List<ChatInfo> list, string log, IConsoleAnalyzer analyzerPlan)
        {
            using var sr = new StringReader(log);
            while (sr.Peek() > 0)
            {
                list.Add(sr.ReadLine(), analyzerPlan);
            }

            return list;
        }

        public static Stack<ChatInfo> AddMultiLine(this Stack<ChatInfo> stack, string log, IConsoleAnalyzer analyzerPlan)
        {
            using var sr = new StringReader(log);
            while (sr.Peek() > 0)
            {
                var line = sr.ReadLine();
                var elem = ChatInfoConverter.ConvertChat(line, analyzerPlan);
                stack.Push(elem);
            }

            return stack;
        }
    }
}
