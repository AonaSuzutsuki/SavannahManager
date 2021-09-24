using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SvManagerLibrary.SteamLibrary
{
    /// <summary>
    /// Provides a information of steam library path.
    /// </summary>
    public class SteamLibraryPath
    {
        /// <summary>
        /// Path of the directory where the game is stored.
        /// </summary>
        public string SteamDirPath { get; }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="dirPath">Path of the directory where the game is stored.</param>
        public SteamLibraryPath(string dirPath)
        {
            SteamDirPath = dirPath;
        }

        /// <summary>
        /// Check the equivalence of this object and the argument object.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <returns>It returns True if equivalent, False otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is SteamLibraryPath path &&
                   SteamDirPath == path.SteamDirPath;
        }

        /// <summary>
        /// Object.GetHashCode()
        /// </summary>
        /// <returns>The hash value.</returns>
        public override int GetHashCode()
        {
            return -786403207 + EqualityComparer<string>.Default.GetHashCode(SteamDirPath);
        }
    }

    /// <summary>
    /// Provides an analyzer of steam library path.
    /// </summary>
    public class SteamLibraryLoader
    {
        /// <summary>
        /// List of steam library path.
        /// </summary>
        public List<SteamLibraryPath> SteamLibraryPathList { get; }

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="filePath">Filepath to be analyzed.</param>
        public SteamLibraryLoader(string filePath)
        {
            SteamLibraryPathList = AnalyzeJson(filePath);
        }

        private static List<SteamLibraryPath> AnalyzeJson(string filePath)
        {
            var dirList = new List<SteamLibraryPath>();
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(fs);
            while (sr.Peek() > -1)
            {
                const string expression = @"^[\s]*""path""[\s]*""(?<path>.*)""$";
                var reg = new Regex(expression);
                var line = sr.ReadLine();
                var match = reg.Match(line ?? string.Empty);
                if (match.Success)
                {
                    dirList.Add(new SteamLibraryPath(match.Groups["path"].Value.Replace("\\\\", "\\")));
                }
            }
            return dirList;
        }
    }
}
