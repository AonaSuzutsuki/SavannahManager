using System;
using System.Windows;

namespace _7dtd_svmanager_fix_mvvm.Views.Permissions
{
    /// <summary>
    /// UnBanDateSetting.xaml の相互作用ロジック
    /// </summary>
    public partial class UnBanDateSetting : Window
    {
        public UnBanDateSetting()
        {
            InitializeComponent();
        }

        public void SetDisplayDate(DateTime dateTime)
        {
            DateCalendar.DisplayDate = dateTime;
        }
    }
}
