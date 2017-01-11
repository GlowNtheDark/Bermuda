using Bermuda.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bermuda.ViewModels
{
    public class MessageItemViewModel : INotifyPropertyChanged
    {
        public string Message { get; private set; }

        MessageList List;

        public MessageItemViewModel(String message, MessageList list)
        {
            this.Message = message;
            this.List = list;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
