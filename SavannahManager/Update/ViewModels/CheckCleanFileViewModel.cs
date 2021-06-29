using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Update.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Reactive.Bindings;

namespace _7dtd_svmanager_fix_mvvm.Update.ViewModels
{
    public class CheckCleanFileViewModel : ViewModelBase
    {
        public ICollection<DeleteFileInfo> DeleteFiles { get; set; }

        public CheckCleanFileViewModel(IWindowService windowService, CheckCleanFileModel model) : base(windowService, model)
        {
            DeleteFiles = model.DeleteFiles.ToReadOnlyReactiveCollection(m => m);
        }
    }
}
