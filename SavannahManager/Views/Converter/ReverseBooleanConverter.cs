using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace _7dtd_svmanager_fix_mvvm.Views.Converter
{
    public class ReverseBooleanConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return false;

            return !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool boolValue)
                return false;

            return !boolValue;
        }
    }
}
