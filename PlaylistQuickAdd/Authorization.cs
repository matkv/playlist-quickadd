using Microsoft.Extensions.Configuration;
using System.IO;

namespace PlaylistQuickAdd
{
    internal class Authorization
    {
        private string _clientId;
        private string _clientSecret;

        public Authorization()
        {
            LoadConfigFromJSON();
        }

        private void LoadConfigFromJSON()
        {
            var configPath = Directory.GetCurrentDirectory(); // TODO not getting project root path
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.SetBasePath(configPath)
                .AddJsonFile("/Config/config.json", optional: false, reloadOnChange: true);

            var configuration = configurationBuilder.Build();

            _clientId = configuration["ClientId"];
            _clientSecret = configuration["ClientSecret"];

            // print the values to the console
            System.Console.WriteLine($"Client ID: {_clientId}");
            System.Console.WriteLine($"Client Secret: {_clientSecret}");
        }
    }
}
