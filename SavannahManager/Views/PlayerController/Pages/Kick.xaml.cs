using System;
using System.Windows.Controls;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController.Pages;
using _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController.Pages;

namespace _7dtd_svmanager_fix_mvvm.Views.PlayerController.Pages
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

        public Kick(KickViewModel kickViewModel, KickModel kickModel)
        {
            InitializeComponent();

            kickModel.Ended += Model_Ended;
            DataContext = kickViewModel;
        }

        private void Model_Ended(object sender, EventArgs e)
        {
            OnEnded(e);
        }
    }
}
