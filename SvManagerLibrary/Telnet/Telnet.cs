using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{

    public class TelnetClient : IDisposable, ITelnetClient
    {
        #region StaticMember
        /// <summary>
        /// Carriage Return.
        /// </summary>
        public static byte[] Cr { get; } = { 0x0D };
        /// <summary>
        /// Line Feed.
        /// </summary>
        public static byte[] Lf { get; } = { 0x0A };
        /// <summary>
        /// Carriage Return & Line Feed.
        /// </summary>
        public static byte[] Crlf { get; } = { 0x0D, 0x0A };
        #endregion

        #region ReadedEvent
        public class TelnetReadEventArgs : EventArgs
        {
            public string IpAddress;
            public string Log;
        }
        public delegate void TelnetReadEventHandler(object sender, TelnetReadEventArgs e);
        public event TelnetReadEventHandler ReadEvent;
        public event TelnetReadEventHandler Started;
        public event TelnetReadEventHandler Finished;
        protected virtual void OnRead(TelnetReadEventArgs e)
        {
            ReadEvent?.Invoke(this, e);
        }
        protected virtual void OnStarted(TelnetReadEventArgs e)
        {
            Started?.Invoke(this, e);
        }
        protected virtual void OnFinished(TelnetReadEventArgs e)
        {
            Finished?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// Get or Set Receiving Time Out Time (millisecond).
        /// </summary>
        public int ReceiveTimeout { set; get; } = 5000;

        /// <summary>
        /// Get or Set Receiving Buffer Size.
        /// </summary>
        public int ReceiveBufferSize { set; get; } = 10240;

        /// <summary>
        /// Get or Set Sending and Receiving text encoding.
        /// </summary>
        public System.Text.Encoding Encoding { set; get; } = System.Text.Encoding.UTF8;

        /// <summary>
        /// Get a state of connection
        /// </summary>
        public bool Connected
        {
            get
            {
                var isPoll = clientSocket?.Poll(0, SelectMode.SelectRead) ?? false;
                var isAvailable = clientSocket != null && clientSocket.Available == 0;
                return !(isPoll & isAvailable);
            }
        }

        public bool DestructionEvent { get; set; } = false;

        private Socket clientSocket;

        private void LockAction(Action<Socket> action)
        {
            if (clientSocket == null) return;
            lock (clientSocket)
            {
                action(clientSocket);
            }
        }

        private T LockFunction<T>(Func<Socket, T> func)
        {
            if (clientSocket == null) return default;
            lock (clientSocket)
            {
                return func(clientSocket);
            }
        }

        public bool Connect(string address, int port)
        {
            _disposedValue = false;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = ReceiveTimeout,
                ReceiveBufferSize = ReceiveBufferSize
            };

            try
            {
                clientSocket.Connect(address, port);

                var endPoint = (IPEndPoint)clientSocket.RemoteEndPoint;
                OnStarted(new TelnetReadEventArgs() { IpAddress = endPoint.Address.ToString() });
                if (ReadEvent != null)
                    _ = HandleTcp(endPoint);
                return true;
            }
            catch
            {
                clientSocket = null;
                return false;
            }
        }

        public async Task HandleTcp(IPEndPoint end)
        {
            await Task.Factory.StartNew(() =>
            {
                var logCollection = new LogCollection();
                while (LockFunction((socket) => Connected))
                {
                    if (DestructionEvent)
                        continue;

                    var _log = Read()?.TrimEnd('\0');
                    logCollection.Append(_log);

                    var log = logCollection.GetFirst();

                    if (log != null)
                        OnRead(new TelnetReadEventArgs() { IpAddress = end.Address.ToString(), Log = $"{log}\n" });
                    Thread.Sleep(10);
                }

                LockAction(socket => clientSocket = null);
                OnFinished(new TelnetReadEventArgs() { IpAddress = end.Address.ToString() });
            });
        }

        public string Read()
        {
            return LockFunction((socket) =>
            {
                if (socket == null) return null;
                if (!Connected) return string.Empty;
                if (socket.Available <= 0) return string.Empty;

                var bytes = new byte[socket.ReceiveBufferSize];
                _ = socket.Receive(bytes, SocketFlags.None);
                var returner = Encoding.GetString(bytes);

                return returner;

            });
        }

        /// <summary>
        /// Send the string to connected server.
        /// </summary>
        /// <param name="cmd">Sent string</param>
        /// <returns>Length of sent byte</returns>
        public int Write(string cmd)
        {
            var data = Encoding.GetBytes(cmd);
            var sendByte = Write(data);

            return sendByte;
        }

        /// <summary>
        /// Send the data to connected server.
        /// </summary>
        /// <param name="data">Sent byte array</param>
        /// <returns>Length of sent byte</returns>
        public int Write(byte[] data)
        {
            var sendByte = clientSocket.Send(data, data.Length, SocketFlags.None);

            return sendByte;
        }

        /// <summary>
        /// Send the string to connected server and send linebreak.
        /// </summary>
        /// <param name="cmd">Sent string</param>
        /// <returns>Length of sent byte</returns>
        public int WriteLine(string cmd)
        {
            var data = Encoding.GetBytes(cmd);
            var sendByte = WriteLine(data);

            return sendByte;
        }

        /// <summary>
        /// Send the data to connected server and send linebreak.
        /// </summary>
        /// <param name="data">Sent byte array</param>
        /// <returns>Length of sent byte</returns>
        public int WriteLine(byte[] data)
        {
            var sendByte = clientSocket.Send(data, data.Length, SocketFlags.None);

            clientSocket.Send(Crlf, Crlf.Length, SocketFlags.None);

            return sendByte;
        }

        #region IDisposable Support
        private bool _disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                LockAction((socket) =>
                {
                    if (socket == null || Connected) return;
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(false);
                    socket.Dispose();
                    clientSocket = null;
                });
            }

            _disposedValue = true;
        }

        ~TelnetClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release the telnet session.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
