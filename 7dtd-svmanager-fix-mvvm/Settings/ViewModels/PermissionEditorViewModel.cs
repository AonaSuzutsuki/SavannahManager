using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Settings.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Reactive.Bindings;

namespace _7dtd_svmanager_fix_mvvm.Settings.ViewModels
{
    public class PermissionEditorViewModel : ViewModelBase
    {
        public PermissionEditorViewModel(IWindowService windowService, PermissionEditorModel model) : base(windowService, model)
        {

            CommandPermissions = model.CommandPermissions.ToReadOnlyReactiveCollection(m => m);
        }

        #region Fields



        #endregion

        #region Properties

        public ReadOnlyReactiveCollection<PermissionInfo> CommandPermissions { get; set; }

        #endregion

        #region Event Properties



        #endregion

        #region Event Methods



        #endregion
    }
}
