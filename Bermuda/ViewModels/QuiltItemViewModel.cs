using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class QuiltItemViewModel : INotifyPropertyChanged
    {
        public BitmapImage image;

        public BitmapImage Image
        {
            get { return image; }
            set
            {
                if (image != value)
                {
                    image = value;
                    RaisePropertyChanged("Image");
                }
            }
        }

        public QuiltItemViewModel(BitmapImage image)
        {
            this.Image = image;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
