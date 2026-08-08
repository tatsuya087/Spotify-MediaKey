using System.Collections.Generic;

namespace SpotifyMediaKey
{
    public static class Strings
    {
        public static string Language = "en";

        private static readonly Dictionary<string, Dictionary<string, string>> _table = new()
        {
            ["ja"] = new()
            {
                //初期設定画面
                ["InitialSetupTitle"] = "初期設定",
                ["InitialSetupBody"] = "このアプリを使うには Spotify for Developers への登録と、Spotify Premiumアカウントが必要です。下のボタンから Dashboard を開いてアプリを作成し Redirect URI に下記のURLを登録してください。発行された Client ID を下に貼り付けてください。",
                ["OpenDashboard"] = "Spotify for Developersを開く",
                ["RedirectUri"] = "Redirect URI",
                ["Copy"] = "コピー",
                ["ClientId"] = "Client ID",
                ["ClientIdRequired"] = "Client IDを入力してください",
                ["AuthSuccessHtml"] = "<html><head><meta charset=\"utf-8\"><script>setTimeout(function(){window.close();},3000);</script></head><body>ログインできました。3秒後に自動的に閉じます。<br><span style=\"font-size:0.8em;color:gray;\">(自動で閉じない場合は手動で閉じてください)</span></body></html>",
                //設定画面
                ["Settings"] = "設定",
                ["VolumeStep"] = "音量ステップ",
                ["OsdModeLabel"] = "ポップアップ表示",
                ["OsdOff"] = "表示しない",
                ["OsdOnKeyPress"] = "キー操作時のみ表示",
                ["OsdAlways"] = "常に表示",
                ["AutoStart"] = "Windows起動時に自動的に起動する",
                ["OsdPositionLabel"] = "表示位置",
                ["PositionBottomRight"] = "右下",
                ["PositionTopRight"] = "右上",
                ["PositionBottomLeft"] = "左下",
                ["PositionTopLeft"] = "左上",
                ["Language"] = "言語",
                //トレイアイコン
                ["TraySettings"] = "設定",
                ["TrayExit"] = "終了",
                //Windows通知
                ["LoggedInBalloon"] = "ログインしました: {0}",
                ["LoginSuccess"] = "ログイン成功: {0}",
                ["AuthCodeError"] = "認証コードを取得できませんでした",
                //OSD
                ["Start"] = "開始",
                ["Playing"] = "再生中",
                ["Paused"] = "一時停止中",
                ["VolumeLevel"] = "音量: {0}%",
                //エラー
                ["Error"] = "エラー",
            },
            ["en"] = new()
            {
                //SetupWindow
                ["InitialSetupTitle"] = "Initial Setup",
                ["InitialSetupBody"] = "This app requires your own Spotify for Developers app and a Spotify Premium account. Open the dashboard below, create an app, and add the URL below as a Redirect URI. Then paste the generated Client ID below.",
                ["OpenDashboard"] = "Open Spotify for Developers",
                ["RedirectUri"] = "Redirect URI",
                ["Copy"] = "Copy",
                ["ClientId"] = "Client ID",
                ["ClientIdRequired"] = "Please enter a Client ID",
                ["AuthSuccessHtml"] = "<html><head><meta charset=\"utf-8\"><script>setTimeout(function(){window.close();},3000);</script></head><body>Login successful. This tab will close automatically in 3 seconds.<br><span style=\"font-size:0.8em;color:gray;\">(If it doesn't close, please close it manually)</span></body></html>",
                //SettingsWindow
                ["Settings"] = "Settings",
                ["VolumeStep"] = "Volume Step",
                ["OsdModeLabel"] = "Popup display",
                ["OsdOff"] = "Never show",
                ["OsdOnKeyPress"] = "Show on key press",
                ["OsdAlways"] = "Always show",
                ["AutoStart"] = "Start automatically with Windows",
                ["Language"] = "Language",
                ["OsdPositionLabel"] = "Position",
                ["PositionBottomRight"] = "Bottom Right",
                ["PositionTopRight"] = "Top Right",
                ["PositionBottomLeft"] = "Bottom Left",
                ["PositionTopLeft"] = "Top Left",
                //Tray
                ["TraySettings"] = "Settings",
                ["TrayExit"] = "Exit",
                //WindowsBalloon
                ["LoggedInBalloon"] = "Logged in as {0}",
                ["LoginSuccess"] = "Logged in as {0}",
                ["AuthCodeError"] = "Failed to get authorization code",
                //OSD
                ["Start"] = "Start",
                ["Playing"] = "Playing",
                ["Paused"] = "Paused",
                ["VolumeLevel"] = "Volume: {0}%",
                //Error
                ["Error"] = "Error",
            }
        };

        public static string Get(string key) => _table[Language].TryGetValue(key, out var value) ? value : key;
    }
}