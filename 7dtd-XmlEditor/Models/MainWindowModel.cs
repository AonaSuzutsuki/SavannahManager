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
using CommonCoreLib;
using CommonExtensionLib.Extensions;
using CommonStyleLib.File;
using CommonStyleLib.Models;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.Models
{
    public class MainWindowModel : ModelBase
    {
        #region Properties
        public ObservableCollection<string> EditModeComboItems
        {
            get => editModeComboItems;
            set => SetProperty(ref editModeComboItems, value);
        }

        public string EditModeSelectedItem
        {
            get => editModeSelectedItem;
            set => SetProperty(ref editModeSelectedItem, value);
        }
        #endregion

        #region Fields
        private string declaration;
        private TreeViewItemInfo root;
        private ObservableCollection<string> editModeComboItems;
        private string editModeSelectedItem;

        private NavigationService navigation;
        private INodeView commonPage;
        #endregion

        public MainWindowModel(NavigationService navigation)
        {
            this.navigation = navigation;
            EditModeComboItems = new ObservableCollection<string>();

            OpenFile("vehicles.xml");
        }

        public void OpenFile()
        {
            var filePath = FileSelector.GetFilePath(AppInfo.GetAppPath(), "XML Files(*.xml)|*.xml|All Files(*.*)|*.*"
                , "", FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(filePath))
            {
                OpenFile(filePath);
            }
        }

        public void NodeViewModeChange(string mode)
        {
            if (mode == "Common")
            {
                var selected = commonPage?.Model.SelectedItem;
                var model = new CommonModel(root)
                {
                    Declaration = declaration
                };
                model.ItemApplied += (sender, args) => root = args.ItemInfo;
                commonPage = new CommonView(model);
                if (selected != null)
                    model.ChangeItem(selected);
                navigation.Navigate(commonPage);
            }
        }

        private void OpenFile(string filePath)
        {
            //filePath = "vehicles.xml";
            var reader = new CommonXmlReader(filePath);
            declaration = reader.Declaration;
            root = new TreeViewItemInfo(reader.GetAllNodes());

            //var model = new CommonModel(root)
            //{
            //    Declaration = declaration
            //};
            //model.ItemApplied += (sender, args) => root = args.ItemInfo;
            //commonPage = new CommonView(model);
            //this.navigation.Navigate(commonPage);

            EditModeComboItems.Clear();
            EditModeComboItems.AddAll(new[]
            {
                "Common",
                "Vehicle"
            });
            EditModeSelectedItem = "Common";
            NodeViewModeChange(EditModeSelectedItem);

        }
    }
}
