using CommonLib.Models;
using CommonLib.Extentions;
using KimamaLib.File;
using Prism.Mvvm;
using SvManagerLibrary.Config;
using SvManagerLibrary.XMLWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ConfigEditor_mvvm.Models
{
    public enum ConfigType
    {
        none,
        String,
        Integer,
        Combo
    }
    public class ConfigListInfo : BindableBase, ICloneable
    {
        public string Property { get; set; }
        private string value;
        public string Value
        {
            get => value;
            set => SetProperty(ref this.value, value);
        }
        public string[] Selection { get; set; }
        public ConfigType Type { get; set; }
        public string Description { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class MainWindowModel : ModelBase
    {
        #region Public Property
        private Visibility modifiedVisibility = Visibility.Hidden;
        /// <summary>
        /// タイトルの編集マークを表示するかどうか
        /// </summary>
        public Visibility ModifiedVisibility
        {
            get => modifiedVisibility;
            set => SetProperty(ref modifiedVisibility, value);
        }

        private bool saveBtEnabled;
        /// <summary>
        /// 保存ボタンを有効にするかどうか
        /// </summary>
        public bool SaveBtEnabled
        {
            get => saveBtEnabled;
            set => SetProperty(ref saveBtEnabled, value);
        }

        /// <summary>
        /// バージョンコンボボックスの本体
        /// </summary>
        public ObservableCollection<string> VersionList = new ObservableCollection<string>();
        private ObservableCollection<ConfigListInfo> configList = new ObservableCollection<ConfigListInfo>();
        /// <summary>
        /// コンフィグリストビューの本体及び管理データ用リスト
        /// </summary>
        public ObservableCollection<ConfigListInfo> ConfigList
        {
            get => configList;
            set => SetProperty(ref configList, value);
        }
        /// <summary>
        /// 値候補リストの本体
        /// </summary>
        public ObservableCollection<string> ValueList { get; set; } = new ObservableCollection<string>();

        private int versionListSelectedIndex;
        /// <summary>
        /// バージョンコンボボックスの選択されているインデックス値或いはその設定
        /// </summary>
        public int VersionListSelectedIndex
        {
            get => versionListSelectedIndex;
            set => SetProperty(ref versionListSelectedIndex, value);
        }
        private int configListSelectedIndex;
        /// <summary>
        /// コンフィグリストの選択されているインデックス値或いはその設定
        /// </summary>
        public int ConfigListSelectedIndex
        {
            get => configListSelectedIndex;
            set => SetProperty(ref configListSelectedIndex, value);
        }
        private int valueListSelectedIndex;
        /// <summary>
        /// 値候補リストの選択されているインデックス値或いはその設定
        /// </summary>
        public int ValueListSelectedIndex
        {
            get => valueListSelectedIndex;
            set => SetProperty(ref valueListSelectedIndex, value);
        }

        private string nameLabel;
        /// <summary>
        /// プロパティ名の表示値
        /// </summary>
        public string NameLabel
        {
            get => nameLabel;
            set => SetProperty(ref nameLabel, value);
        }
        private string descriptionLabel;
        /// <summary>
        /// 説明の表示値
        /// </summary>
        public string DescriptionLabel
        {
            get => descriptionLabel;
            set => SetProperty(ref descriptionLabel, value);
        }

        private string valueText;
        /// <summary>
        /// コンフィグの値を取得または設定
        /// </summary>
        public string ValueText
        {
            get => valueText;
            set => SetProperty(ref valueText, value);
        }

        private Visibility valueListVisibility = Visibility.Hidden;
        /// <summary>
        /// 値候補リストの表示するかどうか
        /// </summary>
        public Visibility ValueListVisibility
        {
            get => valueListVisibility;
            set => SetProperty(ref valueListVisibility, value);
        }
        private Visibility valueTextBoxVisibility = Visibility.Hidden;
        /// <summary>
        /// 値設定用テキストボックスを表示するかどうか
        /// </summary>
        public Visibility ValueTextBoxVisibility
        {
            get => valueTextBoxVisibility;
            set => SetProperty(ref valueTextBoxVisibility, value);
        }
        #endregion

        #region Properties
        private bool isModified = false;
        /// <summary>
        /// 編集されたかどうかを表します。またタイトルのマークの有無も同時に変更します。
        /// true: 編集済
        /// false: 無編集
        /// </summary>
        private bool IsModified
        {
            get => isModified;
            set
            {
                isModified = value;
                if (value)
                    ModifiedVisibility = Visibility.Visible;
                else
                    ModifiedVisibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Fields
        private SettingLoader settingLoader;
        private ConfigLoader configLoader;
        private TemplateLoader templateLoader;

        // ロード時のイベント回避
        private bool isSetConfig = false;
        #endregion

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            settingLoader = new SettingLoader(StaticData.SettingFilePath);

            var language = LangResources.CommonResources.Language;
            templateLoader = new TemplateLoader(language, StaticData.VersionListPath);
            VersionList.AddAll(templateLoader.VersionList);

            string[] cmds = Environment.GetCommandLineArgs();
            if (cmds.Length > 1)
                configLoader = new ConfigLoader(cmds[1]);

            // Select Version
            VersionListSelectedIndex = VersionList.Count - 1;
        }

        /// <summary>
        /// ショートカットキーの処理
        /// </summary>
        /// <param name="e">キーの入力値</param>
        /// <param name="modKey">特殊キーの入力値/param>
        public void ShortcutKey(KeyEventArgs e, ModifierKeys modKey)
        {
            Key mainKey = e.Key;

            if (modKey == ModifierKeys.Control && mainKey == Key.S)
            {
                Save();
            }
        }

        /// <summary>
        /// 新規ファイルを読み込む
        /// </summary>
        public void LoadNewData()
        {
            configLoader = null;
            LoadToConfigList();
        }
        /// <summary>
        /// 任意のファイルの選択とロード処理
        /// </summary>
        public void OpenFile()
        {
            var dirName = settingLoader.OpenDirectoryPath;
            var filePath = FileSelector.GetFilePath(dirName, 
                LangResources.CommonResources.Filter_XmlFile, StaticData.ServerConfigFileName, FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(filePath))
            {
                configLoader = new ConfigLoader(filePath);
                LoadToConfigList();
                settingLoader.OpenDirectoryPath = Path.GetDirectoryName(filePath);
            }
        }

        /// <summary>
        /// コンフィグのロードと描写
        /// </summary>
        public void LoadToConfigList()
        {
            if (VersionListSelectedIndex < 0) return;

            ConfigList.Clear();
            var version = VersionList[VersionListSelectedIndex];
            if (configLoader == null)
            {
                var list = new List<ConfigListInfo>(templateLoader.GetConfigList(version));
                ConfigList.AddAll(list);
                SaveBtEnabled = false;
            }
            else
            {
                var templateDic = templateLoader.GetConfigDictionary(version);
                var templateKeys = new List<string>(templateDic.Keys);
                var configDic = configLoader.GetAll();
                var keys = new List<string>(configDic.Keys);

                // コンフィグファイルとtemplateに含まれるプロパティだけ追加
                foreach (var configInfo in configDic.Values)
                {
                    var propertyName = configInfo.PropertyName;
                    if (templateDic.ContainsKey(propertyName))
                    {
                        var configListInfo = templateDic[propertyName];
                        configListInfo.Value = configInfo.Value;
                        ConfigList.Add(configListInfo);
                    }
                }

                // templateにあるがコンフィグファイルにないものを追加
                string[] nRepetitions = templateKeys.ToArray().ArrayExcept(keys.ToArray());
                foreach (string key in nRepetitions)
                {
                    if (templateDic.ContainsKey(key))
                    {
                        var configListInfo = templateDic[key];
                        ConfigList.Add(configListInfo);
                    }
                }

                SaveBtEnabled = true;
            }
        }

        /// <summary>
        /// 選択されているコンフィグの設定と描写
        /// </summary>
        public void SetConfigProperty()
        {
            if (ConfigListSelectedIndex < 0) return;
            var configListInfo = ConfigList[ConfigListSelectedIndex];

            isSetConfig = true;
            ValueListSelectedIndex = -1;
            ValueList.Clear();

            NameLabel = configListInfo.Property;
            DescriptionLabel = configListInfo.Description;
            var value = configListInfo.Value;
            ConfigType type = configListInfo.Type;
            if (type == ConfigType.Combo)
            {
                var selections = configListInfo.Selection;
                foreach (var item in selections.Select((v, i) => new { v, i }))
                {
                    ValueList.Add(item.v);
                    if (item.v.Equals(value))
                        ValueListSelectedIndex = item.i;
                }

                ValueListVisibility = Visibility.Visible;
                ValueTextBoxVisibility = Visibility.Hidden;
            }
            else
            {
                ValueText = value;
                ValueListVisibility = Visibility.Hidden;
                ValueTextBoxVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// コンフィグ値の変更
        /// </summary>
        /// <param name="confType"></param>
        public void ChangeValue(ConfigType confType)
        {
            if (isSetConfig)
            {
                isSetConfig = false;
                return;
            }

            string value = ValueText;
            if (confType == ConfigType.Combo)
            {
                if (ValueListSelectedIndex < 0) return;
                value = ValueList[ValueListSelectedIndex];
            }

            if (ConfigListSelectedIndex < 0) return;
            var index = ConfigListSelectedIndex;
            var configListInfo = ConfigList[index];
            configListInfo.Value = value;
            IsModified = true;
        }
        
        /// <summary>
        /// 名前をつけて保存の際のファイル選択とパスの取得
        /// </summary>
        /// <returns></returns>
        private bool SelectFileOnSaveAs()
        {
            var dirName = settingLoader.OpenDirectoryPath;
            var filePath = FileSelector.GetFilePath(dirName,
                LangResources.CommonResources.Filter_XmlFile, StaticData.ServerConfigFileName, FileSelector.FileSelectorType.Write);
            if (!string.IsNullOrEmpty(filePath))
            {
                configLoader = new ConfigLoader(filePath, true);
                SaveBtEnabled = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 名前をつけて保存
        /// </summary>
        public void SaveAs()
        {
            if (SelectFileOnSaveAs())
                Save();
        }
        /// <summary>
        /// 上書き保存、ファイルが選択されていない場合は名前をつけて保存
        /// </summary>
        public void Save()
        {
            if (configLoader == null)
            {
                if (!SelectFileOnSaveAs()) return;
            }

            configLoader.Clear();
            foreach (var configListInfo in ConfigList)
            {
                if (configListInfo.Property.Equals("SaveGameFolder") && string.IsNullOrEmpty(configListInfo.Value))
                    continue;
                configLoader.AddValue(configListInfo.Property, configListInfo.Value);
            }

            configLoader.Write();
            IsModified = false;
        }
    }
}
