using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using _7dtd_XmlEditor.Models;
using _7dtd_XmlEditor.Models.NodeView;
using _7dtd_XmlEditor.Models.TreeView;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SavannahXmlLib.XmlWrapper;

namespace _7dtd_XmlEditor.ViewModels.NodeView
{
    public class CommonViewModel
    {
        public CommonViewModel(CommonModel model)
        {
            this.model = model;

            TreeViewItems = model.TreeViewItems.ToReadOnlyReactiveCollection(m => m);
            TreeViewSelectedItem = model.ToReactivePropertyAsSynchronized(m => m.SelectedItem);

            FullPath = model.ObserveProperty(m => m.FullPath).ToReactiveProperty();
            Attributes = model.Attributes.ToReadOnlyReactiveCollection(m => m);
            InnerXml = model.ToReactivePropertyAsSynchronized(m => m.InnerXml);


            ApplyBtClicked = new DelegateCommand(ApplyBt_Clicked);
            TreeViewSelectedItemChanged = new DelegateCommand(TreeView_SelectedItemChanged);
            TreeViewMouseRightButtonDown = new DelegateCommand(TreeView_MouseRightButtonDown);
            AddAttributeBtClicked = new DelegateCommand(AddAttributeBt_Clicked);
            RemoveAttributeBtClicked = new DelegateCommand(RemoveAttributeBt_Clicked);
            AttributesSelectionChanged = new DelegateCommand<ViewAttributeInfo>(Attributes_SelectionChanged);
            InnerXmlLostFocus = new DelegateCommand(InnerXml_LostFocus);
            InnerXmlTextChanged = new DelegateCommand(InnerXml_TextChanged);
        }

        #region Fields
        private CommonModel model;
        #endregion

        #region Properties
        public ReadOnlyReactiveCollection<TreeViewItemInfo> TreeViewItems { get; set; }
        public ReactiveProperty<TreeViewItemInfo> TreeViewSelectedItem { get; set; }

        public ReactiveProperty<string> FullPath { get; set; }
        public ReadOnlyReactiveCollection<ViewAttributeInfo> Attributes { get; set; }
        public ReactiveProperty<string> InnerXml { get; set; }
        #endregion


        #region Event Properties
        public ICommand ApplyBtClicked { get; set; }
        public ICommand TreeViewSelectedItemChanged { get; set; }
        public ICommand TreeViewMouseRightButtonDown { get; set; }

        public ICommand AddAttributeBtClicked { get; set; }
        public ICommand RemoveAttributeBtClicked { get; set; }

        public ICommand AttributesSelectionChanged { get; set; }

        public ICommand InnerXmlLostFocus { get; set; }
        public ICommand InnerXmlTextChanged { get; set; }
        #endregion

        #region Event Methods
        public void ApplyBt_Clicked()
        {
            model.Apply();
        }

        public void TreeView_SelectedItemChanged()
        {
            model.SelectionChange();
        }

        public void TreeView_MouseRightButtonDown()
        {
            Console.WriteLine(TreeViewSelectedItem.Value.Path);
        }

        public void AddAttributeBt_Clicked()
        {
            model.AddAttribute();
        }

        public void RemoveAttributeBt_Clicked()
        {
            model.RemoveAttribute();
        }

        public void Attributes_SelectionChanged(ViewAttributeInfo info)
        {
            if (info != null)
                model.AttributesSelectedItem = info;
        }

        public void InnerXml_LostFocus()
        {
            model.ApplyInnerXml();
        }

        public void InnerXml_TextChanged()
        {
            if (model.SelectedItem != null)
                if (model.SelectedItem.Node.InnerXml != model.InnerXml)
                    model.SelectedItem.IsEdited = true;
        }
        #endregion
    }
}
