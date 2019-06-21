using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SvManagerLibrary.Chat
{
    public class ChatInfoArray : IList<ChatInfo>
    {
        readonly List<ChatInfo> _chatData = new List<ChatInfo>();

        public ChatInfo this[int index]
        {
            get => _chatData[index];
            set => _chatData[index] = value;
        }

        public void Add(ChatInfo item)
        {
            _chatData.Add(item);
        }
        public void Add(string log)
        {
            _chatData.Add(ChatInfoConverter.ConvertChat(log));
        }
        public void AddMultiLine(string log)
        {
            using (var sr = new StringReader(log))
            {
                while (sr.Peek() > 0)
                {
                    Add(sr.ReadLine());
                }
            }
        }
        public ChatInfo GetLast()
        {
            return this[Count - 1];
        }

        public int Count => _chatData.Count;
        public bool IsReadOnly => ((IList<ChatInfo>)_chatData).IsReadOnly;
        public void Clear()
        {
            _chatData.Clear();
        }
        public bool Contains(ChatInfo item)
        {
            return _chatData.Contains(item);
        }
        public void CopyTo(ChatInfo[] array, int arrayIndex)
        {
            _chatData.CopyTo(array, arrayIndex);
        }
        public IEnumerator<ChatInfo> GetEnumerator()
        {
            return ((IList<ChatInfo>)_chatData).GetEnumerator();
        }
        public int IndexOf(ChatInfo item)
        {
            return _chatData.IndexOf(item);
        }
        public void Insert(int index, ChatInfo item)
        {
            _chatData.Insert(index, item);
        }
        public bool Remove(ChatInfo item)
        {
            return _chatData.Remove(item);
        }
        public void RemoveAt(int index)
        {
            _chatData.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<ChatInfo>)_chatData).GetEnumerator();
        }
    }
}
