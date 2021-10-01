using Blazored.LocalStorage;
using Newtonsoft.Json;
using SpotifyWebApp.Data.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpotifyWebApp.Data.Spotify
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        protected readonly ILocalStorageService _localStorage;

        private readonly string SpotifyURL = "https://api.spotify.com/v1/";

        public SpotifyService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(SpotifyURL);
            _localStorage = localStorageService;
        }
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

        private void ClearHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();
        }

        private async Task<string> GetAccessTokenFromLocalStorage()
        {
            string spotifyCredsInJson = await _localStorage.GetItemAsync<string>("SpotifyAPICredentials");
            if(!string.IsNullOrEmpty(spotifyCredsInJson))
            {
                SpotifyCredentials spotifyCredentials = JsonConvert.DeserializeObject<SpotifyCredentials>(spotifyCredsInJson);
                // TODO refresh token if we know its already expired
                return spotifyCredentials.AccessToken;
            }
            return null;
        }
    }
}
