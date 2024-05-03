using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PlaylistQuickAdd
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<Playlist> Playlists { get; } = new ObservableCollection<Playlist>();

        public MainWindow()
        {
            this.InitializeComponent();

            _ = ConnectToSpotifyAsync();

            Playlists.Add(new Playlist("Playlist 1"));
            Playlists.Add(new Playlist("Playlist 2"));
            Playlists.Add(new Playlist("Playlist 3"));
        }

        private async Task ConnectToSpotifyAsync()
        {
            var authorization = new Authorization();
            var token = await authorization.GetSpotifyAccessToken();
            AccessTokenTextBlock.Text = token.AccessToken;
        }

    }
}
