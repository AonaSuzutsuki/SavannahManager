using System.Collections.Generic;
using System.IO;

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
        public static void Add(this List<PlayerInfo> playerData, string log)
        {
            var sr = new StringReader(log);
            while (sr.Peek() > -1)
            {
                var uDetail = PlayerInfoConverter.ConvertPlayerDetail(sr.ReadLine());
                if (uDetail != null)
                    playerData.Add(uDetail);
            }
        }
    }
}
