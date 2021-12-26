using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models.Interfaces
{
    /// <summary>
    /// Interface of releasing the object.
    /// </summary>
    interface IRelease : IDisposable
    {
        /// <summary>
        /// Release the object. Unlike Dispose, the object can be reused.
        /// </summary>
        void Release();
    }
}
