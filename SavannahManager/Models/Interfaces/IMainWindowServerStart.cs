using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models.Interfaces
{
    public class MainWindowServerStart
    {
        public IMainWindowServerStart Model { get; set; }

        public bool IsSsh { get; }

        public MainWindowServerStart(IMainWindowServerStart model, bool isSsh)
        {
            Model = model;
            IsSsh = isSsh;
        }
    }

    public interface IMainWindowServerStart
    {
        public bool IsConnected { get; }
        public SettingLoader Setting { get; }
        public int CurrentProcessId { get; }

        Task<bool> ServerStart();
        Task<bool> ServerStartWithSsh();
        bool ServerStop();
    }
}
