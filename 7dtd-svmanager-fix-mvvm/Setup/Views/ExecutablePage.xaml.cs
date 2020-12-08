using _7dtd_svmanager_fix_mvvm.Setup.Models;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using _7dtd_svmanager_fix_mvvm.Setup.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.Setup.Views
{
    /// <summary>
    /// ExecutablePage.xaml の相互作用ロジック
    /// </summary>
    public partial class ExecutablePage : UserControl
    {
        public ExecutablePage(NavigationWindowService<InitializeData> service)
        {
            InitializeComponent();

            var model = new ExecutablePageModel(service.Share);
            model.CanChanged += Model_CanChanged;
            DataContext = new ViewModels.ExecutablePageViewModel(service, model);
        }

        private void Model_CanChanged(object sender, CanChangedEventArgs e)
        {
            OnCanChanged(this, e.CanChanged);
        }

        public event CanChangedEventHandler CanChanged;
        public void OnCanChanged(object sender, bool canChanged)
        {
            CanChanged?.Invoke(sender, new CanChangedEventArgs(canChanged));
        }
    }
}
