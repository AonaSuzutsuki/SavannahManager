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
        #endregion

        #region Properties
        public object BindableSelectedItem
        {
            get => GetValue(BindableSelectedItemProperty);
            set => SetValue(BindableSelectedItemProperty, value);
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
            if (SelectedItem == null)
            {
                return;
            }

            SetValue(BindableSelectedItemProperty, SelectedItem);
        }
        #endregion
    }
}
