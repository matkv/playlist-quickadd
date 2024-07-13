using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PlaylistQuickAdd.Models
{
    public class Spotify
    {
        public SpotifyClient Client { get; private set; }

        public string LoggedInUser { get; set; } // TEMP
        public List<string> Playlists { get; internal set; }

        private readonly string redirectUri;
        private static EmbedIOAuthServer authServer;

        public Spotify()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();

            redirectUri = configuration.GetSection("RedirectURI").Value;
        }

        internal async Task Login()
        {
            authServer = new EmbedIOAuthServer(new Uri(redirectUri), 3000);
            await authServer.Start();

            authServer.ImplictGrantReceived += OnImplicitGrantReceived;
            authServer.ErrorReceived += OnErrorReceived;

            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");

            var loginRequest = new LoginRequest(new Uri(redirectUri), clientId, LoginRequest.ResponseType.Token)
            {
                Scope = [Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative]
            };
            
            BrowserUtil.Open(loginRequest.ToUri());
        }

        private async Task<SpotifyClient> OnImplicitGrantReceived(object sender, ImplictGrantResponse response)
        {
            await authServer.Stop();

            Client = new SpotifyClient(response.AccessToken);
            return Client;
        }

        private static async Task OnErrorReceived(object sender, string error, string state)
        {
            Console.WriteLine($"Aborting authorization, error received: {error}");
            await authServer.Stop();
        }
    }
}
