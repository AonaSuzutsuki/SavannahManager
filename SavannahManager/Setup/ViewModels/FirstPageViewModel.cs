using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.Setup.Models;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class FirstPageViewModel : NavigationPageViewModelBase
    {
        private readonly FirstPageModel _model;

        public ReactiveProperty<int> LanguagesSelectedIndex { get; set; }
        public ReadOnlyCollection<string> Languages { get; set; }

        public ICommand SelectionChangedCommand { get; set; }

        public FirstPageViewModel(NavigationWindowService<InitializeData> bindableValue, FirstPageModel model) : base(bindableValue?.NavigationValue)
        {
            _model = model;
            LanguagesSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.LanguagesSelectedIndex);
            Languages = model.Languages.ToReadOnlyReactiveCollection(m => m.Item1);

            SelectionChangedCommand = new DelegateCommand(SelectionChanged);
        }

        public void SelectionChanged()
        {
            _model.ChangeCulture();
            base.RefreshValues();
        }
    }
}
