using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.Views.NodeView;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.Models
{
    public class MainWindowModel : ModelBase
    {

        public const string XML_ID = "savannah.xml.id";
        public const string XML_SELECTED = "savannah.selected";
        public const string XML_EXPANDED = "savannah.expanded";


        public ObservableCollection<string> EditModeComboItems
        {
            get => editModeComboItems;
            set => SetProperty(ref editModeComboItems, value);
        }

        public ObservableCollection<TreeViewItemInfo> TreeViewItems
        {
            get => treeViewItems;
            set => SetProperty(ref treeViewItems, value);
        }

        private ObservableCollection<string> editModeComboItems;
        private ObservableCollection<TreeViewItemInfo> treeViewItems;

        private TreeViewItemInfo root;
        private string declaration;
        private NavigationService navigation;
        private INodeView commonPage;

        public MainWindowModel(NavigationService navigation)
        {
            this.navigation = navigation;

            commonPage = new CommonView();
            this.navigation.Navigate(commonPage);

            EditModeComboItems = new ObservableCollection<string>
            {
                "Common",
                "Vehicle"
            };

            var reader = new CommonXmlReader("vehicles.xml");
            declaration = reader.Declaration;
            root = new TreeViewItemInfo(reader.GetAllNodes());
            TreeViewItems = new ObservableCollection<TreeViewItemInfo>
            {
                root
            };
        }

        public void NodeViewModeChange(string mode, TreeViewItemInfo info)
        {
            if (mode == "Common")
            {
                commonPage = new CommonView();
                commonPage.ChangeItem(info);
                navigation.Navigate(commonPage);
            }
        }
        public void SelectionChange(TreeViewItemInfo info)
        {
            commonPage.ChangeItem(info);
        }

        public void Apply(TreeViewItemInfo info)
        {
            commonPage.Apply();

            AssignExpanded(root);
            info.Node.AppendAttribute(XML_SELECTED, true.ToString());

            using var ms = new MemoryStream();
            var writer = new CommonXmlWriter(declaration);
            writer.Write(ms, root.Node);

            ms.Seek(0, SeekOrigin.Begin);
            var reader = new CommonXmlReader(ms);
            var node = reader.GetAllNodes();

            root = new TreeViewItemInfo(node);
            var selectedNode = GetSelectedInfo(root);

            TreeViewItems.Clear();
            TreeViewItems.Add(root);

            SelectionChange(selectedNode);
        }

        private TreeViewItemInfo GetSelectedInfo(TreeViewItemInfo info)
        {
            var node = info.Node;
            bool.TryParse(node.GetAttribute(XML_SELECTED).Value, out var isSelected);
            node.RemoveAttribute(XML_SELECTED);
            info.Name = TreeViewItemInfo.GetName(info);
            if (isSelected)
                return info;

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
