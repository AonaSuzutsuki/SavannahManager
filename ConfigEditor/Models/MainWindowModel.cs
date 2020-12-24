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
        private Visibility _modifiedVisibility = Visibility.Hidden;
        /// <summary>
        /// Set or get the presence or absence of the edit mark of the title.
        /// </summary>
        public Visibility ModifiedVisibility
        {
            get => _modifiedVisibility;
            set => SetProperty(ref _modifiedVisibility, value);
        }

        private bool _saveBtEnabled;
        /// <summary>
        /// Set or get enable / disable of save button.
        /// </summary>
        public bool SaveBtEnabled
        {
            get => _saveBtEnabled;
            set => SetProperty(ref _saveBtEnabled, value);
        }

        private ObservableCollection<string> _versionList;
        /// <summary>
        /// Set or get the body of the Version combo box.
        /// </summary>
        public ObservableCollection<string> VersionList
        {
            get => _versionList;
            set => SetProperty(ref _versionList, value);
        }
        private ObservableCollection<ConfigListInfo> _configList;
        /// <summary>
        /// Set or get the body of the ConfigList and list for management.
        /// </summary>
        public ObservableCollection<ConfigListInfo> ConfigList
        {
            get => _configList;
            set => SetProperty(ref _configList, value);
        }
        private ObservableCollection<string> _valueList;
        /// <summary>
        /// Set or get body of value candidate list.
        /// </summary>
        public ObservableCollection<string> ValueList
        {
            get => _valueList;
            set => SetProperty(ref _valueList, value);
        }

        private int _versionListSelectedIndex;
        /// <summary>
        /// Set or get the selected index value of the Version combo box.
        /// </summary>
        public int VersionListSelectedIndex
        {
            get => _versionListSelectedIndex;
            set => SetProperty(ref _versionListSelectedIndex, value);
        }
        private int _configListSelectedIndex;
        /// <summary>
        /// Set or get the selected index value of the ConfigList.
        /// </summary>
        public int ConfigListSelectedIndex
        {
            get => _configListSelectedIndex;
            set => SetProperty(ref _configListSelectedIndex, value);
        }
        private int _valueListSelectedIndex;
        /// <summary>
        /// Set or get the selected index value of the Value candidate list.
        /// </summary>
        public int ValueListSelectedIndex
        {
            get => _valueListSelectedIndex;
            set => SetProperty(ref _valueListSelectedIndex, value);
        }

        private string _nameLabel;
        /// <summary>
        /// Set or get displaied property name.
        /// </summary>
        public string NameLabel
        {
            get => _nameLabel;
            set => SetProperty(ref _nameLabel, value);
        }
        private string _descriptionLabel;
        /// <summary>
        /// Set or get displaied description label.
        /// </summary>
        public string DescriptionLabel
        {
            get => _descriptionLabel;
            set => SetProperty(ref _descriptionLabel, value);
        }

        private string _valueText;
        /// <summary>
        /// Set or get displaied config value.
        /// </summary>
        public string ValueText
        {
            get => _valueText;
            set => SetProperty(ref _valueText, value);
        }

        private Visibility _valueListVisibility = Visibility.Hidden;
        /// <summary>
        /// Set or get whether to display value candidate list.
        /// </summary>
        public Visibility ValueListVisibility
        {
            get => _valueListVisibility;
            set => SetProperty(ref _valueListVisibility, value);
        }
        private Visibility _valueTextBoxVisibility = Visibility.Hidden;
        /// <summary>
        /// Set or get whether to display text box for value setting.
        /// </summary>
        public Visibility ValueTextBoxVisibility
        {
            get => _valueTextBoxVisibility;
            set => SetProperty(ref _valueTextBoxVisibility, value);
        }
        #endregion

        #region Properties
        private bool _isModified;
        /// <summary>
        /// Set or get the state of editing. Also change the presence or absence of the title mark.
        /// true: Edited
        /// false: No editing
        /// </summary>
        private bool IsModified
        {
            get => _isModified;
            set
            {
                _isModified = value;
                if (value)
                    ModifiedVisibility = Visibility.Visible;
                else
                    ModifiedVisibility = Visibility.Hidden;
            }
        }
        #endregion

        #region Fields
        private readonly SettingLoader _settingLoader;
        private ConfigLoader _configLoader;
        private readonly TemplateLoader _templateLoader;

        // Event avoidance at loading
        private bool _isSetConfig;
        #endregion

        /// <summary>
        /// Perform initialization processing.
        /// </summary>
        public MainWindowModel()
        {
            VersionList = new ObservableCollection<string>();
            ConfigList = new ObservableCollection<ConfigListInfo>();
            ValueList = new ObservableCollection<string>();

            _settingLoader = new SettingLoader(ConstantValues.SettingFilePath);

            var language = LangResources.CommonResources.Language;
            _templateLoader = new TemplateLoader(language, ConstantValues.VersionListPath);
            VersionList.AddAll(_templateLoader.VersionList);

            var cmdArray = Environment.GetCommandLineArgs();
            if (cmdArray.Length > 1)
            {
                var filePath = cmdArray[1];
                if (File.Exists(filePath))
                    _configLoader = new ConfigLoader(cmdArray[1]);
            }

            // Select Version
            VersionListSelectedIndex = VersionList.Count - 1;
            LoadToConfigList();
        }

        /// <summary>
        /// Process shortcut keys.
        /// </summary>
        /// <param name="e">input value of key</param>
        /// <param name="modKey">input value of modifier key</param>
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
            _configLoader = null;
            LoadToConfigList();
        }
        /// <summary>
        /// Perform arbitrary file selection and load processing.
        /// </summary>
        public void OpenFile()
        {
            var dirName = _settingLoader.OpenDirectoryPath;
            var filePath = FileSelector.GetFilePath(dirName, 
                LangResources.CommonResources.Filter_XmlFile, ConstantValues.ServerConfigFileName, FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(filePath))
            {
                _configLoader = new ConfigLoader(filePath);
                LoadToConfigList();
                _settingLoader.OpenDirectoryPath = Path.GetDirectoryName(filePath);
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
            if (_configLoader == null)
            {
                var list = new List<ConfigListInfo>(_templateLoader.GetConfigList(version));
                ConfigList.AddAll(list);
                SaveBtEnabled = false;
            }
            else
            {
                var templateDic = _templateLoader.GetConfigDictionary(version);
                var templateKeys = new List<string>(templateDic.Keys);
                var configDic = _configLoader.GetAll();
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

            _isSetConfig = true;
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
            if (_isSetConfig)
            {
                _isSetConfig = false;
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
            var dirName = _settingLoader.OpenDirectoryPath;
            var filePath = FileSelector.GetFilePath(dirName,
                LangResources.CommonResources.Filter_XmlFile, ConstantValues.ServerConfigFileName, FileSelector.FileSelectorType.Write);
            if (!string.IsNullOrEmpty(filePath))
            {
                _configLoader = new ConfigLoader(filePath, true);
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
            if (_configLoader == null)
            {
                if (!SelectFileOnSaveAs()) return;
            }

            _configLoader.Clear();
            foreach (var configListInfo in ConfigList)
            {
                if (configListInfo.Property.Equals("SaveGameFolder") && string.IsNullOrEmpty(configListInfo.Value))
                    continue;
                _configLoader.AddProperty(configListInfo.Property, configListInfo.Value);
            }

            _configLoader.Write();
            IsModified = false;
        }
    }
}
