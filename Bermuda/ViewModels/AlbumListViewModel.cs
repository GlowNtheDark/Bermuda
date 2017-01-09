using Bermuda.DataModels;
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
    public class AlbumListViewModel : ObservableCollection<AlbumViewModel>, IDisposable
    {
        public CoreDispatcher dispatcher;

        bool disposed;
        bool initializing;

        //public AlbumList AlbumList { get; private set; }

        public AlbumListViewModel(SearchResponse response, CoreDispatcher dispatcher)
        {
            try
            {
                this.dispatcher = dispatcher;

                // Initialize the view model items
                initializing = true;

                foreach (SearchResult result in response.Entries)
                {
                    if(result.Album != null)
                        Add(new AlbumViewModel(this, result.Album));
                }

                initializing = false;

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
