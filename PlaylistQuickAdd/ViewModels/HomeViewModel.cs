using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PlaylistQuickAdd.Models;
using SpotifyAPI.Web;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Image = Microsoft.UI.Xaml.Controls.Image;

namespace PlaylistQuickAdd.ViewModels;

internal class HomeViewModel : ObservableObject, IViewModel
{
    private SharedDataService _sharedDataService;
    private DispatcherTimer _refreshTimer;
    
    public Spotify Spotify
    {
        get => _sharedDataService.Spotify; set
        {
            if (_sharedDataService.Spotify == value) return;
            _sharedDataService.Spotify = value;
            OnPropertyChanged();
        }
    }

    public string LoggedInUserText
    {
        get => Spotify.LoggedInUser; set
        {
            if (Spotify.LoggedInUser != value)
            {
                Spotify.LoggedInUser = value;
            }

            OnPropertyChanged();
        }
    }

    private string _tempCurrentlyPlaying;
    public string CurrentlyPlaying
    {
        get => _tempCurrentlyPlaying;
        set
        {
            if (!string.Equals(_tempCurrentlyPlaying, value, StringComparison.Ordinal))
            {
                _tempCurrentlyPlaying = value;
            }
            OnPropertyChanged();
        }
    }

    private ImageSource _currentAlbumCover;

    public ImageSource CurrentAlbumCover
    {
        get => _currentAlbumCover;
        
        set
        {
            if (_currentAlbumCover != value)
                _currentAlbumCover = value;
            
            OnPropertyChanged();
        }
    }

    private async Task InitializeAsync()
    {
        await ShowUserData();
        await ShowSpotifyPlayer();
    }

    public HomeViewModel()
    {
        SetupSharedDataService();
        InitializeAsync().ConfigureAwait(false);
    }

    private async Task ShowUserData()
    {
        if (Spotify.Client != null)
        {
            var profile = await Spotify.Client.UserProfile.Current();
            Spotify.LoggedInUser = profile.DisplayName;

            LoggedInUserText = $"The currently logged in user is {Spotify.LoggedInUser}";
        }
    }

    private async Task ShowSpotifyPlayer()
    {
        if (Spotify.Client != null)
        {
            var currentlyPlaying = await Spotify.Client.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest());

            if (currentlyPlaying?.Item is FullTrack track)
            {
                CurrentlyPlaying = $"Currently playing: {track.Name} by {track.Artists[0].Name}";

                var image = new Image
                {
                    Source = track.Album.Images?.Count > 0 ? new BitmapImage(new Uri(track.Album.Images[0].Url)) 
                    : new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png")) // TEMP
                };
                
                CurrentAlbumCover = image.Source;
            }
        }
    }

    public void SetupSharedDataService()
    {
        var app = (App)Application.Current;

        var serviceProvider = app.ServiceProvider;
        _sharedDataService = serviceProvider.GetService<SharedDataService>();
    }

    public void StartTimer()
    {
        // TODO figure out if there is some way to get an event or something when the 
        // song changes instead of constantly pinging the API
        
        if (_refreshTimer == null)
        {
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _refreshTimer.Tick += async (sender, e) => await ShowSpotifyPlayer();
        }

        _refreshTimer.Start(); 
    }

    public void StopTimer()
    {
        if (_refreshTimer == null) return;
        _refreshTimer.Stop();
        _refreshTimer = null;
    }
}