using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonStyleLib.Models;
using static System.String;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class DeleteFileInfo
    {
        public string DisplayPath { get; set; }
        public string FilePath { get; set; }
        public bool IsDelete { get; set; } = true;
    }

    public class FileNode
    {
        public DirectoryNode Parent { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }

        public override string ToString()
        {
            if (Parent == null)
                return Name;

            return Parent.IsRoot ? Name : $"{Parent}\\{Name}";
        }
    }

    public class DirectoryNode
    {
        public bool IsDirectory { get; set; }
        public bool IsRoot { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public DirectoryNode Parent { get; set; }
        public SortedSet<DirectoryNode> ChildNodes { get; set; }

        public DirectoryNode()
        {
            ChildNodes = new SortedSet<DirectoryNode>(Comparer<DirectoryNode>.Create((a, b) =>
                Compare(a.Name, b.Name, StringComparison.Ordinal)));
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

        public IEnumerable<DirectoryNode> GetAllFilePaths()
        {
            var files = new List<DirectoryNode>(GetFiles());

            foreach (var directory in GetDirectories())
            {
                files.AddRange(directory.GetAllFilePaths());
            }

            return files;
        }
    }

    public class DirectoryTree
    {
        public static DirectoryNode Make(string basePath, IEnumerable<string> files)
        {
            //.Select(x => new Uri(CommonCoreLib.AppInfo.GetAppPath() + "\\").MakeRelativeUri(new Uri(x)).ToString())

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
                let path = new Uri(CommonCoreLib.AppInfo.GetAppPath() + "\\").MakeRelativeUri(new Uri(x)).ToString()
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
        public ObservableCollection<DeleteFileInfo> DeleteFiles { get; }

        public CheckCleanFileModel(IEnumerable<string> files)
        {
            var fileList = files.OrderBy(Path.GetFileName).ToList();
            var node = DirectoryTree.Make(CommonCoreLib.AppInfo.GetAppPath() + "\\", fileList);
            var f = node.GetAllFilePaths();

            DeleteFiles = new ObservableCollection<DeleteFileInfo>(from x in f
                                                                   select new DeleteFileInfo
            {
                FilePath = x.FullPath,
                DisplayPath = x.ToString()
            });
        }
    }
}
