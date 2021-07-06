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
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.Views
{

    public class MainWindowService : WindowService
    {
        public MainWindowService(Window window) : base(window)
        {

        }

        public TextBox ConsoleTextBox { get; set; }

        public void Select(int start, int length)
        {
            ConsoleTextBox.Select(start, length);
        }

        public void ScrollToEnd()
        {
            ConsoleTextBox.ScrollToEnd();
        }
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        private readonly IDisposable _model;
        public MainWindow()
        {
            InitializeComponent();

            var windowService = new MainWindowService(this)
            {
                ConsoleTextBox =  ConsoleTextBox
            };
            var mainWindowModel = new Models.MainWindowModel();
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
