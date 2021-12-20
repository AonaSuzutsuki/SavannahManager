using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models.Interfaces
{
    public interface IMainWindowServerStart
    {
        public bool IsConnected { get; }

        Task ServerStart();
        bool ServerStop();
    }
}
