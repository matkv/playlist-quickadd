using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Windows.System;

namespace PlaylistQuickAdd
{
    internal class Authorization
    {
        private readonly string spotifyEndpointURL;
        private const string redirectUri = "http://localhost:3000";

        public Authorization()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();

            spotifyEndpointURL = configuration.GetSection("SpotifyEndpointURL").Value;
        }

        public async Task<SpotifyAccessToken> GetSpotifyAccessTokenForClient()
        {
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");

            var data = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";

            using var client = new HttpClient();
            var response = await client.PostAsync(spotifyEndpointURL, new StringContent(data, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded"));
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<SpotifyAccessToken>(responseContent);
        }

        public async Task<SpotifyAccessToken> GetSpotifyAccessTokenForUser(string authorizationCode, string clientToken)
        {
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
            var clientsecret64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));


            var data = $"grant_type=authorization_code&code={authorizationCode}&redirect_uri={redirectUri}";


            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", clientsecret64);
            var response = await client.PostAsync(spotifyEndpointURL, new StringContent(data, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded"));
            var responseContent = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<SpotifyAccessToken>(responseContent);
        }

        public static async Task<string> Login()
        {
            var state = Guid.NewGuid().ToString();
            using var client = new HttpClient();

            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var scope = "user-read-private user-read-email";

            var response = await client.GetAsync($"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&scope={scope}&state={state}");

            await Launcher.LaunchUriAsync(new Uri(response.RequestMessage.RequestUri.ToString()));
            await response.Content.ReadAsStringAsync();

            return await StartHTTPListenerAsync(state);
        }

        private static async Task<String> StartHTTPListenerAsync(string state)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri + "/");
            listener.Start();

            var context = await listener.GetContextAsync();

            var response = context.Response;
            string responseString = "<html><body>Please return to the app.</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer);
            response.OutputStream.Close();

            var queryParams = HttpUtility.ParseQueryString(context.Request.Url.Query);
            string code = queryParams["code"];
            var stateReceived = queryParams["state"];

            // Ensure state matches to prevent CSRF
            if (stateReceived == state)
            {
                Console.WriteLine($"Authorization code: {code}");
            }

            listener.Stop();
            return code;
        }
    }

    internal class SpotifyAccessToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
