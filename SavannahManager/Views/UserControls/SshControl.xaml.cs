using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonExtensionLib.Extensions;
using CommonStyleLib.File;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using SavannahManagerStyleLib.Extensions;
using SavannahManagerStyleLib.Models.SshFileSelector;
using SavannahManagerStyleLib.ViewModels.SshFileSelector;
using SavannahManagerStyleLib.Views.SshFileSelector;

namespace _7dtd_svmanager_fix_mvvm.Views.UserControls
{
    /// <summary>
    /// SshControl.xaml の相互作用ロジック
    /// </summary>
    public partial class SshControl : UserControl
    {
        #region Fields

        private Window _owner;
        private WindowService _windowService;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SshAddressProperty = DependencyProperty.Register(nameof(SshAddress),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SshPortProperty = DependencyProperty.Register(nameof(SshPort),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SshUserNameProperty = DependencyProperty.Register(nameof(SshUserName),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SshAuthModeProperty = DependencyProperty.Register(nameof(SshAuthMode),
            typeof(AuthMode),
            typeof(SshControl),
            new FrameworkPropertyMetadata(AuthMode.Password, 
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SshAuthModePropertyChanged));

        public static readonly DependencyProperty SshPasswordProperty = DependencyProperty.Register(nameof(SshPassword),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SshKeyPathProperty = DependencyProperty.Register(nameof(SshKeyPath),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SshPassPhraseProperty = DependencyProperty.Register(nameof(SshPassPhrase),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SshExeFileDirectoryProperty = DependencyProperty.Register(nameof(SshExeFileDirectory),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SshShellScriptFileNameProperty = DependencyProperty.Register(nameof(SshShellScriptFileName),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SshArgumentProperty =DependencyProperty.Register(nameof(SshArgument),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string SshAddress
        {
            get => (string)GetValue(SshAddressProperty);
            set => SetValue(SshAddressProperty, value);
        }

        public string SshPort
        {
            get => (string)GetValue(SshPortProperty);
            set => SetValue(SshPortProperty, value);
        }

        public string SshUserName
        {
            get => (string)GetValue(SshUserNameProperty);
            set => SetValue(SshUserNameProperty, value);
        }

        public AuthMode SshAuthMode
        {
            get => (AuthMode)GetValue(SshAuthModeProperty);
            set => SetValue(SshAuthModeProperty, value);
        }

        public string SshPassword
        {
            get => (string)GetValue(SshPasswordProperty);
            set => SetValue(SshPasswordProperty, value);
        }

        public string SshKeyPath
        {
            get => (string)GetValue(SshKeyPathProperty);
            set => SetValue(SshKeyPathProperty, value);
        }

        public string SshPassPhrase
        {
            get => (string)GetValue(SshPassPhraseProperty);
            set => SetValue(SshPassPhraseProperty, value);
        }

        public string SshExeFileDirectory
        {
            get => (string)GetValue(SshExeFileDirectoryProperty);
            set => SetValue(SshExeFileDirectoryProperty, value);
        }

        public string SshShellScriptFileName
        {
            get => (string)GetValue(SshShellScriptFileNameProperty);
            set => SetValue(SshShellScriptFileNameProperty, value);
        }

        public string SshArgument
        {
            get => (string)GetValue(SshArgumentProperty);
            set => SetValue(SshArgumentProperty, value);
        }

        #endregion

        #region Properties

        public ReactiveProperty<bool> SshPasswordChecked { get; set; }
        public ReactiveProperty<bool> SshKeyChecked { get; set; }

        #endregion

        #region Event Properties

        public ICommand SetKeyPathCommand { get; set; }
        public ICommand SetPathFromSftpCommand { get; set; }

        #endregion

        public SshControl()
        {
            InitializeComponent();

            SshPasswordChecked = new ReactiveProperty<bool>(true);
            SshPasswordChecked.PropertyChanged += SshAuthModeChanged;
            SshKeyChecked = new ReactiveProperty<bool>();
            SshKeyChecked.PropertyChanged += SshAuthModeChanged;

            SetKeyPathCommand = new DelegateCommand(SetKeyPath);
            SetPathFromSftpCommand = new DelegateCommand<string>(SetPathFromSftp);

            Loaded += (sender, args) =>
            {
                if (sender is not SshControl control)
                    return;

                var w = Window.GetWindow(this);
                _owner = w;

                _windowService = new WindowService(_owner);
            };
        }

        private void SetKeyPath()
        {
            var path = FileSelector.GetFilePath("", "All files (*.*)|*.*", "", FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(path))
                SshKeyPath = path;
        }

        private void SetPathFromSftp(string mode)
        {
            var directoryMode = FileDirectoryMode.File;

            var model = new FileSelectorModel
            {
                IsOpenStream = false
            };

            if (mode == "7dtdDirectory")
            {
                directoryMode = FileDirectoryMode.Directory;

                model.OpenCallbackAction = item =>
                {
                    var path = item.FullPath;
                    SshExeFileDirectory = path.Replace(" ", "\\ ");
                };
            }
            else if (mode == "7dtdShellScriptFileName")
            {
                directoryMode = FileDirectoryMode.File;

                model.TargetExtensions = new HashSet<string>
                {
                    ".sh"
                };
                model.OpenCallbackAction = item =>
                {
                    var path = item.FullPath;
                    SshShellScriptFileName = System.IO.Path.GetFileName(path);
                };
            }

            model.DirectoryMode = directoryMode;

            var vm = new FileSelectorViewModel(new FileSelectorWindowService(), model)
            {
                Mode = FileSelectorMode.Open,
                ConnectionInformation = new ConnectionInformation
                {
                    Address = SshAddress,
                    Port = SshPort.ToInt(),
                    IsPassword = SshAuthMode == AuthMode.Password,
                    DefaultWorkingDirectory = "",
                    KeyPath = SshKeyPath,
                    Username = SshUserName,
                    Password = SshPassword,
                    PassPhrase = SshPassPhrase
                }
            };

            _windowService.ShowDialog<FileSelectorView>(vm);
        }

        private static void SshAuthModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not SshControl control)
                return;
            if (e.NewValue is not AuthMode authMode)
                return;

            if (authMode == AuthMode.Password && !control.SshPasswordChecked.Value)
            {
                control.SshPasswordChecked.Value = true;
                control.SshKeyChecked.Value = false;
            }
            else if (authMode == AuthMode.Key && !control.SshKeyChecked.Value)
            {
                control.SshPasswordChecked.Value = false;
                control.SshKeyChecked.Value = true;
            }
        }

        private void SshAuthModeChanged(object sender, PropertyChangedEventArgs e)
        {
            if (SshPasswordChecked.Value)
            {
                SshAuthMode = AuthMode.Password;
            }
            else if (SshKeyChecked.Value)
            {
                SshAuthMode = AuthMode.Key;
            }
        }
    }
}
