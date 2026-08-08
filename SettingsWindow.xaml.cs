using System.Windows;
using System.Windows.Controls;

namespace SpotifyMediaKey
{
    public partial class SettingsWindow : Wpf.Ui.Controls.FluentWindow
    {
        private bool _isLoading = true;

        public SettingsWindow()
        {
            InitializeComponent();
            if (AppSettings.SettingsLeft.HasValue && AppSettings.SettingsTop.HasValue)
            {
                Left = AppSettings.SettingsLeft.Value;
                Top = AppSettings.SettingsTop.Value;
            }
            else
            {
                var workArea = SystemParameters.WorkArea;
                Left = workArea.Left + (workArea.Width - Width) / 2;
                Top = workArea.Top + (workArea.Height - Height) / 2;
            }

            Closing += (s, e) =>
            {
                AppSettings.SettingsLeft = Left;
                AppSettings.SettingsTop = Top;
                AppSettings.Save();
            };
            AppTitleBar.Icon = new Wpf.Ui.Controls.ImageIcon { Source = AppIconHelper.GetIcon() };

            ApplyLocalizedText();

            StepSlider.Value = AppSettings.Step;
            StepValueText.Text = $"{AppSettings.Step}%";
            AutoStartCheckBox.IsChecked = AutoStart.IsAutoStartEnabled();

            switch (AppSettings.OsdMode)
            {
                case "Off": OsdOffRadio.IsChecked = true; break;
                case "Always": OsdAlwaysRadio.IsChecked = true; break;
                default: OsdKeyPressRadio.IsChecked = true; break;
            }

            foreach (ComboBoxItem item in LanguageComboBox.Items)
            {
                if ((string)item.Tag == AppSettings.Language)
                {
                    LanguageComboBox.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in OsdPositionComboBox.Items)
            {
                if ((string)item.Tag == AppSettings.OsdPosition)
                {
                    OsdPositionComboBox.SelectedItem = item;
                    break;
                }
            }

            _isLoading = false;
        }

        private void ApplyLocalizedText()
        {
            HeaderText.Text = Strings.Get("Settings");
            StepLabel.Text = Strings.Get("VolumeStep");
            OsdModeLabel.Text = Strings.Get("OsdModeLabel");
            OsdOffRadio.Content = Strings.Get("OsdOff");
            OsdKeyPressRadio.Content = Strings.Get("OsdOnKeyPress");
            OsdAlwaysRadio.Content = Strings.Get("OsdAlways");
            OsdPositionLabel.Text = Strings.Get("OsdPositionLabel");
            AutoStartCheckBox.Content = Strings.Get("AutoStart");
            LanguageLabel.Text = Strings.Get("Language");

            foreach (ComboBoxItem item in OsdPositionComboBox.Items)
            {
                item.Content = Strings.Get("Position" + (string)item.Tag);
            }
        }

        private void StepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (StepValueText == null) return;

            StepValueText.Text = $"{(int)e.NewValue}%";
            if (_isLoading) return;

            AppSettings.Step = (int)e.NewValue;
            AppSettings.Save();
        }

        private void OsdMode_Changed(object sender, RoutedEventArgs e)
        {
            if (_isLoading) return;

            AppSettings.OsdMode = OsdOffRadio.IsChecked == true ? "Off"
                : OsdAlwaysRadio.IsChecked == true ? "Always"
                : "OnKeyPress";
            AppSettings.Save();
            MainWindow.Instance?.ApplyOsdSettingsChange();
        }

        private void OsdPositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;
            if (OsdPositionComboBox.SelectedItem is ComboBoxItem item)
            {
                AppSettings.OsdPosition = (string)item.Tag;
                AppSettings.Save();
                MainWindow.Instance?.ApplyOsdSettingsChange();
            }
        }

        private void AutoStartCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isLoading) return;
            AutoStart.SetAutoStart(AutoStartCheckBox.IsChecked == true);
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoading) return;
            if (LanguageComboBox.SelectedItem is ComboBoxItem item)
            {
                Strings.Language = (string)item.Tag;
                AppSettings.Language = Strings.Language;
                AppSettings.Save();
                ApplyLocalizedText();
                MainWindow.Instance?.RefreshTrayMenu();
            }
        }
    }
}