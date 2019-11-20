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
using _7dtd_XmlEditor.ViewModels.NodeView;

namespace _7dtd_XmlEditor.Views.NodeView
{
    /// <summary>
    /// VehicleView.xaml の相互作用ロジック
    /// </summary>
    public partial class VehicleView : Page, INodeView
    {
        public VehicleView(VehicleModel model)
        {
            InitializeComponent();

            Model = model;
            var vm = new VehicleViewModel(model);
            DataContext = vm;
        }

        public ICommonModel Model { get; }
    }
}
