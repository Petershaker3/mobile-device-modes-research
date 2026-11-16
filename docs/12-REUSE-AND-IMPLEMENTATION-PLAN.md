# خطة إعادة الاستخدام والمعمارية والتنفيذ المقترح

> **الغرض:** تحويل نتائج البحث في الوثائق 00–11 إلى خطة تقنية قابلة للتنفيذ، تحدد المكونات المرشحة لإعادة الاستخدام، والعقود المشتركة، وترتيب بناء المحاور، وقرارات التقنية التي ما زالت تحتاج PoC. هذه الوثيقة ليست موافقة تنفيذ، ولا تعني اعتماد أي مشروع أو كود للدمج.

**تاريخ تثبيت مراجع المصدر:** 12 نوفمبر 2026  
**المنصة المستهدفة:** Windows 10/11، وAndroid 7.0 / API 24 فأحدث.  
**حالة المستودع، 14 نوفمبر 2026:** أضيف تطبيق WPF/Core أولي يعيد استخدام AdvancedSharpAdbClient ومكوّنات Logcat الموثقة في 21. نُقلت قراءات معلومات الجهاز وجرد التطبيقات ونسخة التطبيق إلى `ReadOnlyDeviceService` داخل Core، وتفوض إليها واجهة WPF. provider WPD في `src/DeviceToolkit.Wpd/` يعيد استخدام MediaDevices 2.0.0 داخل worker process مع مهلات. اختُبر على هاتف فعلي: 58 مجلدًا في التخزين الداخلي، و25 مجلدًا في Download، وتنزيل ملف 8,602 بايت مع مطابقة SHA-256 والتنظيف. اختبار المهلة المنفصل لم يصل إلى تعداد الملفات لأن فتح جلسة WPD انتهى بمهلة حتى عند 120 ثانية؛ لا يُعد هذا الاختبار ناجحًا. فحص الواجهة المرئي وMCP وBackup والـfirmware غير مكتمل. هذا منتج جزئي، لا اعتماد شامل لكل المحاور.

**آخر تحقق WPD USB، 14 نوفمبر 2026:** عند إعادة الاختبار بعد وضع الملفات في الهاتف، ظهر جهاز WPD واحد ونجحت جلسة القراءة فقط. وجد الجرد 74 ملفًا في `Download`؛ وأكمل تعداد `DCIM` و`Pictures` و`Download` و`Documents` و`Movies` و`Music` ضمن مهلة العملية البالغة 8 ثوانٍ، فلم تحدث مهلة فعلية. انتهت الجلسة ولم تبق عمليات worker. اختبار timeout/cancellation/disposal ما زال مثبتًا بالعامل الوهمي فقط، وليس على هاتف حقيقي.

**متابعة نقل WPD على الهاتف، 14 نوفمبر 2026:** ظهر 65 اسماً يحوي Unicode من أصل 74 في `Download`؛ أكبر ملف معروف 67,588,123 بايت. نُزّل ملف Unicode إلى مسار محلي مؤقت، وطابق الحجم 9,971,441 بايت ثم حُذف. أُلغي تنزيل الملف الأكبر بعد 250 ms؛ توقف worker، ورفضت الجلسة طلباً تالياً، ولم تبق ملفات مؤقتة. نجح فتح جلسة جديدة وتعداد 74 ملفاً. لم يُختبر فصل كابل USB أثناء النقل أو reconnect تلقائي.

**تقدم Source Revalidation:** اكتمل على **جولتين**. الجولة الأولى: AdvancedSharpAdbClient وADB-Explorer وAdbGui في [13](13-source-revalidation-adb-file-core.md)، bundletool وapkfile وPacMan-APK وxapkInstaller في [14](14-source-revalidation-app-installers.md)، adb-mcp وus-all/android-mcp-server في [15](15-source-revalidation-mcp-codex.md)، payload-dumper-go وlucky-arch وlpunpack_cyth وchen3feng/android-backup في [16](16-source-revalidation-firmware-storage-backup.md). **الجولة الثانية** في [20](20-source-revalidation-round2.md) بعد قرار الستاك: logcat-viewer، APK-Installer، AdbFileManager، WinDroidFS، LogCater، Bugreport_analyzer — وأنتجت أربعة تصحيحات لوثائق قائمة.

> **⚠️ تحديث 13 نوفمبر 2026 — قرار الستاك محسوم:** [ADR-0001](17-adr-0001-stack-decision.md) يثبت **.NET 10 + WPF** و**AdvancedSharpAdbClient** و**CsWin32** و**SQLite** و**.NET MCP SDK**. لذلك صفوف `Host Core` و`GUI` و`ADB` في §6 **أُغلقت**، ولم يبق PoC "مقارنة لغتين". ما يبقى من §6 هو قرارات MTP وPersistence وMCP وDependency delivery، وقرارات التنفيذ التفصيلية في [18](18-ENGINEERING-DECISIONS.md). العقود في §4.2 أصبحت مواصفة منفذة في [19](19-SHARED-CONTRACTS.md) — ارجع إليها بدلاً من النثر هنا.

## 1. القرار التنفيذي

المشروع ليس APK installer ولا ADB GUI موسعاً. هو منصة Windows لإدارة أجهزة Android وفهم بياناتها وأوضاعها وسوفتويرها وتطبيقاتها وملفاتها وتشخيصها وشبكتها، مع واجهة MCP محلية للذكاء الاصطناعي.

**الاتجاه المعماري الموصى به:**
- Application Service Layer واحدة هي بوابة كل الوظائف.
- Device/Transport Adapters مستقلة وقابلة للاستبدال.
- Format/Source Providers مستقلة عن محرك التثبيت.
- Operation Engine مشترك يدير كل عملية طويلة أو مؤثرة.
- GUI وMCP عميلان لنفس الخدمات؛ لا يتصل أي منهما مباشرة بـADB shell أو parsers.
- Windows APIs منفصلة لاكتشاف USB/PnP وMTP/WPD وتشخيص التعريفات.
- أدوات خارجية موثقة ومثبتة الإصدار تُشغّل عبر wrappers آمنة عند الحاجة، ولا تُدمج نسخ كاملة من برامج أخرى دون ضرورة.

