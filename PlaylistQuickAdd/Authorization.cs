using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;
using Windows.System;

namespace PlaylistQuickAdd
{
    internal class Authorization
    {
        private readonly string spotifyEndpointURL;

        public Authorization()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();

            spotifyEndpointURL = configuration.GetSection("SpotifyEndpointURL").Value;
        }

        public async Task<SpotifyAccessToken> GetSpotifyAccessToken()
        {
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");

            var data = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";

            using var client = new HttpClient();
            var response = await client.PostAsync(spotifyEndpointURL, new StringContent(data, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded"));
            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON response
            var accessToken = System.Text.Json.JsonSerializer.Deserialize<SpotifyAccessToken>(responseContent);

            return accessToken;
        }

        public async Task Login()
        {
            var redirectUri = "http://localhost:3000"; 
            var state = Guid.NewGuid().ToString();


            using var client = new HttpClient();

            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var scope = "user-read-private user-read-email";

            var response = await client.GetAsync($"https://accounts.spotify.com/authorize?client_id={clientId}&response_type=code&redirect_uri={redirectUri}&scope={scope}&state={state}");

            await Launcher.LaunchUriAsync(new Uri(response.RequestMessage.RequestUri.ToString()));
            await StartHTTPListenerAsync(redirectUri, state);

            var test = response.Content.ReadAsStringAsync();
        }

        private async Task StartHTTPListenerAsync(string redirectUri, string state)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(redirectUri + "/");
            listener.Start();

            var context = await listener.GetContextAsync(); // This will wait until a request is received

            var response = context.Response;
            string responseString = "<html><body>Please return to the app.</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            var queryParams = HttpUtility.ParseQueryString(context.Request.Url.Query);
            var code = queryParams["code"];
            var stateReceived = queryParams["state"];

            // Ensure state matches to prevent CSRF
            if (stateReceived == state)
            {
                // Process the authorization code as needed
                Console.WriteLine($"Authorization code: {code}");
            }

            listener.Stop();
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
