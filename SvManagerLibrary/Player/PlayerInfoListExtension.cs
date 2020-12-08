using System.Collections.Generic;
using System.IO;

namespace SvManagerLibrary.Player
{
    public static class PlayerInfoListExtension
    {
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
