using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CommonCoreLib.CommonLinq;
using CommonExtensionLib.Extensions;
using SvManagerLibrary.XmlWrapper;

namespace SvManagerLibrary.XmlWrapper
{
    public class CommonXmlReader
    {
        private readonly XmlDocument document = new XmlDocument();

        public CommonXmlNode Root { get; private set; }

        public string Declaration { get; private set; }

        public CommonXmlReader(string xmlPath)
        {
            using var fs = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            Initialize(fs);
        }

        public CommonXmlReader(Stream stream)
        {
            Initialize(stream);
        }

        private void Initialize(Stream stream)
        {
            document.Load(stream);
            var declaration = document.ChildNodes
                                .OfType<XmlDeclaration>()
                                .FirstOrDefault();
            Declaration = declaration.InnerText;
        }

        public IList<string> GetAttributes(string name, string xpath, bool isContaisNoValue = true)
        {
            var nodeList = GetNodes(ConvertXmlNode(document.SelectNodes(xpath)));
            var cond = Conditions.If<IList<string>>(() => isContaisNoValue)
                .Then(() => (from node in nodeList
                             let attr = node.GetAttribute(name).Value
                             select attr).ToList())
                .Else(() => (from node in nodeList
                             let attr = node.GetAttribute(name).Value
                             where !string.IsNullOrEmpty(attr)
                             select attr).ToList());
            return cond.Invoke();
        }

        public IList<string> GetValues(string xpath, bool enableLineBreak = true, bool isRemoveSpace = true)
        {
            var nodeList = GetNodes(ConvertXmlNode(document.SelectNodes(xpath)), isRemoveSpace);
            return (from node in nodeList
                    let text = node.InnerText
                    where !string.IsNullOrEmpty(text) select text).ToList();
        }

        public CommonXmlNode GetNode(string xpath)
        {
            var node = document.SelectSingleNode(xpath);
            return node == null ? null : GetNode(node);
        }

        public CommonXmlNode GetNode(XmlNode node, bool isRemoveSpace = true)
        {
            return new CommonXmlNode
            {
                NodeType = XmlNodeType.Tag,
                TagName = node.Name,
                InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                Attributes = ConvertAttributeInfoArray(node.Attributes),
                ChildNodes = GetElements(node.ChildNodes, isRemoveSpace)
            };
        }

        public CommonXmlNode[] GetNodes(string xpath, bool isRemoveSpace = true)
        {
            var nodeList = ConvertXmlNode(document.SelectNodes(xpath));
            return GetNodes(nodeList, isRemoveSpace);
        }

        public CommonXmlNode[] GetNodes(XmlNode[] nodeList, bool isRemoveSpace = true)
        {
            var list = from node in nodeList
                       select new CommonXmlNode
                       {
                           NodeType = XmlNodeType.Tag,
                           TagName = node.Name,
                           InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                           Attributes = ConvertAttributeInfoArray(node.Attributes),
                           ChildNodes = GetElements(node.ChildNodes, isRemoveSpace)
                       };
            return list.ToArray();
        }

        public CommonXmlNode GetAllNodes(bool isRemoveSpace = true)
        {
            var nodeList = document.SelectSingleNode("/*");
            var root = new CommonXmlNode
            {
                NodeType = XmlNodeType.Tag,
                TagName = nodeList.Name,
                InnerText = ResolveInnerText(nodeList, isRemoveSpace).Text,
                Attributes = ConvertAttributeInfoArray(nodeList.Attributes),
                ChildNodes = GetElements(nodeList.ChildNodes, isRemoveSpace).ToArray()
            };
            return root;
        }

        private static string RemoveSpace(string text, bool isAddLine = false)
        {
            var sb = new StringBuilder();

            text = text.Replace("\r\n", "\r").Replace("\r", "\n").TrimStart('\n');
            var spaceLength = GetSpaceLength(text);

            var expression = spaceLength > 0 ? $"^( {{0,{spaceLength.ToString()}}})(?<text>.*)$" : "^ *(?<text>.*)$";
            var reg = new Regex(expression);
            using var sr = new StringReader(text);
            while (sr.Peek() > -1)
            {
                var line = sr.ReadLine() ?? string.Empty;

                var match = reg.Match(line);
                if (match.Success)
                    if (isAddLine)
                        sb.Append($"{match.Groups["text"].Value}\n");
                    else
                        sb.Append(match.Groups["text"].Value);
                else
                    sb.Append(sr.ReadLine());
            }
            return sb.ToString().TrimStart('\n').TrimEnd('\n');
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

        private CommonXmlText ResolveInnerText(XmlNode node, bool isRemoveSpace)
        {
            var xml = node.InnerXml;
            var xmlText = new CommonXmlText
            {
                Xml = xml.Replace("&#xD;&#xA;", "&#xD;").Replace("&#xD;", "&#xA;").Replace("&#xA;", "\n")
            };

            if (xml.Contains("<") || xml.Contains(">"))
                return xmlText;

            xmlText.Text = Conditions.IfElse(isRemoveSpace, () => RemoveSpace(node.InnerText, true),
                () => node.InnerText).UnifiedBreakLine();
            return xmlText;
        }

        private XmlNode[] ConvertXmlNode(XmlNodeList nodeList)
        {
            if (nodeList == null)
                return null;

            var list = new List<XmlNode>(nodeList.Count);
            foreach (var node in nodeList)
            {
                if (node is XmlNode)
                    list.Add((XmlNode)node);
            }

            return list.ToArray();
        }

        private AttributeInfo[] ConvertAttributeInfoArray(XmlAttributeCollection collection)
        {
            if (collection == null)
                return null;

            var list = new List<XmlAttribute>(collection.Count);
            foreach (var attr in collection)
            {
                if (attr is XmlAttribute)
                    list.Add((XmlAttribute)attr);
            }

            return (from attr in list
                    select new AttributeInfo
                    {
                        Name = attr.Name,
                        Value = attr.Value
                    }).ToArray();
        }

        private List<CommonXmlNode> GetElements(XmlNodeList nodeList, bool isRemoveSpace)
        {
            var list = new List<CommonXmlNode>();
            foreach (var n in nodeList)
            {
                if (n is XmlElement)
                {
                    var node = (XmlElement)n;
                    var commonXmlNode = new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
                        TagName = node.Name,
                        InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                        Attributes = ConvertAttributeInfoArray(node.Attributes)
                    };
                    if (node.ChildNodes.Count > 0)
                        commonXmlNode.ChildNodes = GetElements(node.ChildNodes, isRemoveSpace).ToArray();
                    list.Add(commonXmlNode);
                }

                if (n is XmlText)
                {
                    var node = (XmlText) n;
                    var commonXmlNode = new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Text,
                        TagName = node.Name,
                        InnerText = ResolveInnerText(node, isRemoveSpace).Text,
                    };
                    list.Add(commonXmlNode);
                }
            }
            return list;
        }
    }
}
