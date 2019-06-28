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
        public string SteamDirPath { get; }

        public SteamLibraryPath(string dirPath)
        {
            SteamDirPath = dirPath;
        }

        public override bool Equals(object obj)
        {
            return obj is SteamLibraryPath path &&
                   SteamDirPath == path.SteamDirPath;
        }

        public override int GetHashCode()
        {
            return -786403207 + EqualityComparer<string>.Default.GetHashCode(SteamDirPath);
        }
    }
    public class SteamLibraryLoader
    {
        public List<SteamLibraryPath> SteamLibraryPathList { get; }
        
        public SteamLibraryLoader(string filePath)
        {
            SteamLibraryPathList = AnalyzeJson(filePath);
        }

        private static List<SteamLibraryPath> AnalyzeJson(string filePath)
        {
            var dirList = new List<SteamLibraryPath>();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var sr = new StreamReader(fs);
                while (sr.Peek() > -1)
                {
                    const string expression = @"^\t""[0-9]+""\t\t""(?<path>.*?)""$";
                    var reg = new Regex(expression);
                    var line = sr.ReadLine();
                    var match = reg.Match(line);
                    if (match.Success)
                    {
                        dirList.Add(new SteamLibraryPath(match.Groups["path"].Value.Replace("\\\\", "\\")));
                    }
                }
                return dirList;
            }
        }
    }
}
