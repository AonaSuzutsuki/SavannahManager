using System.Windows.Controls;
using _7dtd_svmanager_fix_mvvm.Models.Setup;
using _7dtd_svmanager_fix_mvvm.ViewModels.Setup;

namespace _7dtd_svmanager_fix_mvvm.Views.Setup
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
            DataContext = new ExecutablePageViewModel(service, model);
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
