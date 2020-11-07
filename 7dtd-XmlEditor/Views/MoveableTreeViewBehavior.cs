using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Prism.Commands;
using _7dtd_XmlEditor.Extensions;
using _7dtd_XmlEditor.Models.TreeView;

namespace _7dtd_XmlEditor.Views
{

    public class DropArguments
    {
        public TreeViewItemInfoBase Source { get; set; }
        public TreeViewItemInfoBase Target { get; set; }
        public MoveableTreeViewBehavior.InsertType Type { get; set; }
    }

    public class MoveableTreeViewBehavior : Behavior<TreeView>
    {

        public enum InsertType
        {
            After,
            Before,
            Children
        }

        private readonly HashSet<TreeViewItemInfoBase> _changedBlocks = new HashSet<TreeViewItemInfoBase>();
        private InsertType _insertType;
        private Point? _startPos;

        #region DropCommand
        public ICommand DropCommand
        {
            get => (ICommand)GetValue(DropCommandProperty);
            set => SetValue(DropCommandProperty, value);
        }

        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.Register(
            "DropCommand",
            typeof(ICommand),
            typeof(MoveableTreeViewBehavior),
            new UIPropertyMetadata(null));
        #endregion

        public Type DataType
        {
            get => (Type)GetValue(DataTypeProperty);
            set => SetValue(DataTypeProperty, value);
        }

        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register(
            "DataType",
            typeof(Type),
            typeof(MoveableTreeViewBehavior),
            new UIPropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove += OnPreviewMouseMove;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.DragOver += OnDragOver;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.AllowDrop = false;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            AssociatedObject.PreviewMouseMove -= OnPreviewMouseMove;
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.DragOver -= OnDragOver;
        }

        private void OnDragOver(object sender, DragEventArgs e)
        {
            ResetSeparator(_changedBlocks);

            if (!(sender is ItemsControl itemsControl) || !e.Data.GetDataPresent(DataType))
                return;

            DragScroll(itemsControl, e);

            var sourceItem = (TreeViewItemInfoBase)e.Data.GetData(DataType);
            var targetElement = HitTest<FrameworkElement>(itemsControl, e.GetPosition);

            var parentGrid = targetElement?.GetParent<Grid>();
            if (parentGrid == null || !(targetElement.DataContext is TreeViewItemInfoBase targetElementInfo) || targetElementInfo == sourceItem)
                return;

            if (targetElementInfo.ContainsParent(sourceItem))
                return;

            e.Effects = DragDropEffects.Move;

            var targetParentLast = GetParentLastChild(targetElementInfo);

            const int boundary = 10;
            var pos = e.GetPosition(parentGrid);
            if (pos.Y > 0 && pos.Y < boundary)
            {
                _insertType = InsertType.Before;
                targetElementInfo.BeforeSeparatorVisibility = Visibility.Visible;
            }
            else if (targetParentLast == targetElementInfo
                     && pos.Y < parentGrid.ActualHeight && pos.Y > parentGrid.ActualHeight - boundary)
            {
                _insertType = InsertType.After;
                targetElementInfo.AfterSeparatorVisibility = Visibility.Visible;
            }
            else
            {
                _insertType = InsertType.Children;
                targetElementInfo.Background = Brushes.Gray;
            }

            if (!_changedBlocks.Contains(targetElementInfo))
                _changedBlocks.Add(targetElementInfo);
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            ResetSeparator(_changedBlocks);

            if (!(sender is ItemsControl itemsControl))
                return;

            var sourceItem = (TreeViewItemInfoBase)e.Data.GetData(DataType);
            var targetItem = HitTest<FrameworkElement>(itemsControl, e.GetPosition)?.DataContext as TreeViewItemInfoBase;

            if (targetItem == null || sourceItem == null || sourceItem == targetItem)
                return;

            if (targetItem.ContainsParent(sourceItem))
                return;

            var sourceItemParent = sourceItem.Parent;
            var targetItemParent = targetItem.Parent;
            RemoveCurrentItem(sourceItemParent, sourceItem);
            switch (_insertType)
            {
                case InsertType.Before:
                    targetItemParent.InsertBeforeChildren(sourceItem, targetItem);
                    sourceItem.Parent = targetItemParent;
                    sourceItem.IsSelected = true;
                    break;
                case InsertType.After:
                    targetItemParent.InsertAfterChildren(sourceItem, targetItem);
                    sourceItem.Parent = targetItemParent;
                    sourceItem.IsSelected = true;
                    break;
                case InsertType.Children:
                    targetItem.AddChildren(sourceItem);
                    targetItem.IsExpanded = true;
                    sourceItem.IsSelected = true;
                    sourceItem.Parent = targetItem;
                    break;
            }

            DropCommand?.Execute(new DropArguments
            {
                Source = sourceItem,
                Target = targetItem,
                Type = _insertType
            });
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is TreeView treeView) || treeView.SelectedItem == null || _startPos == null)
                return;

            var cursorPoint = treeView.PointToScreen(e.GetPosition(treeView));
            var diff = cursorPoint - (Point)_startPos;
            if (!CanDrag(diff))
                return;

            DragDrop.DoDragDrop(treeView, treeView.SelectedItem, DragDropEffects.Move);

            _startPos = null;
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _startPos = null;
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ItemsControl itemsControl))
                return;

            var pos = e.GetPosition(itemsControl);
            var hit = HitTest<FrameworkElement>(itemsControl, e.GetPosition);
            if (hit.DataContext is TreeViewItemInfoBase)
                _startPos = itemsControl.PointToScreen(pos);
            else
                _startPos = null;
        }




        private static TreeViewItemInfoBase GetParentLastChild(TreeViewItemInfoBase info)
        {
            var targetParent = info.Parent;
            var last = targetParent?.Children.LastOrDefault();
            return last;
        }

        private static void RemoveCurrentItem(TreeViewItemInfoBase sourceItemParent, TreeViewItemInfoBase sourceItem)
        {
            sourceItemParent.RemoveChildren(sourceItem);
        }

        private static void ResetSeparator(ICollection<TreeViewItemInfoBase> collection)
        {
            var list = collection.ToList();
            foreach (var pair in list)
            {
                ResetSeparator(pair);
                collection.Remove(pair);
            }
        }

        private static void ResetSeparator(TreeViewItemInfoBase info)
        {
            info.Background = Brushes.Transparent;
            info.BeforeSeparatorVisibility = Visibility.Hidden;
            info.AfterSeparatorVisibility = Visibility.Hidden;
        }

        private static void DragScroll(FrameworkElement itemsControl, DragEventArgs e)
        {
            var scrollViewer = itemsControl.Descendants<ScrollViewer>().FirstOrDefault();
            const double tolerance = 10d;
            const double offset = 3d;
            var verticalPos = e.GetPosition(itemsControl).Y;
            if (verticalPos < tolerance)
                scrollViewer?.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
            else if (verticalPos > itemsControl.ActualHeight - tolerance)
                scrollViewer?.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
        }

        private static T HitTest<T>(UIElement itemsControl, Func<IInputElement, Point> getPosition) where T : class
        {
            var pt = getPosition(itemsControl);
            var result = itemsControl.InputHitTest(pt);
            if (result is T ret)
                return ret;
            return null;
        }

        private static bool CanDrag(Vector delta)
        {
            return (SystemParameters.MinimumHorizontalDragDistance < Math.Abs(delta.X)) ||
                   (SystemParameters.MinimumVerticalDragDistance < Math.Abs(delta.Y));
        }
    }
}
