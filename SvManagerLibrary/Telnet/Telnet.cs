using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{

    public class TelnetClient : IDisposable
    {
        #region StaticMember
        /// <summary>
        /// Carriage Return.
        /// </summary>
        public static byte[] CR { get; } = { 0x0D };
        /// <summary>
        /// Line Feed.
        /// </summary>
        public static byte[] LF { get; } = { 0x0A };
        /// <summary>
        /// Carriage Return & Line Feed.
        /// </summary>
        public static byte[] CRLF { get; } = { 0x0D, 0x0A };
        #endregion

        #region ReadedEvent
        public class TelnetReadedEventArgs : EventArgs
        {
            public string IpAddress;
            public string Log;
        }
        public delegate void TelnetReadedEventHandler(object sender, TelnetReadedEventArgs e);
        public event TelnetReadedEventHandler Readed;
        public event TelnetReadedEventHandler Started;
        public event TelnetReadedEventHandler Finished;
        protected virtual void OnReaded(TelnetReadedEventArgs e)
        {
            Readed?.Invoke(this, e);
        }
        protected virtual void OnStarted(TelnetReadedEventArgs e)
        {
            Started?.Invoke(this, e);
        }
        protected virtual void OnFinished(TelnetReadedEventArgs e)
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
        public int ReceiveBufferSize { set; get; } = 8192;

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
                bool isPoll = clientSocket != null ? clientSocket.Poll(0, SelectMode.SelectRead) : false;
                bool isAvailable = clientSocket != null ? clientSocket.Available == 0 : false;
                if (isPoll & isAvailable)
                {
                    return false;
                }
                return true;
            }
        }

        public bool DestructionEvent = false;

        public Socket clientSocket;

        public void LockAction(Action<Socket> action)
        {
            if (clientSocket != null)
            {
                lock (clientSocket)
                {
                    action(clientSocket);
                }
            }
        }

        public T LockFunction<T>(Func<Socket, T> func)
        {
            if (clientSocket != null)
            {
                lock (clientSocket)
                {
                    return func(clientSocket);
                }
            }
            return default;
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
                OnStarted(new TelnetReadedEventArgs() { IpAddress = endPoint.Address.ToString() });
                if (Readed != null)
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
                while (LockFunction((socket) => Connected))
                {
                    if (DestructionEvent)
                        continue;

                    var log = Read()?.TrimEnd('\0');
                    if (!string.IsNullOrEmpty(log))
                        OnReaded(new TelnetReadedEventArgs() { IpAddress = end.Address.ToString(), Log = log });
                }
                OnFinished(new TelnetReadedEventArgs() { IpAddress = end.Address.ToString() });
            });
        }

        public string Read()
        {
            return LockFunction((socket) =>
            {
                if (socket != null)
                {
                    byte[] bytes = new byte[socket.ReceiveBufferSize];
                    if (Connected)
                    {
                        if (socket.Available > 0)
                        {
                            _ = socket.Receive(bytes, SocketFlags.None);
                            string returnstr = Encoding.GetString(bytes);

                            return returnstr;
                        }
                    }

                    return string.Empty;
                }
                return null;
            });
        }

        /// <summary>
        /// Send the string to connected server.
        /// </summary>
        /// <param name="cmd">Sent string</param>
        /// <returns>Length of sent byte</returns>
        public int Write(string cmd)
        {
            byte[] data = Encoding.GetBytes(cmd);
            int sendByte = Write(data);

            return sendByte;
        }

        /// <summary>
        /// Send the data to connected server.
        /// </summary>
        /// <param name="data">Sent byte array</param>
        /// <returns>Length of sent byte</returns>
        public int Write(byte[] data)
        {
            int sendByte = clientSocket.Send(data, data.Length, SocketFlags.None);

            return sendByte;
        }

        /// <summary>
        /// Send the string to connected server and send linebreak.
        /// </summary>
        /// <param name="cmd">Sent string</param>
        /// <returns>Length of sent byte</returns>
        public int WriteLine(string cmd)
        {
            byte[] data = Encoding.GetBytes(cmd);
            int sendByte = WriteLine(data);

            return sendByte;
        }

        /// <summary>
        /// Send the data to connected server and send linebreak.
        /// </summary>
        /// <param name="data">Sent byte array</param>
        /// <returns>Length of sent byte</returns>
        public int WriteLine(byte[] data)
        {
            int sendByte = clientSocket.Send(data, data.Length, SocketFlags.None);

            byte[] newLine = { 0x0D, 0x0A };
            clientSocket.Send(newLine, newLine.Length, SocketFlags.None);

            return sendByte;
        }

        #region IDisposable Support
        private bool _disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    LockAction((socket) =>
                    {
                        if (socket != null)
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Disconnect(false);
                            socket.Dispose();
                            clientSocket = null;
                        }
                    });
                }

                _disposedValue = true;
            }
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
