using System.Collections.Generic;
using System.IO;
using System.Linq;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace SvManagerLibrary.Config
{
    /// <summary>
    /// Provides a loader of config.
    /// </summary>
    public class ConfigLoader
    {
        private readonly string _fileName;
        private SavannahXmlReader _reader;

        private readonly Dictionary<string, ConfigInfo> _configs = new Dictionary<string, ConfigInfo>();

        /// <summary>
        /// Initialize the ConfigLoader.
        /// </summary>
        /// <param name="path">A filepath to be loaded or created.</param>
        /// <param name="newFile">Whether to create a new file.</param>
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

        /// <summary>
        /// Add a property.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public void AddProperty(string propertyName, string value)
        {
            if (_configs.ContainsKey(propertyName))
            {
                ChangeProperty(propertyName, value);
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

        /// <summary>
        /// Add a properties.
        /// </summary>
        /// <param name="configs">Array of ConfigInfo to be added.</param>
        public void AddProperties(ConfigInfo[] configs)
        {
            foreach (var config in configs)
            {
                AddProperty(config.PropertyName, config.Value);
            }
        }

        /// <summary>
        /// Change the value of property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="value">A value to change.</param>
        /// <returns>Success or failure.</returns>
        public bool ChangeProperty(string propertyName, string value)
        {
            if (_configs.ContainsKey(propertyName))
            {
                _configs[propertyName].Value = value;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get the property.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The ConfigInfo object that was found.</returns>
        public ConfigInfo GetProperty(string propertyName)
        {
            if (_configs.ContainsKey(propertyName))
            {
                return _configs[propertyName];
            }
            return null;
        }

        /// <summary>
        /// Clear the internal config list.
        /// </summary>
        public void Clear()
        {
            _configs.Clear();
        }

        /// <summary>
        /// Get all configs.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, ConfigInfo> GetAll()
        {
            var dict = _configs.ToDictionary(pair => pair.Key, (pair) =>
                new ConfigInfo
                {
                    PropertyName = pair.Value.PropertyName,
                    Value = pair.Value.Value
                });
            return dict;
        }

        /// <summary>
        /// Write config to saved path.
        /// </summary>
        public void Write()
        {
            using var fs = new FileStream(_fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            Write(fs, _configs);
        }


        /// <summary>
        /// Write config to a stream.
        /// </summary>
        /// <param name="stream">A stream to be saved.</param>
        /// <param name="configs">A dictionary to save.</param>
        public static void Write(Stream stream, Dictionary<string, ConfigInfo> configs)
        {
            var writer = new SavannahXmlWriter();
            var root = SavannahTagNode.CreateRoot("ServerSettings");
            var configXmlArray = (from config in configs.Values
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
