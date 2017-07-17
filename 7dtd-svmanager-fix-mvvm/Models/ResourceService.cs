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

        private readonly _7dtd_svmanager_fix_mvvm.LangResources.CommonResources commonResources = new _7dtd_svmanager_fix_mvvm.LangResources.CommonResources();
        private readonly _7dtd_svmanager_fix_mvvm.LangResources.Resources resources = new _7dtd_svmanager_fix_mvvm.LangResources.Resources();
        private readonly _7dtd_svmanager_fix_mvvm.LangResources.CmdListResources commandListResources = new _7dtd_svmanager_fix_mvvm.LangResources.CmdListResources();
        private readonly _7dtd_svmanager_fix_mvvm.LangResources.SettingsResources settingsResources = new _7dtd_svmanager_fix_mvvm.LangResources.SettingsResources();
        private readonly _7dtd_svmanager_fix_mvvm.LangResources.ForceShutdownerResources forceShutdownerResources = new _7dtd_svmanager_fix_mvvm.LangResources.ForceShutdownerResources();
        private readonly _7dtd_svmanager_fix_mvvm.LangResources.UpdResources updResources = new _7dtd_svmanager_fix_mvvm.LangResources.UpdResources();
        private readonly _7dtd_svmanager_fix_mvvm.LangResources.PlayerResources playerResources = new _7dtd_svmanager_fix_mvvm.LangResources.PlayerResources();
        private readonly _7dtd_svmanager_fix_mvvm.LangResources.SetupResource setupResources = new _7dtd_svmanager_fix_mvvm.LangResources.SetupResource();

        /// <summary>
        /// 多言語化されたリソースを取得します。
        /// </summary>
        public _7dtd_svmanager_fix_mvvm.LangResources.CommonResources CommonResources
        {
            get { return this.commonResources; }
        }
        public _7dtd_svmanager_fix_mvvm.LangResources.Resources Resources
        {
            get { return this.resources; }
        }
        public _7dtd_svmanager_fix_mvvm.LangResources.CmdListResources CmdListResources
        {
            get { return this.commandListResources; }
        }
        public _7dtd_svmanager_fix_mvvm.LangResources.SettingsResources SettingsResources
        {
            get { return this.settingsResources; }
        }
        public _7dtd_svmanager_fix_mvvm.LangResources.ForceShutdownerResources ForceShutdownerResources
        {
            get { return this.forceShutdownerResources; }
        }
        public _7dtd_svmanager_fix_mvvm.LangResources.UpdResources UpdResources
        {
            get { return this.updResources; }
        }
        public _7dtd_svmanager_fix_mvvm.LangResources.PlayerResources PlayerResources
        {
            get { return this.playerResources; }
        }
        public _7dtd_svmanager_fix_mvvm.LangResources.SetupResource SetupResources
        {
            get { return this.setupResources; }
        }


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
            _7dtd_svmanager_fix_mvvm.LangResources.CommonResources.Culture = cultureInfo;
            _7dtd_svmanager_fix_mvvm.LangResources.Resources.Culture = cultureInfo;
            _7dtd_svmanager_fix_mvvm.LangResources.CmdListResources.Culture = cultureInfo;
            _7dtd_svmanager_fix_mvvm.LangResources.SettingsResources.Culture = cultureInfo;
            _7dtd_svmanager_fix_mvvm.LangResources.ForceShutdownerResources.Culture = cultureInfo;
            _7dtd_svmanager_fix_mvvm.LangResources.UpdResources.Culture = cultureInfo;
            _7dtd_svmanager_fix_mvvm.LangResources.PlayerResources.Culture = cultureInfo;
            _7dtd_svmanager_fix_mvvm.LangResources.SetupResource.Culture = cultureInfo;
            RaisePropertyChanged("CommonResources");
            RaisePropertyChanged("Resources");
            RaisePropertyChanged("CmdListResources");
            RaisePropertyChanged("SettingsResources");
            RaisePropertyChanged("ForceShutdownerResources");
            RaisePropertyChanged("UpdResources");
            RaisePropertyChanged("PlayerResources");
            RaisePropertyChanged("SetupResources");
        }

        public string GetCulture()
        {
            if (_7dtd_svmanager_fix_mvvm.LangResources.Resources.Culture != null)
            {
                return _7dtd_svmanager_fix_mvvm.LangResources.Resources.Culture.Name;
            }
            else
            {
                return CultureInfo.CurrentCulture.Name;
            }
        }
    }
}
