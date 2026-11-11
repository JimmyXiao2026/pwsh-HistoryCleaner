# PowerShell 历史清理

[English](README.md) | **简体中文**

一个常驻后台的小工具：每 5 分钟检查 PowerShell 的 `ConsoleHost_history.txt`，按黑名单（包含匹配）/白名单（整行匹配）规则自动清理历史命令。界面支持 **简体中文 / 繁體中文 / English**。

- 目标文件：`%APPDATA%\Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt`
- 运行环境：Windows 7+，.NET Framework 4.x（系统自带，零第三方依赖）
- 技术栈：单文件 C# WinForms（`src/PSHistoryCleaner.cs`），用系统自带 `csc.exe` 编译为 `winexe`（无控制台窗口）

## 功能

| 功能 | 说明 |
| --- | --- |
| 黑名单 | 一行里**包含**关键字即命中 |
| 白名单 | 被命中的行只有**整行完全相同**时才保留（保护） |
| 删除方式 | 先列举命中的行，再整行删除，不留空行、不生成备份 |
| 重复去重 | **完全重复的命令只保留最后一条**（5 行 `cls` 只留最后那行），比较时去掉首尾空白且忽略大小写 |
| 后台运行 | 每 5 分钟检查一次，全程静默，不弹任何窗口 |
| 配置界面 | 固定尺寸：上半白名单、下半黑名单，无限增删，各自独立滚动 |
| 单实例 | 重复双击 exe 不会开第二个窗口，只把已有窗口唤到前台 |
| 左下快捷键 | 创建开机自启动（写 `HKCU\...\Run`，带 `/hidden`）、在任务栏中显示、创建开始目录快捷方式 |
| 多语言 | 标题栏语言按钮循环 简中 / 繁體 / EN，首次按系统语言自动选择并记住 |
| 外观 | 圆角自绘 UI，内置应用图标 |

## 命令行参数

| 参数 | 说明 |
| --- | --- |
| `/hidden` | 静默启动，只跑后台引擎，不显示界面（开机自启动用的就是它） |
| `/checkonce` | 立刻检查一次后退出（排查用） |

## 配置与日志

- `%APPDATA%\PSHistoryCleaner\config.ini` —— `[white]`、`[black]`、`[setting]`（`tray`、`lang`、`removed`、`last`）
- `%APPDATA%\PSHistoryCleaner\cleaner.log` —— 后台处理记录，超过 512 KB 轮换为 `.old`
- 自测钩子：环境变量 `PSHC_CONFIG_DIR`、`PSHC_HISTORY_PATH` 可把配置目录和历史文件指向测试位置，不污染真实环境

## 构建

```powershell
# 编译主程序（用系统自带 csc，无需装 VS）
& "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe /platform:anycpu /optimize+ `
  /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll `
  /win32icon:"favicon.ico" /out:"PSHistoryCleaner.exe" "src\PSHistoryCleaner.cs"

# 打安装包（NSIS 3.x）
cd installer
makensis installer.nsi
```

也可以直接跑 `build.ps1`（加 `-SkipInstaller` 只编 exe）。

## 下载与安装

每个 Release 附带两个文件：

- **安装包**（`PowerShellHistoryCleaner_v1.0.0_Setup.exe`）—— NSIS/MUI2，安装时可选 简中 / 繁體 / English
  - 按用户安装到 `%LOCALAPPDATA%\Programs\PSHistoryCleaner`，**不需要管理员权限、不弹 UAC**
  - 开始菜单快捷方式，可选桌面快捷方式
  - 可选「随系统开机自动启动」：`HKCU\Software\Microsoft\Windows\CurrentVersion\Run` = `"…\PSHistoryCleaner.exe" /hidden`
  - 安装/卸载前自动结束正在运行的实例
  - 带卸载器，并出现在「设置 → 应用」；卸载时保留 `%APPDATA%\PSHistoryCleaner` 里的配置
- **绿色版**（`PowerShellHistoryCleaner_v1.0.0_Portable.zip`）—— 解压即用，不写系统目录

## 说明

- 暂未签名：首次运行可能被 SmartScreen 拦一下，确认来源后点「更多信息 → 仍要运行」。
- 清理时对 `ConsoleHost_history.txt` 采用临时文件 + 原子替换（带重试），并保留原始换行风格，不会生成备份文件。

## 许可证

MIT
