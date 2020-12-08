using System.Collections.Generic;
using System.IO;
using System.Linq;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace SvManagerLibrary.Config
{
    public class ConfigLoader
    {
        private readonly string _fileName;
        private SavannahXmlReader _reader;

        private readonly Dictionary<string, ConfigInfo> _configs = new Dictionary<string, ConfigInfo>();

        public ConfigLoader(string path, bool newFile = false)
        {
            _fileName = path;
            if (!newFile)
            {
                Load();
            }
        }

        private void Load()
        {
            try
            {
                using var fs = new FileStream(_fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
                _reader = new SavannahXmlReader(fs);
                var names = _reader.GetAttributes("name", "ServerSettings/property").ToList();
                var values = _reader.GetAttributes("value", "ServerSettings/property").ToList();

                var length = names.Count > values.Count ? values.Count : names.Count;
                for (var i = 0; i < length; ++i)
                {
                    var configInfo = new ConfigInfo()
                    {
                        PropertyName = names[i],
                        Value = values[i],
                    };
                    _configs.Add(names[i], configInfo);
                }
            }
            catch
            {
                // ignored
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
                var configInfo = new ConfigInfo()
                {
                    PropertyName = propertyName,
                    Value = value,
                };
                _configs.Add(propertyName, configInfo);
            }
        }
        public void AddValues(ConfigInfo[] configs)
        {
            foreach (var config in configs)
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

        public void Clear()
        {
            _configs.Clear();
        }

        public Dictionary<string, ConfigInfo> GetAll()
        {
            return _configs;
        }

        public void Write()
        {
            using var fs = new FileStream(_fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            Write(fs);
        }

        public void Write(Stream stream)
        {
            var writer = new SavannahXmlWriter();
            var root = SavannahTagNode.CreateRoot("ServerSettings");
            var configXmlArray = (from config in _configs.Values
                                  let configAttributeInfo = CreateConfigAttributeInfos(config)
                                  select SavannahTagNode.CreateElement("property", configAttributeInfo)).ToArray();
            root.ChildNodes = configXmlArray;

            writer.Write(stream, root);
        }

        private static AttributeInfo[] CreateConfigAttributeInfos(ConfigInfo configInfo)
        {
            var attributeInfo = new[]
            {
                new AttributeInfo
                {
                    Name = "name",
                    Value = configInfo.PropertyName
                },
                new AttributeInfo
                {
                    Name = "value",
                    Value = configInfo.Value
                }
            };

            return attributeInfo;
        }
    }
}
