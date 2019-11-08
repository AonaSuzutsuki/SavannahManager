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
            var nodeList = document.SelectNodes(xpath).ToList();
            var cond = Conditions.If<IList<string>>(() => isContaisNoValue)
                .Then(() => (from node in nodeList
                             let attr = (node as XmlElement).GetAttribute(name)
                             select attr).ToList())
                .Else(() => (from node in nodeList
                             let attr = (node as XmlElement).GetAttribute(name)
                             where !string.IsNullOrEmpty(attr)
                             select attr).ToList());
            return cond.Invoke();
        }

        public CommonXmlNode GetNode(string xpath)
        {
            var node = document.SelectSingleNode(xpath);
            return new CommonXmlNode
            {
                TagName = node.Name,
                InnerText = ResolveInnerText(node),
                Attributes = ConvertAttributeInfoArray(node.Attributes),
                ChildNodes = GetElements(node.ChildNodes)
            };
        }

        public CommonXmlNode[] GetNodes(string xpath)
        {
            var nodeList = ConvertXmlNode(document.SelectNodes(xpath));
            var list = from node in nodeList
                       select new CommonXmlNode
                       {
                           TagName = node.Name,
                           InnerText = ResolveInnerText(node),
                           Attributes = ConvertAttributeInfoArray(node.Attributes),
                           ChildNodes = GetElements(node.ChildNodes)
                       };
            return list.ToArray();
        }

        public CommonXmlNode GetAllNodes()
        {
            var nodeList = document.SelectSingleNode("/*");
            var root = new CommonXmlNode
            {
                TagName = nodeList.Name,
                InnerText = ResolveInnerText(nodeList),
                Attributes = ConvertAttributeInfoArray(nodeList.Attributes),
                ChildNodes = GetElements(nodeList.ChildNodes).ToArray()
            };
            return root;
        }

        private CommonXmlText ResolveInnerText(XmlNode node)
        {
            var xml = node.InnerXml;
            if (xml.Contains("<") || xml.Contains(">"))
                return new CommonXmlText();
            return new CommonXmlText { Text = node.InnerText };
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
                        commonXmlNode.ChildNodes = GetElements(node.ChildNodes).ToArray();
                    list.Add(commonXmlNode);
                }
            }
            return list;
        }
    }
}
