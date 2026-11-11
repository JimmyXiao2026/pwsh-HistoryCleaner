# 一键编译：主程序 + NSIS 安装包
# 用法：powershell -ExecutionPolicy Bypass -File .\build.ps1 [-SkipInstaller]

param(
    [switch]$SkipInstaller,
    [string]$NsisDir = "C:\yilaiku\NSIS"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$csc = "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path -LiteralPath $csc)) {
    $csc = "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\csc.exe"
}

Write-Host "==> 编译 PSHistoryCleaner.exe"
& $csc /nologo /target:winexe /platform:anycpu /optimize+ `
    /r:System.dll /r:System.Drawing.dll /r:System.Windows.Forms.dll `
    /win32icon:"$root\favicon.ico" /out:"$root\PSHistoryCleaner.exe" "$root\src\PSHistoryCleaner.cs"
if ($LASTEXITCODE -ne 0) { throw "csc 编译失败，exit=$LASTEXITCODE" }
Write-Host "    -> $root\PSHistoryCleaner.exe"

if ($SkipInstaller) { return }

$makensis = Join-Path $NsisDir "makensis.exe"
if (-not (Test-Path -LiteralPath $makensis)) { throw "找不到 makensis.exe：$makensis" }

Write-Host "==> 生成 NSIS 安装包"
Push-Location "$root\installer"
& $makensis /V2 installer.nsi
$code = $LASTEXITCODE
Pop-Location
if ($code -ne 0) { throw "makensis 失败，exit=$code" }
Write-Host "    -> $root\PowerShell历史清理_v1.0.0_安装包.exe"
