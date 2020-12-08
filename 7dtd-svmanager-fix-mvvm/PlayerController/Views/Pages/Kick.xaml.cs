using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.Models.Pages;
using _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels.Pages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages
{
    /// <summary>
    /// Kick.xaml の相互作用ロジック
    /// </summary>
    public partial class Kick : Page, IPlayerPage
    {
        #region EndedEvent
        public event EventHandler Ended;
        protected virtual void OnEnded(EventArgs e)
        {
            Ended?.Invoke(this, e);
        }
        #endregion

        public Kick(IMainWindowTelnet telnet, string playerId = "")
        {
            InitializeComponent();

            var model = new KickModel(telnet)
            {
                Name = playerId
            };
            model.Ended += Model_Ended;

            var vm = new KickViewModel(model);
            DataContext = vm;
        }

        private void Model_Ended(object sender, EventArgs e)
        {
            OnEnded(e);
        }
    }
}
