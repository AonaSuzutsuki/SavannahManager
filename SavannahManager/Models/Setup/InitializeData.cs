namespace _7dtd_svmanager_fix_mvvm.Models.Setup
{
    public class InitializeData
    {
        public string ServerFilePath { get; set; } = string.Empty;
        public string ServerConfigFilePath { get; set; } = string.Empty;
        public string ServerAdminConfigFilePath { get; set; } = string.Empty;
        public SettingLoader Setting { get; set; }
    }
}