using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using CommonExtensionLib.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Views.Converter
{
    public class SettingWaitModeVisibilityConverter : MarkupExtension, IValueConverter
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var comboBoxItem = value as ComboBoxItem;
            if (comboBoxItem == null || parameter == null)
                return Visibility.Collapsed;

            var text = comboBoxItem.Content.ToString();
            var type = parameter.ToString().ToInt();

            switch (type)
            {
                case 0 when text == "Cool Time":
                case 1 when text == "Wait Process":
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
