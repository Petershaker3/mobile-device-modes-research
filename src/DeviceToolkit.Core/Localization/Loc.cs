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

    /// <summary>
    /// الجدول الإنجليزي الكامل (643 مفتاحاً). مملوك بالكامل لـ LocTable.g.cs
    /// المولَّد آلياً من upstream — لا تُضف مفاتيح هنا.
    /// <para>
    /// كانت النسخة الأولى من هذا الملف تحتوي 19 مفتاحاً اخترتها يدوياً، فأسقط
    /// اختبار في CI مفتاح رسالة «طول الحمولة» فسقط الترجمة إلى الصينية. الدرس:
    /// لا تخمّن أي مفاتيح يحتاجها كود منقول — انقلها كلها.
    /// </para>
    /// </summary>
    static readonly Dictionary<string, string> En = LocTable.All();

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
