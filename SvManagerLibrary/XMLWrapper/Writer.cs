using System.IO;
using System.Xml;

namespace SvManagerLibrary.XMLWrapper
{
    /// <summary>
    /// Write to a file as XML Document.
    /// </summary>
    public class Writer
    {
        private XmlDocument _xDocument = new XmlDocument();
        private XmlElement _xRoot;
        
        /// <summary>
        /// Set a root.
        /// </summary>
        /// <param name="rootName">Root name</param>
        public void SetRoot(string rootName)
        {
            _xRoot = _xDocument.CreateElement(rootName);
        }
        /// <summary>
        /// Add an element.
        /// </summary>
        /// <param name="elementName">Element name.</param>
        /// <param name="attributeInfos">Attribute informations.</param>
        public void AddElement(string elementName, AttributeInfo[] attributeInfos)
        {
            XmlElement xmeta = _xDocument.CreateElement(elementName);
            foreach (AttributeInfo attributeInfo in attributeInfos)
            {
                xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);
            }

            _xRoot.AppendChild(xmeta);
        }
        public void AddElement(string elementName, AttributeInfo[] attributeInfos, string value)
        {
            XmlElement xmeta = _xDocument.CreateElement(elementName);
            foreach (AttributeInfo attributeInfo in attributeInfos)
            {
                xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);
            }
            xmeta.InnerText = value;

            _xRoot.AppendChild(xmeta);
        }
        public void AddElement(string elementName, AttributeInfo attributeInfo)
        {
            XmlElement xmeta = _xDocument.CreateElement(elementName);
            xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);

            _xRoot.AppendChild(xmeta);
        }
        public void AddElement(string elementName, AttributeInfo attributeInfo, string value)
        {
            XmlElement xmeta = _xDocument.CreateElement(elementName);
            xmeta.SetAttribute(attributeInfo.Name, attributeInfo.Value);
            xmeta.InnerText = value;

            _xRoot.AppendChild(xmeta);
        }

        /// <summary>
        /// Write to a file as XML Dcoument.
        /// </summary>
        public void Write(string xmlPath)
        {
            XmlProcessingInstruction xDeclaration = _xDocument.CreateProcessingInstruction("xml", @"version=""1.0""");
            
            //宣言の追加
            _xDocument.AppendChild(xDeclaration);
            //ServerSettingsの追加
            _xDocument.AppendChild(_xRoot);

            FileStream _fs = new FileStream(xmlPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _xDocument.Save(_fs);
            _fs.Dispose();
        }
        /// <summary>
        /// Write to a file as XML Dcoument.
        /// </summary>
        public void Write(FileStream fileStream)
        {
            XmlProcessingInstruction xDeclaration = _xDocument.CreateProcessingInstruction("xml", @"version=""1.0""");

            //宣言の追加
            _xDocument.AppendChild(xDeclaration);
            //ServerSettingsの追加
            _xDocument.AppendChild(_xRoot);
            
            _xDocument.Save(fileStream);
        }
    }
}
