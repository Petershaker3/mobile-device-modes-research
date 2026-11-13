# النقل الأول — طبقة Logcat والتشخيص

> **القاعدة المتبعة هنا:** لا كود جديد إلا حيث لا يوجد كود. كل ما هو موجود على GitHub يُنقل حرفياً، ويُسجَّل في [ATTRIBUTION.md](../ATTRIBUTION.md).
>
> **ماذا يعني هذا الملف:** توثيق أول عملية نقل فعلية — 8,530 سطراً من `daitouniao/logcat-viewer`، منها 3,151 سطر كود و5,383 سطر اختبار. **صفر تعديل** على أي ملف كود.

**التاريخ:** 13 نوفمبر 2026 — **المصدر:** `a985616224a342d6064ed962eec28ace3f38365a`

---

## 1. لماذا هذا المحور أولاً

بعد قرار [ADR-0001](17-adr-0001-stack-decision.md) (‏.NET 10) ومراجعة المصادر في [20](20-source-revalidation-round2.md)، صار واضحاً أن `logcat-viewer` يحتوي على **أكبر كتلة كود C# جاهزة ومختبرة** في كل مرشحينا:

| المكوّن | الأسطر | ما يقدمه | يحتاج جهازاً؟ |
|---|---:|---|---|
| `LogParser` | 441 | تحليل ست صيغ logcat (`threadtime`/`time`/`brief`/`long`/`tag`/`ymd`) مع مسار بايت سريع | لا |
| `LogDocument` | 1,100 | فتح ملفات ضخمة بـmemory-mapped، فهرس أعمدة، تحميل تدريجي قابل للإلغاء | لا |
| `FilterEngine` | 1,133 | فلترة بـSIMD/Vector، مسح كتل متوازٍ | لا |
| `CrashExtractor` + `CrashAggregator` + `CrashTime` | 316 | استخراج Java crash / ANR / tombstone وتجميعها | لا |
| `TextProbe` | 55 | كشف الترميز وحجم الملف قبل الفتح | لا |

هذه هي **الطبقة الأصعب والأغلى** في محور التشخيص. وقرار نقلها يعني أن محور التشخيص في V1 لم يعد يبدأ من الصفر.

## 2. ما نُقل وما لم يُنقل ولماذا

### نُقل حرفياً — 9 ملفات كود

`LogParser.cs` · `LogDocument.cs` · `FilterEngine.cs` · `CrashExtractor.cs` · `CrashAggregator.cs` · `CrashTime.cs` · `TextProbe.cs` · `CrashEvent.cs` · `FilterSpec.cs`

**كيف عرفنا أنها قابلة للنقل؟** ليس تخميناً — بحث نصي داخل الملفات التسعة:
```bash
grep -n "System.Windows.Forms\|System.Drawing\|DllImport\|#if WINDOWS" <الملفات>
# لا نتائج
```
هذا هو الفرق بين "يبدو قابلاً للنقل" و"مثبت أنه لا يعتمد على Windows".

### نُقل حرفياً — 10 ملفات اختبار

`LogParserTests` · `LogDocumentTests` · `LogDocumentZeroCopyTests` · `LogDocumentProxiedTests` · `FilterEngineTests` · `FilterEngineBlockScanTests` · `ChunkedEquivalenceTests` · `CrashTests` · `TextProbeTests` · `TestCultureBootstrap`

تحتوي **246 `[Fact]` + 24 `[Theory]` بـ98 `[InlineData]`**. أي: المنقول لا يعمل فقط، بل **مُختبر مسبقاً**.

### لم يُنقل — والسبب لكل حالة

