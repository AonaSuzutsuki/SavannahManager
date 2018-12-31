using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.Setup.Views;
using CommonStyleLib.Models;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace _7dtd_svmanager_fix_mvvm.Setup.Models
{
    public class InitializeData
    {
        public string ServerFilePath { get; set; } = string.Empty;
        public string ServerConfigFilePath { get; set; } = string.Empty;
    }

    public class InitializeWindowModel : ModelBase
    {

        public InitializeWindowModel(SettingLoader settingLoader, NavigationService nav)
        {
            this.settingLoader = settingLoader;
            this.nav = nav;

            SharedInitializeData.ServerFilePath = settingLoader.ExeFilePath;
            SharedInitializeData.ServerConfigFilePath = settingLoader.ConfigFilePath;

            var page2 = new Page2(SharedInitializeData);
            page2.CanChenged += Page_CanChenged;
            var page3 = new Page3(SharedInitializeData);
            page3.CanChenged += Page_CanChenged;

            uriList = new List<Page>()
            {
                new Page1(),
                page2,
                page3,
                new Page4(),
            };

            Navigate(0);
        }

        private void Page_CanChenged(object sender, CanChangedEventArgs e)
        {
            NextBTEnabled = e.CanChanged;
        }

        #region PropertyForViewModel
        private bool prevBTEnabled = false;
        public bool PrevBTEnabled
        {
            get => prevBTEnabled;
            set => SetProperty(ref prevBTEnabled, value);
        }
        private bool nextBTEnabled = false;
        public bool NextBTEnabled
        {
            get => nextBTEnabled;
            set => SetProperty(ref nextBTEnabled, value);
        }
        private Visibility exitBTVisibility = Visibility.Collapsed;
        public Visibility ExitBTVisibility
        {
            get => exitBTVisibility;
            set => SetProperty(ref exitBTVisibility, value);
        }
        private Visibility cancelBTVisibility = Visibility.Visible;
        public Visibility CancelBTVisibility
        {
            get => cancelBTVisibility;
            set => SetProperty(ref cancelBTVisibility, value);
        }
        #endregion

        SettingLoader settingLoader;
        int currentPage = 0;
        NavigationService nav;
        List<Page> uriList;

        public InitializeData SharedInitializeData { get; set; } = new InitializeData();

        private void Navigate(int page)
        {
            if (uriList.Count >= page)
                nav.Navigate(uriList[page]);
            CheckPage(page);

            if (page == 1)
            {
                NextBTEnabled = false;
                if (!string.IsNullOrEmpty(SharedInitializeData.ServerFilePath))
                {
                    NextBTEnabled = true;
                }
            }
            else if (page == 2)
            {
                NextBTEnabled = false;
                if (!string.IsNullOrEmpty(SharedInitializeData.ServerConfigFilePath))
                {
                    NextBTEnabled = true;
                }
            }
        }
        private void CheckPage(int pageArg)
        {
            if (pageArg > 0)
            {
                PrevBTEnabled = true;
                CancelBTVisibility = Visibility.Collapsed;
                ExitBTVisibility = Visibility.Visible;
            }
            else
            {
                PrevBTEnabled = false;
                CancelBTVisibility = Visibility.Visible;
                ExitBTVisibility = Visibility.Collapsed;
            }

            if (uriList.Count - 1 > pageArg)
            {
                NextBTEnabled = true;
                CancelBTVisibility = Visibility.Visible;
                ExitBTVisibility = Visibility.Collapsed;
            }
            else
            {
                NextBTEnabled = false;
                CancelBTVisibility = Visibility.Collapsed;
                ExitBTVisibility = Visibility.Visible;
            }
        }

        public void NextPage()
        {
            int maxPage = uriList.Count - 1;
            if (maxPage >= currentPage + 1)
            {
                Navigate(++currentPage);
            }
        }
        public void PreviousPage()
        {
            if (0 <= currentPage - 1)
            {
                Navigate(--currentPage);
            }
        }

        public void Save()
        {
            settingLoader.ExeFilePath = SharedInitializeData.ServerFilePath;
            settingLoader.ConfigFilePath = SharedInitializeData.ServerConfigFilePath;
            settingLoader.IsFirstBoot = false;
            return;
        }
    }
}
