using System;
using System.IO;
using System.Text.RegularExpressions;
using SvManagerLibrary.Telnet;

namespace SvManagerLibrary.Time
{
    public class Time
    {
        public static TimeInfo ConvertTime(string text)
        {
            var timeInfo = new TimeInfo();

            const string expression = "^Day (?<day>.*?), (?<hour>.*?):(?<minute>.*?)$";
            var reg = new Regex(expression);
            var sr = new StringReader(text);
            while (sr.Peek() > -1)
            {
                var match = reg.Match(sr.ReadLine());
                if (match.Success)
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
        public static TimeInfo GetTimeFromTelnet(ITelnetClient telnet)
        {
            TelnetException.CheckTelnetClient(telnet);

            telnet.DestructionEvent = true;
            telnet.WriteLine("gt");
            System.Threading.Thread.Sleep(200);
            var log = telnet.Read().TrimEnd('\0');
            telnet.DestructionEvent = false;
            return ConvertTime(log);
        }
        public static void SendTime(ITelnetClient telnet, TimeInfo timeInfo)
        {
            TelnetException.CheckTelnetClient(telnet);

            var minute = Math.Ceiling(timeInfo.Minute * 16.666666666666666666666666666667);
            //16.66666666667 ≒ 50.0f ÷ 3.0f
            var stTime = (timeInfo.Day - 1) * 24000 + (timeInfo.Hour * 1000) + (int)minute;
            //(Day - 1) * 24000 + (Hour * 1000) + (Minute * 16.666666666666666666666666666667)
            telnet.WriteLine("st " + stTime.ToString());
        }
    }
}
