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
    public class TreeViewItemInfo : BindableBase
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

        private string name = string.Empty;
        private string tagName = string.Empty;

        private bool isExpanded;
        private bool isSelected;
        private bool isTextBoxFocus;

        private readonly ObservableCollection<TreeViewItemInfo> children;

        private Brush background = Brushes.Transparent;

        private Visibility textBlockVisibility = Visibility.Visible;
        private Visibility textBoxVisibility = Visibility.Collapsed;
        private Visibility beforeSeparatorVisibility = Visibility.Hidden;
        private Visibility afterSeparatorVisibility = Visibility.Hidden;

        public IEditedModel EditedModel { get; set; }

        public bool IsRoot { get; set; }

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

        public IEnumerable<TreeViewItemInfo> Children => children;

        public bool IgnoreAttributeRedraw { get; set; }
        public bool IsEdited { get; set; }

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

        public bool IsTextBoxFocus
        {
            get => isTextBoxFocus;
            set => SetProperty(ref isTextBoxFocus, value);
        }

        public Brush Background
        {
            get => background;
            set => SetProperty(ref background, value);
        }
        public string ParentPath => Parent == null ? "/" : $"{Parent.ParentPath}{Parent.TagName}/";
        public string Path => $"{ParentPath}{Node.TagName}";

        public TreeViewItemInfo Parent { get; set; }
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

        public Visibility BeforeSeparatorVisibility
        {
            get => beforeSeparatorVisibility;
            set => SetProperty(ref beforeSeparatorVisibility, value);
        }

        public Visibility AfterSeparatorVisibility
        {
            get => afterSeparatorVisibility;
            set => SetProperty(ref afterSeparatorVisibility, value);
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

            children = new ObservableCollection<TreeViewItemInfo>(from node in Node.ChildNodes
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

        public void AddChildren(TreeViewItemInfo info)
        {
            children.Add(info);
        }

        public void RemoveChildren(TreeViewItemInfo info)
        {
            children.Remove(info);
        }

        public void InsertBeforeChildren(TreeViewItemInfo from, TreeViewItemInfo to)
        {
            var index = children.IndexOf(to);
            if (index < 0)
                return;

            children.Insert(index, from);
        }

        public void InsertAfterChildren(TreeViewItemInfo from, TreeViewItemInfo to)
        {
            var index = children.IndexOf(to);
            if (index < 0)
                return;

            children.Insert(index + 1, from);
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
