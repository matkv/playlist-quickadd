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

internal class QuickAddViewModel : ObservableObject, IViewModel
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

    public ObservableCollection<Track> Tracks
    {
        get => Spotify.SavedTracks; set
        {
            if (Spotify.SavedTracks == value) return;
            Spotify.SavedTracks = value;
            OnPropertyChanged();
        }
    }

    public QuickAddViewModel()
    {
        SetupSharedDataService();
        _ = InitializeQuickAdd();
    }

    private async Task InitializeQuickAdd()
    {
        await LoadSavedSongs();
    }

    private async Task LoadSavedSongs()
    {
        if (Tracks == null)
            Tracks = [];
        else if (Tracks.Count > 0)
            return;

        var tracks = await Spotify.Client.Library.GetTracks();

        if (tracks.Items == null) return;
        foreach (var track in tracks.Items)
        {
            var newTrack = new Track
            {
                ID = track.Track.Id,
                Title = track.Track.Name,
                Artist = track.Track.Artists[0].Name
            };

            var image = new Image
            {
                Source = track.Track.Album.Images.Count > 0
                    ? new BitmapImage(new Uri(track.Track.Album.Images[0].Url))
                    : new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png")) // TEMP
            };

            newTrack.AlbumCover = image.Source;

            Tracks.Add(newTrack);
        }
    }

    public void SetupSharedDataService()
    {
        var app = (App)Application.Current;

        var serviceProvider = app.ServiceProvider;
        _sharedDataService = serviceProvider.GetService<SharedDataService>();
    }
}