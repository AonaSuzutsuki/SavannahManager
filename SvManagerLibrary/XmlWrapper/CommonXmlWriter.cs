using System.IO;
using System.Text;
using System.Xml;
using CommonExtensionLib.Extensions;

namespace SvManagerLibrary.XMLWrapper
{
    public class CommonXmlWriter
    {
        private XmlDocument xDocument = new XmlDocument();
        private XmlProcessingInstruction xDeclaration;

        public CommonXmlWriter()
        {
            xDeclaration = xDocument.CreateProcessingInstruction("xml", "version=\"1.0\"");
        }

        public void Write(string path, CommonXmlNode root)
        {
            using var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            Write(fs, root);
        }

        public void Write(Stream stream, CommonXmlNode root)
        {
            using var ms = new MemoryStream();

            var elem = CreateXmlElement(root);
            xDocument.AppendChild(xDeclaration);
            xDocument.AppendChild(elem);

            xDocument.Save(ms);

            ms.Seek(0, SeekOrigin.Begin);
            using var sr = new StreamReader(ms, Encoding.UTF8);
            while (sr.Peek() > -1)
            {
                var text = $"{ConvertStringReference(sr.ReadLine())}\r\n".UnifiedBreakLine();
                var data = Encoding.UTF8.GetBytes(text);
                stream.Write(data, 0, data.Length);
            }
        }

        private XmlElement CreateXmlElement(CommonXmlNode root)
        {
            var elem = xDocument.CreateElement(root.TagName);

            if (root.Attributes.Length > 0)
                foreach (AttributeInfo attributeInfo in root.Attributes)
                    elem.SetAttribute(attributeInfo.Name, attributeInfo.Value);
            if (!string.IsNullOrEmpty(root.InnerText.Text))
                elem.InnerText = root.InnerText.Text;

            if (root.ChildNode.Length > 0)
                foreach (var child in root.ChildNode)
                    elem.AppendChild(CreateXmlElement(child));

            return elem;
        }

        private string ConvertStringReference(string text)
        {
            return text.Replace("&#xD;", "\r").Replace("&#xA;", "\n");
        }
    }
}
