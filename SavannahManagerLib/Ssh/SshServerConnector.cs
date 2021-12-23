using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Subjects;
using Renci.SshNet;

namespace SvManagerLibrary.Ssh
{
    public class SshServerConnector : IDisposable
    {
        #region Fields

        private SshClient _sshClient;
        private ShellStream _shellStream;
        private StreamReader _streamReader;
        private ConnectionInfo _connectionInfo;

        private readonly string _hostAddress;
        private readonly int _hostPort;

        #endregion

        #region Properties



        #endregion

        #region Events

        private readonly Subject<StreamReader> _sshDataReceivedSubject = new();
        public IObservable<StreamReader> SshDataReceived => _sshDataReceivedSubject;

        #endregion

        public SshServerConnector(string hostAddress, int hostPort = 22)
        {
            _hostAddress = hostAddress;
            _hostPort = hostPort;
        }

        public void SetLoginInformation(string user, string password)
        {
            _connectionInfo = new ConnectionInfo(_hostAddress, _hostPort, user,
                new PasswordAuthenticationMethod(user, password))
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
        }

        public void SetLoginInformation(string user, string passPhrase, string keyPath)
        {
            _connectionInfo = new ConnectionInfo(_hostAddress, _hostPort, user,
                new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile(keyPath, passPhrase)))
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
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
            _streamReader = new StreamReader(_shellStream);
            _shellStream.DataReceived += (_, _) =>
            {
                _sshDataReceivedSubject?.OnNext(_streamReader);
            };

            return true;
        }

        public void WriteLine(string command)
        {
            _shellStream.WriteLine(command);
            _shellStream.Flush();
        }

        #region IDisposable Members
        public void Dispose()
        {
            _streamReader?.Dispose();
            _shellStream?.Dispose();
            _sshClient?.Dispose();
        }
        #endregion
    }
}
