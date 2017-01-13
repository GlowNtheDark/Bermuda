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

        public ColorItemViewModel(SolidColorBrush color)
        {
            this.Color = color;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    
    }
}
