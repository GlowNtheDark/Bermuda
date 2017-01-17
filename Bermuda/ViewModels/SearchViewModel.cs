using Bermuda.DataModels;
using GoogleMusicApi.UWP.Structure;
using System;
using System.ComponentModel;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bermuda.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged, IDisposable
    {
        MediaPlayer Player;
        TrackList SongList;
        CoreDispatcher dispatcher;
        AlbumListViewModel alviewmodel;
        TrackListViewModel tlviewmodel;
        ArtistListViewModel arlviewmodel;
        Visibility gridViewVisibility;
        MessagingViewModel MessageViewModel;
        public ColorListViewModel colorlistviewmodel;


        public event PropertyChangedEventHandler PropertyChanged;

        public SearchViewModel(MediaPlayer player, TrackList tracklist, CoreDispatcher dispatcher, MessagingViewModel MessageViewModel)
        {
            this.Player = player;
            this.SongList = tracklist;
            this.dispatcher = dispatcher;
            GridViewVisibility = Visibility.Collapsed;
            this.MessageViewModel = MessageViewModel;
            colorlistviewmodel = new ColorListViewModel();
        }

        public Visibility GridViewVisibility
        {
            get { return gridViewVisibility; }

            set
            {
                if (gridViewVisibility != value)
                {
                    gridViewVisibility = value;
                    RaisePropertyChanged("GridViewVisibility");
                }
            }
        }

        public AlbumListViewModel ALViewModel
        {
            get { return alviewmodel; }

            set
            {
                if (alviewmodel != value)
                {
                    alviewmodel = value;
                    RaisePropertyChanged("ALViewModel");
                }
            }
        }

        public TrackListViewModel TLViewModel
        {
            get { return tlviewmodel; }

            set
            {
                if (tlviewmodel != value)
                {
                    tlviewmodel = value;
                    RaisePropertyChanged("TLViewModel");
                }
            }
        }

        public ArtistListViewModel ArLViewModel
        {
            get { return arlviewmodel; }

            set
            {
                if (arlviewmodel != value)
                {
                    arlviewmodel = value;
                    RaisePropertyChanged("ArLViewModel");
                }
            }
        }

        public void AlbumItemClick(object sender, ItemClickEventArgs e)
        {
            AlbumViewModel item = e.ClickedItem as AlbumViewModel;
            item.openCloseMenu();
            RaisePropertyChanged("ALViewModel");
        }

        public void TrackItemClick(object sender, ItemClickEventArgs e)
        {
            TrackViewModel item = e.ClickedItem as TrackViewModel;
            item.openCloseMenu();
            RaisePropertyChanged("TLViewModel");
        }

        public void ArtistItemClick(object sender, ItemClickEventArgs e)
        {
            ArtistViewModel item = e.ClickedItem as ArtistViewModel;
            item.openCloseMenu();
            RaisePropertyChanged("ArLViewModel");
        }

        public async void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (ALViewModel != null)
                ALViewModel = null;
            if (ArLViewModel != null)
                ArLViewModel = null;
            if (TLViewModel != null)
                TLViewModel = null;

            SearchResponse trackresponse = await NewMain.Current.mc.SearchAsync(args.QueryText, 1); //1 for Track Search

            SearchResponse artistresponse = await NewMain.Current.mc.SearchAsync(args.QueryText, 2); //2 for Artist Search

            SearchResponse albumresponse = await NewMain.Current.mc.SearchAsync(args.QueryText, 3); //3 for Album Search

            if(albumresponse.Entries != null)
                ALViewModel = new AlbumListViewModel(albumresponse, dispatcher, MessageViewModel, colorlistviewmodel);

            else
            {
                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Part of that search came back null."));
                MessageViewModel.ShowAlert();
            }

            if (trackresponse.Entries != null)
                TLViewModel = new TrackListViewModel(trackresponse, dispatcher, MessageViewModel, colorlistviewmodel);

            else
            {
                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Part of that search came back null."));
                MessageViewModel.ShowAlert();
            }

            if (artistresponse.Entries != null)
                ArLViewModel = new ArtistListViewModel(artistresponse, dispatcher, MessageViewModel, colorlistviewmodel);

            else
            {
                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Part of that search came back null."));
                MessageViewModel.ShowAlert();
            }

            GridViewVisibility = Visibility.Visible;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            ALViewModel.Dispose();
            ALViewModel = null;
            ArLViewModel.Dispose();
            ArLViewModel = null;
            TLViewModel.Dispose();
            TLViewModel = null;
            SongList = null;
            MessageViewModel = null;
            colorlistviewmodel = null;
        }
    }
}
