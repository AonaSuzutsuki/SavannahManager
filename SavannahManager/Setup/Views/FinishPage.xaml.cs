﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.Setup.Models;
using _7dtd_svmanager_fix_mvvm.Setup.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.Setup.Views
{
    /// <summary>
    /// FinishPage.xaml の相互作用ロジック
    /// </summary>
    public partial class FinishPage : UserControl
    {
        public FinishPage(NavigationWindowService<InitializeData> service)
        {
            InitializeComponent();

            DataContext = new FinishPageViewModel(service, new FinishPageModel(service.Share));
        }
    }
}
