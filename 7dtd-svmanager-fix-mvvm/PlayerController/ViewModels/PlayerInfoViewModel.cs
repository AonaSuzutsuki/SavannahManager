using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.PlayerController.Models;
using _7dtd_svmanager_fix_mvvm.ViewModels;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using SvManagerLibrary.Player;

namespace _7dtd_svmanager_fix_mvvm.PlayerController.ViewModels
{
    public class PlayerInfoViewModel : ViewModelBase
    {
        public PlayerInfoViewModel(WindowService service, PlayerInfoModel model) : base(service, model)
        {
            this.model = model;

            OpenSteamIdCommand = new DelegateCommand(SteamId_MouseDown);
        }

        #region Fields

        private readonly PlayerInfoModel model;

        #endregion

        #region Properties

        public PlayerInfo Player { get; private set; }

        #endregion

        #region Event Properties

        public ICommand OpenSteamIdCommand { get; set; }

        #endregion

        #region Event Methods

        public void SteamId_MouseDown()
        {
            var dialogResult = ExMessageBoxBase.Show("Are you sure open steam profile with default browser?", "Open Browser", ExMessageBoxBase.MessageType.Question,
                ExMessageBoxBase.ButtonType.YesNo);
            if (dialogResult == ExMessageBoxBase.DialogResult.Yes)
                Process.Start(model.UserPageLink);
        }

        #endregion

        #region Methods

        public void SetPlayer(PlayerInfo player)
        {
            Player = player;
            model.SetUserPageLink(player.SteamId);
        }

        #endregion
    }
}
