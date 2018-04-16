using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonLib.Models;
using CommonLib.ViewModels;
using System.Windows.Input;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using ConfigEditor_mvvm.Models;
using Reactive.Bindings.Extensions;
using Prism.Commands;

namespace ConfigEditor_mvvm.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private MainWindowModel model;
        public MainWindowViewModel(Window view, MainWindowModel model) : base(view, model)
        {
            this.model = model;

            #region Event Initialize
            Loaded = new DelegateCommand(MainWindow_Loaded);
            KeyDown = new DelegateCommand<KeyEventArgs>(MainWindow_KeyDown);

            NewFileBtClicked = new DelegateCommand(NewFileBt_Clicked);
            OpenBtClicked = new DelegateCommand(OpenBt_Clicked);
            SaveAsBtClicked = new DelegateCommand(SaveAsBt_Clicked);
            SaveBtClicked = new DelegateCommand(SaveBt_Clicked);

            VersionsListSelectionChanged = new DelegateCommand(VersionsList_SelectionChanged);
            ConfigListSelectionChanged = new DelegateCommand(ConfigList_SelectionChanged);
            ValueListSelectionChanged = new DelegateCommand(ValueList_SelectionChanged);
            ValueTextTextChanged = new DelegateCommand(ValueTextText_Changed);
            #endregion

            #region Propertiy Initialize
            ModifiedVisibility = model.ToReactivePropertyAsSynchronized(m => m.ModifiedVisibility);
            SaveBtEnabled = model.ToReactivePropertyAsSynchronized(m => m.SaveBtEnabled);

            VersionList = model.ObserveProperty(m => m.VersionList).ToReactiveProperty();
            ConfigList = model.ObserveProperty(m => m.ConfigList).ToReactiveProperty();
            ValueList = model.ObserveProperty(m => m.ValueList).ToReactiveProperty();
            VersionListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.VersionListSelectedIndex);
            ConfigListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.ConfigListSelectedIndex);
            ValueListSelectedIndex = model.ToReactivePropertyAsSynchronized(m => m.ValueListSelectedIndex);

            NameLabel = model.ToReactivePropertyAsSynchronized(m => m.NameLabel);
            DescriptionLabel = model.ToReactivePropertyAsSynchronized(m => m.DescriptionLabel);
            ValueText = model.ToReactivePropertyAsSynchronized(m => m.ValueText);
            ValueListVisibility = model.ToReactivePropertyAsSynchronized(m => m.ValueListVisibility);
            ValueTextBoxVisibility = model.ToReactivePropertyAsSynchronized(m => m.ValueTextBoxVisibility);
            #endregion

            if (Loaded != null && Loaded.CanExecute(null))
            {
                Loaded?.Execute(null);
            }
        }

        #region Properties
        public ReactiveProperty<Visibility> ModifiedVisibility { get; set; }
        public ReactiveProperty<bool> SaveBtEnabled { get; set; }

        public ReactiveProperty<ObservableCollection<string>> VersionList { get; }
        public ReactiveProperty<int> VersionListSelectedIndex { get; set; }

        /// <summary>
        /// It is an intermediary property of the Config list.
        /// </summary>
        public ReactiveProperty<ObservableCollection<ConfigListInfo>> ConfigList { get; }
        public ReactiveProperty<int> ConfigListSelectedIndex { get; set; }

        public ReactiveProperty<string> NameLabel { get; set; }
        public ReactiveProperty<string> DescriptionLabel { get; set; }

        /// <summary>
        /// It is an intermediary property of the Value list.
        /// </summary>
        public ReactiveProperty<ObservableCollection<string>> ValueList { get; }
        public ReactiveProperty<int> ValueListSelectedIndex { get; set; }

        public ReactiveProperty<string> ValueText { get; set; }
        public ReactiveProperty<Visibility> ValueListVisibility { get; set; }
        public ReactiveProperty<Visibility> ValueTextBoxVisibility { get; set; }

        #endregion

        #region Event Properties
        public ICommand KeyDown { get; set; }

        public ICommand NewFileBtClicked { get; }
        public ICommand OpenBtClicked { get; }
        public ICommand SaveAsBtClicked { get; }
        public ICommand SaveBtClicked { get; }

        public ICommand VersionsListSelectionChanged { get; }
        public ICommand ConfigListSelectionChanged { get; }
        public ICommand ValueListSelectionChanged { get; }

        public ICommand ValueTextTextChanged { get; }
        #endregion

        #region Event Methods
        public void MainWindow_KeyDown(KeyEventArgs e)
        {
            model.ShortcutKey(e, Keyboard.Modifiers);
        }

        public void NewFileBt_Clicked()
        {
            model.LoadNewData();
        }
        public void OpenBt_Clicked()
        {
            model.OpenFile();
        }
        public void SaveAsBt_Clicked()
        {
            model.SaveAs();
        }
        public void SaveBt_Clicked()
        {
            model.Save();
        }

        public void VersionsList_SelectionChanged()
        {
            model.LoadToConfigList();
        }
        public void ConfigList_SelectionChanged()
        {
            model.SetConfigProperty();
        }
        public void ValueList_SelectionChanged()
        {
            model.ChangeValue(ConfigType.Combo);
        }
        public void ValueTextText_Changed()
        {
            model.ChangeValue(ConfigType.String);
        }
        #endregion
    }
}
