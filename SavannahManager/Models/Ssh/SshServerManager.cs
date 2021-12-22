using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace _7dtd_svmanager_fix_mvvm.Models.Ssh
{
    public class SshServerManager : IDisposable
    {

        #region Fields

        private SshClient _sshClient;
        private ShellStream _shellStream;
        private StreamReader _shellStreamReader;
        private StreamWriter _shellStreamWriter;
        private ConnectionInfo _connectionInfo;

        private readonly string _hostAddress;
        private readonly int _hostPort;

        #endregion

        #region Properties



        #endregion

        public SshServerManager(string hostAddress, int hostPort = 22)
        {
            _hostAddress = hostAddress;
            _hostPort = hostPort;
        }

        public void SetLoginInformation(string user, string password)
        {
            _connectionInfo = new ConnectionInfo(_hostAddress, _hostPort, user,
                new PasswordAuthenticationMethod(user, password));
        }

        public void SetLoginInformation(string user, string passPhrase, string keyPath)
        {
            _connectionInfo = new ConnectionInfo(_hostAddress, _hostPort, user, 
                new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile(keyPath, passPhrase)));
        }

        public bool Connect()
        {
            if (_connectionInfo == null)
                return false;

            _sshClient = new SshClient(_connectionInfo);
            _sshClient.Connect();

            if (!_sshClient.IsConnected)
                return false;

            _shellStream = _sshClient.CreateShellStream("savannah_manager", 0, 0, 0, 0, 1024);
            _shellStream.DataReceived += (sender, args) =>
            {
                Debug.WriteLine(_shellStream.Read());
            };

            return true;
        }

        public void SendCommand(string command)
        {
            _shellStream.WriteLine(command);
            _shellStream.Flush();
        }

        public void StartServer(string command)
        {
            SendCommand($"nohup sh -c '( ( {command} &>/dev/null ) & )'");
        }

        #region IDisposable Members
        public void Dispose()
        {
            _shellStream?.Dispose();
            _sshClient?.Dispose();
        }
        #endregion
    }
}