**الاختيار محسوم: .NET 10 Host Core وWPF** وفق [ADR-0001](17-adr-0001-stack-decision.md). أُغلقت مقارنة Tauri 2/Rust، وأي عمل جديد يختبر تنفيذ الستاك المعتمد بدل إعادة فتح المقارنة. تبقى قرارات تكامل MTP وSQLite وMCP وتوزيع الاعتماديات محكومة بالتفاصيل في [18](18-ENGINEERING-DECISIONS.md).

## 2. ما الذي نعيد استخدامه؟

إعادة الاستخدام ليست قراراً واحداً؛ نختار أصغر مستوى مفيد: CLI/SDK، مكتبة، parser، adapter، أو نمط UI. لا ننسخ تطبيقاً كاملاً لمجرد أن واجهته قريبة.

| المحور | المرشح الأساسي | ما يمكن الاستفادة منه | ما لا نأخذه كما هو | الحالة الحالية |
|---|---|---|---|---|
| ADB protocol/client | [AdvancedSharpAdbClient](https://github.com/SharpAdb/AdvancedSharpAdbClient) | DeviceData/Serial/TransportId، SyncService، PackageManager، install-multiple، progress/cancellation | raw shell argument construction، basename collisions، cleanup/session abandon وpost-verify gaps | Source revalidated عند SHA محدد؛ مرشح Host Core مع فجوات مسجلة في [13](13-source-revalidation-adb-file-core.md) |
| إدارة الملفات | [ADB-Explorer](https://github.com/Alex4SSB/ADB-Explorer) | File listing، transfer queue، SyncPipe bounded streaming/backpressure، progress throttling | WPF/fork محلي من AdvancedSharpAdbClient؛ Hardware tests تحتاج جهازين ومسار test مخصص | Source revalidated عند SHA محدد؛ مرشح استخراج patterns فقط وفق [13](13-source-revalidation-adb-file-core.md) |
| واجهة/معلومات ADB وLogcat | [AdbGui](https://github.com/RookieJay/AdbGui) | Device discovery، read info، Logcat ring buffer/filter/controller، parser fixtures | Kotlin/Compose؛ generic shell موجود؛ clear/fix-logcat لها آثار على الجهاز | Source revalidated عند SHA محدد؛ parser/fixtures/design فقط وفق [13](13-source-revalidation-adb-file-core.md) |
| تثبيت APKS القياسي | [Google bundletool](https://github.com/google/bundletool) | DeviceSpec، TOC/APKS selection، install-apks | SAI APKS صيغة مختلفة؛ target serial إلزامي؛ Java process adapter يحتاج timeout/cancel | Source revalidated في [14](14-source-revalidation-app-installers.md)؛ المرجع القياسي لـtoc.pb |
| APK metadata وAPKM/XAPK/APKS | [apkfile](https://github.com/david-lev/apkfile) أولاً؛ PacMan-APK وxapkInstaller للمقارنة | metadata، signing information، split selection، OBB وformat edge cases | archive hardening، cryptographic signature verification، remote install lifecycle، Python runtime decision | Source revalidated في [14](14-source-revalidation-app-installers.md)؛ مرشح parser أولي وليس installer معتمداً |
| OTA payload | [payload-dumper-go](https://github.com/ssut/payload-dumper-go) | Header/manifest/partition metadata وhash، list-only، Go API وWindows builds | manifest resource caps وOTA signature trust وunsupported delta ops | Source revalidated في [16](16-source-revalidation-firmware-storage-backup.md)؛ مرشح metadata أول |
| Dynamic partitions | [AOSP partition_tools/liblp](https://android.googlesource.com/platform/system/extras/+/master/partition_tools/)، [lucky-arch](https://github.com/soe1hom-arch/lucky-arch) | AOSP correctness oracle؛ lucky-arch list-only sparse ReaderAt | lucky-arch slot0 فقط، لا tests، header checksum/block mapping/resource caps gaps | Source revalidated في [16](16-source-revalidation-firmware-storage-backup.md)؛ AOSP المرجع وlucky-arch PoC فقط |
| Logcat viewer | [logcat-viewer](https://github.com/daitouniao/logcat-viewer)، AdbGui | parsing/viewer/indexing/performance | لا اعتماد تلقائي على UI أو auto-device selection | مرجعان لاستخراج أجزاء محددة |
| Bugreport/Perfetto | [Bugreport_analyzer](https://github.com/VenkateshJallu/Bugreport_analyzer)، [Perfetto](https://github.com/google/perfetto) | parser/analysis references وأداة متخصصة | لا تحميل تقارير الهاتف إلى خدمات خارجية، ولا parser دون ZIP limits/privacy | مرشحان للتجارب المعزولة |
| Backup files | [chen3feng/android-backup](https://github.com/chen3feng/android-backup) | scan/sync، include/exclude، hardlink snapshots، Windows Unicode workaround | auto-all-devices، size/mtime-only، exclude-after-pull، لا restore/tests واضحة | Source revalidated في [16](16-source-revalidation-firmware-storage-backup.md)؛ patterns فقط |
| Legacy .ab | [Android Backup Extractor](https://github.com/nelenkov/android-backup-extractor) | import/unpack legacy .ab | لا ينشئ backup حديثاً ولا يتجاوز Android 12 restrictions | ميزة استيراد اختيارية P2 |
| MCP/ADB | [iksnerd/adb-mcp](https://github.com/iksnerd/adb-mcp) و[us-all/android-mcp-server](https://github.com/us-all/android-mcp-server) | typed ADB client، stdio، tool schemas، context lifecycle، validators، categories | annotations ليست approval؛ auto-single-device مرفوض؛ generic shell/write flags غير معتمدة | Source revalidated في [15](15-source-revalidation-mcp-codex.md)؛ Read-only V1 فقط |
| Windows PnP/Drivers | Windows PnP/SetupAPI/Configuration Manager وPnPUtil، ومصادر Microsoft/Google/OEM | APIs الرسمية للتشخيص والمطابقة | لا تثبيت/إزالة driver تلقائياً دون exact Hardware ID وpreview/approval/rollback | خدمة Core read-only عبر CsWin32؛ smoke طابق 135 IDs مع PnPUtil /connected. يوجد اختلاف Problem Code بين CM/WMI وPnPUtil؛ لا إصلاح تلقائي |
| مصادر التطبيقات | F-Droid signed indexes؛ APKPure/APKMirror provider pages | تكامل فهرس موثق لـF-Droid، وProvider Adapter للمصادر الأخرى | لا تجاوز CAPTCHA/Cloudflare أو APIs غير موثقة أو مشاركة cookies | F-Droid أولاً؛ APKPure/APKMirror تدفق آمن محدود |

**مبدأ الدمج:** كل مرشح له مصدر أصلي واحد داخل 02 أو وثيقة المجال. هذا الجدول لا يستبدل مراجعة الكود ولا يعيد نشر كل تفاصيلها.

## 3. تثبيت مراجع المصدر (Source Snapshot)

التقطنا HEAD لكل مستودع في 12 نوفمبر 2026 لتكون إعادة المراجعة قابلة للتكرار. هذه الـSHAs تثبت نسخة المصدر التي ينبغي أن يبدأ منها التدقيق القادم؛ ولا تعني أن كل SHA تمت مراجعته سطراً بسطر أو اختباره runtime. إذا اختلف SHA عن commit الموثق داخل 02، نعيد مراجعة الفرق قبل أي اقتباس.

| المشروع | الفرع | HEAD SHA | ملاحظة |
|---|---|---|---|
| [AdvancedSharpAdbClient](https://github.com/SharpAdb/AdvancedSharpAdbClient/commit/cd46a145ac4a9f05ec3b21a7d2b10882411d93d7) | main | cd46a145ac4a9f05ec3b21a7d2b10882411d93d7 | نفس commit الذي تمت مراجعته في 02 §20.54 |
| [ADB-Explorer](https://github.com/Alex4SSB/ADB-Explorer/commit/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56) | master | ce9331b4a3fa3f76b8d43c47d4a514d0099adb56 | HEAD snapshot؛ يلزم تثبيت commit المراجعة قبل الاقتباس |
| [AdbGui](https://github.com/RookieJay/AdbGui/commit/4ca23e550b8040041bfeec474b1ac8ba65660c92) | master | 4ca23e550b8040041bfeec474b1ac8ba65660c92 | مرجع Host/Logcat؛ أعد فحص source-vs-README |
| [AdbFileManager](https://github.com/wkbin/AdbFileManager/commit/852c0bb83d8febce12a87d39148e57fa54b4e1c3) | main | 852c0bb83d8febce12a87d39148e57fa54b4e1c3 | مرجع dual-pane/Unicode؛ لا تنقل عمليات الحذف دون إصلاح |
| [WinDroidFS](https://github.com/Wilsoon7721/WinDroidFS/commit/7deb96eb7aeff6b095f4aeabebdab4a9af68dc63) | main | 7deb96eb7aeff6b095f4aeabebdab4a9af68dc63 | Virtual drive اختياري فقط |
| [bundletool](https://github.com/google/bundletool/commit/586a43a450712a1067f3d92cf7574dee68226302) | master | 586a43a450712a1067f3d92cf7574dee68226302 | نفس commit المشار إليه في مراجعة 02 §20.30 |
| [xapkInstaller](https://github.com/adhu2018/xapkInstaller/commit/a88e05386a0fc9751ef7637ad437659bfdd60a43) | main | a88e05386a0fc9751ef7637ad437659bfdd60a43 | HEAD snapshot؛ مراجعة كل التغييرات قبل الاعتماد |
| [pacman-apk](https://github.com/nuloperrito/pacman-apk/commit/87af1d04541297fdf272813567928a2a6de6e756) | main | 87af1d04541297fdf272813567928a2a6de6e756 | HEAD snapshot؛ مرشح مقارنة |
| [Android Application Installer](https://github.com/havokentity/android-application-installer/commit/11acb70c5a8886a1f0e985217a635321d681bc49) | main | 11acb70c5a8886a1f0e985217a635321d681bc49 | مرجع Tauri 2/Rust/ADB |
| [apkfile](https://github.com/david-lev/apkfile/commit/4b910abae705106cde80a44d607917d038f0c36e) | main | 4b910abae705106cde80a44d607917d038f0c36e | مرشح parser؛ review للـZIP/security مطلوب |
| [payload-dumper-go](https://github.com/ssut/payload-dumper-go/commit/c3a50ba8d784764c06143c1f82971cd81a77f9d6) | main | c3a50ba8d784764c06143c1f82971cd81a77f9d6 | HEAD snapshot |
| [lucky-arch](https://github.com/soe1hom-arch/lucky-arch/commit/09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b) | main | 09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b | sparse/list parser؛ لا tests واضحة في المراجعة |
| [lpunpack_cyth](https://github.com/ahsanihlwn/lpunpack_cyth/commit/7ace45e6166d5b3b243c085a92e931d6a2809663) | main | 7ace45e6166d5b3b243c085a92e931d6a2809663 | Windows executable معلن؛ slot/parser gaps |
| [logcat-viewer](https://github.com/daitouniao/logcat-viewer/commit/a985616224a342d6064ed962eec28ace3f38365a) | master | a985616224a342d6064ed962eec28ace3f38365a | HEAD snapshot |
| [LogCater](https://github.com/kefengwei/LogCater/commit/916cae7fd277f645c933723c2979d7415505604d) | master | 916cae7fd277f645c933723c2979d7415505604d | مراجعة أمان MCP/Shell قبل أي اقتباس |
| [Bugreport_analyzer](https://github.com/VenkateshJallu/Bugreport_analyzer/commit/8fa9e26d51eee5fa1aeab098f796a6d709c7b7e9) | main | 8fa9e26d51eee5fa1aeab098f796a6d709c7b7e9 | HEAD snapshot |
| [chen3feng/android-backup](https://github.com/chen3feng/android-backup/commit/dca2c38c60cc17237fa4aaff0cac06475d02ee66) | master | dca2c38c60cc17237fa4aaff0cac06475d02ee66 | incremental shared files؛ لا app-private data |
| [open-android-backup](https://github.com/mrrfv/open-android-backup/commit/7382ad7703b62e02a7855fbffba5b99efa84a966) | master | 7382ad7703b62e02a7855fbffba5b99efa84a966 | WSL2 workflow؛ ليس Windows-native |
| [Android Backup Extractor](https://github.com/nelenkov/android-backup-extractor/commit/38fd634a50a1c5d8972a4eca83f5e5d47bfa8411) | master | 38fd634a50a1c5d8972a4eca83f5e5d47bfa8411 | legacy .ab import فقط |
| [android-mcp-server](https://github.com/us-all/android-mcp-server/commit/bacd6eac34273e7bbaef666954ee399bdf1de1e3) | main | bacd6eac34273e7bbaef666954ee399bdf1de1e3 | مرجع أدوات؛ راجع Shell gates |
| [adb-mcp](https://github.com/iksnerd/adb-mcp/commit/6b12c977ee576091fe34553c0dcf9cfa2258d377) | main | 6b12c977ee576091fe34553c0dcf9cfa2258d377 | HEAD snapshot؛ لا اعتماد أمني تلقائي |
| [APK-Installer](https://github.com/Paving-Base/APK-Installer/commit/c8f4e634b72c5555cffe9cc5759074a866b68ae1) | main | c8f4e634b72c5555cffe9cc5759074a866b68ae1 | نفس commit مراجعة 02 §20.53؛ مرجع UI/ADB جزئي |

**ملاحظة:** إذا لم يثبت أن HEAD snapshot هو نفس revision الذي قرأه تدقيق سابق، لا نكتب "Source reviewed at HEAD". نسجل "HEAD captured; review pending" حتى يعاد فحص التغييرات. المراجع الرسمية مثل Android Platform Tools وAOSP partition_tools تُثبت بإصدارات/tag/commit عند تحويلها إلى dependency فعلية.

## 3.1 تقدم Source Revalidation

| المحور | مراجعة مكتملة | ما تبقى |
|---|---|---|
| ADB/File/Logcat | [13](13-source-revalidation-adb-file-core.md) عند SHAs محددة | PoC .NET/Tauri، build/tests، USB/Wireless/Hardware |
| APK/APKM/XAPK/APKS | [14](14-source-revalidation-app-installers.md) عند SHAs محددة | archive corpus، apksigner verification، PoC Python sidecar مقابل adapters، install session failure tests |
| Drivers | مصادر Windows الرسمية وتصميم 04، وخدمة Core read-only وsmoke على المضيف | تشغيل/فحص التبويب بصرياً، تكرار الاختبار مع هاتف USB؛ لا يوجد repair scope في V1 |

## 3.2 تقدم Source Revalidation — OTA/Partitions/Backup

تمت مراجعة payload-dumper-go وlucky-arch وlpunpack_cyth وchen3feng/android-backup عند SHAs المثبتة في [16](16-source-revalidation-firmware-storage-backup.md). هذه المراجعة لا تغني عن corpus/PoC ولا تثبت runtime.

## 4. المعمارية المستهدفة

### 4.1 الطبقات

~~~text
Desktop UI                         MCP Server (local STDIO)
    |                                      |
    +-------------- Application API -------+
                       |
              Application Services
                       |
       +---------------+-----------------------------+
       |               |              |              |
   Device Service   App Service   File Service   Diagnostics
       |               |              |              |
   Transport Adapters  Format/Source  ADB/MTP/WPD   Logcat/Bugreport
       |               |              |              |
       +---------------+--------------+--------------+
                       |
             Shared Operation Engine
                       |
       Targeting / Capabilities / Approval / Audit
                       |
       Windows PnP/WPD APIs + Android ADB/Fastboot
                       |
                Local State & Evidence
~~~

### 4.2 عقود أساسية لا تتغير بين المحاور

**DeviceHandle**
- device_id داخلي غير حساس.
- transport_kind مثل ADB_USB / ADB_WIRELESS / FASTBOOT / MTP.
- serial أو transport identity عند الحاجة، مع redaction في logs.
- connection_state وlast_seen.
- capabilities[] مع source/evidence/version، لا مجرد boolean عام.
- android_api وmodel وbuild_fingerprint حقول اختيارية لا تستخدم كهوية وحيدة.

**Operation**
- operation_id وoperation_type وtarget_device_id وcreated_at.
- state: Pending / Preview / AwaitingApproval / Running / Succeeded / Failed / Cancelled / OutcomeUnknown / NeedsReconciliation.
- progress_phase وprogress_current وprogress_total.
- deadline وcancellation_requested وresult وwarnings[].
- audit_event_ids[] وartifacts[].
- لا retry آلياً بعد نتيجة OutcomeUnknown قبل reconciliation.

**Capability**
- capability_id وsupported_transport وandroid_api_range وdevice_scope.
- availability: Available / Unsupported / PermissionDenied / Unknown.
- evidence_source وobserved_at وconfidence.
- لا نعمم قدرة على كل أجهزة الشركة من جهاز واحد.

**Result**
- نتيجة كل خطوة مستقلة؛ لا تختزل النجاح في exit code.
- حالات Partial وUnsupported وPermissionDenied وTimeout وDeviceDisconnected وParseError.
- التفريق بين command success وstate verified.

### 4.3 Device targeting

- لا تستخدم FirstOrDefault أو أول جهاز ظاهر.
- كل عملية تقبل DeviceHandle صريحاً أو ترجع DeviceSelectionRequired.
- إذا تغير transport/serial أو انقطع الجهاز، لا تنقل العملية تلقائياً إلى جهاز آخر.
- MCP لا يسمح بتحديد الجهاز بالاسم التسويقي فقط.
- العمليات الجماعية تحتاج قائمة أهداف صريحة ومعاينة لكل جهاز.

## 5. Operation Engine والسياسات المشتركة

كل وظيفة تمر عبر نفس lifecycle:

1. Discover & Select Target.
2. Build Plan من typed inputs.
3. Validate Target Capabilities.
4. Preview الآثار والملفات والأوامر.
5. Approval حسب مستوى التأثير.
6. Execute بمهلات وحدود وCancellation.
7. Verify state من مصدر مستقل إن أمكن.
8. Reconcile بعد timeout/disconnect.
9. Cleanup للموارد التي أنشأتها العملية فقط.
10. Audit وإخراج تقرير قابل للمراجعة.

### مستويات العمليات

| المستوى | أمثلة | سياسة الموافقة |
|---|---|---|
| Read-only | getprop، جرد التطبيقات، list/stat files، Logcat capture، metadata inspection | يمكن السماح به عبر أدوات MCP محددة بعد اختيار الجهاز؛ بعض البيانات الحساسة تحتاج تنقيحاً |
| Local write | تنزيل ملف إلى PC، استخراج محلي، إنشاء backup archive | موافقة على الوجهة والحجم/المساحة وسياسة الاحتفاظ |
| Device write | install/uninstall، push/rename/delete، DNS change، disable/enable app | Preview + موافقة صريحة على الجهاز والهدف والآثار |
| Windows system change | driver install/repair/remove، إعدادات PnP | Preview + elevation عند الحاجة + مصدر رسمي + rollback plan |
| Destructive/unsupported | flash/erase/format/bootloader unlock أو تجاوز حماية | غير متاح في هذا النطاق |

لا توجد generic shell tool مكشوفة للـGUI أو MCP. أي command adapter يبني أوامر من allowlist ومعاملات typed، وليس من نص حر.

## 6. قرارات تقنية تحتاج PoC قبل اختيار Stack

| القرار | المرشح A | المرشح B | ما الذي يجب قياسه؟ |
|---|---|---|---|
| ~~Host Core~~ | **محسوم: .NET 10** | — | [ADR-0001](17-adr-0001-stack-decision.md) |
| ~~GUI~~ | **محسوم: WPF** | — | [ADR-0001](17-adr-0001-stack-decision.md) |
| ~~ADB~~ | **محسوم: AdvancedSharpAdbClient 3.6.16+** (مع `adb.exe` كمسار احتياطي) | — | [ADR-0001](17-adr-0001-stack-decision.md) |
| MTP | Windows WPD COM/API wrapper | ADB-only first ثم WPD provider | enumeration، Unicode، large files، Android API/OEM differences |
| Local persistence | SQLite | JSON/flat files | transactions، schema migration، audit retention، corruption recovery |
| MCP | .NET MCP SDK الرسمي عبر STDIO | MCP host مستقل فوق خدمات Core المشتركة | device_id محلي، ADB runtime lifecycle، Codex handshake، tool schemas، process lifecycle |
| Dependency delivery | bundled pinned tools | managed install/update of signed official tools | offline-first، integrity/hash، rollback، Windows Defender/SmartScreen behavior |

### PoC المطلوب قبل بدء تنفيذ النواة (الستاك محسوم)

- Windows 10 وWindows 11 على x64 كبداية اختبار؛ ARM64 قرار مستقل لاحقاً.
- اكتشاف جهازين ADB USB مع اختيار serial صريح.
- Wireless pairing/connect/disconnect/reconnect.
- قراءة getprop/df/package list مع timeout وcancellation.
- push/pull streaming لملفات صغيرة وكبيرة وأسماء Unicode.
- Device discovery من Windows PnP، ثم تجربة MTP/WPD منفصلة.
- محاكاة device disconnect أثناء upload/install session.
- MCP STDIO tool read-only يقرأ device info من نفس Application Service، لا من مسار موازٍ.
- قياس cold start، idle memory، package size، وoffline behavior.
- ~~نفس الاختبارات على تنفيذ .NET وTauri/Rust قبل قرار نهائي~~ — **أُلغي:** الستاك محسوم في [ADR-0001](17-adr-0001-stack-decision.md)؛ PoC الآن يقيس تنفيذ النواة نفسها لا يقارن لغتين.

## 7. خطة ترتيب البناء بعد موافقة تنفيذ مستقلة

| المرحلة | الناتج | شروط الخروج | الحالة الحالية، 14 نوفمبر 2026 |
|---|---|---|---|
| 0 — Source Revalidation | تثبيت commits النهائية، فروق المصدر، الاعتماديات، build instructions، security review | لا توجد فجوة حرجة مجهولة في المرشح المختار | **مكتملة للمرشحين المسجلين** في الجولتين؛ إعادة التحقق مطلوبة عند اختيار مكوّن جديد |
| 1 — Host Core PoC | Device Registry، explicit DeviceHandle، ADB discovery، typed command execution | USB/Wireless/multi-device/cancel/disconnect tests | **جزئية**: WPF وCore وADB discovery والاتصال اللاسلكي موجودة؛ `device_id` محلي وDevice Registry موحد واختبارات فصل/تعدد الأجهزة غير مكتملة |
| 2 — Shared Contracts | Device/Capability/Operation/Result/Audit schemas | عقود موحدة ومختبرة ومصدّرة لـGUI وMCP | **جزئية**: 7 schemas ومدقّق صارم (48 فحصاً و13 حالة سلبية)، مع `OperationPlan` و`ApprovalService` و`AuditStore` على SQLite؛ يغطي الآن تدفقات الكتابة الأربع وقراءات facts/apps/APK/Logcat/file listing وADB/mDNS discovery وWindows PnP والاقتران؛ بقية الخدمات وأنواع العقود و`OperationEngine` ما زالت غير مكتملة |
| 3: Read-only Device Intelligence | System info، mode detection، software inventory، storage/partition metadata | corpus fixtures وAPI 24+ tests وsource provenance | **جزئية**: قراءة ADB وFastboot العامة صارت allowlisted. أضيف profile Huawei تجريبي لاستعلامي getvar وقراءة موديل OEM ثابتة، مع بقاء توافقه مع موديل محدد غير مثبت، ولا تزال تجربة Fastboot على Huawei وفحص الواجهة بصرياً معلقين. اكتشاف الأوضاع وبيانات الأقسام الشاملة غير مكتملة. راجع [24](24-DEVICE-MODE-READ-INFO.md) و[25](25-FASTBOOT-SOURCE-REVALIDATION.md). |
| 4 — Application Management & Installer | app inventory، format adapters، package inspection، APK/APKS ثم APKM/XAPK | format corpus، signature/split checks، device install verification | **جزئية**: جرد ومعاينة APK وتثبيت/إزالة تطبيق مستخدم مع تحقق بعدي وموافقة مرتبطة بالخطة؛ لم يُعَد اختبار الهاتف بعد ربط الموافقة، وصيغ الحزم المتعددة والتوقيع ناقصة |
| 5 — File & Backup | ADB file manager، MTP provider، file backup/restore | path safety، hash verification، cancellation/recovery | **جزئية**: ADB files قائم، وأضيف تبويب WPF `MTP storage` للتصفح read-only وتنزيل محلي بمعاينة وموافقة وتدقيق. على الهاتف عبر USB نجح تعداد 74 ملفًا، ومسار Unicode، تنزيل 9,971,441 بايت مع تحقق الحجم، وإلغاء تنزيل أكبر ملف (67,588,123 بايت) مع إنهاء العامل ورفض الجلسة طلبات جديدة؛ نجحت جلسة جديدة بعدها. fake-worker يغطي timeout/cancellation/disposal؛ فشل فصل USB الفعلي وإعادة الاتصال تلقائياً وفحص الواجهة المرئي لم يُختبرا. النسخ الاحتياطي/الاستعادة غير منفذين |
| 6 — Diagnostics | Logcat، Bugreport، dumpsys، Perfetto integration | privacy/ZIP limits، streaming/retention tests | **جزئية**: تحليل والتقاط Logcat بحدود دقيقتين و64 MiB؛ الكاتب الجديد يضمن flush واجتاز اختبارات الوحدة. اختبار الهاتف الأخير على SM-N976N أكمل 30 ثانية وقرأ 1,299 سطراً دون crash events/groups؛ Bugreport وPerfetto غير منفذين |
| 7 — Network & Drivers | DNS preview/apply/verify/restore؛ Windows driver diagnosis/repair | جهاز/Windows test matrix، rollback، official source verification | **جزئية**: قراءة Private DNS وتشخيص PnP فقط؛ تغيير DNS وإصلاح التعريفات خارج V1 الحالي |
| 8 — MCP | tools/resources فوق Application Services | Codex integration، explicit target، approval، injection/security tests | **لم تبدأ**: لا يوجد MCP host؛ العقود ومراجعة المرشحين فقط |
| 9 — Hardening | Offline packaging، migrations، update/rollback، performance، accessibility | CI + runtime matrix + documented release gates | **جزئية**: CI strict وlockfiles مقفلة واختبارات .NET؛ لا تغليف إصدار أو مصفوفة runtime أو فحص واجهة شامل |

**المراحل متداخلة عند وجود اعتماديات واضحة، لكن لا يتم بناء كل محور كبرنامج مستقل.** لا نؤجل المحاور التي طلبها المستخدم لمجرد أن APK installer أسهل؛ نربطها بالنواة المشتركة.

## 8. Test Corpus وEvidence Plan

يجب بناء corpus لا يحتوي بيانات شخصية:
- getprop وdf وmount وpm list packages وdumpsys outputs من fixtures مصطنعة أو من أجهزة اختبار مصرح بها بعد تنقيحها.
- APK/APKS/APKM/XAPK عينات صالحة وتالفة، splits ناقصة، signatures مختلفة، ZIP bombs/Zip Slip fixtures آمنة.
- OTA metadata وpayload headers وsuper.img raw/sparse fixtures؛ لا نحتاج صور أقسام شخصية.
- Logcat/Bugreport ZIP fixtures من نسخ Android متعددة، مع redaction.
- MTP/WPD fixture أو جهاز اختبار منفصل، وملفات Unicode/large/many-files.
- Operation failure fixtures: timeout، disconnect، cancellation، partial write، unknown outcome.

كل دليل يحمل: source URL، source SHA، review date، tool version، OS build، Android API/model عند hardware test، خطوات قابلة للتكرار، output redacted، evidence grade.

## 9. إصلاحات مراجعة الحالة الحالية — 14 نوفمبر 2026

تُنفّذ هذه الإصلاحات على الكود الحالي قبل توسيع خدمات Android، مع فصل النتائج المؤكدة من التقرير عن البنود التي تحتاج إعادة تحقق. معيار القبول هو السلوك والاختبارات، لا وجود التوثيق أو مخطط JSON وحده.

| الأولوية | الإصلاح | الحالة والدليل المطلوب |
|---|---|---|
| P0 | منع أي فحص `su` تلقائي أو ضمني أثناء تعداد الأجهزة؛ الحالة غير المفحوصة لا تُعرض كأنها نفي مؤكد لـroot. إبقاء اختيار الجهاز صريحًا وعرض الحالات غير المتصلة بدل إسقاطها. | مطبق في `AdbManager.ListDevicesAsync`؛ الاختبار يثبت أن التعداد لا يرسل أوامر shell ويحتفظ بحالات الأجهزة. |
| P0 | جعل بوابة Linux تفشل عند نتائج اختبار مفقودة أو غير قابلة للتحليل أو فشل غير مسموح؛ السماح يطابق فقط أسماء الفروق المعروفة، والمدخلات المفقودة لا تُعامل نجاحًا. | مطبق: Linux يستدعي التقرير بـ`--strict`، والتقرير يرفض النتائج المفقودة أو غير القابلة للتحليل والفشل غير المعروف؛ فحوص الحالات السلبية مضافة. |
| P1 | اعتبار exit code غير الصفري من `adb shell` فشلًا صريحًا مع حفظ رمز الخروج للجهة المستدعية. | مطبق في `AdbShell.RunAsync`؛ اختبارات exit code 0 وغير الصفري والمهلة والإلغاء. |
| P1 | حماية إلغاء Logcat من فقد آخر دفعة، والحفاظ على حدود جلسة/حجم، وعدم تسجيل serial الجهاز. | مسار WPF الفعلي يفرض حتى دقيقتين و64 MiB باستخدام `-T 1`؛ فصلت الكتابة المحدودة إلى Core وأضيف flush مضمون واختبارات إلغاء/حجم/بانر. `LogcatStream` المنقول لا يستعمله المسار الحالي، لكنه ما زال يسجل serial ولا يملك حد جلسة/حجم أو اختباراً؛ يبقى مستبعداً من مسار المنتج حتى تكتمل حماية المصدر وتحديث attribution/gate. |
| P1 | ربط operation schema بسياسة تنفيذ typed تفرض موافقة مناسبة للعملية والتحقق بعد الكتابة، بدل الثقة في قيم يرسلها المستدعي. | المخطط يربط نوع العملية بأثر ثابت، ويطلب موافقة LocalWrite، ويتطلب تحققاً مستقلاً للنجاح بعد الكتابة؛ المدقّق الصارم يمر على 48 فحصاً و13 حالة سلبية. أضيف `OperationImpactPolicy` و`OperationPlan` و`ApprovalService`؛ وُصلت الموافقة بخطط تثبيت/إزالة التطبيقات ورفع/تنزيل الملفات، مع هدف صريح ومدخلات ثابتة وخطوة تحقق ورمز أحادي الاستخدام لخمس دقائق. أضيف `AuditStore`، وسُجل `ApprovalGranted` ثم `OperationStarted` والنجاح/الفشل/الإلغاء للتدفقات الأربع؛ فشل تسجيل البداية يمنع التنفيذ، وفشل تسجيل النتيجة يوضح أن الحالة تحتاج مراجعة. بقية الخدمات و`OperationEngine` غير مربوطة بعد، ولا يوجد تحقق ميداني لهذه التدفقات في هذه المتابعة. |
| P2 | تسوية تعارضات الحالة بين README والخطة والـhandoff، والتحقق من أرقام الترجمة وقرار lockfile. | README حُدث إلى 46/46 مفتاحاً و545/545 في آخر تشغيل محلي كامل. أُنشئت lockfiles للمشاريع الخمسة ونجح `dotnet restore --locked-mode`، وأصبح CI يستخدم الوضع المقفل. ملخص handoff والخطوة التالية حُدّثا؛ بقيت بعض أرقام التشغيل التاريخية ومصفوفة الحالة القديمة تشير إلى `docs/23-DESKTOP-APP.md` و`TODO.md`. |

**حالة التنفيذ والترتيب التالي:** اكتمل منع فحص root التلقائي وإصلاح بوابة Linux وتمرير أخطاء أوامر ADB؛ كما أضيفت كتابة Logcat محدودة تضمن flush عند الإلغاء أو بلوغ حد الحجم. أُغلقت ثغرتا مخطط العقود، وفُرضت موافقات مرتبطة بالخطة على تدفقات التثبيت والإزالة ونقل الملفات. أضيف `AuditStore` على SQLite ووُصل بتدفقات الكتابة الأربع وقراءات facts/apps/APK/Logcat وقائمة الملفات، واكتشاف ADB وmDNS وقراءة Windows PnP. أصبح الاقتران اللاسلكي عملية `DeviceWrite` مستهدفة بعنوان خدمة الاقتران؛ نجح زر WPF على SM-N976N بعد رفع انتظار خدمة Connect إلى 20 ثانية، وأكد AuditStore بدء ونجاح الاقتران ثم discovery. رمز الاقتران لا يدخل السجل. أضيف نوعا discovery والاقتران إلى schema. فحص العقود الصارم مرّ بـ48 فحصاً و13 حالة سلبية. الاختبارات المحلية الكاملة: 548 ناجحاً؛ وبناء WPF: 0 تحذيرات و0 أخطاء. smoke قراءة الهاتف أكمل facts والتقاط Logcat لمدة 30 ثانية، 1,299 سطراً دون crash events/groups. اختُبر WPD worker محلياً بعامل وهمي: عملية واحدة عبر connect/list/download، مسار Unicode، تنزيل 16 MiB، وإنهاء العامل عند timeout والإلغاء والتخلص من الجلسة؛ أضيف هذا إلى CI على Windows. تعداد WPD الفعلي الأخير أعاد 0 أجهزة؛ الهاتف حاضر عبر Wi-Fi فقط، لذلك اختبار USB/MTP البطيء ما زال غير مثبت. **الخطوة التالية:** وصل الهاتف عبر USB واختيار File Transfer/MTP، ثم إعادة اختبار WPD الحقيقي داخل جلسة مفتوحة وقراءة مجلد بطيء والتحقق من قتل العامل؛ بعدها اختبر انقطاع النقل وأحجام/أسماء واقعية، ثم اربط provider بواجهة WPF. تظل Device Registry و`device_id` المحلي و`OperationEngine` وMCP مراحل لاحقة. يبقى `LogcatStream` المنقول خارج مسار المنتج حتى تُحل حدوده وتسجيل serial دون كسر بوابة attribution. لا تُعتبر أي مرحلة مكتملة قبل فحوصها المحددة.

## 10. قرارات المنتج التي نثبتها ولا نعيد فتحها في هذه الخطة

- Windows 10 وWindows 11.
- Android 7.0/API 24 فأحدث، مع capability checks لكل ميزة.
- المنتج منصة شاملة، لا يقتصر على APK installation.
- تثبيت APK وSplit APK وAPKS أساسي داخل محور التطبيقات؛ APKM/XAPK/OBB امتدادات مستقلة حسب العينات والاختبار.
- F-Droid تكامل أصلي أولاً؛ APKPure وAPKMirror عبر تدفقات تحترم آليات المواقع.
- لا root افتراضياً، ولا generic shell عبر MCP.
- لا قراءة Qualcomm/MediaTek أو بروتوكولاتهما.
- لا تفليش أو كتابة أقسام أو فتح bootloader في النطاق الحالي.
- لا ادعاء بنسخ بيانات التطبيقات الخاصة أو full-device backup.
- إعادة استخدام مشاريع GitHub الجاهزة قبل كتابة بديل، مع مراجعة الكود والأمان والاختبارات.
- لا تعتبر مراجعة المصدر أو CI أو README دليلاً على Runtime/Hardware.

## 11. ما لم يتم إثباته بعد

- مقارنة Tauri/Rust ألغيت عند اعتماد .NET 10 + WPF؛ PoC المطلوب الآن يثبت التنفيذ المختار فقط.
- لم يُختبر زر التطبيق أو واجهة WPF بصرياً؛ أداة Computer Use غير متاحة حالياً.
- لا يوجد تحقق USB فعلي من تعريف هاتف. smoke خدمة PnP على المضيف ليس اختباراً لهاتف.
- أضيف provider WPD read-only فوق MediaDevices 2.0.0 داخل worker process مستقل. تحقق تعداد المجلدات وتنزيل ملف صغير على هاتف فعلي. اختبار Unicode والملفات الكبيرة والانقطاع، وفحص مهلة عملية بطيئة بعد فتح الجلسة، ما زالت لاحقاً.
- Backup/Restore الكامل غير منفذ، ولا نعد بنسخ بيانات التطبيقات الخاصة. `chen3feng/android-backup` مرجع خوارزمية فقط بسبب auto-all-devices والتحقق الضعيف؛ لا يُنسخ كما هو.
- OTA/partition metadata لا يزال غير منفذ. `payload-dumper-go` مرشح list-only مع حدود manifest/signature موثقة؛ `lucky-arch` يحتاج hardening واختبارات، و`lpunpack_cyth` لا يحقق metadata-only على sparse input بأمان.
- MCP لم يُربط بـCodex. مراجع `adb-mcp` و`android-mcp-server` ليست خوادم جاهزة للدمج؛ القرار استخدام .NET MCP SDK فوق Application Services نفسها مع read-only tools وtarget صريح. `ReadOnlyDeviceService` يغطي الآن facts وapp inventory/version فقط؛ device_id/runtime binding وstdio host لم تُنفذ.
- مصادر التطبيقات وF-Droid ليست مدمجة؛ الأولوية الموثقة هي signed-index integration، بينما APKPure/APKMirror يظلان تدفق صفحات/تنزيل محدوداً.
- يجب إعادة فحص المصدر والرخصة والصيانة وtests عند اختيار أي مرشح لتنفيذ جديد؛ مراجعة SHA في 13–16 و20 ليست اعتماداً نهائياً أو دليل تشغيل.

**الحالة:** الستاك والعقود الأساسية مغلقة، وبعض خدمات Android وPnP نُفذت واختُبرت. WPD worker حقق تعداد المجلدات وتنزيل ملف مع التحقق على هاتف فعلي؛ اختبار مهلة عملية داخل جلسة لم يُحسم لأن فتح الجلسة نفسه انتهى بمهلة. لا تبدأ محور MCP أو Backup/firmware حتى يكتمل هذا الفحص أو يتغير ترتيب الأولويات. لا تعتبر هذه الوثيقة موافقة لتغيير حالة جهاز أو Windows.

**تحديث حالة WPD بعد النقل والإلغاء على الهاتف:** اكتمل تنزيل ملف Unicode بحجم 9,971,441 بايت إلى مجلد محلي مؤقت مع تطابق الحجم، ثم حُذف. أُلغي طلب تنزيل أكبر ملف معروف (67,588,123 بايت) بعد 250 ms؛ توقف worker ورفضت جلسته طلباً جديداً ولم تبق ملفات مؤقتة. نجحت جلسة جديدة وأعادت تعداد 74 ملفاً. لا يزال فصل USB أثناء النقل واختبار timeout فعلي على الهاتف غير متحققين. لا تُعدّل ملفات الهاتف لأجل الاختبار.

**تحديث تكامل WPD بسطح المكتب:** أضيف `WpdBrowserViewModel` وخدمة WPD المدققة إلى تبويب `MTP storage`؛ الاختيار صريح، والقراءة لا تعرض إلا البيانات الوصفية، والتنزيل المحلي لا يبدأ إلا بعد مراجعة معاينة والضغط على التأكيد. تُسجل القراءات بـ`file.list` والتنزيلات بـ`file.pull`. بناء المشروع في مجلد تحقق معزول نجح بـ0 تحذيرات و0 أخطاء، والمجموعة الكاملة مرّت بـ548 اختباراً. العرض الفعلي والنقر لم يُتحققا لأن Computer Use فشل في تهيئة kernel ولأن exe الحالي مفتوح.
