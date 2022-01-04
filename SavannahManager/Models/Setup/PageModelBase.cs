using System;
using System.IO;
using Prism.Mvvm;
using SvManagerLibrary.SteamLibrary;

namespace _7dtd_svmanager_fix_mvvm.Models.Setup
{
    public abstract class PageModelBase : BindableBase
    {
        public event CanChangedEventHandler CanChanged;
        protected void OnCanChanged(object sender, bool canChanged)
        {
            CanChanged?.Invoke(sender, new CanChangedEventArgs(canChanged));
        }

        protected InitializeData InitializeData { get; }

        protected PageModelBase(InitializeData initializeData)
        {
            InitializeData = initializeData;
        }

        protected static string GetFileName(string steamPath, string target, string name)
        {
            var filename = GetFullPath(steamPath + target, name);

            if (string.IsNullOrEmpty(filename))
            {
                try
                {
                    var slLoader = new SteamLibraryLoader(steamPath + ConstantValues.SteamLibraryPath);
                    var dirPaths = slLoader.SteamLibraryPathList;
                    foreach (var dirPath in dirPaths)
                        filename = GetFullPath(dirPath.SteamDirPath + target, name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return filename;
        }
        protected static string GetFullPath(string dirPath, string exeName)
        {
            var filename = string.Empty;
            if (Directory.Exists(dirPath))
            {
                var fi = new FileInfo(dirPath + @"\" + exeName);
                if (fi.Exists)
                    filename = fi.FullName;
            }
            return filename;
        }
    }
}