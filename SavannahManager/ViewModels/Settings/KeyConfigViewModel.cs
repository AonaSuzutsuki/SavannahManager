using System.Collections.ObjectModel;
using System.Windows.Input;
using _7dtd_svmanager_fix_mvvm.Models.Settings;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace _7dtd_svmanager_fix_mvvm.ViewModels.Settings
{
    public class KeyConfigViewModel : ViewModelBase
    {
        KeyConfigModel model;
        public KeyConfigViewModel(WindowService windowService, KeyConfigModel model) : base(windowService, model)
        {
            this.model = model;

            Loaded = new DelegateCommand(Window_Loaded);
            KeyListSelectionChanged = new DelegateCommand(KeyList_SelectionChanged);
            DeleteCurrentKeyCommand = new DelegateCommand(DeleteCurrentKeyBt_Click);
            ApplyKeyBtCommand = new DelegateCommand(ApplyKeyBt_Click);
            OkayCommand = new DelegateCommand(OKayBt_Click);
            InputKeyTextBoxKeyDown = new DelegateCommand<KeyEventArgs>(InputKeyTextBox_KeyDown);

            KeyList = new ReadOnlyObservableCollection<ShortcutKeyForList>(model.KeyList);
            KeyListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.CurrentIndex);
            CommandText = model.ToReactivePropertyAsSynchronized(m => m.CommandText);
            CurrentKeyText = model.ToReactivePropertyAsSynchronized(m => m.CurrentKeyText);
            InputKeyText = model.ToReactivePropertyAsSynchronized(m => m.KeyString);
        }

        private ReadOnlyObservableCollection<ShortcutKeyForList> _keyList;
        public ReadOnlyObservableCollection<ShortcutKeyForList> KeyList
        {
            get => _keyList;
            set => SetProperty(ref _keyList, value);
        }
        
        public ReactiveProperty<int> KeyListSelectedIndex { get; set; }
        
        public ReactiveProperty<string> CommandText { get; set; }
        public ReactiveProperty<string> CurrentKeyText { get; set; }
        public ReactiveProperty<string> InputKeyText { get; set; }

        #region EventProperties
        public ICommand KeyListSelectionChanged { get; }

        public ICommand DeleteCurrentKeyCommand { get; }
        public ICommand ApplyKeyBtCommand { get; }
        public ICommand OkayCommand { get; }
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
            model.Save(WindowManageService.Close);
        }
        #endregion
    }
}
