using Prism.Mvvm;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace CommonLib.Models
{
    public class ModelBase : BindableBase
    {
        public new event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(object sender, [CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }
        
        public bool IsDeactivated { get; set; } = false;

        protected SolidColorBrush aroundBorderColor = ConstantValues.ActivatedBorderColor;
        public SolidColorBrush AroundBorderColor
        {
            get => aroundBorderColor;
            set => SetProperty(ref aroundBorderColor, value);
        }
        protected double aroundBorderOpacity = 1.0;
        public double AroundBorderOpacity
        {
            get => aroundBorderOpacity;
            set => SetProperty(ref aroundBorderOpacity, value);
        }

        public void SetBorderColor(SolidColorBrush color)
        {
            if (!IsDeactivated)
                AroundBorderColor = color;
        }
        public virtual void Activated()
        {
            IsDeactivated = false;
            AroundBorderColor = ConstantValues.ActivatedBorderColor;
        }
        public void Deactivated()
        {
            IsDeactivated = true;
            AroundBorderColor = ConstantValues.DeactivatedBorderColor;
        }

        protected double width;
        public double Width
        {
            get => width;
            set => SetProperty(ref width, value);
        }
        protected double height;
        public double Height
        {
            get => height;
            set => SetProperty(ref height, value);
        }
        protected double top;
        public double Top
        {
            get => top;
            set => SetProperty(ref top, value);
        }
        protected double left;
        public double Left
        {
            get => left;
            set => SetProperty(ref left, value);
        }
    }
}
