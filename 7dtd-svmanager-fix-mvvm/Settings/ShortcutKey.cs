using _7dtd_svmanager_fix_mvvm.Models;
using CommonLib.Models;
using SvManagerLibrary.XMLWrapper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace _7dtd_svmanager_fix_mvvm.Settings
{
    public class ShortcutKey
    {
        public string ShortcutName { private set; get; }
        public ModifierKeys SpecialKey { private set; get; }
        public Key MainKey { private set; get; }
        public string Description { private set; get; }

        public ShortcutKey(string shortcutName, ModifierKeys specialKey, Key mainKey, string description = null)
        {
            ShortcutName = shortcutName;
            SpecialKey = specialKey;
            MainKey = mainKey;
            Description = description;
        }

        public bool IsPushed(ModifierKeys specialKey, Key mainKey)
        {
            return (SpecialKey == specialKey && MainKey == mainKey);
        }

        public override string ToString()
        {
            string str = string.Empty;
            if (SpecialKey != 0)
            {
                str += SpecialKey.ToString() + " + ";
            }
            str += MainKey.ToString();
            return str;
        }
    }

    public class ShortcutKeyManager
    {
        public Dictionary<string, ShortcutKey> ShortcutKeies { set; get; } = new Dictionary<string, ShortcutKey>();

        private string xmlPath;
        private string xmlBasePath;

        public ShortcutKeyManager(string path, string basePath)
        {
            xmlPath = path;
            xmlBasePath = basePath;
            Load(path, basePath);
        }
        
        private void Load(string path, string basePath)
        {
            if (!File.Exists(basePath)) return;

            var xmlBaseReader = new Reader(basePath);
            var baseDic = new KeyConfigDictionary()
            {
                { "shortcutnames", xmlBaseReader.GetAttributes("shortcutname", "shortcuts/shortcut") },
                { "specialkeies", xmlBaseReader.GetAttributes("specialkey", "shortcuts/shortcut") },
                { "mainkeies", xmlBaseReader.GetAttributes("mainkey", "shortcuts/shortcut") },
                { "descriptions", xmlBaseReader.GetValues("shortcuts/shortcut") }
            };

            var modConverter = new ModifierKeysConverter();
            var keyConverter = new KeyConverter();
            
            for (int i = 0; i < baseDic.MinValueCount; ++i)
            {
                ModifierKeys specialKey = (ModifierKeys)modConverter.ConvertFromString(baseDic["specialkeies"][i]);
                Key mainKey = (Key)keyConverter.ConvertFromString(baseDic["mainkeies"][i]);
                ShortcutKeies.Add(baseDic["shortcutnames"][i],
                    new ShortcutKey(baseDic["shortcutnames"][i], specialKey, mainKey, baseDic["descriptions"][i].TrimEnd('\n').TrimEnd('\r')));
            }

            if (!File.Exists(path)) return;

            var xmlReader = new Reader(path);
            var dic = new KeyConfigDictionary
            {
                { "shortcutnames", xmlReader.GetAttributes("shortcutname", "shortcuts/shortcut") },
                { "specialkeies", xmlReader.GetAttributes("specialkey", "shortcuts/shortcut") },
                { "mainkeies", xmlReader.GetAttributes("mainkey", "shortcuts/shortcut") }
            };

            for (int i = 0; i < dic.MinValueCount; ++i)
            {
                ModifierKeys specialKey;
                if (dic["specialkeies"][i].Equals("None"))
                    specialKey = ModifierKeys.None;
                else
                    specialKey = (ModifierKeys)modConverter.ConvertFromString(dic["specialkeies"][i]);
                Key mainKey = (Key)keyConverter.ConvertFromString(dic["mainkeies"][i]);
                string description = ShortcutKeies[dic["shortcutnames"][i]].Description;
                ShortcutKeies[dic["shortcutnames"][i]] = new ShortcutKey(dic["shortcutnames"][i], specialKey, mainKey, description);
            }
            return;
        }

        public bool IsPushed(string keyName, ModifierKeys specialKey, Key mainKey)
        {
            if (!ShortcutKeies.ContainsKey(keyName)) return false;
            return ShortcutKeies[keyName].IsPushed(specialKey, mainKey);
        }

        public void Save()
        {
            var xmlWriter = new Writer();
            xmlWriter.SetRoot("shortcuts");

            ShortcutKeies.ForEach((key, value) => xmlWriter.AddElement("shortcut", CreateAttributeInfo(value)));

            xmlWriter.Write(xmlPath);
        }
        private AttributeInfo[] CreateAttributeInfo(ShortcutKey shortcutKey)
        {
            AttributeInfo[] attributes = new AttributeInfo[3];
            attributes[0] = new AttributeInfo()
            {
                Name = "shortcutname",
                Value = shortcutKey.ShortcutName
            };
            attributes[1] = new AttributeInfo()
            {
                Name = "specialkey",
                Value = shortcutKey.SpecialKey.ToString()
            };
            attributes[2] = new AttributeInfo()
            {
                Name = "mainkey",
                Value = shortcutKey.MainKey.ToString()
            };
            return attributes;
        }
    }
}
