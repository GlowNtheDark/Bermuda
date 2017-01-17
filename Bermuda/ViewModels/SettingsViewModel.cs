using Bermuda.DataModels;
using Bermuda.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Bermuda.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged, IDisposable
    {
        public ColorListViewModel colorlistviewmodel;

        public int selectedIndex { get; set; }

        public SettingsViewModel()
        {
            colorlistviewmodel = new ColorListViewModel();
        }

        public void accentChanged(object sender, object e)
        {
            ComboBox cb = sender as ComboBox;
            int index = cb.SelectedIndex;
            AppSettings.localSettings.Values["accentColor"] = index.ToString();
            PlayerService.Instance.updateTheme();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            colorlistviewmodel = null;
        }
    }
}
