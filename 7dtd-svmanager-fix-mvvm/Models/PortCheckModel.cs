using CommonStyleLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class PortCheckModel : ModelBase
    {
        #region Fields
        private string externalIpAddress;
        private string portText;
        private string statusLabel;
        private uint port;
        #endregion

        #region Properties
        public string ExternalIpAddress
        {
            get => externalIpAddress;
            set => SetProperty(ref externalIpAddress, value);
        }

        public string PortText
        {
            get => portText;
            set
            {
                SetProperty(ref portText, value);
                uint.TryParse(value, out port);
            }
        }

        public string StatusLabel
        {
            get => statusLabel;
            set => SetProperty(ref statusLabel, value);
        }
        #endregion

        public async Task AutoSetIpAddress()
        {
            var ip = await IpAddressManager.GetExternalIpAddress(ConstantValues.ExternalIpUrl);
            ExternalIpAddress = ip;
        }

        public async Task CheckPort()
        {
            var portChecker = new PortChecker(ExternalIpAddress, port);
            var isOpened = await portChecker.Search();
            StatusLabel = isOpened ? "Opened!" : "Not Opened!";
        }
    }
}
