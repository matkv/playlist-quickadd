using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PlaylistQuickAdd.Models;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace PlaylistQuickAdd.ViewModels;

internal class HomeViewModel : ObservableObject, IViewModel
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

    private string tempCurrentlyPlaying;
    public string CurrentlyPlaying
    {
        get => tempCurrentlyPlaying;
        set
        {
            if (tempCurrentlyPlaying != value)
            {
                tempCurrentlyPlaying = value;
            }
            OnPropertyChanged();
        }
    }

    public async Task InitializeAsync()
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
            var currentlyPlaying = await Spotify.Client.Player.GetCurrentlyPlaying(new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.All));

            var track = currentlyPlaying?.Item as FullTrack;
            if (track != null)
            {
                CurrentlyPlaying = $"Currently playing: {track.Name} by {track.Artists[0].Name}";
            }
        }
    }

    public void SetupSharedDataService()
    {
        var app = (App)Application.Current;

        var serviceProvider = app.ServiceProvider;
        _sharedDataService = serviceProvider.GetService<SharedDataService>();
    }
}