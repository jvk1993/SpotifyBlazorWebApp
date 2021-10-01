using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyWebApp.Data.Spotify
{
    public interface ISpotifyAuthService
    {
        Task<bool> Login(string code);
        Task RefreshToken();
        string GetAccessToken();
    }
}
