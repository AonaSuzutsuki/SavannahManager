using CommonStyleLib.Models;
using CommonExtensionLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class IpAddressGetterModel : ModelBase
    {
        #region Fields
        private string externalIpAddress;
        private string localIpAddress;
        private string statusLabel;
        #endregion

        #region Properties
        public string ExternalIpAddress
        {
            get => externalIpAddress;
            set => SetProperty(ref externalIpAddress, value);
        }
        public string LocalIpAddress
        {
            get => localIpAddress;
            set => SetProperty(ref localIpAddress, value);
        }
        public string StatusLabel
        {
            get => statusLabel;
            set => SetProperty(ref statusLabel, value);
        }
        #endregion


        public void SetIpAddress()
        {
            ExternalIpAddress = IpAddressManager.GetExternalIpAddress(ConstantValues.ExternalIpUrl);
            LocalIpAddress = IpAddressManager.GetLocalIPAddress();
        }

        public void CopyClipboard(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Clipboard.SetText(text);

            var format = "Copied {0}.";
            StatusLabel = format.FormatString(text);
        }
    }
}
