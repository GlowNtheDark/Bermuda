using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class QuiltListViewModel : ObservableCollection<QuiltItemViewModel>, IDisposable
    {
        public QuiltListViewModel()
        {
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
