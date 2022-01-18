using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonCoreLib;

namespace SvManagerLibrary.Telnet
{
    public class LogStream : Stream, IDisposable
    {
        private FileStream _fs;

        public override bool CanRead => _fs.CanRead;
        public override bool CanSeek => _fs.CanSeek;
        public override bool CanWrite => _fs.CanWrite;
        public override long Length => _fs.Length;
        public override long Position
        {
            get => _fs.Position;
            set => _fs.Position = value;
        }

        public bool AutoFlush { get; set; }

        public LogStream(string dirPath)
        {
            var di = new DirectoryInfo(AppInfo.GetAppPath() + @"\logs");
            if (!di.Exists)
                di.Create();
            var dt = DateTime.Now;

            _fs = new FileStream(dirPath + dt.ToString("yyyy-MM-dd- HH-mm-ss") + ".log",
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
        }

        public override void Flush()
        {
            _fs.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _fs.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _fs.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _fs.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _fs.Write(buffer, offset, count);
        }

        public void Write(string text)
        {
            if (text == null)
                return;

            var data = Encoding.UTF8.GetBytes(text);
            Write(data, 0, data.Length);

            if (AutoFlush)
            {
                Flush();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_fs != null)
            {
                lock (_fs)
                {
                    _fs?.Flush();
                    _fs?.Dispose();
                    _fs = null;
                }
            }
        }
    }
}
