using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommonCoreLib.CommonLinq;
using Prism.Mvvm;
using Reactive.Bindings;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.Models.TreeView
{
    public class TreeViewItemInfo : BindableBase
    {
        public string Name { get; set; }
        public TreeViewItemInfo[] Children { get; set; }
        private bool isExpanded;
        private bool isSelected;

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }
        public string Path { get; }

        public CommonXmlNode Node { get; }

        public TreeViewItemInfo(CommonXmlNode root, string parentPath = null)
        {
            bool.TryParse(root.GetAttribute(MainWindowModel.XML_EXPANDED).Value, out var isExpanded);
            IsExpanded = isExpanded;
            root.RemoveAttribute(MainWindowModel.XML_EXPANDED);

            Node = root;
            Path = string.IsNullOrEmpty(parentPath) ? $"/{root.TagName}" : $"{parentPath}/{root.TagName}";
            Name = Conditions.IfElse(root.Attributes.Any(), () => $"{root.TagName} {root.Attributes.ToAttributesText()}"
                , () => $"{root.TagName}");

            Children = (from node in root.ChildNodes
                select new TreeViewItemInfo(node, Path)).ToArray();
        }

        public static string GetName(TreeViewItemInfo info)
        {
            var root = info.Node;
            return Conditions.IfElse(root.Attributes.Any(), () => $"{root.TagName} {root.Attributes.ToAttributesText()}"
                , () => $"{root.TagName}");
        }
    }
}
