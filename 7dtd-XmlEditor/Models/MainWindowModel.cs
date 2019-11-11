using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using _7dtd_XmlEditor.Models.NodeView;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.Views.NodeView;
using CommonExtensionLib.Extensions;
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

        private string declaration;
        private TreeViewItemInfo root;
        private ObservableCollection<string> editModeComboItems;

        private NavigationService navigation;
        private INodeView commonPage;

        public MainWindowModel(NavigationService navigation)
        {
            this.navigation = navigation;

            var reader = new CommonXmlReader("vehicles.xml");
            declaration = reader.Declaration;
            root = new TreeViewItemInfo(reader.GetAllNodes());

            var model = new CommonModel(root)
            {
                Declaration = declaration
            };
            model.ItemApplied += (sender, args) => root = args.ItemInfo;
            commonPage = new CommonView(model);
            this.navigation.Navigate(commonPage);

            EditModeComboItems = new ObservableCollection<string>
            {
                "Common",
                "Vehicle"
            };
        }

        public void NodeViewModeChange(string mode)
        {
            if (mode == "Common")
            {
                var selected = commonPage.Model.SelectedItem;
                var model = new CommonModel(root)
                {
                    Declaration = declaration
                };
                model.ItemApplied += (sender, args) => root = args.ItemInfo;
                commonPage = new CommonView(model);
                model.ChangeItem(selected);
                navigation.Navigate(commonPage);
            }
        }
    }
}
