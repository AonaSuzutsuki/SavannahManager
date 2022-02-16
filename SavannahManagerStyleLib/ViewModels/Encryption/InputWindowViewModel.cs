using System.Windows;
using System.Windows.Input;
using CommonStyleLib.Models;
using CommonStyleLib.ViewModels;
using CommonStyleLib.Views;
using Prism.Commands;
using Reactive.Bindings;

namespace SavannahManagerStyleLib.ViewModels.Encryption
{
    public class InputWindowViewModel : ViewModelBase
    {
        #region Constants

        public const int DefaultWidth = 300;
        public const int DefaultHeight = 250;

        #endregion

        #region Fields



        #endregion

        #region Event Properties

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public ICommand OpenOptionCommand { get; set; }

        #endregion

        #region Properties

        public bool IsCancel { get; set; } = true;

        public ReactiveProperty<string> Title { get; set; }
        public ReactiveProperty<string> Message { get; set; }
        public ReactiveProperty<string> InputText { get; set; }
        public ReactiveProperty<string> InputSaltText { get; set; }
        public ReactiveProperty<Visibility> OptionGridVisibility { get; set; }

        #endregion


        public InputWindowViewModel(IWindowService windowService, ModelBase model) : base(windowService, model)
        {
            InputText = new ReactiveProperty<string>("");
            InputSaltText = new ReactiveProperty<string>("");
            Title = new ReactiveProperty<string>("");
            Message = new ReactiveProperty<string>("");
            OptionGridVisibility = new ReactiveProperty<Visibility>(Visibility.Collapsed);

            OkCommand = new DelegateCommand(OkClick);
            CancelCommand = new DelegateCommand(CancelClick);
            OpenOptionCommand = new DelegateCommand(OpenOption);
        }

        public void OkClick()
        {
            IsCancel = false;
            MainWindowCloseBt_Click();
        }

        public void CancelClick()
        {
            MainWindowCloseBt_Click();
        }

        public void OpenOption()
        {
            if (OptionGridVisibility.Value == Visibility.Collapsed)
                OptionGridVisibility.Value = Visibility.Visible;
            else
                OptionGridVisibility.Value = Visibility.Collapsed;
        }
    }
}
