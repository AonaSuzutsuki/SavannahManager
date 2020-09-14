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
using Control = System.Windows.Controls.Control;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using TreeView = System.Windows.Controls.TreeView;

namespace _7dtd_XmlEditor.Views
{
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
            ItemTreeView.Drop += ItemTreeViewOnDrop;
            ItemTreeView.DragOver += ItemTreeViewOnDragOver;
            //ItemTreeView.PreviewDragOver += ItemTreeViewOnDragOver;
        }

        private void ItemTreeViewOnDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeViewItemInfo)))
            {
                e.Effects = DragDropEffects.Move;

                var itemsControl = sender as ItemsControl;
                if (itemsControl == null)
                    return;
                var pt = e.GetPosition(itemsControl);
                var result = VisualTreeHelper.HitTest(itemsControl, pt);
                var targetElement = result.VisualHit as TextBlock;
                if (targetElement == null)
                    return;
                targetElement.Background = Brushes.Black;
            }
        }

        private void ItemTreeViewOnDrop(object sender, DragEventArgs e)
        {
            var itemsControl = sender as ItemsControl;
            if (itemsControl == null)
                return;

            var sourceItem = (TreeViewItemInfo)e.Data.GetData(typeof(TreeViewItemInfo));

            var pt = e.GetPosition(itemsControl);
            var result = VisualTreeHelper.HitTest(itemsControl, pt);
            var targetElement = result.VisualHit as FrameworkElement;
            var targetItem = targetElement?.DataContext;

        }

        private void ItemTreeViewOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TreeView lb && lb.SelectedItem != null)
            {
                DragDrop.DoDragDrop(lb, lb.SelectedItem, DragDropEffects.Move);
            }
        }
    }
}
