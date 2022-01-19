using CommonStyleLib.Models;
using CommonExtensionLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _7dtd_svmanager_fix_mvvm.Models.WindowModel
{
    public class IpAddressGetterModel : ModelBase
    {
        #region Fields
        private string _externalIpAddress;
        private string _localIpAddress;
        private string _statusLabel;
        #endregion

        #region Properties
        public string ExternalIpAddress
        {
            get => _externalIpAddress;
            set => SetProperty(ref _externalIpAddress, value);
        }
        public string LocalIpAddress
        {
            get => _localIpAddress;
            set => SetProperty(ref _localIpAddress, value);
        }
        public string StatusLabel
        {
            get => _statusLabel;
            set => SetProperty(ref _statusLabel, value);
        }
        #endregion


        public async Task SetIpAddress()
        {
            ExternalIpAddress = await IpAddressManager.GetExternalIpAddress(ConstantValues.ExternalIpUrl);
            LocalIpAddress = IpAddressManager.GetLocalIpAddress();
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
