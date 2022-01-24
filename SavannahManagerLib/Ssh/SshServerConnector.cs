using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using Renci.SshNet;
using SavannahXmlLib.Extensions;

namespace SvManagerLibrary.Ssh
{
    public abstract class AbstractSshServerConnector
    {

        #region Fields
        
        protected ConnectionInfo ConnectionInfo;

        protected readonly string HostAddress;
        protected readonly int HostPort;

        #endregion

        protected AbstractSshServerConnector(string hostAddress, int hostPort = 22)
        {
            HostAddress = hostAddress;
            HostPort = hostPort;
        }

        public void SetLoginInformation(string user, string password)
        {
            ConnectionInfo = new ConnectionInfo(HostAddress, HostPort, user,
                new PasswordAuthenticationMethod(user, password))
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
        }

        public void SetLoginInformation(string user, string passPhrase, string keyPath)
        {
            ConnectionInfo = new ConnectionInfo(HostAddress, HostPort, user,
                new PrivateKeyAuthenticationMethod(user, new PrivateKeyFile(keyPath, passPhrase)))
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
        }

        public abstract bool Connect();
    }

    public class SftpServerConnector : AbstractSshServerConnector, IDisposable
    {
        public class SftpFileInfo
        {
            public string Path { get; set; }
            public bool IsDirectory { get; set; }
        }

        #region Fields

        private SftpClient _sftpClient;

        #endregion

        #region Properties

        public string WorkingDirectory => _sftpClient.WorkingDirectory;

        #endregion

        public SftpServerConnector(string hostAddress, int hostPort = 22) : base(hostAddress, hostPort)
        {
        }

        public override bool Connect()
        {
            if (ConnectionInfo == null)
                return false;

            _sftpClient = new SftpClient(ConnectionInfo);
            _sftpClient.Connect();

            return _sftpClient.IsConnected;
        }

        public void Download(string path, Stream outStream, Action<ulong> callback = null)
        {
            _sftpClient.DownloadFile(path, outStream, callback);
        }

        public void Upload(string path, Stream inStream, Action<ulong> callback = null)
        {
            _sftpClient.UploadFile(inStream, path, callback);
        }

        public IEnumerable<SftpFileInfo> GetItems()
        {
            var items = _sftpClient.ListDirectory(WorkingDirectory);
            return items.Select(x => new SftpFileInfo
            {
                Path = x.FullName,
                IsDirectory = x.IsDirectory
            });
        }

        public void ChangeDirectory(string path)
        {
            _sftpClient.ChangeDirectory(path);
        }

        public void Dispose()
        {
            _sftpClient?.Dispose();
        }
    }

    public class SshServerConnector : AbstractSshServerConnector, IDisposable
    {
        #region Fields

        private SshClient _sshClient;
        private ShellStream _shellStream;
        private StreamReader _streamReader;

        #endregion

        #region Properties
        

        #endregion

        #region Events

        private readonly Subject<StreamReader> _sshDataReceivedSubject = new();
        public IObservable<StreamReader> SshDataReceived => _sshDataReceivedSubject;

        #endregion

        public SshServerConnector(string hostAddress, int hostPort = 22) : base(hostAddress, hostPort)
        {
        }
        
        public override bool Connect()
        {
            if (ConnectionInfo == null)
                return false;

            _sshClient = new SshClient(ConnectionInfo);
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
            _sshDataReceivedSubject.Dispose();
        }
        #endregion
    }
}
