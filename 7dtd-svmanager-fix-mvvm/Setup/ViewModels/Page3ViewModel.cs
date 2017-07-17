using _7dtd_svmanager_fix_mvvm.Setup.Models;
using _7dtd_svmanager_fix_mvvm.Setup.Views;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.Windows.Input;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class Page3ViewModel : NotifyBase
    {
        Page3 page;
        Page3Model model;
        public Page3ViewModel(Page3 page, Page3Model model)
        {
            this.page = page;
            this.model = model;

            GetPathBTClick = new DelegateCommand(GetPathBT_Click);
            AutoSearchBTClick = new DelegateCommand(AutoSearchBT_Click);

            ServerConfigPathText = model.ToReactivePropertyAsSynchronized(m => m.ServerConfigPathText);
        }

        public ReactiveProperty<string> ServerConfigPathText { get; set; }

        public ICommand GetPathBTClick { get; set; }
        public ICommand AutoSearchBTClick { get; set; }

        public void GetPathBT_Click()
        {
            model.SelectAndGetFilePath();
        }
        public void AutoSearchBT_Click()
        {
            model.AutoSearchAndGetFilePath();
        }
    }
}
