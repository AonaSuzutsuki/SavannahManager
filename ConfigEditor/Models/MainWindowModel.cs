﻿using CommonStyleLib.Models;
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
using SavannahManagerStyleLib.ViewModels.SshFileSelector;

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

        public SettingLoader SettingLoader { get; }

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

        public string FilePath { get; private set; }
        #endregion

        #region Fields
        //private ConfigLoader _configLoader;
        private readonly TemplateLoader _templateLoader;

        // Event avoidance at loading
        private bool _isSetConfig;

        private List<ConfigListInfo> _baseConfigListInfo = new();
        #endregion

        /// <summary>
        /// Perform initialization processing.
        /// </summary>
        public MainWindowModel()
        {
            VersionList = new ObservableCollection<string>();
            ConfigList = new ObservableCollection<ConfigListInfo>();
            ValueList = new ObservableCollection<string>();

            SettingLoader = new SettingLoader(ConstantValues.SettingFilePath);

            var language = LangResources.CommonResources.Language;
            _templateLoader = new TemplateLoader(language, ConstantValues.VersionListPath);
            VersionList.AddRange(_templateLoader.VersionList);

            // Select Version
            VersionListSelectedIndex = VersionList.Count - 1;

            var cmdArray = Environment.GetCommandLineArgs();
            if (cmdArray.Length > 1)
            {
                var filePath = cmdArray[1];
                if (File.Exists(filePath))
                {
                    var configLoader = new ConfigLoader(cmdArray[1]);
                    FilePath = filePath;
                    
                    LoadToConfigList(configLoader);
                    SaveBtEnabled = true;

                    NarrowDownConfig("");
                    return;
                }
            }

            LoadNewConfig();
            NarrowDownConfig("");
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
            LoadNewConfig();
            FilePath = null;
            ResetButtons();
        }

        /// <summary>
        /// Perform arbitrary file selection and load processing.
        /// </summary>
        public void OpenFile()
        {
            var dirName = SettingLoader.OpenDirectoryPath;
            var filePath = FileSelector.GetFilePath(dirName, 
                LangResources.CommonResources.Filter_XmlFile, ConstantValues.ServerConfigFileName, FileSelector.FileSelectorType.Read);
            if (!string.IsNullOrEmpty(filePath))
            {
                FilePath = filePath;

                var loader = new ConfigLoader(filePath);
                LoadToConfigList(loader);
                SettingLoader.OpenDirectoryPath = Path.GetDirectoryName(filePath);
                IsModified = false;
                SaveBtEnabled = true;
            }
        }

        public void NarrowDownConfig(string searchWord)
        {
            if (string.IsNullOrEmpty(searchWord))
            {
                ConfigList.Clear();
                ConfigList.AddRange(_baseConfigListInfo);
                return;
            }

            var narrowDownItems = _baseConfigListInfo.Where(x => x.Property.Contains(searchWord));

            ConfigList.Clear();
            ConfigList.AddRange(narrowDownItems);
        }

        private void LoadNewConfig()
        {
            if (VersionListSelectedIndex < 0)
                return;

            _baseConfigListInfo = new();

            var version = VersionList[VersionListSelectedIndex];
            var list = new List<ConfigListInfo>(_templateLoader.GetConfigList(version));
            _baseConfigListInfo.AddRange(list);
        }

        public void VersionListSelectionChanged()
        {
            if (FilePath == null)
            {
                LoadNewConfig();
            }
            else
            {
                var loader = new ConfigLoader(FilePath);
                LoadToConfigList(loader);
            }
        }

        private void ResetButtons()
        {
            IsModified = false;
            SaveBtEnabled = false;
        }

        /// <summary>
        /// Load and display config.
        /// </summary>
        private void LoadToConfigList(ConfigLoader loader)
        {
            if (VersionListSelectedIndex < 0) return;

            var baseList = new List<ConfigListInfo>();

            var version = VersionList[VersionListSelectedIndex];

            var templateDic = _templateLoader.GetConfigDictionary(version);
            var templateKeys = new List<string>(templateDic.Keys);
            var configDic = loader.GetAll();
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
                    baseList.Add(configListInfo);
                }
            }

            // templateにあるがコンフィグファイルにないものを追加
            var nRepetitions = templateKeys.ToArray().Except(keys.ToArray());
            foreach (string key in nRepetitions)
            {
                if (templateDic.ContainsKey(key))
                {
                    var configListInfo = templateDic[key];
                    baseList.Add(configListInfo);
                }
            }

            _baseConfigListInfo = baseList;
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
        private (bool result, string filePath) SelectFileOnSaveAs()
        {
            var dirName = SettingLoader.OpenDirectoryPath;
            var filePath = FileSelector.GetFilePath(dirName,
                LangResources.CommonResources.Filter_XmlFile, ConstantValues.ServerConfigFileName, FileSelector.FileSelectorType.Write);
            if (!string.IsNullOrEmpty(filePath))
            {
                SaveBtEnabled = true;
                return (true, filePath);
            }
            return (false, null);
        }
        /// <summary>
        /// SaveAs
        /// </summary>
        public void SaveAs()
        {
            var (result, filePath) = SelectFileOnSaveAs();
            if (result)
            {
                FilePath = filePath;
                Save();
            }
        }
        /// <summary>
        /// Overwrite save. If no file is selected, do SaveAs.
        /// </summary>
        public void Save()
        {
            if (FilePath == null)
            {
                var (result, filePath) = SelectFileOnSaveAs();
                if (!result)
                    return;

                FilePath = filePath;
            }

            using var fs = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

            var loader = CreateConfigLoader();

            loader.Write(fs);
            IsModified = false;
        }

        private ConfigLoader CreateConfigLoader()
        {
            var loader = new ConfigLoader();
            foreach (var configListInfo in _baseConfigListInfo)
            {
                if (configListInfo.Property.Equals("SaveGameFolder") && string.IsNullOrEmpty(configListInfo.Value))
                    continue;
                loader.AddProperty(configListInfo.Property, configListInfo.Value);
            }

            return loader;
        }

        public ConnectionInformation CreateConnectionInformation()
        {
            return new ConnectionInformation
            {
                Address = SettingLoader.SftpAddress,
                Port = SettingLoader.SftpPort,
                IsPassword = SettingLoader.SftpIsPassword,
                Username = SettingLoader.SftpUserName,
                KeyPath = SettingLoader.SftpKeyPath,
                Password = SettingLoader.SftpPassword,
                DefaultWorkingDirectory = SettingLoader.SftpDefaultWorkingDirectory
            };
        }

        public void SetToSettingLoader(ConnectionInformation information)
        {
            SettingLoader.SftpAddress = information.Address;
            SettingLoader.SftpPort = information.Port;
            SettingLoader.SftpIsPassword = information.IsPassword;
            SettingLoader.SftpUserName = information.Username;
            SettingLoader.SftpPassword = information.Password;
            SettingLoader.SftpKeyPath = information.KeyPath;
            SettingLoader.SftpDefaultWorkingDirectory = information.DefaultWorkingDirectory;
        }
        
        public void OpenFileViaSftp(Stream stream)
        {
            FilePath = null;
            ResetButtons();

            var configLoader = new ConfigLoader(stream);
            LoadToConfigList(configLoader);
        }

        public MemoryStream CreateConfigXml()
        {
            var stream = new MemoryStream();
            var configLoader = CreateConfigLoader();

            configLoader.Write(stream);
            stream.Position = 0;

            ResetButtons();

            return stream;
        }
    }
}
