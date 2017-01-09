using Bermuda.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Bermuda.Services
{
    public class MessagingService
    {
        static MessagingService instance;

        public CoreDispatcher dispatcher;

        public MessageList Messagelist { get; set; }

        public bool isNewAlert { get; set; }

        public static MessagingService Instance
        {
            get
            {
                if (instance == null)
                    instance = new MessagingService();

                return instance;
            }
        }

        public MessagingService()
        {
            Messagelist = new MessageList();
            isNewAlert = false;
        }
    }
}
