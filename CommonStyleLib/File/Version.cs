using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.File
{
    public class Version
    {
        private static System.Reflection.Assembly asm = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetVersion(string filePath = null)
        {
            if (filePath == null)
            {
                if (asm == null)
                    asm = System.Reflection.Assembly.GetCallingAssembly();
                var ver = asm.GetName().Version;

                return ver.ToString();
            }
            else
            {
                var vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath);
                return vi.FileVersion;
            }
        }
    }
}
