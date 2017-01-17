using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Bermuda.ViewModels
{
    public class ColorListViewModel : ObservableCollection<ColorItemViewModel>, IDisposable
    {
        public int index { get; set; }

        public ColorListViewModel()
        {
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Red)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Tomato)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Orange)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Yellow)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Chartreuse)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Green)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Turquoise)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Blue)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Purple)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Indigo)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Magenta)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Pink)));            
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.White)));

            
            if (AppSettings.localSettings.Values["accentColor"] != null)
            {
                string storedColor = AppSettings.localSettings.Values["accentColor"].ToString();
                index = int.Parse(storedColor);
            }

            else
                index = 12;
        }

        public void Dispose()
        {

        }
    }
}
