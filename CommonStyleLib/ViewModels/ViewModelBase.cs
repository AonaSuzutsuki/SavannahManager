using Prism.Commands;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommonLib.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        protected Window view;
        private Models.ModelBase modelBase;
        public ViewModelBase(Window view, Models.ModelBase modelBase)
        {
            this.view = view;
            this.modelBase = modelBase;

            Activated = new DelegateCommand(MainWindow_Activated);
            Deactivated = new DelegateCommand(MainWindow_Deactivated);
            ImageMouseDown = new DelegateCommand<MouseEventArgs>(Image_MouseDown);
            MainWindowMinimumBTClick = new DelegateCommand(MainWindowMinimumBT_Click);
            MainWindowMaximumBTClick = new DelegateCommand(MainWindowMaximumBT_Click);
            MainWindowCloseBTClick = new DelegateCommand(MainWindowCloseBT_Click);
            StateChanged = new DelegateCommand(MainWindow_StateChanged);

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
            set => SetProperty(ref mainWindowMinimumBTContent, value);
            get => mainWindowMinimumBTContent;
        }
        protected string mainWindowMaximumBTContent = "1";
        public string MainWindowMaximumBTContent
        {
            set => SetProperty(ref mainWindowMaximumBTContent, value);
            get => mainWindowMaximumBTContent;
        }
        protected string mainWindowCloseBTContent = "r";
        public string MainWindowCloseBTContent
        {
            set => SetProperty(ref mainWindowCloseBTContent, value);
            get => mainWindowCloseBTContent;
        }

        protected Thickness mainWindowMargin = zeroMargin;
        public Thickness MainWindowMargin
        {
            set => SetProperty(ref mainWindowMargin, value);
            get => mainWindowMargin;
        }
        #endregion

        #region EventMethods
        protected void MainWindow_Activated()
        {
            modelBase.Activated();
        }
        protected void MainWindow_Deactivated()
        {
            modelBase.Deactivated();
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
