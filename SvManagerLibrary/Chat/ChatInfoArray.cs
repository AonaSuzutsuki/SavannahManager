using System.Collections;
using System.Collections.Generic;

namespace SvManagerLibrary.Chat
{
    public class ChatInfoArray : IList<ChatInfo>
    {
        readonly List<ChatInfo> _chatData = new List<ChatInfo>();

        public ChatInfo this[int index]
        {
            get
            {
                return ((IList<ChatInfo>)_chatData)[index];
            }

            set
            {
                ((IList<ChatInfo>)_chatData)[index] = value;
            }
        }

        public void Add(ChatInfo item)
        {
            ((IList<ChatInfo>)_chatData).Add(item);
        }
        public void Add(string log)
        {
            _chatData.Add(ChatInfoConverter.ConvertChat(log));
        }
        public ChatInfo GetLast()
        {
            return this[Count - 1];
        }

        public int Count
        {
            get
            {
                return ((IList<ChatInfo>)_chatData).Count;
            }
        }
        public bool IsReadOnly
        {
            get
            {
                return ((IList<ChatInfo>)_chatData).IsReadOnly;
            }
        }
        public void Clear()
        {
            ((IList<ChatInfo>)_chatData).Clear();
        }
        public bool Contains(ChatInfo item)
        {
            return ((IList<ChatInfo>)_chatData).Contains(item);
        }
        public void CopyTo(ChatInfo[] array, int arrayIndex)
        {
            ((IList<ChatInfo>)_chatData).CopyTo(array, arrayIndex);
        }
        public IEnumerator<ChatInfo> GetEnumerator()
        {
            return ((IList<ChatInfo>)_chatData).GetEnumerator();
        }
        public int IndexOf(ChatInfo item)
        {
            return ((IList<ChatInfo>)_chatData).IndexOf(item);
        }
        public void Insert(int index, ChatInfo item)
        {
            ((IList<ChatInfo>)_chatData).Insert(index, item);
        }
        public bool Remove(ChatInfo item)
        {
            return ((IList<ChatInfo>)_chatData).Remove(item);
        }
        public void RemoveAt(int index)
        {
            ((IList<ChatInfo>)_chatData).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<ChatInfo>)_chatData).GetEnumerator();
        }
    }
}
