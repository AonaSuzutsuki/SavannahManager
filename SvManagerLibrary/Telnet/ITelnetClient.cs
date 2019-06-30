using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{
    public interface ITelnetClient
    {
        bool Connected { get; }
        bool DestructionEvent { get; set; }
        Encoding Encoding { get; set; }
        int ReceiveBufferSize { get; set; }
        int ReceiveTimeout { get; set; }

        event TelnetClient.TelnetReadEventHandler Finished;
        event TelnetClient.TelnetReadEventHandler ReadEvent;
        event TelnetClient.TelnetReadEventHandler Started;

        bool Connect(string address, int port);
        void Dispose();
        Task HandleTcp(IPEndPoint end);
        string Read();
        int Write(byte[] data);
        int Write(string cmd);
        int WriteLine(byte[] data);
        int WriteLine(string cmd);
    }
}