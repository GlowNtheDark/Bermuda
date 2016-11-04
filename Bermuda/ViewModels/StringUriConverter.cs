using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Bermuda.ViewModels
{
    class StringUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var newvalue = value as List<ArtReference>;
            // The value parameter is the data from the source object.
            string url = (string)newvalue[0].Url;

            Uri uri = new Uri(url);

            // Return the uri value to pass to the target.
            return uri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
