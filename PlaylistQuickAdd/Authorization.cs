using Microsoft.Extensions.Configuration;
using SpotifyAPI.Web;
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
        private const string redirectUri = "http://localhost:3000/callback";

        private string challengeCode;
        private string verifierCode;

        public Authorization()
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json")
                .Build();

            spotifyEndpointURL = configuration.GetSection("SpotifyEndpointURL").Value;
            (challengeCode, verifierCode) = PKCEUtil.GenerateCodes(120);
        }

        public async Task<SpotifyClient> Login()
        {
            var loginURI = GenerateLoginURI();
            await Launcher.LaunchUriAsync(loginURI);

            return await StartListener(verifierCode);
        }

        private Uri GenerateLoginURI()
        {
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var loginRequest = new LoginRequest(new Uri(redirectUri), clientId, LoginRequest.ResponseType.Token)
            {
                CodeChallengeMethod = "S256",
                CodeChallenge = challengeCode,
                Scope = [Scopes.PlaylistModifyPrivate, Scopes.PlaylistModifyPublic, Scopes.UserReadCurrentlyPlaying, Scopes.Streaming] // TODO check which ones I need
            };

            return loginRequest.ToUri();
        }

        private async Task<SpotifyClient> GetCallback(string code)
        {
            var initialResponse = await new OAuthClient().RequestToken(
              new PKCETokenRequest("ClientId", code, new Uri(redirectUri), verifierCode)
            );

            return new SpotifyClient(initialResponse.AccessToken);
        }


        private async Task<SpotifyClient> StartListener(string state)
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
            

            // TODO error here, this is all null, check why?

            var queryParams = HttpUtility.ParseQueryString(context.Request.Url.Query);
            string code = queryParams["code"];
            var stateReceived = queryParams["state"];

            var callback = await GetCallback(stateReceived);
            listener.Stop();


            return callback;

            // TODO check refresh logic https://johnnycrazy.github.io/SpotifyAPI-NET/docs/pkce
        }
    }    
}
