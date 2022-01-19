using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SvManagerLibrary.Ssh;

namespace _7dtd_svmanager_fix_mvvm.Models.Ssh
{
    public class SshServerManager : IDisposable
    {

        #region Fields

        private readonly SshServerConnector _sshServerConnector;

        #endregion

        #region Properties



        #endregion

        public SshServerManager(string hostAddress, int hostPort = 22)
        {
            _sshServerConnector = new SshServerConnector(hostAddress, hostPort);
            _sshServerConnector.SshDataReceived.Subscribe(reader => Debug.WriteLine(reader.ReadToEnd()));
        }

        public bool Connect(string user, string password)
        {
            _sshServerConnector.SetLoginInformation(user, password);
            return _sshServerConnector.Connect();
        }

        public bool Connect(string user, string passPhrase, string keyPath)
        {
            _sshServerConnector.SetLoginInformation(user, passPhrase, keyPath);
            return _sshServerConnector.Connect();
        }

        public void StartServer(string command)
        {
            _sshServerConnector.WriteLine($"nohup sh -c '( ( {command} &>/dev/null ) & )' > /dev/null");
        }

        #region IDisposable Members
        public void Dispose()
        {
            _sshServerConnector?.Dispose();
        }
        #endregion
    }
}
