using CommonCoreLib;
using ConfigEditor_mvvm.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommonStyleLib.Views;

namespace ConfigEditor_mvvm
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void MyApp_Startup(object sender, StartupEventArgs e)
        {
            Window mw = new MainWindow();
            mw.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string mes = string.Format("予期せぬエラーが発生しました。\r\nお手数ですが、開発者に例外内容を報告してください。\r\n\r\n---\r\n\r\n{0}\r\n\r\n{1}",
                e.Exception.Message, e.Exception.StackTrace);
            MessageBox.Show(mes, "予期せぬエラー", MessageBoxButton.OK, MessageBoxImage.Error);

            DateTime dt = DateTime.Now;
            OutToFile(AppInfo.GetAppPath() + @"\configeditor-" + dt.ToString("yyyy-MM-dd- HH-mm-ss") + ".log", mes);

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
