using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PlaylistQuickAdd.Models;
using System;
using System.Collections.Generic;


namespace PlaylistQuickAdd.ViewModels
{
    internal class QuickAddViewModel : ObservableObject, IViewModel
    {
        public SharedDataService sharedDataService;

        public Spotify Spotify
        {
            get => sharedDataService.Spotify; set
            {
                if (sharedDataService.Spotify != value)
                {
                    sharedDataService.Spotify = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Track> Tracks
        {
            get => Spotify.SavedTracks; set
            {
                if (Spotify.SavedTracks != value)
                {
                    Spotify.SavedTracks = value;
                    OnPropertyChanged();
                }
            }
        }

        public QuickAddViewModel()
        {
            SetupSharedDataService();
            LoadSavedSongs();
        }

        private void LoadSavedSongs()
        {

        }

        public void SetupSharedDataService()
        {
            var app = (App)Application.Current;

            var serviceProvider = app.ServiceProvider;
            sharedDataService = serviceProvider.GetService<SharedDataService>();
        }
    }
}
