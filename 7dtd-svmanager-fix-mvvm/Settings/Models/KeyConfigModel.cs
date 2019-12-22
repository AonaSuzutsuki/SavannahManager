using CommonStyleLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace _7dtd_svmanager_fix_mvvm.Settings.Models
{
    public class ShortcutKeyForList
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
    }

    public class KeyConfigModel : ModelBase
    {
        private ObservableCollection<ShortcutKeyForList> keyList = new ObservableCollection<ShortcutKeyForList>();
        public ObservableCollection<ShortcutKeyForList> KeyList
        {
            get => keyList;
        }

        private Dictionary<string, ShortcutKey> internalKeyList = new Dictionary<string, ShortcutKey>();

        private ShortcutKeyManager shortcutKeyManager;

        private ModifierKeys specialKey;
        private Key mainKey;

        private string currentKeyText;
        public string CurrentKeyText
        {
            get => currentKeyText;
            set => SetProperty(ref currentKeyText, value);
        }
        private string commandText;
        public string CommandText
        {
            get => commandText;
            set => SetProperty(ref commandText, value);
        }
        private string keyString;
        public string KeyString
        {
            get => keyString;
            set => SetProperty(ref keyString, value);
        }
        private int currentIndex;

        public int CurrentIndex
        {
            get => currentIndex;
            set => SetProperty(ref currentIndex, value);
        }

        public KeyConfigModel(ShortcutKeyManager shortcutKeyManager)
        {
            this.shortcutKeyManager = shortcutKeyManager;
        }

        public void Load()
        {
            var shortcutKeies = shortcutKeyManager.ShortcutKeies;
            foreach (KeyValuePair<string, ShortcutKey> pair in shortcutKeies)
            {
                internalKeyList.Add(pair.Key, pair.Value);

                var keyForList = new ShortcutKeyForList()
                {
                    Name = pair.Value.ShortcutName,
                    Key = pair.Value.ToString(),
                    Description = pair.Value.Description
                };
                AddKey(keyForList);
            }
        }
        public void Save(Action closer)
        {
            shortcutKeyManager.ShortcutKeies = internalKeyList;
            shortcutKeyManager.Save();
            closer();
        }

        public void AddKey(ShortcutKeyForList key)
        {
            keyList.Add(key);
        }
        public void ChangeKey(int index, ShortcutKeyForList key)
        {
            if (keyList.Count - 1 < index)
            {
                keyList[index] = key;
            }
        }

        public void SelectionChaned()
        {
            int index = CurrentIndex;

            if (index == -1 || index > KeyList.Count) return;

            var shortcut = KeyList[index];
            CommandText = shortcut.Name;
            CurrentKeyText = shortcut.Key;
            KeyString = string.Empty;
        }

        public void InputKey(Key key)
        {
            specialKey = Keyboard.Modifiers;

            switch(key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    mainKey = Key.None;
                    break;
                default:
                    mainKey = key;
                    break;
            }

            KeyString = specialKey != ModifierKeys.None ? $"{specialKey} + {mainKey}" : mainKey.ToString();
        }
        public void DeleteKey()
        {
            int index = CurrentIndex;
            if (index > -1)
                return;

            var keyName = KeyList[index].Name;
            KeyList[index] = new ShortcutKeyForList()
            {
                Name = KeyList[index].Name,
                Key = string.Empty,
                Description = KeyList[index].Description
            };
            internalKeyList[keyName] = new ShortcutKey(keyName, ModifierKeys.None, Key.None);
        }
        public void ApplyKey()
        {
            int index = CurrentIndex;
            if (index > -1)
                return;

            var keyName = KeyList[index].Name;

            KeyList[index] = new ShortcutKeyForList()
            {
                Name = KeyList[index].Name,
                Key = keyString,
                Description = KeyList[index].Description
            };
            string description = internalKeyList[keyName].Description;
            internalKeyList[keyName] = new ShortcutKey(keyName, specialKey, mainKey, description);
        }
    }
}
