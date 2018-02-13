using System;
using System.IO;
using System.IO.Compression;

namespace Archive
{
    public class Zip : IDisposable
    {
        public int Count
        {
            get
            {
                return archive.Entries.Count;
            }
        }

        #region Event
        //public class ExtractedEventArgs : EventArgs
        //{
        //    public int intNo;
        //}
        //public delegate void SocketTcpEventHandler(object sender, SocketTcpEventArgs e);
        public event EventHandler Extracted;
        protected virtual void OnExtracted(EventArgs e)
        {
            Extracted?.Invoke(this, e);
        }
        #endregion

        ZipArchive archive;
        string extractDirPath = string.Empty;
        public Zip(string _zipPath, string _extractDirPath)
        {
            if (string.IsNullOrEmpty(_zipPath) || string.IsNullOrEmpty(_extractDirPath))
            {
                throw new Exception();
            }

            extractDirPath = _extractDirPath;
            archive = ZipFile.OpenRead(_zipPath);
        }

        public void Dispose()
        {
            ((IDisposable)archive).Dispose();
        }

        public void Extract()
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string path = Path.Combine(extractDirPath, entry.FullName);
                if (path.Substring(path.Length - 1, 1) == "/")
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    if (!di.Exists)
                    {
                        di.Create();
                    }
                }
                else
                {
                    entry.ExtractToFile(Path.Combine(extractDirPath, entry.FullName), true);
                }
                    
                OnExtracted(new EventArgs());
            }
        }
    }
}
