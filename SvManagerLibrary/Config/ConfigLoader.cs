using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonCoreLib.XMLWrapper;

namespace SvManagerLibrary.Config
{
    public class ConfigLoader
    {
        private string fileName;
        private CommonXmlReader reader;

        private Dictionary<string, ConfigInfo> configs = new Dictionary<string, ConfigInfo>();

        public ConfigLoader(string path, bool newFile = false)
        {
            fileName = path;
            if (!newFile)
            {
                Load();
            }
        }

        private void Load()
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                {
                    reader = new CommonXmlReader(fs);
                    List<string> names = reader.GetAttributes("name", "ServerSettings/property", true);
                    List<string> values = reader.GetAttributes("value", "ServerSettings/property", true);

                    int length = names.Count > values.Count ? values.Count : names.Count;
                    for (int i = 0; i < length; ++i)
                    {
                        ConfigInfo configInfo = new ConfigInfo()
                        {
                            PropertyName = names[i],
                            Value = values[i],
                        };
                        configs.Add(names[i], configInfo);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public void AddValue(string propertyName, string value)
        {
            if (configs.ContainsKey(propertyName))
            {
                ChangeValue(propertyName, value);
            }
            else
            {
                ConfigInfo configInfo = new ConfigInfo()
                {
                    PropertyName = propertyName,
                    Value = value,
                };
                configs.Add(propertyName, configInfo);
            }
        }
        public void AddValues(ConfigInfo[] configs)
        {
            foreach (ConfigInfo config in configs)
            {
                AddValue(config.PropertyName, config.Value);
            }
        }
        public bool ChangeValue(string propertyName, string value)
        {
            if (configs.ContainsKey(propertyName))
            {
                configs[propertyName].Value = value;
                return true;
            }

            return false;
        }
        public ConfigInfo GetValue(string propertyName)
        {
            if (configs.ContainsKey(propertyName))
            {
                return configs[propertyName];
            }
            return null;
        }

        public void Clear()
        {
            configs.Clear();
        }

        public Dictionary<string, ConfigInfo> GetAll()
        {
            return configs;
        }

        public void Write()
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                Write(fs);
            }
        }

        public void Write(Stream stream)
        {
            var writer = new CommonXmlWriter();
            var root = writer.CreateRoot("ServerSettings");
            var configXmlArray = (from config in configs.Values
                                  let configAttributeInfo = CreateConfigAttributeInfos(config)
                                  select writer.CreateElement("property", configAttributeInfo)).ToArray();
            root.Append(configXmlArray);

            writer.Write(stream, root);
        }

        private AttributeInfo[] CreateConfigAttributeInfos(ConfigInfo configInfo)
        {
            var attributeInfo = new AttributeInfo[]
            {
                new AttributeInfo()
                {
                    Name = "name",
                    Value = configInfo.PropertyName
                },
                new AttributeInfo()
                {
                    Name = "value",
                    Value = configInfo.Value
                }
            };

            return attributeInfo;
        }
    }
}
