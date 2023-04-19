using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;
using CommonStyleLib.Models.Errors;
using Reactive.Bindings;

namespace _7dtd_svmanager_fix_mvvm.Models.Settings.ScheduledCommand
{
    public class AddCommandModel : ModelBase
    {
        #region Fields

        private string _commandText;
        private string _intervalText;
        private string _waitTimeText;

        private int _intervalTimeMode;
        private int _waitTimeMode;

        #endregion

        #region Properties

        public Scheduled.ScheduledCommand Command { get; set; }

        public string CommandText
        {
            get => _commandText;
            set => SetProperty(ref _commandText, value);
        }

        public string IntervalText
        {
            get => _intervalText;
            set => SetProperty(ref _intervalText, value);
        }

        public string WaitTimeText
        {
            get => _waitTimeText;
            set => SetProperty(ref _waitTimeText, value);
        }

        public int IntervalTimeMode
        {
            get => _intervalTimeMode;
            set => SetProperty(ref _intervalTimeMode, value);
        }

        public int WaitTimeMode
        {
            get => _waitTimeMode;
            set => SetProperty(ref _waitTimeMode, value);
        }

        #endregion

        public bool Add()
        {
            var item = CheckParameter();
            if (item == null)
                return false;

            Command = new Scheduled.ScheduledCommand(CommandText, item.Value.waitTime, item.Value.interval, WaitTimeMode, IntervalTimeMode);

            return true;
        }

        public bool Edit()
        {
            return Add();
        }

        private (int waitTime, int interval)? CheckParameter()
        {
            if (!int.TryParse(WaitTimeText, out var waitTime))
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs
                {
                    ErrorMessage = "Enter \"Wait Time\" as a number greater than or equal to 0."
                });

                return null;
            }

            if (!int.TryParse(IntervalText, out var interval) || interval <= 0)
            {
                ErrorOccurredSubject.OnNext(new ModelErrorEventArgs
                {
                    ErrorMessage = "Enter \"Interval\" as a number greater than or equal to 1."
                });

                return null;
            }

            return (waitTime, interval);
        }
    }
}
