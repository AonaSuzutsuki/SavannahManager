﻿using _7dtd_svmanager_fix_mvvm.Models;
using _7dtd_svmanager_fix_mvvm.PlayerController.Models.Pages;
using _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels.Pages;
using CommonLib.ExMessageBox;
using SvManagerLibrary.Telnet;
using System;
using System.Windows;
using System.Windows.Controls;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.Views.Pages
{
    public class AddType
    {
        public enum Type
        {
            Admin,
            Whitelist,
        }

        private Type type;
        public AddType(Type type)
        {
            this.type = type;
        }

        public string ToCommand()
        {
            string command = default;
            switch (type)
            {
                case Type.Admin:
                    command = "admin";
                    break;
                case Type.Whitelist:
                    command = "whitelist";
                    break;
            }

            return command;
        }
    }
    

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

        public AdminAdd(IMainWindowTelnet telnet, AddType.Type addType, string playerID = "")
        {
            InitializeComponent();

            var model = new AdminAddModel(telnet, new AddType(addType))
            {
                Name = playerID
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
