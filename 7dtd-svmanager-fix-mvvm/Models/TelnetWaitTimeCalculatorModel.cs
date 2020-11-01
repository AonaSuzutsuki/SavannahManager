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
        private readonly ITelnetClient telnet;
        private readonly SettingLoader settingLoader;
        private string waitTimeText;
        private string recommendedWaitTimeText;

        public string WaitTimeText
        {
            get => waitTimeText;
            set => SetProperty(ref waitTimeText, value);
        }

        public string RecommendedWaitTimeText
        {
            get => recommendedWaitTimeText;
            set => SetProperty(ref recommendedWaitTimeText, value);
        }

        public int WaitTime
        {
            get => waitTimeText.ToInt();
            set => WaitTimeText = value.ToString();
        }

        public int RecommendedWaitTime
        {
            get => RecommendedWaitTimeText.ToInt();
            set => RecommendedWaitTimeText = value.ToString();
        }


        public TelnetWaitTimeCalculatorModel(ITelnetClient telnet, SettingLoader settingLoader)
        {
            this.telnet = telnet;
            this.settingLoader = settingLoader;
        }

        public void CalculateWaitTime()
        {
            if (telnet == null || !telnet.Connected)
                return;
            var time = telnet.CalculateWaitTime();
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

            settingLoader.TelnetWaitTime = value;

            ExMessageBoxBase.Show(string.Format(LangResources.ToolsResource.UI_SetTelnetWaitTimeMessage, value), "Set To Settings",
                ExMessageBoxBase.MessageType.Exclamation);
        }
    }
}
