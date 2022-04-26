using System;
using System.Text.RegularExpressions;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;

namespace _7dtd_svmanager_fix_mvvm.Models.Permissions
{
    public class UnBanDateSettingModel : ModelBase
    {

        private DateTime? _selectedDate;
        private int _hourText;
        private int _minuteText;
        private int _secondText;

        public int HourText
        {
            get => _hourText;
            set => SetProperty(ref _hourText, value);
        }
        public int MinuteText
        {
            get => _minuteText;
            set => SetProperty(ref _minuteText, value);
        }
        public int SecondText
        {
            get => _secondText;
            set => SetProperty(ref _secondText, value);
        }

        public DateTime? SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        public string ConvertToString()
        {
            //2035-12-31 23:59:59
            return $"{SelectedDate?.ToString("yyyy-MM-dd")} {HourText:D2}:{MinuteText:D2}:{SecondText:D2}";
        }

        public static DateTime? ConvertStringToDateTime(string text)
        {
            if (text == null)
                return DateTime.Now;

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
