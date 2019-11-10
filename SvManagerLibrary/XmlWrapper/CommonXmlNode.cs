using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonCoreLib.Bool;

namespace SvManagerLibrary.XmlWrapper
{
    public static class CommonXmlNodeExtension
    {
        public static string ToAttributesText(this IEnumerable<AttributeInfo> attributeInfos)
        {
            return string.Join(", ", attributeInfos);
        }
    }

    public class CommonXmlNode
    {
        #region Properties
        public string TagName { get; set; }
        public IEnumerable<AttributeInfo> Attributes
        {
            get => attributes.Values;
            set => attributes = value.ToDictionary((attr) => attr.Name);
        }
        public IEnumerable<CommonXmlNode> ChildNodes
        {
            get => childNodes;
            set => childNodes = new List<CommonXmlNode>(value);
        }
        public CommonXmlText InnerText { get; set; } = new CommonXmlText();
        #endregion

        #region Fields
        private Dictionary<string, AttributeInfo> attributes = new Dictionary<string, AttributeInfo>();
        private List<CommonXmlNode> childNodes = new List<CommonXmlNode>();
        #endregion

        #region Member Methods
        public AttributeInfo GetAttribute(string name)
        {
            if (attributes.ContainsKey(name))
                return attributes[name];
            return new AttributeInfo();
        }

        public CommonXmlNode CreateChildElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<CommonXmlNode> commonXmlNodes = null, string innerText = "")
        {
            var node = CreateElement(tagName, attributeInfos, commonXmlNodes, innerText);
            childNodes.Add(CreateElement(tagName, attributeInfos, commonXmlNodes, innerText));
            return node;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            var attr = string.Join(" ", from x in Attributes select x.ToString());

            sb.Append($"<{TagName} {attr}>{InnerText}</{TagName}>");

            return sb.ToString();
        }
        #endregion

        #region Static Methods
        public static CommonXmlNode CreateRoot(string tagName)
        {
            var root = new CommonXmlNode
            {
                TagName = tagName
            };
            return root;
        }

        public static CommonXmlNode CreateElement(string tagName, IEnumerable<AttributeInfo> attributeInfos = null
            , IEnumerable<CommonXmlNode> commonXmlNodes = null, string innerText = "")
        {
            if (attributeInfos == null)
                attributeInfos = new AttributeInfo[0];

            if (commonXmlNodes == null)
                commonXmlNodes = new CommonXmlNode[0];

            var node = new CommonXmlNode
            {
                TagName = tagName,
                Attributes = attributeInfos,
                ChildNodes = commonXmlNodes,
                InnerText = new CommonXmlText() { Text = innerText }
            };
            return node;
        }
        #endregion

        #region Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var element = (CommonXmlNode)obj;
            var boolcollector = new BoolCollector();

            boolcollector.ChangeBool(TagName, TagName == element.TagName);
            boolcollector.ChangeBool(Attributes, Attributes.SequenceEqual(element.Attributes));
            boolcollector.ChangeBool(ChildNodes, ChildNodes.SequenceEqual(element.ChildNodes));
            boolcollector.ChangeBool(InnerText, InnerText.Equals(element.InnerText));

            return boolcollector.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = 2061855513;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TagName);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<AttributeInfo>>.Default.GetHashCode(Attributes);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<CommonXmlNode>>.Default.GetHashCode(ChildNodes);
            hashCode = hashCode * -1521134295 + EqualityComparer<CommonXmlText>.Default.GetHashCode(InnerText);
            return hashCode;
        }
        #endregion
    }
}
