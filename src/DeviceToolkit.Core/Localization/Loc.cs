// NEW FILE — not copied. See ATTRIBUTION.md §1.5.
//
// Upstream ships this behaviour inside logcat-uicore/Services/Loc.cs, but that
// file is 559 lines of WinForms plumbing (ToolTip, Control tree walking, font
// measurement, persistence) of which the copied code needs only four members:
// UiLang, T, F, SetLangGuard.
//
// Rather than drag WinForms into the core library, this file reproduces those
// four members with upstream's exact semantics. The code paths are quoted
// below so a reviewer can compare them line by line without opening upstream.

using System.Globalization;

namespace logcat.Services;

/// <summary>واجهة اللغة. مطابق لـ upstream: ZhCn = 0، En = 1.</summary>
public enum UiLang
{
    /// <summary>简体中文。</summary>
    ZhCn = 0,

    /// <summary>English。</summary>
    En = 1,
}

/// <summary>
/// الترجمة الأساسية. الإصدار الكامل في upstream (Loc + LocTable.*) يخدم الواجهة؛
/// هنا نُبقي الجزء الذي يستهلكه الكود والتests المنقولة فقط.
/// </summary>
public static class Loc
{
    // Upstream: `static UiLang _lang;` + `static Loc() => _lang = DetectLang();`
    // DetectLang reads the saved language.json and falls back to the system
    // culture, both of which belong to the UI layer. The copied code and tests
    // always set the language explicitly, so zh-CN is the safe deterministic
    // default and matches upstream's behaviour on a zh-CN machine.
    static UiLang _lang = UiLang.ZhCn;

    /// <summary>الجدول الإنجليزي. المفاتيح هي النص الصيني الأصلي — نقل حرفي من LocTable.*.</summary>
    static readonly Dictionary<string, string> En = new()
    {
        // LocTable.CrashUi.cs
        ["Java 崩溃"] = "Java Crash",
        ["Native 崩溃"] = "Native Crash",
        ["ANR 应用无响应"] = "ANR (App Not Responding)",
        ["Native 崩溃（tombstone）"] = "Native Crash (tombstone)",
        ["未知"] = "Unknown",
        // LocTable.FilterUi.cs
        ["（无过滤）"] = "(no filter)",
        ["级别 {0}"] = "Level {0}",
        ["仅标记行"] = "Marked only",
        ["分钟({0})"] = "Minute({0})",
        ["；"] = "; ",
        // LocTable.RuntimeMsg.cs / LocTable.MainUi.cs — progress messages asserted by tests
        ["扫描行…"] = "Scanning lines…",
        ["合并索引…"] = "Merging index…",
        ["过滤…"] = "Filtering…",
        ["加载中…"] = "Loading…",
        ["追加新内容…"] = "Appending new content…",
        ["完成"] = "Done",
        ["候选 {0:N0} 行"] = "{0:N0} candidate rows",
        ["导出 {0:N0}/{1:N0}"] = "Exporting {0:N0}/{1:N0}",
        ["匹配 message {0}/{1}"] = "Matching message {0}/{1}",
    };

    /// <summary>اللغة الحالية. نقل حرفي: `public static UiLang Current => _lang;`</summary>
    public static UiLang Current => _lang;

    /// <summary>
    /// تبديل اللغة. يطابق دلالة upstream: نفس اللغة = لا شيء، والكتابة إلى
    /// language.json تخص الواجهة فلا تحدث هنا (save افتراضي false).
    /// </summary>
    public static void SetLang(UiLang lang, bool save = false)
    {
        if (_lang == lang) return;
        _lang = lang;
        // Upstream persists to language.json here when save: true and then walks
        // the WinForms control tree to re-apply text. Neither belongs in a
        // portable library; the UI layer owns both.
        _ = save;
    }

    /// <summary>
    /// حماية تبديل اللغة وإرجاعها. نقل حرفي:
    /// <code>
    /// public static Action SetLangGuard(UiLang lang)
    /// {
    ///     var origin = _lang;
    ///     SetLang(lang, save: false);
    ///     return () => SetLang(origin, save: false);
    /// }
    /// </code>
    /// </summary>
    public static Action SetLangGuard(UiLang lang)
    {
        var origin = _lang;
        SetLang(lang, save: false);
        return () => SetLang(origin, save: false);
    }

    /// <summary>
    /// ترجمة نص. نقل حرفي:
    /// <code>
    /// public static string T(string zh)
    /// {
    ///     if (zh.Length == 0 || _lang == UiLang.ZhCn) return zh;
    ///     return En.TryGetValue(zh, out var v) && v.Length > 0 ? v : zh;
    /// }
    /// </code>
    /// المفتاح المفقود يرجع الصيني الأصلي — لا استثناء ولا نص فارغ.
    /// </summary>
    public static string T(string zh)
    {
        if (zh.Length == 0 || _lang == UiLang.ZhCn) return zh;
        return En.TryGetValue(zh, out var v) && v.Length > 0 ? v : zh;
    }

    /// <summary>
    /// ترجمة مع تنسيق. نقل حرفي، بما فيه سلوك `FormatException` → النص بلا تنسيق.
    /// </summary>
    public static string F(string zh, params object?[] args)
    {
        var t = T(zh);
        if (args.Length == 0) return t;
        try { return string.Format(CultureInfo.CurrentCulture, t, args); }
        catch (FormatException) { return t; }
    }
}
