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
using _7dtd_XmlEditor.Models.NodeView;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.ViewModels.NodeView;

namespace _7dtd_XmlEditor.Views.NodeView
{
    /// <summary>
    /// CommonView.xaml の相互作用ロジック
    /// </summary>
    public partial class CommonView : Page, INodeView
    {
        private CommonModel model;

        public CommonView()
        {
            InitializeComponent();

            model = new CommonModel();
            var vm = new CommonViewModel(model);
            DataContext = vm;
        }

        public void ChangeItem(TreeViewItemInfo info)
        {
            model.ChangeItem(info);
        }
    }
}
