using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SvManagerLibrary.Telnet
{
    public class TelnetWaiter
    {
        private int count;

        public bool CanLoop => Count < MaxMilliseconds;
        public int MaxMilliseconds { get; set; } = 2000;

        public int Count => count * SleepTime;

        public int SleepTime { get; set; } = 100;

        public void Next()
        {
            count++;
            Thread.Sleep(SleepTime);
        }
    }
}
