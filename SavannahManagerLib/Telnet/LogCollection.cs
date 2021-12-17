using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonExtensionLib.Extensions;

namespace SvManagerLibrary.Telnet
{
    /// <summary>
    /// Log string info.
    /// </summary>
    public class StringInfo
    {
        /// <summary>
        /// Log text.
        /// </summary>
        public StringBuilder Text { get; set; } = new StringBuilder();

        /// <summary>
        /// Whether the log was retrieved to the end or not.
        /// </summary>
        public bool EndLine { get; set; }

        /// <summary>
        /// Text to string.
        /// </summary>
        /// <returns>Text.</returns>
        public override string ToString() => Text.ToString();
    }

    /// <summary>
    /// Keeps the log in line with the newline.
    /// </summary>
    public class LogCollection : IEnumerable<StringInfo>
    {
        private readonly LinkedList<StringInfo> _list = new LinkedList<StringInfo>();

        /// <summary>
        /// Append log text.
        /// </summary>
        /// <param name="text">The text to be appended.</param>
        public void Append(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            text = text.UnifiedBreakLine();

            var array = (from x in text.Split('\n') select x + "\n").ToList();
            if (text.EndsWith("\n"))
                array[array.Count - 1] = array[array.Count - 1].TrimEnd('\n');
            else
                array.RemoveAt(array.Count - 1);

            foreach (var item in array.Select((v, i) => new { Index = i, Value = v }))
            {
                var builder = GetStringBuilder();
                var value = item.Value;
                if (value != "\n")
                    builder.Text.Append(item.Value.TrimEnd('\n'));
                else
                    builder.Text.Append("\n");

                if (item.Value.EndsWith("\n"))
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

        /// <summary>
        /// Get first log. Once retrieved, the element will be deleted.
        /// </summary>
        /// <returns>A first log.</returns>
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

        /// <summary>
        /// Get first log.
        /// </summary>
        /// <returns>A first log.</returns>
        public string GetFirstNoneRemove()
        {
            var info = _list.First?.Value;
            if (info != null && info.EndLine)
            {
                return info.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Get last log.
        /// </summary>
        /// <returns>A last log.</returns>
        public string GetLastNoneRemove()
        {
            var info = _list.Last?.Value;
            if (info != null && info.EndLine)
            {
                return info.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Get last log. Once retrieved, the element will be deleted.
        /// </summary>
        /// <returns>A last log.</returns>
        public IEnumerable<StringInfo> ReversEnumerable()
        {
            var reverse = new List<StringInfo>(this._list);
            reverse.Reverse();
            return reverse;
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        public IEnumerator<StringInfo> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        /// <summary>
        /// GetEnumerator
        /// </summary>
        /// <returns>IEnumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Get the all logs included breakline.
        /// </summary>
        /// <returns>all logs.</returns>
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
