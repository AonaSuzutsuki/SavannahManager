using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{
    public class TelnetCounter
    {
        public bool CanLoop => Count < Max;
        public int Max { get; set; } = 20;

        public int Count { get; private set; } = 0;

        public void Next()
        {
            Count++;
        }
    }
}
