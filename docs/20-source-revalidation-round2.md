# Source Revalidation — الجولة الثانية (بعد ADR-0001)

> **الغرض:** استكمال Source Revalidation للمرشحين الذين لم يُراجَعوا بعد في [13](13-source-revalidation-adb-file-core.md) و[14](14-source-revalidation-app-installers.md) و[15](15-source-revalidation-mcp-codex.md) و[16](16-source-revalidation-firmware-storage-backup.md)، **وبترتيب جديد** لأن [ADR-0001](17-adr-0001-stack-decision.md) جعل معيار الترتيب هو "قابلية الاقتباس داخل .NET 10".
>
> **ما يثبته:** كود موجود عند commits محددة، وحدود قابلة للتحقق.
> **ما لا يثبته:** لم يُبنَ أي مشروع، ولم تُشغَّل اختبارات، ولم يُوصَل جهاز.

**تاريخ المراجعة:** 13 نوفمبر 2026
**طريقة المراجعة:** تنزيل archive لكل مستودع عند SHA محدد (`codeload.github.com`)، قراءة الشجرة والملفات، عدّ الاختبارات. لم يُشغَّل build.

---

## 1. جدول القرار السريع

| المشروع | Commit | الترخيص | الستاك | اختبارات | استحقاق الاقتباس |
|---|---|---|---|---|---|
| `daitouniao/logcat-viewer` | `a985616224a342d6064ed962eec28ace3f38365a` | Apache-2.0 | **.NET 10 / WinForms / AdvancedSharpAdbClient 3.6.16** | 852 (94.26% سطور) | **الأعلى** — كود C# ناضج بنفس الستاك |
| `Paving-Base/APK-Installer` | `c8f4e634b72c5555cffe9cc5759074a866b68ae1` | MIT | .NET 10 / **WinUI 3** / AdvancedSharpAdbClient 3.0.9 | لا اختبارات لهذا المسار | متوسط — نموذج metadata فقط |
| `wkbin/AdbFileManager` | `852c0bb83d8febce12a87d39148e57fa54b4e1c3` | MIT | **Kotlin 2.4 / Compose Desktop** | 8 ملفات اختبار | منخفض — أنماط فقط |
| `Wilsoon7721/WinDroidFS` | `7deb96eb7aeff6b095f4aeabebdab4a9af68dc63` | **GPL-3.0** | **Rust** + Kotlin companion + WinFsp | Kotlin tests موجودة | **محجوب — لا اقتباس كود** |
| `kefengwei/LogCater` | `916cae7fd277f645c933723c2979d7415505604d` | — | **C++ 20 / Dear ImGui / OpenGL** | — | منخفض جداً |
| `VenkateshJallu/Bugreport_analyzer` | `8fa9e26d51eee5fa1aeab098f796a6d709c7b7e9` | — | **Python 3.8 / single file 3,459 سطر** | **لا اختبارات** | منخفض — قواعد تحليل فقط |

---

## 2. اكتشافات تصحيحية (تُخالف ما هو مكتوب في الوثائق الحالية)

هذه أهم مخرجات الجولة: أربعة تصحيحات لوثائق قائمة.

### 2.1 `docs/10 §6.2` تصف logcat-viewer بأنه "مرجع أداء" — الوصف ناقص

الوثيقة تصفه بأنه "مرجع Windows للأحجام الكبيرة" وتذكره مع LogFilter وSherlog كمراجع أداء. الحقيقة من الشجرة:

- **مبني على `net10.0-windows`** — أي نفس runtime الذي اختاره ADR-0001.
- يستخدم **`AdvancedSharpAdbClient 3.6.16`** — نفس المكتبة المرشحة كـHost Core.
- حل من **4 مشاريع**: `logcat.csproj` (viewer)، `logcat.AndTools.csproj` (toolbox)، `logcat.UiCore.csproj` (**مكتبة مشتركة** تحمل طبقة ADB)، `logcat.Tests`.
- **`logcat-uicore/Services/AdbManager.cs` = 636 سطراً**: تعداد أجهزة، `DeviceMonitor`، self-heal لـadb server، **مهلة على كل أمر root**، `Shell`/`ShellAsync`، push/pull، screenshot، root detection بـcache.
- **52 ملف اختبار**، 852 حالة، تغطية 88.64% branches على `Services`+`Models` (باستثناء UI وطبقة الجهاز).
- رخصة **Apache-2.0** + `THIRD-PARTY-NOTICES.md` + `DISCLAIMER.md` موجودة.
- فيه `openspec/` بتغييرات موثقة (change specs) — دليل على عملية تغيير منضبطة.

