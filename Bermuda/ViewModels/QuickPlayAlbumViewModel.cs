﻿using GoogleMusicApi.UWP.Requests.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Bermuda.ViewModels
{
    public class QuickPlayAlbumViewModel : ObservableCollection<ListenNowItemViewModel>, IDisposable
    {
        public CoreDispatcher dispatcher;
        public QuickPlayViewModel QPViewModel;

        public QuickPlayAlbumViewModel(CoreDispatcher dispatcher, QuickPlayViewModel qpviewmodel)
        {
            this.dispatcher = dispatcher;
            this.QPViewModel = qpviewmodel;
        }

        public void Dispose()
        {
            QPViewModel = null;
        }
    }
}
    
