using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_XmlEditor.Models.TreeView;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.Models.NodeView
{
    public class CommonModel : ModelBase
    {
        public string FullPath
        {
            get => fullPath;
            set => SetProperty(ref fullPath, value);
        }

        public ObservableCollection<AttributeInfo> Attributes
        {
            get => attributes;
            set => SetProperty(ref attributes, value);
        }

        public string InnerXml
        {
            get => innerXml;
            set => SetProperty(ref innerXml, value);
        }


        private string fullPath;
        private ObservableCollection<AttributeInfo> attributes = new ObservableCollection<AttributeInfo>();
        private string innerXml;

        public void ChangeItem(TreeViewItemInfo info)
        {
            FullPath = info.Path;
            Attributes.Clear();
            Attributes.AddAll(info.Node.Attributes);
            InnerXml = info.Node.InnerText.Xml;
        }
    }
}
