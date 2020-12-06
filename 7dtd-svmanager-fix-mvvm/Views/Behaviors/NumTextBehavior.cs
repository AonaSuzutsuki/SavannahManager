using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace _7dtd_svmanager_fix_mvvm.Views.Behaviors
{
    public class NumTextBehavior : Behavior<TextBox>
    {
        #region 最大値プロパティ
        public int Max
        {
            get => (int)GetValue(MaxIntProperty);
            set => SetValue(MaxIntProperty, value);
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
            AssociatedObject.KeyDown += OnKeyDown;
            AssociatedObject.TextChanged += OnTextChanged;
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
            var textBox = sender as TextBox;
            var clipboard = e.DataObject.GetData(typeof(string)) as string;
            if (clipboard == null)
                return;

            var r = new Regex("^[0-9]+$");
            var match = r.Match(clipboard);
            if (match.Success)
                if (textBox != null)
                    textBox.Text = clipboard;
            e.CancelCommand();
            e.Handled = true;
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox))
                return;

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
            if (!(sender is TextBox textBox))
                return;

            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0";
                return;
            }

            if (int.TryParse(textBox.Text, out var textBoxNum))
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
