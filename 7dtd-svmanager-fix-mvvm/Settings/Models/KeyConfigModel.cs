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
        public ObservableCollection<ShortcutKeyForList> KeyList { get; } = new ObservableCollection<ShortcutKeyForList>();

        private readonly Dictionary<string, ShortcutKey> _internalKeyList = new Dictionary<string, ShortcutKey>();

        private readonly ShortcutKeyManager _shortcutKeyManager;

        private ModifierKeys _specialKey;
        private Key _mainKey;

        private string _currentKeyText;
        private string _commandText;
        private string _keyString;
        private int _currentIndex;

        public string CurrentKeyText
        {
            get => _currentKeyText;
            set => SetProperty(ref _currentKeyText, value);
        }
        public string CommandText
        {
            get => _commandText;
            set => SetProperty(ref _commandText, value);
        }
        public string KeyString
        {
            get => _keyString;
            set => SetProperty(ref _keyString, value);
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set => SetProperty(ref _currentIndex, value);
        }

        public KeyConfigModel(ShortcutKeyManager shortcutKeyManager)
        {
            _shortcutKeyManager = shortcutKeyManager;
        }

        public void Load()
        {
            var shortcutKeies = _shortcutKeyManager.ShortcutKeies;
            foreach (KeyValuePair<string, ShortcutKey> pair in shortcutKeies)
            {
                _internalKeyList.Add(pair.Key, pair.Value);

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
            _shortcutKeyManager.ShortcutKeies = _internalKeyList;
            _shortcutKeyManager.Save();
            closer();
        }

        public void AddKey(ShortcutKeyForList key)
        {
            KeyList.Add(key);
        }
        public void ChangeKey(int index, ShortcutKeyForList key)
        {
            if (KeyList.Count - 1 < index)
            {
                KeyList[index] = key;
            }
        }

        public void SelectionChaned()
        {
            var index = CurrentIndex;

            if (index == -1 || index > KeyList.Count) return;

            var shortcut = KeyList[index];
            CommandText = shortcut.Name;
            CurrentKeyText = shortcut.Key;
            KeyString = string.Empty;
        }

        public void InputKey(Key key)
        {
            _specialKey = Keyboard.Modifiers;

            switch(key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                case Key.LeftAlt:
                case Key.RightAlt:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    _mainKey = Key.None;
                    break;
                default:
                    _mainKey = key;
                    break;
            }

            KeyString = _specialKey != ModifierKeys.None ? $"{_specialKey} + {_mainKey}" : _mainKey.ToString();
        }
        public void DeleteKey()
        {
            int index = CurrentIndex;
            if (index < 0)
                return;

            var keyName = KeyList[index].Name;
            var description = KeyList[index].Description;
            KeyList[index] = new ShortcutKeyForList()
            {
                Name = KeyList[index].Name,
                Key = string.Empty,
                Description = description
            };
            _internalKeyList[keyName] = new ShortcutKey(keyName, ModifierKeys.None, Key.None, description);
        }
        public void ApplyKey()
        {
            var index = CurrentIndex;
            if (index < 0)
                return;

            var keyName = KeyList[index].Name;

            KeyList[index] = new ShortcutKeyForList()
            {
                Name = KeyList[index].Name,
                Key = _keyString,
                Description = KeyList[index].Description
            };
            var description = _internalKeyList[keyName].Description;
            _internalKeyList[keyName] = new ShortcutKey(keyName, _specialKey, _mainKey, description);
        }
    }
}
