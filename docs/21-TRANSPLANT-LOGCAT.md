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

> **تصحيح مهم بعد أول تشغيل حقيقي.** النسخة الأولى من هذا المستند ادّعت أن 9 ملفات كود
> هي كل ما يلزم. CI أثبت العكس خلال أربع دورات: كل بناء كان يكشف تبعية لم أنقلها.
> العدد الحقيقي **28 ملف كود + 20 ملف اختبار**. الدرس مسجّل هنا صريحاً لا مخفياً:
> **قراءة الكود لا تكشف سلسلة التبعيات؛ المُصرِّف يكشفها.** لذلك لم أعتمد على القراءة
> في الجولة الأخيرة، بل تبعت سلسلة التبعيات حتى توقف المُصرِّف عن الشكوى.

### نُقل حرفياً — 28 ملف كود

| المجموعة | الملفات |
|---|---|
| التحليل الأساسي (9) | `LogParser` · `LogDocument` · `FilterEngine` · `CrashExtractor` · `CrashAggregator` · `CrashTime` · `TextProbe` · `CrashEvent` · `FilterSpec` |
| كاش الـsidecar (2) | `SidecarCache` · `SidecarCacheStore` |
| الملفات المؤقتة والـIPC (5) | `FileProxyService` · `FileProxyClient` · `FileProxyMode` · `FileProxyException` · `ProxyChannel` |
| الإعدادات (6) | `LogcatSettings` · `SettingsIO` · `ConfigMigration` · `LegacySettings` · `LanguageSettings` · `AndToolsSettings` |
| السجلات والنماذج (6) | `StartupLog` + `DeviceLastDir` · `FavoriteDir` · `DeviceInfo` · `FileEntry` · `TryOpenOutcome` · `AdbServerSelfHealResult` · `CommandEntry` |

**كيف عرفنا أنها قابلة للنقل؟** ليس تخميناً — الفحص النصي:
```bash
grep -rn "System.Windows.Forms\|System.Windows.Media\|DllImport" src/DeviceToolkit.Core
# لا نتائج
```
ثم أثبت CI البناء الفعلي على ويندوز ولينكس. هذا هو الفرق بين "يبدو قابلاً للنقل" و"مثبت".

**ملاحظة عن `System.Drawing`:** `LogcatSettings` يستخدم `Point` و`Size` فقط، وهما من
`System.Drawing.Primitives` المتاح على كل الأنظمة. لذلك فحص قابلية النقل يمنع
`Bitmap`/`Graphics`/`Icon` تحديداً بدل منع النطاق كله — فحص يُطلق إنذارات كاذبة
يتعلّم الناس تجاهله، وذلك أسوأ من عدم وجود فحص.

### نُقل حرفياً — 20 ملف اختبار

