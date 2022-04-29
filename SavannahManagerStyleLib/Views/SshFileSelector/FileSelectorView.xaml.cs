using System.Windows;

namespace SavannahManagerStyleLib.Views.SshFileSelector
{
    /// <summary>
    /// FileSelectorView.xaml の相互作用ロジック
    /// </summary>
    public partial class FileSelectorView : Window
    {
        public FileSelectorView()
        {
            InitializeComponent();
        }

        public void PathTextBoxScrollToEnd()
        {
            PathTextBox.CaretIndex = PathTextBox.Text.Length;
            var rect = PathTextBox.GetRectFromCharacterIndex(PathTextBox.CaretIndex);
            PathTextBox.ScrollToHorizontalOffset(rect.Right);
        }
    }
}
