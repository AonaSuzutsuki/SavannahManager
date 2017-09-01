using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.Models.Pages;
using _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages
{
    /// <summary>
    /// Ban.xaml の相互作用ロジック
    /// </summary>
    public partial class Ban : Page, IPlayerPage
    {
        #region EndedEvent
        public event EventHandler Ended;
        protected virtual void OnEnded(EventArgs e)
        {
            Ended?.Invoke(this, e);
        }
        #endregion
        
        public Ban(IMainWindowTelnet telnet, string playerID = "")
        {
            InitializeComponent();

            var model = new BanModel(telnet);
            var vm = new BanViewModel(model);
            DataContext = vm;

            model.Ended += Model_Ended;

            model.Name = playerID;
        }

        private void Model_Ended(object sender, EventArgs e)
        {
            OnEnded(e);
        }
    }
}
