using CommonLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class IpAddressGetterModel : ModelBase
    {

        public void SetIpAddress()
        {
            ExternalIpAddress = IpAddressManager.GetExternalIpAddress("https://aonsztk.xyz/api/?mode=externalip");
            LocalIpAddress = IpAddressManager.GetLocalIPAddress();
        }

        #region Fields
        private string externalIpAddress;
        private string localIpAddress;
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
        #endregion
    }
}
