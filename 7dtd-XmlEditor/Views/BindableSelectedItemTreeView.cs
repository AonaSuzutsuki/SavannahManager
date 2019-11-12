using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _7dtd_XmlEditor.Views
{
    public class BindableSelectedItemTreeView : TreeView
    {
        #region Fields
        public static readonly DependencyProperty BindableSelectedItemProperty
            = DependencyProperty.Register(nameof(BindableSelectedItem),
                typeof(object), typeof(BindableSelectedItemTreeView), new UIPropertyMetadata(null));

        //public static readonly DependencyProperty IgnoreNullSelectedItemProperty
        //    = DependencyProperty.Register(nameof(IgnoreNullSelectedItem),
        //        typeof(bool), typeof(BindableSelectedItemTreeView), new UIPropertyMetadata(false));

        public static readonly DependencyProperty IgnoreNullSelectedItemProperty =
            DependencyProperty.Register(
                "IgnoreNullSelectedItem", // プロパティ名を指定
                typeof(bool), // プロパティの型を指定
                typeof(BindableSelectedItemTreeView), // プロパティを所有する型を指定
                new PropertyMetadata(true));
        #endregion

        #region Properties
        public object BindableSelectedItem
        {
            get => GetValue(BindableSelectedItemProperty);
            set => SetValue(BindableSelectedItemProperty, value);
        }
        public bool IgnoreNullSelectedItem
        {
            get => (bool)GetValue(IgnoreNullSelectedItemProperty);
            set => SetValue(IgnoreNullSelectedItemProperty, value);
        }
        #endregion

        #region Constructors
        public BindableSelectedItemTreeView()
        {
            SelectedItemChanged += OnSelectedItemChanged;
        }
        #endregion

        #region Event Methods
        protected virtual void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (IgnoreNullSelectedItem && SelectedItem == null)
            {
                return;
            }

            SetValue(BindableSelectedItemProperty, SelectedItem);
        }
        #endregion
    }

    public class BindableSelectedItemListView : ListView
    {
        #region Fields
        public static readonly DependencyProperty BindableSelectedItemProperty
            = DependencyProperty.Register(nameof(BindableSelectedItem),
                typeof(object), typeof(BindableSelectedItemListView), new UIPropertyMetadata(null));
        #endregion

        #region Properties
        public object BindableSelectedItem
        {
            get => GetValue(BindableSelectedItemProperty);
            set => SetValue(BindableSelectedItemProperty, value);
        }
        #endregion

        #region Constructors
        public BindableSelectedItemListView()
        {
            SelectionChanged += OnSelectionChanged;
        }
        #endregion

        #region Event Methods
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItem == null)
            {
                return;
            }

            SetValue(BindableSelectedItemProperty, SelectedItem);
        }
        #endregion
    }
}
