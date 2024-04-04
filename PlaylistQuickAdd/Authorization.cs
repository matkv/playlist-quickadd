using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace PlaylistQuickAdd
{
    internal static class Authorization
    {
        public static async Task<string> GetSpotifyAccessToken()
        {
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");

            var url = "https://accounts.spotify.com/api/token";
            var data = $"grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";

            using var client = new HttpClient();
            var response = await client.PostAsync(url, new StringContent(data, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded"));
            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }
    }
}
