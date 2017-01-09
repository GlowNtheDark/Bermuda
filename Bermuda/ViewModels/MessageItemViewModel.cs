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

        public MessageItemViewModel(String message)
        {
            this.Message = message;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
