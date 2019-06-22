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

        event TelnetClient.TelnetReadedEventHandler Finished;
        event TelnetClient.TelnetReadedEventHandler Readed;
        event TelnetClient.TelnetReadedEventHandler Started;

        bool Connect(string address, int port);
        void Dispose();
        Task HandleTcp(IPEndPoint end);
        void LockAction(Action<Socket> action);
        T LockFunction<T>(Func<Socket, T> func);
        string Read();
        int Write(byte[] data);
        int Write(string cmd);
        int WriteLine(byte[] data);
        int WriteLine(string cmd);
    }
}