using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlaylistQuickAdd.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlaylistQuickAdd.ViewModels
{
    internal class HomeViewModel : ObservableObject
    {
        public string LoggedInUserText
        {
            get => spotify.LoggedInUser; set
            {
                if (spotify.LoggedInUser != value)
                {
                    spotify.LoggedInUser = value;
                }

                OnPropertyChanged(); // TODO check why it doesn't work as it should when inside the if clause
            }
        }

        public List<string> Playlists
        {

            get => spotify.Playlists; set
            {
                if (spotify.Playlists != value)
                {
                    spotify.Playlists = value;
                }

                OnPropertyChanged(); // TODO check why it doesn't work as it should when inside the if clause
            }

        }

        public ICommand LoginSpotifyCommand { get; private set; }
        public ICommand ShowUserDataCommand { get; private set; }

        private Spotify spotify;

        public HomeViewModel()
        {
            spotify = new Spotify();

            LoginSpotifyCommand = new AsyncRelayCommand(LoginSpotify);
            ShowUserDataCommand = new RelayCommand(ShowUserData);
        }

        private async Task LoginSpotify()
        {
            await spotify.Login();
        }

        private async void ShowUserData()
        {
            if (spotify.Client != null)
            {
                SpotifyAPI.Web.PrivateUser profile = await spotify.Client.UserProfile.Current();
                spotify.LoggedInUser = profile.DisplayName;

                LoggedInUserText = spotify.LoggedInUser;

                Playlists = spotify.Client.Playlists.CurrentUsers().Result.Items.ConvertAll(playlist => playlist.Name);
            }
        }
    }
}
