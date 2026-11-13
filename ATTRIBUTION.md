# ATTRIBUTION — الكود المنقول

> **قاعدة هذا الملف:** كل ملف منقول يُسجَّل هنا بمصدره وSHA وماذا عُدِّل فيه. أي ملف لم يُذكر هنا هو كود أصلي في هذا المستودع.
>
> **سياسة النقل:** الملف المنقول يُنسخ **حرفياً**. لا يُعدَّل سطر واحد. إن احتجنا سلوكاً مختلفاً، نضيف ملفاً جديداً فوقه ولا نلمس المنقول.

---

## 1. المصدر الأول: `daitouniao/logcat-viewer`

| البند | القيمة |
|---|---|
| المستودع | https://github.com/daitouniao/logcat-viewer |
| Commit | `a985616224a342d6064ed962eec28ace3f38365a` |
| التاريخ | 10 نوفمبر 2026 |
| الترخيص | **Apache-2.0** |
| لماذا | أقرب كود C# ناضج لـlogcat لنا: parser لست صيغ، فهرس memory-mapped لملفات ضخمة، ومحرك فلترة بـSIMD. 852 اختباراً upstream (94.26% تغطية) تُنقل معه |

**إشعار Apache-2.0 (المادة 4):** الكود التالي مأخوذ من مشروع بترخيص Apache License 2.0، ويُعاد استخدامه هنا مع الإشارة إلى المصدر. يحتفظ كل ملف بتعليقاته الأصلية كما هي. نص الترخيص الكامل: https://www.apache.org/licenses/LICENSE-2.0

### 1.1 الملفات المنقولة حرفياً — `src/DeviceToolkit.Core/`

| الملف المستهدف | المصدر في upstream | الأسطر | تعديل |
|---|---|---:|---|
| `Logcat/LogParser.cs` | `logcat-viewer/Services/LogParser.cs` | 441 | **لا شيء** |
| `Logcat/LogDocument.cs` | `logcat-viewer/Services/LogDocument.cs` | 1100 | **لا شيء** |
| `Logcat/FilterEngine.cs` | `logcat-viewer/Services/FilterEngine.cs` | 1133 | **لا شيء** |
| `Diagnostics/CrashExtractor.cs` | `logcat-viewer/Services/CrashExtractor.cs` | 225 | **لا شيء** |
| `Diagnostics/CrashAggregator.cs` | `logcat-viewer/Services/CrashAggregator.cs` | 59 | **لا شيء** |
| `Diagnostics/CrashTime.cs` | `logcat-viewer/Services/CrashTime.cs` | 32 | **لا شيء** |
| `Diagnostics/TextProbe.cs` | `logcat-viewer/Services/TextProbe.cs` | 55 | **لا شيء** |
| `Diagnostics/CrashEvent.cs` | `logcat-viewer/Models/CrashEvent.cs` | — | **لا شيء** |
| `Diagnostics/FilterSpec.cs` | `logcat-viewer/Models/FilterSpec.cs` | — | **لا شيء** |

