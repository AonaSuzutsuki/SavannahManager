﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class ServerProcessManager
    {
        private Process p = new Process();
        public ServerProcessManager(string exeFilePath, string configFilePath)
        {
            p.StartInfo = new ProcessStartInfo() {
                FileName = exeFilePath,
                Arguments = string.Format("-quit -batchmode -nographics -configfile=\"{0}\" -dedicated", configFilePath),
                UseShellExecute = false,
                CreateNoWindow = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = Path.GetDirectoryName(exeFilePath)
            };
        }

        public bool ProcessStart(Action<string> failedAction = null)
        {
            try
            {
                p.Start();
            }
            catch (Win32Exception ex)
            {
                failedAction?.Invoke(ex.Message);
                return false;
            }
            return true;
        }

        public void ProcessKill()
        {
            p.Kill();
        }
    }
}
