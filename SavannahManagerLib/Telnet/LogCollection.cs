using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonExtensionLib.Extensions;

namespace SvManagerLibrary.Telnet
{
    public class StringInfo
    {
        public StringBuilder Text { get; set; } = new StringBuilder();
        public bool EndLine { get; set; }

        public override string ToString() => Text.ToString();
    }

    public class LogCollection : IEnumerable<StringInfo>
    {
        private readonly LinkedList<StringInfo> _list = new LinkedList<StringInfo>();

        public void Append(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            text = text.UnifiedBreakLine();

            var array = (from x in text.Split('\n') select x + "\n").ToList();
            if (text[text.Length - 1] != '\n')
                array[array.Count - 1] = array[array.Count - 1].TrimEnd('\n');
            else
                array.RemoveAt(array.Count - 1);

            foreach (var item in array.Select((v, i) => new { Index = i, Value = v }))
            {
                var builder = GetStringBuilder();
                builder.Text.Append(item.Value.TrimEnd('\n'));

                if (item.Value[item.Value.Length - 1] == '\n')
                    builder.EndLine = true;
            }
        }

        private StringInfo GetStringBuilder()
        {
            if (_list.Count <= 0)
            {
                var builder = new StringInfo();
                _list.AddLast(builder);
                return builder;
            }

            var last = _list.Last.Value;
            if (last.EndLine)
            {
                var builder = new StringInfo();
                _list.AddLast(builder);
                return builder;
            }

            return _list.Last.Value;

        }

        public string GetFirst()
        {
            var info = _list.First?.Value;
            if (info != null && info.EndLine)
            {
                _list.RemoveFirst();
                return info.ToString();
            }

            return string.Empty;
        }

        public string GetFirstNoneRemove()
        {
            var info = _list.First?.Value;
            if (info != null && info.EndLine)
            {
                return info.ToString();
            }

            return string.Empty;
        }

        public string GetLastNoneRemove()
        {
            var info = _list.Last?.Value;
            if (info != null && info.EndLine)
            {
                return info.ToString();
            }

            return string.Empty;
        }

        public IEnumerable<StringInfo> ReversEnumerable()
        {
            var reverse = new List<StringInfo>(this._list);
            reverse.Reverse();
            return reverse;
        }

        public IEnumerator<StringInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var stringInfo in _list)
            {
                var end = stringInfo.EndLine ? "\n" : "";
                sb.Append($"{stringInfo}{end}");
            }

            return sb.ToString();
        }
    }
}
