using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace SvManagerLibrary.XMLWrapper
{
    /// <summary>
    /// Read from a file as XML Document.
    /// </summary>
    public class Reader
    {
        public string XmlPath { get; } = string.Empty;

        private readonly XmlDocument document = new XmlDocument();

        public Reader(string xmlPath, bool isFile = true)
        {
            if (isFile)
            {
                XmlPath = xmlPath;
                document.Load(xmlPath);
            }
            else
            {
                document.LoadXml(xmlPath);
            }
        }
        public Reader(Stream stream)
        {
            document.Load(stream);
        }

        public List<string> GetAttributes(string attribute, string xpath, bool isContainNoValue = false)
        {
            var values = new List<string>();

            // /items/item/property/property[@name='DegradationMax']
            var nodeList = document.SelectNodes(xpath);
            foreach (var xmlNode in nodeList)
            {
                var attr = (xmlNode as XmlElement).GetAttribute(attribute);
                if (isContainNoValue)
                {
                    values.Add(attr);
                }
                else
                {
                    if (!string.IsNullOrEmpty(attr))
                        values.Add(attr);
                }
            }

            return values;
        }
        public string GetAttribute(string attribute, string xpath)
        {
            var attributes = GetAttributes(attribute, xpath);
            return attributes.Count < 1 ? string.Empty : attributes[0];
        }

        public List<string> GetValues(string xpath, bool enableLineBreak = true, bool isRemoveSpace = true)
        {
            var values = new List<string>();

            var nodeList = document.SelectNodes(xpath);
            foreach (var xmlNode in nodeList)
            {
                var value = (xmlNode as XmlElement).InnerText;
                if (isRemoveSpace)
                    value = RemoveSpace(value, enableLineBreak);
                values.Add(value);
            }

            return values;
        }
        public string GetValue(string xpath, bool enableLineBreak = true, bool isRemoveSpace = true)
        {
            var values = GetValues(xpath, enableLineBreak, isRemoveSpace);
            return values.Count < 1 ? default : values[0];
        }

        private static string RemoveSpace(string text, bool isAddLine = false)
        {
            var sb = new StringBuilder();

            text = text.Replace("\r\n", "\r").Replace("\r", "\n").TrimStart('\n');
            var spaceLength = GetSpaceLength(text);

            var expression = spaceLength > 0 ? $"^( {{0,{spaceLength.ToString()}}})(?<text>.*)$" : "^ *(?<text>.*)$";
            var reg = new Regex(expression);
            using (var sr = new StringReader(text))
            {
                while (sr.Peek() > -1)
                {
                    var line = sr.ReadLine() ?? string.Empty;

                    var match = reg.Match(line);
                    if (match.Success)
                    {
                        if (isAddLine)
                            sb.Append($"{match.Groups["text"].Value}\n");
                        else
                            sb.Append(match.Groups["text"].Value);
                    }
                    else
                    {
                        sb.Append(sr.ReadLine());
                    }
                }
                return sb.ToString().TrimStart('\n').TrimEnd('\n');
            }
        }

        private static int GetSpaceLength(string text)
        {
            const string expression = "^(?<space>[\\s\\t]*)(?<text>.*)$";
            var reg = new Regex(expression);
            var textArray = text.Split('\n');
            if (textArray.Length > 0)
            {
                var match = reg.Match(textArray[0]);
                if (match.Success)
                    return match.Groups["space"].Value.Length;
            }

            return 0;
        }
    }
}
