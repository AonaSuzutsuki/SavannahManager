using System.Threading.Tasks;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Permissions.Models
{
    public abstract class AbstractGetSteamIdModel : ModelBase
    {
        private string steam64Id;
        private bool canWrite;

        public string Steam64Id
        {
            get => steam64Id;
            protected set => SetProperty(ref steam64Id, value);
        }

        public bool IsWritten { get; set; }

        public bool CanWrite
        {
            get => canWrite;
            protected set => SetProperty(ref canWrite, value);
        }

        public abstract Task Analyze(string url);
    }
}