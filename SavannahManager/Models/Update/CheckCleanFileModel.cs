using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using _7dtd_svmanager_fix_mvvm.Models.Update.Node;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Models.Update
{
    public class DirectoryTree
    {
        public static DirectoryNode Make(string basePath, IEnumerable<string> files)
        {
            var root = new DirectoryNode
            {
                IsRoot = true
            };
            var map = new Dictionary<string, DirectoryNode>
            {
                { "root", root }
            };

            var enumerable = files as string[] ?? files.ToArray();
            var rootRelativeFiles = from x in enumerable
                let path = new Uri(basePath).MakeRelativeUri(new Uri(x)).ToString()
                select $"root\\{path}";
            var zip = enumerable.Zip(rootRelativeFiles, (real, relative) => (real, relative));

            foreach (var item in zip.Select((v, i) => new { Index = i, Value = v }))
            {
                var file = item.Value.real.Replace("/", "\\");
                var relative = item.Value.relative.Replace("/", "\\");

                var split = relative.Split('\\');
                var currentPath = "";
                var currentNode = map.First().Value;
                foreach (var subItem in split.Select((v, i) => new { Index = i, Value = v }))
                {
                    currentPath += subItem.Value;

                    if (subItem.Index == split.Length - 1)
                    {
                        currentNode.ChildNodes.Add(new DirectoryNode()
                        {
                            Parent = currentNode,
                            Name = subItem.Value,
                            FullPath = file
                        });
                        continue;
                    }

                    if (map.ContainsKey(currentPath))
                    {
                        currentNode = map[currentPath];
                    }
                    else
                    {
                        var subNode = new DirectoryNode
                        {
                            IsDirectory = true,
                            Parent = currentNode,
                            Name = subItem.Value
                        };
                        currentNode.ChildNodes.Add(subNode);
                        currentNode = subNode;
                        map.Add(currentPath, currentNode);
                    }

                    currentPath += "/";
                }
            }

            return root;
        }
    }

    public class CheckCleanFileModel : ModelBase
    {
        private readonly DirectoryNode _root;

        public ObservableCollection<DirectoryNode> TreeViewItems { get; }

        public bool CanCleanUpdate { get; set; }

        public CheckCleanFileModel(IEnumerable<string> files)
        {
            var node = DirectoryTree.Make($"{CommonCoreLib.AppInfo.GetAppPath()}\\", files);

            TreeViewItems = new ObservableCollection<DirectoryNode>();
            TreeViewItems.AddRange(node.ChildNodes);
            _root = node;
        }

        public void SetAllDeleteTarget()
        {
            _root.IsDelete = true;
        }

        public void SetAllNotDeleteTarget()
        {
            _root.IsDelete = false;
        }

        public IEnumerable<string> GetTargetFiles()
        {
            return _root.GetAllDeleteFiles();
        }
    }
}
