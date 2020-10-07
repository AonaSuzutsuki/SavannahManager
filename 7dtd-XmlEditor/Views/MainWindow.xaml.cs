using _7dtd_XmlEditor.Models;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.ViewModels;
using CommonStyleLib.Views;
using SavannahXmlLib.XmlWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using _7dtd_XmlEditor.Extensions;

namespace _7dtd_XmlEditor.Views
{
    public enum InsertType
    {
        After,
        Before,
        Children
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var model = new MainWindowModel();
            var vm = new MainWindowViewModel(new WindowService(this), model);
            this.DataContext = vm;

            ItemTreeView.AllowDrop = true;
            ItemTreeView.PreviewMouseLeftButtonDown += ItemTreeViewOnPreviewMouseLeftButtonDown;
            ItemTreeView.PreviewMouseLeftButtonUp += ItemTreeViewOnPreviewMouseLeftButtonUp;
            ItemTreeView.PreviewMouseMove += ItemTreeViewOnPreviewMouseMove;
            ItemTreeView.Drop += ItemTreeViewOnDrop;
            ItemTreeView.DragOver += ItemTreeViewOnDragOver;
        }

        #region Fields

        private readonly HashSet<TreeViewItemInfo> changedBlocks = new HashSet<TreeViewItemInfo>();
        private InsertType insertType;
        private Point? startPos;

        #endregion

        private void ItemTreeViewOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            startPos = null;
        }

        private void ItemTreeViewOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ItemsControl itemsControl))
                return;

            var pos = e.GetPosition(itemsControl);
            var hit = HitTest<FrameworkElement>(itemsControl, e.GetPosition);

            if (hit?.DataContext is TreeViewItemInfo)
                startPos = itemsControl.PointToScreen(pos);
            else
                startPos = null;
        }

        private void ItemTreeViewOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is TreeView lb) || lb.SelectedItem == null)
                return;
            if (startPos == null)
                return;

            var curPos = lb.PointToScreen(e.GetPosition(lb));
            var diff = curPos - (Point)startPos;
            if (!CanDrag(diff))
                return;

            DragDrop.DoDragDrop(lb, lb.SelectedItem, DragDropEffects.Move);

            startPos = null;
        }

        private void ItemTreeViewOnDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeViewItemInfo)))
            {
                e.Effects = DragDropEffects.Move;

                if (!(sender is ItemsControl itemsControl))
                    return;

                var scrollViewer = itemsControl.Descendants<ScrollViewer>().FirstOrDefault();
                DragScroll(scrollViewer, itemsControl, e);

                var sourceItem = (TreeViewItemInfo)e.Data.GetData(typeof(TreeViewItemInfo));
                var targetElement = HitTest<FrameworkElement>(itemsControl, e.GetPosition);

                var parentGrid = targetElement?.Parent<Grid>();
                if (parentGrid == null || !(targetElement.DataContext is TreeViewItemInfo targetElementInfo))
                    return;

                ResetSeparator(changedBlocks);

                var targetParent = targetElementInfo.Parent;
                if (targetElementInfo == sourceItem || targetParent == null)
                    return;

                const int boundary = 10;
                var pos = e.GetPosition(parentGrid);
                if (pos.Y > 0 && pos.Y < boundary)
                {
                    insertType = InsertType.Before;
                    targetElementInfo.BeforeSeparatorVisibility = Visibility.Visible;
                }
                else if (targetParent.Children.Last() == targetElementInfo
                         && pos.Y < parentGrid.ActualHeight && pos.Y > parentGrid.ActualHeight - boundary)
                {
                    insertType = InsertType.After;
                    targetElementInfo.AfterSeparatorVisibility = Visibility.Visible;
                }
                else
                {
                    insertType = InsertType.Children;
                    targetElementInfo.Background = Brushes.Black;
                }

                if (!changedBlocks.Contains(targetElementInfo))
                    changedBlocks.Add(targetElementInfo);
            }
        }

        private static void DragScroll(ScrollViewer scrollViewer, ItemsControl itemsControl, DragEventArgs e)
        {
            const double tolerance = 10d;
            const double offset = 3d;
            var verticalPos = e.GetPosition(itemsControl).Y;
            if (verticalPos < tolerance) // Top of visible list?
                scrollViewer?.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset); //Scroll up.
            else if (verticalPos > itemsControl.ActualHeight - tolerance) //Bottom of visible list?
                scrollViewer?.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset); //Scroll down.
        }

        private void ItemTreeViewOnDrop(object sender, DragEventArgs e)
        {
            ResetSeparator(changedBlocks);

            if (!(sender is ItemsControl itemsControl))
                return;

            var sourceItem = (TreeViewItemInfo)e.Data.GetData(typeof(TreeViewItemInfo));
            var targetItem = HitTest<FrameworkElement>(itemsControl, e.GetPosition)?.DataContext as TreeViewItemInfo;

            if (targetItem == null || sourceItem == null || sourceItem == targetItem)
                return;

            var targetItemParent = targetItem.Parent;
            var sourceItemParent = sourceItem.Parent;
            if (insertType == InsertType.Before)
            {
                RemoveCurrentItem(sourceItemParent, sourceItem);

                targetItemParent.InsertBeforeChildren(sourceItem, targetItem);
                targetItemParent.Node.AddBeforeChildElement(targetItem.Node, sourceItem.Node);
                sourceItem.Parent = targetItemParent;
            }
            else if (insertType == InsertType.After)
            {
                RemoveCurrentItem(sourceItemParent, sourceItem);

                targetItemParent.InsertAfterChildren(sourceItem, targetItem);
                targetItemParent.Node.AddAfterChildElement(targetItem.Node, sourceItem.Node);
                sourceItem.Parent = targetItemParent;
            }
            else
            {
                if (targetItem.Node.NodeType == XmlNodeType.Tag)
                {
                    RemoveCurrentItem(sourceItemParent, sourceItem);

                    targetItem.AddChildren(sourceItem);
                    targetItem.Node.AddChildElement(sourceItem.Node);
                    targetItem.IsExpanded = true;
                    sourceItem.Parent = targetItem;
                }
            }
        }

        private static void RemoveCurrentItem(TreeViewItemInfo sourceItemParent, TreeViewItemInfo sourceItem)
        {
            sourceItemParent.RemoveChildren(sourceItem);
            sourceItemParent.Node.RemoveChildElement(sourceItem.Node);
        }

        private static T HitTest<T>(UIElement itemsControl, Func<IInputElement, Point> getPosition) where T : class
        {
            var pt = getPosition(itemsControl);
            var result = itemsControl.InputHitTest(pt) as DependencyObject;
            if (result is T ret)
                return ret;
            return null;
        }

        private static bool CanDrag(Vector delta)
        {
            return (SystemParameters.MinimumHorizontalDragDistance < Math.Abs(delta.X)) ||
                   (SystemParameters.MinimumVerticalDragDistance < Math.Abs(delta.Y));
        }

        private static void ResetSeparator(ICollection<TreeViewItemInfo> collection)
        {
            var list = collection.ToList();
            foreach (var pair in list)
            {
                ResetSeparator(pair);
                collection.Remove(pair);
            }
        }

        private static void ResetSeparator(TreeViewItemInfo info)
        {
            info.Background = Brushes.Transparent;
            info.BeforeSeparatorVisibility = Visibility.Hidden;
            info.AfterSeparatorVisibility = Visibility.Hidden;
        }
    }
}