| غير منقول | السبب الفعلي |
|---|---|
| `AdbManager.cs` | يستخدم `AdvancedSharpAdbClient` + `System.Drawing` للـscreenshot. **والأهم:** فيه الفجوة الموثقة — `Shell(serial, cmd, timeoutSec = 30)` لا يستخدم `timeoutSec` في جسمه. نقله كما هو = نقل عيب. يُنقل بعد إصلاح حقيقي |
| `LogcatStream.cs` | يحتاج `AdvancedSharpAdbClient` واتصال جهاز حقيقي؛ اختباره بلا جهاز وهمي |
| `FileProxyService/Client` | بنية IPC/spool خاصة بتطبيقهم، لا يحتاجها أي محور عندنا |
| `SidecarCache` | تحسين لتحميل ملفات 1GB؛ يُؤجل حتى قياس حقيقي |
| `Loc.cs` (الملف الأصلي) | 559 سطراً، غالبه WinForms. نُقل منه 4 أعضاء فقط في ملف جديد ([§4](#4-الملف-الجديد-الوحيد)) |
| كل `Forms/` و`Controls/` | واجهة WinForms — مستبعدة بقرار WPF |
| 42 ملف اختبار آخر | تخص مكوّنات لم تُنقل |

## 3. كيف بُني بلا تعديل أي سطر منقول

المشكلة الظاهرية: الملفات المنقولة تعلن `namespace logcat.Services` وتحتاج `InternalsVisibleTo("logcat.Tests")` وتعتمد على `unsafe`.

الحل: **كل ذلك في ملفات `.csproj`، لا في الكود.**

| الحاجة | الحل | الملف |
|---|---|---|
| `namespace logcat.Services` يبقى كما هو | `<RootNamespace>logcat</RootNamespace>` | `DeviceToolkit.Core.csproj` |
| الكود يعتمد على `InternalsVisibleTo("logcat.Tests")` | نعلنها كـ`AssemblyAttribute`، ونسمّي مشروع الاختبار `logcat.Tests` | الـcsproj ـان |
| مؤشرات خام في `LogDocument`/`FilterEngine` | `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>` | `DeviceToolkit.Core.csproj` |
| الـtests تحتاج `xunit` بإصداراتهم | نفس الإصدارات المثبتة عندهم | `DeviceToolkit.Tests.csproj` |
| الكود كان داخل تطبيق WinForms | `net10.0` عادي بلا `-windows` | `DeviceToolkit.Core.csproj` |

**النتيجة:** `DeviceToolkit.Core` لا يعرف WinForms إطلاقاً، ورغم ذلك يبني كوداً لم يُكتب ليكون مكتبة.

## 4. الملف الجديد الوحيد

`src/DeviceToolkit.Core/Localization/Loc.cs` — **52 سطراً** (مع التعليقات والجدول).

**لماذا لم يُنقل الأصل؟** `Loc.cs` الأصلي 559 سطراً، منها: `ToolTip` مشترك، `ConditionalWeakTable` لربط النصوص بالـcontrols، قياس عرض النص بالبكسل، `ApplyToAllForms`، حفظ `language.json`. كل هذا واجهة. الملفات المنقولة تحتاج **أربعة أعضاء**:

```
Loc.T(string)                      — ترجمة
Loc.F(string, params object[])     — ترجمة + تنسيق
Loc.SetLangGuard(UiLang)           — تبديل مؤقت وإرجاع
UiLang { ZhCn, En }
```

كل واحد منها **منقول حرفياً من جسمه في upstream**، والكود الأصلي مقتبس في تعليق فوقه ليتمكن المراجع من المقارنة بلا فتح المستودع الآخر:

```csharp
// نقل حرفي:
// public static string T(string zh)
// {
//     if (zh.Length == 0 || _lang == UiLang.ZhCn) return zh;
//     return En.TryGetValue(zh, out var v) && v.Length > 0 ? v : zh;
// }
```

**جدول الترجمة** (19 مفتاحاً) لم يُكتب من عندنا؛ استُخرج آلياً من `LocTable.CrashUi.cs` و`LocTable.FilterUi.cs` و`LocTable.MainUi.cs` و`LocTable.RuntimeMsg.cs` بسكربت يقرأ `["مفتاح"] = "value"`، لأن اختبارات منقولة تُثبّت النص الإنجليزي حرفياً:
```csharp
LangScope.InEn(() => Assert.Equal("Unknown", CrashKind.Name(kind)));
```
لو ترجمناها بأنفسنا لتغير النص ولفشل الاختبار.

**الفرق السلوكي الوحيد المسجّل:** الأصل يستنتج اللغة الأولية من `language.json` أو ثقافة النظام؛ هنا الافتراضي `ZhCn` ثابت. الكود والاختبارات المنقولة تضبط اللغة صراحةً دائماً، فالفارق لا يظهر — لكنه مسجّل لأنه فارق.

## 5. الملف المنقول الوحيد الذي مُسّ

`src/DeviceToolkit.Tests/TestHelpers.cs` — **7 أسطر محذوفة**:

```diff
- [CollectionDefinition(WinFormsCollection.Name, DisableParallelization = true)]
- public sealed class WinFormsCollection
- {
-     /// <summary>集合名。引用它即可把类并入该集合。</summary>
-     public const string Name = "WinForms-static-state";
- }
+ // ── SHIM (new; see ATTRIBUTION.md §1.6) ─────────────────────────────
+ // Upstream defines WinFormsCollection here, to serialise tests that build
+ // WinForms controls against a shared ToolTip. No copied test builds a
+ // control, and this assembly has no WinForms reference, so the type is
+ // dropped. LangCollection above stays verbatim: the copied CrashTests and
+ // LogDocumentTests do mutate the process-wide Loc._lang state.
```

**لماذا هذا التعديل ضروري:** النوع يخدم اختبارات لم تُنقل، ويشير إلى `ToolTip` غير موجود في مكتبتنا. إبقاؤه = compile error؛ حذفه بلا تعويض = صمت عن تغيير.

**ولماذا `LangCollection` بقي؟** لأنه ضروري فعلاً: `TestHelpers` يوثّق حادثة حقيقية عندهم — اختبارات تغيير اللغة تفشل بشكل متقطع عند التوازي. الحلقة نفسها تنطبق عندنا لأن `Loc._lang` ما زال حالة ثابتة على مستوى العملية.

## 6. كيف تتحقق أنت — أمر واحد

```bash
scripts/verify-transplant.sh
```

يفحص ثلاثة أشياء مستقلة:

| الفحص | ماذا يفعل |
|---|---|
| **1. سلامة النقل** | يستنسخ upstream عند نفس الـcommit ويشغّل `diff` على 19 ملفاً. يفشل لو تغيّر بايت واحد |
| **2. قابلية النقل** | يرفض وجود `System.Windows.Forms`/`System.Drawing`/`DllImport` في Core |
| **3. البناء والاختبار** | `dotnet build` ثم `dotnet test` |

```
1/3  integrity: copied files vs upstream @ a9856162
  OK   src/DeviceToolkit.Core/Logcat/LogParser.cs
  ...  (19 ملفاً)
  OK   TestHelpers.cs (documented shim: WinFormsCollection removed, 7 lines)
2/3  portability: no Windows-only dependency in the core
  OK   no WinForms / System.Drawing / DllImport in src/DeviceToolkit.Core
```

الفحصان 1 و2 يعملان **الآن** وقد أثبتا نجاحهما. الفحص 3 يحتاج جهازك لأن بيئتي بلا .NET SDK وبلا NuGet.

## 7. ما لم يُثبت بعد — بصراحة

| الادعاء | الحالة |
|---|---|
| الكود يبني على `net10.0` | **غير مثبت**: لم أشغّل `dotnet build`. الاحتمال قوي (لا WinForms/P-Invoke) لكنه ليس إثباتاً |
| الاختبارات تمر | **غير مثبت**: لم أشغّل `dotnet test`. الأرقام (852 اختباراً عندهم) **معلنة في مستودعهم**، لا مُتحقَّق منها هنا |
| `Microsoft.Extensions.Logging.Abstractions 10.0.11` متاح على NuGet | **غير مثبت**: NuGet محجوب في بيئتي. رقم الإصدار منسوخ من csproj الخاص بهم |
| `net10.0` SDK موجود عندك | **غير معروف**: إن لم يكن، `global.json` يطلب `10.0.100` بـ`rollForward: latestFeature` |

**أول ما تشغّله:** `scripts/verify-transplant.sh`. لو ظهر خطأ build، أرسل لي النص وأصلحه فوراً — هذا أسرع من التخمين.

## 8. التالي في قائمة النقل

| # | المكوّن | المصدر | الشرط |
|---:|---|---|---|
| 1 | إصلاح مهلة `AdbManager.Shell` **ثم** نقله | logcat-viewer | إصلاح حقيقي + اختبار يثبت أن أمراً معلقاً يُلغى |
| 2 | `LogcatStream` + `FakeAdbSocket` (اختبار ADB بلا جهاز) | logcat-viewer | يحتاج `AdvancedSharpAdbClient` من NuGet |
| 3 | `ApkInfo` + فلاتر ABI/SDK | APK-Installer | قرار: نقل `aapt` wrapper أم parser مباشر |
| 4 | `DeviceInfo`/`FileEntry`/`AdbServerSelfHealResult` | logcat-viewer | تحتاج مراجعة تبعيات |

**قاعدة النقل المستمرة:** كل ملف يُسجَّل في [ATTRIBUTION.md](../ATTRIBUTION.md) قبل الدمج، ويُضاف سطر `check_identical` في سكربت التحقق.

**آخر تحديث:** 13 نوفمبر 2026.
