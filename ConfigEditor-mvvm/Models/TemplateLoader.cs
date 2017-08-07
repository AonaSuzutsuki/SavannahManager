using SvManagerLibrary.XMLWrapper;
using CommonLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEditor_mvvm.Models
{
    public class TemplateLoader
    {
        public List<string> VersionList { get; private set; }
        public List<string> VersionPathList { get; private set; }

        // ver -> config
        private Dictionary<string, Dictionary<string, ConfigListInfo>> templateData = new Dictionary<string, Dictionary<string, ConfigListInfo>>();

        public TemplateLoader(string lang, string templateListPath)
        {
            Reader xmlReader = new Reader(StaticData.VersionListPath);
            VersionListLoad(xmlReader);
            TemplateLoad(lang, xmlReader);
        }

        private void VersionListLoad(Reader xmlReader)
        {
            VersionList = xmlReader.GetAttributes("version", "/root/configs/config");
            VersionPathList = xmlReader.GetValues("/root/configs/config", false);
        }

        private void TemplateLoad(string lang, Reader xmlReader)
        {
            VersionPathList.ForEachInIndex((index, path) =>
            {
                var version = VersionList[index];
                var dic = new Dictionary<string, ConfigListInfo>();
                var baseReader = new Reader(string.Format(StaticData.BaseTemplateFileName, path, lang));
                var names = baseReader.GetAttributes("name", "/ServerSettings/property");
                names.ForEach((name) =>
                {
                    var xpath = string.Format("/ServerSettings/property[@name=\"{0}\"]", name);
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
        private ConfigType ConvertConfigType(string stype)
        {
            ConfigType configType = ConfigType.String;
            switch (stype)
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

        public Dictionary<string, ConfigListInfo> GetConfigDictionary(string version)
        {
            if (templateData.ContainsKey(version))
                return templateData[version];
            else
                return null;
        }
        public List<ConfigListInfo> GetConfigList(string version)
        {
            var dic = GetConfigDictionary(version);
            var list = new List<ConfigListInfo>();
            dic.ForEach((key, configInfo) => list.Add(configInfo));
            return list;
        }
    }
}
