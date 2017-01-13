using Bermuda.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Bermuda.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public ColorListViewModel colorlistviewmodel;

        public int selectedIndex { get; set; }

        public SettingsViewModel()
        {
            colorlistviewmodel = new ColorListViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {

        }
    }
}
