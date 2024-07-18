using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using PlaylistQuickAdd.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlaylistQuickAdd.ViewModels
{
    public class PlaylistsViewModel : ObservableObject, IViewModel
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

        public List<Playlist> Playlists
        {
            get => Spotify.PlaylistsWithImages; set
            {
                if (Spotify.PlaylistsWithImages != value)
                {
                    Spotify.PlaylistsWithImages = value;
                    OnPropertyChanged();
                }
            }
        }

        public PlaylistsViewModel()
        {
            SetupSharedDataService();
            LoadPlaylists();
        }

        private async Task LoadPlaylists()
        {
            Playlists = [];

            foreach (var playlist in Spotify.Client.Playlists.CurrentUsers().Result.Items)
            {
                var newPlaylist = new Playlist(playlist.Name);

                var image = new Image();

                if (playlist.Images.Any())
                    image.Source = new BitmapImage(new Uri(playlist.Images[0].Url)); // TEMP
                else
                    image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"));

                newPlaylist.PlaylistCover = image.Source;
                Playlists.Add(newPlaylist);
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
