using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Prism.Mvvm;

namespace _7dtd_XmlEditor.Models.TreeView
{
    public class TreeViewItemInfoBase : BindableBase
    {
        private string name = string.Empty;
        private bool isExpanded;
        private bool isSelected;
        protected ObservableCollection<TreeViewItemInfoBase> children;
        private Brush background = Brushes.Transparent;
        private Visibility beforeSeparatorVisibility = Visibility.Hidden;
        private Visibility afterSeparatorVisibility = Visibility.Hidden;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public IEnumerable<TreeViewItemInfoBase> Children => children;

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public bool IsSelected
        {
            get => isSelected;
            set => SetProperty(ref isSelected, value);
        }

        public Brush Background
        {
            get => background;
            set => SetProperty(ref background, value);
        }

        public TreeViewItemInfoBase Parent { get; set; }

        public Visibility BeforeSeparatorVisibility
        {
            get => beforeSeparatorVisibility;
            set => SetProperty(ref beforeSeparatorVisibility, value);
        }

        public Visibility AfterSeparatorVisibility
        {
            get => afterSeparatorVisibility;
            set => SetProperty(ref afterSeparatorVisibility, value);
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