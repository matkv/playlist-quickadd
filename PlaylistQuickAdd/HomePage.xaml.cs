using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PlaylistQuickAdd
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public ObservableCollection<Playlist> Playlists { get; set; }

        public HomePage()
        {
            this.InitializeComponent();
            CreateSamplePlaylists();
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

        private void LoginSpotify(object sender, RoutedEventArgs e)
        {
            _ = ConnectToSpotifyAsync();
        }

        private async Task ConnectToSpotifyAsync()
        {
            var authorization = new Authorization();
            var token = await authorization.GetSpotifyAccessToken();
            AccessTokenTextBlock.Text = token.AccessToken;

            string authorizationCode = await Authorization.Login();

            if (authorizationCode != null)
            {
                //var accessToken = await authorization.GetAccessToken();
                //AccessTokenTextBlock.Text = accessToken.AccessToken;
            }
        }
    }
}
