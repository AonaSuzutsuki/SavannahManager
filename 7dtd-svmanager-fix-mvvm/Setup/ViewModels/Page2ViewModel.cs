using _7dtd_svmanager_fix_mvvm.Setup.Models;
using Reactive.Bindings;
using System.Windows.Input;
using Reactive.Bindings.Extensions;
using Prism.Commands;
using _7dtd_svmanager_fix_mvvm.Setup.Views;
using CommonStyleLib.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.Setup.ViewModels
{
    public class Page2ViewModel
    {
        Page2 page;
        public Page2ViewModel(Page2 page,Page2Model model)
        {
            this.page = page;
            this.model = model;

            GetPathBtClick = new DelegateCommand(GetPathBt_Click);
            AutoSearchBtClick = new DelegateCommand(AutoSearchBt_Click);

            ServerFilePathText = model.ToReactivePropertyAsSynchronized(m => m.ServerFilePathText);
        }

        public ReactiveProperty<string> ServerFilePathText { get; set; }

        public ICommand GetPathBtClick { get; set; }
        public ICommand AutoSearchBtClick { get; set; }

        private Page2Model model;

        public void GetPathBt_Click()
        {
            model.SelectAndGetFilePath();
        }
        public void AutoSearchBt_Click()
        {
            model.AutoSearchAndGetFilePath();
        }
    }
}
