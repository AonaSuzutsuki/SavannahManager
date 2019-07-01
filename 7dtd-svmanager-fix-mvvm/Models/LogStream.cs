using CommonCoreLib;
using System;
using System.IO;

namespace Log
{
    public class LogStream
    {

        public bool IsLogGetter { get; set; }

        private FileStream fs = null;
        private StreamWriter sw = null;

        /// <summary>
        /// Create Instance of LogFile Stream.
        /// </summary>
        public void MakeStream(string dirPath)
        {
            if (IsLogGetter)
            {
                var di = new DirectoryInfo(AppInfo.GetAppPath() + @"\logs");
                if (!di.Exists)
                    di.Create();
                var dt = DateTime.Now;
                fs = new FileStream(dirPath +
                    dt.ToString("yyyy-MM-dd- HH-mm-ss") + ".log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

                sw = new StreamWriter(fs, System.Text.Encoding.UTF8)
                {
                    AutoFlush = true
                };
            }
        }

        /// <summary>
        /// Dispose LogFile Stream.
        /// </summary>
        public void StreamDisposer()
        {
            sw?.Dispose();
            sw = null;
            fs?.Dispose();
            fs = null;
        }

        public void WriteSteam(string text) {
            sw?.Write(text);
        }
    }
}
