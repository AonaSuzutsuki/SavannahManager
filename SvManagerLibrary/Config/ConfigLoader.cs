using System;
using System.Collections.Generic;
using System.IO;

namespace SvManagerLibrary.Config
{
    public class ConfigLoader : IDisposable
    {
        FileStream _fs;
        XMLWrapper.Reader _reader;

        Dictionary<string, ConfigInfo> _configs = new Dictionary<string, ConfigInfo>();

        public ConfigLoader(string path, bool newFile = false)
        {
            _fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            if (!newFile)
            {
                Load();
            }
        }

        private void Load()
        {
            try
            {
                _reader = new XMLWrapper.Reader(_fs);
                List<string> names = _reader.GetAttributes("name", "ServerSettings/property");
                List<string> values = _reader.GetAttributes("value", "ServerSettings/property");

                int length = names.Count > values.Count ? values.Count : names.Count;
                for (int i = 0; i < length; ++i)
                {
                    ConfigInfo configInfo = new ConfigInfo()
                    {
                        PropertyName = names[i],
                        Value = values[i],
                    };
                    _configs.Add(names[i], configInfo);
                }
            }
            catch
            {
                return;
            }
        }

        public void AddValue(string propertyName, string value)
        {
            if (_configs.ContainsKey(propertyName))
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
                _configs.Add(propertyName, configInfo);
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
            if (_configs.ContainsKey(propertyName))
            {
                _configs[propertyName].Value = value;
                return true;
            }

            return false;
        }
        public ConfigInfo GetValue(string propertyName)
        {
            if (_configs.ContainsKey(propertyName))
            {
                return _configs[propertyName];
            }
            return null;
        }
        public Dictionary<string, ConfigInfo> GetAll()
        {
            return _configs;
        }

        public void Write()
        {
            XMLWrapper.Writer writer = new XMLWrapper.Writer();
            writer.SetRoot("ServerSettings");
            foreach (ConfigInfo configInfo in _configs.Values)
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
            writer.Write(_fs);
        }

        public void Dispose()
        {
            ((IDisposable)_fs).Dispose();
        }
    }
}
