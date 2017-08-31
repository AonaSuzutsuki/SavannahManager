using _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages;
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

namespace _7dtd_svmanager_fix_mvvm.PlayerController.Views
{
    /// <summary>
    /// PlayerBase.xaml の相互作用ロジック
    /// </summary>
    public partial class PlayerBase : Window
    {
        public PlayerBase(string windowTitle, IPlayerPage page)
        {
            InitializeComponent();

            var model = new Models.PlayerBaseModel();
            var vm = new ViewModels.PlayerBaseViewModel(this, model)
            {
                WindowTitle = windowTitle
            };
            DataContext = vm;

            page.Ended += Page_Ended;

            NavigationService navi = MainFrame.NavigationService;
            navi.Navigate(page);
        }

        private void Page_Ended(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
