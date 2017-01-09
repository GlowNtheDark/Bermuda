﻿using Bermuda.DataModels;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchViewModel(MediaPlayer player, TrackList tracklist, CoreDispatcher dispatcher)
        {
            this.Player = player;
            this.SongList = tracklist;
            this.dispatcher = dispatcher;
            GridViewVisibility = Visibility.Collapsed;
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
            SearchResponse trackresponse = await NewMain.Current.mc.SearchAsync(args.QueryText, 1); //1 for Track Search

            SearchResponse artistresponse = await NewMain.Current.mc.SearchAsync(args.QueryText, 2); //2 for Artist Search

            SearchResponse albumresponse = await NewMain.Current.mc.SearchAsync(args.QueryText, 3); //3 for Album Search

            ALViewModel = new AlbumListViewModel(albumresponse, dispatcher);

            TLViewModel = new TrackListViewModel(trackresponse, dispatcher);

            ArLViewModel = new ArtistListViewModel(artistresponse, dispatcher);

            GridViewVisibility = Visibility.Visible;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {

        }
    }
}
