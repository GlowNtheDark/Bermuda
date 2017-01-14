using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Bermuda.ViewModels
{
    public class ColorItemViewModel : INotifyPropertyChanged
    {
        public SolidColorBrush Color { get; set; }
        public Byte r { get; set; }
        public Byte g { get; set; }
        public Byte b { get; set; }
        public Byte a { get; set; }

        public ColorItemViewModel(SolidColorBrush color)
        {
            this.Color = color;
            this.r = color.Color.R;
            this.g = color.Color.G;
            this.b = color.Color.B;
            this.a = color.Color.A;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
    }
}
