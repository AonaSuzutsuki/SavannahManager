using System.Collections.ObjectModel;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController.Pages;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController.Pages
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

            BanPlayerCommanded = new DelegateCommand(BanBt_Clicked);
            CancelCommand = new DelegateCommand(model.Cancel);
        }

        #region Properties
        public ReadOnlyCollection<string> DurationUnitList { get; }
        public int DurationUnitListSelectedIndex { get; set; }

        public ReactiveProperty<string> NameText { get; set; }
        public ReactiveProperty<int> Duration { get; set; }
        #endregion

        #region EventProperties
        public ICommand BanPlayerCommanded { get; }
        public ICommand CancelCommand { get; }
        #endregion

        #region EventMethods
        private void BanBt_Clicked()
        {
            var index = DurationUnitListSelectedIndex;
            model.AddBan(index);
        }
        #endregion
    }
}
