using System.Windows.Controls;
using _7dtd_svmanager_fix_mvvm.Models.Setup;
using _7dtd_svmanager_fix_mvvm.ViewModels.Setup;

namespace _7dtd_svmanager_fix_mvvm.Views.Setup
{
    /// <summary>
    /// FirstPage.xaml の相互作用ロジック
    /// </summary>
    public partial class FirstPage : UserControl
    {
        public FirstPage(NavigationWindowService<InitializeData> service)
        {
            InitializeComponent();

            DataContext = new FirstPageViewModel(service, new FirstPageModel(service.Share));
        }
    }
}
