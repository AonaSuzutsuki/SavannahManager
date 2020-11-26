using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm.Views
{
    public static class PlaceHolderBehavior
    {
        public static readonly DependencyProperty PlaceHolderProperty = DependencyProperty.RegisterAttached(
            "PlaceHolder",
            typeof(string),
            typeof(PlaceHolderBehavior),
            new PropertyMetadata(null, OnPlaceHolderChanged));

        private static void OnPlaceHolderChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;

            var placeHolder = e.NewValue as string;
            var handler = CreateEventHandler(placeHolder);
            if (string.IsNullOrEmpty(placeHolder))
            {
                textBox.TextChanged -= handler;
            }
            else
            {
                textBox.TextChanged += handler;
                if (string.IsNullOrEmpty(textBox.Text))
                    textBox.Background = CreateVisualBrush(placeHolder);
            }
        }

        private static TextChangedEventHandler CreateEventHandler(string placeHolder)
        {
            return (sender, e) =>
            {
                var textBox = (TextBox)sender;
                if (string.IsNullOrEmpty(textBox.Text))
                    textBox.Background = CreateVisualBrush(placeHolder);
                else
                    textBox.Background = new SolidColorBrush(Colors.Transparent);
            };
        }

        private static VisualBrush CreateVisualBrush(string placeHolder)
        {
            var visual = new TextBlock
            {
                Text = placeHolder,
                Padding = new Thickness(5, 1, 1, 1),
                Foreground = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            return new VisualBrush(visual)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Center,
            };
        }

        public static void SetPlaceHolder(TextBox textBox, string placeHolder)
        {
            textBox.SetValue(PlaceHolderProperty, placeHolder);
        }

        public static string GetPlaceHolder(TextBox textBox)
        {
            return textBox.GetValue(PlaceHolderProperty) as string;
        }
    }
}
