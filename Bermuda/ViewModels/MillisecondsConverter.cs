using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Bermuda.ViewModels
{
    class MillisecondsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                TimeSpan t = TimeSpan.FromMilliseconds((double)value);
                // The value parameter is the data from the source object.
                string time = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);

                // Return the uri value to pass to the target.
                return time;
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
