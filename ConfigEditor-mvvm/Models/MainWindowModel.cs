using CommonStyleLib.Models;
using CommonExtensionLib.Extensions;
using CommonStyleLib.File;
using SvManagerLibrary.Config;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ConfigEditor_mvvm.Models
{
    public class MainWindowModel : ModelBase
    {
        #region Public Property
        private Visibility modifiedVisibility = Visibility.Hidden;
        /// <summary>
        /// Set or get the presence or absence of the edit mark of the title.
        /// </summary>
        public Visibility ModifiedVisibility
        {
            get => modifiedVisibility;
            set => SetProperty(ref modifiedVisibility, value);
        }

        private bool saveBtEnabled;
        /// <summary>
        /// Set or get enable / disable of save button.
        /// </summary>
        public bool SaveBtEnabled
        {
            get => saveBtEnabled;
            set => SetProperty(ref saveBtEnabled, value);
        }

        private ObservableCollection<string> versionList;
        /// <summary>
        /// Set or get the body of the Version combo box.
        /// </summary>
        public ObservableCollection<string> VersionList
        {
            get => versionList;
            set => SetProperty(ref versionList, value);
        }
        private ObservableCollection<ConfigListInfo> configList;
        /// <summary>
        /// Set or get the body of the ConfigList and list for management.
        /// </summary>
        public ObservableCollection<ConfigListInfo> ConfigList
        {
            get => configList;
            set => SetProperty(ref configList, value);
        }
        private ObservableCollection<string> valueList;
        /// <summary>
        /// Set or get body of value candidate list.
        /// </summary>
        public ObservableCollection<string> ValueList
        {
            get => valueList;
            set => SetProperty(ref valueList, value);
        }

        private int versionListSelectedIndex;
        /// <summary>
        /// Set or get the selected index value of the Version combo box.
        /// </summary>
        public int VersionListSelectedIndex
        {
            get => versionListSelectedIndex;
            set => SetProperty(ref versionListSelectedIndex, value);
        }
        private int configListSelectedIndex;
        /// <summary>
        /// Set or get the selected index value of the ConfigList.
        /// </summary>
        public int ConfigListSelectedIndex
        {
            get => configListSelectedIndex;
            set => SetProperty(ref configListSelectedIndex, value);
        }
        private int valueListSelectedIndex;
        /// <summary>
        /// Set or get the selected index value of the Value candidate list.
        /// </summary>
        public int ValueListSelectedIndex
        {
            get => valueListSelectedIndex;
            set => SetProperty(ref valueListSelectedIndex, value);
        }

        private string nameLabel;
        /// <summary>
        /// Set or get displaied property name.
        /// </summary>
        public string NameLabel
        {
            get => nameLabel;
            set => SetProperty(ref nameLabel, value);
        }
        private string descriptionLabel;
        /// <summary>
        /// Set or get displaied description label.
        /// </summary>
        public string DescriptionLabel
        {
            get => descriptionLabel;
            set => SetProperty(ref descriptionLabel, value);
        }

        private string valueText;
        /// <summary>
        /// Set or get displaied config value.
        /// </summary>
        public string ValueText
        {
            get => valueText;
            set => SetProperty(ref valueText, value);
        }

        private Visibility valueListVisibility = Visibility.Hidden;
        /// <summary>
        /// Set or get whether to display value candidate list.
        /// </summary>
        public Visibility ValueListVisibility
        {
            get => valueListVisibility;
            set => SetProperty(ref valueListVisibility, value);
        }
        private Visibility valueTextBoxVisibility = Visibility.Hidden;
        /// <summary>
        /// Set or get whether to display text box for value setting.
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
        /// Set or get the state of editing. Also change the presence or absence of the title mark.
        /// true: Edited
        /// false: No editing
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

        // Event avoidance at loading
        private bool isSetConfig = false;
        #endregion

        /// <summary>
        /// Perform initialization processing.
        /// </summary>
        public MainWindowModel()
        {
            VersionList = new ObservableCollection<string>();
            ConfigList = new ObservableCollection<ConfigListInfo>();
            ValueList = new ObservableCollection<string>();

            settingLoader = new SettingLoader(ConstantValues.SettingFilePath);

            var language = LangResources.CommonResources.Language;
            templateLoader = new TemplateLoader(language, ConstantValues.VersionListPath);
            VersionList.AddAll(templateLoader.VersionList);

            var cmdArray = Environment.GetCommandLineArgs();
            if (cmdArray.Length > 1)
            {
                string filePath = cmdArray[1];
                if (File.Exists(filePath))
                    configLoader = new ConfigLoader(cmdArray[1]);
            }

            // Select Version
            VersionListSelectedIndex = VersionList.Count - 1;
            LoadToConfigList();
        }

        /// <summary>
        /// Process shortcut keys.
        /// </summary>
        /// <param name="e">input value of key</param>
        /// <param name="modKey">input value of modifier key/param>
        public void ShortcutKey(KeyEventArgs e, ModifierKeys modKey)
        {
            var mainKey = e.Key;

            if (modKey == ModifierKeys.Control && mainKey == Key.S)
            {
                Save();
            }
        }

        /// <summary>
        /// Load New Data.
        /// </summary>
        public void LoadNewData()
        {
            configLoader = null;
            LoadToConfigList();
        }
        /// <summary>
        /// Perform arbitrary file selection and load processing.
        /// </summary>
        public void OpenFile()
        {
            var dirName = settingLoader.OpenDirectoryPath;
            var filePath = FileSelector.GetFilePath(dirName, 
                LangResources.CommonResources.Filter_XmlFile, ConstantValues.ServerConfigFileName, FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(filePath))
            {
                configLoader = new ConfigLoader(filePath);
                LoadToConfigList();
                settingLoader.OpenDirectoryPath = Path.GetDirectoryName(filePath);
            }
        }

        /// <summary>
        /// Load and display config.
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

                //var templateKeysClone = new List<string>(templateKeys);
                //var keysClone = new List<string>(keys);

                //templateKeysClone.RemoveAll(keysClone.Contains);
                //keysClone.RemoveAll(templateKeys.Contains);

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
        /// Configure and display the selected config.
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
        /// Change config value.
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
        /// Get file selection and path for SaveAs.
        /// </summary>
        /// <returns></returns>
        private bool SelectFileOnSaveAs()
        {
            var dirName = settingLoader.OpenDirectoryPath;
            var filePath = FileSelector.GetFilePath(dirName,
                LangResources.CommonResources.Filter_XmlFile, ConstantValues.ServerConfigFileName, FileSelector.FileSelectorType.Write);
            if (!string.IsNullOrEmpty(filePath))
            {
                configLoader = new ConfigLoader(filePath, true);
                SaveBtEnabled = true;
                return true;
            }
            return false;
        }
        /// <summary>
        /// SaveAs
        /// </summary>
        public void SaveAs()
        {
            if (SelectFileOnSaveAs())
                Save();
        }
        /// <summary>
        /// Overwrite save. If no file is selected, do SaveAs.
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
