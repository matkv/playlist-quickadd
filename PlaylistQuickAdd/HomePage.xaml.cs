using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
            ];
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

            await authorization.Login();
        }
    }
}
