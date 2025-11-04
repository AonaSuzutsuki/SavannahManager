using System;
using System.IO;
using System.Text.RegularExpressions;
using SvManagerLibrary.AnalyzerPlan.Console;
using SvManagerLibrary.Telnet;

namespace SvManagerLibrary.Time
{
    /// <summary>
    /// Prorivdes some methods for time.
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// Convert a text of 7dtd telnet log to a TimeInfo object.
        /// </summary>
        /// <param name="text">7dtd telnet log.</param>
        /// <param name="analyzerPlan"></param>
        /// <returns>TimeInfo object.</returns>
        public static TimeInfo ConvertTime(string text, IConsoleAnalyzer analyzerPlan)
        {
            var timeInfo = new TimeInfo();

            var expression = analyzerPlan.GetTimeExpression();
            var reg = new Regex(expression);
            var sr = new StringReader(text);
            while (sr.Peek() > -1)
            {
                var match = reg.Match(sr.ReadLine() ?? string.Empty);
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

        /// <summary>
        /// Get time from TelnetClient.
        /// </summary>
        /// <param name="telnet">The TelnetClient.</param>
        /// <param name="analyzerPlan"></param>
        /// <returns>TimeInfo.</returns>
        public static TimeInfo GetTimeFromTelnet(ITelnetClient telnet, IConsoleAnalyzer analyzerPlan)
        {
            TelnetException.CheckTelnetClient(telnet);

            var log = telnet.DestructionEventRead("gt", analyzerPlan.GetTimeExpression());
            return ConvertTime(log, analyzerPlan);
        }

        /// <summary>
        /// Send time to destination using TelnetClient.
        /// </summary>
        /// <param name="telnet">The TelnetClient</param>
        /// <param name="timeInfo">The time to be sent.</param>
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
