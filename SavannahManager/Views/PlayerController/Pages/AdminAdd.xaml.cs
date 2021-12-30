using System;
using System.Windows.Controls;
using _7dtd_svmanager_fix_mvvm.Models.Interfaces;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController.Pages;
using _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController.Pages;

namespace _7dtd_svmanager_fix_mvvm.Views.PlayerController.Pages
{
    /// <summary>
    /// Add.xaml の相互作用ロジック
    /// </summary>
    public partial class AdminAdd : Page, IPlayerPage
    {
        #region EndedEvent
        public event EventHandler Ended;
        protected virtual void OnEnded(EventArgs e)
        {
            Ended?.Invoke(this, e);
        }
        #endregion

        #region Fiels
        #endregion

        public AdminAdd(IMainWindowTelnet telnet, AddType.Type addType, string playerId = "")
        {
            InitializeComponent();

            var model = new AdminAddModel(telnet, new AddType(addType))
            {
                Name = playerId
            };
            model.Ended += Model_Ended;

            var vm = new AdminAddViewModel(model);
            DataContext = vm;
        }

        private void Model_Ended(object sender, EventArgs e)
        {
            OnEnded(e);
        }
    }
}
