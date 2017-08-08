using SvManagerLibrary.XMLWrapper;
using System.Collections.Generic;
using CommonLib.Extentions;

namespace ConfigEditor_mvvm.Models
{
    public class TemplateLoader
    {
        /// <summary>
        /// バージョン一覧を管理します。
        /// </summary>
        public List<string> VersionList { get; private set; }

        private List<string> versionPathList;
        /// <summary>
        /// &lt;バージョン, &lt;プロパティ名, テンプレート要素&gt;&gt;
        /// </summary>
        private Dictionary<string, Dictionary<string, ConfigListInfo>> templateData = new Dictionary<string, Dictionary<string, ConfigListInfo>>();

        /// <summary>
        /// テンプレートファイルの読み込みを行います。
        /// </summary>
        /// <param name="lang">言語名</param>
        /// <param name="templateListPath">テンプレートリストを管理するファイルのパス</param>
        public TemplateLoader(string lang, string templateListPath)
        {
            VersionListLoad();
            TemplateLoad(lang);
        }
        /// <summary>
        /// バージョンリストをロードします。
        /// </summary>
        private void VersionListLoad()
        {
            var xmlReader = new Reader(StaticData.VersionListPath);
            VersionList = xmlReader.GetAttributes("version", "/root/configs/config");
            versionPathList = xmlReader.GetValues("/root/configs/config", false);
        }

        /// <summary>
        /// テンプレートをロードします。
        /// </summary>
        /// <param name="lang">言語名</param>
        private void TemplateLoad(string lang)
        {
            versionPathList.ForEachInIndex((index, path) =>
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
        /// <summary>
        /// 文字列をConfigTypeへ変換します。
        /// </summary>
        /// <param name="stype">ConfigTypeと対応する文字列</param>
        /// <returns></returns>
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

        /// <summary>
        /// バージョン別にテンプレートディクショナリを複製して返します。
        /// </summary>
        /// <param name="version">バージョン</param>
        /// <returns>バージョン別のテンプレートディクショナリ</returns>
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
        /// バージョン別のテンプレート配列を複製して返します。
        /// </summary>
        /// <param name="version">バージョン</param>
        /// <returns>バージョンのテンプレート配列</returns>
        public ConfigListInfo[] GetConfigList(string version)
        {
            var dic = GetConfigDictionary(version);
            var list = new List<ConfigListInfo>();
            dic.ForEach((key, configInfo) => list.Add(configInfo));
            return list.ToArray();
        }
    }
}
