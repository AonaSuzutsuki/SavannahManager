using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using CommonCoreLib.CommonLinq;
using CommonExtensionLib.Extensions;

namespace SvManagerLibrary.XMLWrapper
{
    public class CommonXmlText
    {
        public string Text { get; internal set; }

        public override string ToString()
        {
            return Text;
        }
    }

    public class CommonXmlNode
    {
        public string TagName { get; internal set; }
        public AttributeInfo[] Attributes { get; internal set; } = new AttributeInfo[0];
        public CommonXmlNode[] ChildNode { get; internal set; } = new CommonXmlNode[0];
        public CommonXmlText InnerText { get; internal set; } = new CommonXmlText();

        public override string ToString()
        {
            var sb = new StringBuilder();
            var attr = string.Join(" ", from x in Attributes select x.ToString());

            sb.Append($"<{TagName} {attr}>{InnerText}</{TagName}>");

            return sb.ToString();
        }

        public static CommonXmlNode CreateRoot(string tagName)
        {
            var root = new CommonXmlNode
            {
                TagName = tagName
            };
            return root;
        }
    }

    public class CommonXmlReader
    {
        public string XmlPath { get; } = string.Empty;

        private readonly XmlDocument document = new XmlDocument();

        public CommonXmlNode Root { get; private set; }

        public CommonXmlReader(string xmlPath)
        {
            XmlPath = xmlPath;

            document.Load(xmlPath);
        }

        public CommonXmlNode GetNode(string xpath)
        {
            var node = document.SelectSingleNode(xpath);
            return new CommonXmlNode
            {
                TagName = node.Name,
                InnerText = new CommonXmlText { Text = node.InnerText },
                Attributes = ConvertAttributeInfoArray(node.Attributes),
                ChildNode = GetElements(node.ChildNodes).ToArray()
            };
        }

        public CommonXmlNode[] GetNodes(string xpath)
        {
            var nodeList = ConvertXmlNode(document.SelectNodes(xpath));
            var list = from node in nodeList
                       select new CommonXmlNode
                       {
                           TagName = node.Name,
                           InnerText = new CommonXmlText { Text = node.InnerText },
                           Attributes = ConvertAttributeInfoArray(node.Attributes),
                           ChildNode = GetElements(node.ChildNodes).ToArray()
                       };
            return list.ToArray();
        }

        public CommonXmlNode GetAllNodes()
        {
            var nodeList = document.SelectSingleNode("/*");
            var root = new CommonXmlNode
            {
                TagName = nodeList.Name,
                InnerText = new CommonXmlText { Text = nodeList.InnerText },
                Attributes = ConvertAttributeInfoArray(nodeList.Attributes),
                ChildNode = GetElements(nodeList.ChildNodes).ToArray()
            };
            return root;
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

        private List<CommonXmlNode> GetElements(XmlNodeList nodeList)
        {
            var list = new List<CommonXmlNode>();
            foreach (var n in nodeList)
            {
                if (n is XmlElement)
                {
                    var node = (XmlElement)n;
                    var commonXmlNode = new CommonXmlNode
                    {
                        TagName = node.Name,
                        InnerText = new CommonXmlText { Text = node.InnerText },
                        Attributes = ConvertAttributeInfoArray(node.Attributes)
                    };
                    if (node.ChildNodes.Count > 0)
                        commonXmlNode.ChildNode = GetElements(node.ChildNodes).ToArray();
                    list.Add(commonXmlNode);
                }
            }
            return list;
        }
    }
}
