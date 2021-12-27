# ASHttpStarter

[AssistantSeika](https://hgotoh.jp/wiki/doku.php/documents/voiceroid/assistantseika/assistantseika-001a) のUIを自動操作して、
AssistantSeikaのHTTPサーバを自動的に起動するプログラムです。

Windows起動時に、音声合成製品と、それを操作するAssistantSeika、AssistantSeikaのHTTPサーバ機能の3つを自動的に起動する目的に使用することができます。

音声合成製品やAssistantSeika自体を起動する機能は本プログラムに含まれていないため、PowerShellスクリプトなどを併用することを想定しています。


## 処理の流れ（想定）

```
PowerShellスクリプトの自動実行（スタートアップ時）
    音声合成製品1の起動
    音声合成製品2の起動
    起動待機
    AssistantSeikaの起動
    起動待機
    本プログラムの起動
        AssistantSeikaの製品スキャン実行
        AssistantSeikaのHTTPサーバの起動
```

## 動作確認
### v0.1.0.0
- AssistantSeika 20210617/u, 20210723/u

## コマンドライン引数
柔軟に仕様変更に対応するため、操作に使用する固有情報はコマンドライン引数から変更できるようにしています。

```
$ ./ASHttpStarter.exe --help
ASHttpStarter 0.1.0.0
Copyright © 2021 aoirint

  --TargetWindowTitlePrefix       (Default: AssistantSeika)

  --TabControlAutomationId        (Default: tabControl)

  --ProductTabName                (Default: 使用製品)

  --ScanningButtonAutomationId    (Default: ButtonScan)

  --SpeakerTabName                (Default: 話者一覧)

  --HTTPFuncTabName               (Default: HTTP機能設定)

  --HTTPButtonAutomationId        (Default: ButtonHTTP)

  -v, --Verbose                   (Default: false)

  --help                          Display this help screen.

  --version                       Display version information.
```

## 使用例
あらかじめAssistantSeika上で「使用する製品」の一覧にチェックを入れておいてください。
本プログラムは使用する製品を自動で選択する機能を持ちませんが、AssistantSeikaは終了後も選択状態を保持するため、自動起動時に好きな選択状態で開始できます。

ダウンロードした本プログラムの配布物を`%USERPROFILE%/apps/ASHttpStarter v0.1.0.0`に展開してください（以下の手順を修正すれば好きなディレクトリでも可）。

PowerShellスクリプト`StartVoiceServer.ps1`を`%USERPROFILE%/apps/ASHttpStarter v0.1.0.0`以下に作成してください。
待機時間`Sleep`の値（秒単位）は、使用環境での各製品の起動にかかる時間に合わせて、適宜調整してください。

```ps1
Start-Process -FilePath "AIVoiceEditor.exe" -WorkingDirectory "C:\Program Files\AI\AIVoice\AIVoiceEditor"
Sleep 25
Start-Process -FilePath "GynoidTalkEditor.exe" -WorkingDirectory "C:\Program Files (x86)\Gynoid\GynoidTalk"
Sleep 25
Start-Process -FilePath "VOICEVOX.exe" -WorkingDirectory "$env:USERPROFILE\AppData\Local\Programs\VOICEVOX"
Sleep 15
Start-Process -FilePath "AssistantSeika.exe" -WorkingDirectory "C:\Program Files\510product\AssistantSeika"
Sleep 5
Start-Process -FilePath "ASHttpStarter.exe" -WorkingDirectory "$env:USERPROFILE\apps\ASHttpStarter v0.1.0.0"
```

`shell:startup`をエクスプローラのアドレス欄（Ctrl+L）に入力して開き、以下のようなショートカットを適当な名前で作成してください（右クリック→新規作成→ショートカット）。

```
"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -WindowStyle hidden -File "%USERPROFILE%\apps\ASHttpStarter v0.1.0.0\StartVoiceServer.ps1"
```

Windowsを再起動して、動作を確認してください。


## Windows 自動ログオン用設定
### Microsoft公式ツールによる方法（推奨）
自動ログオン設定用の公式ツール: https://docs.microsoft.com/en-us/sysinternals/downloads/autologon

### レジストリ編集による方法
以下のコードを`.reg`ファイルとして保存して実行することで、パスワードレスログオンに関するレジストリ設定を変更できます。

#### パスワードレスログオン機能有効化（旧バージョンを使用?）
```reg
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\PasswordLess\Device]
"DevicePasswordLessBuildVersion"=dword:00000000
```

`netplwiz`を実行し、`ユーザーがこのコンピューターを使うには、ユーザー名とパスワードの入力が必要`のチェックを外す。

#### パスワードレスログオン機能無効化
```reg
Windows Registry Editor Version 5.00

[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\PasswordLess\Device]
"DevicePasswordLessBuildVersion"=dword:00000002
```
