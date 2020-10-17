using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Permissions.Models
{
    public class UnBanDateSettingModel : ModelBase
    {

        private int hourText;
        private int minuteText;
        private int secondText;

        public int HourText
        {
            get => hourText;
            set => SetProperty(ref hourText, value);
        }
        public int MinuteText
        {
            get => minuteText;
            set => SetProperty(ref minuteText, value);
        }
        public int SecondText
        {
            get => secondText;
            set => SetProperty(ref secondText, value);
        }

        public static DateTime? ConvertStringToDateTime(string text)
        {
            //2035-12-31 23:59:59
            var regex = new Regex("(?<year>[0-9]{4})-(?<month>[0-9]{2})-(?<day>[0-9]{2}) (?<hour>[0-9]{2}):(?<minute>[0-9]{2}):(?<second>[0-9]{2})");
            var match = regex.Match(text);
            if (match.Success)
            {
                var year = match.Groups["year"].Value.ToInt();
                var month = match.Groups["month"].Value.ToInt();
                var day = match.Groups["day"].Value.ToInt();
                var hour = match.Groups["hour"].Value.ToInt();
                var minute = match.Groups["minute"].Value.ToInt();
                var second = match.Groups["second"].Value.ToInt();

                var date = new DateTime(year, month, day, hour, minute, second);
                return date;
            }

            return null;
        }
    }
}
