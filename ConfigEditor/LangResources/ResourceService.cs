using System.ComponentModel;
using System.Globalization;

namespace ConfigEditor_mvvm.LangResources
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

        public static ResourceService Current { get; } = new ResourceService();

        #endregion

        /// <summary>
        /// 多言語化されたリソースを取得します。
        /// </summary>
        public CommonResources CommonResources { get; } = new CommonResources();

        public Resources Resources { get; } = new Resources();

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
            RaisePropertyChanged("CommonResources");
            RaisePropertyChanged("Resources");
        }

        public string GetCulture()
        {
            if (Resources.Culture != null)
            {
                return Resources.Culture.Name;
            }
            else
            {
                return CultureInfo.CurrentCulture.Name;
            }
        }
    }
}
