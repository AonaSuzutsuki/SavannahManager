using System;
using System.IO;
using System.Text.RegularExpressions;
using SvManagerLibrary.Telnet;

namespace SvManagerLibrary.Time
{
    public class Time
    {
        public TimeInfo TimeData { set; get; } = new TimeInfo();

        private static TimeInfo ConvertTime(string text)
        {
            TimeInfo timeInfo = new TimeInfo();

            const string expression = "^Day (?<day>.*?), (?<hour>.*?):(?<minute>.*?)$";
            var reg = new Regex(expression);
            var sr = new StringReader(text);
            while (sr.Peek() > -1)
            {
                var match = reg.Match(sr.ReadLine());
                if (match.Success == true)
                {
                    int.TryParse(match.Groups["day"].Value, out int day);
                    timeInfo.Day = day;

                    int.TryParse(match.Groups["hour"].Value, out int hour);
                    timeInfo.Hour = hour;

                    int.TryParse(match.Groups["minute"].Value, out int minute);
                    timeInfo.Minute = minute;
                }
            }

            return timeInfo;
        }
        public static TimeInfo GetTimeFromTelnet(TelnetClient telnet)
        {
            if (!telnet.Connected || telnet == null)
                throw new System.NullReferenceException();

            telnet.WriteLine("gt");
            System.Threading.Thread.Sleep(100);
            string log = telnet.Read().TrimEnd('\0');
            return ConvertTime(log);
        }
        public static void SendTime(TelnetClient telnet, TimeInfo timeInfo)
        {
            if (!telnet.Connected || telnet == null)
            {
                throw new System.NullReferenceException();
            }

            double Minute = Math.Ceiling((double)timeInfo.Minute * 16.666666666666666666666666666667);
            //16.66666666667 ≒ 50.0f ÷ 3.0f
            int stTime = (timeInfo.Day - 1) * 24000 + (timeInfo.Hour * 1000) + (int)Minute;
            //(Day - 1) * 24000 + (Hour * 1000) + (Minute * 16.666666666666666666666666666667)
            telnet.WriteLine("st " + stTime.ToString());
        }

        //public void ConvertTime(string text)
        //{
        //    string expression = "^Day (?<day>.*?), (?<hour>.*?):(?<minute>.*?)$";
        //    Regex reg = new Regex(expression);
        //    Match match;
        //    StringReader sr;

        //    sr = new StringReader(text);
        //    while (sr.Peek() > -1)
        //    {
        //        match = reg.Match(sr.ReadLine());
        //        if (match.Success == true)
        //        {
        //            int day = 1;
        //            int.TryParse(match.Groups["day"].Value, out day);
        //            TimeData.Day = day;

        //            int hour = 0;
        //            int.TryParse(match.Groups["hour"].Value, out hour);
        //            TimeData.Hour = hour;

        //            int minute = 0;
        //            int.TryParse(match.Groups["minute"].Value, out minute);
        //            TimeData.Minute = minute;
        //        }
        //    }
        //}
        public void SetTimeFromTelnet(TelnetClient telnet)
        {
            TimeInfo timeInfo = GetTimeFromTelnet(telnet);
            TimeData = timeInfo;
        }
        public void SendTime(TelnetClient telnet)
        {
            SendTime(telnet, TimeData);
        }
    }
}
