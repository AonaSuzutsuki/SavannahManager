using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using _7dtd_XmlEditor.Models.NodeView;
using _7dtd_XmlEditor.ViewModels.NodeView;
using CommonCoreLib.CommonLinq;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SavannahXmlLib.XmlWrapper;

namespace _7dtd_XmlEditor.Models.TreeView
{
    public class TreeViewItemInfo : BindableBase
    {
        public IEditedModel EditedModel { get; set; }
        public ICommonModel ParentCommonModel { get; set; }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string TagName
        {
            get => tagName;
            set => SetProperty(ref tagName, value);
        }
        public TreeViewItemInfo[] Children { get; set; }

        public bool IgnoreAttributeRedraw { get; set; }
        public bool IsEdited { get; set; }

        private string name = string.Empty;
        private string tagName = string.Empty;


        private bool isExpanded;
        private bool isSelected;

        private Visibility textBlockVisibility = Visibility.Visible;
        private Visibility textBoxVisibility = Visibility.Collapsed;

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

        public string ParentPath => Parent == null ? "/" : $"{Parent.ParentPath}{Parent.TagName}/";
        public string Path => $"{ParentPath}{Node.TagName}";

        public TreeViewItemInfo Parent { get; }
        public CommonXmlNode Node { get; }


        public Visibility TextBlockVisibility
        {
            get => textBlockVisibility;
            set => SetProperty(ref textBlockVisibility, value);
        }
        public Visibility TextBoxVisibility
        {
            get => textBoxVisibility;
            set => SetProperty(ref textBoxVisibility, value);
        }

        public ICommand TextBoxLostFocus { get; set; }


        public TreeViewItemInfo(CommonXmlNode root, ICommonModel commonModel, TreeViewItemInfo parent = null)
        {
            bool.TryParse(root.GetAttribute(CommonModel.XML_EXPANDED).Value, out var isExpanded);
            IsExpanded = isExpanded;
            root.RemoveAttribute(CommonModel.XML_EXPANDED);

            Node = root;
            Parent = parent;
            TagName = Node.TagName;
            Name = GetNodeName(Node);
            ParentCommonModel = commonModel;

            Children = (from node in Node.ChildNodes
                select new TreeViewItemInfo(node, commonModel, this)
                {
                    EditedModel = this.EditedModel
                }).ToArray();

            TextBoxLostFocus = new DelegateCommand(() =>
            {
                DisableTextEdit();
                ApplyTagChange();
            });
        }

        public void EnableTextEdit()
        {
            if (Node.NodeType != XmlNodeType.Tag)
                return;

            TextBlockVisibility = Visibility.Collapsed;
            TextBoxVisibility = Visibility.Visible;
        }
        public void DisableTextEdit()
        {
            TextBlockVisibility = Visibility.Visible;
            TextBoxVisibility = Visibility.Collapsed;
        }

        private void ApplyTagChange()
        {
            if (Node.TagName == TagName)
                return;

            if (string.IsNullOrEmpty(TagName))
            {
                TagName = Node.TagName;
                return;
            }

            ParentCommonModel.EditedModel.IsEdited = true;
            Node.TagName = TagName;
            Name = GetNodeName(Node);
            ParentCommonModel.FullPath = Path;
        }

        public static string GetNodeName(CommonXmlNode node) => Conditions.IfElse(node.Attributes.Any(),
            () => $"{node.TagName} {node.Attributes.ToAttributesText()}" , () => $"{node.TagName}");

        public static string GetName(TreeViewItemInfo info)
        {
            var root = info.Node;
            return Conditions.IfElse(root.Attributes.Any(), () => $"{root.TagName} {root.Attributes.ToAttributesText()}"
                , () => $"{root.TagName}");
        }
    }
}
