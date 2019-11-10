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
using Reactive.Bindings;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.Models.TreeView
{
    public class TreeViewItemInfo
    {
        public string Name { get; set; }
        public TreeViewItemInfo[] Children { get; set; }

        public string Path { get; }

        public CommonXmlNode Node { get; }

        public TreeViewItemInfo(CommonXmlNode root, string parentPath = null)
        {
            Node = root;
            Path = string.IsNullOrEmpty(parentPath) ? $"/{root.TagName}" : $"{parentPath}/{root.TagName}";
            Name = Conditions.IfElse(root.Attributes.Any(), () => $"{root.TagName} {root.Attributes.ToAttributesText()}"
                , () => $"{root.TagName}");
            Children = (from node in root.ChildNodes
                select new TreeViewItemInfo(node, Path)).ToArray();
        }
    }



    public class TreeViewItemModel : TreeViewItem
    {
        #region Properties
        public CommonXmlNode Node { get; set; }
        public ReactiveProperty<TreeViewItemModel> SelectionItem { get; set; } = new ReactiveProperty<TreeViewItemModel>();
        #endregion

        #region Fields
        private bool isOpened;
        private Window view;
        #endregion

        public TreeViewItemModel(Window window)
        {
            var reader = new CommonXmlReader("vehicles.xml");
            var root = reader.GetAllNodes();
            Initialize(root, window);
        }

        public TreeViewItemModel(CommonXmlNode node, Window window)
        {
            Initialize(node, window);
        }

        public void Initialize(CommonXmlNode node, Window window)
        {
            Node = node;
            view = window;

            if (node.ChildNodes.Any())
            {
                Items.Add(new TreeViewItem());
                Expanded += TreeViewItemModel_Expanded;
            }
            Selected += TreeViewItemModel_Selected;

            var sp = new StackPanel { Orientation = Orientation.Horizontal };
            sp.Children.Add(new TextBlock
            {
                Text = Conditions.IfElse(node.Attributes.Any(), () => $"{node.TagName} {node.Attributes.ToAttributesText()}",
                    () => $"{node.TagName}"),
                Style = (Style)view.FindResource("TreeViewItemText")
            });

            Header = sp;
        }

        private void TreeViewItemModel_Expanded(object sender, RoutedEventArgs e)
        {
            if (isOpened)
                return;

            Items.Clear();
            foreach (var node in Node.ChildNodes)
                Items.Add(new TreeViewItemModel(node, view));
            isOpened = true;
        }

        private void TreeViewItemModel_Selected(object sender, RoutedEventArgs e)
        {
            SelectionItem.Value = (IsSelected) ? this : (TreeViewItemModel)e.Source;
        }
    }
}
