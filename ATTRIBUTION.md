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

### 1.1 الملفات المنقولة حرفياً — `src/DeviceToolkit.Core/` (30 ملفاً)

| الملف المستهدف | المصدر في upstream | الأسطر | تعديل |
|---|---|---:|---|
| `Logcat/LogParser.cs` | `logcat-viewer/Services/LogParser.cs` | 441 |
| `Logcat/LogDocument.cs` | `logcat-viewer/Services/LogDocument.cs` | 1100 |
| `Logcat/FilterEngine.cs` | `logcat-viewer/Services/FilterEngine.cs` | 1133 |
| `Logcat/SidecarCache.cs` | `logcat-viewer/Services/SidecarCache.cs` | 414 |
| `Logcat/SidecarCacheStore.cs` | `logcat-viewer/Services/SidecarCacheStore.cs` | 241 |
| `Diagnostics/CrashExtractor.cs` | `logcat-viewer/Services/CrashExtractor.cs` | 225 |
| `Diagnostics/CrashAggregator.cs` | `logcat-viewer/Services/CrashAggregator.cs` | 59 |
| `Diagnostics/CrashTime.cs` | `logcat-viewer/Services/CrashTime.cs` | 32 |
| `Diagnostics/TextProbe.cs` | `logcat-viewer/Services/TextProbe.cs` | 55 |
| `Diagnostics/CrashEvent.cs` | `logcat-viewer/Models/CrashEvent.cs` | 41 |
| `Diagnostics/FilterSpec.cs` | `logcat-viewer/Models/FilterSpec.cs` | 65 |
| `Diagnostics/FileProxyService.cs` | `logcat-viewer/Services/FileProxyService.cs` | 503 |
| `Diagnostics/FileProxyClient.cs` | `logcat-viewer/Services/FileProxyClient.cs` | 338 |
| `Diagnostics/FileProxyMode.cs` | `logcat-viewer/Services/FileProxyMode.cs` | 40 |
| `Diagnostics/FileProxyException.cs` | `logcat-viewer/Services/FileProxyException.cs` | 56 |
| `Diagnostics/ProxyChannel.cs` | `logcat-viewer/Services/ProxyChannel.cs` | 43 |
| `Infrastructure/LogcatSettings.cs` | `logcat-viewer/Services/LogcatSettings.cs` | 144 |
| `Infrastructure/SettingsIO.cs` | `logcat-uicore/Services/SettingsIO.cs` | 69 |
| `Infrastructure/ConfigMigration.cs` | `logcat-uicore/Services/ConfigMigration.cs` | 53 |
| `Infrastructure/LegacySettings.cs` | `logcat-uicore/Services/LegacySettings.cs` | 53 |
| `Infrastructure/LanguageSettings.cs` | `logcat-uicore/Services/LanguageSettings.cs` | 59 |
| `Infrastructure/AndToolsSettings.cs` | `logcat-uicore/Services/AndToolsSettings.cs` | 131 |
| `Infrastructure/StartupLog.cs` | `logcat-uicore/Services/StartupLog.cs` | 41 |
| `Models/DeviceLastDir.cs` | `logcat-uicore/Models/DeviceLastDir.cs` | 17 |
| `Models/FavoriteDir.cs` | `logcat-uicore/Models/FavoriteDir.cs` | 20 |
| `Models/DeviceInfo.cs` | `logcat-uicore/Models/DeviceInfo.cs` | 45 |
| `Models/FileEntry.cs` | `logcat-uicore/Models/FileEntry.cs` | 18 |
| `Models/TryOpenOutcome.cs` | `logcat-uicore/Models/TryOpenOutcome.cs` | 20 |
| `Models/AdbServerSelfHealResult.cs` | `logcat-uicore/Models/AdbServerSelfHealResult.cs` | 11 |
| `Models/CommandEntry.cs` | `logcat-uicore/Models/CommandEntry.cs` | 99 |
| `Adb/AdbManager.cs` | `logcat-uicore/Services/AdbManager.cs` | 578 | **حذف 60 سطراً** — انظر §1.7 |
| `Adb/TransferProgress.cs` | `logcat-uicore/Services/TransferProgress.cs` | 35 | **لا شيء** |
| `Adb/LogcatStream.cs` | `logcat-viewer/Services/LogcatStream.cs` | 139 | **لا شيء** |
| **الإجمالي** | | **5566** | **صفر تعديلات** |

> **لماذا 30 ملفاً وليس 9؟** النسخة الأولى من هذا الجدول ذكرت 9. CI أثبت في أربع دورات
> متتالية أن `LogDocument` و`FileProxyService` و`LegacySettings` يسحبون تبعيات لم أتوقعها.
> العدد الحقيقي أعلاه مُشتق من الملفات الموجودة فعلاً، لا من قراءتي للكود.

