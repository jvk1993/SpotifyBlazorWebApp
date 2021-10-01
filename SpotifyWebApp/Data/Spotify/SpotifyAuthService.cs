using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SpotifyWebApp.Data.Spotify.APIResponsemodels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpotifyWebApp.Data.Spotify
{
    public class SpotifyAuthService : ISpotifyAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        protected readonly ILocalStorageService _localStorage;

        private readonly string SpotifyURL = "https://accounts.spotify.com/";
        public SpotifyAuthService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorageService)
        {
            _config = configuration;
            _localStorage = localStorageService;

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(SpotifyURL);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {ConvertToBase64(_config["ClientId"], _config["ClientSecret"])}");
        }
        public async Task<bool> Login(string code)
        {
            var values = new Dictionary<string, string>
            {
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", "https://localhost:44362/callback" }
            };
            var response = await _httpClient.PostAsync("api/token", new FormUrlEncodedContent(values));
            var contents = await response.Content.ReadAsStringAsync();
            SpotifyCredentials spotifyCredentials = JsonConvert.DeserializeObject<SpotifyCredentials>(contents);
            if(spotifyCredentials.AccessToken != null)
            {
                spotifyCredentials.TokenExpireDateTime = DateTime.UtcNow.AddSeconds(spotifyCredentials.ExpiresIn);
                await _localStorage.SetItemAsync("SpotifyAPICredentials", JsonConvert.SerializeObject(spotifyCredentials));
                return true;
            }
            else { return false;}
        }

        public async Task<string> RefreshToken()
        {
            string refreshToken = await GetRefreshTokenFromLocalStorage();
            var values = new Dictionary<string, string>
            {
                { "refresh_token", refreshToken },
                { "grant_type", "refresh_token" },
            };
            var response = await _httpClient.PostAsync("api/token", new FormUrlEncodedContent(values));
            var contents = await response.Content.ReadAsStringAsync();
            SpotifyCredentials newSpotifyCredentials = JsonConvert.DeserializeObject<SpotifyCredentials>(contents);
            if (newSpotifyCredentials.AccessToken != null)
            {
                newSpotifyCredentials.TokenExpireDateTime = DateTime.Now.AddSeconds(newSpotifyCredentials.ExpiresIn);
                string spotifyCredsInJson = await _localStorage.GetItemAsync<string>("SpotifyAPICredentials");
                if (!string.IsNullOrEmpty(spotifyCredsInJson))
                {
                    SpotifyCredentials spotifyCredentials = JsonConvert.DeserializeObject<SpotifyCredentials>(spotifyCredsInJson);
                    spotifyCredentials.AccessToken = newSpotifyCredentials.AccessToken;
                    spotifyCredentials.ExpiresIn = newSpotifyCredentials.ExpiresIn;
                    spotifyCredentials.TokenExpireDateTime = DateTime.UtcNow.AddSeconds(newSpotifyCredentials.ExpiresIn);
                    await _localStorage.SetItemAsync("SpotifyAPICredentials", JsonConvert.SerializeObject(spotifyCredentials));
                }
                return newSpotifyCredentials.AccessToken;
            }
            return null;
        }

        private async Task<string> GetRefreshTokenFromLocalStorage()
        {
            string spotifyCredsInJson = await _localStorage.GetItemAsync<string>("SpotifyAPICredentials");
            if (!string.IsNullOrEmpty(spotifyCredsInJson))
            {
                SpotifyCredentials spotifyCredentials = JsonConvert.DeserializeObject<SpotifyCredentials>(spotifyCredsInJson);
                return spotifyCredentials.RefreshToken;
            }
            return null;
        }

        private static string ConvertToBase64(string clientId, string clientSecret)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
