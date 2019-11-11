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
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.Models.NodeView
{
    public interface ICommonModel
    {
        TreeViewItemInfo SelectedItem { get; }
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
        
        public string Declaration { get; set; }

        private TreeViewItemInfo root;
        private ObservableCollection<TreeViewItemInfo> treeViewItems;
        private TreeViewItemInfo selectedItem;

        private string fullPath;
        private ObservableCollection<AttributeInfo> attributes = new ObservableCollection<AttributeInfo>();
        private string innerXml;
        private CommonXmlNode node;

        public CommonModel(TreeViewItemInfo info)
        {
            root = info;

            TreeViewItems = new ObservableCollection<TreeViewItemInfo>
            {
                root
            };
        }


        public void SelectionChange()
        {
            var info = SelectedItem;
            ChangeItem(info);
        }

        public void ChangeItem(TreeViewItemInfo info)
        {
            node = info.Node;

            FullPath = info.Path;
            Attributes.Clear();
            Attributes.AddAll(info.Node.Attributes);
            InnerXml = info.Node.InnerText.Xml;
        }

        public void ChangeInnerXml()
        {
        }

        public void Apply()
        {
            this.node.InnerText.Xml = innerXml;
            this.node.PrioritizeInnerText = true;

            var info = SelectedItem;
            AssignExpanded(root);
            info.Node.AppendAttribute(XML_SELECTED, true.ToString());

            using var ms = new MemoryStream();
            var writer = new CommonXmlWriter(Declaration);
            writer.Write(ms, root.Node);

            ms.Seek(0, SeekOrigin.Begin);
            var reader = new CommonXmlReader(ms);
            var node = reader.GetAllNodes();

            root = new TreeViewItemInfo(node);

            TreeViewItems.Clear();
            TreeViewItems.Add(root);

            SelectedItem = GetSelectedInfo(root);

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
