using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PlaylistQuickAdd.Models
{
    public class SpotifyUser
    {
        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("external_urls")]
        public ExternalUrls ExternalUrls { get; set; }

        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("images")]
        public List<SpotifyImage> Images { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("followers")]
        public Followers Followers { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("product")]
        public string Product { get; set; }

        [JsonPropertyName("explicit_content")]
        public ExplicitContent ExplicitContent { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        public SpotifyAccessToken UserAccessToken { get; set; }

        internal async Task<ObservableCollection<Playlist>> GetPlaylists()
        {
            // TODO REWRITE ALL OF THIS TO USER https://github.com/JohnnyCrazy/SpotifyAPI-NET

            using HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", UserAccessToken.AccessToken);
            HttpResponseMessage response = await client.GetAsync("https://api.spotify.com/v1/me/playlists");

            await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                List<Playlist> playlists = JsonSerializer.Deserialize<List<Playlist>>(responseBody);

                return new ObservableCollection<Playlist>(playlists);
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return null;
            }
        }
    }

    public class ExternalUrls
    {
        [JsonPropertyName("spotify")]
        public string Spotify { get; set; }
    }

    public class SpotifyImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    public class Followers
    {
        [JsonPropertyName("href")]
        public string Href { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }
    }

    public class ExplicitContent
    {
        [JsonPropertyName("filter_enabled")]
        public bool FilterEnabled { get; set; }

        [JsonPropertyName("filter_locked")]
        public bool FilterLocked { get; set; }
    }
}
