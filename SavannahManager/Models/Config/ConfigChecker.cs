using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.ExMessageBox.Views;
using CommonStyleLib.Views;
using SvManagerLibrary.Config;

namespace _7dtd_svmanager_fix_mvvm.Models.Config
{
    public class CheckValue : ICheckedValue
    {
        public string TelnetPort { get; set; }
        public string ControlPanelPort { get; set; }
        public string ServerPort { get; set; }
        public string TelnetEnabled { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }

        public bool IsFailed { get; set; }
        public string Message { get; set; }
    }
    public interface ICheckedValue
    {
        string Password { get; }
        int Port { get; }
        public bool IsFailed { get; }
        public string Message { get; }
    }

    public static class ConfigChecker
    {
        public static ICheckedValue GetConfigInfo(string configFilePath)
        {
            var configLoader = new ConfigLoader(configFilePath);
            var portConfig = configLoader.GetProperty("TelnetPort");
            var strPort = portConfig == null ? string.Empty : portConfig.Value;
            var cpPortConfig = configLoader.GetProperty("ControlPanelPort");
            var cpPort = cpPortConfig == null ? string.Empty : cpPortConfig.Value;
            var svPortConfig = configLoader.GetProperty("ServerPort");
            var svPort = svPortConfig == null ? string.Empty : svPortConfig.Value;

            var passConfig = configLoader.GetProperty("TelnetPassword");
            var password = passConfig == null ? "CHANGEME" : passConfig.Value;
            var telnetEnabledString = configLoader.GetProperty("TelnetEnabled").Value;

            var checkValues = new CheckValue()
            {
                TelnetPort = strPort,
                ControlPanelPort = cpPort,
                ServerPort = svPort,
                TelnetEnabled = telnetEnabledString,
                Password = password
            };
            var checkedValues = CheckRightfulness(checkValues);

            return checkedValues;
        }
        public static ICheckedValue CheckRightfulness(CheckValue checkValues)
        {
            var checkedValues = new CheckValue();

            //TelnetEnabledチェック
            if (!bool.TryParse(checkValues.TelnetEnabled, out var telnetEnabled))
            {
                ApplyFailed(string.Format(LangResources.Resources._0_Is_Invalid, "TelnetEnabled"), checkedValues);
            }
            if (!telnetEnabled)
            {
                ApplyFailed(string.Format(LangResources.Resources._0_is_1, "TelnetEnabled", "False"), checkedValues);
            }

            // ポート被りチェック
            if (checkValues.TelnetPort.Equals(checkValues.ControlPanelPort))
            {
                ApplyFailed(string.Format(LangResources.Resources.Same_Port_as_0_has_been_used_in_1,
                "TelnetPort", "ControlPanelPort"), checkedValues);
            }
            if (checkValues.TelnetPort.Equals(checkValues.ServerPort))
            {
                ApplyFailed(string.Format(LangResources.Resources.Same_Port_as_0_has_been_used_in_1,
                    "TelnetPort", "ServerPort"), checkedValues);
            }

            // Telnetポート変換チェック
            if (!int.TryParse(checkValues.TelnetPort, out var port))
            {
                ApplyFailed(string.Format(LangResources.Resources._0_Is_Invalid, LangResources.Resources.Port), checkedValues);
            }
            checkedValues.Port = port;
            checkedValues.Password = checkValues.Password;
            return checkedValues;
        }

        private static void ApplyFailed(string message, CheckValue checkValue)
        {
            checkValue.Message = message;
            checkValue.IsFailed = true;
        }
    }
}
