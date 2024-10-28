using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace PlaylistQuickAdd.Models
{
    public class Spotify
    {
        public SpotifyClient Client { get; private set; }

        public string LoggedInUser { get; set; } // TEMP
        public List<string> Playlists { get; internal set; }
        public ObservableCollection<Playlist> PlaylistsWithImages { get; internal set; } // TODO rename
        public ObservableCollection<Track> SavedTracks { get; internal set; }

        private readonly string redirectUriString;
        private string credentialsPath;

        private readonly string clientID = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");

        private static readonly EmbedIOAuthServer server = new(new Uri("http://localhost:3000/callback"), 3000);

        public Spotify()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();

            redirectUriString = configuration.GetSection("RedirectURI").Value;

            CheckValidClientID();
            SetupCredentialsPath();
        }

        private void SetupCredentialsPath()
        {
            var applicationDataDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            var playlistQuickAddDir = Path.Combine(applicationDataDir.FullName, "PlaylistQuickAdd");
            Directory.CreateDirectory(playlistQuickAddDir);

            credentialsPath = Path.Combine(playlistQuickAddDir, "credentials.json");
        }

        private void CheckValidClientID()
        {
            if (string.IsNullOrEmpty(clientID))
            {
                throw new NullReferenceException("Please set SPOTIFY_CLIENT_ID via environment variables before starting the program");
            }
        }

        internal async Task Login()
        {
            if (File.Exists(credentialsPath))
            {
                await ConnectToSpotify();
            }
            else
            {
                await StartAuthentication();
            }
        }

        private async Task ConnectToSpotify()
        {
            var json = await File.ReadAllTextAsync(credentialsPath);
            var token = JsonConvert.DeserializeObject<PKCETokenResponse>(json);

            var authenticator = new PKCEAuthenticator(clientID!, token!); // TODO check what the ! does
            authenticator.TokenRefreshed += (sender, token) => File.WriteAllText(credentialsPath, JsonConvert.SerializeObject(token));

            var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);

            Client = new SpotifyClient(config);

            server.Dispose();
        }

        private async Task StartAuthentication()
        {
            var (verifier, challenge) = PKCEUtil.GenerateCodes();

            var redirectUri = new Uri(redirectUriString);

            await server.Start();
            server.AuthorizationCodeReceived += async (sender, response) =>
            {
                await server.Stop();
                var token = await new OAuthClient().RequestToken(
                  new PKCETokenRequest(clientID!, response.Code, redirectUri, verifier)
                );

                await File.WriteAllTextAsync(credentialsPath, JsonConvert.SerializeObject(token));
                await ConnectToSpotify();
            };

            var request = new LoginRequest(redirectUri, clientID!, LoginRequest.ResponseType.Code)
            {
                CodeChallenge = challenge,
                CodeChallengeMethod = "S256",
                Scope = [Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserLibraryRead, Scopes.UserLibraryModify]
            };

            var uri = request.ToUri();
            try
            {
                BrowserUtil.Open(uri);
            }
            catch (Exception)
            {
                Console.WriteLine("Unable to open URL, manually open: {0}", redirectUri);
            }
        }
    }
}
