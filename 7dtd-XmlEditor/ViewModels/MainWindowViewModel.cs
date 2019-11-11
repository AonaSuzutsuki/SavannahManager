using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using _7dtd_XmlEditor.Models;
using _7dtd_XmlEditor.Models.TreeView;
using CommonExtensionLib.Extensions;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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

            this.model = model;

            TreeViewItems = model.TreeViewItems.ToReadOnlyReactiveCollection(m => m);
            EditModeComboItems = model.EditModeComboItems.ToReadOnlyReactiveCollection(m => m);

            TreeViewSelectedItemChanged = new DelegateCommand(TreeView_SelectedItemChanged);
            TreeViewMouseRightButtonDown = new DelegateCommand(TreeView_MouseRightButtonDown);
            EditModeComboSelectionChanged = new DelegateCommand<string>(EditModeCombo_SelectionChanged);
            ApplyBtClicked = new DelegateCommand(ApplyBt_Clicked);
        }

        private MainWindowModel model;

        public ReadOnlyReactiveCollection<TreeViewItemInfo> TreeViewItems { get; set; }
        public TreeViewItemInfo TreeViewSelectedItem { get; set; }
        public ReadOnlyReactiveCollection<string> EditModeComboItems { get; set; }

        public ICommand TreeViewSelectedItemChanged { get; set; }
        public ICommand TreeViewMouseRightButtonDown { get; set; }
        public ICommand EditModeComboSelectionChanged { get; set; }
        public ICommand ApplyBtClicked { get; set; }

        public void TreeView_SelectedItemChanged()
        {
            model.SelectionChange(TreeViewSelectedItem);
        }
        public void TreeView_MouseRightButtonDown()
        {
            Console.WriteLine(TreeViewSelectedItem.Path);
        }
        public void EditModeCombo_SelectionChanged(string mode)
        {
            model.NodeViewModeChange(mode, TreeViewSelectedItem);
        }

        public void ApplyBt_Clicked()
        {
            model.Apply(TreeViewSelectedItem);
        }
    }
}
