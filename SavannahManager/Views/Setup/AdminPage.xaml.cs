using System.Windows.Controls;
using _7dtd_svmanager_fix_mvvm.Models.Setup;
using _7dtd_svmanager_fix_mvvm.ViewModels.Setup;

namespace _7dtd_svmanager_fix_mvvm.Views.Setup
{
    /// <summary>
    /// ConfigPage.xaml の相互作用ロジック
    /// </summary>
    public partial class AdminPage : UserControl
    {
        public AdminPage(NavigationWindowService<InitializeData> service)
        {
            InitializeComponent();

            var model = new AdminPageModel(service.Share);
            model.CanChanged += ModelCanChanged;
            DataContext = new AdminPageViewModel(service, model);
        }

        private void ModelCanChanged(object sender, CanChangedEventArgs e)
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
