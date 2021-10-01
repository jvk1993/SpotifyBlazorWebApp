using System.Threading.Tasks;

namespace SpotifyWebApp.Data.Spotify
{
    public interface ISpotifyAuthService
    {
        Task<bool> Login(string code);
        Task<string> RefreshToken();
    }
}
