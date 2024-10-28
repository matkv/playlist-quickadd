using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PlaylistQuickAdd.Models;

namespace PlaylistQuickAdd.ViewModels;

public class SettingsViewModel : ObservableObject, IViewModel
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

    public SettingsViewModel()
    {
        SetupSharedDataService();            
    }

    public void SetupSharedDataService()
    {
        var app = (App)Application.Current;

        var serviceProvider = app.ServiceProvider;
        _sharedDataService = serviceProvider.GetService<SharedDataService>();
    }
}