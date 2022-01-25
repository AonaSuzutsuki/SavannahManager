using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using System.Windows.Input;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommonStyleLib.Views;
using ConfigEditor_mvvm.Models;
using Reactive.Bindings.Extensions;
using Prism.Commands;
using SavannahManagerStyleLib.Models.SshFileSelector;
using SavannahManagerStyleLib.ViewModels.SshFileSelector;
using SavannahManagerStyleLib.Views.SshFileSelector;

namespace ConfigEditor_mvvm.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindowModel _model;
        public MainWindowViewModel(WindowService windowService, MainWindowModel model) : base(windowService, model)
        {
            _model = model;

            #region Event Initialize
            Loaded = new DelegateCommand(MainWindow_Loaded);
            KeyDown = new DelegateCommand<KeyEventArgs>(MainWindow_KeyDown);

            NewFileBtClicked = new DelegateCommand(NewFileBt_Clicked);
            OpenBtClicked = new DelegateCommand(OpenBt_Clicked);
            OpenSftpCommand = new DelegateCommand(OpenSftp);
            SaveAsBtClicked = new DelegateCommand(SaveAsBt_Clicked);
            SaveBtClicked = new DelegateCommand(SaveBt_Clicked);
            SaveAsSftpCommand = new DelegateCommand(SaveAsSftp);

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
        public ICommand NewFileBtClicked { get; }
        public ICommand OpenBtClicked { get; }
        public ICommand OpenSftpCommand { get; }
        public ICommand SaveAsBtClicked { get; }
        public ICommand SaveBtClicked { get; }
        public ICommand SaveAsSftpCommand { get; }

        public ICommand VersionsListSelectionChanged { get; }
        public ICommand ConfigListSelectionChanged { get; }
        public ICommand ValueListSelectionChanged { get; }

        public ICommand ValueTextTextChanged { get; }
        #endregion

        #region Event Methods
        protected override void MainWindow_KeyDown(KeyEventArgs e)
        {
            _model.ShortcutKey(e, Keyboard.Modifiers);
        }

        public void NewFileBt_Clicked()
        {
            _model.LoadNewData();
        }
        public void OpenBt_Clicked()
        {
            _model.OpenFile();
        }
        public void SaveAsBt_Clicked()
        {
            _model.SaveAs();
        }
        public void SaveBt_Clicked()
        {
            _model.Save();
        }

        public void OpenSftp()
        {
            var model = new FileSelectorModel();
            var vm = new FileSelectorViewModel(new WindowService(), model)
            {
                Mode = FileSelectorMode.Open,
                IsNewConnection = true
            };
            model.OpenCallbackAction = item =>
            {
                using var stream = item.Stream;

                _model.OpenFileViaSftp(stream);
            };

            WindowManageService.ShowDialog<FileSelectorView>(vm);
        }

        public void SaveAsSftp()
        {
            var model = new FileSelectorModel();
            var vm = new FileSelectorViewModel(new WindowService(), model)
            {
                Mode = FileSelectorMode.SaveAs,
                IsNewConnection = true
            };
            model.SaveDataFunction = () =>
            {
                using var stream = _model.CreateConfigXml();
                return stream.ToArray();
            };

            WindowManageService.ShowDialog<FileSelectorView>(vm);
        }

        public void VersionsList_SelectionChanged()
        {
            _model.VersionListSelectionChanged();
        }
        public void ConfigList_SelectionChanged()
        {
            _model.SetConfigProperty();
        }
        public void ValueList_SelectionChanged()
        {
            _model.ChangeValue(ConfigType.Combo);
        }
        public void ValueTextText_Changed()
        {
            _model.ChangeValue(ConfigType.String);
        }
        #endregion
    }
}
