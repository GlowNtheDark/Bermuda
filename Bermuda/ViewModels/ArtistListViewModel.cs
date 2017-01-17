using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Bermuda.ViewModels
{
    public class ArtistListViewModel : ObservableCollection<ArtistViewModel>, IDisposable
    {
        public CoreDispatcher dispatcher;
        MessagingViewModel MessageViewModel;
        bool disposed;
        bool initializing;
        ColorListViewModel colorlistviewmodel;

        public ArtistListViewModel(SearchResponse response, CoreDispatcher dispatcher, MessagingViewModel MessageViewModel, ColorListViewModel colorlistviewmodel)
        {
            try
            {
                this.dispatcher = dispatcher;
                this.MessageViewModel = MessageViewModel;
                this.colorlistviewmodel = colorlistviewmodel;
                // Initialize the view model items
                initializing = true;

                foreach (SearchResult result in response.Entries)
                {
                    if(result.Artist != null)
                        Add(new ArtistViewModel(this, result.Artist, MessageViewModel, colorlistviewmodel[colorlistviewmodel.index].Color));
                }

                initializing = false;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Unexpected error -- " + ex));
                MessageViewModel.ShowAlert();
            }
        }

        public void Dispose()
        {
            MessageViewModel = null;
            colorlistviewmodel = null;
            disposed = true;
        }


    }
}
