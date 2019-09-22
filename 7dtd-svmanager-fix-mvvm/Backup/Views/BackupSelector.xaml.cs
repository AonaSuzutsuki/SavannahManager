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
using System.Windows.Shapes;
using _7dtd_svmanager_fix_mvvm.Backup.Models;
using _7dtd_svmanager_fix_mvvm.Backup.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.Backup.Views
{
    /// <summary>
    /// BackupSelector.xaml の相互作用ロジック
    /// </summary>
    public partial class BackupSelector : Window
    {
        public BackupSelector()
        {
            InitializeComponent();

            var model = new BackupSelectorModel();
            var vm = new BackupSelectorViewModel(this, model);
            this.DataContext = vm;
        }
    }
}
