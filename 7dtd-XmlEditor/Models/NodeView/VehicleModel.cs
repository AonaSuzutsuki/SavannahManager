using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using _7dtd_XmlEditor.Models.TreeView;
using CommonCoreLib.CommonLinq;
using SavannahXmlLib.XmlWrapper;
using CommonExtensionLib.Extensions;
using Prism.Mvvm;

namespace _7dtd_XmlEditor.Models.NodeView
{
    public class VehicleInfo
    {
        public string Name { get; set; }
        public TreeViewItemInfo TreeViewItem { get; set; }
    }

    public class VehicleModel : BindableBase, ICommonModel
    {
        #region Properties

        public ObservableCollection<VehicleInfo> VehicleItems
        {
            get => vehicleItems;
            set => SetProperty(ref vehicleItems, value);
        }

        #endregion
        public event CommonModel.ItemAppliedEventHandler ItemApplied;

        public VehicleInfo VehicleSelectedItem
        {
            get => vehicleSelectedItem;
            set
            {
                SetProperty(ref vehicleSelectedItem, value);
                SelectedItem = value?.TreeViewItem;
            }
        }
        public TreeViewItemInfo SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public string VehicleName
        {
            get => vehicleName;
            set => SetProperty(ref vehicleName, value);
        }
        public bool VehicleSelected
        {
            get => vehicleSelected;
            set => SetProperty(ref vehicleSelected, value);
        }

        public string PropertyNameText
        {
            get => propertyNameText;
            set => SetProperty(ref propertyNameText, value);
        }
        public string DescriptionText
        {
            get => descriptionText;
            set => SetProperty(ref descriptionText, value);
        }
        public string VehicleAttributeText
        {
            get => vehicleAttributeText;
            set => SetProperty(ref vehicleAttributeText, value);
        }

        public string Declaration { get; set; }

        #region Fields
        private CommonXmlReader templaReader;

        private TreeViewItemInfo root;
        private ObservableCollection<VehicleInfo> vehicleItems = new ObservableCollection<VehicleInfo>();
        private VehicleInfo vehicleSelectedItem;
        private TreeViewItemInfo selectedItem;

        private string vehicleName;
        private bool vehicleSelected;

        private string propertyNameText;
        private string descriptionText;
        private string vehicleAttributeText;
        #endregion

        public VehicleModel(TreeViewItemInfo info)
        {
            using var fs = new FileStream("XmlTemplates/a18/Vehicles/vehicles.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            templaReader = new CommonXmlReader(fs);

            root = info;
            DrawMainPage(info);
        }

        public void DrawMainPage(TreeViewItemInfo info)
        {
            var items = from child in info.Children
                let attr = child.Node.GetAttribute("name")
                where !string.IsNullOrEmpty(attr.Name)
                select new VehicleInfo { Name = attr.Value, TreeViewItem = child };
            var convertedItems = ConvertVehicleName(templaReader, items);
            VehicleItems.AddAll(convertedItems);
        }

        public void OpenVehicleItem()
        {
            if (VehicleSelectedItem == null)
                return;

            VehicleItems.Clear();
            var items = from child in VehicleSelectedItem.TreeViewItem.Children
                        let attr = child.Node.GetAttribute("name")
                        where !string.IsNullOrEmpty(attr.Name)
                        select new VehicleInfo { Name = attr.Value, TreeViewItem = child };
            var convertedItem = ConvertVehicleName(templaReader, items);
            VehicleItems.AddAll(convertedItem);

            var attrInfo = VehicleSelectedItem.TreeViewItem.Node.GetAttribute("name");
            VehicleName = attrInfo.Value;
            VehicleSelected = true;
        }

        private static IEnumerable<VehicleInfo> ConvertVehicleName(CommonXmlReader templateReader, IEnumerable<VehicleInfo> vehicleInfos)
        {
            var nodes = templateReader.GetNodes("/vehicles/names/name").ToDictionary((node) => node.GetAttribute("name").Value, 
                        (node) => node.GetAttribute("value").Value);

            var items = from vehicleInfo in vehicleInfos
                        let name = vehicleInfo.Name
                        let convertedName = Conditions.IfElse(nodes.ContainsKey(name), () => nodes[name], () => name)
                        select new VehicleInfo { Name = convertedName, TreeViewItem = vehicleInfo.TreeViewItem };

            return items;
        }

        public void PageBack()
        {
            VehicleItems.Clear();
            DrawMainPage(root);

            VehicleName = string.Empty;
            VehicleSelected = false;
        }

        public void VehicleCustomChanged()
        {
            var info = VehicleSelectedItem?.TreeViewItem;
            if (!VehicleSelected || info == null)
                return;

            var nameAttribute = info.Node.GetAttribute("name");
            var valueAttribute = info.Node.GetAttribute("value");

            var templateNode = templaReader.GetNode($"/vehicles/vehicle[@name='common']/property[@name='{nameAttribute.Value}']");

            if (templateNode != null)
            {
                PropertyNameText = VehicleSelectedItem.Name;
                DescriptionText = templateNode.InnerText;
            }

            if (!string.IsNullOrEmpty(valueAttribute.Name))
                VehicleAttributeText = valueAttribute.Value;
        }

        public void ChangeItem(TreeViewItemInfo info)
        {
        }

        public void NewFile()
        {

        }
    }
}
