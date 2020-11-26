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

namespace ConfigFileMaker
{
    class Program
    {
        static void Main()
        {
            var writer = new SavannahXmlWriter();
            var root = SavannahTagNode.CreateRoot("ServerSettings");

            var text = GetInnerXml(File.ReadAllText("serverconfig.xml"));
            var regex = new Regex("^( |\\t)*<property( |\\t)+name=\"(?<name>.*)\"( |\\t)+value=\"(?<value>.*)\"( |\\t)*\\/>( |\\t)*([\r\n])*( |\t)*<!--(?<description>.*)-->",
                RegexOptions.Multiline);
            var match = regex.Match(text);
            while (match.Success)
            {
                var name = match.Groups["name"].ToString();
                var value = match.Groups["value"].ToString();
                var description = match.Groups["description"].ToString().TrimStart(' ').TrimEnd(' ');

                string selection = "";
                string selectionType = "string";
                if (int.TryParse(value, out _))
                {
                    selectionType = "integer";
                }
                else if (bool.TryParse(value, out _))
                {
                    selectionType = "combo";
                    selection = "true/false";
                }

                var attributes = new List<AttributeInfo>
                {
                    new AttributeInfo() { Name = "name", Value = name },
                    new AttributeInfo() { Name = "value", Value = value },
                    new AttributeInfo() { Name = "selection", Value = selection },
                    new AttributeInfo() { Name = "type", Value = selectionType }
                };
                description = AddDescription(attributes, description);

                var elem = SavannahTagNode.CreateElement("property", attributes.ToArray());
                elem.InnerText = description;
                elem.AddChildElement(SavannahTextNode.CreateTextNode(description));

                root.AddChildElement(elem);

                match = match.NextMatch();
            }

            var memory = new MemoryStream();
            writer.Write(memory, root);
            Console.WriteLine(Encoding.UTF8.GetString(memory.ToArray()));
        }

        static string AddDescription(List<AttributeInfo> attributeInfos, string description)
        {
            var reader = new SavannahXmlReader("template.xml");

            string name = attributeInfos[0].Value;
            var value = reader.GetValue("/ServerSettings/property[@name='{0}']".FormatString(name));
            var selection = reader.GetAttribute("selection", "/ServerSettings/property[@name='{0}']".FormatString(name));
            var type = reader.GetAttribute("type", "/ServerSettings/property[@name='{0}']".FormatString(name));

            attributeInfos[2].Value = selection;
            attributeInfos[3].Value = type;

            if (value != null)
            {
                if (value.EndsWith("\r\n"))
                    value = value.Substring(0, value.Length - 2);
                return value;
            }
            
            return description;
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
