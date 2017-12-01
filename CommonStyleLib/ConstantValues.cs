using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CommonLib
{
    public static class ConstantValues
    {
        private static Color GetColor(string hexCode)
        {
            return (Color)ColorConverter.ConvertFromString(hexCode);
        }

        public static SolidColorBrush ActivatedBorderColor { get; } = new SolidColorBrush(GetColor("#4090ff"));
        public static SolidColorBrush ActivatedBorderColor2 { get; } = new SolidColorBrush(GetColor("#ff6b00"));
        public static SolidColorBrush DeactivatedBorderColor { get; } = new SolidColorBrush(GetColor("#4090ff66"));

    }
}