**الخلاصة:** هذا ليس "مرجع أداء"، بل **أقرب مرجع تنفيذي متاح** لطبقة ADB في .NET، وأعلى قيمة اقتباس من كل المرشحين. تصحيح الوثيقة 10 مطلوب.

**لكن الفجوات خطيرة ويجب تسجيلها:**

| الفجوة | الدليل | الأثر |
|---|---|---|
| `Shell(serial, cmd, timeoutSec = 30)` — **المعامل `timeoutSec` غير مستخدم في الجسم** | `AdbManager.cs:188-194`؛ فقط `RootProbeTimeoutSec` مستخدم عبر `cts.CancelAfter` في دالة أخرى | ⚠️ **مهلة غير منفَّذة**: أمر معلَّق يعلّق الـUI. هذا خطر مباشر على سياستنا في [18 §3](18-ENGINEERING-DECISIONS.md) |
| `Shell` يأخذ `cmd` كنص واحد | `ExecuteRemoteCommand(cmd, …)` | لا quoting مركزي؛ يجب إضافة طبقة `ShellQuoter` فوقه |
| لا يوجد فصل `Result` عن `string` | يُعيد `string` أو يرفع exception | لا `problemCode`؛ التصنيف يضيع |
| ملفات التثبيت تستخدم `adb.exe` كعملية منفصلة | `ApkInstallDialog.cs:304` يبني `$"install {flags} \"{apk}\""` كسلسلة | quoting يدوي + `-t`/`-g` معرّضة للاختيار، وتعارض مع سیاستنا ([18 §5](18-ENGINEERING-DECISIONS.md): لا test-only تلقائي) |
| `DeviceSelector` يدير "الجهاز الحالي" بمؤشر | `DeviceSelector.cs` (`SelectedIndex`, `CurrentSerial`) | نموذج اختيار جهاز واحد، لا `DeviceHandle` صريح لكل عملية — يجب استبدال النموذج لا نقله |
| الـroot detection يُشغّل `su` | `RootDetectEntry`, `BuildRootShellCmd` | مسار root مرفوض في سياستنا ([18 §5](18-ENGINEERING-DECISIONS.md)) — يجب تعطيله لا اقتباسه |

**الحكم:** **اقتباس انتقائي + إصلاح إلزامي.** يُقتبس: بنية الحل (library مشتركة)، نمط `DeviceMonitor` + self-heal، نمط حقن الـseams للاختبار، تصميم memory-mapped log index. **لا يُقتبس:** `Shell` بلا مهلة، نموذج `DeviceSelector`، مسار root، بناء أوامر التثبيت.

### 2.2 `docs/10 §6.3` تصف LogCater كأداة "C++/ImGui" — مؤكَّد، لكن الوثيقة لم تذكر MCP

الشجرة فيها `src/mcp/` — أي أن LogCater يضم **خادم MCP داخلي**. الوثيقة 10 تذكر فقط "مسارات MCP/الأوامر التي تحتاج إعادة تصميم". يجب أن يُضاف كمرجع لتصميم MCP، مع نفس التحذيرات (اختيار جهاز تلقائي).

### 2.3 `docs/12 §2` يضع APK-Installer كـ"مرجع UI/ADB جزئي" — الأدق: مرجع **metadata model** فقط

- `AAPT2ForNet` مشروع مستقل: `AAPTool.cs`، `ApkParser.cs`، `ApkExtractor.cs`، `ResourceDetector.cs`، و6 filters (`ABIFilter`, `SDKFilter`, `PackageFilter`, `PermissionFilter`, `SupportScrFilter`, `ApplicationFilter`).
- `ApkParser` يعمل على مخرجات `aapt dump badging` **كنص**، مع `BaseFilter.CanHandle/AddMessage`.
- هذا **نموذج metadata مرجعي جيد**: ABI, SDK, package, permissions, supported screens, application info.
- **لكن** التطبيق كله WinUI 3 (`Microsoft.WindowsAppSDK 2.0.1`) — تطبيق ADR-0001 يستبعد WinUI 3. إذن لا يُقتبس UI. يُقتبس **تصميم حقول `ApkInfo` وترتيب الـfilters** فقط، والتنفيذ يُكتب فوق parser manifest مباشر (وليس نص aapt) لتفادي هشاشة صيغة المخرجات.
- الرخصة: MIT (تحقق من `LICENSE` في المستودع قبل الاقتباس الفعلي).

