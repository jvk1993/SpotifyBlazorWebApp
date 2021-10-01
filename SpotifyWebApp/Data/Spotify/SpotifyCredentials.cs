using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyWebApp.Data.Spotify
{
    public class SpotifyCredentials
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
