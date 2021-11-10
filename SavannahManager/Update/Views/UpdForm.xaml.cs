using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _7dtd_svmanager_fix_mvvm.Update.ViewModels;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.ExMessageBox.Views;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.Update.Views
{
    /// <summary>
    /// UpdForm.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdForm : Window
    {
        public UpdForm()
        {
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as UpdFormViewModel;
            var hyperLink = sender as Hyperlink;
            if (vm == null || hyperLink == null)
                return;

            var url = hyperLink.NavigateUri.ToString();

            vm.OpenLinkCommand.Execute(url);
        }
    }
}