### 2.4 `docs/09 §5.3` تصف WinDroidFS كـ"مرشح اختياري" — **الترخيص GPL-3.0 يحجب الاقتباس**

`apps/windows-daemon/Cargo.toml`: `license = "GPL-3.0-only"`. الوثيقة 09 لا تذكر الترخيص إطلاقاً، وتقول "مرشح لتكامل اختياري". **GPL-3.0 يمنع نسخ كود إلى منتجنا المغلق.** يُصحَّح الوصف إلى "مرجع تصميم مُوثَّق، لا اقتباس كود"، ويُسجَّل سبب الرفض مع بقاء الفكرة (AOA + WinFsp + WebDAV fallback) كفكرة قابلة لإعادة التنفيذ المستقل.

**تصحيح إضافي مفيد:** الـdaemon فيه **fallback إلى WebDAV** عند غياب WinFsp (`build.rs` يوضح delay-load). هذه فكرة جيدة تُنقل نصاً (فكرة، لا كود): لا تجعل تركيب قرص افتراضي شرطاً، بل fallback إلى WebDAV.

### 2.5 Bugreport_analyzer — أصغر مما يُتصور

**ملف واحد، 3,459 سطر، Python، لا اختبارات، لا `requirements.txt`**، يعمل على `bugreport.txt` (لا ZIP)، يستورد `matplotlib` فقط، ويُخرج HTML + PNG.
**الحكم:** قواعد التحليل (battery drain, ANR, crashes, LMK, thermal, wakelocks) مرجع مفيد؛ لا اقتباس كود، ولا فكرة "نستورد السكربت كـsidecar" — نُعيد تنفيذ القواعد في C# على corpus من الفئة ب/هـ.

### 2.6 AdbFileManager — مؤكَّد Kotlin، وإضافة تصحيحية

- الوثيقة 09 تقول "Kotlin/Compose Desktop" — صحيح (`Kotlin 2.4.10`, `Compose 1.9.0`).
- **جديد:** يضم ADB مضمّناً ويتحقق من SHA-256 الخاص به قبل الاستخدام، ويذكر صراحة "identical or currently running adb.exe is not overwritten". هذه **فكرة** تُقتبس: سياسة "لا تستبدل adb.exe قيد التشغيل".
- 8 ملفات اختبار فقط لـ28M مشروع — تغطية أقل مما يوحي README.
- **الحكم:** أنماط Unicode staging + dual-pane + "لا تستبدل adb الجاري" فقط.

---

## 3. ما يُقتبس فعلياً — قائمة تنفيذية

| # | المصدر | ما يُقتبس | كيف (اقتباس/إعادة تنفيذ) | الشرط |
|---:|---|---|---|---|
| 1 | logcat-viewer | فصل حل إلى library مشتركة + exe | إعادة تنفيذ | — |
| 2 | logcat-viewer | نمط `DeviceMonitor` مع `DevicesChanged` + snapshot خارج القفل | **نقل نمط** | يمر عبر `DeviceHandle` |
| 3 | logcat-viewer | حقن الـseams (`Func<bool> statusProbe`, `Func<IAdbSocket> socketFactory`) للاختبار | **نقل نمط** | إلزامي في كل عميل ADB |
| 4 | logcat-viewer | memory-mapped + columnar index + virtual list للـlogcat | إعادة تنفيذ (WPF) | بعد قياس على corpus |
| 5 | logcat-viewer | self-heal لـadb server عند فقده | إعادة تنفيذ | مع مهلة |
| 6 | logcat-viewer | `THIRD-PARTY-NOTICES` + `DISCLAIMER` كممارسة إصدار | **نقل نصي** | إلزامي قبل أول إصدار |
| 7 | APK-Installer | حقول `ApkInfo` وترتيب filters | تصميم | parser manifest مباشر لا نص aapt |
| 8 | AdbFileManager | Unicode staging + remote rename | إعادة تنفيذ | في File Service |
| 9 | AdbFileManager | لا تستبدل `adb.exe` قيد التشغيل | **نقل نصي** | في Tool Manager |
| 10 | WinDroidFS | فكرة WebDAV fallback عند تعذر FS الأصلي | فكرة فقط | **لا كود (GPL)** |
| 11 | Bugreport_analyzer | قواعد تحليل battery/ANR/LMK/thermal | إعادة تنفيذ C# | على corpus فئة ب |

