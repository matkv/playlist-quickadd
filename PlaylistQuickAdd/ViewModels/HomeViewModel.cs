using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using PlaylistQuickAdd.Models;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlaylistQuickAdd.ViewModels
{
    internal class HomeViewModel : ObservableObject
    {
        public ObservableCollection<Playlist> Playlists
        {
            get => playlists; set
            {
                if (playlists != value)
                {
                    playlists = value;
                    OnPropertyChanged();
                }
            }
        }

        public string AccessTokenClientText
        {
            get => authorization.AccessTokenClient; set
            {
                if (authorization.AccessTokenClient != value)
                {
                    authorization.AccessTokenClient = value;
                    OnPropertyChanged();
                }
            }
        }
        public string AccessTokenUserText
        {
            get => authorization.AccessTokenUser; set
            {
                if (authorization.AccessTokenUser != value)
                {
                    authorization.AccessTokenUser = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand LoginSpotifyCommand { get; private set; }
        public ICommand LoadPlaylistsCommand { get; private set; }

        private ObservableCollection<Playlist> playlists;
        private Authorization authorization;

        private SpotifyUser loggedInUser;

        public HomeViewModel()
        {
            authorization = new Authorization();

            LoginSpotifyCommand = new AsyncRelayCommand(LoginSpotify);
            LoadPlaylistsCommand = new AsyncRelayCommand(LoadPlaylists);

            CreateSamplePlaylists();
        }

        private async Task LoadPlaylists()
        {
            return; // TODO rewrite with SpotifyAPI-NET and then check again
            //if (loggedInUser != null)
            //    playlists = await loggedInUser.GetPlaylists();
        }

        private async Task LoginSpotify()
        {
            var token = await authorization.GetSpotifyAccessTokenForClient();

            AccessTokenClientText = token.AccessToken;

            string authorizationCode = await Authorization.Login();

            if (authorizationCode != null)
            {
                var accessTokenForUser = await authorization.GetSpotifyAccessTokenForUser(authorizationCode);
                AccessTokenUserText = accessTokenForUser.AccessToken;

                var userTest = await Authorization.GetSpotifyUser(accessTokenForUser.AccessToken);

                loggedInUser = JsonSerializer.Deserialize<SpotifyUser>(userTest);
                loggedInUser.UserAccessToken = accessTokenForUser; // TEMP
            }
        }

        private void CreateSamplePlaylists()
        {
            Playlists =
            [
                new Playlist("Playlist 1"),
                new Playlist("Playlist 2"),
                new Playlist("Playlist 3"),
                new Playlist("Playlist 4"),
                new Playlist("Playlist 5"),
                new Playlist("Playlist 6"),
                new Playlist("Playlist 7"),
                new Playlist("Playlist 8"),
            ];

            TempSetImagesForPlaylists();
        }

        private void TempSetImagesForPlaylists()
        {
            foreach (var playlist in Playlists)
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"))
                };

                playlist.PlaylistCover = image.Source;
            }
        }
    }
}
