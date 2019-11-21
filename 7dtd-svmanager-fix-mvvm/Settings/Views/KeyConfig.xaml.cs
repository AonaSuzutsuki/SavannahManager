using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using _7dtd_svmanager_fix_mvvm.Views;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.Settings.Views
{
    /// <summary>
    /// KeyConfig.xaml の相互作用ロジック
    /// </summary>
    public partial class KeyConfig : Window
    {
        public KeyConfig(ShortcutKeyManager shortcutKeyManager)
        {
            InitializeComponent();

            var model = new Models.KeyConfigModel(shortcutKeyManager);
            var vm = new ViewModels.KeyConfigViewModel(new WindowService(this), model);
            DataContext = vm;
        }
        

        private void KeyList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
        }
    }
}
