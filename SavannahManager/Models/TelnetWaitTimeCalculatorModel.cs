using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonExtensionLib.Extensions;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.ExMessageBox.Views;
using CommonStyleLib.Models;
using SvManagerLibrary.Telnet;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class TelnetWaitTimeCalculatorModel : ModelBase
    {
        private readonly ITelnetClient _telnet;
        private readonly SettingLoader _settingLoader;
        private string _waitTimeText;
        private string _recommendedWaitTimeText;

        public string WaitTimeText
        {
            get => _waitTimeText;
            set => SetProperty(ref _waitTimeText, value);
        }

        public string RecommendedWaitTimeText
        {
            get => _recommendedWaitTimeText;
            set => SetProperty(ref _recommendedWaitTimeText, value);
        }

        public int WaitTime
        {
            get => _waitTimeText.ToInt();
            set => WaitTimeText = value.ToString();
        }

        public int RecommendedWaitTime
        {
            get => RecommendedWaitTimeText.ToInt();
            set => RecommendedWaitTimeText = value.ToString();
        }


        public TelnetWaitTimeCalculatorModel(ITelnetClient telnet, SettingLoader settingLoader)
        {
            _telnet = telnet;
            _settingLoader = settingLoader;
        }

        public void CalculateWaitTime()
        {
            if (_telnet == null || !_telnet.Connected)
                return;
            var time = _telnet.CalculateWaitTime();
            WaitTime = time;

            if (time < 900)
                RecommendedWaitTime = 1000;
            RecommendedWaitTime = time + 500;
        }

        public void SetToSettings(string text)
        {
            var value = text.ToInt();
            if (value <= 0)
                return;

            _settingLoader.TelnetWaitTime = value;

            ExMessageBoxBase.Show(string.Format(LangResources.ToolsResource.UI_SetTelnetWaitTimeMessage, value), "Set To Settings",
                ExMessageBoxBase.MessageType.Exclamation);
        }
    }
}
