using System;
using System.Net.Sockets;
using System.Threading;

namespace SvManagerLibrary.Telnet
{
    /// <summary>
    /// Manage Telnet Connection.
    /// </summary>
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
            public string log;
        }
        public delegate void TelnetReadedEventHandler(object sender, TelnetReadedEventArgs e);
        public event TelnetReadedEventHandler Readed;
        protected virtual void OnReaded(TelnetReadedEventArgs e)
        {
            Readed?.Invoke(this, e);
        }
        #endregion

        bool isAsync = false;
        public TelnetClient(bool isAsync = false)
        {
            this.isAsync = isAsync;
        }

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

        public bool ThreadExited
        {
            get
            {
                bool isAlive = false;
                try
                {
                    isAlive = readThread.IsAlive;
                }
                catch { }
                return isAlive;
            }
        }

        private Socket _clientSocket = null;
        private Thread readThread;

        /// <summary>
        /// Connect to a server with telnet.
        /// </summary>
        /// <param name="ipAddress">IP Address</param>
        /// <param name="port">Port</param>
        /// <returns></returns>
        public bool Connect(string ipAddress, int port)
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = ReceiveTimeout,
                ReceiveBufferSize = ReceiveBufferSize
            };
            try
            {
                _clientSocket.Connect(ipAddress, port);
                if (isAsync)
                {
                    readThread = new Thread(InternalRead);
                    readThread.Start();
                }
                return true;
            }
            catch
            {
                _clientSocket = null;
                return false;
            }
        }

        public void Login(string password)
        {
            WriteLine(password);
        }
        
        /// <summary>
        /// Get a state of connection
        /// </summary>
        public bool Connected
        {
            get
            {
                if (_clientSocket == null)
                {
                    return false;
                }
                else
                {
                    bool isPoll = _clientSocket.Poll(0, SelectMode.SelectRead);
                    bool isAvailable = (_clientSocket.Available == 0);
                    if (isPoll & isAvailable)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        /// <summary>
        /// Get a returning string from server.
        /// </summary>
        /// <returns>Rturning string</returns>
        public string Read()
        {
            if (_clientSocket != null)
            {
                byte[] bytes = new byte[_clientSocket.ReceiveBufferSize];
                if (_clientSocket.Connected)
                {
                    if (_clientSocket.Available > 0)
                    {
                        int bytesReceive = _clientSocket.Receive(bytes, SocketFlags.None);
                        string returnstr = Encoding.GetString(bytes);

                        return returnstr;
                    }
                }

                return string.Empty;
            }
            return null;
        }

        bool threadStop = false;
        private void InternalRead()
        {
            while (true)
            {
                if (threadStop)
                {
                    break;
                }

                string readLog = Read().TrimEnd('\0');
                if (!string.IsNullOrEmpty(readLog))
                {
                    TelnetReadedEventArgs telnetReadedEventArgs = new TelnetReadedEventArgs()
                    {
                        log = readLog,
                    };
                    OnReaded(telnetReadedEventArgs);
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Send the string to connected server.
        /// </summary>
        /// <param name="cmd">Sent string</param>
        /// <returns>Length of sent byte</returns>
        public int Write(string cmd)
        {
            byte[] data = Encoding.GetBytes(cmd);
            int sendByte = _clientSocket.Send(data, data.Length, SocketFlags.None);

            return sendByte;
        }

        /// <summary>
        /// Send the data to connected server.
        /// </summary>
        /// <param name="data">Sent byte array</param>
        /// <returns>Length of sent byte</returns>
        public int Write(byte[] data)
        {
            int sendByte = _clientSocket.Send(data, data.Length, SocketFlags.None);

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
            int sendByte = _clientSocket.Send(data, data.Length, SocketFlags.None);

            byte[] newLine = { 0x0D, 0x0A };
            _clientSocket.Send(newLine, newLine.Length, SocketFlags.None);

            return sendByte;
        }

        /// <summary>
        /// Send the data to connected server and send linebreak.
        /// </summary>
        /// <param name="data">Sent byte array</param>
        /// <returns>Length of sent byte</returns>
        public int WriteLine(byte[] data)
        {
            int sendByte = _clientSocket.Send(data, data.Length, SocketFlags.None);

            byte[] newLine = { 0x0D, 0x0A };
            _clientSocket.Send(newLine, newLine.Length, SocketFlags.None);

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
                    if (_clientSocket != null)
                    {
                        _clientSocket.Shutdown(SocketShutdown.Both);
                        _clientSocket.Disconnect(false);
                        _clientSocket.Dispose();
                    }
                    _clientSocket = null;

                    threadStop = true;
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
