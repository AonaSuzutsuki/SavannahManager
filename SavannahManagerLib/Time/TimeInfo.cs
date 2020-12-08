namespace SvManagerLibrary.Time
{
    public class TimeInfo
    {
        public int Day { set; get; } = 1;

        private int _hour;
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
            get => _hour;
        }
        private int _minute;
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
            get => _minute;
        }

        public override bool Equals(object obj)
        {
            return obj is TimeInfo info &&
                   Day == info.Day &&
                   _hour == info._hour &&
                   Hour == info.Hour &&
                   _minute == info._minute &&
                   Minute == info.Minute;
        }

        public override int GetHashCode()
        {
            var hashCode = 980657963;
            hashCode = hashCode * -1521134295 + Day.GetHashCode();
            hashCode = hashCode * -1521134295 + _hour.GetHashCode();
            hashCode = hashCode * -1521134295 + Hour.GetHashCode();
            hashCode = hashCode * -1521134295 + _minute.GetHashCode();
            hashCode = hashCode * -1521134295 + Minute.GetHashCode();
            return hashCode;
        }
    }
}
