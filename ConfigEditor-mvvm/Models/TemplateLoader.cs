using System.Collections.Generic;
using CommonCoreLib.XMLWrapper;
using CommonExtensionLib.Extensions;

namespace ConfigEditor_mvvm.Models
{
    public class TemplateLoader
    {
        /// <summary>
        /// Get managed version list.
        /// </summary>
        public List<string> VersionList { get; private set; }

        private List<string> versionPathList;
        /// <summary>
        /// Manage template data.
        /// &lt;Version, &lt;PropertyName, Element&gt;&gt;
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, ConfigListInfo>> templateData = new Dictionary<string, Dictionary<string, ConfigListInfo>>();

        /// <summary>
        /// Load template files.
        /// </summary>
        /// <param name="lang">Language Name</param>
        /// <param name="templateListPath">File path of managed template list</param>
        public TemplateLoader(string lang, string templateListPath)
        {
            VersionListLoad(templateListPath);
            TemplateLoad(lang);
        }
        /// <summary>
        /// Load version list.
        /// </summary>
        private void VersionListLoad(string templateListPath)
        {
            var xmlReader = new CommonXmlReader(templateListPath);
            VersionList = xmlReader.GetAttributes("version", "/root/configs/config");
            versionPathList = xmlReader.GetValues("/root/configs/config", false);
        }

        /// <summary>
        /// Load template files. Main process.
        /// </summary>
        /// <param name="lang">言語名</param>
        private void TemplateLoad(string lang)
        {
            versionPathList.ForEachInIndex((index, path) =>
            {
                var version = VersionList[index];
                var dic = new Dictionary<string, ConfigListInfo>();
                var baseReader = new CommonXmlReader(string.Format(ConstantValues.BaseTemplateFileName, path, lang));
                var names = baseReader.GetAttributes("name", "/ServerSettings/property");
                names.ForEach((name) =>
                {
                    var xpath = $"/ServerSettings/property[@name=\"{name}\"]";
                    var value = baseReader.GetAttribute("value", xpath);
                    var selection = baseReader.GetAttribute("selection", xpath);
                    var type = baseReader.GetAttribute("type", xpath);
                    var description = baseReader.GetValue(xpath);
                    dic.Add(name, new ConfigListInfo()
                    {
                        Property = name,
                        Value = value,
                        Selection = string.IsNullOrEmpty(selection) ? null : selection.Split('/'),
                        Type = ConvertConfigType(type),
                        Description = description
                    });
                });

                templateData.Add(version, dic);
            });
        }
        /// <summary>
        /// Convert string to ConfigType.
        /// </summary>
        /// <param name="propType">String corresponding to ConfigType</param>
        /// <returns></returns>
        private ConfigType ConvertConfigType(string propType)
        {
            var configType = ConfigType.String;
            switch (propType)
            {
                case "string":
                    configType = ConfigType.String;
                    break;
                case "integer":
                    configType = ConfigType.Integer;
                    break;
                case "combo":
                    configType = ConfigType.Combo;
                    break;
            }
            return configType;
        }

        /// <summary>
        /// Duplicate and return the template dictionary by version.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>Template dictionary by version</returns>
        public Dictionary<string, ConfigListInfo> GetConfigDictionary(string version)
        {
            if (templateData.ContainsKey(version))
            {
                var dic = new Dictionary<string, ConfigListInfo>();
                templateData[version].ForEach((key, val) => dic.Add(key, val.Clone() as ConfigListInfo));

                return dic;
            }
            else
                return null;
        }
        /// <summary>
        /// Duplicate and return the template array by version.
        /// </summary>
        /// <param name="version">Version</param>
        /// <returns>Template array by version</returns>
        public ConfigListInfo[] GetConfigList(string version)
        {
            var dic = GetConfigDictionary(version);
            var list = new List<ConfigListInfo>();
            dic.ForEach((key, configInfo) => list.Add(configInfo));
            return list.ToArray();
        }
    }
}
