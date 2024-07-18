using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PlaylistQuickAdd.Models;

namespace PlaylistQuickAdd.ViewModels
{
    public class SettingsViewModel : ObservableObject, IViewModel
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

        public SettingsViewModel()
        {
            SetupSharedDataService();            
        }

        public void SetupSharedDataService()
        {
            var app = (App)Application.Current;

            var serviceProvider = app.ServiceProvider;
            sharedDataService = serviceProvider.GetService<SharedDataService>();
        }
    }
}
