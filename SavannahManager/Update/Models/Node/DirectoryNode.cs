using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.Update.Models.Node
{
    public class DirectoryNode : BindableBase
    {
        private bool _isDelete = true;
        private bool _isSelected;
        private bool _isExpanded;

        public bool IsDelete
        {
            get => _isDelete;
            set
            {
                foreach (var directoryNode in ChildNodes)
                {
                    directoryNode.IsDelete = value;
                }

                SetProperty(ref _isDelete, value);
            }
        }
        
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public bool IsDirectory { get; set; }
        public bool IsRoot { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public DirectoryNode Parent { get; set; }
        public SortedSet<DirectoryNode> ChildNodes { get; set; }

        public DirectoryNode()
        {
            ChildNodes = new SortedSet<DirectoryNode>(Comparer<DirectoryNode>.Create((a, b) =>
            {
                return a.IsDirectory switch
                {
                    true when b.IsDirectory => string.Compare(a.Name, b.Name, StringComparison.Ordinal),
                    true when !b.IsDirectory => -1,
                    false when b.IsDirectory => 1,
                    _ => string.Compare(a.Name, b.Name, StringComparison.Ordinal)
                };
            }));
        }

        public IEnumerable<DirectoryNode> GetDirectories()
        {
            return ChildNodes.Where(x => x.IsDirectory);
        }

        public IEnumerable<DirectoryNode> GetFiles()
        {
            return ChildNodes.Where(x => !x.IsDirectory);
        }

        public override string ToString()
        {
            if (Parent == null)
                return Name;

            return Parent.IsRoot ? Name : $"{Parent}\\{Name}";
        }

        public IEnumerable<string> GetAllDeleteFiles()
        {
            var files = new List<string>(from x in GetFiles() where x.IsDelete select x.FullPath);

            foreach (var directory in GetDirectories().Where(x => x.IsDelete))
            {
                files.AddRange(directory.GetAllDeleteFiles());
            }

            return files;
        }
    }
}