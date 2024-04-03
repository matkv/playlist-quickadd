using Microsoft.Extensions.Configuration;
using System.IO;

namespace PlaylistQuickAdd
{
    internal class Authorization
    {
        public Authorization() => LoadConfigFromJSON();

        private static void LoadConfigFromJSON()
        {
            var configPath = Directory.GetCurrentDirectory(); // TODO not getting project root path
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.SetBasePath(configPath)
                .AddJsonFile("/Config/config.json", optional: false, reloadOnChange: true);

            var configuration = configurationBuilder.Build();

            string _clientId = configuration["ClientId"];
            string _clientSecret = configuration["ClientSecret"];

            // print the values to the console
            System.Console.WriteLine($"Client ID: {_clientId}");
            System.Console.WriteLine($"Client Secret: {_clientSecret}");
        }
    }
}
