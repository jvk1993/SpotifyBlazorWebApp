using Newtonsoft.Json;
using System;

namespace SpotifyWebApp.Data.Spotify.APIResponsemodels
{
    public class SpotifyCredentials
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        public DateTime? TokenExpireDateTime { get; set; }
    }
}
