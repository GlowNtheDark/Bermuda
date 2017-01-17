using Bermuda.DataModels;
using Bermuda.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Bermuda.ViewModels
{
    public class MessagingViewModel : INotifyPropertyChanged, IDisposable
    {
        MessageListViewModel mlviewmodel;
        MessageList mList;

        CoreDispatcher dispatcher;
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

        public MessagingViewModel(CoreDispatcher dispatcher, MessageList list)
        {
            this.dispatcher = dispatcher;
            this.mList = list;

            MLViewModel = new MessageListViewModel(list);

            if (MessagingService.Instance.isNewAlert)
                AlertVisibility = Visibility.Visible;
            else
                AlertVisibility = Visibility.Collapsed;
        }

        public void ShowAlert()
        {
            AlertVisibility = Visibility.Visible;
            MessagingService.Instance.isNewAlert = true;
            Update();
        }

        public void DismissAlert()
        {
            AlertVisibility = Visibility.Collapsed;
            MessagingService.Instance.isNewAlert = false;
            Update();
        }

        async void Update()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                RaisePropertyChanged("AlertVisibility");
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            MLViewModel = null;
        }
    }
}
