using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm.Styles
{
    [ValueConversion(typeof(DateTime), typeof(Brush))]
    public class DateTimeToDayOfWeekBrushConverter : IValueConverter
    {
        public Brush SundayBrush { set; get; } = Brushes.Red;
        public Brush SaturdayBrush { set; get; } = Brushes.Blue;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime dateTime))
                return DependencyProperty.UnsetValue;

            var dayOfWeek = dateTime.DayOfWeek;
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return SundayBrush;
                case DayOfWeek.Saturday:
                    return SaturdayBrush;
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
