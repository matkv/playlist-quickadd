using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PlaylistQuickAdd.Models;
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
                    OnPropertyChanged(); 
                }
            }
        }

        public List<string> Playlists
        {

            get => Spotify.Playlists; set
            {
                if (Spotify.Playlists != value)
                {
                    Spotify.Playlists = value;
                    OnPropertyChanged();
                }
            }
        }

        public HomeViewModel()
        {
            SetupSharedDataService();
            ShowUserData();
        }

        private async Task ShowUserData()
        {
            if (Spotify.Client != null)
            {
                SpotifyAPI.Web.PrivateUser profile = await Spotify.Client.UserProfile.Current();
                Spotify.LoggedInUser = profile.DisplayName;

                LoggedInUserText = Spotify.LoggedInUser;

                Playlists = Spotify.Client.Playlists.CurrentUsers().Result.Items.ConvertAll(playlist => playlist.Name);
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
