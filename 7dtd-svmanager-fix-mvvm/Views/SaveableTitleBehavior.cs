using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace _7dtd_svmanager_fix_mvvm.Views
{
    public static class SaveableTitleBehavior
    {
        public static readonly DependencyProperty CanSaveProperty = DependencyProperty.RegisterAttached(
            "CanSave",
            typeof(bool),
            typeof(SaveableTitleBehavior),
            new PropertyMetadata(false, OnCanSaveChanged));

        public static void SetCanSave(DependencyObject label, bool placeHolder)
        {
            label.SetValue(CanSaveProperty, placeHolder);
        }

        public static bool GetCanSave(DependencyObject label)
        {
            return (bool)label.GetValue(CanSaveProperty);
        }

        private static void OnCanSaveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBlock textBlock))
                return;

            var canSave = (bool)e.NewValue;
            textBlock.Text = canSave ? $"{textBlock.Text} *" : textBlock.Text.Substring(0, textBlock.Text.Length - 2);
        }
    }
}
