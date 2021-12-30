namespace _7dtd_svmanager_fix_mvvm.Models.Setup
{
    public class FinishPageModel : PageModelBase
    {
        #region Fields

        #endregion

        public FinishPageModel(InitializeData initializeData) : base(initializeData)
        {
        }

        public void ApplySetting()
        {
            var setting = InitializeData.Setting;
            setting.ConfigFilePath = InitializeData.ServerConfigFilePath;
            setting.ExeFilePath = InitializeData.ServerFilePath;
            setting.AdminFilePath = InitializeData.ServerAdminConfigFilePath;
            setting.IsFirstBoot = false;
        }
    }
}
