using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{
    public class TelnetCounter
    {
        public bool CanLoop => count < Max;
        public int Max { get; set; } = 20;

        private int count = 0;

        public void Next()
        {
            count++;
        }
    }
}
