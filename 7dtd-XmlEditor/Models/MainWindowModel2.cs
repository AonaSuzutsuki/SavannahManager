using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_XmlEditor.Models.TreeView;
using CommonCoreLib;
using CommonExtensionLib.Extensions;
using CommonStyleLib.File;
using CommonStyleLib.Models;
using SavannahXmlLib.XmlWrapper;

namespace _7dtd_XmlEditor.Models
{
    public class MainWindowModel2 : ModelBase, IEditedModel
    {
        #region Constants

        public const string XML_ID = "savannah.xml.id";
        public const string XML_SELECTED = "savannah.selected";
        public const string XML_EXPANDED = "savannah.expanded";

        #endregion

        #region Fields

        private string declaration;

        private string openedFilePath = string.Empty;
        private string isEditedTitle;
        private bool isEdited;

        private TreeViewItemInfo root;
        private ObservableCollection<TreeViewItemInfo> treeViewItems = new ObservableCollection<TreeViewItemInfo>();
        private TreeViewItemInfo selectedItem;

        private string fullPath;
        private bool isAttributesEnabled;
        private ObservableCollection<ViewAttributeInfo> attributes = new ObservableCollection<ViewAttributeInfo>();
        private ViewAttributeInfo attributesSelectedItem;
        private string innerXml;
        private bool contextMenuEnabled;
        private bool _addElementEnabled;
        #endregion

        #region Properties

        public string IsEditedTitle
        {
            get => isEditedTitle;
            set => SetProperty(ref isEditedTitle, value);
        }

        public bool IsEdited
        {
            get => isEdited;
            set
            {
                isEdited = value;
                IsEditedTitle = value ? "*" : "";
            }
        }

        public ObservableCollection<TreeViewItemInfo> TreeViewItems
        {
            get => treeViewItems;
            set => SetProperty(ref treeViewItems, value);
        }

        public TreeViewItemInfo SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }


        public string FullPath
        {
            get => fullPath;
            set => SetProperty(ref fullPath, value);
        }

        public bool IsAttributesEnabled
        {
            get => isAttributesEnabled;
            set => SetProperty(ref isAttributesEnabled, value);
        }

        public ObservableCollection<ViewAttributeInfo> Attributes
        {
            get => attributes;
            set => SetProperty(ref attributes, value);
        }

        public ViewAttributeInfo AttributesSelectedItem
        {
            get => attributesSelectedItem;
            set => SetProperty(ref attributesSelectedItem, value);
        }

        public string InnerXml
        {
            get => innerXml;
            set => SetProperty(ref innerXml, value);
        }

        public bool ContextMenuEnabled
        {
            get => contextMenuEnabled;
            set => SetProperty(ref contextMenuEnabled, value);
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
            openedFilePath = string.Empty;
            IsEdited = false;

            declaration = CommonXmlWriter.Utf8Declaration;
            root = new TreeViewItemInfo(new CommonXmlNode { TagName = "root" }, this)
            {
                IsRoot = true
            };
            TreeViewItems.Add(root);
        }

        public void OpenFile()
        {
            var filePath = FileSelector.GetFilePath(AppInfo.GetAppPath(), "XML Files(*.xml)|*.xml|All Files(*.*)|*.*"
                , "", FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(filePath))
            {
                var (dec, itemInfo) = OpenFile(filePath, this);
                declaration = dec;
                root = itemInfo;
                TreeViewItems.Clear();
                TreeViewItems.Add(itemInfo);

                IsEdited = false;
            }
            openedFilePath = filePath;
        }

        public void Save()
        {
            if (string.IsNullOrEmpty(openedFilePath))
            {
                SaveAs();
            }
            else
            {
                if (!IsEdited)
                    return;

                SaveFile(openedFilePath, declaration, root.Node);
            }

            IsEdited = false;
        }

        public void SaveAs()
        {
            var filePath = FileSelector.GetFilePath(AppInfo.GetAppPath(), "XML Files(*.xml)|*.xml|All Files(*.*)|*.*",
                "", FileSelector.FileSelectorType.Write);

            if (!string.IsNullOrEmpty(filePath))
                SaveFile(filePath, declaration, root.Node);
            openedFilePath = filePath;
            IsEdited = false;
        }



