using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class JsonLoader
    {
        private List<Dictionary<string, string>> nodes = new List<Dictionary<string, string>>();

        public JsonLoader(string url, bool localFile = false)
        {
            string json;
            if (localFile)
            {
                using (var fs = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        json = sr.ReadToEnd();
                    }
                }
            }
            else
            {
                var webClient = new WebClient();
                byte[] data = webClient.DownloadData(url);
                webClient.Dispose();
                json = Encoding.UTF8.GetString(data);
            }

            json = DecodeEncodedNonAsciiCharacters(json);

            Decode(json, "root");
        }

        private void Decode(string json, string xpath = "root/item")
        {
            var xmlReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(json), XmlDictionaryReaderQuotas.Max);

            var element = XElement.Load(xmlReader);
            var xmlLoader = new XmlDocument();
            xmlLoader.LoadXml(element.ToString());

            var itemNodes = xmlLoader.SelectNodes(xpath);
            foreach (XmlNode node in itemNodes)
            {
                var dic = new Dictionary<string, string>();
                HandleNode(node, dic);
                nodes.Add(dic);
            }
        }

        private static void HandleNode(XmlNode node, Dictionary<string, string> dic)
        {
            if (node.HasChildNodes)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    HandleNode(child, dic);
                }
            }
            else
            {
                var name = node.ParentNode.LocalName;
                var value = node.InnerText;

                if (!dic.ContainsKey(name))
                    dic.Add(name, value);
            }
        }
        private string DecodeEncodedNonAsciiCharacters(string value)
        {
            return Regex.Replace(value, @"\\u(?<Value>[a-zA-Z0-9]{4})", m => 
                ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString()
            );
        }

        public string GetValue(int index, string key)
        {
            if (nodes.Count < index || index < 0) return null;
            var dic = nodes[index];
            foreach (var pair in dic)
            {
                if (pair.Key.Equals(key)) return pair.Value;
            }
            return null;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var dic in nodes)
            {
                foreach (var pair in dic)
                {
                    sb.AppendFormat("{0} : {1}\r\n", pair.Key, pair.Value);
                }
                sb.Append("----------\r\n");
            }
            return sb.ToString();
        }
    }
}
