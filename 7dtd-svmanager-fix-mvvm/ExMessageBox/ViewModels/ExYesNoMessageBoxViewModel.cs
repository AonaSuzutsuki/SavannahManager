using _7dtd_svmanager_fix_mvvm.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_svmanager_fix_mvvm.Models;
using System.Windows;
using Reactive.Bindings;
using System.Windows.Input;
using System.Windows.Interop;
using ExMessageBox.Models;
using Reactive.Bindings.Extensions;
using Prism.Commands;

namespace ExMessageBox.ViewModels
{
    public class ExYesNoMessageBoxViewModel : ViewModelBase
    {
        new Views.ExYesNoMessageBox view;
        ExMassageBoxModel model;
        public ExYesNoMessageBoxViewModel(Views.ExYesNoMessageBox view, Models.ExMassageBoxModel model) : base(view, model)
        {
            this.view = view;
            this.model = model;

            MsgText = model.ToReactivePropertyAsSynchronized(m => m.Text);
            MsgTitle = model.ToReactivePropertyAsSynchronized(m => m.Title);

            YesBtClick = new DelegateCommand(YesBt_Click);
            NoBtClick = new DelegateCommand(NoBt_Click);
        }

        public ReactiveProperty<string> MsgText { get; set; }
        public ReactiveProperty<string> MsgTitle { get; set; }

        public ICommand YesBtClick { get; set; }
        public ICommand NoBtClick { get; set; }

        public void Loaded()
        {
            model.Load(view.GetHandel());
        }
        private void YesBt_Click()
        {
            model.Result = ExMessageBoxBase.DialogResult.Yes;
            base.MainWindowCloseBT_Click();
        }
        private void NoBt_Click()
        {
            model.Result = ExMessageBoxBase.DialogResult.No;
            base.MainWindowCloseBT_Click();
        }
    }
}
