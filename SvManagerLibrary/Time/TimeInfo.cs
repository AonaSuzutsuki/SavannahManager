namespace SvManagerLibrary.Time
{
    public class TimeInfo
    {
        public int Day { set; get; } = 1;

        private int _hour = 0;
        public int Hour
        {
            set
            {
                if (value > 23)
                    _hour = 23;
                else if (value < 0)
                    _hour = 0;
                else
                    _hour = value;
            }
            get
            {
                return _hour;
            }
        }
        private int _minute = 0;
        public int Minute
        {
            set
            {
                if (value > 60)
                    _minute = 60;
                else if (value < 0)
                    _minute = 0;
                else
                    _minute = value;
            }
            get
            {
                return _minute;
            }
        }
    }
}
