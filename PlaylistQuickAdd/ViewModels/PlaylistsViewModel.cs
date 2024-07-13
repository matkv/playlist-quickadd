using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using PlaylistQuickAdd.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlaylistQuickAdd.ViewModels
{
    public class PlaylistsViewModel : ObservableObject
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
        public ICommand LoadPlaylistsCommand { get; private set; }
        private ObservableCollection<Playlist> playlists;

        public PlaylistsViewModel()
        {

            LoadPlaylistsCommand = new AsyncRelayCommand(LoadPlaylists);
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

        private async Task LoadPlaylists()
        {
            return; // TODO rewrite with SpotifyAPI-NET and then check again
            //if (loggedInUser != null)
            //    playlists = await loggedInUser.GetPlaylists();
        }
    }
}
