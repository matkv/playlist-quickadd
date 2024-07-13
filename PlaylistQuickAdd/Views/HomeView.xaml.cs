using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using PlaylistQuickAdd.Models;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PlaylistQuickAdd
{
    /// <summary>
    /// An empty page that can be used on its own or navi��gated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : Page
    {

        public HomeView()
        {
            this.InitializeComponent();
            SetupSharedDataService();
        }

        private void SetupSharedDataService()
        {
            var app = (App)Application.Current;

            var serviceProvider = app.ServiceProvider;
            this.HomeViewModel.sharedDataService = serviceProvider.GetService<SharedDataService>();
        }
    }
}
