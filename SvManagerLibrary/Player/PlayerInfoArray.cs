using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SvManagerLibrary.Player
{
    public class PlayerInfoArray : IList<PlayerInfo>
    {
        private readonly List<PlayerInfo> _playerData = new List<PlayerInfo>();

        public PlayerInfo this[int index]
        {
            get
            {
                return ((IList<PlayerInfo>)_playerData)[index];
            }

            set
            {
                ((IList<PlayerInfo>)_playerData)[index] = value;
            }
        }

        public void Add(PlayerInfo item)
        {
            ((IList<PlayerInfo>)_playerData).Add(item);
        }
        public void Add(string log)
        {
            StringReader sr = new StringReader(log);
            while (sr.Peek() > -1)
            {
                PlayerInfo uDetail = PlayerInfoConverter.ConvertPlayerDetail(sr.ReadLine());
                if (uDetail != null)
                {
                    _playerData.Add(uDetail);
                }
            }
        }

        public int Count
        {
            get
            {
                return ((IList<PlayerInfo>)_playerData).Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return ((IList<PlayerInfo>)_playerData).IsReadOnly;
            }
        }
        public void Clear()
        {
            ((IList<PlayerInfo>)_playerData).Clear();
        }
        public bool Contains(PlayerInfo item)
        {
            return ((IList<PlayerInfo>)_playerData).Contains(item);
        }
        public void CopyTo(PlayerInfo[] array, int arrayIndex)
        {
            ((IList<PlayerInfo>)_playerData).CopyTo(array, arrayIndex);
        }
        public IEnumerator<PlayerInfo> GetEnumerator()
        {
            return ((IList<PlayerInfo>)_playerData).GetEnumerator();
        }
        public int IndexOf(PlayerInfo item)
        {
            return ((IList<PlayerInfo>)_playerData).IndexOf(item);
        }
        public void Insert(int index, PlayerInfo item)
        {
            ((IList<PlayerInfo>)_playerData).Insert(index, item);
        }
        public bool Remove(PlayerInfo item)
        {
            return ((IList<PlayerInfo>)_playerData).Remove(item);
        }
        public void RemoveAt(int index)
        {
            ((IList<PlayerInfo>)_playerData).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<PlayerInfo>)_playerData).GetEnumerator();
        }
    }
}
