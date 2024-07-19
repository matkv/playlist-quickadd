using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PlaylistQuickAdd.Models;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlaylistQuickAdd.ViewModels
{
    internal class HomeViewModel : ObservableObject, IViewModel
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

        public string LoggedInUserText
        {
            get => Spotify.LoggedInUser; set
            {
                if (Spotify.LoggedInUser != value)
                {
                    Spotify.LoggedInUser = value;
                }

                OnPropertyChanged();
            }
        }

        private string tempCurrentlyPlaying;
        public string CurrentlyPlaying
        {
            get => tempCurrentlyPlaying;
            set
            {
                if (tempCurrentlyPlaying != value)
                {
                    tempCurrentlyPlaying = value;
                }
                OnPropertyChanged();
            }
        }

        public async Task InitializeAsync()
        {
            await ShowUserData();
            await ShowSpotifyPlayer();
        }

        public HomeViewModel()
        {
            SetupSharedDataService();
            InitializeAsync().ConfigureAwait(false);
        }

        private async Task ShowUserData()
        {
            if (Spotify.Client != null)
            {
                PrivateUser profile = await Spotify.Client.UserProfile.Current();
                Spotify.LoggedInUser = profile.DisplayName;

                LoggedInUserText = $"The currently logged in user is {Spotify.LoggedInUser}";
            }
        }

        private async Task ShowSpotifyPlayer()
        {
            var currentlyPlaying = await Spotify.Client.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.All));

            FullTrack track = currentlyPlaying?.Item as FullTrack;
            if (track != null)
            {
                CurrentlyPlaying = $"Currently playing: {track.Name} by {track.Artists[0].Name}";
            }
        }

        public void SetupSharedDataService()
        {
            var app = (App)Application.Current;

            var serviceProvider = app.ServiceProvider;
            sharedDataService = serviceProvider.GetService<SharedDataService>();
        }
    }
}