**لماذا نُقلت هذه بعينها:** لا تحتوي أي إشارة إلى `System.Windows.Forms` أو WPF أو
Win32 (`DllImport`)؛ والاستخدام الوحيد لـ`System.Drawing` هو `Point`/`Size` من
`System.Drawing.Primitives` المتاح على كل الأنظمة (`LogcatSettings`). تحقّق آلي لا تخمين:
انظر [§4](#4-كيف-نتحقق-أن-المنقول-سليم)، والبناء الفعلي على لينكس هو الإثبات النهائي.

### 1.2 الاختبارات المنقولة حرفياً — `src/DeviceToolkit.Tests/` (17 ملفاً)

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
| `TestCultureBootstrap.cs` | `tests/logcat.Tests/TestCultureBootstrap.cs` | **لا شيء** |
| `SidecarCacheTests.cs` | `tests/logcat.Tests/SidecarCacheTests.cs` | **لا شيء** |
| `FileProxyProtocolTests.cs` | `tests/logcat.Tests/FileProxyProtocolTests.cs` | **لا شيء** |
| `FileProxyLoopbackTests.cs` | `tests/logcat.Tests/FileProxyLoopbackTests.cs` | **لا شيء** |
| `FileProxyChannelStateTests.cs` | `tests/logcat.Tests/FileProxyChannelStateTests.cs` | **لا شيء** |
| `FileProxyModeTests.cs` | `tests/logcat.Tests/FileProxyModeTests.cs` | **لا شيء** |
| `LegacySettingsDtoTests.cs` | `tests/logcat.Tests/LegacySettingsDtoTests.cs` | **لا شيء** |
| `FakeAdbSocket.cs` | `tests/logcat.Tests/FakeAdbSocket.cs` | **لا شيء** |
| `AdbManagerServerSelfHealTests.cs` | `tests/logcat.Tests/AdbManagerServerSelfHealTests.cs` | **لا شيء** |
| `TransferProgressTests.cs` | `tests/logcat.Tests/TransferProgressTests.cs` | **لا شيء** |
| `FakeProxyServer.cs` | `tests/logcat.Tests/FakeProxyServer.cs` | **لا شيء** |

**437 اختباراً تمر كلها** على `windows-latest` — رقم من CI، لا من مستودع upstream.
(upstream يعلن 852 اختباراً لمشروعه كاملاً؛ لم ننقل إلا ما يخص المكوّنات المنقولة.)

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
| ~~`FileProxyService/Client`, `SidecarCache`~~ | **نُقلت.** هذه الحجة كانت خطأ أثبتته CI: `LogDocument` نفسه يعتمد على `SidecarPayload` و`SpoolDirLease` منهما، فتركهما يمنع البناء |
| `Loc.cs` الأصلي (559 سطراً) | غالبه WinForms (ToolTip، شجرة التحكم، قياس البكسل، `language.json`). نُقلت منه 4 أعضاء في ملف جديد (§1.5) و**جدول الترجمة كاملاً** بمولّد آلي (§1.5.1) |
| `LocTable.*.cs` الأصلي | لم يُنقل كملفات، بل يُستخرج منه جدول CS مُولَّد — انظر §1.5.1 |
| كل ملفات `Forms/`, `Controls/` | UI — مستبعد بقرار ADR-0001 (WPF) |
| 42 ملف اختبار آخر | تخص مكوّنات لم تُنقل |

### 1.5 `Loc` — ملف جديد يحمل 4 أعضاء منقولة حرفياً

الأصل: `logcat-uicore/Services/Loc.cs` (559 سطراً). الكود المنقول يحتاج منه أربعة أعضاء
فقط. نقلُه كاملاً كان سيسحب WinForms إلى مكتبة محمولة.

| العضو | حالة النقل |
|---|---|
| `UiLang { ZhCn = 0, En = 1 }` | حرفي — نفس القيم والترتيب |
| `T(string zh)` | حرفي؛ الجسم الأصلي مقتبس في تعليق فوقه |
| `F(string zh, params object[])` | حرفي، بما فيه `FormatException` ⇒ نص بلا تنسيق |
| `SetLangGuard(UiLang)` | حرفي |

**فرق سلوكي واحد مسجَّل:** الأصل يستنتج اللغة الأولية من `language.json` ثم ثقافة النظام؛
هنا الافتراضي ثابت على `ZhCn`، لأن الاختبارات والكود المنقولين يضبطون اللغة صراحةً دائماً.
الوقف على `ZhCn` مطابق لسلوك upstream على جهاز صيني.

### 1.5.1 `LocTable.g.cs` — مُولَّد آلياً من upstream (643 مفتاحاً)

| البند | القيمة |
|---|---|
| المصدر | 11 ملفاً: `logcat-uicore/Services/LocTable.*.cs` |
| المولّد | `scripts/gen-loctable.py` |
| المحتوى | 643 مفتاحاً، **0 تعارض** بين الملفات |
| التحقق في CI | `--check` يعيد التوليد ويفشل على أي فرق بايت |
| التغطية في CI | `scripts/check-loc-keys.py` — كل `Loc.T/F("...")` حرفي في الكود موجود في الجدول |

**لماذا مولَّد لا منقول؟** لأن upstream يوزّع نفس الجدول على 11 ملف `partial`.
نسخها كما هي يتطلب إعادة بناء `partial class LocTable` وأساليب المصنع؛ والتوليد يعطي
نفس المحتوى بملف واحد قابل للتحقق الآلي. المحتوى نفسه منسوخ حرفياً بلا ترجمة من عندنا.

> **الدرس المسجَّل:** المحاولة الأولى يدوياً نقلت 19 مفتاحاً اخترتها بنفسي. CI أثبت أن
> اختباراً منقولاً يسأل عن مفتاح لم أنقله، فيسقط إلى الصينية صامتاً. لا تخمّن أي مفاتيح
> يستخدمها كود منقول — انقل الجدول كله آلياً.

### 1.6 التعديل الوحيد على ملف منقول

`src/DeviceToolkit.Tests/TestHelpers.cs` — **7 أسطر محذوفة، موضعان**، وهو الملف المنقول
الوحيد الذي مُسّ. التفصيل في §5.1 أدناه، وسكربت التحقق يطبع تحذيراً صريحاً عند هذا الملف
بدلاً من اعتباره متطابقاً.

### 1.7 `AdbManager.cs` — التعديل الثاني، ولسبب مختلف

`logcat-uicore/Services/AdbManager.cs` (636 سطراً) هو الملف المنقول الثاني الذي مُسّ.
السطر الوحيد الذي يمنع نقله كما هو هو Screenshot: 58 سطراً تبني `Bitmap` من
framebuffer الجهاز عبر `System.Drawing`، وهي حزمة Windows-only كانت ستنقض قابلية
النقل التي أثبتها CI على لينكس.

| ما أُزيل | العدد | السبب |
|---|---:|---|
| منطقة `截图` كاملة (`Screenshot`, `ScreenshotAsync`, `FramebufferToImage`) | 58 سطراً | `System.Drawing` — تخص الواجهة لا المكتبة |
| `using System.Drawing;` و`using System.Drawing.Imaging;` | 2 | لم يبقَ ما يحتاجهما |
| **مُضاف**: سطران يعلنان التعديل | +2 | ليقرأه من يفتح الملف |

**كل ما تبقّى حرفي.** لكن «حذفنا حوالي ٦٠ سطراً» ليست ضمانة: لو حذف أحدهم ٦١ سطراً
وأضاف سطراً آخر لطابق العدد وبقي التعديل خفياً. لذلك الفحص **بنيوي** لا حسابي:
`scripts/check-subsequence.py` يقسّم ملفنا إلى أطول مقاطع متتالية موجودة حرفياً في
upstream، ويشترط ظهورها بترتيب upstream، ويفشل على أي سطر مُعدَّل أو مُدخَل أو مُعاد
ترتيبه. اختبرتُه على الحالات الثلاث قبل وصله بالفحص:

| الحالة | النتيجة |
|---|---|
| ملفنا كما هو | ✅ 2 مقطعاً، 576 سطراً محفوظاً، 58 محذوفاً |
| تغيير سطر واحد (`RootProbeTimeoutSec`) | ❌ فشل عند السطر 41 |
| إدخال سطر واحد | ❌ فشل عند السطر 100 |
| إعادة ترتيب المقاطع | ❌ فشل — «المقاطع خارج ترتيب upstream» |

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

**أمر واحد يكفي، ويعمل في CI:**

```bash
scripts/verify-transplant.sh --no-build   # 48 ملفاً + فحص قابلية النقل
scripts/verify-transplant.sh              # ويضيف build + test (يحتاج .NET SDK)
```

والنتيجة الموثّقة من CI: 48/48 ملفاً متطابقة، والبناء ناجح على ويندوز ولينكس، و437/437
اختباراً تمر. الأوامر اليدوية التالية محفوظة لمن يريد التحقق دون السكربت:

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
| `DeviceToolkit.Core/Localization/Loc.cs` | `UiLang`, `T`, `F`, `SetLangGuard` | الملف الأصلي 559 سطراً، غالبه بناء WinForms. الكود المنقول يحتاج 4 أعضاء فقط. كل عضو هنا منقول حرفياً من جسمه في upstream، والاقتباس مذكور في تعليق فوقه. التفصيل: §1.5 |
| `DeviceToolkit.Core/Localization/LocTable.g.cs` | جدول الترجمة الكامل (643 مفتاحاً) | **مُولَّد** من 11 ملف `LocTable.*.cs` — لا يُحرَّر يدوياً. التفصيل: §1.5.1 |
| `scripts/gen-loctable.py` | يولد `LocTable.g.cs` ويتحقق من عدم انحرافه عن upstream | upstream لا يملك مولّداً؛ الجدول عنده موزّع على ملفات `partial` |
| `scripts/check-loc-keys.py` | يثبت أن كل مفتاح يستخدمه الكود موجود في الجدول | يمنع السقوط الصامت إلى الصينية — عيب حقيقي كشفته CI |
| `scripts/verify-transplant.sh` | يثبت أن الملفات المنقولة متطابقة + قابلية النقل + البناء | upstream لا يملك آلية تحقق من النقل |
| `scripts/check-subsequence.py` | يثبت أن `AdbManager.cs` هو upstream مع حذف مقاطع كاملة فقط | الفحص الحسابي («حُذف ٦٠ سطراً») يقبل تعديلاً خفياً |
| `Adb/AdbShell.cs` | مهلة مفروضة على `adb shell` | **upstream نفسه لا يفعلها** — انظر §5.2 |
| `DeviceToolkit.Core/DeviceToolkit.Core.csproj` | تعريف المشروع | لا وجود له upstream بهذا الشكل (الكود كان داخل تطبيق WinForms) |
| `DeviceToolkit.Tests/DeviceToolkit.Tests.csproj` | تعريف مشروع الاختبار | نفس السبب |
| `Directory.Build.props`, `global.json`, `DeviceToolkit.slnx` | بناء الحل | بنية حل جديدة |

### 5.2 لماذا `AdbShell` جديد ولم يُصلَح `AdbManager.Shell`؟

`AdbManager.Shell(serial, cmd, timeoutSec = 30)` **يأخذ مهلة ولا يستخدمها**:

```csharp
public string Shell(string serial, string cmd, int timeoutSec = 30)
{
    var device = FindDevice(serial);
    var receiver = new ConsoleOutputReceiver();
    _client.ExecuteRemoteCommand(cmd, device, receiver, Encoding.UTF8);
    return receiver.ToString();          // ← timeoutSec لا يُذكر في الجسم
}
```

وقناة الأوامر في `AdvancedSharpAdbClient` بلا أي مهلة داخلية، فجهاز ينتظر لمسة على
نافذة `su` — أو `logcat` معلّق، أو `find /` على قسم ممتلئ — يحجز الخيط إلى الأبد.
upstream يعرف ذلك: تعليق `RootProbeTimeoutSec` يصف المشكلة حرفياً، ثم يحلّها لمسار
root probe وحده بسِباق يدوي ٨ ثوانٍ.

**الخيار المرفوض: تعديل `AdbManager.cs`.** سبب وجود هذا المستودع هو أن المنقول مطابق
قابل للإثبات، و`check-subsequence.py` يثبت ذلك بنيوياً. شيم واحد لحذف `System.Drawing`
لا مفرّ منه مفهوم؛ شيم ثانٍ يغيّر **السلوك** كان سيحوّل «نقلنا upstream» إلى «نقلنا
شيئاً يشبه upstream».

**الخيار المتَّخذ:** `AdbShell` يلتف حول العيب بدل أن يخفيه — لا يستدعي `Shell` إطلاقاً،
بل يركّب بديلين upstream نفسه يملكهما ويصحّهما:
`AdbManager.AdbArgs` (يضيف `-s <serial>`) و`AdbManager.RunAdbAsync` (يقتل شجرة العملية
عند الإلغاء) — خلف `CancellationTokenSource` مرتبط بمهلة المتصل. انتهاء المهلة ⇒
`TimeoutException`؛ وإلغاء المتصل يبقى إلغاءً لا مهلة.

**العيب يبقى قائماً في `AdbManager.Shell`.** أي كود يستدعي `Shell` مباشرة — كما تفعل
واجهة upstream في ٢٥ موضعاً — لا يزال عرضة للتعليق. هذا مسجَّل هنا كدَين تقني معلوم،
لا كمشكلة محلولة.

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
