using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using PlaylistQuickAdd.Models;
using PlaylistQuickAdd.ViewModels;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace PlaylistQuickAdd
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.CreateDefaultUI();
            await SetupSharedData();

            m_window = new MainWindow();            
            m_window.Activate();

            var test = m_window.DispatcherQueue;

            // Replace back with e.Arguments when https://github.com/microsoft/microsoft-ui-xaml/issues/3368 is fixed
            Microsoft.VisualStudio.TestPlatform.TestExecutor.UnitTestClient.Run(Environment.CommandLine);
        }

        private async Task SetupSharedData()
        {
            var serviceCollection = new ServiceCollection();

            var sharedDataService = new SharedDataService();
            await sharedDataService.LoginSpotify();

            serviceCollection.AddSingleton(sharedDataService);

            AddViewAndViewModels(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void AddViewAndViewModels(ServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<HomeView>();
            serviceCollection.AddTransient<PlaylistsView>();
            serviceCollection.AddTransient<SettingsView>();
            serviceCollection.AddTransient<QuickAddView>();

            serviceCollection.AddTransient<HomeViewModel>();
            serviceCollection.AddTransient<PlaylistsViewModel>();
            serviceCollection.AddTransient<SettingsViewModel>();
            serviceCollection.AddTransient<QuickAddViewModel>();
        }

        private Window m_window;
    }
}
