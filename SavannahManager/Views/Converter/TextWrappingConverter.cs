using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace _7dtd_svmanager_fix_mvvm.Views.Converter
{
    public class TextWrappingConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool boolValue))
                return TextWrapping.NoWrap;

            return boolValue ? TextWrapping.WrapWithOverflow : TextWrapping.NoWrap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TextWrapping textWrapping))
                return false;

            return textWrapping != TextWrapping.NoWrap ? true : false;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
