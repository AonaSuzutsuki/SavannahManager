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

            var model = new Models.UpdFormModel();
            var vm = new ViewModels.UpdFormViewModel(this, model);
            DataContext = vm;
        }
    }
}
