Unicode true
SetCompressor /SOLID lzma
SetCompressorDictSize 32

!define APPNAME "PowerShell 历史清理"
!define APPVER "1.0.0"
!define PUBLISHER "JimmyXiao"
!define URL "https://github.com/JimmyXiao2026/pwsh-HistoryCleaner"
!define REGUNINST "Software\Microsoft\Windows\CurrentVersion\Uninstall\PSHistoryCleaner"

Name "${APPNAME}"
Caption "${APPNAME} ${APPVER} 安装程序"
OutFile "..\PowerShell历史清理_v${APPVER}_安装包.exe"
InstallDir "$LOCALAPPDATA\Programs\PSHistoryCleaner"
InstallDirRegKey HKCU "Software\PSHistoryCleaner" "InstallDir"
RequestExecutionLevel user
ShowInstDetails show
ShowUninstDetails show

!include "MUI2.nsh"

!define MUI_ICON "..\favicon.ico"
!define MUI_UNICON "..\favicon.ico"
!define MUI_ABORTWARNING
!define MUI_FINISHPAGE_RUN "$INSTDIR\PSHistoryCleaner.exe"
!define MUI_FINISHPAGE_RUN_TEXT "$(FINISH_RUN)"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "SimpChinese"
!insertmacro MUI_LANGUAGE "TradChinese"
!insertmacro MUI_LANGUAGE "English"

LangString FINISH_RUN ${LANG_SIMPCHINESE} "立即运行 ${APPNAME}"
LangString FINISH_RUN ${LANG_TRADCHINESE} "立即執行 ${APPNAME}"
LangString FINISH_RUN ${LANG_ENGLISH} "Run ${APPNAME} now"

LangString APP_DISPLAY ${LANG_SIMPCHINESE} "PowerShell 历史清理"
LangString APP_DISPLAY ${LANG_TRADCHINESE} "PowerShell 歷史清理"
LangString APP_DISPLAY ${LANG_ENGLISH} "PowerShell History Cleaner"

LangString SEC_CORE ${LANG_SIMPCHINESE} "程序文件（必需）"
LangString SEC_CORE ${LANG_TRADCHINESE} "程式檔案（必需）"
LangString SEC_CORE ${LANG_ENGLISH} "Program files (required)"

LangString SEC_DESKTOP ${LANG_SIMPCHINESE} "创建桌面快捷方式"
LangString SEC_DESKTOP ${LANG_TRADCHINESE} "建立桌面捷徑"
LangString SEC_DESKTOP ${LANG_ENGLISH} "Create desktop shortcut"

LangString SEC_AUTOSTART ${LANG_SIMPCHINESE} "随系统开机自动启动（静默 /hidden）"
LangString SEC_AUTOSTART ${LANG_TRADCHINESE} "隨系統開機自動啟動（靜默 /hidden）"
LangString SEC_AUTOSTART ${LANG_ENGLISH} "Start with Windows (silent /hidden)"

LangString DESC_CORE ${LANG_SIMPCHINESE} "主程序、开始菜单快捷方式与卸载程序。"
LangString DESC_CORE ${LANG_TRADCHINESE} "主程式、開始功能表捷徑與解除安裝程式。"
LangString DESC_CORE ${LANG_ENGLISH} "Main program, Start menu shortcuts and the uninstaller."

LangString DESC_DESKTOP ${LANG_SIMPCHINESE} "在桌面放一个快捷方式。"
LangString DESC_DESKTOP ${LANG_TRADCHINESE} "在桌面放置一個捷徑。"
LangString DESC_DESKTOP ${LANG_ENGLISH} "Put a shortcut on the Desktop."

LangString DESC_AUTOSTART ${LANG_SIMPCHINESE} "写入 HKCU 的 Run 项，开机后带 /hidden 参数静默在后台运行，不弹界面。"
LangString DESC_AUTOSTART ${LANG_TRADCHINESE} "寫入 HKCU 的 Run 值，開機後帶 /hidden 參數靜默在背景執行，不彈出介面。"
LangString DESC_AUTOSTART ${LANG_ENGLISH} "Adds an HKCU Run entry that launches silently with /hidden at logon."

