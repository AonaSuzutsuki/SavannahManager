using CommonStyleLib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonStyleLib.Models;
using System.Windows;
using _7dtd_svmanager_fix_mvvm.Models;
using System.Windows.Input;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using _7dtd_svmanager_fix_mvvm.Models.WindowModel;

namespace _7dtd_svmanager_fix_mvvm.ViewModels
{
    public class VersionInfoViewModel : ViewModelBase
    {
        VersionInfoModel model;
        public VersionInfoViewModel(WindowService windowService, VersionInfoModel model) : base(windowService, model)
        {
            this.model = model;

            Loaded = new DelegateCommand(Window_Loaded);
            CopyDonationAddressCommand = new DelegateCommand<string>(DonateBt_Clicked);

            VersionLabel = model.ObserveProperty(m => m.Version).ToReactiveProperty().AddTo(CompositeDisposable);
            Copyright = model.ObserveProperty(m => m.Copyright).ToReactiveProperty().AddTo(CompositeDisposable);
        }

        #region Properties
        public ReactiveProperty<string> VersionLabel { get; set; }
        public ReactiveProperty<string> Copyright { get; set; }
        #endregion

        #region EventProperties
        public ICommand CopyDonationAddressCommand { get; set; }
        #endregion

        #region EventMethods
        public void Window_Loaded()
        {
            model.SetVersion();
        }
        public void DonateBt_Clicked(string e)
        {
            model.SetAddressToClipboard(e);
        }
        #endregion
    }
}
