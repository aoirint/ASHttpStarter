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
柔軟に仕様変更に対応するため、操作に使用する固有情報はコマンドライン引数から変更できるようにしている。

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

## 使用例（AIVoiceの場合）
あらかじめ使用製品一覧にチェックを入れておく。

PowerShellスクリプト`StartAIVoice.ps1`を作成する。
待機時間は適宜調整する。

```ps1
Start-Process -FilePath "AIVoiceEditor.exe" -WorkingDirectory "C:\Program Files\AI\AIVoice\AIVoiceEditor"
Sleep 30
Start-Process -FilePath "AssistantSeika.exe" -WorkingDirectory "C:\Program Files\510product\AssistantSeika"
Sleep 5
Start-Process -FilePath "ASHttpStarter.exe" -WorkingDirectory "$env:USERPROFILE\apps\ASHttpStarter v0.1.0.0"

```

`shell:startup`にPowerShellで実行するためのショートカットを配置する。

```
"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -WindowStyle hidden -File "%USERPROFILE%\apps\ASHttpStarter v0.1.0.0\StartAIVoice.ps1"
```

## Windows 自動ログオン用設定
### Microsoft公式ツールによる方法（推奨）
自動ログオン設定用の公式ツール: https://docs.microsoft.com/en-us/sysinternals/downloads/autologon

### レジストリ編集による方法
`.reg`ファイルとして保存して実行する。

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
