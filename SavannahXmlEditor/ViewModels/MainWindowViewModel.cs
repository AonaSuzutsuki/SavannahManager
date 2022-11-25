using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_XmlEditor.Models;
using _7dtd_XmlEditor.Models.TreeView;
using _7dtd_XmlEditor.Views;
using CommonCoreLib;
using CommonStyleLib.File;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using CommonStyleLib.Views.Behaviors.TreeViewBehavior;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SavannahManagerStyleLib.Models.SshFileSelector;
using SavannahManagerStyleLib.ViewModels.SshFileSelector;
using SavannahManagerStyleLib.Views.SshFileSelector;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace _7dtd_XmlEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(IWindowService windowService, MainWindowModel model) : base(windowService, model)
        {
            this._model = model;

            IsEditedTitle = model.ObserveProperty(m => m.IsEditedTitle).ToReactiveProperty().AddTo(CompositeDisposable);
            TreeViewItems = model.TreeViewItems.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable);
            TreeViewSelectedItem = model.ToReactivePropertyAsSynchronized(m => m.SelectedItem).AddTo(CompositeDisposable);

            FullPath = model.ObserveProperty(m => m.FullPath).ToReactiveProperty().AddTo(CompositeDisposable);
            IsAttributesEnabled = model.ObserveProperty(m => m.IsAttributesEnabled).ToReactiveProperty().AddTo(CompositeDisposable);
            Attributes = model.Attributes.ToReadOnlyReactiveCollection().AddTo(CompositeDisposable);
            InnerXml = model.ToReactivePropertyAsSynchronized(m => m.InnerXml).AddTo(CompositeDisposable);
            ContextMenuEnabled = model.ObserveProperty(m => m.ContextMenuEnabled).ToReactiveProperty().AddTo(CompositeDisposable);
            AddElementEnabled = model.ObserveProperty(m => m.AddElementEnabled).ToReactiveProperty().AddTo(CompositeDisposable);

            FileNewBtClick = new DelegateCommand(FileNewBt_Click);
            FileOpenBtClick = new DelegateCommand(FileOpenBt_Click);
            FileSaveBtClick = new DelegateCommand(FileSaveBt_Click);
            FileSaveAsBtClick = new DelegateCommand(FileSaveAsBt_Click);
            OpenSftpCommand = new DelegateCommand(OpenSftp);
            SaveAsSftpCommand = new DelegateCommand(SaveAsSftp);

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

        private readonly MainWindowModel _model;

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
        public ICommand OpenSftpCommand { get; }
        public ICommand SaveAsSftpCommand { get; }


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

        protected override void MainWindow_Closing()
        {
            base.MainWindow_Closing();

            _model.SaveSettings();
        }

        public void FileNewBt_Click()
        {
            _model.NewFile();
        }
        public void FileOpenBt_Click()
        {
            var filePath = FileSelector.GetFilePath(AppInfo.GetAppPath(), "XML Files(*.xml)|*.xml|All Files(*.*)|*.*"
                , "", FileSelector.FileSelectorType.Read);
            _model.OpenFile(filePath);
        }

        public void FileSaveBt_Click()
        {

            _model.Save(FileSaveAsBt_Click);
        }

        public void FileSaveAsBt_Click()
        {
            var filePath = FileSelector.GetFilePath(AppInfo.GetAppPath(), "XML Files(*.xml)|*.xml|All Files(*.*)|*.*",
                "", FileSelector.FileSelectorType.Write);

            _model.SaveAs(filePath);
        }

        public void OpenSftp()
        {
            var model = new FileSelectorModel();
            var vm = new FileSelectorViewModel(new FileSelectorWindowService(), model)
            {
                Mode = FileSelectorMode.Open,
                ConnectionInformation = _model.CreateConnectionInformation()
            };
            model.OpenCallbackAction = item =>
            {
                using var stream = item.Stream;

                _model.OpenFileViaSftp(stream);
            };

            WindowManageService.ShowDialog<FileSelectorView>(vm);

            if (!vm.IsCancel)
                _model.SetToSettingLoader(vm.ConnectionInformation);
        }

        public void SaveAsSftp()
        {
            var model = new FileSelectorModel();
            var vm = new FileSelectorViewModel(new FileSelectorWindowService(), model)
            {
                Mode = FileSelectorMode.SaveAs,
                ConnectionInformation = _model.CreateConnectionInformation()
            };
            model.SaveDataFunction = () =>
            {
                using var stream = _model.CreateXml();
                return stream.ToArray();
            };

            WindowManageService.ShowDialog<FileSelectorView>(vm);

            if (!vm.IsCancel)
                _model.SetToSettingLoader(vm.ConnectionInformation);
        }

        public void TreeViewDrop(DropArguments info)
        {
            var insertType = info.Type;
            var targetItem = (TreeViewItemInfo)info.Target;
            var sourceItem = (TreeViewItemInfo)info.Source;
            var targetItemParent = targetItem.Parent;
            var sourceItemParent = sourceItem.Parent;

            if (!(sourceItemParent.Node is SavannahTagNode sourceItemParentNode) || !(targetItemParent.Node is SavannahTagNode targetItemParentNode))
                return;

            if (insertType == DropArguments.InsertType.Before)
            {
                sourceItemParentNode.RemoveChildElement(sourceItem.Node);
                targetItemParentNode.AddBeforeChildElement(targetItem.Node, sourceItem.Node);
            }
            else if (insertType == DropArguments.InsertType.After)
            {
                sourceItemParentNode.RemoveChildElement(sourceItem.Node);
                targetItemParentNode.AddAfterChildElement(targetItem.Node, sourceItem.Node);
            }
            else
            {
                if (targetItem.Node is SavannahTagNode targetNode)
                {
                    sourceItemParentNode.RemoveChildElement(sourceItem.Node);
                    targetNode.AddChildElement(sourceItem.Node);
                }
                else
                {
                    info.Handled = true;
                }
            }
        }
        public void TreeViewSelectedItemChanged()
        {
            _model.ItemChanged();
        }

        public void TreeView_MouseRightButtonDown()
        {
            var item = TreeViewSelectedItem.Value;
            _model.ContextMenuEnabled = item != null;
            _model.AddElementEnabled = item != null && !item.IsRoot;
        }


        public void AddAttributeBt_Clicked()
        {
            _model.AddAttribute();
        }

        public void RemoveAttributeBt_Clicked()
        {
            _model.RemoveAttribute();
        }

        public void Attributes_SelectionChanged(ViewAttributeInfo info)
        {
            if (info != null)
                _model.AttributesSelectedItem = info;
        }

        public void InnerXml_LostFocus()
        {
            _model.ApplyInnerXml();
        }

        public void InnerXml_TextChanged()
        {
            if (_model.SelectedItem == null) return;

            if (_model.SelectedItem.Node.InnerXml != _model.InnerXml)
                _model.SelectedItem.IsEdited = true;
        }

        public void ChangeTagName()
        {
            var item = TreeViewSelectedItem.Value;

            item?.EnableTextEdit();
        }

        public void AddChildElement()
        {
            _model.AddChildElement();
        }

        public void RemoveElement()
        {
            _model.RemoveElement();
        }
        #endregion
    }
}
