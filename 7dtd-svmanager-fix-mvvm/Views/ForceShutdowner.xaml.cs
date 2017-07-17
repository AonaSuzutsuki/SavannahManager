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
using System.Windows.Shapes;

namespace _7dtd_svmanager_fix_mvvm.Views
{
    /// <summary>
    /// ForceShutdowner.xaml の相互作用ロジック
    /// </summary>
    public partial class ForceShutdowner : Window
    {
        public ForceShutdowner()
        {
            InitializeComponent();

            var model = new Models.ForceShutdownerModel();
            var vm = new ViewModels.ForceShutdownerViewModel(this, model);
            DataContext = vm;
        }
    }
}
