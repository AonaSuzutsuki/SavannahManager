using _7dtd_svmanager_fix_mvvm.PlayerController.Models.Pages;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels.Pages
{
    public class BanViewModel
    {
        private BanModel model;
        public BanViewModel(BanModel model)
        {
            this.model = model;

            DurationUnitList = model.DurationList.ToReadOnlyReactiveCollection(x => x.Name);
            NameText = model.ToReactivePropertyAsSynchronized(m => m.Name);
            Duration = model.ToReactivePropertyAsSynchronized(m => m.Duration);

            BanBtClicked = new DelegateCommand(BanBt_Clicked);
            CancelBtClicked = new DelegateCommand(model.Cancel);
        }

        #region Properties
        public ReadOnlyCollection<string> DurationUnitList { get; }
        public int DurationUnitListSelectedIndex { get; set; }

        public ReactiveProperty<string> NameText { get; set; }
        public ReactiveProperty<int> Duration { get; set; }
        #endregion

        #region EventProperties
        public ICommand BanBtClicked { get; }
        public ICommand CancelBtClicked { get; }
        #endregion

        #region EventMethods
        private void BanBt_Clicked()
        {
            int index = DurationUnitListSelectedIndex;
            model.AddBan(index);
        }
        #endregion
    }
}
