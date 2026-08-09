using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyMediaKey
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class SpotifyPlayer
    {
        public static int CurrentVolume { get; private set; } = 50;
        public static string? DeviceId { get; private set; }
        public static string? TrackName { get; private set; }
        public static string? ArtistName { get; private set; }
        public static string? ArtworkUrl { get; private set; }

        public static async Task RefreshStateAsync()
        {
            await SpotifyAuth.EnsureValidTokenAsync();
            var http = CreateClient();
            var res = await http.GetAsync(Constants.Spotify.PlayerUrl);

            if (res.StatusCode == HttpStatusCode.NoContent) return;

            string json = await res.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("item", out var item) && item.ValueKind == JsonValueKind.Object)
            {
                TrackName = item.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;

                if (item.TryGetProperty("artists", out var artists) && artists.ValueKind == JsonValueKind.Array)
                {
                    ArtistName = string.Join(", ", artists.EnumerateArray()
                        .Select(a => a.TryGetProperty("name", out var an) ? an.GetString() : null)
                        .Where(n => n != null));
                }

                if (item.TryGetProperty("album", out var album) &&
                    album.TryGetProperty("images", out var images) &&
                    images.ValueKind == JsonValueKind.Array && images.GetArrayLength() > 0)
                {
                    var lastImage = images[images.GetArrayLength() - 1];
                    ArtworkUrl = lastImage.TryGetProperty("url", out var urlProp) ? urlProp.GetString() : null;
                }
            }
            DeviceId = doc.RootElement.GetProperty("device").GetProperty("id").GetString();
            CurrentVolume = doc.RootElement.GetProperty("device").GetProperty("volume_percent").GetInt32();
            IsPlaying = doc.RootElement.TryGetProperty("is_playing", out var playingProp) && playingProp.GetBoolean();
        }

        public static async Task<int> ChangeVolumeAsync(int delta)
        {
            await SpotifyAuth.EnsureValidTokenAsync();
            if (DeviceId == null) await RefreshStateAsync();
            if (DeviceId == null) return CurrentVolume;

            CurrentVolume = Math.Clamp(CurrentVolume + delta, 0, 100);

            var http = CreateClient();
            await http.PutAsync(
                $"{Constants.Spotify.PlayerUrl}/volume?volume_percent={CurrentVolume}&device_id={DeviceId}",
                null);

            return CurrentVolume;
        }

        private static readonly HttpClient _httpClient = new();

        private static HttpClient CreateClient()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SpotifyAuth.AccessToken);

            _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(AppSettings.Language));

            return _httpClient;
        }
        private static int _volumeBeforeMute = -1;

        public static async Task<int> ToggleMuteAsync()
        {
            await SpotifyAuth.EnsureValidTokenAsync();
            if (DeviceId == null) await RefreshStateAsync();
            if (DeviceId == null) return CurrentVolume;

            if (_volumeBeforeMute == -1)
            {
                _volumeBeforeMute = CurrentVolume;
                CurrentVolume = 0;
            }
            else
            {
                CurrentVolume = _volumeBeforeMute;
                _volumeBeforeMute = -1;
            }

            var http = CreateClient();
            await http.PutAsync(
                $"{Constants.Spotify.PlayerUrl}/volume?volume_percent={CurrentVolume}&device_id={DeviceId}",
                null);

            return CurrentVolume;
        }
        public static bool IsPlaying { get; private set; }

        public static async Task TogglePlayPauseAsync()
        {
            await SpotifyAuth.EnsureValidTokenAsync();
            if (DeviceId == null) await RefreshStateAsync();
            if (DeviceId == null) return;

            var http = CreateClient();
            string endpoint = IsPlaying ? "pause" : "play";
            await http.PutAsync($"{Constants.Spotify.PlayerUrl}/{endpoint}?device_id={DeviceId}", null);
            IsPlaying = !IsPlaying;
        }

        public static async Task SkipNextAsync()
        {
            await SpotifyAuth.EnsureValidTokenAsync();
            if (DeviceId == null) await RefreshStateAsync();
            if (DeviceId == null) return;

            var http = CreateClient();
            await http.PostAsync($"{Constants.Spotify.PlayerUrl}/next?device_id={DeviceId}", null);
        }

        public static async Task SkipPreviousAsync()
        {
            await SpotifyAuth.EnsureValidTokenAsync();
            if (DeviceId == null) await RefreshStateAsync();
            if (DeviceId == null) return;

            var http = CreateClient();
            await http.PostAsync($"{Constants.Spotify.PlayerUrl}/previous?device_id={DeviceId}", null);
        }
    }
}