using System;
using System.IO;
using CommonCoreLib;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class LogStream : IDisposable
    {
        private FileStream _fs;
        private StreamWriter _sw;


        /// <summary>
        /// Create Instance of LogFile Stream.
        /// </summary>
        public LogStream(string dirPath)
        {
            var di = new DirectoryInfo(AppInfo.GetAppPath() + @"\logs");
            if (!di.Exists)
                di.Create();
            var dt = DateTime.Now;

            _fs = new FileStream(dirPath +
                                 dt.ToString("yyyy-MM-dd- HH-mm-ss") + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            _sw = new StreamWriter(_fs, System.Text.Encoding.UTF8)
            {
                AutoFlush = true
            };
        }


        /// <summary>
        /// Dispose LogFile Stream.
        /// </summary>
        public void Dispose()
        {
            if (_sw != null)
            {
                lock (_sw)
                {
                    _sw?.Dispose();
                    _sw = null;
                }
            }

            if (_fs != null)
            {
                lock (_fs)
                {
                    _fs?.Dispose();
                    _fs = null;
                }
            }
        }

        public void WriteSteam(string text) {
            _sw?.Write(text);
        }
    }
}
