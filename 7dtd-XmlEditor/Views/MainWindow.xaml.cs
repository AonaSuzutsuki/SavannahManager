using _7dtd_XmlEditor.Models;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.ViewModels;
using CommonStyleLib.Views;
using SavannahXmlLib.XmlWrapper;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using TreeView = System.Windows.Controls.TreeView;

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

            var model = new MainWindowModel2();
            var vm = new MainWindowViewModel2(new WindowService(this), model);
            this.DataContext = vm;

            ItemTreeView.AllowDrop = true;
            ItemTreeView.PreviewMouseLeftButtonDown += ItemTreeViewOnPreviewMouseLeftButtonDown;
            ItemTreeView.PreviewMouseLeftButtonUp += ItemTreeViewOnPreviewMouseLeftButtonUp;
            ItemTreeView.PreviewMouseMove += ItemTreeViewOnPreviewMouseMove;
            ItemTreeView.Drop += ItemTreeViewOnDrop;
            ItemTreeView.DragOver += ItemTreeViewOnDragOver;
            //ItemTreeView.PreviewDragOver += ItemTreeViewOnDragOver;
        }

        private void ItemTreeViewOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            startPos = null;
        }

        private void ItemTreeViewOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ItemsControl itemsControl))
                return;

            var pos = e.GetPosition(itemsControl);
            startPos = itemsControl.PointToScreen(pos);
        }

        private bool IsDragStartable(Vector delta)
        {
            return (SystemParameters.MinimumHorizontalDragDistance < Math.Abs(delta.X)) ||
                   (SystemParameters.MinimumVerticalDragDistance < Math.Abs(delta.Y));
        }

        private readonly Dictionary<DependencyObject, (ColorChanger, TreeViewItemInfo)> changedBlocks
            = new Dictionary<DependencyObject, (ColorChanger, TreeViewItemInfo)>();
        private InsertType insertType;
        private Point? startPos;
        private void ItemTreeViewOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is TreeView lb) || lb.SelectedItem == null)
                return;
            if (startPos == null)
                return;

            var curPos = lb.PointToScreen(e.GetPosition(lb));
            var diff = curPos - (Point)startPos;
            if (!IsDragStartable(diff))
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

                var sourceItem = (TreeViewItemInfo)e.Data.GetData(typeof(TreeViewItemInfo));
                var pt = e.GetPosition(itemsControl);
                var result = VisualTreeHelper.HitTest(itemsControl, pt);

                var colorChanger = GetColorChanger(result.VisualHit, changedBlocks);

                if (!(result.VisualHit is FrameworkElement targetElement) || colorChanger == null)
                    return;
                if (!(targetElement.DataContext is TreeViewItemInfo targetElementInfo) || !(targetElement.Parent is FrameworkElement grid))
                    return;

                foreach (var pair in changedBlocks)
                {
                    ResetSeparator(pair.Value.Item1, pair.Value.Item2);
                }

                if (targetElementInfo == sourceItem)
                    return;
                var targetParent = targetElementInfo.Parent;

                var pos = e.GetPosition(grid);
                if (pos.Y > 0 && pos.Y < 5)
                {
                    insertType = InsertType.Before;
                    targetElementInfo.BeforeSeparatorVisibility = Visibility.Visible;
                }
                else if (targetParent.Children.Last() == targetElementInfo && pos.Y < grid.ActualHeight && pos.Y > grid.ActualHeight - 5)
                {
                    insertType = InsertType.After;
                    targetElementInfo.AfterSeparatorVisibility = Visibility.Visible;
                }
                else
                {
                    insertType = InsertType.Children;
                    colorChanger.BackgroundAction(Brushes.Black);
                }

                if (!changedBlocks.ContainsKey(grid))
                    changedBlocks.Add(grid, (colorChanger, targetElementInfo));
            }
        }

        public class ColorChanger
        {
            public Action<Brush> BackgroundAction { get; set; }
        }

        private void ItemTreeViewOnDrop(object sender, DragEventArgs e)
        {
            if (!(sender is ItemsControl itemsControl))
                return;

            var sourceItem = (TreeViewItemInfo)e.Data.GetData(typeof(TreeViewItemInfo));

            var pt = e.GetPosition(itemsControl);
            var result = VisualTreeHelper.HitTest(itemsControl, pt);

            var targetElement = result.VisualHit as FrameworkElement;
            if (!(targetElement?.DataContext is TreeViewItemInfo targetItem) || sourceItem == null)
                return;

            if (sourceItem == targetItem)
                return;

            foreach (var pair in changedBlocks)
            {
                ResetSeparator(pair.Value.Item1, pair.Value.Item2);
            }

            var targetItemParent = targetItem.Parent;
            var sourceItemParent = sourceItem.Parent;

            void RemoveCurrentItem()
            {
                sourceItemParent.RemoveChildren(sourceItem);
                sourceItemParent.Node.RemoveChildElement(sourceItem.Node);
            }

            if (insertType == InsertType.Before)
            {
                RemoveCurrentItem();

                targetItemParent.InsertBeforeChildren(sourceItem, targetItem);
                targetItemParent.Node.AddBeforeChildElement(targetItem.Node, sourceItem.Node);
                sourceItem.Parent = targetItemParent;
            }
            else if (insertType == InsertType.After)
            {
                RemoveCurrentItem();

                targetItemParent.InsertAfterChildren(sourceItem, targetItem);
                targetItemParent.Node.AddAfterChildElement(targetItem.Node, sourceItem.Node);
                sourceItem.Parent = targetItemParent;
            }
            else
            {
                if (targetItem.Node.NodeType == XmlNodeType.Tag)
                {
                    RemoveCurrentItem();

                    targetItem.AddChildren(sourceItem);
                    targetItem.Node.AddChildElement(sourceItem.Node);
                    sourceItem.Parent = targetItem;
                }
            }
        }

        private static ColorChanger GetColorChanger(DependencyObject obj, Dictionary<DependencyObject, (ColorChanger, TreeViewItemInfo)> cache)
        {
            ColorChanger colorChanger = null;

            if (cache.ContainsKey(obj))
                return cache[obj].Item1;

            switch (obj)
            {
                case TextBlock textBlock:
                    colorChanger = new ColorChanger
                    {
                        BackgroundAction = brush => textBlock.Background = brush
                    };
                    break;
                case Border border:
                    colorChanger = new ColorChanger
                    {
                        BackgroundAction = brush => border.Background = brush
                    };
                    break;
            }

            return colorChanger;
        }

        private static void ResetSeparator(ColorChanger colorChanger, TreeViewItemInfo info)
        {
            colorChanger.BackgroundAction(Brushes.Transparent);
            info.BeforeSeparatorVisibility = Visibility.Hidden;
            info.AfterSeparatorVisibility = Visibility.Hidden;
        }
    }
}
