using SvManagerLibrary.XMLWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            load(path, basePath);
        }
        
        private void load(string path, string basePath)
        {
            if (!File.Exists(basePath)) return;

            var xmlBaseReader = new Reader(basePath);
            List<string> baseNames = xmlBaseReader.GetAttributes("shortcutname", "shortcuts/shortcut");
            List<string> baseSpecialkeies = xmlBaseReader.GetAttributes("specialkey", "shortcuts/shortcut");
            List<string> baseMainkeies = xmlBaseReader.GetAttributes("mainkey", "shortcuts/shortcut");
            List<string> baseDescriptions = xmlBaseReader.GetValues("shortcuts/shortcut");
            
            var modConverter = new ModifierKeysConverter();
            var keyConverter = new KeyConverter();

            int count = baseNames.Count > baseSpecialkeies.Count ? baseSpecialkeies.Count : baseNames.Count;
            count = count > baseMainkeies.Count ? baseMainkeies.Count : count;
            count = count > baseDescriptions.Count ? baseDescriptions.Count : count;
            for (int i = 0; i < count; ++i)
            {
                ModifierKeys specialKey = (ModifierKeys)modConverter.ConvertFromString(baseSpecialkeies[i]);
                Key mainKey = (Key)keyConverter.ConvertFromString(baseMainkeies[i]);
                ShortcutKeies.Add(baseNames[i], new ShortcutKey(baseNames[i], specialKey, mainKey, baseDescriptions[i].TrimEnd('\n').TrimEnd('\r')));
            }

            if (!File.Exists(path)) return;

            var xmlReader = new Reader(path);
            List<string> names = xmlReader.GetAttributes("shortcutname", "shortcuts/shortcut");
            List<string> specialkeies = xmlReader.GetAttributes("specialkey", "shortcuts/shortcut");
            List<string> mainkeies = xmlReader.GetAttributes("mainkey", "shortcuts/shortcut");

            foreach (var item in names.Select((v, i) => new { v, i }))
            {
                if (ShortcutKeies.ContainsKey(item.v))
                {
                    ModifierKeys specialKey;
                    if (specialkeies[item.i].Equals("None"))
                    {
                        specialKey = ModifierKeys.None;
                    }
                    else
                    {
                        specialKey = (ModifierKeys)modConverter.ConvertFromString(specialkeies[item.i]);
                    }
                    Key mainKey = (Key)keyConverter.ConvertFromString(mainkeies[item.i]);
                    string description = ShortcutKeies[item.v].Description;
                    ShortcutKeies[item.v] = new ShortcutKey(item.v, specialKey, mainKey, description);
                }
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
            
            foreach (ShortcutKey shortcutKey in ShortcutKeies.Values)
            {
                xmlWriter.AddElement("shortcut", CreateAttributeInfo(shortcutKey));
            }

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
