using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonCoreLib;

namespace SvManagerLibrary.Telnet
{
    /// <summary>
    /// Provides a Stream for logging.
    /// </summary>
    public class LogStream : IDisposable
    {
#if DEBUG
        public Stream Stream { get; private set; }
#else
        private Stream Stream { get; set; }
#endif

        public long Length => Stream.Length;

        /// <summary>
        /// Automatically writes to a file when writing.
        /// </summary>
        public bool AutoFlush { get; set; }

        /// <summary>
        /// The encoding of strings when writing files.
        /// </summary>
        public Encoding TextEncoding { get; set; } = Encoding.UTF8;

        public LogStream(string dirPath)
        {
            var di = new DirectoryInfo(dirPath);
            if (!di.Exists)
                di.Create();
            var dt = DateTime.Now;

            var stream = new FileStream(di.FullName + "\\" + dt.ToString("yyyy-MM-dd- HH-mm-ss") + ".log",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);

            Stream = stream;
        }

        public void Flush()
        {
            Stream.Flush();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Write the string to this stream.
        /// </summary>
        /// <param name="text"></param>
        public void Write(string text)
        {
            if (text == null)
                return;

            var data = TextEncoding.GetBytes(text);
            Write(data, 0, data.Length);

            if (AutoFlush)
            {
                Flush();
            }
        }

        public void Dispose()
        {
            if (Stream != null)
            {
                lock (Stream)
                {
                    Stream?.Flush();
                    Stream?.Dispose();
                    Stream = null;
                }
            }
        }
    }
}
