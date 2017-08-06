using System;
using System.Windows;
using System.Windows.Interop;
using static CommonStyleLib.ExMessageBox.ExMessageBoxBase;

namespace CommonStyleLib.ExMessageBox.Views
{
    /// <summary>
    /// ExYesNoMessageBox.xaml の相互作用ロジック
    /// </summary>
    public partial class ExYesNoMessageBox : Window
    {
        private Models.ExMassageBoxModel model;
        private ViewModels.ExYesNoMessageBoxViewModel vm;
        public ExYesNoMessageBox(string text, string title, MessageType mType)
        {
            InitializeComponent();

            model = new Models.ExMassageBoxModel(text, title, mType);
            vm = new ViewModels.ExYesNoMessageBoxViewModel(this, model);
            DataContext = vm;

            this.Loaded += ExYesNoMessageBox_Loaded;
        }

        private void ExYesNoMessageBox_Loaded(object sender, RoutedEventArgs e)
        {
            vm.Loaded();
        }

        public IntPtr GetHandel()
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(this);
            return source.Handle;
        }

        public DialogResult GetDialogResult()
        {
            return model.Result;
        }
    }
}