## 4. ما يُرفض صراحةً

| # | المصدر | المرفوض | السبب |
|---:|---|---|---|
| 1 | logcat-viewer | `Shell(serial,cmd,timeoutSec)` كما هو | المهلة غير منفَّذة |
| 2 | logcat-viewer | `DeviceSelector` بنموذج "الجهاز الحالي" | يخالف explicit targeting |
| 3 | logcat-viewer | root detection + `su` | خارج نطاق الكتابة |
| 4 | logcat-viewer | بناء أوامر التثبيت كسلسلة | يخالف `argv[]` في [19](19-SHARED-CONTRACTS.md) |
| 5 | APK-Installer | WinUI 3 UI كامل | يخالف ADR-0001 |
| 6 | APK-Installer | مسار WSA (`localhost:58526`) | لا علاقة بأجهزة حقيقية |
| 7 | APK-Installer | parser نص `aapt dump` كعقد | هشاشة صيغة المخرجات |
| 8 | WinDroidFS | **كل كود** | GPL-3.0 |
| 9 | LogCater | كود + MCP + auto-device | C++/ImGui + سياسة جهاز |
| 10 | Bugreport_analyzer | السكربت كـsidecar | لا اختبارات، ملف واحد، Python runtime |

## 5. أثر على الوثائق القائمة

| الملف | التصحيح المطلوب | الحالة |
|---|---|---|
| [10 §6.2](10-android-diagnostics-and-logs.md) | ترقية logcat-viewer من "مرجع أداء" إلى "أعلى قيمة اقتباس (.NET 10)" + تسجيل فجوة المهلة | 📝 مطلوب |
| [10 §6.3](10-android-diagnostics-and-logs.md) | إضافة `src/mcp/` إلى وصف LogCater | 📝 مطلوب |
| [12 §2 و§6](12-REUSE-AND-IMPLEMENTATION-PLAN.md) | إغلاق صفوف Host/UI/ADB بإحالة ADR-0001 + إضافة logcat-viewer كمرشح Core | 📝 مطلوب |
| [09 §5.3](09-device-file-management.md) | تسجيل GPL-3.0 كحاجز اقتباس | 📝 مطلوب |
| [05](05-RESEARCH-COVERAGE-MATRIX.md) | إضافة صفوف العقود/Threat Model/Corpus، وتحديث حالة Source Revalidation إلى "مكتمل لجولتين" | 📝 مطلوب |
| [00 §9.2](00-PRODUCT-BASELINE.md) | تحويل "معمارية Windows" و"أدوات التشغيل" إلى محسوم | 📝 مطلوب |
| [14](14-source-revalidation-app-installers.md) | إحالة إلى النتائج الجديدة عن APK-Installer metadata | 📝 مطلوب |

## 6. ما لم يتم إثباته في هذه الجولة

- لم يُشغَّل `dotnet build` ولا `dotnet test` على logcat-viewer؛ الأرقام المذكورة (852/94.26%) **معلنة في مستودعهم**، لم تُتحقق عندنا.
- لم يُتحقق أن `AdvancedSharpAdbClient 3.6.16` متاحة على NuGet (شبكة NuGet غير متاحة في بيئة المراجعة) — **يجب التحقق عند أول restore**.
- لم تُقرأ تراخيص LogCater وBugreport_analyzer من ملف LICENSE (لم يُعثر عليه في الشجرة المفحوصة) — **يجب حسمها قبل أي اقتباس**.
- لم تُراجَع كل ملفات أي مشروع؛ المراجعة مركّزة على الطبقات ذات الصلة.
- لا دليل Runtime أو Hardware لأي منها.

**الحالة:** Source Revalidation مكتمل لجولتين على 10 مشاريع. القرارات التنفيذية في [17](17-adr-0001-stack-decision.md) و[18](18-ENGINEERING-DECISIONS.md) جاهزة، والعقود في [19](19-SHARED-CONTRACTS.md) قابلة للفحص. **لا كود منتج مكتوب بعد.**
