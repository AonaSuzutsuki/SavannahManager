using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using _7dtd_XmlEditor.Models.NodeView;
using _7dtd_XmlEditor.Models.TreeView;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SvManagerLibrary.XmlWrapper;

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
            InnerXmlTextChanged = new DelegateCommand(InnerXml_TextChanged);
        }

        private CommonModel model;

        public ReadOnlyReactiveCollection<TreeViewItemInfo> TreeViewItems { get; set; }
        public ReactiveProperty<TreeViewItemInfo> TreeViewSelectedItem { get; set; }

        public ReactiveProperty<string> FullPath { get; set; }
        public ReadOnlyReactiveCollection<AttributeInfo> Attributes { get; set; }
        public ReactiveProperty<string> InnerXml { get; set; }


        public ICommand ApplyBtClicked { get; set; }
        public ICommand TreeViewSelectedItemChanged { get; set; }
        public ICommand TreeViewMouseRightButtonDown { get; set; }

        public ICommand InnerXmlTextChanged { get; set; }


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
        public void InnerXml_TextChanged()
        {
            model.ChangeInnerXml();
        }
    }
}
