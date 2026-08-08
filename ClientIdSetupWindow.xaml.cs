using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace SpotifyMediaKey
{
    public partial class ClientIdSetupWindow : Wpf.Ui.Controls.FluentWindow
    {
        private bool _isLoading = true;
        public string? EnteredClientId { get; private set; }

        public ClientIdSetupWindow()
        {
            InitializeComponent();
            AppTitleBar.Icon = new Wpf.Ui.Controls.ImageIcon { Source = AppIconHelper.GetIcon() };

            foreach (ComboBoxItem item in LanguageComboBox.Items)
            {
                if ((string)item.Tag == Strings.Language)
                {
                    LanguageComboBox.SelectedItem = item;
                    break;
                }
            }

            ApplyLocalizedText();
            _isLoading = false;
        }

        private void ApplyLocalizedText()
        {
            HeaderText.Text = Strings.Get("InitialSetupTitle");
            LanguageLabel.Text = Strings.Get("Language");
            BodyText.Text = Strings.Get("InitialSetupBody");
            OpenDashboardButton.Content = Strings.Get("OpenDashboard");
            RedirectUriLabel.Text = Strings.Get("RedirectUri");
            CopyButton.Content = Strings.Get("Copy");
            ClientIdLabel.Text = Strings.Get("ClientId");
            StartButtonText.Content = Strings.Get("Start");
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;
            if (LanguageComboBox.SelectedItem is ComboBoxItem item)
            {
                Strings.Language = (string)item.Tag;
                ApplyLocalizedText();
            }
        }

        private void OpenDashboard_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Constants.Spotify.DashboardUrl) { UseShellExecute = true });
        }

        private void CopyRedirectUri_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetText(Constants.Spotify.RedirectUri);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string input = ClientIdTextBox.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                ErrorText.Text = Strings.Get("ClientIdRequired");
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            EnteredClientId = input;
            DialogResult = true;
            Close();
        }
    }
}