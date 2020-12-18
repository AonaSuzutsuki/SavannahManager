using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Permissions.Models;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Permissions.ViewModels
{
    public class UnBanDateSettingViewModel : ViewModelBase
    {
        public UnBanDateSettingViewModel(IWindowService windowService, UnBanDateSettingModel model) : base(windowService, model)
        {
            SelectedDate = model.ToReactivePropertyAsSynchronized(m => m.SelectedDate);
            HourText = model.ToReactivePropertyAsSynchronized(m => m.HourText);
            MinuteText = model.ToReactivePropertyAsSynchronized(m => m.MinuteText);
            SecondText = model.ToReactivePropertyAsSynchronized(m => m.SecondText);
        }

        #region Properties

        public ReactiveProperty<DateTime?> SelectedDate { get; set; }

        public ReactiveProperty<int> HourText { get; set; }
        public ReactiveProperty<int> MinuteText { get; set; }
        public ReactiveProperty<int> SecondText { get; set; }

        #endregion
    }
}
