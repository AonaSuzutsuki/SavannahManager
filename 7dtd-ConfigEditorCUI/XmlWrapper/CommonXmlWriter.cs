using System.IO;
using System.Xml;

namespace _7dtd_ConfigEditorCUI.XMLWrapper
{
    public class CommonXmlWriter2
    {

    }

    public class CommonXmlElement
    {
        private readonly XmlElement element;

        public CommonXmlElement(XmlElement elem)
        {
            element = elem;
        }

        public void Append(CommonXmlElement elem)
        {
            element.AppendChild(elem.element);
        }

        public void Append(params CommonXmlElement[] elements)
        {
            foreach (var elem in elements)
                Append(elem);
        }

        public void AppendToXmlDocument(XmlDocument xmlDocument)
        {
            xmlDocument.AppendChild(element);
        }
    }

    /// <summary>
    /// Write to a file as XML Document.
    /// </summary>
    public class CommonXmlWriter
    {
        private XmlDocument xDocument = new XmlDocument();
        private XmlProcessingInstruction xDeclaration;

        public CommonXmlWriter()
        {
            xDeclaration = xDocument.CreateProcessingInstruction("xml", "version=\"1.0\"");
        }
        
        /// <summary>
        /// Set a root.
        /// </summary>
        /// <param name="rootName">Root name</param>
        public CommonXmlElement CreateRoot(string rootName)
        {
            return new CommonXmlElement(xDocument.CreateElement(rootName));
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeInfos">Attribute informations.</param>
        /// <param name="value">Attribute value.</param>
        public CommonXmlElement CreateElement(string elementName, AttributeInfo[] attributeInfos, string value = null)
        {
            XmlElement xmeta = xDocument.CreateElement(elementName);
            foreach (AttributeInfo attributeInfo in attributeInfos)
                xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);
            if (!string.IsNullOrEmpty(value))
                xmeta.InnerText = value;

            return new CommonXmlElement(xmeta);
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeInfo">Attribute information.</param>
        /// <param name="value">Attribute value.</param>
        public CommonXmlElement CreateElement(string elementName, AttributeInfo attributeInfo, string value = null)
        {
            XmlElement xmeta = xDocument.CreateElement(elementName);
            xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);
            if (!string.IsNullOrEmpty(value))
                xmeta.InnerText = value;

            return new CommonXmlElement(xmeta);
        }
        public CommonXmlElement CreateElement(string elementName, string value = null)
        {
            XmlElement xmeta = xDocument.CreateElement(elementName);
            if (!string.IsNullOrEmpty(value))
                xmeta.InnerText = value;

            return new CommonXmlElement(xmeta);
        }

        public CommonXmlElement CreateElement(string elementName, AttributeInfo attributeInfo, CommonXmlElement value)
        {
            XmlElement xmeta = xDocument.CreateElement(elementName);
            xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);

            var commonXmlElem = new CommonXmlElement(xmeta);
            commonXmlElem.Append(value);

            return commonXmlElem;
        }

        public CommonXmlElement CreateElement(string elementName, CommonXmlElement value)
        {
            XmlElement xmeta = xDocument.CreateElement(elementName);

            var commonXmlElem = new CommonXmlElement(xmeta);
            commonXmlElem.Append(value);

            return commonXmlElem;
        }

        /// <summary>
        /// Write to a file as XML Dcoument.
        /// </summary>
        public void Write(string xmlPath, CommonXmlElement root)
        {
            var fs = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            Write(fs, root);
            fs.Dispose();
        }
        /// <summary>
        /// Write to a file as XML Dcoument.
        /// </summary>
        public void Write(Stream stream, CommonXmlElement root)
        {
            xDocument.AppendChild(xDeclaration);
            root.AppendToXmlDocument(xDocument);
            
            xDocument.Save(stream);
        }
    }
}
