using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using _7dtd_XmlEditor.Models.TreeView;
using CommonCoreLib;
using CommonExtensionLib.Extensions;
using CommonStyleLib.File;
using CommonStyleLib.Models;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace _7dtd_XmlEditor.Models
{
    public class MainWindowModel : ModelBase, IEditedModel
    {
        #region Constants

        public const string XmlId = "savannah.xml.id";
        public const string XmlSelected = "savannah.selected";
        public const string XmlExpanded = "savannah.expanded";

        #endregion

        #region Fields

        private string _declaration;

        private string _openedFilePath = string.Empty;
        private string _isEditedTitle;
        private bool _isEdited;

        private TreeViewItemInfo _root;
        private ObservableCollection<TreeViewItemInfo> _treeViewItems = new ObservableCollection<TreeViewItemInfo>();
        private TreeViewItemInfo _selectedItem;

        private string _fullPath;
        private bool _isAttributesEnabled;
        private ObservableCollection<ViewAttributeInfo> _attributes = new ObservableCollection<ViewAttributeInfo>();
        private ViewAttributeInfo _attributesSelectedItem;
        private string _innerXml;
        private bool _contextMenuEnabled;
        private bool _addElementEnabled;
        #endregion

        #region Properties

        public string IsEditedTitle
        {
            get => _isEditedTitle;
            set => SetProperty(ref _isEditedTitle, value);
        }

        public bool IsEdited
        {
            get => _isEdited;
            set
            {
                _isEdited = value;
                IsEditedTitle = value ? "*" : "";
            }
        }

        public ObservableCollection<TreeViewItemInfo> TreeViewItems
        {
            get => _treeViewItems;
            set => SetProperty(ref _treeViewItems, value);
        }

        public TreeViewItemInfo SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }


        public string FullPath
        {
            get => _fullPath;
            set => SetProperty(ref _fullPath, value);
        }

        public bool IsAttributesEnabled
        {
            get => _isAttributesEnabled;
            set => SetProperty(ref _isAttributesEnabled, value);
        }

        public ObservableCollection<ViewAttributeInfo> Attributes
        {
            get => _attributes;
            set => SetProperty(ref _attributes, value);
        }

        public ViewAttributeInfo AttributesSelectedItem
        {
            get => _attributesSelectedItem;
            set => SetProperty(ref _attributesSelectedItem, value);
        }

        public string InnerXml
        {
            get => _innerXml;
            set => SetProperty(ref _innerXml, value);
        }

        public bool ContextMenuEnabled
        {
            get => _contextMenuEnabled;
            set => SetProperty(ref _contextMenuEnabled, value);
        }

        public bool AddElementEnabled
        {
            get => _addElementEnabled;
            set => SetProperty(ref _addElementEnabled, value);
        }
        #endregion

        public void NewFile()
        {
            TreeViewItems.Clear();
            Attributes.Clear();
            InnerXml = string.Empty;
            _openedFilePath = string.Empty;
            IsEdited = false;

            _declaration = SavannahXmlConstants.Utf8Declaration;
            _root = new TreeViewItemInfo(new SavannahTagNode { TagName = "root" }, this)
            {
                IsRoot = true
            };
            TreeViewItems.Add(_root);
        }

        public void OpenFile()
        {
            var filePath = FileSelector.GetFilePath(AppInfo.GetAppPath(), "XML Files(*.xml)|*.xml|All Files(*.*)|*.*"
                , "", FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(filePath))
            {
                var (dec, itemInfo) = OpenFile(filePath, this);
                _declaration = dec;
                _root = itemInfo;
                TreeViewItems.Clear();
                TreeViewItems.Add(itemInfo);

                IsEdited = false;
            }
            _openedFilePath = filePath;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(_openedFilePath))
            {
                SaveAs();
            }
            else
            {
                if (!IsEdited)
                    return;

                SaveFile(_openedFilePath, _declaration, _root.Node);
            }

            IsEdited = false;
        }

        public void SaveAs()
        {
            var filePath = FileSelector.GetFilePath(AppInfo.GetAppPath(), "XML Files(*.xml)|*.xml|All Files(*.*)|*.*",
                "", FileSelector.FileSelectorType.Write);

            if (!string.IsNullOrEmpty(filePath))
                SaveFile(filePath, _declaration, _root.Node);
            _openedFilePath = filePath;
            IsEdited = false;
        }



        public void ItemChanged()
        {
            var info = SelectedItem;
            if (info == null)
                return;

            FullPath = info.Path;
            if (!info.IgnoreAttributeRedraw && info.Node is SavannahTagNode tagNode)
            {
                Attributes.Clear();
                Attributes.AddRange(from attribute in tagNode.Attributes
                    select new ViewAttributeInfo
                    {
                        Attribute = new AttributeInfo { Name = attribute.Name, Value = attribute.Value },
                        LostFocusAction = LostFocus
                    });
            }
            else
            {
                info.IgnoreAttributeRedraw = false;
            }

            if (info.Node is SavannahTagNode tag)
                InnerXml = !string.IsNullOrEmpty(tag.PrioritizeInnerXml) ? tag.PrioritizeInnerXml : tag.InnerXml;
            else
                InnerXml = info.Node.InnerText;

            IsAttributesEnabled = !(info.Node is SavannahTextNode);
        }

        public void LostFocus(ViewAttributeInfo info)
        {
            if (info.IsEdited)
                Apply(true);
        }

        public void AddChildElement()
        {
            var info = SelectedItem;
            if (info == null || !(info.Node is SavannahTagNode currentNode))
                return;

            var node = new SavannahTagNode();
            var prevChildren = currentNode.ChildNodes;
            var currentNodeChildNodes = prevChildren.ToList();
            var children = new List<AbstractSavannahXmlNode>(currentNodeChildNodes)
            {
                node
            };
            currentNode.ChildNodes = children;

            var newInfo = new TreeViewItemInfo(node, this, info);
            newInfo.FailedLostFocus.Subscribe(failedInfo =>
            {
                currentNode.ChildNodes = currentNodeChildNodes;
                info.RemoveChildren(failedInfo);
            });

            info.AddChildren(newInfo);
            info.IsExpanded = true;
            info.IsSelected = false;
            newInfo.IsSelected = true;

            newInfo.EnableTextEdit();
        }

        public void RemoveElement()
        {
            var info = SelectedItem;
            if (info == null || !(info.Parent.Node is SavannahTagNode parentNode))
                return;

            var savannahXmlNodes = new List<AbstractSavannahXmlNode>(parentNode.ChildNodes);
            savannahXmlNodes.Remove(info.Node);
            parentNode.ChildNodes = savannahXmlNodes;
            info.Parent.RemoveChildren(info);
            IsEdited = true;
        }

        public void AddAttribute()
        {
            var last = Attributes.LastOrDefault();
            if (last == null || !string.IsNullOrEmpty(last.Attribute.Name))
            {
                var attr = new ViewAttributeInfo() { LostFocusAction = LostFocus };
                Attributes.Add(attr);
            }
        }

        public void RemoveAttribute()
        {
            if (AttributesSelectedItem != null)
            {
                Attributes.Remove(AttributesSelectedItem);
                AttributesSelectedItem = null;
                Apply();
            }
        }

        public void ApplyInnerXml()
        {
            if (SelectedItem == null)
                return;

            if (SelectedItem.IsEdited)
            {
                try
                {
                    Apply();
                }
                catch (System.Xml.XmlException)
                {
                    if (SelectedItem == null)
                        return;
                    SelectedItem.Background = Brushes.Red;
                    _root.RemoveReservedAttributesIncludedChildren();
                }
            }
        }

        public void Apply(bool ignoreAttributeRedraw = false)
        {
            var node = SelectedItem?.Node;
            if (node is SavannahTagNode tagNode)
            {
                if (InnerXml != node.InnerXml)
                    tagNode.PrioritizeInnerXml = InnerXml;
                tagNode.Attributes = from attribute in Attributes
                    where !string.IsNullOrEmpty(attribute.Attribute.Name)
                    select new AttributeInfo { Name = attribute.Attribute.Name, Value = attribute.Attribute.Value };
                tagNode.AppendAttribute(XmlSelected, true.ToString());
            }
            else if (node is SavannahTextNode)
            {
                node.InnerText = InnerXml;
            }

            _root.AssignExpanded();

            using var ms = new MemoryStream();
            var writer = new SavannahXmlWriter(_declaration)
            {
                IgnoreComments = false
            };
            writer.Write(ms, _root.Node as SavannahTagNode);

            ms.Seek(0, SeekOrigin.Begin);
            var reader = new SavannahXmlReader(ms, false);
            var readNode = reader.GetAllNodes();

            _root = new TreeViewItemInfo(readNode, this)
            {
                IsRoot = true
            };

            TreeViewItems.Clear();
            TreeViewItems.Add(_root);

            var item = GetSelectedInfo(_root);
            if (item != null)
            {
                item.IgnoreAttributeRedraw = ignoreAttributeRedraw;
                item.IsEdited = false;
                SelectedItem = item;
            }

            IsEdited = true;
            //OnItemApplied(new CommonModel.ItemAppliedEventArgs { ItemInfo = root });

            //SelectionChange();
        }

        

        private TreeViewItemInfo GetSelectedInfo(TreeViewItemInfo info)
        {
            var node = info.Node;
            if (!(node is SavannahTagNode tagNode))
                return info;

            bool.TryParse(tagNode.GetAttribute(XmlSelected)?.Value, out var isSelected);
            tagNode.RemoveAttribute(XmlSelected);
            info.Name = TreeViewItemInfo.GetName(info);
            if (isSelected)
            {
                info.IsSelected = true;
                return info;
            }

            if (info.Children.Any())
            {
                foreach (var treeViewItemInfo in info.GetChildrenEnumerable())
                {
                    var retInfo = GetSelectedInfo(treeViewItemInfo);
                    if (retInfo != null)
                        return retInfo;
                }
            }

            return null;
        }

        private static (string declaration, TreeViewItemInfo root) OpenFile(string filePath, IEditedModel editedModel)
        {
            var reader = new SavannahXmlReader(filePath, false);
            var declaration = reader.Declaration;
            var root = new TreeViewItemInfo(reader.GetAllNodes(), editedModel)
            {
                IsRoot = true
            };

            return (declaration, root);
        }

        private static void SaveFile(string filePath, string declaration, AbstractSavannahXmlNode node)
        {
            if (!(node is SavannahTagNode tagNode))
                return;

            var writer = new SavannahXmlWriter(declaration)
            {
                IgnoreComments = false
            };
            Console.WriteLine(node.InnerXml);
            writer.Write(filePath, tagNode);
        }
    }
}
