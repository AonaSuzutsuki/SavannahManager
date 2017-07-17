using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm.Models
{
    public class ModelBase : BindableBase
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(object sender, [CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsTelnetLoading { get; protected set; } = false;
        public bool IsDeactivated { get; set; } = false;

        private SolidColorBrush aroundBorderColor = StaticData.ActivatedBorderColor;
        public SolidColorBrush AroundBorderColor
        {
            get => aroundBorderColor;
            set => SetProperty(ref aroundBorderColor, value);
        }
        private double aroundBorderOpacity = 1.0;
        public double AroundBorderOpacity
        {
            get => aroundBorderOpacity;
            set => SetProperty(ref aroundBorderOpacity, value);
        }

        private double width;
        public double Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }
        private double height;
        public double Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }
        private double top;
        public double Top
        {
            get => top;
            set => SetProperty(ref top, value);
        }
        private double left;
        public double Left
        {
            get => left;
            set => SetProperty(ref left, value);
        }
    }
}
