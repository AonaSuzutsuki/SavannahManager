﻿using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace MyTextBox
{
    public class NumTextBehavior : Behavior<TextBox>
    {
        #region 最大値プロパティ
        public int Max
        {
            get { return (int)GetValue(MaxIntProperty); }
            set { SetValue(MaxIntProperty, value); }
        }

        public static readonly DependencyProperty MaxIntProperty =
            DependencyProperty.Register("Max", typeof(int), typeof(NumTextBehavior), new UIPropertyMetadata(null));
        #endregion

        public NumTextBehavior()
        {
            Max = int.MaxValue;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.KeyDown += OnKeyDown;
            this.AssociatedObject.TextChanged += OnTextChanged;
            DataObject.AddPastingHandler(this.AssociatedObject, TextBoxPastingEventHandler);
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.KeyDown -= OnKeyDown;
            this.AssociatedObject.TextChanged -= OnTextChanged;
            DataObject.RemovePastingHandler(this.AssociatedObject, TextBoxPastingEventHandler);
        }

        private void TextBoxPastingEventHandler(object sender, DataObjectPastingEventArgs e)
        {
            var textBox = (sender as TextBox);
            var clipboard = e.DataObject.GetData(typeof(string)) as string;
            Regex r = new Regex("^[0-9]+$");
            Match match = r.Match(clipboard);
            if (match.Success)
            {
                if (textBox != null && !string.IsNullOrEmpty(clipboard))
                {
                    textBox.Text = clipboard;
                }
            }
            e.CancelCommand();
            e.Handled = true;
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if ((Key.D0 <= e.Key && e.Key <= Key.D9) ||
                (Key.NumPad0 <= e.Key && e.Key <= Key.NumPad9) ||
                (Key.Delete == e.Key) || (Key.Back == e.Key) || (Key.Tab == e.Key))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
                return;
            }

            if (int.TryParse(textBox.Text, out int textBoxNum))
            {
                if (textBoxNum > Max)
                {
                    textBox.Text = Max.ToString();
                }
            }
            else
            {
                textBox.Text = Max.ToString();
            }
        }
    }
}