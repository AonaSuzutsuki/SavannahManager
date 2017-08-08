using CommonLib.Models;
using Prism.Mvvm;
using SvManagerLibrary.Config;
using SvManagerLibrary.XMLWrapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ConfigListInfo : BindableBase
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
    }

    public class MainWindowModel : ModelBase
    {
        #region Public Property
        private Visibility modifiedVisibility = Visibility.Hidden;
        public Visibility ModifiedVisibility
        {
            get => modifiedVisibility;
            set => SetProperty(ref modifiedVisibility, value);
        }

        public ObservableCollection<string> VersionList = new ObservableCollection<string>();
        private ObservableCollection<ConfigListInfo> configList = new ObservableCollection<ConfigListInfo>();
        public ObservableCollection<ConfigListInfo> ConfigList
        {
            get => configList;
            set => SetProperty(ref configList, value);
        }
        public ObservableCollection<string> ValueList { get; set; } = new ObservableCollection<string>();

        private int versionListSelectedIndex;
        public int VersionListSelectedIndex
        {
            get => versionListSelectedIndex;
            set => SetProperty(ref versionListSelectedIndex, value);
        }
        private int configListSelectedIndex;
        public int ConfigListSelectedIndex
        {
            get => configListSelectedIndex;
            set => SetProperty(ref configListSelectedIndex, value);
        }
        private string valueListSelectedItem;
        public string ValueListSelectedItem
        {
            get => valueListSelectedItem;
            set => SetProperty(ref valueListSelectedItem, value);
        }
        private int valueListSelectedIndex;
        public int ValueListSelectedIndex
        {
            get => valueListSelectedIndex;
            set => SetProperty(ref valueListSelectedIndex, value);
        }

        private string nameLabel;
        public string NameLabel
        {
            get => nameLabel;
            set => SetProperty(ref nameLabel, value);
        }
        private string descriptionLabel;
        public string DescriptionLabel
        {
            get => descriptionLabel;
            set => SetProperty(ref descriptionLabel, value);
        }
        private string valueText;
        public string ValueText
        {
            get => valueText;
            set => SetProperty(ref valueText, value);
        }
        private Visibility valueListVisibility = Visibility.Hidden;
        public Visibility ValueListVisibility
        {
            get => valueListVisibility;
            set => SetProperty(ref valueListVisibility, value);
        }
        private Visibility valueTextBoxVisibility = Visibility.Hidden;
        public Visibility ValueTextBoxVisibility
        {
            get => valueTextBoxVisibility;
            set => SetProperty(ref valueTextBoxVisibility, value);
        }
        #endregion

        #region Properties
        private bool isModified = false;
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
        private ConfigLoader configLoader;
        private TemplateLoader templateLoader;

        // ロード時のイベント回避
        private bool isSetConfig = false;
        #endregion

        public void Initialize()
        {
            var language = LangResources.CommonResources.Language;
            templateLoader = new TemplateLoader(language, StaticData.VersionListPath);
            VersionList.AddAll(templateLoader.VersionList);

            string[] cmds = Environment.GetCommandLineArgs();
            if (cmds.Length > 1)
                configLoader = new ConfigLoader(cmds[1]);

            // Select Version
            VersionListSelectedIndex = VersionList.Count - 1;
        }

        public void ShortcutKey(KeyEventArgs e)
        {
            ModifierKeys modKey = Keyboard.Modifiers;
            Key mainKey = e.Key;

            if (modKey == ModifierKeys.Control && mainKey == Key.S)
            {
                Save();
            }
        }

        public void LoadToConfigList()
        {
            if (VersionListSelectedIndex < 0) return;

            ConfigList.Clear();
            var version = VersionList[VersionListSelectedIndex];
            if (configLoader == null)
            {
                var list = templateLoader.GetConfigList(version);
                ConfigList.AddAll(list);
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
                string[] nRepetitions = StringExceptWith(templateKeys.ToArray(), keys.ToArray());
                foreach (string key in nRepetitions)
                {
                    if (templateDic.ContainsKey(key))
                    {
                        var configListInfo = templateDic[key];
                        ConfigList.Add(configListInfo);
                    }
                }
            }
        }
        private string[] StringExceptWith(string[] ary1, string[] ary2)
        {
            //HashSetに変換する
            HashSet<string> hs1 = new HashSet<string>(ary1);
            HashSet<string> hs2 = new HashSet<string>(ary2);

            // h1にのみ存在する要素を取得
            var query1 = new HashSet<string>(hs1.Intersect(hs2));
            var query2 = new HashSet<string>(hs1.Except(query1));

            //配列に変換する
            string[] resultArray = new string[query2.Count];
            query2.CopyTo(resultArray, 0);
            return resultArray;
        }

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

        public void Save()
        {
            configLoader.Clear();
            foreach (var configListInfo in ConfigList)
            {
                if (configListInfo.Property.Equals("SaveGameFolder") && string.IsNullOrEmpty(configListInfo.Value))
                    continue;
                configLoader.AddValue(configListInfo.Property, configListInfo.Value);
            }

            configLoader.Write();
        }
    }
}
