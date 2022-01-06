using System;
using System.Collections.Generic;
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

namespace _7dtd_svmanager_fix_mvvm.Views.UserControls
{
    /// <summary>
    /// SshControl.xaml の相互作用ロジック
    /// </summary>
    public partial class SshControl : UserControl
    {
        public string SshAddress
        {
            get => (string)GetValue(SshAddressProperty);
            set => SetValue(SshAddressProperty, value);
        }
        public static readonly DependencyProperty SshAddressProperty = DependencyProperty.Register(nameof(SshAddress),
                typeof(string),
                typeof(SshControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public string SshPort
        {
            get => (string)GetValue(SshPortProperty);
            set => SetValue(SshPortProperty, value);
        }
        public static readonly DependencyProperty SshPortProperty = DependencyProperty.Register(nameof(SshPort),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public string SshUserName
        {
            get => (string)GetValue(SshUserNameProperty);
            set => SetValue(SshUserNameProperty, value);
        }
        public static readonly DependencyProperty SshUserNameProperty = DependencyProperty.Register(nameof(SshUserName),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public string SshPassword
        {
            get => (string)GetValue(SshPasswordProperty);
            set => SetValue(SshPasswordProperty, value);
        }
        public static readonly DependencyProperty SshPasswordProperty = DependencyProperty.Register(nameof(SshPassword),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public string SshExeFileDirectory
        {
            get => (string)GetValue(SshExeFileDirectoryProperty);
            set => SetValue(SshExeFileDirectoryProperty, value);
        }
        public static readonly DependencyProperty SshExeFileDirectoryProperty = DependencyProperty.Register(nameof(SshExeFileDirectory),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public string SshConfigFileName
        {
            get => (string)GetValue(SshConfigFileNameProperty);
            set => SetValue(SshConfigFileNameProperty, value);
        }
        public static readonly DependencyProperty SshConfigFileNameProperty = DependencyProperty.Register(nameof(SshConfigFileName),
            typeof(string),
            typeof(SshControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public SshControl()
        {
            InitializeComponent();
        }
    }
}
