using System.Collections.Generic;
using System.IO;
using SvManagerLibrary.AnalyzerPlan.Console;

namespace SvManagerLibrary.Player
{
    /// <summary>
    /// Extension methods of a PlayerInfo list object.
    /// </summary>
    public static class PlayerInfoListExtension
    {
        /// <summary>
        /// Convert the text log to PlayerInfo object and then add it.
        /// </summary>
        /// <param name="playerData">The list to add.</param>
        /// <param name="log">The text log to be added.</param>
        /// <param name="analyzerPlan"></param>
        public static List<PlayerInfo> Add(this List<PlayerInfo> playerData, string log, IConsoleAnalyzer analyzerPlan)
        {
            var sr = new StringReader(log);
            while (sr.Peek() > -1)
            {
                var uDetail = PlayerInfoConverter.ConvertPlayerDetail(sr.ReadLine(), analyzerPlan);
                if (uDetail != null)
                    playerData.Add(uDetail);
            }

            return playerData;
        }
    }
}
