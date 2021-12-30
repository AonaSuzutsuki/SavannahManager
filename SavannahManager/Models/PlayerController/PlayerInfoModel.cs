using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Models.PlayerController
{
    public class PlayerInfoModel : ModelBase
    {
        private const string Permalink = "http://steamcommunity.com/profiles/{0}";
        
        public string UserPageLink { get; private set; }

        public void SetUserPageLink(string steamId)
        {
            UserPageLink = Permalink.FormatString(steamId);
        }
    }
}
