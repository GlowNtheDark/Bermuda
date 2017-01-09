using Bermuda.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bermuda.ViewModels
{
    public class MessageListViewModel : ObservableCollection<MessageItemViewModel>, IDisposable
    {
        public MessageListViewModel(MessageList list)
        {
            foreach (string text in list)
                Add(new MessageItemViewModel(text));
        }

        public void Dispose()
        {

        }
    }
}
