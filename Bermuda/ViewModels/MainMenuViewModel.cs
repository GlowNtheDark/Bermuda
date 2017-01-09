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
    public class MainMenuViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        MediaPlayer player;
        public bool isEnabledNP;
        CoreDispatcher dispatcher;
        MessageListViewModel mlviewmodel;
        MessageList list;
        Visibility alertVisibility;

        public Visibility AlertVisibility
            {
                get { return alertVisibility; }

                set
                {
                if (alertVisibility != value)
                {
                    alertVisibility = value;

                    Update();
                }
            }
            }

        public MainMenuViewModel(MediaPlayer player, MessageList list, CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.player = player;
            this.list = list;

            MLViewModel = new MessageListViewModel(list);

            player.SourceChanged += Player_SourceChanged;
            
            if (player.Source == null)
                IsEnabledNP = false;
            else
                IsEnabledNP = true;

            if(MessagingService.Instance.isNewAlert)
                AlertVisibility = Visibility.Visible;
            else
                AlertVisibility = Visibility.Collapsed;
        }

        private void Player_SourceChanged(MediaPlayer sender, object args)
        {
            if (player.Source != null)
                IsEnabledNP = true;

            list.Add("Source Changed!");
            AlertVisibility = Visibility.Visible;
            MessagingService.Instance.isNewAlert = true;

            MLViewModel = null;
            MLViewModel = new MessageListViewModel(list);
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

        public MessageListViewModel MLViewModel
        {
            get { return mlviewmodel; }

            set
            {
                if (mlviewmodel != value)
                {
                    mlviewmodel = value;

                    Update();
                }
            }
        }

        async void Update()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RaisePropertyChanged("IsEnabledNP");
                RaisePropertyChanged("MLViewModel");
                RaisePropertyChanged("AlertVisibility");
            });
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
