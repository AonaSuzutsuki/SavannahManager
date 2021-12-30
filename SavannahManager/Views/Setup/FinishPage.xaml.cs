using System.Windows.Controls;
using _7dtd_svmanager_fix_mvvm.Models.Setup;
using _7dtd_svmanager_fix_mvvm.ViewModels.Setup;

namespace _7dtd_svmanager_fix_mvvm.Views.Setup
{
    /// <summary>
    /// FinishPage.xaml の相互作用ロジック
    /// </summary>
    public partial class FinishPage : UserControl
    {
        public FinishPage(NavigationWindowService<InitializeData> service)
        {
            InitializeComponent();

            DataContext = new FinishPageViewModel(service, new FinishPageModel(service.Share));
        }
    }
}
