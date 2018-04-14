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
            var jsonLoader = new JsonLoader("https://aonsztk.xyz/api/?mode=externalip");
            Console.WriteLine(jsonLoader.ToString());
        }
    }
}
