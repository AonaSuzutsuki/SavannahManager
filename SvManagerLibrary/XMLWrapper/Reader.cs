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

        public Reader(string xmlPath)
        {
            XmlPath = xmlPath;
            document.Load(xmlPath);
        }
        public Reader(Stream stream)
        {
            document.Load(stream);
        }

        public List<string> GetAttributes(string attribute, string xpath)
        {
            var values = new List<string>();

            // /items/item/property/property[@name='DegradationMax']
            var nodeList = document.SelectNodes(xpath);
            foreach (var xmlNode in nodeList)
                values.Add((xmlNode as XmlElement).GetAttribute(attribute));

            return values;
        }
        public string GetAttribute(string attribute, string xpath)
        {
            return GetAttributes(attribute, xpath)[0];
        }

        public List<string> GetValues(string xpath)
        {
            var values = new List<string>();

            var nodeList = document.SelectNodes(xpath);
            foreach (var xmlNode in nodeList)
            {
                string value = (xmlNode as XmlElement).InnerText;
                value = RemoveSpace(value, true);
                values.Add(value);
            }

            return values;
        }
        public string GetValue(string xpath)
        {
            return GetValues(xpath)[0];
        }

        private static string RemoveSpace(string text, bool isAddLine = false)
        {
            var sb = new StringBuilder();

            const string expression = "^ *(?<text>.*?)$";
            var reg = new Regex(expression);
            var sr = new StringReader(text);
            while (sr.Peek() > -1)
            {
                var match = reg.Match(sr.ReadLine());
                if (match.Success)
                {
                    if (isAddLine)
                        sb.AppendLine(match.Groups["text"].Value);
                    else
                        sb.Append(match.Groups["text"].Value);
                }
                else
                {
                    sb.Append(sr.ReadLine());
                }
            }

            return sb.ToString();
        }
    }
}
