using System;
using System.Windows;
using _7dtd_svmanager_fix_mvvm.Views.PlayerController.Pages;

namespace _7dtd_svmanager_fix_mvvm.Views.PlayerController
{
    /// <summary>
    /// PlayerBase.xaml の相互作用ロジック
    /// </summary>
    public partial class PlayerBase : Window
    {
        public IPlayerPage Page { get; set; }

        public PlayerBase()
        {
            InitializeComponent();
        }

        public void AssignEnded()
        {
            Page.Ended += Page_Ended;
        }

        public void Navigate()
        {
            var navi = MainFrame.NavigationService;
            navi.Navigate(Page);
        }

        private void Page_Ended(object sender, EventArgs e)
        {
            Close();
        }
    }
}
