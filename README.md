# PowerShell 历史清理 / PowerShell History Cleaner

一个常驻后台的小工具：每 5 分钟检查 PowerShell 的 `ConsoleHost_history.txt`，按黑名单/白名单规则自动清理历史命令，界面支持 简体中文 / 繁體中文 / English。

A tiny background tool that trims your PowerShell command history every 5 minutes using a blacklist (substring match) and a whitelist (whole-line match). UI available in Simplified Chinese, Traditional Chinese and English.

- 目标文件 / Target file：`%APPDATA%\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt`
- 运行时 / Runtime：Windows 7+，.NET Framework 4.x（系统自带，无需安装依赖）/ .NET Framework 4.x (preinstalled, no third-party deps)
- 语言 / Language：C# WinForms，单文件源码 `src/PSHistoryCleaner.cs`，用 `csc.exe` 编译为 `winexe`（无控制台窗口）

## 功能 / Features

| 中文 | English |
| --- | --- |
| 黑名单：一行里**包含**关键字即命中 | Blacklist: a line is matched if it **contains** the rule |
| 白名单：**整行完全相同**才保留（保护被黑名单命中的行） | Whitelist: the **whole line** must match, then it is protected from deletion |
| 命中的行会被列举，判断后再整行删除，不留空行、不备份 | Matched lines are listed, then the whole line is removed — no blank lines left, no backup made |
| **完全重复的命令只保留最后一条**（如多行 `cls` 只留最后一行） | **Fully duplicated commands are reduced to the last occurrence** (e.g. several `cls` lines keep only the last one) |
| 每 5 分钟后台检查一次，全程静默、不弹窗 | Checks every 5 minutes, silently, no window popping up |
| 固定尺寸配置窗口，上半白名单、下半黑名单，可无限增删，各自滚动 | Fixed-size window: whitelist on top, blacklist below, unlimited entries, each list scrolls |
| 配置界面单实例（重复双击 exe 只唤起已有窗口） | Single instance: launching the exe again just brings the existing window forward |
| 左下快捷键：创建开机自启动、在任务栏中显示、创建开始目录快捷方式 | Bottom-left shortcuts: enable auto-start, show tray icon, create Start menu shortcut |
| 标题栏语言按钮循环切换 简中 / 繁體 / EN，首次按系统语言自动选择 | Language button in the title bar cycles 简中 / 繁體 / EN; first run follows the system language |
| 圆角自绘 UI，自带图标 | Rounded custom-drawn UI with its own icon |

## 命令行参数 / Command line

| 参数 | 说明 |
| --- | --- |
| `/hidden` | 静默启动，只跑后台引擎，不显示界面（开机自启动用的就是它）/ silent start, background engine only |
| `/checkonce` | 立刻检查一次后退出，用于排查 / run one check then exit |

## 配置与日志 / Config & log

- `%APPDATA%\PSHistoryCleaner\config.ini` —— `[white]` / `[black]` / `[setting]`（`tray`、`lang`、`removed`、`last`）
- `%APPDATA%\PSHistoryCleaner\cleaner.log` —— 后台处理记录，超过 512 KB 轮换为 `.old`
- 可用环境变量 `PSHC_CONFIG_DIR`、`PSHC_HISTORY_PATH` 指向测试目录与测试历史文件（不影响真实环境）

## 构建 / Build

```powershell
# 编译主程序（用系统自带的 csc，无需安装 VS）
& "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe /platform:anycpu /optimize+ `
  /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll `
  /win32icon:"favicon.ico" /out:"PSHistoryCleaner.exe" "src\PSHistoryCleaner.cs"

# 打安装包（NSIS 3.x，需已安装或解压 NSIS）
cd installer
makensis installer.nsi
```

## 安装包 / Installer

NSIS 安装包（MUI2，界面可选 简中/繁體/English）：

- 按用户安装到 `%LOCALAPPDATA%\Programs\PSHistoryCleaner`，不需要管理员权限
- 开始菜单快捷方式 + 可选桌面快捷方式
- 可选「随系统开机自动启动」：写入 `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`，值为 `"…\PSHistoryCleaner.exe" /hidden`
- 安装/卸载前会自动结束正在运行的实例；卸载不删除 `%APPDATA%\PSHistoryCleaner`（配置保留）
- 另提供绿色版：解压即用，不写系统目录

## License

MIT
