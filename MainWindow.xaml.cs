using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SpotifyMediaKey
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Wpf.Ui.Controls.FluentWindow
    {
        public static MainWindow? Instance { get; private set; }
        private readonly OsdWindow _osd = new();
        public MainWindow()
        {
            Instance = this;
            AppSettings.Load();
            Strings.Language = AppSettings.Language;
            InitializeComponent();
            MediaKeyHook.KeyPressed += OnVolumeKey;
            MediaKeyHook.Start();
            InitializeTrayIcon();
            _ = LoginAsync();
        }
        private async Task LoginAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(AppSettings.ClientId))
                {
                    var setup = new ClientIdSetupWindow();
                    bool? result = setup.ShowDialog();
                    if (result != true || string.IsNullOrEmpty(setup.EnteredClientId))
                    {
                        System.Windows.Application.Current.Shutdown();
                        return;
                    }

                    AppSettings.ClientId = setup.EnteredClientId;
                    AppSettings.Language = Strings.Language;
                    AppSettings.Save();
                }

                SpotifyAuth.ClientId = AppSettings.ClientId;

                bool restored = await SpotifyAuth.TryRestoreSessionAsync();
                if (!restored)
                {
                    await SpotifyAuth.LoginAsync();
                }

                using var http = new HttpClient();
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SpotifyAuth.AccessToken);
                var res = await http.GetStringAsync(Constants.Spotify.MeUrl);
                using var doc = JsonDocument.Parse(res);
                string name = doc.RootElement.GetProperty("display_name").GetString() ?? "?";

                await SpotifyPlayer.RefreshStateAsync();
                StatusText.Text = string.Format(Strings.Get("LoginSuccess"), name);
                _trayIcon?.ShowBalloonTip(3000, "Spotify MediaKey", string.Format(Strings.Get("LoggedInBalloon"), name), System.Windows.Forms.ToolTipIcon.None);

                _pollTimer.Tick += async (s, e) =>
                {
                    try
                    {
                        await SpotifyPlayer.RefreshStateAsync();
                        if (AppSettings.OsdMode == "Always")
                            _osd.ShowNowPlaying($"{SpotifyPlayer.CurrentVolume}%");
                    }
                    catch { }
                };
                _pollTimer.Start();

                if (AppSettings.OsdMode == "Always")
                    _osd.ShowNowPlaying($"{SpotifyPlayer.CurrentVolume}%");
            }
            catch (Exception ex)
            {
                StatusText.Text = Strings.Get("Error");
                _trayIcon?.ShowBalloonTip(5000, "Spotify MediaKey", ex.Message, System.Windows.Forms.ToolTipIcon.Error);
            }
        }
        private async void OnVolumeKey(VolumeKeyType key)
        {
            try
            {
                switch (key)
                {
                    case VolumeKeyType.PlayPause:
                        await SpotifyPlayer.TogglePlayPauseAsync();
                        if (AppSettings.OsdMode != "Off")
                            _osd.ShowNowPlaying(SpotifyPlayer.IsPlaying ? Strings.Get("Playing") : Strings.Get("Paused"));
                        return;

                    case VolumeKeyType.Next:
                        await SpotifyPlayer.SkipNextAsync();
                        await Task.Delay(Constants.App.SkipDelayMs);
                        await SpotifyPlayer.RefreshStateAsync();
                        if (AppSettings.OsdMode != "Off")
                            _osd.ShowNowPlaying(SpotifyPlayer.IsPlaying ? Strings.Get("Playing") : Strings.Get("Paused"));
                        return;

                    case VolumeKeyType.Previous:
                        await SpotifyPlayer.SkipPreviousAsync();
                        await Task.Delay(Constants.App.SkipDelayMs);
                        await SpotifyPlayer.RefreshStateAsync();
                        if (AppSettings.OsdMode != "Off")
                            _osd.ShowNowPlaying(SpotifyPlayer.IsPlaying ? Strings.Get("Playing") : Strings.Get("Paused"));
                        return;
                }

                int newVolume = key == VolumeKeyType.Mute
                    ? await SpotifyPlayer.ToggleMuteAsync()
                    : await SpotifyPlayer.ChangeVolumeAsync(key == VolumeKeyType.Up ? AppSettings.Step : -AppSettings.Step);

                StatusText.Text = string.Format(Strings.Get("VolumeLevel"), newVolume);
                if (AppSettings.OsdMode != "Off") _osd.ShowNowPlaying($"{newVolume}%");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OnVolumeKey error: {ex.Message}");
            }
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSettingsWindow();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        private System.Windows.Forms.NotifyIcon? _trayIcon;

        private void InitializeTrayIcon()
        {
            _trayIcon = new System.Windows.Forms.NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Visible = true,
                Text = "Spotify MediaKey"
            };

            var menu = new System.Windows.Forms.ContextMenuStrip();
            RefreshTrayMenu();

            _trayIcon.DoubleClick += (s, e) => OpenSettingsWindow();
        }
        private void OpenSettingsWindow()
        {
            var settings = new SettingsWindow();
            settings.ShowDialog();
        }
        public void RefreshTrayMenu()
        {
            var menu = new System.Windows.Forms.ContextMenuStrip();
            menu.Items.Add(Strings.Get("TraySettings"), null, (s, e) => OpenSettingsWindow());
            menu.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            menu.Items.Add(Strings.Get("TrayExit"), null, (s, e) =>
            {
                _trayIcon!.Visible = false;
                _trayIcon.Dispose();
                MediaKeyHook.Stop();
                System.Windows.Application.Current.Shutdown();
            });

            if (_trayIcon != null)
                _trayIcon.ContextMenuStrip = menu;
        }
        public void ApplyOsdSettingsChange()
        {
            if (AppSettings.OsdMode == "Always")
                _osd.ShowNowPlaying($"{SpotifyPlayer.CurrentVolume}%");
            else
                _osd.HidePersistent();
        }
        private readonly DispatcherTimer _pollTimer = new()
        {
            Interval = TimeSpan.FromSeconds(Constants.App.PollIntervalSec)
        };
    }
}