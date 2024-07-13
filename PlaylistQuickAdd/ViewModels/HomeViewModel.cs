using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using PlaylistQuickAdd.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlaylistQuickAdd.ViewModels
{
    internal class HomeViewModel : ObservableObject
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

        public ICommand ShowUserDataCommand { get; private set; }

        public HomeViewModel()
        {
            ShowUserDataCommand = new RelayCommand(ShowUserData);
        }

        private async void ShowUserData()
        {
            if (Spotify.Client != null)
            {
                SpotifyAPI.Web.PrivateUser profile = await Spotify.Client.UserProfile.Current();
                Spotify.LoggedInUser = profile.DisplayName;

                LoggedInUserText = Spotify.LoggedInUser;

                Playlists = Spotify.Client.Playlists.CurrentUsers().Result.Items.ConvertAll(playlist => playlist.Name);
            }
        }
    }
}
