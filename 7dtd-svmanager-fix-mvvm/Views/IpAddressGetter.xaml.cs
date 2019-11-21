﻿using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.ViewModels;
using System;
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
using System.Windows.Shapes;
using CommonStyleLib.Views;

namespace _7dtd_svmanager_fix_mvvm.Views
{
    /// <summary>
    /// IpAddress.xaml の相互作用ロジック
    /// </summary>
    public partial class IpAddressGetter : Window
    {
        public IpAddressGetter()
        {
            InitializeComponent();

            var model = new IpAddressGetterModel();
            var vm = new IpAddressGetterViewModel(new WindowService(this), model);
            this.DataContext = vm;
        }
    }
}
