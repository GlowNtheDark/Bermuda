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
    public class MainMenuViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        MediaPlayer player;
        public bool isEnabledNP;
        public bool isEnabledQP;
        public bool isEnabledSR;
        public bool isEnabledPL;
        CoreDispatcher dispatcher;

        public MainMenuViewModel(MediaPlayer player, CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.player = player;

            player.SourceChanged += Player_SourceChanged;
            
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

        public bool IsEnabledQP
        {
            get { return isEnabledQP; }

            set
            {
                if (isEnabledQP != value)
                {
                    isEnabledQP = value;

                    Update();
                }
            }
        }

        public bool IsEnabledSR
        {
            get { return isEnabledSR; }

            set
            {
                if (isEnabledSR != value)
                {
                    isEnabledSR = value;

                    Update();
                }
            }
        }

        public bool IsEnabledPL
        {
            get { return isEnabledPL; }

            set
            {
                if (isEnabledPL != value)
                {
                    isEnabledPL = value;

                    Update();
                }
            }
        }

        async void Update()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RaisePropertyChanged("IsEnabledNP");
                RaisePropertyChanged("IsEnabledQP");
                RaisePropertyChanged("IsEnabledSR");
                RaisePropertyChanged("IsEnabledPL");
                RaisePropertyChanged("MLViewModel");
                RaisePropertyChanged("AlertVisibility");
            });
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            player.SourceChanged -= Player_SourceChanged;
        }
    }
}
