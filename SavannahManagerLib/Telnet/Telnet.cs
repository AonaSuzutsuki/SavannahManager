using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{

    public class TelnetClient : IDisposable, ITelnetClient
    {
        public enum BreakLineType
        {
            Cr,
            Lf,
            CrLf,
        }

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

        /// <summary>
        /// Event arguments for Telnet.
        /// </summary>
        public class TelnetReadEventArgs : EventArgs
        {
            /// <summary>
            /// IP address of the connection
            /// </summary>
            public string IpAddress;

            /// <summary>
            /// The log.
            /// </summary>
            public string Log;
        }

        /// <summary>
        /// The delegate of some events.
        /// </summary>
        /// <param name="sender">The sender who called the delegate.</param>
        /// <param name="e">Event argument.</param>
        public delegate void TelnetReadEventHandler(object sender, TelnetReadEventArgs e);

        /// <summary>
        /// This event occurs when the log is read by the telnet.
        /// </summary>
        public event TelnetReadEventHandler ReadEvent;

        /// <summary>
        /// This event occurs when the Telnet connection is started.
        /// </summary>
        public event TelnetReadEventHandler Started;

        /// <summary>
        /// This event occurs when the Telnet connection is terminated.
        /// </summary>
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
        public int ReceiveTimeout { get; set; } = 5000;

        /// <summary>
        /// Get or Set Receiving Buffer Size.
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 10240;

        /// <summary>
        /// The maximum value of the wait time used by the DestructionEventRead method.
        /// </summary>
        public int TelnetEventWaitTime { get; set; } = 2000;

        /// <summary>
        /// Breakline code to be sent with WriteLine methods.
        /// </summary>
        public BreakLineType BreakLine { get; set; } = BreakLineType.CrLf;

        public byte[] BreakLineData
        {
            get
            {
                return BreakLine switch
                {
                    BreakLineType.Cr => Cr,
                    BreakLineType.Lf => Lf,
                    BreakLineType.CrLf => Crlf,
                    _ => Crlf,
                };
            }
        }

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
                var isPoll = _clientSocket?.Poll(0, SelectMode.SelectRead) ?? false;
                var isAvailable = _clientSocket != null && _clientSocket.Available == 0;
                return !(isPoll & isAvailable);
            }
        }

        private bool _destructionEvent;
        private ITelnetSocket _clientSocket;

        /// <summary>
        /// Initialize.
        /// </summary>
        public TelnetClient()
        {
            _clientSocket = new TelnetSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveTimeout = ReceiveTimeout,
                ReceiveBufferSize = ReceiveBufferSize
            };
        }

        /// <summary>
        /// Initialize it with the TelnetSocket argument.
        /// </summary>
        /// <param name="socket">TelnetSocket.</param>
        public TelnetClient(ITelnetSocket socket)
        {
            _clientSocket = socket;
        }

        private void LockAction(Action<ITelnetSocket> action)
        {
            if (_clientSocket == null) return;
            lock (_clientSocket)
            {
                action(_clientSocket);
            }
        }

        private T LockFunction<T>(Func<ITelnetSocket, T> func)
        {
            if (_clientSocket == null) return default;
            lock (_clientSocket)
            {
                return func(_clientSocket);
            }
        }

        /// <summary>
        /// Connect to address and port.
        /// </summary>
        /// <param name="address">The address to connect.</param>
        /// <param name="port">The port to connect.</param>
        /// <returns>Whether the connection was established or not.</returns>
        public bool Connect(string address, int port)
        {
            try
            {
                _clientSocket.Connect(address, port);

                var endPoint = (IPEndPoint)_clientSocket.RemoteEndPoint;
                OnStarted(new TelnetReadEventArgs() { IpAddress = endPoint.Address.ToString() });
                if (ReadEvent != null)
                    _ = HandleTcp(endPoint);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task HandleTcp(IPEndPoint end)
        {
            await Task.Factory.StartNew(() =>
            {
                var logCollection = new LogCollection();
                while (LockFunction((socket) => Connected))
                {
                    if (!_destructionEvent)
                    {
                        var temp = Read()?.TrimEnd('\0');
                        logCollection.Append(temp);

                        var log = logCollection.GetFirst();

                        if (log != null)
                            OnRead(new TelnetReadEventArgs() { IpAddress = end.Address.ToString(), Log = $"{log}\n" });
                    }

                    Thread.Sleep(10);
                }
                OnFinished(new TelnetReadEventArgs() { IpAddress = end.Address.ToString() });
            });
        }

        /// <summary>
        /// Read log from the connection.
        /// </summary>
        /// <returns>The read log.</returns>
        public string Read()
        {
            return LockFunction((socket) =>
            {
                if (socket == null) return string.Empty;
                if (!Connected) return string.Empty;
                if (socket.Available <= 0) return string.Empty;

                var bytes = new byte[socket.ReceiveBufferSize];
                _ = socket.Receive(bytes, SocketFlags.None);
                var returner = Encoding.GetString(bytes);

                return returner;

            });
        }

        /// <summary>
        /// Send command and suppresses the occurrence of events and reads the log.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <returns>The read log.</returns>
        public string DestructionEventRead(string cmd)
        {
            _destructionEvent = true;

            WriteLine(cmd);
            var counter = new TelnetWaiter
            {
                MaxMilliseconds = TelnetEventWaitTime
            };

            var logCollection = new LogCollection();
            while (counter.CanLoop)
            {
                var log = Read().TrimEnd('\0');
                logCollection.Append(log);

                Console.WriteLine(logCollection.ToString());

                if (!string.IsNullOrEmpty(logCollection.GetFirstNoneRemove()))
                    break;

                counter.Next();
            }

            _destructionEvent = false;

            return logCollection.ToString();
        }

        /// <summary>
        /// Send command and suppresses the occurrence of events and reads the log.
        /// Detect the end by regular expression in the last read line.
        /// </summary>
        /// <param name="cmd">The command to send</param>
        /// <param name="expressionForLast">A regular expression indicating the end of reading.</param>
        /// <returns>The read log.</returns>
        public string DestructionEventRead(string cmd, string expressionForLast)
        {
            static bool IsMatch(LogCollection logCollection, string expression)
            {
                var regex = new Regex(expression);
                return logCollection.ReversEnumerable().Select(log => regex.Match(log.ToString())).Any(match => match.Success);
            }

            _destructionEvent = true;

            WriteLine(cmd);
            var counter = new TelnetWaiter
            {
                MaxMilliseconds = TelnetEventWaitTime
            };

            var logCollection = new LogCollection();
            while (counter.CanLoop)
            {
                var log = Read().TrimEnd('\0');
                logCollection.Append(log);

                if (IsMatch(logCollection, expressionForLast))
                    break;

                counter.Next();
            }

            _destructionEvent = false;

            return logCollection.ToString();
        }

        /// <summary>
        /// Get the time required to communicate with the destination.
        /// </summary>
        /// <param name="maxMilliseconds">Timeout in milliseconds</param>
        /// <returns>The time required to communicate with the destination</returns>
        public int CalculateWaitTime(int maxMilliseconds = 10000)
        {
            _destructionEvent = true;

            WriteLine("gt");
            var counter = new TelnetWaiter
            {
                MaxMilliseconds = maxMilliseconds
            };

            var logCollection = new LogCollection();
            while (counter.CanLoop)
            {
                var log = Read().TrimEnd('\0');
                logCollection.Append(log);
                Console.WriteLine(logCollection.ToString());

                if (!string.IsNullOrEmpty(logCollection.GetFirstNoneRemove()))
                    break;

                counter.Next();
            }

            _destructionEvent = false;

            return counter.ElapsedSeconds;
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
            var sendByte = _clientSocket.Send(data, data.Length, SocketFlags.None);

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
            var concat = data.Concat(BreakLineData).ToArray();
            var sendByte = _clientSocket.Send(concat, concat.Length, SocketFlags.None);

            return sendByte;
        }

        #region IDisposable Support
        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                LockAction((socket) =>
                {
                    if (socket != null && socket.Connected)
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Disconnect(false);
                        socket.Dispose();
                    }

                    _clientSocket = null;
                });
            }

            _disposedValue = true;
        }

        ~TelnetClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// Release the telnet connection and resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
