using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyMediaKey
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class SpotifyAuth
    {
        public static string? ClientId { get; set; }

        public static string? AccessToken { get; private set; }
        public static string? RefreshToken { get; private set; }
        private static DateTime _expiresAt = DateTime.MinValue;

        public static async Task LoginAsync()
        {
            string verifier = GenerateCodeVerifier();
            string challenge = GenerateCodeChallenge(verifier);

            string authUrl =
                Constants.Spotify.AuthorizeUrl +
                "?response_type=code" +
                $"&client_id={ClientId}" +
                $"&scope={Uri.EscapeDataString(Constants.Spotify.Scope)}" +
                $"&redirect_uri={Uri.EscapeDataString(Constants.Spotify.RedirectUri)}" +
                "&code_challenge_method=S256" +
                $"&code_challenge={challenge}";

            using var listener = new HttpListener();
            listener.Prefixes.Add(Constants.Spotify.RedirectListenerPrefix);
            listener.Start();

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(authUrl) { UseShellExecute = true });

            var context = await listener.GetContextAsync();
            string? code = context.Request.QueryString["code"];

            context.Response.ContentType = "text/html; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(Strings.Get("AuthSuccessHtml"));
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer);
            context.Response.OutputStream.Close();
            listener.Stop();

            if (code == null) throw new Exception(Strings.Get("AuthCodeError"));

            await ExchangeCodeForTokenAsync(code, verifier);
        }

        private static async Task ExchangeCodeForTokenAsync(string code, string verifier)
        {
            using var http = new HttpClient();
            var form = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("grant_type", "authorization_code"),
            new KeyValuePair<string,string>("code", code),
            new KeyValuePair<string,string>("redirect_uri", Constants.Spotify.RedirectUri),
            new KeyValuePair<string,string>("client_id", ClientId ?? ""),
            new KeyValuePair<string,string>("code_verifier", verifier),
             });

            var response = await http.PostAsync(Constants.Spotify.TokenUrl, form);
            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            AccessToken = doc.RootElement.GetProperty("access_token").GetString();
            RefreshToken = doc.RootElement.GetProperty("refresh_token").GetString();

            int expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
            _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 60);
            PersistRefreshToken();
        }
        public static async Task EnsureValidTokenAsync()
        {
            if (DateTime.UtcNow < _expiresAt) return;
            if (RefreshToken == null) return;

            using var http = new HttpClient();
            var form = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string,string>("grant_type", "refresh_token"),
            new KeyValuePair<string,string>("refresh_token", RefreshToken),
            new KeyValuePair<string,string>("client_id", ClientId ?? ""),
        });

            var response = await http.PostAsync(Constants.Spotify.TokenUrl, form);
            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            AccessToken = doc.RootElement.GetProperty("access_token").GetString();

            int expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
            _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn - 60);

            if (doc.RootElement.TryGetProperty("refresh_token", out var rt))
                RefreshToken = rt.GetString();
            PersistRefreshToken();
        }
        private static string GenerateCodeVerifier()
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(32);
            return Base64UrlEncode(bytes);
        }

        private static string GenerateCodeChallenge(string verifier)
        {
            byte[] hash = SHA256.HashData(Encoding.ASCII.GetBytes(verifier));
            return Base64UrlEncode(hash);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }
        private static void PersistRefreshToken()
        {
            AppSettings.RefreshToken = RefreshToken;
            AppSettings.Save();
        }

        public static async Task<bool> TryRestoreSessionAsync()
        {
            if (AppSettings.RefreshToken == null) return false;

            RefreshToken = AppSettings.RefreshToken;
            _expiresAt = DateTime.MinValue;
            await EnsureValidTokenAsync();

            return AccessToken != null;
        }
    }
}