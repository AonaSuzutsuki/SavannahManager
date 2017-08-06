using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SvManagerLibrary.Player
{
    public class PlayerInfoArray : IList<PlayerInfo>
    {
        private readonly List<PlayerInfo> playerData = new List<PlayerInfo>();

        public PlayerInfo this[int index]
        {
            get => playerData[index];
            set => playerData[index] = value;
        }

        public void Add(PlayerInfo item)
        {
            playerData.Add(item);
        }
        public void Add(string log)
        {
            StringReader sr = new StringReader(log);
            while (sr.Peek() > -1)
            {
                PlayerInfo uDetail = PlayerInfoConverter.ConvertPlayerDetail(sr.ReadLine());
                if (uDetail != null)
                {
                    playerData.Add(uDetail);
                }
            }
        }

        public int Count => playerData.Count;
        public bool IsReadOnly => ((IList<PlayerInfo>)playerData).IsReadOnly;

        public void Clear()
        {
            playerData.Clear();
        }
        public bool Contains(PlayerInfo item)
        {
            return playerData.Contains(item);
        }
        public void CopyTo(PlayerInfo[] array, int arrayIndex)
        {
            playerData.CopyTo(array, arrayIndex);
        }
        public IEnumerator<PlayerInfo> GetEnumerator()
        {
            return ((IList<PlayerInfo>)playerData).GetEnumerator();
        }
        public int IndexOf(PlayerInfo item)
        {
            return playerData.IndexOf(item);
        }
        public void Insert(int index, PlayerInfo item)
        {
            playerData.Insert(index, item);
        }
        public bool Remove(PlayerInfo item)
        {
            return playerData.Remove(item);
        }
        public void RemoveAt(int index)
        {
            playerData.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<PlayerInfo>)playerData).GetEnumerator();
        }
    }
}
