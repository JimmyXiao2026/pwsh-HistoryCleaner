# PowerShell History Cleaner

**English** | [简体中文](README.zh-CN.md)

A tiny always-on background tool that trims your PowerShell command history: every 5 minutes it checks `ConsoleHost_history.txt`, matches lines against a blacklist (substring) and a whitelist (whole line), and removes matched lines. The UI is available in **Simplified Chinese / Traditional Chinese / English**.

- Target file: `%APPDATA%\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt`
- Runtime: Windows 7+ with .NET Framework 4.x (preinstalled — no third-party dependencies)
- Stack: single-file C# WinForms (`src/PSHistoryCleaner.cs`), compiled with the built-in `csc.exe` into a `winexe` (no console window)

## Features

| Feature | Detail |
| --- | --- |
| Blacklist | A line is matched when it **contains** the rule (substring match) |
| Whitelist | A matched line is protected only when the **whole line** equals the whitelist rule |
| Deletion | Matched lines are listed first, then removed as whole lines — no blank lines left behind, no backup made |
| Duplicate cleanup | **Fully duplicated commands keep only the last occurrence** (five `cls` lines become the single last one); comparison is trimmed and case-insensitive |
| Background | Checks every 5 minutes, fully silent, never pops a window |
| Config UI | Fixed-size window: whitelist on top, blacklist below, unlimited entries, each list scrolls independently |
| Single instance | Launching the exe again never opens a second window; the existing one is brought to the foreground |
| Bottom-left shortcuts | Enable auto-start (writes `HKCU\...\Run` with `/hidden`), show tray icon, create Start menu shortcut |
| Language switch | Language button in the title bar cycles 简中 / 繁體 / EN; first run follows the system UI language and the choice is remembered |
| Look & feel | Rounded custom-drawn UI, embedded application icon |

## Command line

| Argument | Meaning |
| --- | --- |
| `/hidden` | Silent start: background engine only, no window (used by the auto-start entry) |
| `/checkonce` | Run one check, then exit (handy for troubleshooting) |

## Config & log

- `%APPDATA%\PSHistoryCleaner\config.ini` — `[white]`, `[black]`, `[setting]` (`tray`, `lang`, `removed`, `last`)
- `%APPDATA%\PSHistoryCleaner\cleaner.log` — background activity log, rotated to `.old` above 512 KB
- Test hooks: environment variables `PSHC_CONFIG_DIR` and `PSHC_HISTORY_PATH` point the app at a scratch config folder and a scratch history file, so tests never touch your real history

## Build

```powershell
# compile the app (system csc, no Visual Studio needed)
& "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe /platform:anycpu /optimize+ `
  /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll `
  /win32icon:"favicon.ico" /out:"PSHistoryCleaner.exe" "src\PSHistoryCleaner.cs"

# build the installer (NSIS 3.x)
cd installer
makensis installer.nsi
```

Or just run `build.ps1` (adds `-SkipInstaller` to build only the exe).

## Download & install

Two artifacts are attached to every GitHub Release:

- **Installer** (`PowerShellHistoryCleaner_v1.0.0_Setup.exe`) — NSIS/MUI2, language selectable during setup (Simplified Chinese / Traditional Chinese / English)
  - per-user install into `%LOCALAPPDATA%\Programs\PSHistoryCleaner`, **no UAC / admin rights needed**
  - Start menu shortcuts, optional desktop shortcut
  - optional "start with Windows": `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` = `"…\PSHistoryCleaner.exe" /hidden`
  - any running instance is terminated before install/uninstall
  - proper uninstaller plus an entry under Settings → Apps; user config in `%APPDATA%\PSHistoryCleaner` is kept on uninstall
- **Portable** (`PowerShellHistoryCleaner_v1.0.0_Portable.zip`) — unzip and run, writes nothing to system directories

## Notes

- No signing certificate yet: Windows SmartScreen may warn on first run. Choose "More info → Run anyway" if you trust the source.
- The cleaner edits `ConsoleHost_history.txt` in place (temp file + atomic replace, with retries) and keeps the original line endings; it never creates a backup file.

## License

MIT
