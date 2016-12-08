using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels 
{
    class PlaybackStateToButtonIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is MediaPlaybackState)
            { 
                Image img = new Image();
                Uri uri;

                var state = (MediaPlaybackState)value;

                if (state == MediaPlaybackState.Playing)
                {
                    //img.Source = new BitmapImage(new Uri("ms-appx:///Assets/PauseButton.png"));
                    //return img;
                    uri = new Uri("ms-appx:///Assets/PauseButton.png");
                    return uri;
                }
                else
                {
                    //img.Source = new BitmapImage(new Uri("ms-appx:///Assets/PlayButton.png"));
                    //return img;
                    uri = new Uri("ms-appx:///Assets/PlayButton.png");
                    return uri;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
