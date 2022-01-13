using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController.Pages
{
    public class PageViewModelBase : BindableBase, IDisposable
    {
        protected CompositeDisposable CompositeDisposable { get; } = new();

        public void Dispose()
        {
            CompositeDisposable?.Dispose();
        }
    }
}
