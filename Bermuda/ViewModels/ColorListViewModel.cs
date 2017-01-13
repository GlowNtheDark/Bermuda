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
        public ColorListViewModel()
        {
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Blue)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Green)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Orange)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Pink)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Purple)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Red)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.White)));
            Add(new ColorItemViewModel(new SolidColorBrush(Colors.Yellow)));

        }

        public void Dispose()
        {

        }
    }
}
