using System.Windows;
using System.Windows.Interop;
using System;
using static CommonStyleLib.ExMessageBox.ExMessageBoxBase;

namespace CommonStyleLib.ExMessageBox.Views
{
    /// <summary>
    /// ExMassageBox.xaml の相互作用ロジック
    /// </summary>
    public partial class ExOKMassageBox : Window
    {
        private Models.ExMassageBoxModel model;
        private ViewModels.ExOKMassageBoxViewModel vm;
        public ExOKMassageBox(string text, string title, MessageType mType)
        {
            InitializeComponent();

            model = new Models.ExMassageBoxModel(text, title, mType);
            vm = new ViewModels.ExOKMassageBoxViewModel(this, model);
            DataContext = vm;

            this.Loaded += ExOKMassageBox_Loaded;
        }

        private void ExOKMassageBox_Loaded(object sender, RoutedEventArgs e)
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
