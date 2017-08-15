using _7dtd_svmanager_fix_mvvm.LangResources;
using System.ComponentModel;
using System.Globalization;

namespace LanguageEx
{
    /// <summary>
    /// 多言語化されたリソースと、言語の切り替え機能を提供します。
    /// </summary>
    public class ResourceService : INotifyPropertyChanged
    {
        #region Static Members
        public const string Japanese = "ja";
        public const string English = "en-US";
        #endregion

        #region singleton members

        private static readonly ResourceService current = new ResourceService();
        public static ResourceService Current
        {
            get { return current; }
        }

        #endregion
        
        /// <summary>
        /// 多言語化されたリソースを取得します。
        /// </summary>
        public CommonResources CommonResources { get; } = new CommonResources();
        public Resources Resources { get; } = new Resources();
        public CmdListResources CmdListResources { get; } = new CmdListResources();
        public SettingsResources SettingsResources { get; } = new SettingsResources();
        public ForceShutdownerResources ForceShutdownerResources { get; } = new ForceShutdownerResources();
        public UpdResources UpdResources { get; } = new UpdResources();
        public PlayerResources PlayerResources { get; } = new PlayerResources();
        public SetupResource SetupResources { get; } = new SetupResource();
        public VersionInfoResources VersionInfoResources { get; } = new VersionInfoResources();


        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// 指定されたカルチャ名を使用して、リソースのカルチャを変更します。
        /// </summary>
        /// <param name="name">カルチャの名前。</param>
        public void ChangeCulture(string name)
        {
            var cultureInfo = CultureInfo.GetCultureInfo(name);
            CommonResources.Culture = cultureInfo;
            Resources.Culture = cultureInfo;
            CmdListResources.Culture = cultureInfo;
            SettingsResources.Culture = cultureInfo;
            ForceShutdownerResources.Culture = cultureInfo;
            UpdResources.Culture = cultureInfo;
            PlayerResources.Culture = cultureInfo;
            SetupResource.Culture = cultureInfo;
            VersionInfoResources.Culture = cultureInfo;
            RaisePropertyChanged("CommonResources");
            RaisePropertyChanged("Resources");
            RaisePropertyChanged("CmdListResources");
            RaisePropertyChanged("SettingsResources");
            RaisePropertyChanged("ForceShutdownerResources");
            RaisePropertyChanged("UpdResources");
            RaisePropertyChanged("PlayerResources");
            RaisePropertyChanged("SetupResources");
            RaisePropertyChanged("VersionInfoResources");
        }

        public string Culture
        {
            get
            {
                if (Resources.Culture != null)
                    return Resources.Culture.Name;
                else
                    return CultureInfo.CurrentCulture.Name;
            }
        }
    }
}
