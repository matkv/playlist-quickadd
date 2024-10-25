using System.Threading.Tasks;

namespace PlaylistQuickAdd.Models;

public class SharedDataService
{
    public Spotify Spotify { get; set; }

    public SharedDataService()
    {
        Spotify = new Spotify();
    }

    public async Task LoginSpotify()
    {
        await Spotify.Login();
    }
}