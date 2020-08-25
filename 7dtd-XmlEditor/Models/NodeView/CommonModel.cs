using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_XmlEditor.Models.TreeView;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;
using SavannahXmlLib.XmlWrapper;

namespace _7dtd_XmlEditor.Models.NodeView
{
    public interface ICommonModel
    {
        event CommonModel.ItemAppliedEventHandler ItemApplied;
        IEditedModel EditedModel { get; set; }
        TreeViewItemInfo SelectedItem { get; }
        string Declaration { get; set; }
        string FullPath { get; set; }
        void SetRoot(TreeViewItemInfo info);
        void NewFile();
        void ChangeItem(TreeViewItemInfo info);
    }

    public class CommonModel : ModelBase, ICommonModel
    {
        public const string XML_ID = "savannah.xml.id";
        public const string XML_SELECTED = "savannah.selected";
        public const string XML_EXPANDED = "savannah.expanded";

        #region Events
        public class ItemAppliedEventArgs : EventArgs
        {
            public TreeViewItemInfo ItemInfo { get; set; }
        }

        public delegate void ItemAppliedEventHandler(object sender, ItemAppliedEventArgs eventArgs);

        public event ItemAppliedEventHandler ItemApplied;

        public void OnItemApplied(ItemAppliedEventArgs e)
        {
            ItemApplied?.Invoke(this, e);
        }
        #endregion

        #region Properties
        public IEditedModel EditedModel { get; set; }

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

        public bool AddRootEnabled
        {
            get => addRootEnabled;
            set => SetProperty(ref addRootEnabled, value);
        }
        #endregion

        #region Fields

        public string Declaration { get; set; }

        private TreeViewItemInfo root;
        private ObservableCollection<TreeViewItemInfo> treeViewItems = new ObservableCollection<TreeViewItemInfo>();
        private TreeViewItemInfo selectedItem;

        private string fullPath;
        private bool isAttributesEnabled;
        private ObservableCollection<ViewAttributeInfo> attributes = new ObservableCollection<ViewAttributeInfo>();
        private ViewAttributeInfo attributesSelectedItem;
        private string innerXml;
        private CommonXmlNode node;

        private bool contextMenuEnabled;
        private bool addRootEnabled;
        #endregion


        public CommonModel()
        {
            
        }

        public void SetRoot(TreeViewItemInfo info)
        {
            root = info;

            TreeViewItems.Add(root);
        }

        public void NewFile()
        {
            var commonXmlNode = new CommonXmlNode
            {
                TagName = "root"
            };
            root = new TreeViewItemInfo(commonXmlNode, this);
            TreeViewItems.Clear();
            TreeViewItems.Add(root);
        }

        public void SelectionChange()
        {
            var info = SelectedItem;
            if (info == null)
                return;

            ChangeItem(info);
        }

        public void ChangeItem(TreeViewItemInfo info)
        {
            if (info == null)
                return;

            node = info.Node;

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

        public void LostFocus(ViewAttributeInfo attributeInfo)
        {
            if (attributeInfo.isEdited)
                Apply(true);
        }

        public void AddAttribute()
        {
            var last = Attributes.LastOrDefault();
            if (last == null || !string.IsNullOrEmpty(last.Attribute.Name))
            {
                var attr = new ViewAttributeInfo() { LostFocusAction = LostFocus };
                Attributes.Add(attr);
                //node.AppendAttribute(attr);
            }
        }

        public void RemoveAttribute()
        {
            if (AttributesSelectedItem != null)
            {
                Attributes.Remove(AttributesSelectedItem);
                AttributesSelectedItem = null;
                Apply();
                //node.RemoveAttribute(AttributesSelectedItem);
            }
        }

        public void ApplyInnerXml()
        {
            if (SelectedItem.IsEdited)
                Apply();
        }

        public void Apply(bool ignoreAttributeRedraw = false)
        {
            if (this.node.NodeType == XmlNodeType.Tag && InnerXml != this.node.InnerXml)
                this.node.PrioritizeInnerXml = InnerXml;
            else if (this.node.NodeType == XmlNodeType.Text)
                this.node.InnerText = InnerXml;

            this.node.Attributes = from attribute in Attributes where !string.IsNullOrEmpty(attribute.Attribute.Name)
                select new AttributeInfo {Name = attribute.Attribute.Name, Value = attribute.Attribute.Value};

            var info = SelectedItem;
            AssignExpanded(root);
            info.Node.AppendAttribute(XML_SELECTED, true.ToString());

            using var ms = new MemoryStream();
            var writer = new CommonXmlWriter(Declaration);
            writer.Write(ms, root.Node);

            ms.Seek(0, SeekOrigin.Begin);
            var reader = new CommonXmlReader(ms);
            var node = reader.GetAllNodes();

            root = new TreeViewItemInfo(node, this);

            TreeViewItems.Clear();
            TreeViewItems.Add(root);

            var item = GetSelectedInfo(root);
            if (item != null)
            {
                item.IgnoreAttributeRedraw = ignoreAttributeRedraw;
                item.IsEdited = false;
                SelectedItem = item;
            }

            OnItemApplied(new ItemAppliedEventArgs { ItemInfo = root});

            //SelectionChange();
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

        //private TreeViewItemInfo GetSelectedNode(TreeViewItemInfo info, long targetId)
        //{
        //    TreeViewItemInfo retInfo = null;
        //    var node = info.Node;
        //    long.TryParse(node.GetAttribute(XML_ID).Value, out var id);
        //    node.RemoveAttribute(XML_ID);
        //    if (targetId == id)
        //        retInfo = info;

        //    if (info.Children.Any())
        //    {
        //        foreach (var nodeChildNode in info.Children)
        //        {
        //            var retNode = GetSelectedNode(nodeChildNode, targetId);
        //            if (retNode != null)
        //                retInfo = retNode;
        //        }
        //    }

        //    return retInfo;
        //}

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

        private long AssignId(CommonXmlNode node, long id)
        {
            node.AppendAttribute(XML_ID, (id++).ToString());
            if (node.ChildNodes.Any())
            {
                foreach (var childNode in node.ChildNodes)
                {
                    id = AssignId(childNode, id);
                }
            }

            return id;
        }
    }
}