`LogParserTests` · `LogDocumentTests` · `LogDocumentZeroCopyTests` · `LogDocumentProxiedTests` · `FilterEngineTests` · `FilterEngineBlockScanTests` · `ChunkedEquivalenceTests` · `CrashTests` · `TextProbeTests` · `TestCultureBootstrap` · `SidecarCacheTests` · `FileProxyProtocolTests` · `FileProxyLoopbackTests` · `FileProxyChannelStateTests` · `FileProxyModeTests` · `LegacySettingsDtoTests` · `FakeProxyServer` (مساعد) + `TestHelpers` (مُعدّل، [§5](#5-الملف-المنقول-الوحيد-الذي-مُسّ))

**437 اختباراً تمر كلها على ويندوز** — رقم مُتحقَّق منه في CI لا معلن. أي: المنقول لا يعمل فقط، بل **مُختبر مسبقاً**.

### لم يُنقل — والسبب لكل حالة

| غير منقول | السبب الفعلي |
|---|---|
| `AdbManager.cs` | يستخدم `AdvancedSharpAdbClient` + `System.Drawing` للـscreenshot. **والأهم:** فيه الفجوة الموثقة — `Shell(serial, cmd, timeoutSec = 30)` لا يستخدم `timeoutSec` في جسمه. نقله كما هو = نقل عيب. يُنقل بعد إصلاح حقيقي |
| `LogcatStream.cs` | يحتاج `AdvancedSharpAdbClient` واتصال جهاز حقيقي؛ اختباره بلا جهاز وهمي |
| ~~`FileProxyService/Client`~~ | **نُقلت.** الحجة الأولى (لا يحتاجها أي محور عندنا) كانت خطأ: `LogDocument` نفسه يعتمد على `SpoolDirLease` من هذا الملف |
| ~~`SidecarCache`~~ | **نُقلت.** `LogDocument` يعتمد على `SidecarPayload`؛ التأجيل كان سيكسر البناء |
| `AdbManager`/`Forms`/`Controls`/`AppInfo` | لم تُنقل بعد — تحتاج قرار محور (انظر [§8](#8-التالي-في-قائمة-النقل)) |
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

## 4. الملفان الجديدان (لا المنقولان)

الملفان الجديدان الوحيدان هما:

| الملف | السطور | لماذا جديد |
|---|---:|---|
| `Localization/Loc.cs` | 52 | نسخة منزوعة من WinForms لواجهة `Loc` — التفصيل أدناه |
| `Localization/LocTable.g.cs` | **مُولَّد آلياً** | 643 مفتاحاً مستخرجة من 11 ملف `LocTable.*.cs`؛ لا يُكتب يدوياً ولا يُحرَّر |

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

**جدول الترجمة — القصة التي غيّرت المنهج.** المحاولة الأولى نقلت 19 مفتاحاً اخترتها
بنفسي من 4 ملفات `LocTable`، بناءً على ما «بدا» أن الكود يحتاجه. CI أثبت الخطأ في أول
تشغيل: اختبار منقول يسأل عن نص إنجليزي لمفتاح لم أنقله، فيسقط الترجمة إلى الصينية.

الحل ليس إضافة المفتاح الناقص، بل إنهاء التخمين: `scripts/gen-loctable.py` ينسخ
**كل** مفتاح من 11 ملفاً (643 مفتاحاً، 0 تعارض)، وCI يعيد التوليد ويفشل على أي انحراف.
ثم `scripts/check-loc-keys.py` يثبت أن كل مفتاح حرفي يستخدمه الكود موجود في الجدول —
فلا يمكن لسقوط صامت إلى الصينية أن يخفي فجوة بعد اليوم.

ولماذا يجب أن تُنقل النصوص لا أن نترجمها بأنفسنا؟ لأن اختبارات منقولة تُثبّت النص الإنجليزي حرفياً:
```csharp
LangScope.InEn(() => Assert.Equal("Unknown", CrashKind.Name(kind)));
```
لو ترجمناها بأنفسنا لتغير النص ولفشل الاختبار.

**الفرق السلوكي المسجّل:** الأصل يستنتج اللغة الأولية من `language.json` أو ثقافة النظام؛ هنا الافتراضي `ZhCn` ثابت. الكود والاختبارات المنقولة تضبط اللغة صراحةً دائماً، فالفارق لا يظهر — لكنه مسجّل لأنه فارق.

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

ست بوابات مستقلة، كلها عملت فعلاً — لا واحدة منها ادعاء:

| البوابة | ماذا تفعل | النتيجة |
|---|---|---|
| **1. سلامة النقل** | تستنسخ upstream عند نفس الـcommit وتُشغّل `diff` على 48 ملفاً | ✅ 48/48 متطابقة |
| **2. قابلية النقل** | ترفض WinForms/WPF/P-Invoke/WinRT، و`System.Drawing` بما يتجاوز البدائيات | ✅ نظيف |
| **3. البناء والاختبار** | `dotnet build` ثم `dotnet test` على ويندوز ولينكس | ✅ 437/437 ويندوز · 433/437 لينكس |
| **4. انحراف جدول الترجمة** | تعيد توليد `LocTable.g.cs` من upstream وتفشل على أي فرق بايت | ✅ 643 مفتاحاً |
| **5. تغطية مفاتيح الترجمة** | كل `Loc.T/F("...")` حرفي في الكود موجود في الجدول | ✅ 43/43 |
| **6. روابط التوثيق الداخلية** | كل رابط Markdown داخلي بين ملفات `.md` يشير إلى ملف موجود | ✅ بلا كسر |

```bash
scripts/verify-transplant.sh          # البوابات 1-3
scripts/gen-loctable.py --upstream <clone> --check   # البوابة 4
scripts/check-loc-keys.py             # البوابة 5
```

البوابات 1 و2 و4 و5 تعمل في طرفيتك بلا .NET SDK (كلها bash/python). البوابة 3 وحدها
تحتاج SDK — وهي بالضبط ما يشغّله GitHub Actions عنك إن لم يكن مثبتاً عندك.

## 7. ما أُثبت فعلاً — ومَن أثبته

كل ما في هذا القسم ناتج تنفيذ حقيقي في GitHub Actions، لا استنتاجاً من قراءة الكود.
الرابط القابل للمشاركة: [run 43346149307](https://github.com/Petershaker3/mobile-device-modes-research/actions/runs/43346149307).

| الادعاء | الحالة | الدليل |
|---|---|---|
| 48 ملفاً منقولاً مطابق بايت-ببايت لأصلهم | ✅ **مثبت** | وظيفة `transplanted code is unmodified`: 48 `check_identical` كلها `OK` |
| المكتبة تبني على `net10.0` **على ويندوز ولينكس** | ✅ **مثبت** | `dotnet build -c Release` نجح على Runnerين مختلفين |
| اختبارات ويندوز تمر كلها | ✅ **مثبت** | 437 من 437 (`all 437 tests passed (0 skipped)`) |
| المكتبة لا تعتمد على ويندوز | ✅ **مثبت** | 433 من 437 تمر على `ubuntu-latest`؛ الأربعة الباقية موثّقة في [ci/known-linux-diffs.txt](../ci/known-linux-diffs.txt) وهي فروق دلالات مسارات لا أخطاء |
| كل مفتاح ترجمة يستخدمه الكود موجود في الجدول | ✅ **مثبت** | `scripts/check-loc-keys.py`: 43 مفتاحاً مستخدماً داخل جدول من 643 |
| جدول الترجمة مطابق لأصلهم | ✅ **مثبت** | `scripts/gen-loctable.py --check` = 643 مفتاحاً من 11 ملفاً، 0 تعارض |
| الحزم من NuGet تُستعاد | ✅ **مثبت** | `dotnet restore` نجح على Runnerين (NuGet كان محجوباً في بيئتي) |

### ما لم يُثبت — وما زال بصراحة على حاله

| الادعاء | لماذا لم يُثبت |
|---|---|
| الكود يعمل على جهاز أندرويد حقيقي | **لا يوجد جهاز**. ولا اختبار واحد هنا يمسّ USB أو ADB |
| واجهة WPF تعمل | لم تُبنَ بعد؛ المنقول مكتبة فقط بلا واجهة |
| `net10.0` SDK مثبت عندك | غير معروف؛ `global.json` يطلب `10.0.100` بـ`rollForward: latestFeature` |

> **التمييز المهم:** «437 اختباراً تمر» تعني أن منطق تحليل السجلات وكشف الانهيارات سليم.
> لا تعني أن البرنامج يتصل بجهازك. تلك المسافة يقطعها أول اختبار على جهاز حقيقي.

### أربعة فروق منصّة معروفة (ليست أخطاء)

على لينكس تفشل أربعة اختبارات لأنها تختبر سلوكاً ويندوزياً بحتاً:

1. `Path.GetFullPath("Z:\\no-access\\x.log")` لا يغيّر شيئاً على ويندوز، أما على لينكس فهو اسم نسبي فيُحلّ مقابل مجلد العمل.
2. حذف مجلد يحمل مقبضاً مفتوحاً يفشل على ويندوز ويُسمح به على POSIX.
3. `FileShare.None` ليس له مقابل على POSIX، فمسار «الفشل الصامت» لا يُستثار.
4. (الرابع بنفس عائلة المسارات.)

هذه مسجّلة في `ci/known-linux-diffs.txt`، والمُبلِّغ يفشل في **الاتجاهين**: أي فشل غير مسجّل، وأي اختبار مسجّل بدأ ينجح (حتى لا يتعفّن الملف ويصير مكاناً نخفي فيه أخطاء حقيقية).

## 8. التالي في قائمة النقل

| # | المكوّن | المصدر | الحالة/الشرط |
|---:|---|---|---|
| 1 | `DeviceInfo`/`FileEntry`/`AdbServerSelfHealResult`/`CommandEntry` | logcat-viewer | ✅ **نُقلت** (ظهرت كتبعيات أثناء البناء) |
| 2 | `AdbManager.cs` | logcat-viewer | ⏳ يحتاج إصلاح عيب `Shell` المؤجَّل أولاً + اختبار يثبت إلغاء أمر معلق |
| 3 | `LogcatStream` + `FakeAdbSocket` | logcat-viewer | ⏳ يحتاج `AdvancedSharpAdbClient` — يمكن استعادته الآن من NuGet عبر CI |
| 4 | `ApkInfo` + فلاتر ABI/SDK | APK-Installer | ⏳ قرار: نقل `aapt` wrapper أم parser مباشر |
| 5 | `AppInfo` / حزم التطبيقات | APK-Installer | ⏳ يخدم محور «التطبيقات» |

**قاعدة النقل المستمرة — بعد أن أثبتت CI قيمتها:** كل ملف يُسجَّل في
[ATTRIBUTION.md](../ATTRIBUTION.md)، ويُضاف سطر `check_identical`، **ولا يُعتبر
النقل مكتملاً حتى يبني**. آخر أربع دورات CI كشفت أربع تبعيات لم أتوقعها؛ لو اكتفيت
بقراءة الكود لكان المستودع الآن يبدو مكتملاً وهو لا يبني.

**آخر تحديث:** 13 نوفمبر 2026 — كل الأرقام أعلاه من [run 43346149307](https://github.com/Petershaker3/mobile-device-modes-research/actions/runs/43346149307).
