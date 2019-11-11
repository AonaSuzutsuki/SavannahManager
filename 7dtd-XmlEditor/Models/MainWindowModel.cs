using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.Views.NodeView;
using CommonStyleLib.Models;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.Models
{
    public class MainWindowModel : ModelBase
    {

        public ObservableCollection<string> EditModeComboItems
        {
            get => editModeComboItems;
            set => SetProperty(ref editModeComboItems, value);
        }

        public ObservableCollection<TreeViewItemInfo> TreeViewItems
        {
            get => treeViewItems;
            set => SetProperty(ref treeViewItems, value);
        }

        private ObservableCollection<string> editModeComboItems;
        private ObservableCollection<TreeViewItemInfo> treeViewItems;

        private NavigationService navigation;
        private INodeView commonPage;

        public MainWindowModel(NavigationService navigation)
        {
            this.navigation = navigation;

            commonPage = new CommonView();
            this.navigation.Navigate(commonPage);

            EditModeComboItems = new ObservableCollection<string>
            {
                "Common",
                "Vehicle"
            };

            var reader = new CommonXmlReader("vehicles.xml");
            TreeViewItems = new ObservableCollection<TreeViewItemInfo>
            {
                new TreeViewItemInfo(reader.GetAllNodes())
            };
        }

        public void NodeViewModeChange(string mode, TreeViewItemInfo info)
        {
            if (mode == "Common")
            {
                commonPage = new CommonView();
                commonPage.ChangeItem(info);
                navigation.Navigate(commonPage);
            }
        }
        public void SelectionChange(TreeViewItemInfo info)
        {
            commonPage.ChangeItem(info);
        }
    }
}
