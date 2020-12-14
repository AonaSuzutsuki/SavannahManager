using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Prism.Mvvm;

namespace _7dtd_XmlEditor.Models.TreeView
{
    public class TreeViewItemInfoBase : BindableBase
    {
        private string _name = string.Empty;
        private bool _isExpanded;
        private bool _isSelected;
        protected ObservableCollection<TreeViewItemInfoBase> children;
        private Brush _background = Brushes.Transparent;
        private Visibility _beforeSeparatorVisibility = Visibility.Hidden;
        private Visibility _afterSeparatorVisibility = Visibility.Hidden;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public IEnumerable<TreeViewItemInfoBase> Children => children;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public Brush Background
        {
            get => _background;
            set => SetProperty(ref _background, value);
        }

        public TreeViewItemInfoBase Parent { get; set; }

        public Visibility BeforeSeparatorVisibility
        {
            get => _beforeSeparatorVisibility;
            set => SetProperty(ref _beforeSeparatorVisibility, value);
        }

        public Visibility AfterSeparatorVisibility
        {
            get => _afterSeparatorVisibility;
            set => SetProperty(ref _afterSeparatorVisibility, value);
        }

        public void AddChildren(TreeViewItemInfoBase info)
        {
            children.Add(info);
        }

        public void RemoveChildren(TreeViewItemInfoBase info)
        {
            children.Remove(info);
        }

        public void InsertBeforeChildren(TreeViewItemInfoBase from, TreeViewItemInfoBase to)
        {
            var index = children.IndexOf(to);
            if (index < 0)
                return;

            children.Insert(index, @from);
        }

        public void InsertAfterChildren(TreeViewItemInfoBase from, TreeViewItemInfoBase to)
        {
            var index = children.IndexOf(to);
            if (index < 0)
                return;

            children.Insert(index + 1, @from);
        }

        public bool ContainsParent(TreeViewItemInfoBase info)
        {
            if (Parent == null)
                return false;
            return Parent == info || Parent.ContainsParent(info);
        }
    }
}