Section "$(SEC_CORE)" SecCore
  SectionIn RO
  nsExec::Exec 'taskkill /f /im PSHistoryCleaner.exe'
  Sleep 400
  SetOutPath "$INSTDIR"
  SetOverwrite try
  File "..\PSHistoryCleaner.exe"
  WriteUninstaller "$INSTDIR\uninstall.exe"

  WriteRegStr HKCU "Software\PSHistoryCleaner" "InstallDir" "$INSTDIR"
  WriteRegStr HKCU "${REGUNINST}" "DisplayName" "$(APP_DISPLAY)"
  WriteRegStr HKCU "${REGUNINST}" "DisplayVersion" "${APPVER}"
  WriteRegStr HKCU "${REGUNINST}" "Publisher" "${PUBLISHER}"
  WriteRegStr HKCU "${REGUNINST}" "DisplayIcon" "$INSTDIR\PSHistoryCleaner.exe"
  WriteRegStr HKCU "${REGUNINST}" "URLInfoAbout" "${URL}"
  WriteRegStr HKCU "${REGUNINST}" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegStr HKCU "${REGUNINST}" "QuietUninstallString" '"$INSTDIR\uninstall.exe" /S'
  WriteRegDWORD HKCU "${REGUNINST}" "NoModify" 1
  WriteRegDWORD HKCU "${REGUNINST}" "NoRepair" 1
  WriteRegDWORD HKCU "${REGUNINST}" "EstimatedSize" 512

  CreateDirectory "$SMPROGRAMS\PowerShell 历史清理"
  CreateShortCut "$SMPROGRAMS\PowerShell 历史清理\PowerShell 历史清理.lnk" "$INSTDIR\PSHistoryCleaner.exe" "" "$INSTDIR\PSHistoryCleaner.exe" 0
  CreateShortCut "$SMPROGRAMS\PowerShell 历史清理\卸载 PowerShell 历史清理.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
SectionEnd

Section "$(SEC_DESKTOP)" SecDesktop
  CreateShortCut "$DESKTOP\PowerShell 历史清理.lnk" "$INSTDIR\PSHistoryCleaner.exe" "" "$INSTDIR\PSHistoryCleaner.exe" 0
SectionEnd

Section "$(SEC_AUTOSTART)" SecAutostart
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "PSHistoryCleaner" '"$INSTDIR\PSHistoryCleaner.exe" /hidden'
SectionEnd

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecCore} "$(DESC_CORE)"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} "$(DESC_DESKTOP)"
  !insertmacro MUI_DESCRIPTION_TEXT ${SecAutostart} "$(DESC_AUTOSTART)"
!insertmacro MUI_FUNCTION_DESCRIPTION_END

Function .onInit
  ; 兼容旧版的安装位置记录
  ReadRegStr $0 HKCU "Software\PSHistoryCleaner" "InstallDir"
  ${If} $0 != ""
    StrCpy $INSTDIR $0
  ${EndIf}
FunctionEnd

Section "Uninstall"
  nsExec::Exec 'taskkill /f /im PSHistoryCleaner.exe'
  Sleep 400

  Delete "$DESKTOP\PowerShell 历史清理.lnk"
  Delete "$SMPROGRAMS\PowerShell 历史清理\PowerShell 历史清理.lnk"
  Delete "$SMPROGRAMS\PowerShell 历史清理\卸载 PowerShell 历史清理.lnk"
  RMDir "$SMPROGRAMS\PowerShell 历史清理"

  DeleteRegValue HKCU "Software\Microsoft\Windows\CurrentVersion\Run" "PSHistoryCleaner"
  DeleteRegKey HKCU "${REGUNINST}"
  DeleteRegKey HKCU "Software\PSHistoryCleaner"

  Delete "$INSTDIR\PSHistoryCleaner.exe"
  Delete "$INSTDIR\uninstall.exe"
  Delete "$INSTDIR\*.log"
  RMDir "$INSTDIR"

  ; 用户配置保存于 %APPDATA%\PSHistoryCleaner，默认保留；需要完全清理请手动删除该目录
SectionEnd
