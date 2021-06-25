using System.Collections.Generic;
using System.Linq;
using CommonCoreLib.XMLWrapper;
using CommonExtensionLib.Extensions;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace ConfigEditor_mvvm.Models
{
    public class TemplateLoader
    {
        /// <summary>
        /// Get managed version list.
        /// </summary>
        public IList<string> VersionList { get; private set; }

        private IList<string> _versionPathList;
        /// <summary>
        /// Manage template data.
        /// &lt;Version, &lt;PropertyName, Element&gt;&gt;
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, ConfigListInfo>> _templateData = new Dictionary<string, Dictionary<string, ConfigListInfo>>();

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
            var xmlReader = new CachedSavannahXmlReader(templateListPath);
            VersionList = xmlReader.GetAttributes("version", "/root/configs/config").ToList();
            _versionPathList = xmlReader.GetValues("/root/configs/config", false).ToList();
        }

        /// <summary>
        /// Load template files. Main process.
        /// </summary>
        /// <param name="lang">言語名</param>
        private void TemplateLoad(string lang)
        {
            foreach (var item in _versionPathList.Select((v, i) => new { Index = i, Value = v }))
            {
                var version = VersionList[item.Index];
                var dic = new Dictionary<string, ConfigListInfo>();
                var baseReader = new CachedSavannahXmlReader(string.Format(ConstantValues.BaseTemplateFileName, item.Value, lang));
                var names = baseReader.GetAttributes("name", "/ServerSettings/property");
                foreach (var name in names)
                {
                    var xpath = $"/ServerSettings/property[@name=\"{name}\"]";
                    if (!(baseReader.GetNode(xpath) is SavannahTagNode node))
                        continue;
                    var value = node.GetAttribute("value").Value;
                    var selection = node.GetAttribute("selection").Value;
                    var type = node.GetAttribute("type").Value;
                    var description = node.InnerText;
                    dic.Add(name, new ConfigListInfo()
                    {
                        Property = name,
                        Value = value,
                        Selection = string.IsNullOrEmpty(selection) ? null : selection.Split('/'),
                        Type = ConvertConfigType(type),
                        Description = description
                    });
                }

                _templateData.Add(version, dic);
            }
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
            if (_templateData.ContainsKey(version))
            {
                return _templateData[version].ToDictionary(item => item.Key, item => item.Value.Clone() as ConfigListInfo);
            }

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
            return dic.Select(item => item.Value).ToArray();
        }
    }
}
