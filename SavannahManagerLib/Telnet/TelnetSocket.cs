using System;
using System.Net;
using System.Net.Sockets;

namespace SvManagerLibrary.Telnet
{
    /// <summary>
    /// Socket interface.
    /// </summary>
    public interface ITelnetSocket : IDisposable
    {
        /// <summary>
        /// Gets a value that indicates whether a Socket is connected to a remote host as of the last Send or Receive operation.
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Gets the amount of data that has been received from the network and is available to be read.
        /// </summary>
        int Available { get; }

        /// <summary>
        /// Gets the remote endpoint.
        /// </summary>
        EndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets or sets a value that specifies the size of the receive buffer of the Socket.
        /// </summary>
        int ReceiveBufferSize { get; set; }

        /// <summary>
        /// Determines the status of the Socket.
        /// </summary>
        /// <param name="microSeconds">The time to wait for a response, in microseconds.</param>
        /// <param name="selectMode">One of the SelectMode values.</param>
        /// <returns>The status of the Socket based on the polling mode value passed in the mode parameter.</returns>
        bool Poll(int microSeconds, SelectMode selectMode);

        /// <summary>
        /// Disables sends and receives on a Socket.
        /// </summary>
        /// <param name="socketShutdown">One of the SocketShutdown values that specifies the operation that will no longer be allowed.</param>
        void Shutdown(SocketShutdown socketShutdown);

        /// <summary>
        /// Closes the socket connection and allows reuse of the socket.
        /// </summary>
        /// <param name="reuseSocket">true if this socket can be reused after the current connection is closed; otherwise, false.</param>
        void Disconnect(bool reuseSocket);

        /// <summary>
        /// Sends the specified number of bytes of data to a connected Socket, starting at the specified offset, and using the specified SocketFlags.
        /// </summary>
        /// <param name="buffer">An array of type Byte that contains the data to be sent.</param>
        /// <param name="offset">The position in the data buffer at which to begin sending data.</param>
        /// <param name="size">The number of bytes to send.</param>
        /// <param name="socketFlags">A bitwise combination of the SocketFlags values.</param>
        /// <returns>The number of bytes sent to the Socket.</returns>
        int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags);

        /// <summary>
        /// Sends the specified number of bytes of data to a connected Socket, using the specified SocketFlags.
        /// </summary>
        /// <param name="buffer">An array of type Byte that contains the data to be sent.</param>
        /// <param name="size">The number of bytes to send.</param>
        /// <param name="socketFlags">A bitwise combination of the SocketFlags values.</param>
        /// <returns>The number of bytes sent to the Socket.</returns>
        int Send(byte[] buffer, int size, SocketFlags socketFlags);

        /// <summary>
        /// Receives data from a bound Socket into a receive buffer, using the specified SocketFlags.
        /// </summary>
        /// <param name="buffer">An array of type Byte that is the storage location for the received data.</param>
        /// <param name="socketFlags">A bitwise combination of the SocketFlags values.</param>
        /// <returns>The number of bytes received.</returns>
        int Receive(byte[] buffer, SocketFlags socketFlags);

        /// <summary>
        /// Establishes a connection to a remote host. The host is specified by a host name and a port number.
        /// </summary>
        /// <param name="host">The name of the remote host.</param>
        /// <param name="port">The port number of the remote host.</param>
        void Connect(string host, int port);
    }

    /// <summary>
    /// Socket for test.
    /// </summary>
    public class TelnetSocket : Socket, ITelnetSocket
    {
        public TelnetSocket(SocketInformation socketInformation) : base(socketInformation)
        {
        }

        public TelnetSocket(SocketType socketType, ProtocolType protocolType) : base(socketType, protocolType)
        {
        }

        public TelnetSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) : base(addressFamily, socketType, protocolType)
        {
        }
    }
}
