using Newtonsoft.Json;

namespace SpotifyWebApp.Data.Spotify.APIResponsemodels
{
    public class CurrentSongModel
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        // Maybe use in the future for playlist feature
        //[JsonProperty("context")]
        //public Context Context { get; set; }

        [JsonProperty("progress_ms")]
        public int ProgressMs { get; set; }

        //[JsonProperty("item")]
        //public Item Item { get; set; }

        [JsonProperty("currently_playing_type")]
        public string CurrentlyPlayingType { get; set; }

        [JsonProperty("is_playing")]
        public bool IsPlaying { get; set; }
    }
}
