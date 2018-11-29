using System.Windows;
using System.Collections.ObjectModel;
using _7dtd_svmanager_fix_mvvm.Settings.Models;
using System.Windows.Input;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using CommonStyleLib.ViewModels;

namespace _7dtd_svmanager_fix_mvvm.Settings.ViewModels
{
    public class KeyConfigViewModel : ViewModelBase
    {
        KeyConfigModel model;
        public KeyConfigViewModel(Window view, KeyConfigModel model) : base(view, model)
        {
            this.model = model;
            base.view = view;

            Loaded = new DelegateCommand(Window_Loaded);
            KeyListSelectionChanged = new DelegateCommand(KeyList_SelectionChanged);
            DeleteCurrentKeyBtClick = new DelegateCommand(DeleteCurrentKeyBt_Click);
            ApplyKeyBtClick = new DelegateCommand(ApplyKeyBt_Click);
            OKayBtClick = new DelegateCommand(OKayBt_Click);
            InputKeyTextBoxKeyDown = new DelegateCommand<KeyEventArgs>(InputKeyTextBox_KeyDown);

            KeyList = new ReadOnlyObservableCollection<ShortcutKeyForList>(model.KeyList);
            KeyListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.CurrentIndex);
            CommandText = model.ToReactivePropertyAsSynchronized(m => m.CommandText);
            CurrentKeyText = model.ToReactivePropertyAsSynchronized(m => m.CurrentKeyText);
            InputKeyText = model.ToReactivePropertyAsSynchronized(m => m.KeyString);
        }

        private ReadOnlyObservableCollection<ShortcutKeyForList> keyList;
        public ReadOnlyObservableCollection<ShortcutKeyForList> KeyList
        {
            get => keyList;
            set => SetProperty(ref keyList, value);
        }
        
        public ReactiveProperty<int> KeyListSelectedIndex { get; set; }
        
        public ReactiveProperty<string> CommandText { get; set; }
        public ReactiveProperty<string> CurrentKeyText { get; set; }
        public ReactiveProperty<string> InputKeyText { get; set; }

        #region EventProperties
        public ICommand KeyListSelectionChanged { get; }

        public ICommand DeleteCurrentKeyBtClick { get; }
        public ICommand ApplyKeyBtClick { get; }
        public ICommand OKayBtClick { get; }
        public ICommand InputKeyTextBoxKeyDown { get; }
        #endregion

        #region EventMethod
        private void Window_Loaded()
        {
            model.Load();
        }

        private void InputKeyTextBox_KeyDown(KeyEventArgs e)
        {
            model.InputKey(e.Key);
        }

        private void KeyList_SelectionChanged()
        {
            model.SelectionChaned();
        }

        private void DeleteCurrentKeyBt_Click()
        {
            model.DeleteKey();
        }
        private void ApplyKeyBt_Click()
        {
            model.ApplyKey();
        }

        private void OKayBt_Click()
        {
            model.Save(view.Close);
        }
        #endregion
    }
}
