using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Setup
{
    public class NavigationPageViewModel : NavigationPageViewModelBase
    {
        public NavigationPageViewModel(NavigationBindableValue bindableValue) : base(bindableValue)
        {
        }
    }

    public abstract class NavigationPageViewModelBase : BindableBase, INavigationRefresh
    {
        protected NavigationPageViewModelBase(NavigationBindableValue bindableValue)
        {
            BindableValue = bindableValue;

            LoadedCommand = new DelegateCommand(Loaded);
        }

        protected NavigationBindableValue BindableValue { get; }

        public ICommand LoadedCommand { get; set; }

        public virtual void Loaded()
        {

        }

        public virtual void RefreshValues()
        {
            BindableValue.InitDefaultValue();
        }
    }
}
