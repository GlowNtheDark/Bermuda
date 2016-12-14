using Bermuda.DataModels;
using GoogleMusicApi.UWP.Requests.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Bermuda.ViewModels
{
    public class QuickPlayListViewModel : ObservableCollection<ListenNowItemViewModel>, IDisposable
    {
        public CoreDispatcher dispatcher;
        public QuickPlayViewModel QPViewModel;

        public QuickPlayListViewModel(CoreDispatcher dispatcher, QuickPlayViewModel qpviewmodel)
        {
            this.dispatcher = dispatcher;
            this.QPViewModel = qpviewmodel;
            getListenNow();
        }

        private async void getListenNow()
        {

            try
            {
                ListListenNowTracksResponse listenNowResult = await NewMain.Current.mc.ListListenNowTracksAsync();

                if (listenNowResult != null)
                {
                    foreach (var item in listenNowResult.Items)
                    {
                        if(item != null)
                            Add(new ListenNowItemViewModel(item, QPViewModel));
                    }
                }

                else
                {

                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);

            }
        }

        public void Dispose()
        {

        }
    }
}
