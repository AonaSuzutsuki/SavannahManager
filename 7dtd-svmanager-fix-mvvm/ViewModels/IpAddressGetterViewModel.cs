using _7dtd_svmanager_fix_mvvm.Models;
using CommonLib.Models;
using CommonLib.ViewModels;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class IpAddressGetterViewModel : ViewModelBase
    {
        public IpAddressGetterViewModel(Window view, IpAddressGetterModel model) : base(view, model)
        {
            this.model = model;

            #region EventInitialize
            GetIpClicked = new DelegateCommand(GetIp_Clicked);
            #endregion
        }

        #region Fields
        private IpAddressGetterModel model;
        #endregion

        #region Properties

        #endregion

        #region EventProperties
        public ICommand GetIpClicked { get; set; }
        #endregion

        #region EventMethods
        private void GetIp_Clicked()
        {
            model.SetIpAddress();
        }
        #endregion
    }
}
