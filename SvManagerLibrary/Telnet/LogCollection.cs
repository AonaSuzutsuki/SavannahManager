using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonExtensionLib.Extensions;

namespace SvManagerLibrary.Telnet
{
    public class StringInfo
    {
        public StringBuilder Text { get; set; } = new StringBuilder();
        public bool EndLine { get; set; }

        public override string ToString() => Text.ToString();
    }

    public class LogCollection
    {
        private LinkedList<StringInfo> list = new LinkedList<StringInfo>();

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
            if (list.Count <= 0)
            {
                var builder = new StringInfo();
                list.AddLast(builder);
                return builder;
            }

            var last = list.Last.Value;
            if (last.EndLine)
            {
                var builder = new StringInfo();
                list.AddLast(builder);
                return builder;
            }

            return list.Last.Value;

        }

        public string GetFirst()
        {
            var info = list.First?.Value;
            if (info != null && info.EndLine)
            {
                list.RemoveFirst();
                return info.ToString();
            }

            return string.Empty;
        }

        public string GetFirstNoneRemove()
        {
            var info = list.First?.Value;
            if (info != null && info.EndLine)
            {
                return info.ToString();
            }

            return string.Empty;
        }

        public string GetLastNoneRemove()
        {
            var info = list.Last?.Value;
            if (info != null && info.EndLine)
            {
                return info.ToString();
            }

            return string.Empty;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var stringInfo in list)
            {
                var end = stringInfo.EndLine ? "\n" : "";
                sb.Append($"{stringInfo}{end}");
            }

            return sb.ToString();
        }
    }
}
