using System;
using System.Net;
using System.Net.Sockets;

namespace SvManagerLibrary.Telnet
{
    public interface ITelnetSocket : IDisposable
    {
        bool Connected { get; }
        int Available { get; }
        EndPoint RemoteEndPoint { get; }
        int ReceiveBufferSize { get; set; }

        bool Poll(int microSeconds, SelectMode selectMode);
        void Shutdown(SocketShutdown socketShutdown);
        void Disconnect(bool reuseSocket);

        int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags);
        int Send(byte[] buffer, int size, SocketFlags socketFlags);
        int Receive(byte[] buffer, SocketFlags socketFlags);
        void Connect(string host, int port);
    }

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
