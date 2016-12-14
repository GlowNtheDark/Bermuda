using Bermuda.DataModels;
using Bermuda.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Bermuda.ViewModels
{
    class MainMenuViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        MediaPlayer player;
        public bool isEnabledNP;
        CoreDispatcher dispatcher;

        public MainMenuViewModel(MediaPlayer player, CoreDispatcher dispatcher)
        {
            this.player = player;
            player.SourceChanged += Player_SourceChanged;
            this.dispatcher = dispatcher;
            if (player.Source == null)
                IsEnabledNP = false;
            else
                IsEnabledNP = true;
        }

        private void Player_SourceChanged(MediaPlayer sender, object args)
        {
            if (player.Source != null)
                IsEnabledNP = true;
        }

        public bool IsEnabledNP
        {
            get { return isEnabledNP; }

            set
            {
                if (isEnabledNP != value)
                {
                    isEnabledNP = value;

                    Update();
                }
            }
        }

        async void Update()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RaisePropertyChanged("IsEnabledNP");
            });
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
