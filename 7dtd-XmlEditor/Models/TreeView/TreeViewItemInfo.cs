using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CommonCoreLib.CommonLinq;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SavannahXmlLib.Extensions;
using SavannahXmlLib.XmlWrapper;

namespace _7dtd_XmlEditor.Models.TreeView
{
    public class TreeViewItemInfo : TreeViewItemInfoBase
    {
        #region Constants

        public const string XmlId = "savannah.xml.id";
        public const string XmlSelected = "savannah.selected";
        public const string XmlExpanded = "savannah.expanded";

        #endregion

        #region Event

        private readonly Subject<TreeViewItemInfo> failedLostFocus = new Subject<TreeViewItemInfo>();
        public IObservable<TreeViewItemInfo> FailedLostFocus => failedLostFocus;

        #endregion

        private string tagName = string.Empty;

        private bool isTextBoxFocus;

        private Visibility textBlockVisibility = Visibility.Visible;
        private Visibility textBoxVisibility = Visibility.Collapsed;

        public IEditedModel EditedModel { get; set; }

        public bool IsRoot { get; set; }

        public string TagName
        {
            get => tagName;
            set => SetProperty(ref tagName, value);
        }

        public new TreeViewItemInfo Parent
        {
            get => (TreeViewItemInfo) base.Parent;
            set => base.Parent = value;
        }

        public bool IgnoreAttributeRedraw { get; set; }
        public bool IsEdited { get; set; }

        public bool IsTextBoxFocus
        {
            get => isTextBoxFocus;
            set => SetProperty(ref isTextBoxFocus, value);
        }

        public string ParentPath => Parent == null ? "/" : $"{Parent.ParentPath}{Parent.TagName}/";
        public string Path => $"{ParentPath}{Node.TagName}";

        public SavannahXmlNode Node { get; }


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


        public TreeViewItemInfo(SavannahXmlNode root, IEditedModel editedModel, TreeViewItemInfo parent = null)
        {
            bool.TryParse(root.GetAttribute(XmlExpanded).Value, out var isExpanded);
            IsExpanded = isExpanded;
            root.RemoveAttribute(XmlExpanded);

            Node = root;
            Parent = parent;
            TagName = Node.TagName;
            Name = GetNodeName(Node);
            EditedModel = editedModel;

            children = new ObservableCollection<TreeViewItemInfoBase>(from node in Node.ChildNodes
                select new TreeViewItemInfo(node, editedModel, this));

            TextBoxLostFocus = new DelegateCommand(() =>
            {
                if (DisableTextEdit())
                    ApplyTagChange();
                else
                    failedLostFocus.OnNext(this);
                failedLostFocus.OnCompleted();
            });
        }

        public void EnableTextEdit()
        {
            if (Node.NodeType != XmlNodeType.Tag)
                return;

            TextBlockVisibility = Visibility.Collapsed;
            TextBoxVisibility = Visibility.Visible;
            IsTextBoxFocus = true;
        }
        public bool DisableTextEdit()
        {
            if (string.IsNullOrEmpty(TagName))
                return false;

            TextBlockVisibility = Visibility.Visible;
            TextBoxVisibility = Visibility.Collapsed;

            return true;
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

            EditedModel.IsEdited = true;
            Node.TagName = TagName;
            Name = GetNodeName(Node);
            EditedModel.FullPath = Path;
        }

        public IEnumerable<TreeViewItemInfo> GetChildrenEnumerable()
        {
            foreach (var info in children)
            {
                yield return info as TreeViewItemInfo;
            }
        }

        public static string GetNodeName(SavannahXmlNode node) => Conditions.IfElse(node.Attributes.Any(),
            () => $"{node.TagName} {node.Attributes.ToAttributesText(", ")}" , () => $"{node.TagName}");

        public static string GetName(TreeViewItemInfo info)
        {
            var root = info.Node;
            return Conditions.IfElse(root.Attributes.Any(), () => $"{root.TagName} {root.Attributes.ToAttributesText(", ")}"
                , () => $"{root.TagName}");
        }
    }
}
