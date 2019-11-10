using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using _7dtd_XmlEditor.Models;
using _7dtd_XmlEditor.Models.TreeView;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(Window view, MainWindowModel model) : base(view, model)
        {
            //TreeViewItems = new List<TreeViewItemModel>
            //{
            //    new TreeViewItemModel(view)
            //};

            var reader = new CommonXmlReader("vehicles.xml");
            TreeViewItems = new List<TreeViewItemInfo>
            {
                new TreeViewItemInfo(reader.GetAllNodes())
            };

            TreeViewMouseRightButtonDown = new DelegateCommand(TreeView_MouseRightButtonDown);
        }

        public List<TreeViewItemInfo> TreeViewItems { get; set; }
        public TreeViewItemInfo TreeViewSelectedItem { get; set; }

        public ICommand TreeViewMouseRightButtonDown { get; set; }

        public void TreeView_MouseRightButtonDown()
        {
            Console.WriteLine(TreeViewSelectedItem.Path);
        }
    }
}