**لماذا نُقلت هذه بعينها:** لا تحتوي أي إشارة إلى `System.Windows.Forms` أو `System.Drawing` أو Win32 (`DllImport`)، فتعمل على `net10.0` عادي بلا `-windows`. هذا تحقّق آلي، لا تخمين: انظر [§4](#4-كيف-نتحقق-أن-المنقول-سليم).

### 1.2 الاختبارات المنقولة حرفياً — `src/DeviceToolkit.Tests/`

| الملف | المصدر | تعديل |
|---|---|---|
| `LogParserTests.cs` | `tests/logcat.Tests/LogParserTests.cs` | **لا شيء** |
| `LogDocumentTests.cs` | `tests/logcat.Tests/LogDocumentTests.cs` | **لا شيء** |
| `LogDocumentZeroCopyTests.cs` | `tests/logcat.Tests/LogDocumentZeroCopyTests.cs` | **لا شيء** |
| `LogDocumentProxiedTests.cs` | `tests/logcat.Tests/LogDocumentProxiedTests.cs` | **لا شيء** |
| `FilterEngineTests.cs` | `tests/logcat.Tests/FilterEngineTests.cs` | **لا شيء** |
| `FilterEngineBlockScanTests.cs` | `tests/logcat.Tests/FilterEngineBlockScanTests.cs` | **لا شيء** |
| `ChunkedEquivalenceTests.cs` | `tests/logcat.Tests/ChunkedEquivalenceTests.cs` | **لا شيء** |
| `CrashTests.cs` | `tests/logcat.Tests/CrashTests.cs` | **لا شيء** |
| `TextProbeTests.cs` | `tests/logcat.Tests/TextProbeTests.cs` | **لا شيء** |
| `TestHelpers.cs` | `tests/logcat.Tests/TestHelpers.cs` | **لا شيء** |
| `TestCultureBootstrap.cs` | `tests/logcat.Tests/TestCultureBootstrap.cs` | **لا شيء** |

مجموع ما فيها: **246 `[Fact]` + 24 `[Theory]` مع 98 `[InlineData]`** = مئات الحالات، كلها تعمل بلا جهاز وبلا Windows.

### 1.3 كيف حُلّت الاعتماديات بدون تعديل أي سطر منقول

| المشكلة | الحل | أين |
|---|---|---|
| الملفات المنقولة تعلن `namespace logcat.Services` و`logcat.Models` | `RootNamespace=logcat` في الـcsproj — لا حاجة لتعديل الأسماء | `DeviceToolkit.Core.csproj` |
| الكود الأصلي يعلن `InternalsVisibleTo("logcat.Tests")` | `AssemblyName=logcat.Tests` في مشروع الاختبار + `AssemblyAttribute` للـInternalsVisibleTo في Core | الـcsproj ـان |
| `LogDocument`/`FilterEngine` تستخدم مؤشرات خام | `AllowUnsafeBlocks=true` كما في upstream | `DeviceToolkit.Core.csproj` |
| كان الكود داخل تطبيق WinForms | Core أصبح `net10.0` بلا WinForms — لأن الكود المنقول لا يستخدمه أصلاً | `DeviceToolkit.Core.csproj` |

**النتيجة:** صفر تعديل على 8,530 سطراً منقولة. الـcsproj ـان هما الشيء الوحيد المكتوب جديداً في هذا الجزء.

### 1.4 ما لم يُنقل من هذا المصدر وسببه

| غير منقول | السبب |
|---|---|
| `AdbManager.cs` (636 سطراً) | يستخدم `AdvancedSharpAdbClient` + `System.Drawing` للـscreenshot، وفيه الفجوة الموثقة (`timeoutSec` غير مستخدم). سيُنقل لاحقاً **بعد** إضافة مهلة فعلية — كملفنا لا كملفه |
| `LogcatStream.cs` | يحتاج `AdvancedSharpAdbClient` + اتصال جهاز حقيقي للاختبار. يُنقل عند بناء طبقة ADB |
| `FileProxyService/Client`, `SidecarCache` | مرتبطة ببنية التطبيق (channel, spool, sidecar) وإعدادات UI. ليست أساساً لأي محور في V1 |
| `Loc.cs` + جدول الترجمة | نظام تعريب خاص بهم؛ منتجنا سيتخذ قرار تعريب مستقل |
| كل ملفات `Forms/`, `Controls/` | UI — مستبعد بقرار ADR-0001 (WPF) |
| 42 ملف اختبار آخر | تخص مكوّنات لم تُنقل |

---

## 2. المصادر المرشحة القادمة (لم يُنقل منها شيء بعد)

| المصدر | ما سيُنقل | الحالة |
|---|---|---|
| `daitouniao/logcat-viewer` | `AdbManager.cs` بعد إصلاح المهلة، `LogcatStream.cs` | التالي |
| `Paving-Base/APK-Installer` | نموذج `ApkInfo` + فلاتر ABI/SDK/فلاتر الحزمة (MIT) | لاحقاً |
| `wkbin/AdbFileManager` | **أنماط فقط** (Kotlin — لا كود) | لاحقاً |
| `Wilsoon7721/WinDroidFS` | **لا شيء** — المستخدم أذن، لكن في بيئة Rust ونقلها لـ.NET إعادة كتابة كاملة بلا فائدة | مستبعد تقنياً |

---

## 3. سياسة الاستخدام

بيئة التشغيل: **استخدام شخصي على جهازك**. لذلك لا يوجد قيد نشر، ويمكن خلط تراخيص بحرية.

**لكن هذا لا يغير قاعدتين:**
1. الترخيص GPL-3.0 يفرض إتاحة المصدر **لو** وزّعت البرنامج. ما دام شخصياً، لا أثر.
2. `THIRD-PARTY-NOTICES` يبقى مكتوباً، ليس لأسباب قانونية فقط، بل لأنك بعد 6 أشهر لن تتذكر أي ملف جاء من أين. هذا الملف هو ذاكرتك.

---

## 4. كيف نتحقق أن المنقول سليم

بدل الثقة، هذه أوامر تُثبت أن النقل حرفي:

```bash
# 1) بصمة المنقول تساوي بصمة المصدر عند نفس الـcommit
#    (نستخدم نسخة مرجعية نظيفة من upstream)
git clone --depth 1 https://github.com/daitouniao/logcat-viewer.git /tmp/up
git -C /tmp/up fetch --depth 1 origin a985616224a342d6064ed962eec28ace3f38365a
git -C /tmp/up checkout a985616224a342d6064ed962eec28ace3f38365a

for f in LogParser LogDocument FilterEngine; do
  diff -q /tmp/up/logcat-viewer/Services/$f.cs src/DeviceToolkit.Core/Logcat/$f.cs && echo "$f identical"
done
for f in CrashExtractor CrashAggregator CrashTime TextProbe; do
  diff -q /tmp/up/logcat-viewer/Services/$f.cs src/DeviceToolkit.Core/Diagnostics/$f.cs && echo "$f identical"
done

# 2) لا شيء منقول يعتمد على Windows
grep -rn "System.Windows.Forms\|System.Drawing\|DllImport" src/DeviceToolkit.Core/ || echo "portable: no WinForms/Drawing/P-Invoke"

# 3) يبني ويعمل
dotnet test DeviceToolkit.slnx
```

**آخر تحديث:** 13 نوفمبر 2026.

---

## 5. ملفات جديدة (ليست منقولة) — `src/`

| الملف | الغرض | لماذا كُتب جديداً |
|---|---|---|
| `DeviceToolkit.Core/Localization/Loc.cs` | `UiLang`, `T`, `F`, `SetLangGuard` + جدول الترجمة المطلوب | الملف الأصلي 559 سطراً، غالبه بناء WinForms (ToolTip، شجرة Controls، قياس خطوط). الكود المنقول يحتاج 4 أعضاء فقط. كل عضو هنا منقول حرفياً من جسمه في upstream، والاقتباس مذكور في تعليق فوقه |
| `DeviceToolkit.Core/DeviceToolkit.Core.csproj` | تعريف المشروع | لا وجود له upstream بهذا الشكل (الكود كان داخل تطبيق WinForms) |
| `DeviceToolkit.Tests/DeviceToolkit.Tests.csproj` | تعريف مشروع الاختبار | نفس السبب |
| `Directory.Build.props`, `global.json`, `DeviceToolkit.slnx` | بناء الحل | بنية حل جديدة |

### 5.1 تعديل واحد على ملف منقول

`src/DeviceToolkit.Tests/TestHelpers.cs` هو الملف الوحيد المنقول الذي **مُسّ**. التعديل محدود وبنيوي:

| ما أُزيل | السبب |
|---|---|
| تعريف `WinFormsCollection` (6 أسطر) | لا اختبار منقول يبني WinForm، والمكتبة بلا أي referencia لـWinForms |
| تعليق `[Collection(WinFormsCollection.Name)]` في وثيقة XML فوقه | لأن النوع لم يبقَ |

**كل ما عداه حرفي.** الفرق مع upstream محدود بهذين الموضعين بالضبط — يظهر بأمر واحد:
```bash
diff /tmp/up/tests/logcat.Tests/TestHelpers.cs src/DeviceToolkit.Tests/TestHelpers.cs
```
