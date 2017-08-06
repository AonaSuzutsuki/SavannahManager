using System.IO;
using System.Xml;

namespace SvManagerLibrary.XMLWrapper
{
    /// <summary>
    /// Write to a file as XML Document.
    /// </summary>
    public class Writer
    {
        private XmlDocument xDocument = new XmlDocument();
        private XmlProcessingInstruction xDeclaration;
        private XmlElement xRoot;

        public Writer()
        {
            xDeclaration = xDocument.CreateProcessingInstruction("xml", "version=\"1.0\"");
        }
        
        /// <summary>
        /// Set a root.
        /// </summary>
        /// <param name="rootName">Root name</param>
        public void SetRoot(string rootName)
        {
            xRoot = xDocument.CreateElement(rootName);
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeInfos">Attribute informations.</param>
        /// <param name="value">Attribute value.</param>
        public void AddElement(string elementName, AttributeInfo[] attributeInfos, string value = "")
        {
            XmlElement xmeta = xDocument.CreateElement(elementName);
            foreach (AttributeInfo attributeInfo in attributeInfos)
                xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);
            xmeta.InnerText = value;

            xRoot.AppendChild(xmeta);
        }

        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeInfo">Attribute information.</param>
        /// <param name="value">Attribute value.</param>
        public void AddElement(string elementName, AttributeInfo attributeInfo, string value = "")
        {
            XmlElement xmeta = xDocument.CreateElement(elementName);
            xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);
            xmeta.InnerText = value;

            xRoot.AppendChild(xmeta);
        }

        /// <summary>
        /// Write to a file as XML Dcoument.
        /// </summary>
        public void Write(string xmlPath)
        {
            FileStream fs = new FileStream(xmlPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            Write(fs);
            fs.Dispose();
        }
        /// <summary>
        /// Write to a file as XML Dcoument.
        /// </summary>
        public void Write(FileStream fileStream)
        {
            //宣言の追加
            xDocument.AppendChild(xDeclaration);
            //ServerSettingsの追加
            xDocument.AppendChild(xRoot);
            
            xDocument.Save(fileStream);
        }
    }
}
