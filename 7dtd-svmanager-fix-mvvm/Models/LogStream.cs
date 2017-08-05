using System;
using System.IO;

namespace Log
{
    static class LogStream
    {

        public static bool IsLogGetter { get; set; }

        private static FileStream fs = null;
        private static StreamWriter sw = null;

        /// <summary>
        /// Create Instance of LogFile Stream.
        /// </summary>
        public static void MakeStream(string dirPath)
        {
            if (IsLogGetter)
            {
                DirectoryInfo di = new DirectoryInfo(KimamaLib.AppInfo.GetAppPath() + @"\logs");
                if (!di.Exists)
                    di.Create();
                DateTime dt = DateTime.Now;
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
        public static void StreamDisposer()
        {
            sw?.Dispose();
            sw = null;
            fs?.Dispose();
            fs = null;
        }

        public static void WriteSteam(string text) {
            sw?.Write(text);
        }
    }
}
