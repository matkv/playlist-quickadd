using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using PlaylistQuickAdd.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PlaylistQuickAdd.ViewModels;

public class PlaylistsViewModel : ObservableObject, IViewModel
{
    private SharedDataService _sharedDataService;

    public Spotify Spotify
    {
        get => _sharedDataService.Spotify; set
        {
            if (_sharedDataService.Spotify == value) return;
            _sharedDataService.Spotify = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Playlist> Playlists
    {
        get => Spotify.PlaylistsWithImages; set
        {
            if (Spotify.PlaylistsWithImages == value) return;
            Spotify.PlaylistsWithImages = value;
            OnPropertyChanged();
        }
    }

    public PlaylistsViewModel()
    {
        SetupSharedDataService();
        _ = InitializePlaylists();    
    }

    private async Task InitializePlaylists()
    {
        await LoadPlaylists();
    }

    private async Task LoadPlaylists()
    {
        if (Playlists == null)
            Playlists = [];
        else if (Playlists.Count > 0)
            return;

        var playlists = await Spotify.Client.Playlists.CurrentUsers();
            
        foreach (var playlist in playlists.Items)
        {
            var newPlaylist = new Playlist(playlist.Name);

            var image = new Image();

            if (playlist.Images?.Count > 0)
                image.Source = new BitmapImage(new Uri(playlist.Images[0].Url)); // TEMP
            else
                image.Source = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"));

            newPlaylist.PlaylistCover = image.Source;
            Playlists.Add(newPlaylist);
        }

        return;
    }

    public void SetupSharedDataService()
    {
        var app = (App)Application.Current;

        var serviceProvider = app.ServiceProvider;
        _sharedDataService = serviceProvider.GetService<SharedDataService>();
    }
}