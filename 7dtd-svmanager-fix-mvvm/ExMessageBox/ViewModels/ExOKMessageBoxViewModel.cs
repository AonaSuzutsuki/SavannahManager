using _7dtd_svmanager_fix_mvvm.ViewModels;
using ExMessageBox.Models;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace ExMessageBox.ViewModels
{
    public class ExOKMassageBoxViewModel : ViewModelBase
    {
        new Views.ExOKMassageBox view;
        ExMassageBoxModel model;
        public ExOKMassageBoxViewModel(Views.ExOKMassageBox view, ExMassageBoxModel model) : base(view, model)
        {
            this.view = view;
            this.model = model;

            MsgText = model.ToReactivePropertyAsSynchronized(m => m.Text);
            MsgTitle = model.ToReactivePropertyAsSynchronized(m => m.Title);

            OkBtClick = new DelegateCommand(OkBt_Click);
        }
        
        public ReactiveProperty<string> MsgText { get; set; }
        public ReactiveProperty<string> MsgTitle { get; set; }

        public ICommand OkBtClick { get; set; }

        public void Loaded()
        {
            model.Load(view.GetHandel());
        }
        private void OkBt_Click()
        {
            model.Result = ExMessageBoxBase.DialogResult.OK;
            base.MainWindowCloseBT_Click();
        }
    }
}
