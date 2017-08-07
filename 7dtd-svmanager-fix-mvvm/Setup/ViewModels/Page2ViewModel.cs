using _7dtd_svmanager_fix_mvvm.Setup.Models;
using Reactive.Bindings;
using System.Windows.Input;
using Reactive.Bindings.Extensions;
using Prism.Commands;
using _7dtd_svmanager_fix_mvvm.Setup.Views;
using CommonLib.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class Page2ViewModel
    {
        Page2 page;
        public Page2ViewModel(Page2 page,Page2Model model)
        {
            this.page = page;
            this.model = model;

            GetPathBTClick = new DelegateCommand(GetPathBT_Click);
            AutoSearchBTClick = new DelegateCommand(AutoSearchBT_Click);

            ServerFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ServerFilePathText);
        }

        public ReactiveProperty<string> ServerFilePathText { get; set; }

        public ICommand GetPathBTClick { get; set; }
        public ICommand AutoSearchBTClick { get; set; }

        private Page2Model model;

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
