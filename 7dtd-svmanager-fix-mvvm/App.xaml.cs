using _7dtd_svmanager_fix_mvvm.Views;
using CommonCoreLib;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace _7dtd_svmanager_fix_mvvm
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private IDisposable mainWindow;
        private void MyApp_Startup(object sender, StartupEventArgs e)
        {
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

            var mainWindow = new MainWindow();
            this.mainWindow = mainWindow;
            mainWindow.Show();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                ShowAndWriteException(exception);
                mainWindow.Dispose();
            }
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            ShowAndWriteException(e.Exception);
            mainWindow.Dispose();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ShowAndWriteException(e.Exception);
            mainWindow.Dispose();

            e.Handled = true;
            Shutdown();
        }

        public static void ShowAndWriteException(Exception exception)
        {
            if (exception == null)
                return;

            var mes = string.Format("予期せぬエラーが発生しました。\r\nお手数ですが、開発者に例外内容を報告してください。\r\n\r\n---\r\n\r\n{0}\r\n\r\n{1}",
                exception.Message, exception.StackTrace);
            MessageBox.Show(mes, "予期せぬエラー", MessageBoxButton.OK, MessageBoxImage.Error);

            DateTime dt = DateTime.Now;
            OutToFile(AppInfo.GetAppPath() + @"\error-" + dt.ToString("yyyy-MM-dd- HH-mm-ss") + ".log", mes);
        }

        private static void OutToFile(string filename, string text)
        {
            using var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
            using var sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
            sw.Write(text);
        }
    }
}
