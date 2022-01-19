using System;
using _7dtd_svmanager_fix_mvvm.Models.Permissions;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Permissions
{
    public class UnBanDateSettingViewModel : ViewModelBase
    {
        public UnBanDateSettingViewModel(IWindowService windowService, UnBanDateSettingModel model) : base(windowService, model)
        {
            SelectedDate = model.ToReactivePropertyAsSynchronized(m => m.SelectedDate).AddTo(CompositeDisposable);
            HourText = model.ToReactivePropertyAsSynchronized(m => m.HourText).AddTo(CompositeDisposable);
            MinuteText = model.ToReactivePropertyAsSynchronized(m => m.MinuteText).AddTo(CompositeDisposable);
            SecondText = model.ToReactivePropertyAsSynchronized(m => m.SecondText).AddTo(CompositeDisposable);
        }

        #region Properties

        public ReactiveProperty<DateTime?> SelectedDate { get; set; }

        public ReactiveProperty<int> HourText { get; set; }
        public ReactiveProperty<int> MinuteText { get; set; }
        public ReactiveProperty<int> SecondText { get; set; }

        #endregion
    }
}
