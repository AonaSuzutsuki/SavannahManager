using CommonStyleLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models.WindowModel
{
    public class PortCheckModel : ModelBase
    {
        #region Fields
        private string _externalIpAddress;
        private string _portText;
        private string _statusLabel;
        private uint _port;
        #endregion

        #region Properties
        public string ExternalIpAddress
        {
            get => _externalIpAddress;
            set => SetProperty(ref _externalIpAddress, value);
        }

        public string PortText
        {
            get => _portText;
            set
            {
                SetProperty(ref _portText, value);
                uint.TryParse(value, out _port);
            }
        }

        public string StatusLabel
        {
            get => _statusLabel;
            set => SetProperty(ref _statusLabel, value);
        }
        #endregion

        public async Task AutoSetIpAddress()
        {
            var ip = await IpAddressManager.GetExternalIpAddress(ConstantValues.ExternalIpUrl);
            ExternalIpAddress = ip;
        }

        public async Task CheckPort()
        {
            var portChecker = new PortChecker(ExternalIpAddress, _port);
            var isOpened = await portChecker.Search();
            StatusLabel = isOpened ? "Opened!" : "Not Opened!";
        }
    }
}
