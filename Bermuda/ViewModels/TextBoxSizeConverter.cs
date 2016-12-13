using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Bermuda.ViewModels
{
    class TextBoxSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                string thing = (string)value;

                if (thing == "1")
                    return (double)80;
                else
                    return (double)80;      
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
