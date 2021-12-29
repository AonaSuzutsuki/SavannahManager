using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using CommonStyleLib.Views;
using _7dtd_svmanager_fix_mvvm.Models.WindowModel;

namespace _7dtd_svmanager_fix_mvvm.Views
{

    public class MainWindowService : WindowService
    {
        public MainWindowService(Window window) : base(window)
        {

        }

        public TextBox ConsoleTextBox { get; set; }
        public TextBox CmdTextBox { get; set; }

        public void Select(TextBox textBox, int start, int length)
        {
            textBox.Select(start, length);
        }

        public void ScrollToEnd(TextBox textBox)
        {
            textBox.ScrollToEnd();
        }
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, IRelease
    {
        private readonly IRelease _model;
        public MainWindow()
        {
            InitializeComponent();

            var windowService = new MainWindowService(this)
            {
                ConsoleTextBox =  ConsoleTextBox,
                CmdTextBox = CmdTextBox
            };
            var mainWindowModel = new MainWindowModel();
            var vm = new ViewModels.MainWindowViewModel(windowService, mainWindowModel);
            DataContext = vm;
            _model = mainWindowModel;

            ContentRendered += (sender, args) => { vm.Loaded.Execute(null); };
        }

        #region IDisposable
        // Flag: Has Dispose already been called?
        private bool _disposed;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Release()
        {
            _model?.Release();
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _model?.Dispose();
            }

            _disposed = true;
        }
        #endregion

        private static readonly FieldInfo MenuDropAlignmentField;
        static MainWindow()
        {
            MenuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);
            
            System.Diagnostics.Debug.Assert(MenuDropAlignmentField != null);

            EnsureStandardPopupAlignment();
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
        }
        private static void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            EnsureStandardPopupAlignment();
        }
        private static void EnsureStandardPopupAlignment()
        {
            if (SystemParameters.MenuDropAlignment && MenuDropAlignmentField != null)
            {
                MenuDropAlignmentField.SetValue(null, false);
            }
        }
    }
}
