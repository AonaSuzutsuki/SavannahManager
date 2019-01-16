using SvManagerLibrary.XMLWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConfigFileMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            var writer = new Writer();
            writer.SetRoot("ServerSettings");

            var text = GetInnerXml(File.ReadAllText("serverconfig.xml"));
            var regex = new Regex("^( |\\t)*<property( |\\t)+name=\"(?<name>.*)\"( |\\t)+value=\"(?<value>.*)\"( |\\t)*\\/>( |\\t)*<!--(?<description>.*)-->",
                RegexOptions.Multiline);
            var match = regex.Match(text);
            while (match.Success)
            {
                var name = match.Groups["name"].ToString();
                var value = match.Groups["value"].ToString();
                var description = match.Groups["description"].ToString().TrimStart(' ').TrimEnd(' ');

                var attributes = new AttributeInfo[]
                {
                    new AttributeInfo() { Name = "name", Value = name },
                    new AttributeInfo() { Name = "value", Value = value },
                    new AttributeInfo() { Name = "selection" },
                    new AttributeInfo() { Name = "type" }
                };
                writer.AddElement("property", attributes, description);

                match = match.NextMatch();
            }

            var memory = new MemoryStream();
            writer.Write(memory);
            Console.WriteLine(Encoding.UTF8.GetString(memory.ToArray()));

            Console.ReadLine();
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
