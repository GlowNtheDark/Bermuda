using Bermuda.DataModels;
using Bermuda.Services;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        CoreDispatcher dispatcher;
        MediaPlayer Player;
        PlaylistGroupViewModel groupviewmodel;
        PlaylistGroup listgroup;
        bool disposed;
        bool initializing;

        public event PropertyChangedEventHandler PropertyChanged;


        public PlaylistViewModel(MediaPlayer player, CoreDispatcher dispatcher)
        {
            this.Player = player;
            this.dispatcher = dispatcher;
        }

        public PlaylistGroupViewModel GroupViewModel
        {
            get { return groupviewmodel; }

            set
            {
                if (groupviewmodel != value)
                {
                    groupviewmodel = value;
                    RaisePropertyChanged("GroupViewModel");
                }
            }
        }


        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {

        }

    }
}
