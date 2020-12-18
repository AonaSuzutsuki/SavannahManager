using System.Threading.Tasks;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Permissions.Models
{
    public abstract class AbstractGetSteamIdModel : ModelBase
    {
        private string _steam64Id;
        private bool _canWrite;

        public string Steam64Id
        {
            get => _steam64Id;
            protected set => SetProperty(ref _steam64Id, value);
        }

        public bool IsWritten { get; set; }

        public bool CanWrite
        {
            get => _canWrite;
            protected set => SetProperty(ref _canWrite, value);
        }

        public abstract Task Analyze(string url);
    }
}