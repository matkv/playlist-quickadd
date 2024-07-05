using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlaylistQuickAdd.Models;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlaylistQuickAdd.ViewModels
{
    internal class HomeViewModel : ObservableObject
    {

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

        private Authorization authorization;

        private SpotifyUser loggedInUser;

        public HomeViewModel()
        {
            authorization = new Authorization();

            LoginSpotifyCommand = new AsyncRelayCommand(LoginSpotify);
            LoadPlaylistsCommand = new AsyncRelayCommand(LoadPlaylists);
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
    }
}
