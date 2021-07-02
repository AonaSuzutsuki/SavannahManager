using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Update.Models;
using _7dtd_svmanager_fix_mvvm.Update.Models.Node;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;

namespace _7dtd_svmanager_fix_mvvm.Update.ViewModels
{
    public class CheckCleanFileViewModel : ViewModelBase
    {
        private readonly CheckCleanFileModel _model;

        public ReadOnlyReactiveCollection<DirectoryNode> TreeViewItems { get; set; }

        public ICommand AllSelectCommand { get; set; }
        public ICommand AllDeSelectCommand { get; set; }
        public ICommand DoUpdateCommand { get; set; }

        public CheckCleanFileViewModel(IWindowService windowService, CheckCleanFileModel model) : base(windowService, model)
        {
            _model = model;

            TreeViewItems = model.TreeViewItems.ToReadOnlyReactiveCollection();

            AllSelectCommand = new DelegateCommand(AllSelect);
            AllDeSelectCommand = new DelegateCommand(AllDeSelect);
            DoUpdateCommand = new DelegateCommand(DoUpdate);
        }

        public void AllSelect()
        {
            _model.SetAllDeleteTarget();
        }

        public void AllDeSelect()
        {
            _model.SetAllNotDeleteTarget();
        }

        public void DoUpdate()
        {
            _model.CanCleanUpdate = true;
            WindowManageService.Close();
        }
    }
}
