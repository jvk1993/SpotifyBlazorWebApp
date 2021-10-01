using SpotifyWebApp.Data.Spotify.APIResponsemodels;
using System.Threading.Tasks;

namespace SpotifyWebApp.Data.Spotify
{
    public interface ISpotifyService
    {
        Task Play();
        Task Pause();
        Task NextSong();
        Task PreviousSong();
        Task<CurrentSongModel> GetCurrentSongPlayingForUser();
    }
}
