using System;
using System.Collections.Generic;
using System.IO;

namespace SvManagerLibrary.Config
{
    public class ConfigLoader
    {
        private string fileName;
        private XMLWrapper.Reader reader;

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
                    reader = new XMLWrapper.Reader(fs);
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
        public void ChangeOrAddValue(string propertyName, string value)
        {
            if (!ChangeValue(propertyName, value))
            {
                configs.Add(propertyName, new ConfigInfo()
                {
                    PropertyName = propertyName,
                    Value = value
                });
            }
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
                XMLWrapper.Writer writer = new XMLWrapper.Writer();
                writer.SetRoot("ServerSettings");
                foreach (ConfigInfo configInfo in configs.Values)
                {
                    XMLWrapper.AttributeInfo[] attributeInfo = new XMLWrapper.AttributeInfo[2];
                    attributeInfo[0] = new XMLWrapper.AttributeInfo()
                    {
                        Name = "name",
                        Value = configInfo.PropertyName,
                    };
                    attributeInfo[1] = new XMLWrapper.AttributeInfo()
                    {
                        Name = "value",
                        Value = configInfo.Value,
                    };

                    writer.AddElement("property", attributeInfo);
                }
                writer.Write(fs);
            }
        }
    }
}
