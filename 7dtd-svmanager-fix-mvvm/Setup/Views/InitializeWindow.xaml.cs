using _7dtd_svmanager_fix_mvvm.Models;
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

namespace _7dtd_svmanager_fix_mvvm.Setup.Views
{
    /// <summary>
    /// InitializeWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InitializeWindow : Window
    {
        InitializeWindowModel model;
        public InitializeWindow(SettingLoader settingLoader)
        {
            InitializeComponent();

            model = new InitializeWindowModel(settingLoader, MainFrame.NavigationService);
            var vm = new ViewModels.InitializeWindowViewModel(this, model);
            DataContext = vm;
        }

        public InitializeData GetInitializeData()
        {
            return model.SharedInitializeData;
        }
    }
}
