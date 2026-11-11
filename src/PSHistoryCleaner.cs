using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace PSHistoryCleaner
{
    static class Theme
    {
        public static Color Bg = Color.FromArgb(243, 245, 249);
        public static Color Card = Color.White;
        public static Color CardLine = Color.FromArgb(228, 232, 240);
        public static Color Text = Color.FromArgb(32, 37, 49);
        public static Color Sub = Color.FromArgb(106, 116, 134);
        public static Color Muted = Color.FromArgb(152, 160, 174);
        public static Color Accent = Color.FromArgb(37, 99, 235);
        public static Color AccentHover = Color.FromArgb(56, 118, 248);
        public static Color AccentDown = Color.FromArgb(28, 82, 205);
        public static Color Green = Color.FromArgb(21, 158, 96);
        public static Color GreenHover = Color.FromArgb(33, 176, 108);
        public static Color GreenDown = Color.FromArgb(15, 132, 79);
        public static Color Red = Color.FromArgb(212, 55, 55);
        public static Color RedHover = Color.FromArgb(228, 72, 72);
        public static Color RedDown = Color.FromArgb(182, 42, 42);
        public static Color SoftGreen = Color.FromArgb(233, 249, 242);
        public static Color SoftRed = Color.FromArgb(253, 238, 238);
        public static Color SoftBlue = Color.FromArgb(233, 241, 254);
        public static Color SoftGray = Color.FromArgb(239, 242, 247);
        public static Color SoftGrayDown = Color.FromArgb(226, 231, 240);
        public static Color InputFill = Color.FromArgb(247, 248, 251);
        public static Color Line = Color.FromArgb(225, 229, 237);
        public static Color LogFill = Color.FromArgb(250, 251, 253);
        public static Color BadgeA = Color.FromArgb(59, 130, 246);
        public static Color BadgeB = Color.FromArgb(29, 78, 216);

        static string family;
        public static string Family
        {
            get
            {
                if (family != null) return family;
                string[] names = new string[] { "Microsoft YaHei UI", "Microsoft YaHei", "Segoe UI", "Arial" };
                for (int i = 0; i < names.Length; i++)
                {
                    try { FontFamily ff = new FontFamily(names[i]); ff.Dispose(); family = names[i]; return family; }
                    catch { }
                }
                family = "Arial";
                return family;
            }
        }

        public static Font F(float size, FontStyle style)
        {
            return new Font(Family, size, style, GraphicsUnit.Point);
        }

        public static Color Mix(Color a, Color b, float t)
        {
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }
    }

    static class Gfx
    {
        public static GraphicsPath Round(Rectangle r, int radius)
        {
            GraphicsPath p = new GraphicsPath();
            if (radius <= 0 || r.Width <= 0 || r.Height <= 0)
            {
                p.AddRectangle(r);
                return p;
            }
            int d = radius * 2;
            if (d > r.Width) d = r.Width;
            if (d > r.Height) d = r.Height;
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static void FillRound(Graphics g, Rectangle r, int radius, Color c)
        {
            if (c.A == 0) return;
            using (GraphicsPath p = Round(r, radius))
            using (SolidBrush b = new SolidBrush(c))
                g.FillPath(b, p);
        }

        public static void DrawRound(Graphics g, Rectangle r, int radius, Color c, float w)
        {
            if (c.A == 0) return;
            Rectangle rr = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);
            using (GraphicsPath p = Round(rr, radius))
            using (Pen pen = new Pen(c, w))
                g.DrawPath(pen, p);
        }
    }

    // ---------- 多语言 ----------
    static class L
    {
        public const int ZH_CN = 0;
        public const int ZH_TW = 1;
        public const int EN = 2;
        public static int Cur = ZH_CN;

        public static readonly string[] Names = new string[] { "简体中文", "繁體中文", "English" };
        public static readonly string[] ShortNames = new string[] { "简中", "繁體", "EN" };

        static readonly Dictionary<string, string[]> Map = new Dictionary<string, string[]>();

        static void A(string key, string cn, string tw, string en)
        {
            Map[key] = new string[] { cn, tw, en };
        }

        static L()
        {
            A("app.title", "PowerShell 历史清理", "PowerShell 歷史清理", "PowerShell History Cleaner");
            A("app.subtitle",
              "自动匹配黑名单并清理 ConsoleHost_history.txt · 每 5 分钟后台检查一次",
              "自動比對黑名單並清理 ConsoleHost_history.txt · 每 5 分鐘於背景檢查一次",
              "Cleans ConsoleHost_history.txt by blacklist · checked in background every 5 minutes");
            A("card.white", "白名单", "白名單", "Whitelist");
            A("hint.white", "整行完全命中则保留", "整行完全符合則保留", "kept only on a full-line match");
            A("card.black", "黑名单", "黑名單", "Blacklist");
            A("hint.black", "行内包含关键字即命中", "行內包含關鍵字即命中", "hit when the line contains it");
            A("card.log", "处理记录", "處理記錄", "Activity log");
            A("hint.log", "最近 300 条结果 · 完全重复的命令只保留最后一条",
              "最近 300 筆結果 · 完全重複的指令只保留最後一條",
              "last 300 results · duplicate commands reduced to the last one");
            A("count", "共 {0} 条", "共 {0} 條", "{0} item(s)");
            A("placeholder", "输入要匹配的命令片段…", "輸入要符合的指令片段…", "Type a command fragment…");
            A("btn.add", "添加", "新增", "Add");
            A("btn.del", "删除选中", "刪除選取", "Delete selected");
            A("btn.startup.on", "取消开机自启动", "取消開機自動啟動", "Disable auto-start");
            A("btn.startup.off", "创建开机自启动", "建立開機自動啟動", "Enable auto-start");
            A("btn.tray.on", "隐藏任务栏图标", "隱藏工作列圖示", "Hide tray icon");
            A("btn.tray.off", "在任务栏中显示", "在工作列中顯示", "Show tray icon");
            A("btn.shortcut", "创建开始目录快捷方式", "建立開始目錄捷徑", "Start menu shortcut");
            A("btn.check", "立即检查", "立即檢查", "Check now");
            A("btn.exit", "退出", "結束", "Exit");
            A("empty.list", "暂无条目 · 在下方输入后用「添加」新增", "尚無項目 · 於下方輸入後按「新增」加入", "No entries yet — type below and press Add");
            A("status", "上次检查 {0} · 累计清理 {1} 行", "上次檢查 {0} · 累計清理 {1} 行", "Last check {0} · {1} line(s) removed");
            A("status.never", "尚未检查", "尚未檢查", "not yet");
            A("role.white", "白名单", "白名單", "whitelist");
            A("role.black", "黑名单", "黑名單", "blacklist");
            A("log.added", "已添加{0}规则：{1}", "已新增{0}規則：{1}", "Added {0} rule: {1}");
            A("log.dup", "该规则已存在：{0}", "該規則已存在：{0}", "Rule already exists: {0}");
            A("log.removed", "已删除{0}规则：{1}", "已刪除{0}規則：{1}", "Removed {0} rule: {1}");
            A("log.startup.off", "已取消开机自启动", "已取消開機自動啟動", "Auto-start disabled");
            A("log.startup.on", "已创建开机自启动（参数 /hidden，开机后静默在后台运行）",
              "已建立開機自動啟動（參數 /hidden，開機後於背景靜默執行）",
              "Auto-start enabled (runs with /hidden, silently in background)");
            A("log.startup.fail", "设置开机自启动失败：{0}", "設定開機自動啟動失敗：{0}", "Failed to change auto-start: {0}");
            A("log.shortcut.ok", "已创建开始菜单快捷方式：{0}", "已建立開始功能表捷徑：{0}", "Start menu shortcut created: {0}");
            A("log.shortcut.fail", "创建开始菜单快捷方式失败：{0}", "建立開始功能表捷徑失敗：{0}", "Failed to create Start menu shortcut: {0}");
            A("tray.open", "打开配置界面", "開啟設定介面", "Open settings");
            A("tray.check", "立即检查一次", "立即檢查一次", "Check now");
            A("tray.startup", "开机自启动（隐藏启动）", "開機自動啟動（隱藏啟動）", "Auto-start (hidden launch)");
            A("tray.exit", "退出程序", "結束程式", "Exit");
            A("tray.tip", "PowerShell 历史清理（每 5 分钟检查）", "PowerShell 歷史清理（每 5 分鐘檢查）", "PowerShell History Cleaner (every 5 min)");
            A("eng.match", "命中黑名单 {0} 行：", "命中黑名單 {0} 行：", "{0} line(s) matched the blacklist:");
            A("eng.kept", "其中 {0} 行完全命中白名单，已保留", "其中 {0} 行完全符合白名單，已保留", "{0} of them matched the whitelist fully and were kept");
            A("eng.deleted", "已删除 {0} 行", "已刪除 {0} 行", "Deleted {0} line(s)");
            A("eng.deletefail", "需要删除 {0} 行，但写入失败，将在下次重试",
              "需要刪除 {0} 行，但寫入失敗，將於下次重試",
              "{0} line(s) to delete but the write failed; will retry");
            A("eng.none", "无需删除", "無需刪除", "Nothing to delete");
            A("eng.dup", "删除完全重复的命令 {0} 行（只保留最后一条）",
              "刪除完全重複的指令 {0} 行（只保留最後一條）",
              "{0} duplicate line(s) removed (only the last occurrence kept)");
            A("eng.clean", "检查完成：历史记录中没有命中黑名单的行",
              "檢查完成：歷史記錄中沒有命中黑名單的行",
              "Check finished: no blacklist hits");
            A("eng.error", "检查失败：{0}", "檢查失敗：{0}", "Check failed: {0}");
            A("err.read", "读取历史文件失败（文件被占用或权限不足）",
              "讀取歷史檔案失敗（檔案被占用或權限不足）",
              "Failed to read the history file (locked or access denied)");
        }

        public static string T(string key)
        {
            string[] v;
            if (Map.TryGetValue(key, out v)) return v[Cur];
            return key;
        }

        public static string F(string key, params object[] args)
        {
            return string.Format(T(key), args);
        }

        public static int Detect()
        {
            string n = System.Globalization.CultureInfo.CurrentUICulture.Name.ToLowerInvariant();
            if (n.StartsWith("zh-tw") || n.StartsWith("zh-hk") || n.StartsWith("zh-mo") || n.StartsWith("zh-hant")) return ZH_TW;
            if (n.StartsWith("zh")) return ZH_CN;
            return EN;
        }
    }

    class Spec
    {
        public Rectangle R;
        public int Radius;
        public Color Fill;
        public Color Border;
        public Spec(Rectangle r, int radius, Color fill, Color border)
        {
            R = r; Radius = radius; Fill = fill; Border = border;
        }
    }

    class Config
    {
        public List<string> White = new List<string>();
        public List<string> Black = new List<string>();
        public bool TrayEnabled = true;
        public int LangIndex = -1;
        public int TotalRemoved = 0;
        public string LastCheck = "";
        public string Dir;
        public string ConfigFile;
        readonly object sync = new object();
        readonly UTF8Encoding utf8 = new UTF8Encoding(false);

        public Config()
        {
            string ov = Environment.GetEnvironmentVariable("PSHC_CONFIG_DIR");
            if (ov != null && ov.Trim().Length > 0)
                Dir = ov;
            else
                Dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PSHistoryCleaner");
            try { Directory.CreateDirectory(Dir); } catch { }
            ConfigFile = System.IO.Path.Combine(Dir, "config.ini");
            Load();
        }

        public void Load()
        {
            lock (sync)
            {
                White.Clear();
                Black.Clear();
                try
                {
                    if (!File.Exists(ConfigFile)) return;
                    string section = "";
                    string[] lines = File.ReadAllLines(ConfigFile, Encoding.UTF8);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string raw = lines[i];
                        string line = raw.Trim();
                        if (line.Length == 0) continue;
                        if (line.Length > 2 && line.StartsWith("[") && line.EndsWith("]"))
                        {
                            section = line.Substring(1, line.Length - 2).Trim().ToLowerInvariant();
                            continue;
                        }
                        if (section == "white") { White.Add(line); continue; }
                        if (section == "black") { Black.Add(line); continue; }
                        if (section == "setting")
                        {
                            int eq = line.IndexOf('=');
                            if (eq <= 0) continue;
                            string k = line.Substring(0, eq).Trim().ToLowerInvariant();
                            string v = line.Substring(eq + 1).Trim();
                            if (k == "tray") TrayEnabled = (v == "1");
                            else if (k == "lang") { try { LangIndex = int.Parse(v); } catch { } }
                            else if (k == "removed") { try { TotalRemoved = int.Parse(v); } catch { } }
                            else if (k == "last") LastCheck = v;
                        }
                    }
                }
                catch { }
            }
        }

        public void Save()
        {
            lock (sync)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("; PowerShell 历史清理 配置文件（每行一条规则，[white] 为白名单，[black] 为黑名单）");
                    sb.AppendLine("[white]");
                    for (int i = 0; i < White.Count; i++) sb.AppendLine(White[i]);
                    sb.AppendLine("[black]");
                    for (int i = 0; i < Black.Count; i++) sb.AppendLine(Black[i]);
                    sb.AppendLine("[setting]");
                    sb.AppendLine("tray=" + (TrayEnabled ? "1" : "0"));
                    sb.AppendLine("lang=" + LangIndex);
                    sb.AppendLine("removed=" + TotalRemoved);
                    sb.AppendLine("last=" + LastCheck);
                    File.WriteAllText(ConfigFile, sb.ToString(), utf8);
                }
                catch { }
            }
        }

        public void Snapshot(out List<string> white, out List<string> black)
        {
            lock (sync)
            {
                white = new List<string>(White);
                black = new List<string>(Black);
            }
        }

        public bool Add(bool toWhite, string entry)
        {
            lock (sync)
            {
                if (toWhite)
                {
                    if (White.Contains(entry)) return false;
                    White.Add(entry);
                }
                else
                {
                    if (Black.Contains(entry)) return false;
                    Black.Add(entry);
                }
                Save();
                return true;
            }
        }

        public bool RemoveAt(bool fromWhite, int index)
        {
            lock (sync)
            {
                List<string> list = fromWhite ? White : Black;
                if (index < 0 || index >= list.Count) return false;
                list.RemoveAt(index);
                Save();
                return true;
            }
        }

        public void AddStats(int removed, string last)
        {
            lock (sync)
            {
                TotalRemoved += removed;
                LastCheck = last;
                Save();
            }
        }
    }

    class CheckResult
    {
        public List<string> Matched = new List<string>();
        public List<string> KeptByWhite = new List<string>();
        public List<string> Duplicates = new List<string>();
        public int RemovedCount = 0;
        public bool Written = true;
        public bool NoFile = false;
        public string Error = null;
        public DateTime Time = DateTime.Now;
    }

    class Engine
    {
        public const int IntervalMs = 5 * 60 * 1000;
        readonly Config cfg;
        readonly object runLock = new object();
        Thread worker;
        volatile bool stop = false;

        public Action<string> Logger;
        public Action OnStatsChanged;

        public Engine(Config c) { cfg = c; }

        public static string HistoryPath()
        {
            string ov = Environment.GetEnvironmentVariable("PSHC_HISTORY_PATH");
            if (ov != null && ov.Trim().Length > 0) return ov;
            return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Microsoft\Windows\PowerShell\PSReadLine\ConsoleHost_history.txt");
        }

        public void Start()
        {
            if (worker != null) return;
            worker = new Thread(Loop);
            worker.IsBackground = true;
            worker.Name = "PSHistoryCleanerWorker";
            worker.Start();
        }

        public void Stop()
        {
            stop = true;
        }

        void Loop()
        {
            Thread.Sleep(2000);
            while (!stop)
            {
                RunOnce();
                int step = 1000;
                int waited = 0;
                while (waited < IntervalMs && !stop)
                {
                    Thread.Sleep(step);
                    waited += step;
                }
            }
        }

        public void RunOnce()
        {
            CheckResult r;
            lock (runLock) { r = Check(); }
            Report(r);
            try { cfg.AddStats(r.RemovedCount, r.Time.ToString("HH:mm:ss")); } catch { }
            Action h = OnStatsChanged;
            if (h != null) { try { h(); } catch { } }
        }

        public CheckResult Check()
        {
            CheckResult res = new CheckResult();
            string path = HistoryPath();
            if (!File.Exists(path)) { res.NoFile = true; return res; }

            string text = ReadText(path);
            if (text == null) { res.Error = L.T("err.read"); return res; }

            List<string> white, black;
            cfg.Snapshot(out white, out black);

            List<Seg> segs = SplitLines(text);
            int count = segs.Count;
            bool[] protect = new bool[count];   // 白名单保护
            bool[] drop = new bool[count];      // 需要删除

            // 第一遍：黑名单命中判定 + 白名单完整匹配
            for (int i = 0; i < count; i++)
            {
                string trimmed = segs[i].Text.Trim();
                if (trimmed.Length == 0) continue;

                bool hitBlack = false;
                for (int b = 0; b < black.Count; b++)
                {
                    string rule = black[b];
                    if (rule.Length == 0) continue;
                    if (segs[i].Text.IndexOf(rule, StringComparison.OrdinalIgnoreCase) >= 0) { hitBlack = true; break; }
                }
                if (!hitBlack) continue;

                res.Matched.Add(segs[i].Text);

                bool allow = false;
                for (int w = 0; w < white.Count; w++)
                {
                    string rule = white[w];
                    if (rule.Length == 0) continue;
                    if (string.Equals(rule, trimmed, StringComparison.OrdinalIgnoreCase)) { allow = true; break; }
                }
                if (allow)
                {
                    protect[i] = true;
                    res.KeptByWhite.Add(segs[i].Text);
                }
                else
                {
                    drop[i] = true;
                }
            }

            // 第二遍：完全重复的命令只保留最后一条
            Dictionary<string, int> lastIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < count; i++)
            {
                if (drop[i]) continue;
                string key = segs[i].Text.Trim();
                if (key.Length == 0) continue;
                lastIndex[key] = i;
            }
            for (int i = 0; i < count; i++)
            {
                if (drop[i]) continue;
                string key = segs[i].Text.Trim();
                if (key.Length == 0) continue;
                int last;
                if (!lastIndex.TryGetValue(key, out last)) continue;
                if (last != i && !protect[i])
                {
                    drop[i] = true;
                    res.Duplicates.Add(segs[i].Text);
                }
            }

            // 重组内容：删除整行（含行尾换行），不会留下空行
            StringBuilder kept = new StringBuilder(text.Length + 16);
            int removed = 0;
            for (int i = 0; i < count; i++)
            {
                if (drop[i]) { removed++; continue; }
                kept.Append(segs[i].Raw);
            }

            res.RemovedCount = removed;
            if (removed > 0) res.Written = WriteText(path, kept.ToString());
            return res;
        }

        class Seg
        {
            public string Text;
            public string Raw;
        }

        static List<Seg> SplitLines(string text)
        {
            List<Seg> list = new List<Seg>();
            int i = 0, n = text.Length;
            while (i < n)
            {
                int j = text.IndexOf('\n', i);
                string content;
                string term;
                if (j < 0) { content = text.Substring(i); term = ""; }
                else { content = text.Substring(i, j - i); term = "\n"; }
                if (term == "\n" && content.Length > 0 && content[content.Length - 1] == '\r')
                {
                    content = content.Substring(0, content.Length - 1);
                    term = "\r\n";
                }
                Seg s = new Seg();
                s.Text = content;
                s.Raw = content + term;
                list.Add(s);
                if (j < 0) break;
                i = j + 1;
            }
            return list;
        }

        static string ReadText(string path)
        {
            for (int attempt = 0; attempt < 8; attempt++)
            {
                try
                {
                    using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                    using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))
                        return sr.ReadToEnd();
                }
                catch { Thread.Sleep(200); }
            }
            return null;
        }

        static bool WriteText(string path, string content)
        {
            UTF8Encoding enc = new UTF8Encoding(false);
            string tmp = path + ".pchc.tmp";
            bool tmpOk = false;
            try { File.WriteAllText(tmp, content, enc); tmpOk = true; }
            catch { }

            for (int attempt = 0; attempt < 8; attempt++)
            {
                try
                {
                    if (tmpOk && File.Exists(tmp))
                    {
                        if (File.Exists(path))
                        {
                            try { File.Replace(tmp, path, null); }
                            catch
                            {
                                File.Copy(tmp, path, true);
                                try { File.Delete(tmp); } catch { }
                            }
                        }
                        else
                        {
                            File.Move(tmp, path);
                        }
                    }
                    else
                    {
                        File.WriteAllText(path, content, enc);
                    }
                    return true;
                }
                catch { Thread.Sleep(250); }
            }
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch { }
            return false;
        }

        void Report(CheckResult r)
        {
            string stamp = "[" + r.Time.ToString("HH:mm:ss") + "]";
            StringBuilder sb = new StringBuilder();
            if (r.Error != null)
            {
                sb.AppendLine(stamp + " " + L.F("eng.error", r.Error));
            }
            else if (r.NoFile)
            {
                return;
            }
            else if (r.Matched.Count == 0 && r.Duplicates.Count == 0)
            {
                sb.AppendLine(stamp + " " + L.T("eng.clean"));
            }
            else
            {
                if (r.Matched.Count > 0)
                {
                    sb.AppendLine(stamp + " " + L.F("eng.match", r.Matched.Count));
                    for (int i = 0; i < r.Matched.Count; i++) sb.AppendLine("    " + Truncate(r.Matched[i], 96));
                    if (r.KeptByWhite.Count > 0)
                        sb.AppendLine(stamp + " " + L.F("eng.kept", r.KeptByWhite.Count));
                }
                else
                {
                    sb.AppendLine(stamp + " " + L.T("eng.clean"));
                }
                if (r.Duplicates.Count > 0)
                    sb.AppendLine(stamp + " " + L.F("eng.dup", r.Duplicates.Count));
                if (r.RemovedCount > 0)
                    sb.AppendLine(stamp + " " + (r.Written ? L.F("eng.deleted", r.RemovedCount) : L.F("eng.deletefail", r.RemovedCount)));
                else if (r.Matched.Count > 0)
                    sb.AppendLine(stamp + " " + L.T("eng.none"));
            }
            Emit(sb.ToString().TrimEnd());
        }

        static string Truncate(string s, int len)
        {
            if (s == null) return "";
            s = s.Replace("\r", "").Replace("\n", " ");
            if (s.Length <= len) return s;
            return s.Substring(0, len) + "…";
        }

        void Emit(string block)
        {
            FileLog(block);
            Action<string> lg = Logger;
            if (lg != null) { try { lg(block); } catch { } }
        }

        public void FileLog(string block)
        {
            try
            {
                string f = System.IO.Path.Combine(cfg.Dir, "cleaner.log");
                FileInfo fi = new FileInfo(f);
                if (fi.Exists && fi.Length > 512 * 1024)
                {
                    try { File.Delete(f + ".old"); File.Move(f, f + ".old"); } catch { }
                }
                File.AppendAllText(f, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + block + Environment.NewLine, new UTF8Encoding(false));
            }
            catch { }
        }
    }

    class PillButton : Button
    {
        public Color Fill = Color.Transparent;
        public Color Hover = Theme.SoftGray;
        public Color Down = Theme.SoftGrayDown;
        public Color Txt = Theme.Text;
        public Color Border = Color.Transparent;
        public Color Surround = Theme.Bg;   // 按钮周围的实际底色，用来填满圆角以外的区域
        public int Radius = 9;
        public Font DrawFont;
        bool hovering, pressing;

        public PillButton()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.Opaque | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Theme.Bg;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            UseVisualStyleBackColor = false;
            Font = Theme.F(9.5F, FontStyle.Regular);
            DrawFont = Font;
            Cursor = Cursors.Hand;
            TabStop = false;
        }

        protected override void OnMouseEnter(EventArgs e) { hovering = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { hovering = false; pressing = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs mevent) { pressing = true; Invalidate(); base.OnMouseDown(mevent); }
        protected override void OnMouseUp(MouseEventArgs mevent) { pressing = false; Invalidate(); base.OnMouseUp(mevent); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(0, 0, Width, Height);
            using (SolidBrush sb = new SolidBrush(Surround))
                g.FillRectangle(sb, r);
            if (!Enabled)
            {
                Gfx.FillRound(g, r, Radius, Theme.SoftGray);
                TextRenderer.DrawText(g, Text, DrawFont, r, Theme.Muted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                return;
            }
            Color fill = pressing ? Down : (hovering ? Hover : Fill);
            Gfx.FillRound(g, r, Radius, fill);
            if (Border.A > 0) Gfx.DrawRound(g, r, Radius, Border, 1f);
            TextRenderer.DrawText(g, Text, DrawFont, r, Txt,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }

    class MainForm : Form
    {
        [DllImport("user32.dll")] static extern bool ReleaseCapture();
        [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HTCAPTION = 0x2;

        static readonly int WW = 920;
        static readonly int HH = 700;

        readonly Rectangle rCardWhite = new Rectangle(24, 78, 872, 214);
        readonly Rectangle rCardBlack = new Rectangle(24, 300, 872, 214);
        readonly Rectangle rCardLog = new Rectangle(24, 526, 872, 96);

        readonly List<Spec> specs = new List<Spec>();

        Config cfg;
        Engine engine;
        bool exiting = false;

        ListBox lbWhite, lbBlack;
        TextBox tbWhite, tbBlack, tbLog;
        Label phWhite, phBlack, emptyWhite, emptyBlack;
        PillButton btnAddWhite, btnDelWhite, btnAddBlack, btnDelBlack;
        PillButton btnStartup, btnTray, btnShortcut, btnCheck, btnExit;
        PillButton btnMin, btnClose, btnLang;
        NotifyIcon tray;
        ToolStripMenuItem miOpen, miCheck, miStartup, miExit;

        string whiteCount = "共 0 条";
        string blackCount = "共 0 条";

        Font titleFont, subFont, cardFont, hintFont, countFont, statusFont, monoFont;

        readonly List<string> logLines = new List<string>();
        readonly int logMax = 300;

        public MainForm()
        {
            cfg = new Config();
            L.Cur = (cfg.LangIndex >= 0 && cfg.LangIndex < 3) ? cfg.LangIndex : L.Detect();
            if (cfg.LangIndex < 0) { cfg.LangIndex = L.Cur; cfg.Save(); }

            Text = L.T("app.title");
            FormBorderStyle = FormBorderStyle.None;
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(WW, HH);
            BackColor = Theme.Bg;
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
            KeyPreview = true;
            MaximumSize = new Size(WW, HH);
            MinimumSize = new Size(WW, HH);

            engine = new Engine(cfg);

            titleFont = Theme.F(13F, FontStyle.Bold);
            subFont = Theme.F(9F, FontStyle.Regular);
            cardFont = Theme.F(11F, FontStyle.Bold);
            hintFont = Theme.F(8.5F, FontStyle.Regular);
            countFont = Theme.F(9F, FontStyle.Regular);
            statusFont = Theme.F(8.5F, FontStyle.Regular);
            monoFont = SafeMono(12F, FontStyle.Bold);

            BuildUi();

            engine.Logger = AppendLog;
            engine.OnStatsChanged = StatsChanged;
            engine.Start();

            SetTrayVisible(cfg.TrayEnabled, true);
            UpdateButtonText();

            MouseDown += FormMouseDown;
            Icon = AppIcon();
        }

        static Font SafeMono(float size, FontStyle style)
        {
            try
            {
                FontFamily ff = new FontFamily("Consolas");
                ff.Dispose();
                return new Font("Consolas", size, style, GraphicsUnit.Point);
            }
            catch { return Theme.F(size, style); }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= 0x00020000; // CS_DROPSHADOW
                return cp;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try
            {
                int pref = 2; // DWMWCP_ROUND
                DwmSetWindowAttribute(Handle, 33, ref pref, 4);
                int dark = 0;
                DwmSetWindowAttribute(Handle, 20, ref dark, 4);
            }
            catch { }
        }

        void FormMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Y < 70)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Hide(); }
            base.OnKeyDown(e);
        }

        // ---------- 界面 ----------
        Rectangle ListRect(Rectangle card) { return new Rectangle(card.X + 16, card.Y + 44, 840, 104); }
        Rectangle InputRect(Rectangle card) { return new Rectangle(card.X + 16, card.Y + 156, 596, 34); }
        Rectangle AddRect(Rectangle card) { return new Rectangle(card.X + 620, card.Y + 156, 84, 34); }
        Rectangle DelRect(Rectangle card) { return new Rectangle(card.X + 712, card.Y + 156, 128, 34); }

        void BuildUi()
        {
            specs.Clear();
            specs.Add(new Spec(rCardWhite, 14, Theme.Card, Theme.CardLine));
            specs.Add(new Spec(rCardBlack, 14, Theme.Card, Theme.CardLine));
            specs.Add(new Spec(rCardLog, 14, Theme.Card, Theme.CardLine));

            Rectangle wi = InputRect(rCardWhite);
            Rectangle bi = InputRect(rCardBlack);
            specs.Add(new Spec(wi, 10, Theme.InputFill, Theme.Line));
            specs.Add(new Spec(bi, 10, Theme.InputFill, Theme.Line));
            specs.Add(new Spec(new Rectangle(rCardLog.X + 16, rCardLog.Y + 36, 840, 50), 10, Theme.LogFill, Theme.Line));

            // 列表
            lbWhite = MakeList(ListRect(rCardWhite), DrawWhiteRow);
            lbBlack = MakeList(ListRect(rCardBlack), DrawBlackRow);

            emptyWhite = MakeEmptyHint(ListRect(rCardWhite), L.T("empty.list"));
            emptyBlack = MakeEmptyHint(ListRect(rCardBlack), L.T("empty.list"));

            Controls.Add(lbWhite);
            Controls.Add(lbBlack);
            Controls.Add(emptyWhite);
            Controls.Add(emptyBlack);
            emptyWhite.BringToFront();
            emptyBlack.BringToFront();

            // 输入框
            tbWhite = MakeInput(wi, out phWhite);
            tbBlack = MakeInput(bi, out phBlack);
            Controls.Add(tbWhite);
            Controls.Add(tbBlack);
            Controls.Add(phWhite);
            Controls.Add(phBlack);
            phWhite.BringToFront();
            phBlack.BringToFront();

            // 按钮
            btnAddWhite = MakeButton(AddRect(rCardWhite), L.T("btn.add"), Theme.Green, Theme.GreenHover, Theme.GreenDown, Color.White, Color.Transparent, Color.White);
            btnDelWhite = MakeButton(DelRect(rCardWhite), L.T("btn.del"), Color.White, Theme.SoftGray, Theme.SoftGrayDown, Theme.Text, Theme.Line, Color.White);
            btnAddBlack = MakeButton(AddRect(rCardBlack), L.T("btn.add"), Theme.Red, Theme.RedHover, Theme.RedDown, Color.White, Color.Transparent, Color.White);
            btnDelBlack = MakeButton(DelRect(rCardBlack), L.T("btn.del"), Color.White, Theme.SoftGray, Theme.SoftGrayDown, Theme.Text, Theme.Line, Color.White);
            Controls.Add(btnAddWhite); Controls.Add(btnDelWhite);
            Controls.Add(btnAddBlack); Controls.Add(btnDelBlack);

            btnAddWhite.Click += delegate { AddEntry(true); };
            btnDelWhite.Click += delegate { RemoveSelected(true); };
            btnAddBlack.Click += delegate { AddEntry(false); };
            btnDelBlack.Click += delegate { RemoveSelected(false); };

            tbWhite.KeyDown += delegate(object s, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; AddEntry(true); } };
            tbBlack.KeyDown += delegate(object s, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; AddEntry(false); } };

            lbWhite.SelectedIndexChanged += delegate { btnDelWhite.Enabled = lbWhite.SelectedIndices.Count > 0; };
            lbBlack.SelectedIndexChanged += delegate { btnDelBlack.Enabled = lbBlack.SelectedIndices.Count > 0; };

            // 日志
            tbLog = new TextBox();
            tbLog.Multiline = true;
            tbLog.ReadOnly = true;
            tbLog.BorderStyle = BorderStyle.None;
            tbLog.ScrollBars = ScrollBars.Vertical;
            tbLog.WordWrap = false;
            tbLog.BackColor = Theme.LogFill;
            tbLog.ForeColor = Theme.Sub;
            tbLog.Font = Theme.F(9F, FontStyle.Regular);
            tbLog.Bounds = new Rectangle(rCardLog.X + 28, rCardLog.Y + 40, 816, 42);
            tbLog.TabStop = false;
            Controls.Add(tbLog);

            // 底部快捷方式
            btnStartup = MakeButton(new Rectangle(24, 636, 146, 36), L.T("btn.startup.off"), Color.White, Theme.SoftGray, Theme.SoftGrayDown, Theme.Text, Theme.Line, Theme.Bg);
            btnTray = MakeButton(new Rectangle(176, 636, 146, 36), L.T("btn.tray.off"), Color.White, Theme.SoftGray, Theme.SoftGrayDown, Theme.Text, Theme.Line, Theme.Bg);
            btnShortcut = MakeButton(new Rectangle(328, 636, 178, 36), L.T("btn.shortcut"), Color.White, Theme.SoftGray, Theme.SoftGrayDown, Theme.Text, Theme.Line, Theme.Bg);
            btnCheck = MakeButton(new Rectangle(734, 636, 96, 36), L.T("btn.check"), Theme.Accent, Theme.AccentHover, Theme.AccentDown, Color.White, Color.Transparent, Theme.Bg);
            btnExit = MakeButton(new Rectangle(844, 636, 60, 36), L.T("btn.exit"), Color.White, Theme.SoftGray, Theme.SoftGrayDown, Theme.Sub, Theme.Line, Theme.Bg);
            Controls.Add(btnStartup); Controls.Add(btnTray); Controls.Add(btnShortcut);
            Controls.Add(btnCheck); Controls.Add(btnExit);

            btnStartup.Click += delegate { ToggleStartup(); };
            btnTray.Click += delegate { SetTrayVisible(!cfg.TrayEnabled, false); };
            btnShortcut.Click += delegate { CreateStartMenuShortcut(); };
            btnCheck.Click += delegate { RunCheckAsync(); };
            btnExit.Click += delegate { ExitApp(); };

            // 标题栏按钮
            btnLang = MakeButton(new Rectangle(WW - 176, 18, 70, 34), L.ShortNames[(L.Cur + 1) % 3], Color.White, Theme.SoftGray, Theme.SoftGrayDown, Theme.Sub, Theme.Line, Theme.Bg);
            btnLang.Radius = 8;
            btnLang.DrawFont = Theme.F(9F, FontStyle.Regular);
            btnLang.Click += delegate { CycleLanguage(); };
            Controls.Add(btnLang);

            btnMin = MakeButton(new Rectangle(WW - 96, 18, 34, 34), "—", Color.Transparent, Theme.SoftGray, Theme.SoftGrayDown, Theme.Sub, Color.Transparent, Theme.Bg);
            btnClose = MakeButton(new Rectangle(WW - 52, 18, 34, 34), "✕", Color.Transparent, Theme.SoftRed, Theme.SoftRed, Theme.Red, Color.Transparent, Theme.Bg);
            btnMin.Radius = 8; btnClose.Radius = 8;
            btnMin.DrawFont = Theme.F(10F, FontStyle.Regular);
            btnClose.DrawFont = Theme.F(10F, FontStyle.Regular);
            btnMin.Click += delegate { WindowState = FormWindowState.Minimized; };
            btnClose.Click += delegate { Hide(); };
            Controls.Add(btnMin); Controls.Add(btnClose);

            RefreshLists();
        }

        void CycleLanguage()
        {
            L.Cur = (L.Cur + 1) % 3;
            cfg.LangIndex = L.Cur;
            cfg.Save();
            ApplyLanguage();
        }

        void ApplyLanguage()
        {
            Text = L.T("app.title");
            if (btnAddWhite != null) btnAddWhite.Text = L.T("btn.add");
            if (btnAddBlack != null) btnAddBlack.Text = L.T("btn.add");
            if (btnDelWhite != null) btnDelWhite.Text = L.T("btn.del");
            if (btnDelBlack != null) btnDelBlack.Text = L.T("btn.del");
            if (btnShortcut != null) btnShortcut.Text = L.T("btn.shortcut");
            if (btnCheck != null) btnCheck.Text = L.T("btn.check");
            if (btnExit != null) btnExit.Text = L.T("btn.exit");
            if (btnLang != null) btnLang.Text = L.ShortNames[(L.Cur + 1) % 3];
            if (emptyWhite != null) emptyWhite.Text = L.T("empty.list");
            if (emptyBlack != null) emptyBlack.Text = L.T("empty.list");
            if (phWhite != null) phWhite.Text = L.T("placeholder");
            if (phBlack != null) phBlack.Text = L.T("placeholder");
            UpdateButtonText();
            if (tray != null)
            {
                tray.Text = L.T("tray.tip");
                if (miOpen != null) miOpen.Text = L.T("tray.open");
                if (miCheck != null) miCheck.Text = L.T("tray.check");
                if (miStartup != null) miStartup.Text = L.T("tray.startup");
                if (miExit != null) miExit.Text = L.T("tray.exit");
            }
            whiteCount = L.F("count", lbWhite == null ? 0 : lbWhite.Items.Count);
            blackCount = L.F("count", lbBlack == null ? 0 : lbBlack.Items.Count);
            Invalidate(true);
        }

        ListBox MakeList(Rectangle r, DrawItemEventHandler handler)
        {
            ListBox lb = new ListBox();
            lb.Bounds = r;
            lb.BorderStyle = BorderStyle.None;
            lb.BackColor = Color.White;
            lb.ForeColor = Theme.Text;
            lb.Font = Theme.F(9.5F, FontStyle.Regular);
            lb.DrawMode = DrawMode.OwnerDrawFixed;
            lb.ItemHeight = 30;
            lb.IntegralHeight = false;
            lb.HorizontalScrollbar = false;
            lb.SelectionMode = SelectionMode.MultiExtended;
            lb.DrawItem += handler;
            return lb;
        }

        Label MakeEmptyHint(Rectangle r, string text)
        {
            Label l = new Label();
            l.Bounds = r;
            l.BackColor = Color.White;
            l.ForeColor = Theme.Muted;
            l.Font = Theme.F(9F, FontStyle.Regular);
            l.TextAlign = ContentAlignment.MiddleCenter;
            l.Text = text;
            return l;
        }

        TextBox MakeInput(Rectangle r, out Label placeholder)
        {
            TextBox tb = new TextBox();
            tb.BorderStyle = BorderStyle.None;
            tb.BackColor = Theme.InputFill;
            tb.ForeColor = Theme.Text;
            tb.Font = Theme.F(10.5F, FontStyle.Regular);
            tb.AutoSize = false;
            tb.Bounds = new Rectangle(r.X + 14, r.Y + 7, r.Width - 28, 22);

            Label ph = new Label();
            ph.AutoSize = false;
            ph.Bounds = new Rectangle(r.X + 14, r.Y + 6, r.Width - 28, 22);
            ph.BackColor = Theme.InputFill;
            ph.ForeColor = Theme.Muted;
            ph.Font = Theme.F(9.5F, FontStyle.Regular);
            ph.TextAlign = ContentAlignment.MiddleLeft;
            ph.Text = L.T("placeholder");
            ph.Cursor = Cursors.IBeam;
            ph.Click += delegate { tb.Focus(); };

            tb.TextChanged += delegate { ph.Visible = tb.Text.Length == 0; };
            tb.Enter += delegate { ph.Visible = false; };
            tb.Leave += delegate { ph.Visible = tb.Text.Length == 0; };

            placeholder = ph;
            return tb;
        }

        PillButton MakeButton(Rectangle r, string text, Color fill, Color hover, Color down, Color txt, Color border, Color surround)
        {
            PillButton b = new PillButton();
            b.Bounds = r;
            b.Text = text;
            b.Fill = fill;
            b.Hover = hover;
            b.Down = down;
            b.Txt = txt;
            b.Border = border;
            b.Surround = surround;
            b.BackColor = surround;
            b.DrawFont = Theme.F(9.5F, FontStyle.Regular);
            return b;
        }

        // ---------- 绘制 ----------
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush b = new SolidBrush(Theme.Bg))
                g.FillRectangle(b, ClientRectangle);
            for (int i = 0; i < specs.Count; i++)
            {
                Spec s = specs[i];
                Gfx.FillRound(g, s.R, s.Radius, s.Fill);
                if (s.Border.A > 0) Gfx.DrawRound(g, s.R, s.Radius, s.Border, 1f);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // 顶部徽标
            Rectangle badge = new Rectangle(26, 18, 36, 36);
            using (LinearGradientBrush lg = new LinearGradientBrush(new Rectangle(badge.X - 1, badge.Y - 1, badge.Width + 2, badge.Height + 2),
                Theme.BadgeA, Theme.BadgeB, 55f))
            using (GraphicsPath p = Gfx.Round(badge, 11))
                g.FillPath(lg, p);
            TextRenderer.DrawText(g, ">_", monoFont, badge,
                Color.FromArgb(250, 250, 255),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            TextRenderer.DrawText(g, L.T("app.title"), titleFont, new Point(72, 17), Theme.Text, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(g, L.T("app.subtitle"),
                subFont, new Point(73, 41), Theme.Sub, TextFormatFlags.NoPadding);

            DrawCardTitle(g, rCardWhite, L.T("card.white"), L.T("hint.white"), whiteCount, Theme.Green);
            DrawCardTitle(g, rCardBlack, L.T("card.black"), L.T("hint.black"), blackCount, Theme.Red);
            DrawCardTitle(g, rCardLog, L.T("card.log"), L.T("hint.log"), "", Theme.Accent);

            // 底部状态
            string status = L.F("status",
                cfg.LastCheck.Length > 0 ? cfg.LastCheck : L.T("status.never"),
                cfg.TotalRemoved);
            Rectangle sr = new Rectangle(516, 646, 214, 20);
            TextRenderer.DrawText(g, status, statusFont, sr, Theme.Sub,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }

        void DrawCardTitle(Graphics g, Rectangle card, string title, string hint, string count, Color dot)
        {
            int x = card.X + 18;
            int y = card.Y + 13;
            using (SolidBrush b = new SolidBrush(dot))
                g.FillEllipse(b, new Rectangle(x, y + 8, 8, 8));

            Point tp = new Point(x + 16, y);
            TextRenderer.DrawText(g, title, cardFont, tp, Theme.Text, TextFormatFlags.NoPadding);
            int tw = TextRenderer.MeasureText(g, title, cardFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
            TextRenderer.DrawText(g, hint, hintFont, new Point(tp.X + tw + 12, y + 4), Theme.Muted, TextFormatFlags.NoPadding);

            if (count.Length > 0)
            {
                int cw = TextRenderer.MeasureText(g, count, countFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding).Width;
                TextRenderer.DrawText(g, count, countFont, new Point(card.Right - 18 - cw, y + 2), Theme.Sub, TextFormatFlags.NoPadding);
            }
        }

        void DrawWhiteRow(object sender, DrawItemEventArgs e) { DrawRow((ListBox)sender, e, Theme.Green, Theme.SoftGreen); }
        void DrawBlackRow(object sender, DrawItemEventArgs e) { DrawRow((ListBox)sender, e, Theme.Red, Theme.SoftRed); }

        void DrawRow(ListBox lb, DrawItemEventArgs e, Color dot, Color sel)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush bg = new SolidBrush(Color.White))
                g.FillRectangle(bg, e.Bounds);
            if (e.Index < 0 || e.Index >= lb.Items.Count) return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Rectangle row = new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, e.Bounds.Width - 8, e.Bounds.Height - 4);
            if (selected) Gfx.FillRound(g, row, 8, sel);

            using (SolidBrush db = new SolidBrush(dot))
                g.FillEllipse(db, new Rectangle(e.Bounds.X + 12, e.Bounds.Y + (e.Bounds.Height / 2) - 3, 6, 6));

            Rectangle tr = new Rectangle(e.Bounds.X + 26, e.Bounds.Y, e.Bounds.Width - 36, e.Bounds.Height);
            string text = lb.Items[e.Index].ToString();
            TextRenderer.DrawText(g, text, lb.Font, tr, Theme.Text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }

        // ---------- 逻辑 ----------
        void AddEntry(bool toWhite)
        {
            TextBox tb = toWhite ? tbWhite : tbBlack;
            string entry = tb.Text.Trim();
            if (entry.Length == 0) { tb.Focus(); return; }
            if (!cfg.Add(toWhite, entry))
            {
                AppendLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + L.F("log.dup", entry));
            }
            else
            {
                AppendLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " +
                    L.F("log.added", L.T(toWhite ? "role.white" : "role.black"), entry));
            }
            tb.Clear();
            RefreshLists();
            tb.Focus();
        }

        void RemoveSelected(bool fromWhite)
        {
            ListBox lb = fromWhite ? lbWhite : lbBlack;
            List<string> removed = new List<string>();
            List<int> idx = new List<int>();
            for (int i = 0; i < lb.SelectedIndices.Count; i++) idx.Add(lb.SelectedIndices[i]);
            idx.Sort();
            for (int i = idx.Count - 1; i >= 0; i--)
            {
                if (i < lb.Items.Count)
                {
                    removed.Add(lb.Items[idx[i]].ToString());
                    cfg.RemoveAt(fromWhite, idx[i]);
                }
            }
            for (int i = 0; i < removed.Count; i++)
                AppendLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " +
                    L.F("log.removed", L.T(fromWhite ? "role.white" : "role.black"), removed[i]));
            RefreshLists();
        }

        void RefreshLists()
        {
            List<string> w, b;
            cfg.Snapshot(out w, out b);

            lbWhite.BeginUpdate();
            lbWhite.Items.Clear();
            for (int i = 0; i < w.Count; i++) lbWhite.Items.Add(w[i]);
            lbWhite.EndUpdate();
            emptyWhite.Visible = w.Count == 0;

            lbBlack.BeginUpdate();
            lbBlack.Items.Clear();
            for (int i = 0; i < b.Count; i++) lbBlack.Items.Add(b[i]);
            lbBlack.EndUpdate();
            emptyBlack.Visible = b.Count == 0;

            whiteCount = L.F("count", w.Count);
            blackCount = L.F("count", b.Count);
            btnDelWhite.Enabled = lbWhite.SelectedIndices.Count > 0;
            btnDelBlack.Enabled = lbBlack.SelectedIndices.Count > 0;
            Invalidate();
        }

        void AppendLog(string block)
        {
            if (IsDisposed) return;
            try
            {
                if (InvokeRequired) { BeginInvoke(new Action<string>(AppendLog), block); return; }
            }
            catch { return; }

            string[] lines = block.Replace("\r", "").Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim().Length == 0) continue;
                logLines.Add(lines[i]);
            }
            while (logLines.Count > logMax) logLines.RemoveAt(0);

            if (tbLog != null)
            {
                tbLog.Text = string.Join("\r\n", logLines.ToArray());
                tbLog.SelectionStart = tbLog.Text.Length;
                tbLog.SelectionLength = 0;
                tbLog.ScrollToCaret();
            }
        }

        void StatsChanged()
        {
            if (IsDisposed) return;
            try
            {
                if (InvokeRequired) { BeginInvoke(new Action(StatsChanged)); return; }
            }
            catch { return; }
            Invalidate();
        }

        void RunCheckAsync()
        {
            btnCheck.Enabled = false;
            Thread t = new Thread(delegate()
            {
                try { engine.RunOnce(); }
                catch { }
                try
                {
                    if (!IsDisposed && IsHandleCreated)
                        BeginInvoke(new Action(delegate { btnCheck.Enabled = true; }));
                }
                catch { }
            });
            t.IsBackground = true;
            t.Start();
        }

        // ---------- 任务栏 / 自启动 / 快捷方式 ----------
        static Icon cachedIcon;
        static Icon AppIcon()
        {
            if (cachedIcon != null) return cachedIcon;
            try
            {
                Icon ic = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                if (ic != null) { cachedIcon = ic; return ic; }
            }
            catch { }
            try
            {
                Bitmap bmp = new Bitmap(64, 64);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.Clear(Color.Transparent);
                    Gfx.FillRound(g, new Rectangle(1, 1, 62, 62), 16, Theme.Accent);
                    using (Font f = SafeMono(24F, FontStyle.Bold))
                        TextRenderer.DrawText(g, ">_", f, new Rectangle(1, 1, 62, 62), Color.White,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                cachedIcon = Icon.FromHandle(bmp.GetHicon());
            }
            catch { cachedIcon = SystemIcons.Application; }
            return cachedIcon;
        }

        void SetTrayVisible(bool on, bool initial)
        {
            cfg.TrayEnabled = on;
            if (on)
            {
                if (tray == null)
                {
                    ContextMenuStrip menu = new ContextMenuStrip();
                    menu.Font = Theme.F(9.5F, FontStyle.Regular);
                    miOpen = new ToolStripMenuItem(L.T("tray.open"));
                    miOpen.Font = Theme.F(9.5F, FontStyle.Bold);
                    miOpen.Click += delegate { ShowConfig(); };
                    miCheck = new ToolStripMenuItem(L.T("tray.check"));
                    miCheck.Click += delegate { RunCheckAsync(); };
                    miStartup = new ToolStripMenuItem(L.T("tray.startup"));
                    miStartup.CheckOnClick = false;
                    miStartup.Click += delegate { ToggleStartup(); };
                    miExit = new ToolStripMenuItem(L.T("tray.exit"));
                    miExit.Click += delegate { ExitApp(); };
                    menu.Items.Add(miOpen);
                    menu.Items.Add(miCheck);
                    menu.Items.Add(miStartup);
                    menu.Items.Add(new ToolStripSeparator());
                    menu.Items.Add(miExit);

                    tray = new NotifyIcon();
                    tray.Icon = AppIcon();
                    tray.Text = L.T("tray.tip");
                    tray.ContextMenuStrip = menu;
                    tray.DoubleClick += delegate { ShowConfig(); };
                }
                tray.Visible = true;
            }
            else if (tray != null)
            {
                tray.Visible = false;
            }
            UpdateButtonText();
            if (!initial) cfg.Save();
        }

        void UpdateButtonText()
        {
            if (btnTray != null)
                btnTray.Text = cfg.TrayEnabled ? L.T("btn.tray.on") : L.T("btn.tray.off");
            if (btnStartup != null)
                btnStartup.Text = IsStartupEnabled() ? L.T("btn.startup.on") : L.T("btn.startup.off");
        }

        const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        const string RunValueName = "PSHistoryCleaner";

        static string StartupCommand()
        {
            return "\"" + Application.ExecutablePath + "\" /hidden";
        }

        static bool IsStartupEnabled()
        {
            try
            {
                using (RegistryKey k = Registry.CurrentUser.OpenSubKey(RunKeyPath, false))
                {
                    if (k == null) return false;
                    object v = k.GetValue(RunValueName);
                    return v != null && v.ToString().Trim().Length > 0;
                }
            }
            catch { return false; }
        }

        void ToggleStartup()
        {
            try
            {
                if (IsStartupEnabled())
                {
                    using (RegistryKey k = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                    {
                        if (k != null) k.DeleteValue(RunValueName, false);
                    }
                    AppendLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + L.T("log.startup.off"));
                }
                else
                {
                    using (RegistryKey k = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
                    {
                        if (k == null) k.SetValue(RunValueName, StartupCommand(), RegistryValueKind.String);
                        else k.SetValue(RunValueName, StartupCommand(), RegistryValueKind.String);
                    }
                    AppendLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + L.T("log.startup.on"));
                }
            }
            catch (Exception ex)
            {
                AppendLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + L.F("log.startup.fail", ex.Message));
            }
            UpdateButtonText();
        }

        void CreateStartMenuShortcut()
        {
            try
            {
                string folder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Microsoft\Windows\Start Menu\Programs");
                Directory.CreateDirectory(folder);
                string lnk = System.IO.Path.Combine(folder, "PowerShell 历史清理.lnk");

                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                object shell = Activator.CreateInstance(shellType);
                object sc = shellType.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { lnk });

                Type scType = sc.GetType();
                scType.InvokeMember("TargetPath", BindingFlags.SetProperty, null, sc, new object[] { Application.ExecutablePath });
                scType.InvokeMember("Arguments", BindingFlags.SetProperty, null, sc, new object[] { "" });
                scType.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, sc, new object[] { System.IO.Path.GetDirectoryName(Application.ExecutablePath) });
                scType.InvokeMember("Description", BindingFlags.SetProperty, null, sc, new object[] { "PowerShell 历史清理 配置界面" });
                scType.InvokeMember("IconLocation", BindingFlags.SetProperty, null, sc, new object[] { Application.ExecutablePath + ",0" });
                scType.InvokeMember("Save", BindingFlags.InvokeMethod, null, sc, new object[] { });

                AppendLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + L.F("log.shortcut.ok", lnk));
            }
            catch (Exception ex)
            {
                AppendLog("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + L.F("log.shortcut.fail", ex.Message));
            }
        }

        // ---------- 窗体行为 ----------
        public void ShowConfig()
        {
            if (IsDisposed) return;
            if (!Visible) Show();
            if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
            Activate();
            TopMost = true;
            TopMost = false;
            BringToFront();
            RefreshLists();
        }

        public void ExitApp()
        {
            exiting = true;
            engine.Stop();
            try
            {
                if (tray != null) { tray.Visible = false; tray.Dispose(); tray = null; }
            }
            catch { }
            try { Close(); } catch { }
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!exiting && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }
            base.OnFormClosing(e);
        }
    }

    class Host : ApplicationContext
    {
        readonly MainForm form;

        public Host(bool startHidden, EventWaitHandle showEvent)
        {
            form = new MainForm();
            IntPtr dummy = form.Handle; // 强制创建句柄，隐藏模式下也能接收消息
            if (!startHidden) form.ShowConfig();

            Thread watcher = new Thread(delegate()
            {
                while (true)
                {
                    try
                    {
                        showEvent.WaitOne();
                        if (form.IsDisposed) return;
                        form.BeginInvoke(new Action(delegate { form.ShowConfig(); }));
                    }
                    catch { return; }
                }
            });
            watcher.IsBackground = true;
            watcher.Name = "ShowUiWatcher";
            watcher.Start();
        }
    }

    static class Program
    {
        const string MutexName = "PSHistoryCleaner_SingleInstance_v1";
        const string EventName = "PSHistoryCleaner_ShowUI_v1";

        static bool HasArg(string[] args, string name)
        {
            if (args == null) return false;
            for (int i = 0; i < args.Length; i++)
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        [STAThread]
        static void Main(string[] args)
        {
            // 单次检查模式（用于自检，不影响界面）
            if (HasArg(args, "/checkonce"))
            {
                Config c = new Config();
                Engine en = new Engine(c);
                en.RunOnce();
                Environment.Exit(0);
                return;
            }

            bool createdNew = false;
            Mutex mutex = new Mutex(true, MutexName, out createdNew);
            if (!createdNew)
            {
                // 已有实例在运行：唤醒它显示配置界面
                try
                {
                    EventWaitHandle ev = null;
                    for (int i = 0; i < 10; i++)
                    {
                        try { ev = EventWaitHandle.OpenExisting(EventName); break; }
                        catch { Thread.Sleep(150); }
                    }
                    if (ev != null) { ev.Set(); ev.Close(); }
                }
                catch { }
                return;
            }

            bool hidden = HasArg(args, "/hidden") || HasArg(args, "-hidden") || HasArg(args, "hidden");

            EventWaitHandle showEvent = new EventWaitHandle(false, EventResetMode.AutoReset, EventName);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += delegate(object s, ThreadExceptionEventArgs e)
            {
                try { new Engine(new Config()).FileLog("界面异常：" + e.Exception.Message); } catch { }
            };

            Host host = new Host(hidden, showEvent);
            Application.Run(host);

            GC.KeepAlive(mutex);
            GC.KeepAlive(showEvent);
        }
    }
}
