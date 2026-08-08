# <img src="Assets/Logo.ico" width="36"> Spotify MediaKey

Control Spotify directly with media keys, independent of system volume.

キーボードのメディアキーでSpotifyを直接操作するアプリです。

[English](#English) / [日本語](#日本語)

---

## English

### Features


- Supported keys:
	- Volume Up / Down / Mute
	- Play / Pause / Next / Previous
- Runs in the system tray
- On-screen popup
- Adjustable volume step size
- Auto-start with Windows
- English / Japanese interface

### Requirements

- Windows 10 or later
- Spotify Premium account
- Spotify Developer app (create your own Client ID)

### Setup

1. Download from [Releases](https://github.com/tatsuya087/Spotify-MediaKey/releases).
2. Extract the `.zip` file anywhere and run the executable.
3. On first launch, click "Open Spotify for Developers" to create a new app.
4. Set the Redirect URI to: `http://127.0.0.1:8888/callback`.
5. Copy the Client ID and paste it into the setup window.
6. Approve access when Spotify's consent screen appears.
7. Your media keys now control Spotify. The app will minimize to the tray.

### Settings
- Double-click or right-click the tray icon to open the settings window.
- Available settings: Volume step size, popup visibility, auto-start, and interface language.

### Data

Settings are stored encrypted at `%AppData%\SpotifyMediaKey\settings.json` using Windows DPAPI. They are tied to your user account and cannot be read by other users or machines.

### Building from source

- Visual Studio 2022 with .NET desktop workload
- .NET 8 SDK

### License

MIT

---

## 日本語

### 機能

- 対応キー
	- 音量アップ / ダウン / ミュート
	- 再生 / 一時停止 / 次の曲 / 前の曲
- タスクトレイに常駐
- 操作時にポップアップを表示
- 音量ステップの調整
- Windowsスタートアップ起動
- 日本語 / 英語対応

### 必要環境

- Windows 10 以降
- Spotify Premiumアカウント
- Spotify Developerアプリ

### セットアップ

1. [Releases](https://github.com/tatsuya087/Spotify-MediaKey/releases)からダウンロードします。
2. `.zip` ファイルを展開し、実行ファイルを起動します。
3. 初回起動時 `Spotify for Developersを開く` をクリックして新しいアプリを作成します。
4. Redirect URI を `http://127.0.0.1:8888/callback` に設定します。
5. Client ID をコピーしてセットアップ画面に貼り付けます。
6. Spotify の許可画面が表示されたらアクセスを許可します。
7. これでメディアキーで Spotify を制御できるようになります。アプリはタスクトレイに収まります。

### 設定
- トレイアイコンをダブルクリックまたは右クリックで設定画面が開けます。
- 音量ステップ / ポップアップの表示・非表示 / スタートアップ起動 / 言語 の設定が可能です。

### データ

設定は `%AppData%\SpotifyMediaKey\settings.json` に Windows DPAPI で暗号化して保存されます。このパソコンのこのユーザーアカウントにのみアクセス可能です。

### ソースからビルド

- Visual Studio 2022 (.NET デスクトップワークロード)
- .NET 8 SDK

### ライセンス

MIT
