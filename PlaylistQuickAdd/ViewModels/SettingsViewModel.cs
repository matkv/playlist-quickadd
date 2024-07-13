using CommunityToolkit.Mvvm.ComponentModel;
using PlaylistQuickAdd.Models;

namespace PlaylistQuickAdd.ViewModels
{
    public class SettingsViewModel : ObservableObject
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
            
        }
    }
}
