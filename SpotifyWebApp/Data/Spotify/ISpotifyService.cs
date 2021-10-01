using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyWebApp.Data.Spotify
{
    public interface ISpotifyService
    {
        Task Play();
        Task Pause();
    }
}