        public void ItemChanged()
        {
            var info = SelectedItem;
            if (info == null)
                return;

            FullPath = info.Path;
            if (!info.IgnoreAttributeRedraw)
            {
                Attributes.Clear();
                Attributes.AddAll(from attribute in info.Node.Attributes
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

            InnerXml = info.Node.NodeType == XmlNodeType.Tag ? info.Node.InnerXml : info.Node.InnerText;

            IsAttributesEnabled = info.Node.NodeType != XmlNodeType.Text;
        }

        public void LostFocus(ViewAttributeInfo info)
        {
            if (info.IsEdited)
                Apply(true);
        }

        public void AddChildElement()
        {
            var info = SelectedItem;
            if (info == null)
                return;

            var currentNode = info.Node;
            var node = new CommonXmlNode();
            var prevChildren = currentNode.ChildNodes;
            var currentNodeChildNodes = prevChildren.ToList();
            var children = new List<CommonXmlNode>(currentNodeChildNodes)
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
            if (info == null)
                return;

            var parent = info.Parent;
            var commonXmlNodes = new List<CommonXmlNode>(parent.Node.ChildNodes);
            commonXmlNodes.Remove(info.Node);
            parent.Node.ChildNodes = commonXmlNodes;
            info.Parent.RemoveChildren(info);
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
                Apply();
        }

        public void Apply(bool ignoreAttributeRedraw = false)
        {
            var node = SelectedItem.Node;
            if (node.NodeType == XmlNodeType.Tag && InnerXml != node.InnerXml)
                node.PrioritizeInnerXml = InnerXml;
            else if (node.NodeType == XmlNodeType.Text)
                node.InnerText = InnerXml;

            node.Attributes = from attribute in Attributes
                where !string.IsNullOrEmpty(attribute.Attribute.Name)
                select new AttributeInfo { Name = attribute.Attribute.Name, Value = attribute.Attribute.Value };

            var info = SelectedItem;
            AssignExpanded(root);
            info.Node.AppendAttribute(XML_SELECTED, true.ToString());

            using var ms = new MemoryStream();
            var writer = new CommonXmlWriter(declaration)
            {
                IgnoreComments = false
            };
            writer.Write(ms, root.Node);

            ms.Seek(0, SeekOrigin.Begin);
            var reader = new CommonXmlReader(ms, false);
            var readNode = reader.GetAllNodes();

            root = new TreeViewItemInfo(readNode, this)
            {
                IsRoot = true
            };

            TreeViewItems.Clear();
            TreeViewItems.Add(root);

            var item = GetSelectedInfo(root);
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

        private void AssignExpanded(TreeViewItemInfo info)
        {
            var node = info.Node;
            if (info.IsExpanded)
                node.AppendAttribute(XML_EXPANDED, true.ToString());
            foreach (var child in info.Children)
            {
                AssignExpanded(child);
            }
        }

        private TreeViewItemInfo GetSelectedInfo(TreeViewItemInfo info)
        {
            var node = info.Node;
            bool.TryParse(node.GetAttribute(XML_SELECTED).Value, out var isSelected);
            node.RemoveAttribute(XML_SELECTED);
            info.Name = TreeViewItemInfo.GetName(info);
            if (isSelected)
            {
                info.IsSelected = true;
                return info;
            }

            if (info.Children.Any())
            {
                foreach (var treeViewItemInfo in info.Children)
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
            var reader = new CommonXmlReader(filePath, false);
            var declaration = reader.Declaration;
            var root = new TreeViewItemInfo(reader.GetAllNodes(), editedModel)
            {
                IsRoot = true
            };

            return (declaration, root);
        }

        private static void SaveFile(string filePath, string declaration, CommonXmlNode node)
        {
            var writer = new CommonXmlWriter(declaration)
            {
                IgnoreComments = false
            };
            Console.WriteLine(node.InnerXml);
            writer.Write(filePath, node);
        }
    }
}
