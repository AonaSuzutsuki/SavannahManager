using System.Threading;

namespace SvManagerLibrary.Telnet
{
    /// <summary>
    /// Calculates the Telnet wait time.
    /// </summary>
    public class TelnetWaiter
    {
        private int _count;

        /// <summary>
        /// Checks if it is loopable.
        /// </summary>
        public bool CanLoop => ElapsedSeconds < MaxMilliseconds;

        /// <summary>
        /// Gets or sets the maximum wait time in milliseconds.
        /// </summary>
        public int MaxMilliseconds { get; set; } = 2000;

        /// <summary>
        /// Get the elapsed time for executing Next method.
        /// </summary>
        public int ElapsedSeconds => _count * SleepTime;

        /// <summary>
        /// Gets or sets the time to sleep the thread in milliseconds.
        /// </summary>
        public int SleepTime { get; set; } = 100;

        /// <summary>
        /// Only SleepTime will sleep and proceed to the next.
        /// </summary>
        public void Next()
        {
            _count++;
            Thread.Sleep(SleepTime);
        }
    }
}
