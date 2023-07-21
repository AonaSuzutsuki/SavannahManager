using CommonExtensionLib.Extensions;
using SavannahXmlLib.XmlWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SavannahXmlLib.XmlWrapper.Nodes;
using System.Xml.Linq;
using System.ComponentModel.Design;

namespace ConfigFileMaker
{
    class Program
    {
        static void Main()
        {
            var configDict = FirstAnalyze();
            var templateDict = SecondAnalyze();
            var except = RemoveConfigOnlyElement(configDict, templateDict).Select(x => x.Value);

            var writer = new SavannahXmlWriter();
            var root = SavannahTagNode.CreateRoot("ServerSettings");

            foreach (var info in except)
            {
                var attributes = new List<AttributeInfo>
                {
                    new() { Name = "name", Value = info.Property },
                    new() { Name = "value", Value = info.Value },
                    new() { Name = "selection", Value = info.Selection },
                    new() { Name = "type", Value = info.Type }
                };

                var elem = SavannahTagNode.CreateElement("property", attributes.ToArray());
                elem.InnerText = info.Description;
                elem.AddChildElement(SavannahTextNode.CreateTextNode(info.Description));

                root.AddChildElement(elem);
            }

            var memory = new MemoryStream();
            writer.Write(memory, root);
            var xml = Encoding.UTF8.GetString(memory.ToArray());
            Console.WriteLine(xml);

            using var fs = new FileStream("out.xml", FileMode.Create, FileAccess.Write, FileShare.None);
            using var sw = new StreamWriter(fs);
            sw.Write(xml);
            sw.Flush();
        }

        static Dictionary<string, ConfigListInfo> FirstAnalyze()
        {
            var configDict = new Dictionary<string, ConfigListInfo>();

            var text = GetInnerXml(File.ReadAllText("serverconfig.xml"));
            var regex = new Regex("^( |\\t)*<property( |\\t)+name=\"(?<name>.*)\"( |\\t)+value=\"(?<value>.*)\"( |\\t)*\\/>( |\\t)*([\r\n])*( |\t)*<!--(?<description>.*)-->",
                RegexOptions.Multiline);
            var match = regex.Match(text);
            while (match.Success)
            {
                var name = match.Groups["name"].ToString();
                var value = match.Groups["value"].ToString();
                var description = match.Groups["description"].ToString().TrimStart(' ').TrimEnd(' ');

                var selection = "";
                var selectionType = "string";
                if (int.TryParse(value, out _))
                {
                    selectionType = "integer";
                }
                else if (bool.TryParse(value, out _))
                {
                    selectionType = "combo";
                    selection = "true/false";
                }

                var item = new ConfigListInfo
                {
                    Property = name,
                    Value = value,
                    Selection = selection,
                    Type = selectionType,
                    Description = description
                };

                configDict.Add(name, item);

                match = match.NextMatch();
            }

            return configDict;
        }

        static Dictionary<string, ConfigListInfo> SecondAnalyze()
        {
            var templateDict = new Dictionary<string, ConfigListInfo>();

            var reader = new SavannahXmlReader("template.xml");
            var nodes = reader.GetNodes("/ServerSettings/property");

            foreach (var node in nodes)
            {
                if (node is not SavannahTagNode xmlNode)
                    continue;

                var item = new ConfigListInfo
                {
                    Property = xmlNode.GetAttribute("name").Value,
                    Value = xmlNode.GetAttribute("value").Value,
                    Selection = xmlNode.GetAttribute("selection").Value,
                    Type = xmlNode.GetAttribute("type").Value,
                    Description = xmlNode.InnerText
                };

                templateDict.Add(item.Property, item);
            }

            return templateDict;
        }

        static Dictionary<string, ConfigListInfo> RemoveConfigOnlyElement(Dictionary<string, ConfigListInfo> configDict,
            Dictionary<string, ConfigListInfo> templateDict)
        {
            var except = configDict
                .Select(x => new KeyValuePair<string, ConfigListInfo>(x.Key, new ConfigListInfo
                {
                    Property = x.Value.Property,
                    Value = x.Value.Value,
                    Selection = x.Value.Selection,
                    Type = x.Value.Type,
                    Description = templateDict.Get(x.Key)?.Description ?? x.Value.Description,
                }))
                .ToDictionary(x => x.Key, x => x.Value);

            return except;
        }

        static string GetInnerXml(string text)
        {
            text = text.Replace("\r\n", "\r").Replace("\r", "\n");

            var regex = new Regex("<\\?xml version=\"1\\.0\"\\?>\n<ServerSettings>\n(?<value>[\\s\\S]*)\n<\\/ServerSettings>");
            var match = regex.Match(text);
            if (match.Success)
            {
                return match.Groups["value"].ToString();
            }
            return string.Empty;
        }
    }
}
