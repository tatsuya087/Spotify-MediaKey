using System;
using System.IO;
using System.Text.Json;

namespace SpotifyMediaKey
{
    public static class AppSettings
    {
        public static int Step = Constants.Settings.DefaultVolumeStep;
        public static string OsdMode { get; set; } = Constants.Settings.DefaultOsdMode;
        public static string OsdPosition { get; set; } = Constants.Settings.DefaultPosition;
        public static double? SettingsLeft { get; set; }
        public static double? SettingsTop { get; set; }
        public static string? RefreshToken { get; set; }
        public static string? ClientId { get; set; }
        public static string Language { get; set; } = Constants.Settings.DefaultLanguage;

        private static string FilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.Settings.AppFolder, Constants.Settings.FileName);

        public static void Load()
        {
            if (!File.Exists(FilePath)) return;

            string json = File.ReadAllText(FilePath);
            var data = JsonSerializer.Deserialize<SettingsData>(json);
            if (data != null)
            {
                Step = data.Step;
                OsdMode = data.OsdMode ?? Constants.Settings.DefaultOsdMode;
                OsdPosition = data.OsdPosition ?? Constants.Settings.DefaultPosition;
                SettingsLeft = data.SettingsLeft;
                SettingsTop = data.SettingsTop;
                RefreshToken = SecureStorage.Unprotect(data.RefreshToken);
                ClientId = SecureStorage.Unprotect(data.ClientId);
                Language = data.Language ?? Constants.Settings.DefaultLanguage;
            }
        }

        public static void Save()
        {
            string? dir = Path.GetDirectoryName(FilePath);
            if (dir != null) Directory.CreateDirectory(dir);

            var data = new SettingsData
            {
                Step = Step,
                OsdMode = OsdMode,
                OsdPosition = OsdPosition,
                SettingsLeft = SettingsLeft,
                SettingsTop = SettingsTop,
                RefreshToken = SecureStorage.Protect(RefreshToken),
                ClientId = SecureStorage.Protect(ClientId),
                Language = Language
            };
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(FilePath, json);
        }

        private class SettingsData
        {
            public int Step { get; set; }
            public string? OsdMode { get; set; }
            public string? OsdPosition { get; set; }
            public double? SettingsLeft { get; set; }
            public double? SettingsTop { get; set; }
            public string? RefreshToken { get; set; }
            public string? ClientId { get; set; }
            public string? Language { get; set; }
        }
    }
}