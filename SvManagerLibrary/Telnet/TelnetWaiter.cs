using System.Threading;

namespace SvManagerLibrary.Telnet
{
    public class TelnetWaiter
    {
        private int _count;

        public bool CanLoop => Count < MaxMilliseconds;
        public int MaxMilliseconds { get; set; } = 2000;

        public int Count => _count * SleepTime;

        public int SleepTime { get; set; } = 100;

        public void Next()
        {
            _count++;
            Thread.Sleep(SleepTime);
        }
    }
}
