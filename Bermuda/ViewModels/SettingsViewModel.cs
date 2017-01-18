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
            if (AppSettings.localSettings.Values["accentColor"] != null)
            {
                int index = 12;

                ComboBox cb = sender as ComboBox;

                if (cb.SelectedIndex > -1)
                    index = cb.SelectedIndex;

                AppSettings.localSettings.Values["accentColor"] = index.ToString();
                PlayerService.Instance.updateTheme();
            }

            else
            {
                int index = 12;

                AppSettings.localSettings.Values["accentColor"] = index.ToString();
                PlayerService.Instance.updateTheme();
            }

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
