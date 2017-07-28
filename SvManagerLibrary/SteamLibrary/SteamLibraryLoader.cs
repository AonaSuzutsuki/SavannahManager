using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SvManagerLibrary.SteamLibrary
{
    public class SteamLibraryPath
    {
        public string SteamDirPath
        {
            get;
        }

        public SteamLibraryPath(string dirPath)
        {
            SteamDirPath = dirPath;
        }
    }
    public class SteamLibraryLoader
    {
        public List<SteamLibraryPath> SteamLibraryPathList
        {
            get;
        }
        
        public SteamLibraryLoader(string filePath)
        {
            SteamLibraryPathList = SetJson(filePath);
        }

        private static List<SteamLibraryPath> SetJson(string filePath)
        {
            var dirList = new List<SteamLibraryPath>();
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(fs);
            while (sr.Peek() > -1)
            {
                const string expression = @"^\t""(?<name>.*?)""\t\t""(?<path>.*?)""$";
                var reg = new Regex(expression);
                var match = reg.Match(sr.ReadLine());
                if (match.Success)
                {
                    if (Directory.Exists(match.Groups["path"].Value))
                        dirList.Add(new SteamLibraryPath(match.Groups["path"].Value));
                }
            }
            return dirList;
        }
    }
}
