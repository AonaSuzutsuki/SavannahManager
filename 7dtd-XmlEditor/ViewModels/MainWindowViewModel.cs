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

            EditModeComboSelectionChanged = new DelegateCommand<string>(EditModeCombo_SelectionChanged);
        }

        private MainWindowModel model;

        public ReadOnlyReactiveCollection<string> EditModeComboItems { get; set; }

        public ICommand EditModeComboSelectionChanged { get; set; }

        public void EditModeCombo_SelectionChanged(string mode)
        {
            model.NodeViewModeChange(mode);
        }

    }
}
