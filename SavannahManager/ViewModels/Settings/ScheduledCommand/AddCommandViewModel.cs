using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Settings.ScheduledCommand;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Settings.ScheduledCommand
{
    public class AddCommandViewModel : ViewModelWindowStyleBase
    {
        #region Fields

        private readonly AddCommandModel _model;

        #endregion

        #region Properties

        public bool IsEditMode { get; } = false;

        public bool IsCancel { get; private set; } = true;

        public IEnumerable<string> HourMinuteSecondItems { get; set; }
        public IEnumerable<string> MinuteSecondItems { get; set; }

        public ReactiveProperty<string> CommandText { get; set; }
        public ReactiveProperty<string> IntervalText { get; set; }
        public ReactiveProperty<string> WaitTimeText { get; set; }

        public ReactiveProperty<int> IntervalTimeMode { get; set; }
        public ReactiveProperty<int> WaitTimeMode { get; set; }

        public ReactiveProperty<string> ApplyButtonContent { get; set; }

        #endregion

        #region Event Properties

        public ICommand ApplyCommand { get; set; }

        #endregion

        public AddCommandViewModel(IWindowService windowService, AddCommandModel model, bool isEditMode) : base(windowService, model)
        {
            _model = model;
            IsEditMode = isEditMode;

            HourMinuteSecondItems = new List<string>
            {
                LangResources.SettingsResources.UI_Second,
                LangResources.SettingsResources.UI_Minute,
                LangResources.SettingsResources.UI_Hour
            };

            MinuteSecondItems = new List<string>
            {
                LangResources.SettingsResources.UI_Second,
                LangResources.SettingsResources.UI_Minute
            };

            CommandText = model.ToReactivePropertyAsSynchronized(m => m.CommandText).AddTo(CompositeDisposable);
            IntervalText = model.ToReactivePropertyAsSynchronized(m => m.IntervalText).AddTo(CompositeDisposable);
            WaitTimeText = model.ToReactivePropertyAsSynchronized(m => m.WaitTimeText).AddTo(CompositeDisposable);
            IntervalTimeMode = model.ToReactivePropertyAsSynchronized(m => m.IntervalTimeMode).AddTo(CompositeDisposable);
            WaitTimeMode = model.ToReactivePropertyAsSynchronized(m => m.WaitTimeMode).AddTo(CompositeDisposable);
            ApplyButtonContent = new ReactiveProperty<string>(IsEditMode ? "Edit": "Add");

            ApplyCommand = new DelegateCommand(Apply);

            model.ErrorOccurred.Subscribe(message =>
            {
                if (message.IsAsync)
                {
                    windowService.MessageBoxDispatchShow(message.ErrorMessage,
                        LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                }
                else
                {
                    windowService.MessageBoxShow(message.ErrorMessage,
                        LangResources.CommonResources.Error, ExMessageBoxBase.MessageType.Exclamation);
                }
            });
        }

        public void Apply()
        {
            if (IsEditMode)
            {
                if (!_model.Edit())
                    return;
            }
            else
            {
                if (!_model.Add())
                    return;
            }

            IsCancel = false;
            WindowManageService.Close();
        }
    }
}
