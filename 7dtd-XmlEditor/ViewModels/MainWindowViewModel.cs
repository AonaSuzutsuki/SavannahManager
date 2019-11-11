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

            EditModeComboItems = model.EditModeComboItems.ToReadOnlyReactiveCollection(m => m);
            EditModeSelectedItem = model.ToReactivePropertyAsSynchronized(m => m.EditModeSelectedItem);

            FileOpenBtClick = new DelegateCommand(FileOpenBt_Click);
            EditModeComboSelectionChanged = new DelegateCommand<string>(EditModeCombo_SelectionChanged);
        }

        #region Fields
        private MainWindowModel model;
        #endregion

        #region Properties
        public ReadOnlyReactiveCollection<string> EditModeComboItems { get; set; }
        public ReactiveProperty<string> EditModeSelectedItem { get; set; }
        #endregion

        #region Event Properties
        public ICommand FileOpenBtClick { get; set; }
        public ICommand EditModeComboSelectionChanged { get; set; }
        #endregion

        #region Event Methods

        public void FileOpenBt_Click()
        {
            model.OpenFile();
        }
        public void EditModeCombo_SelectionChanged(string mode)
        {
            model.NodeViewModeChange(mode);
        }
        #endregion
    }
}
