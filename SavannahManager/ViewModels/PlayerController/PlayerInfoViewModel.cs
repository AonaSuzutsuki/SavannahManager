using System.Diagnostics;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.PlayerController;
using CommonStyleLib.ExMessageBox;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using SvManagerLibrary.Player;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.PlayerController
{
    public class PlayerInfoViewModel : ViewModelBase
    {
        public PlayerInfoViewModel(WindowService service, PlayerInfoModel model) : base(service, model)
        {
            _model = model;

            OpenSteamIdCommand = new DelegateCommand(SteamId_MouseDown);
        }

        #region Fields

        private readonly PlayerInfoModel _model;

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
                Process.Start(new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = _model.UserPageLink
                });
        }

        #endregion

        #region Methods

        public void SetPlayer(PlayerInfo player)
        {
            Player = player;
            _model.SetUserPageLink(player.SteamId);
        }

        #endregion
    }
}
