using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

        private string AccessToken;
        public SpotifyAuthService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(SpotifyURL);
            _config = configuration;
            _localStorage = localStorageService;
        }
        public async Task<bool> Login(string code)
        {
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {ConvertToBase64(_config["ClientId"],_config["ClientSecret"])}");
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
                AccessToken = spotifyCredentials.AccessToken;
                await _localStorage.SetItemAsync("SpotifyAPICredentials", JsonConvert.SerializeObject(spotifyCredentials));
                return true;
            }
            else { return false;}
        }

        public Task RefreshToken()
        {
            throw new NotImplementedException();
        }

        private static string ConvertToBase64(string clientId, string clientSecret)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}");
            return Convert.ToBase64String(plainTextBytes);
        }

        public string GetAccessToken()
        {
            return AccessToken;
        }
    }
}
