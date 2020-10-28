using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_XmlEditor.Models;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.Views;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SavannahXmlLib.XmlWrapper;

namespace _7dtd_XmlEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IWindowService windowService, MainWindowModel model) : base(windowService, model)
        {
            this.model = model;

            IsEditedTitle = model.ObserveProperty(m => m.IsEditedTitle).ToReactiveProperty();
            TreeViewItems = model.TreeViewItems.ToReadOnlyReactiveCollection(m => m);
            TreeViewSelectedItem = model.ToReactivePropertyAsSynchronized(m => m.SelectedItem);

            FullPath = model.ObserveProperty(m => m.FullPath).ToReactiveProperty();
            IsAttributesEnabled = model.ObserveProperty(m => m.IsAttributesEnabled).ToReactiveProperty();
            Attributes = model.Attributes.ToReadOnlyReactiveCollection(m => m);
            InnerXml = model.ToReactivePropertyAsSynchronized(m => m.InnerXml);
            ContextMenuEnabled = model.ObserveProperty(m => m.ContextMenuEnabled).ToReactiveProperty();
            AddElementEnabled = model.ObserveProperty(m => m.AddElementEnabled).ToReactiveProperty();

            FileNewBtClick = new DelegateCommand(FileNewBt_Click);
            FileOpenBtClick = new DelegateCommand(FileOpenBt_Click);
            FileSaveBtClick = new DelegateCommand(FileSaveBt_Click);
            FileSaveAsBtClick = new DelegateCommand(FileSaveAsBt_Click);

            DropCommand = new DelegateCommand<DropArguments>(TreeViewDrop);
            TreeViewSelectedItemChangedCommand = new DelegateCommand(TreeViewSelectedItemChanged);
            TreeViewMouseRightButtonDown = new DelegateCommand(TreeView_MouseRightButtonDown);
            AddAttributeBtClicked = new DelegateCommand(AddAttributeBt_Clicked);
            RemoveAttributeBtClicked = new DelegateCommand(RemoveAttributeBt_Clicked);
            AttributesSelectionChanged = new DelegateCommand<ViewAttributeInfo>(Attributes_SelectionChanged);
            InnerXmlLostFocus = new DelegateCommand(InnerXml_LostFocus);
            InnerXmlTextChanged = new DelegateCommand(InnerXml_TextChanged);
            ChangeTagNameCommand = new DelegateCommand(ChangeTagName);
            AddChildElementCommand = new DelegateCommand(AddChildElement);
            RemoveElementCommand = new DelegateCommand(RemoveElement);
        }

        #region Fields

        private readonly MainWindowModel model;

        #endregion

        #region Properties
        public ReactiveProperty<string> IsEditedTitle { get; set; }

        public ReadOnlyReactiveCollection<TreeViewItemInfo> TreeViewItems { get; set; }
        public ReactiveProperty<TreeViewItemInfo> TreeViewSelectedItem { get; set; }

        public ReactiveProperty<string> FullPath { get; set; }
        public ReactiveProperty<bool> IsAttributesEnabled { get; set; }
        public ReadOnlyReactiveCollection<ViewAttributeInfo> Attributes { get; set; }
        public ReactiveProperty<string> InnerXml { get; set; }

        public ReactiveProperty<bool> ContextMenuEnabled { get; set; }
        public ReactiveProperty<bool> AddElementEnabled { get; set; }
        #endregion

        #region Event Properties
        public ICommand FileNewBtClick { get; set; }
        public ICommand FileOpenBtClick { get; set; }
        public ICommand FileSaveBtClick { get; set; }
        public ICommand FileSaveAsBtClick { get; set; }


        public ICommand DropCommand { get; set; }
        public ICommand TreeViewSelectedItemChangedCommand { get; set; }
        public ICommand TreeViewMouseRightButtonDown { get; set; }

        public ICommand AddAttributeBtClicked { get; set; }
        public ICommand RemoveAttributeBtClicked { get; set; }

        public ICommand AttributesSelectionChanged { get; set; }

        public ICommand InnerXmlLostFocus { get; set; }
        public ICommand InnerXmlTextChanged { get; set; }


        public ICommand ChangeTagNameCommand { get; set; }
        public ICommand AddChildElementCommand { get; set; }
        public ICommand RemoveElementCommand { get; set; }
        #endregion

        #region Event Methods
        public void FileNewBt_Click()
        {
            model.NewFile();
        }
        public void FileOpenBt_Click()
        {
            model.OpenFile();
        }

        public void FileSaveBt_Click()
        {
            model.Save();
        }

        public void FileSaveAsBt_Click()
        {
            model.SaveAs();
        }


        public void TreeViewDrop(DropArguments info)
        {
            var insertType = info.Type;
            var targetItem = info.Target;
            var sourceItem = info.Source;
            var targetItemParent = targetItem.Parent;
            var sourceItemParent = sourceItem.Parent;
            if (insertType == MoveableTreeViewBehavior.InsertType.Before)
            {
                sourceItemParent.Node.RemoveChildElement(sourceItem.Node);
                targetItemParent.Node.AddBeforeChildElement(targetItem.Node, sourceItem.Node);
            }
            else if (insertType == MoveableTreeViewBehavior.InsertType.After)
            {
                sourceItemParent.Node.RemoveChildElement(sourceItem.Node);
                targetItemParent.Node.AddAfterChildElement(targetItem.Node, sourceItem.Node);
            }
            else
            {
                if (targetItem.Node.NodeType == XmlNodeType.Tag)
                {
                    sourceItemParent.Node.RemoveChildElement(sourceItem.Node);
                    targetItem.Node.AddChildElement(sourceItem.Node);
                }
            }
        }
        public void TreeViewSelectedItemChanged()
        {
            model.ItemChanged();
        }

        public void TreeView_MouseRightButtonDown()
        {
            var item = TreeViewSelectedItem.Value;
            model.ContextMenuEnabled = item != null;
            model.AddElementEnabled = item != null && !item.IsRoot;
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
            if (model.SelectedItem == null) return;

            if (model.SelectedItem.Node.InnerXml != model.InnerXml)
                model.SelectedItem.IsEdited = true;
        }

        public void ChangeTagName()
        {
            var item = TreeViewSelectedItem.Value;

            item?.EnableTextEdit();
        }

        public void AddChildElement()
        {
            model.AddChildElement();
        }

        public void RemoveElement()
        {
            model.RemoveElement();
        }
        #endregion
    }
}
