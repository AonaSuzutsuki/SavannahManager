using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommonStyleLib.ViewModels
{
    public class ViewModelBase : NotifyBase
    {
        protected Window view;
        private Models.ModelBase modelBase;
        public ViewModelBase(Window view, Models.ModelBase modelBase)
        {
            this.view = view;
            this.modelBase = modelBase;

            Activated = new RelayCommand(MainWindow_Activated);
            Deactivated = new RelayCommand(MainWindow_Deactivated);
            ImageMouseDown = new RelayCommand<MouseEventArgs>(Image_MouseDown);
            MainWindowMinimumBTClick = new RelayCommand(MainWindowMinimumBT_Click);
            MainWindowMaximumBTClick = new RelayCommand(MainWindowMaximumBT_Click);
            MainWindowCloseBTClick = new RelayCommand(MainWindowCloseBT_Click);
            StateChanged = new RelayCommand(MainWindow_StateChanged);

            AroundBorderColor = modelBase.ToReactivePropertyAsSynchronized(m => m.AroundBorderColor);
            AroundBorderOpacity = modelBase.ToReactivePropertyAsSynchronized(m => m.AroundBorderOpacity);
            Width = modelBase.ToReactivePropertyAsSynchronized(m => m.Width);
            Height = modelBase.ToReactivePropertyAsSynchronized(m => m.Height);
            Top = modelBase.ToReactivePropertyAsSynchronized(m => m.Top);
            Left = modelBase.ToReactivePropertyAsSynchronized(m => m.Left);
        }

        #region StaticMember
        private static readonly Thickness zeroMargin = new Thickness(0);
        private static readonly Thickness maximumMargin = new Thickness(7);
        #endregion

        #region EventProperties
        public ICommand Activated { get; set; }
        public ICommand Deactivated { get; set; }

        public ICommand ImageMouseDown
        {
            private set;
            get;
        }
        public ICommand MainWindowMinimumBTClick
        {
            private set;
            get;
        }
        public ICommand MainWindowMaximumBTClick
        {
            private set;
            get;
        }
        public ICommand MainWindowCloseBTClick
        {
            private set;
            get;
        }
        public ICommand StateChanged { get; set; }
        #endregion

        #region MainProperties
        public ReactiveProperty<double> Width { get; set; }
        public ReactiveProperty<double> Height { get; set; }
        public ReactiveProperty<double> Top { get; set; }
        public ReactiveProperty<double> Left { get; set; }

        public ReactiveProperty<SolidColorBrush> AroundBorderColor { get; set; }
        public ReactiveProperty<double> AroundBorderOpacity { get; set; }

        protected string mainWindowMinimumBTContent = "0";
        public string MainWindowMinimumBTContent
        {
            set
            {
                mainWindowMinimumBTContent = value;
                OnPropertyChanged(this);
            }
            get { return mainWindowMinimumBTContent; }
        }
        protected string mainWindowMaximumBTContent = "1";
        public string MainWindowMaximumBTContent
        {
            set
            {
                mainWindowMaximumBTContent = value;
                OnPropertyChanged(this);
            }
            get { return mainWindowMaximumBTContent; }
        }
        protected string mainWindowCloseBTContent = "r";
        public string MainWindowCloseBTContent
        {
            set
            {
                mainWindowCloseBTContent = value;
                OnPropertyChanged(this);
            }
            get { return mainWindowCloseBTContent; }
        }

        protected Thickness mainWindowMargin = zeroMargin;
        public Thickness MainWindowMargin
        {
            set
            {
                mainWindowMargin = value;
                OnPropertyChanged(this);
            }
            get
            {
                return mainWindowMargin;
            }
        }
        #endregion

        #region EventMethods
        protected void MainWindow_Activated()
        {
            modelBase.IsDeactivated = false;
            if (!modelBase.IsTelnetLoading)
            {
                AroundBorderColor.Value = StaticData.ActivatedBorderColor;
            }
            else
            {
                AroundBorderColor.Value = StaticData.ActivatedBorderColor2;
            }
        }
        protected void MainWindow_Deactivated()
        {
            modelBase.IsDeactivated = true;
            AroundBorderColor.Value = StaticData.DeactivatedBorderColor;
        }

        protected void Image_MouseDown(MouseEventArgs e)
        {
            Point p = view.PointToScreen(e.GetPosition(view));
            int x = (int)p.X;
            int y = (int)p.Y;
            Views.WindowCommands.ShowSystemMenu(view, x, y);
        }
        protected void MainWindowMinimumBT_Click()
        {
            view.WindowState = WindowState.Minimized;
        }
        protected void MainWindowMaximumBT_Click()
        {
            if (view.WindowState != WindowState.Maximized)
            {
                view.WindowState = WindowState.Maximized;
                MainWindowMaximumBTContent = "2";
                MainWindowMargin = maximumMargin;
            }
            else
            {
                view.WindowState = WindowState.Normal;
                MainWindowMaximumBTContent = "1";
                MainWindowMargin = zeroMargin;
            }
        }
        protected void MainWindowCloseBT_Click()
        {
            view.Close();
        }
        private void MainWindow_StateChanged()
        {
            if (view.WindowState == WindowState.Maximized)
            {
                MainWindowMaximumBTContent = "2";
                MainWindowMargin = maximumMargin;
            }
            else
            {
                MainWindowMaximumBTContent = "1";
                MainWindowMargin = zeroMargin;
            }
        }
        #endregion
    }
}
