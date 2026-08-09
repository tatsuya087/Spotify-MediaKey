using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SpotifyMediaKey
{
    public partial class OsdWindow : Window
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private readonly DispatcherTimer _timer;
        private string? _lastArtworkUrl;

        public OsdWindow()
        {
            InitializeComponent();
            TitleIcon.Source = AppIconHelper.GetIcon();
            Opacity = 0;

            SourceInitialized += (s, e) =>
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
                SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_NOACTIVATE);
            };

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(Constants.Osd.DisplayDurationMs) };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                FadeOut();
            };
        }

        public void ShowNowPlaying(string statusText)
        {
            var workArea = SystemParameters.WorkArea;
            const double margin = Constants.Osd.MarginPx;

            switch (AppSettings.OsdPosition)
            {
                case "TopRight":
                    Left = workArea.Right - Width - margin;
                    Top = workArea.Top + margin;
                    break;
                case "BottomLeft":
                    Left = workArea.Left + margin;
                    Top = workArea.Bottom - Height - margin;
                    break;
                case "TopLeft":
                    Left = workArea.Left + margin;
                    Top = workArea.Top + margin;
                    break;
                default:
                    Left = workArea.Right - Width - margin;
                    Top = workArea.Bottom - Height - margin;
                    break;
            }

            SetTextWithDissolve(TrackNameText, SpotifyPlayer.TrackName ?? "");
            SetTextWithDissolve(ArtistNameText, SpotifyPlayer.ArtistName ?? "");
            SetArtworkWithDissolve(SpotifyPlayer.ArtworkUrl);
            SetStatusText(statusText);

            Show();
            FadeIn();

            _timer.Stop();
            if (AppSettings.OsdMode != "Always")
            {
                _timer.Start();
            }
        }

        public void HidePersistent()
        {
            _timer.Stop();
            FadeOut();
        }

        private void SetStatusText(string newValue)
        {
            bool bothPercent = StatusText.Text.EndsWith("%") && newValue.EndsWith("%");
            if (bothPercent)
            {
                StatusText.Text = newValue;
            }
            else
            {
                SetTextWithDissolve(StatusText, newValue);
            }
        }

        private void SetTextWithDissolve(TextBlock target, string newValue)
        {
            if (target.Text == newValue) return;

            var fadeOut = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(Constants.Osd.FadeOutMs));
            fadeOut.Completed += (s, e) =>
            {
                target.Text = newValue;
                var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(Constants.Osd.FadeInMs));
                target.BeginAnimation(OpacityProperty, fadeIn);
            };
            target.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void SetArtworkWithDissolve(string? newUrl)
        {
            if (newUrl == null || newUrl == _lastArtworkUrl) return;

            _lastArtworkUrl = newUrl;

            var fadeOut = new DoubleAnimation(1.0, 0.0, TimeSpan.FromMilliseconds(Constants.Osd.FadeOutMs));
            fadeOut.Completed += (s, e) =>
            {
                ArtworkImage.Source = new BitmapImage(new Uri(newUrl));
                var fadeIn = new DoubleAnimation(0.0, 1.0, TimeSpan.FromMilliseconds(Constants.Osd.FadeInMs));
                ArtworkImage.BeginAnimation(OpacityProperty, fadeIn);
            };
            ArtworkImage.BeginAnimation(OpacityProperty, fadeOut);
        }

        private void FadeIn()
        {
            var animation = new DoubleAnimation(Opacity, 1.0, TimeSpan.FromMilliseconds(Constants.Osd.FadeInMs));
            BeginAnimation(OpacityProperty, animation);
        }

        private void FadeOut()
        {
            var animation = new DoubleAnimation(Opacity, 0.0, TimeSpan.FromMilliseconds(Constants.Osd.WindowFadeOutMs));
            animation.Completed += (s, e) => Hide();
            BeginAnimation(OpacityProperty, animation);
        }
    }
}