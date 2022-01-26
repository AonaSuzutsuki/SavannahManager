using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonStyleLib.File;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Microsoft.VisualBasic.ApplicationServices;
using Prism.Commands;
using Reactive.Bindings;
using SavannahManagerStyleLib.Models.SshFileSelector;

namespace SavannahManagerStyleLib.ViewModels.SshFileSelector
{
    public class InputConnectionInfoViewModel: ViewModelBase
    {

        public bool IsCancel { get; private set; } = true;

        public ReactiveProperty<string> Address { get; set; }
        public ReactiveProperty<int> Port { get; set; }
        public ReactiveProperty<string> Username { get; set; }

        public ReactiveProperty<bool> SshPasswordChecked { get; set; }
        public ReactiveProperty<bool> SshKeyChecked { get; set; }

        public ReactiveProperty<string> SshPassword { get; set; }
        public ReactiveProperty<string> SshKeyPath { get; set; }
        public ReactiveProperty<string> SshPassPhrase { get; set; }
        
        public ReactiveProperty<string> WorkingDirectory { get; set; }

        public ICommand SetKeyPathCommand { get; set; }
        public ICommand ConnectCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public InputConnectionInfoViewModel(IWindowService windowService, InputConnectionInfoModel model) : base(windowService, model)
        {
            Address = new ReactiveProperty<string>();
            Port = new ReactiveProperty<int>();
            Username = new ReactiveProperty<string>();
            SshPasswordChecked = new ReactiveProperty<bool>(true);
            SshKeyChecked = new ReactiveProperty<bool>();
            SshPassword = new ReactiveProperty<string>();
            SshKeyPath = new ReactiveProperty<string>();
            SshPassPhrase = new ReactiveProperty<string>();
            WorkingDirectory = new ReactiveProperty<string>();

            SetKeyPathCommand = new DelegateCommand(SetKeyPath);
            ConnectCommand = new DelegateCommand(Connect);
            CancelCommand = new DelegateCommand(Cancel);
        }

        public void SetConnectionInformation(ConnectionInformation information)
        {
            if (information == null)
                return;

            Address.Value = information.Address;
            Port.Value = information.Port;
            Username.Value = information.Username;
            SshPassword.Value = information.Password;
            SshKeyPath.Value = information.KeyPath;
            SshPassPhrase.Value = information.PassPhrase;
            WorkingDirectory.Value = information.DefaultWorkingDirectory;

            if (information.IsPassword)
                SshPasswordChecked.Value = true;
            else
                SshKeyChecked.Value = true;

        }

        private void SetKeyPath()
        {
            var path = FileSelector.GetFilePath("", "All files (*.*)|*.*", "", FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(path))
                SshKeyPath.Value = path;
        }

        public void Connect()
        {
            IsCancel = false;
            WindowManageService.Close();
        }

        public void Cancel()
        {
            WindowManageService.Close();
        }
    }
}
