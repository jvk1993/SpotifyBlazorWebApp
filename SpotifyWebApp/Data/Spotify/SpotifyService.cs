using Blazored.LocalStorage;
using Newtonsoft.Json;
using SpotifyWebApp.Data.Spotify.APIResponsemodels;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpotifyWebApp.Data.Spotify
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        protected readonly ILocalStorageService _localStorage;
        private readonly ISpotifyAuthService _spotifyAuthService;

        private readonly string SpotifyURL = "https://api.spotify.com/v1/";

        public SpotifyService(HttpClient httpClient, ILocalStorageService localStorageService, ISpotifyAuthService spotifyAuthService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(SpotifyURL);
            _localStorage = localStorageService;
            _spotifyAuthService = spotifyAuthService;
        }

        #region Spotify API calls
        public async Task Pause()
        {
            ClearHeaders();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAccessTokenFromLocalStorage()}");
            await _httpClient.PutAsync("me/player/pause", null);
        }

        public async Task Play()
        {
            ClearHeaders();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAccessTokenFromLocalStorage()}");
            await _httpClient.PutAsync("me/player/play",null);
        }


        public async Task NextSong()
        {
            ClearHeaders();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAccessTokenFromLocalStorage()}");
            await _httpClient.PostAsync("me/player/next", null);
        }

        public async Task PreviousSong()
        {
            ClearHeaders();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAccessTokenFromLocalStorage()}");
            await _httpClient.PostAsync("me/player/previous", null);
        }

        public async Task<CurrentSongModel> GetCurrentSongPlayingForUser()
        {
            ClearHeaders();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {await GetAccessTokenFromLocalStorage()}");
            var response = await _httpClient.GetAsync("me/player/currently-playing");
            var content = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(content))
            {
                CurrentSongModel currentSongModel = JsonConvert.DeserializeObject<CurrentSongModel>(content);
                return currentSongModel;
            }          
            return null;
        }

        #endregion

        #region Spotify API helper methods
        private void ClearHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
        }

        private async Task<string> GetAccessTokenFromLocalStorage()
        {
            string spotifyCredsInJson = await _localStorage.GetItemAsync<string>("SpotifyAPICredentials");
            if (!string.IsNullOrEmpty(spotifyCredsInJson))
            {
                SpotifyCredentials spotifyCredentials = JsonConvert.DeserializeObject<SpotifyCredentials>(spotifyCredsInJson);
                if (spotifyCredentials.TokenExpireDateTime > DateTime.UtcNow)
                {
                    return spotifyCredentials.AccessToken;
                }
                else
                {
                    return await _spotifyAuthService.RefreshToken();
                }
            }
            return null;
        }
        #endregion
    }
}
