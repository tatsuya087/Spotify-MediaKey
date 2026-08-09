namespace SpotifyMediaKey
{
    internal static class Constants
    {
        internal static class Spotify
        {
            internal const string AuthorizeUrl = "https://accounts.spotify.com/authorize";
            internal const string TokenUrl = "https://accounts.spotify.com/api/token";
            internal const string PlayerUrl = "https://api.spotify.com/v1/me/player";
            internal const string MeUrl = "https://api.spotify.com/v1/me";
            internal const string DashboardUrl = "https://developer.spotify.com/dashboard";
            internal const string RedirectUri = "http://127.0.0.1:8888/callback";
            internal const string RedirectListenerPrefix = "http://127.0.0.1:8888/callback/";
            internal const string Scope = "user-read-playback-state user-modify-playback-state";
        }

        internal static class Settings
        {
            internal const string AppFolder = "SpotifyMediaKey";
            internal const string FileName = "settings.json";
            internal const string DefaultOsdMode = "OnKeyPress";
            internal const string DefaultPosition = "BottomRight";
            internal const string DefaultLanguage = "en";
            internal const int DefaultVolumeStep = 2;
        }

        internal static class Osd
        {
            //OSD表示時間
            internal const double DisplayDurationMs = 1800;
            internal const double FadeOutMs = 120;
            internal const double FadeInMs = 150;
            internal const double WindowFadeOutMs = 300;
            internal const double MarginPx = 24;
        }

        internal static class App
        {
            //楽曲情報更新時間(秒)
            internal const int PollIntervalSec = 4;
            internal const int SkipDelayMs = 400;
        }
    }
}