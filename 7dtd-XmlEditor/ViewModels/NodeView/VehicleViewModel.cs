using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_XmlEditor.Models.NodeView;
using _7dtd_XmlEditor.Models.TreeView;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_XmlEditor.ViewModels.NodeView
{
    public class VehicleViewModel
    {
        public VehicleViewModel(VehicleModel model)
        {
            this.model = model;

            VehicleItems = model.VehicleItems.ToReadOnlyReactiveCollection(m => m);
            VehicleSelectedItem = model.ToReactivePropertyAsSynchronized(m => m.VehicleSelectedItem);
            VehicleName = model.ObserveProperty(m => m.VehicleName).ToReactiveProperty();
            PageBackBtIsEnabled = model.ObserveProperty(m => m.VehicleSelected).ToReactiveProperty();
            PropertyNameText = model.ObserveProperty(m => m.PropertyNameText).ToReactiveProperty();
            DescriptionText = model.ObserveProperty(m => m.DescriptionText).ToReactiveProperty();
            VehicleAttributeText = model.ToReactivePropertyAsSynchronized(m => m.VehicleAttributeText);

            BackBtClicked = new DelegateCommand(BackBt_Clicked);
            VehicleListMouseDoubleClicked = new DelegateCommand(VehicleList_MouseDoubleClicked);
            VehicleListSelectionChanged = new DelegateCommand(VehicleList_SelectionChanged);
        }

        #region Fields
        private VehicleModel model;
        #endregion

        #region Properties
        public ReadOnlyReactiveCollection<VehicleInfo> VehicleItems { get; set; }
        public ReactiveProperty<VehicleInfo> VehicleSelectedItem { get; set; }

        public ReactiveProperty<string> VehicleName { get; set; }
        public ReactiveProperty<bool> PageBackBtIsEnabled { get; set; }

        public ReactiveProperty<string> PropertyNameText { get; set; }
        public ReactiveProperty<string> DescriptionText { get; set; }
        public ReactiveProperty<string> VehicleAttributeText { get; set; }
        #endregion

        #region Event Properties
        public ICommand BackBtClicked { get; set; }
        public ICommand VehicleListMouseDoubleClicked { get; set; }
        public ICommand VehicleListSelectionChanged { get; set; }
        #endregion

        #region Event Methods

        public void BackBt_Clicked()
        {
            model.PageBack();
        }
        public void VehicleList_MouseDoubleClicked()
        {
            model.OpenVehicleItem();
        }

        public void VehicleList_SelectionChanged()
        {
            model.VehicleCustomChanged();
        }
        #endregion
    }
}
