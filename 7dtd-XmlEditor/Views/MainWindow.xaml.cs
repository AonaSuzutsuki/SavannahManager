using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _7dtd_XmlEditor.Models;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.ViewModels;
using CommonStyleLib.Views;
using SavannahXmlLib.XmlWrapper;
using Control = System.Windows.Controls.Control;
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
            _startPos = null;
        }

        private void ItemTreeViewOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ItemsControl itemsControl))
                return;

            var pos = e.GetPosition(itemsControl);
            _startPos = itemsControl.PointToScreen(pos);
        }

        private bool IsDragStartable(Vector delta)
        {
            return (SystemParameters.MinimumHorizontalDragDistance < Math.Abs(delta.X)) ||
                   (SystemParameters.MinimumVerticalDragDistance < Math.Abs(delta.Y));
        }

        private Dictionary<DependencyObject, (ColorChanger, TreeViewItemInfo)> changedBlocks = new Dictionary<DependencyObject, (ColorChanger, TreeViewItemInfo)>();
        private InsertType _insertType;
        private Point? _startPos;
        private void ItemTreeViewOnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is TreeView lb && lb.SelectedItem != null && sender is ItemsControl itemsControl)
            {
                if (_startPos == null)
                    return;

                var curPos = itemsControl.PointToScreen(e.GetPosition(itemsControl));
                var diff = curPos - (Point)_startPos;
                if (IsDragStartable(diff))
                {
                    DragDrop.DoDragDrop(lb, lb.SelectedItem, DragDropEffects.Move);

                    _startPos = null;
                }
            }
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

                var colorChanger = GetColorChanger(result.VisualHit);

                if (!(result.VisualHit is FrameworkElement targetElement) || colorChanger == null)
                    return;
                if (!(targetElement.DataContext is TreeViewItemInfo targetElementInfo) || !(targetElement.Parent is Grid grid))
                    return;

                foreach (var pair in changedBlocks)
                {
                    ResetSeparator(pair.Value.Item1, pair.Value.Item2);
                }

                if (targetElementInfo == sourceItem)
                    return;

                var pos = e.GetPosition(grid);
                if (pos.Y > 0 && pos.Y < 5)
                {
                    _insertType = InsertType.Before;
                    targetElementInfo.BeforeSeparatorVisibility = Visibility.Visible;
                }
                else if (pos.Y < grid.ActualHeight && pos.Y > grid.ActualHeight - 5)
                {
                    _insertType = InsertType.After;
                    targetElementInfo.AfterSeparatorVisibility = Visibility.Visible;
                }
                else
                {
                    _insertType = InsertType.Children;
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

        private ColorChanger GetColorChanger(DependencyObject obj)
        {
            ColorChanger colorChanger = null;

            if (changedBlocks.ContainsKey(obj))
                return changedBlocks[obj].Item1;

            var textBlock = obj as TextBlock;
            if (textBlock != null)
            {
                colorChanger = new ColorChanger
                {
                    BackgroundAction = brush => textBlock.Background = brush
                };
            }

            var border = obj as Border;
            if (border != null)
            {
                colorChanger = new ColorChanger
                {
                    BackgroundAction = brush => border.Background = brush
                };
            }

            return colorChanger;
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

            if (_insertType == InsertType.Before)
            {
                RemoveCurrentItem();

                targetItemParent.InsertBeforeChildren(sourceItem, targetItem);
                targetItemParent.Node.AddBeforeChildElement(targetItem.Node, sourceItem.Node);
                sourceItem.Parent = targetItemParent;
            }
            else if (_insertType == InsertType.After)
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

        private static void ResetSeparator(ColorChanger colorChanger, TreeViewItemInfo info)
        {
            colorChanger.BackgroundAction(Brushes.Transparent);
            info.BeforeSeparatorVisibility = Visibility.Hidden;
            info.AfterSeparatorVisibility = Visibility.Hidden;
        }
    }
}
