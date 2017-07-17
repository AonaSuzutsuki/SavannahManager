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
        string dirPath = string.Empty;
        public string DirPath
        {
            get
            {
                return dirPath;
            }
        }

        public SteamLibraryPath(string dirPath)
        {
            this.dirPath = dirPath;
        }
    }
    public class SteamLibraryLoader
    {
        string filePath = string.Empty;
        public string FilePath
        {
            get
            {
                return filePath;
            }

            set
            {
                filePath = value;
            }
        }

        List<SteamLibraryPath> dirList = new List<SteamLibraryPath>();
        public List<SteamLibraryPath> SteamLibraryPathList
        {
            get
            {
                return dirList;
            }
        }
        
        public SteamLibraryLoader(string _filePath)
        {
            filePath = _filePath;
            SetJson();
        }

        public void SetJson()
        {
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(fs);
            while (sr.Peek() > -1)
            {
                string str = sr.ReadLine();

                string expression = @"^\t""(?<name>.*?)""\t\t""(?<path>.*?)""$";
                var reg = new Regex(expression);
                Match match = reg.Match(str);
                if (match.Success == true)
                {
                    if (Directory.Exists(match.Groups["path"].Value))
                        dirList.Add(new SteamLibraryPath(match.Groups["path"].Value));
                }
            }
        }
    }
}
