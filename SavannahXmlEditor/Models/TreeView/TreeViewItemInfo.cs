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
using CommonStyleLib.Views.Behaviors.TreeViewBehavior;
using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using SavannahXmlLib.Extensions;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace _7dtd_XmlEditor.Models.TreeView
{
    /// <summary>
    /// Represents a single item in a tree view structure for XML editing.
    /// Manages state, editing, and reserved attribute handling for XML nodes.
    /// </summary>
    public class TreeViewItemInfo : TreeViewItemInfoBase
    {
        #region Constants

        /// <summary>
        /// Reserved attribute key for XML node ID.
        /// </summary>
        public const string XmlId = "savannah.xml.id";
        /// <summary>
        /// Reserved attribute key for selected state.
        /// </summary>
        public const string XmlSelected = "savannah.selected";
        /// <summary>
        /// Reserved attribute key for expanded state.
        /// </summary>
        public const string XmlExpanded = "savannah.expanded";

        #endregion

        #region Event

        private readonly Subject<TreeViewItemInfo> _failedLostFocus = new();
        /// <summary>
        /// Observable event triggered when focus loss fails during text editing.
        /// </summary>
        public IObservable<TreeViewItemInfo> FailedLostFocus => _failedLostFocus;

        #endregion

        private string _tagName = string.Empty;
        private bool _isTextBoxFocus;
        private Visibility _textBlockVisibility = Visibility.Visible;
        private Visibility _textBoxVisibility = Visibility.Collapsed;

        /// <summary>
        /// Gets or sets the edited model associated with this item.
        /// </summary>
        public IEditedModel EditedModel { get; set; }

        /// <summary>
        /// Gets or sets whether this item is the root of the tree.
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        /// Gets or sets the tag name of the XML node.
        /// </summary>
        public string TagName
        {
            get => _tagName;
            set => SetProperty(ref _tagName, value);
        }

        /// <summary>
        /// Gets or sets the parent item in the tree.
        /// </summary>
        public new TreeViewItemInfo Parent
        {
            get => (TreeViewItemInfo)base.Parent;
            set => base.Parent = value;
        }

        /// <summary>
        /// Gets or sets whether attribute redraw should be ignored.
        /// </summary>
        public bool IgnoreAttributeRedraw { get; set; }
        /// <summary>
        /// Gets or sets whether this item is currently edited.
        /// </summary>
        public bool IsEdited { get; set; }

        /// <summary>
        /// Gets or sets whether the text box is focused for editing.
        /// </summary>
        public bool IsTextBoxFocus
        {
            get => _isTextBoxFocus;
            set => SetProperty(ref _isTextBoxFocus, value);
        }

        /// <summary>
        /// Gets the path of the parent node.
        /// </summary>
        public string ParentPath => Parent == null ? "/" : $"{Parent.ParentPath}{Parent.TagName}/";
        /// <summary>
        /// Gets the full path of this node.
        /// </summary>
        public string Path => $"{ParentPath}{Node.TagName}";

        /// <summary>
        /// Gets the underlying XML node.
        /// </summary>
        public AbstractSavannahXmlNode Node { get; }

        /// <summary>
        /// Gets or sets the visibility of the text block (display mode).
        /// </summary>
        public Visibility TextBlockVisibility
        {
            get => _textBlockVisibility;
            set => SetProperty(ref _textBlockVisibility, value);
        }
        /// <summary>
        /// Gets or sets the visibility of the text box (edit mode).
        /// </summary>
        public Visibility TextBoxVisibility
        {
            get => _textBoxVisibility;
            set => SetProperty(ref _textBoxVisibility, value);
        }

        /// <summary>
        /// Gets or sets the command executed when the text box loses focus.
        /// </summary>
        public ICommand TextBoxLostFocus { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewItemInfo"/> class.
        /// </summary>
        /// <param name="root">The root XML node.</param>
        /// <param name="editedModel">The edited model instance.</param>
        /// <param name="parent">The parent tree view item, if any.</param>
        public TreeViewItemInfo(AbstractSavannahXmlNode root, IEditedModel editedModel, TreeViewItemInfo parent = null)
        {
            if (root is SavannahTagNode tagRoot)
            {
                RemoveReservedAttributes(tagRoot);
                Children = new ObservableCollection<ITreeViewItemInfoBase>(from node in tagRoot.ChildNodes
                                                                           select new TreeViewItemInfo(node, editedModel, this));
            }

            Node = root;
            Parent = parent;
            TagName = Node.TagName;
            Name = GetNodeName(Node);
            EditedModel = editedModel;

            TextBoxLostFocus = new DelegateCommand(() =>
            {
                if (DisableTextEdit())
                    ApplyTagChange();
                else
                    _failedLostFocus.OnNext(this);
                _failedLostFocus.OnCompleted();
            });
        }

        /// <summary>
        /// Removes reserved attributes (expanded, selected) from the specified tag node and updates state.
        /// </summary>
        /// <param name="tagNode">The tag node to process.</param>
        public void RemoveReservedAttributes(SavannahTagNode tagNode)
        {
            _ = bool.TryParse(tagNode.GetAttribute(XmlExpanded)?.Value, out var isExpanded);
            _ = bool.TryParse(tagNode.GetAttribute(XmlSelected)?.Value, out var isSelected);
            IsExpanded = isExpanded;
            IsSelected = isSelected;
            tagNode.RemoveAttribute(XmlExpanded);
            tagNode.RemoveAttribute(XmlSelected);
        }

        /// <summary>
        /// Recursively removes reserved attributes from this item and all child items.
        /// </summary>
        /// <param name="info">The root item to start from. If null, uses this item.</param>
        public void RemoveReservedAttributesIncludedChildren(TreeViewItemInfo info = null)
        {
            info ??= this;

            if (info.Node is not SavannahTagNode tagNode)
                return;

            info.RemoveReservedAttributes(tagNode);
            foreach (var child in info.GetChildrenEnumerable())
            {
                RemoveReservedAttributesIncludedChildren(child);
            }
        }

        /// <summary>
        /// Recursively assigns the expanded state to XML attributes for this item and all children.
        /// </summary>
        /// <param name="info">The root item to start from. If null, uses this item.</param>
        public void AssignExpanded(TreeViewItemInfo info = null)
        {
            info ??= this;

            var node = info.Node;
            if (node is SavannahTagNode tagNode && info.IsExpanded)
                tagNode.AppendAttribute(XmlExpanded, true.ToString());
            foreach (var child in info.GetChildrenEnumerable())
            {
                AssignExpanded(child);
            }
        }

        /// <summary>
        /// Enables text editing mode for this item.
        /// </summary>
        public void EnableTextEdit()
        {
            if (Node is not SavannahTagNode)
                return;

            TextBlockVisibility = Visibility.Collapsed;
            TextBoxVisibility = Visibility.Visible;
            IsTextBoxFocus = true;
        }

        /// <summary>
        /// Disables text editing mode for this item.
        /// </summary>
        /// <returns>True if editing can be disabled; otherwise, false.</returns>
        public bool DisableTextEdit()
        {
            if (string.IsNullOrEmpty(TagName))
                return false;

            TextBlockVisibility = Visibility.Visible;
            TextBoxVisibility = Visibility.Collapsed;

            return true;
        }

        /// <summary>
        /// Applies changes to the tag name if it was modified during editing.
        /// </summary>
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

        /// <summary>
        /// Gets an enumerable collection of child <see cref="TreeViewItemInfo"/> items.
        /// </summary>
        /// <returns>Enumerable of child items.</returns>
        public IEnumerable<TreeViewItemInfo> GetChildrenEnumerable()
        {
            if (Children == null)
                yield break;
            foreach (var info in Children)
            {
                yield return info as TreeViewItemInfo;
            }
        }

        /// <summary>
        /// Gets the display name for the specified XML node.
        /// </summary>
        /// <param name="node">The XML node.</param>
        /// <returns>The display name string.</returns>
        public static string GetNodeName(AbstractSavannahXmlNode node)
        {
            if (node is not SavannahTagNode tagNode)
                return $"{node.TagName}";

            return tagNode.Attributes.Any() ? $"{node.TagName} {tagNode.Attributes.ToAttributesText(", ")}" : $"{node.TagName}";
        }

        /// <summary>
        /// Gets the display name for the specified tree view item.
        /// </summary>
        /// <param name="info">The tree view item.</param>
        /// <returns>The display name string.</returns>
        public static string GetName(TreeViewItemInfo info)
        {
            var root = info.Node;
            return GetNodeName(root);
        }
    }
}