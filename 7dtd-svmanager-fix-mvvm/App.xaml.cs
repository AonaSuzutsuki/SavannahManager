using _7dtd_svmanager_fix_mvvm.Views;
using CommonLib;
using System;
using System.IO;
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
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.mainWindow = mainWindow;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string mes = string.Format("予期せぬエラーが発生しました。\r\nお手数ですが、開発者に例外内容を報告してください。\r\n\r\n---\r\n\r\n{0}\r\n\r\n{1}",
                e.Exception.Message, e.Exception.StackTrace);
            MessageBox.Show(mes, "予期せぬエラー", MessageBoxButton.OK, MessageBoxImage.Error);

            DateTime dt = DateTime.Now;
            OutToFile(AppInfo.GetAppPath() + @"\error-" + dt.ToString("yyyy-MM-dd- HH-mm-ss") + ".log", mes);

            mainWindow.Dispose();

            e.Handled = true;
            Shutdown();
        }

        private void OutToFile(string filename, string text)
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
            {
                using (var sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    sw.Write(text);
                }
            }
        }
    }
}
