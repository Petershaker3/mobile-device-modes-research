# خطة إعادة الاستخدام والمعمارية والتنفيذ المقترح

> **الغرض:** تحويل نتائج البحث في الوثائق 00–11 إلى خطة تقنية قابلة للتنفيذ، تحدد المكونات المرشحة لإعادة الاستخدام، والعقود المشتركة، وترتيب بناء المحاور، وقرارات التقنية التي ما زالت تحتاج PoC. هذه الوثيقة ليست موافقة تنفيذ، ولا تعني اعتماد أي مشروع أو كود للدمج.

**تاريخ تثبيت مراجع المصدر:** 12 نوفمبر 2026  
**المنصة المستهدفة:** Windows 10/11، وAndroid 7.0 / API 24 فأحدث.  
**حالة المستودع:** بحث وتصميم؛ لا يوجد منتج منفذ أو اختبار Hardware معتمد.

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

**لا نختار بعد إطار الواجهة أو runtime النهائي.** أقوى فرضية حالية للاختبار هي .NET 10 Host Core بسبب وجود AdvancedSharpAdbClient ومرشحين Windows/C#، مع مقارنة Tauri 2/Rust في PoC محدود قبل تثبيت القرار. لا نبدأ Hybrid Tauri + .NET إلا إذا أثبتت التجربة أن فائدته تبرر تكلفة IPC والتغليف والتحديث.

## 2. ما الذي نعيد استخدامه؟

إعادة الاستخدام ليست قراراً واحداً؛ نختار أصغر مستوى مفيد: CLI/SDK، مكتبة، parser، adapter، أو نمط UI. لا ننسخ تطبيقاً كاملاً لمجرد أن واجهته قريبة.

| المحور | المرشح الأساسي | ما يمكن الاستفادة منه | ما لا نأخذه كما هو | الحالة الحالية |
|---|---|---|---|---|
| ADB protocol/client | [AdvancedSharpAdbClient](https://github.com/SharpAdb/AdvancedSharpAdbClient) | DeviceData/Serial/TransportId، SyncService، PackageManager، install-multiple، progress/cancellation | لا parser صيغ، ولا سياسة أمان أو lifecycle مكتملة؛ نحتاج unique staging وabandon/cleanup وpost-verify | أقوى مرشح Host Core؛ مراجعة مصدر محددة في 02 §20.54 |
| إدارة الملفات | [ADB-Explorer](https://github.com/Alex4SSB/ADB-Explorer) | File listing، transfer queue، sync/streaming، progress patterns | لا ننسخ GUI أو كل dependencies؛ نفصل AdvancedSharpAdbClient خلف adapter | مرشح استخراج مكونات؛ يلزم تثبيت commit المراجعة قبل الاقتباس |
| واجهة/معلومات ADB وLogcat | [AdbGui](https://github.com/RookieJay/AdbGui) | Device discovery، read info، Logcat Core وواجهات التشخيص | لا سياسة اختيار الجهاز أو كل التطبيق؛ مراجعة اختلاف README عن المصدر | مرجع مكونات، لا دمج كامل |
| تثبيت APKS | [Google bundletool](https://github.com/google/bundletool) | DeviceSpec، TOC/APKS selection، install-apks | لا يستبدل APKM/XAPK parser ولا سياسة اختيار الجهاز | المرجع الأساسي لصيغة APKS القياسية |
| تحليل APKM/XAPK | [xapkInstaller](https://github.com/adhu2018/xapkInstaller)، [pacman-apk](https://github.com/nuloperrito/pacman-apk)، [apkfile](https://github.com/david-lev/apkfile) | مقارنة parsers وmanifest/OBB handling | لا نعتمد parser واحداً قبل corpus واختبارات ZIP/signature/path؛ لا نفترض schema موحداً | مرشحون متنافسون؛ لا اعتماد دمج |
| OTA payload | [payload-dumper-go](https://github.com/ssut/payload-dumper-go) | Header/manifest/partition metadata وhash، واستخراج محلي اختياري | لا flash/sideload أو كتابة جهاز؛ نبدأ metadata-only | مرشح مكتبة Go |
| Dynamic partitions | [AOSP partition_tools/liblp](https://android.googlesource.com/platform/system/extras/+/master/partition_tools/)، [lucky-arch](https://github.com/soe1hom-arch/lucky-arch) | مرجع صحة metadata وlist-only/sparse parsing | لا lpmake/lpflash/lpadd في المنتج؛ مرشحو Windows يحتاجون corpus/tests | AOSP مرجع، وlucky-arch مرشح |
| Logcat viewer | [logcat-viewer](https://github.com/daitouniao/logcat-viewer)، AdbGui | parsing/viewer/indexing/performance | لا اعتماد تلقائي على UI أو auto-device selection | مرجعان لاستخراج أجزاء محددة |
| Bugreport/Perfetto | [Bugreport_analyzer](https://github.com/VenkateshJallu/Bugreport_analyzer)، [Perfetto](https://github.com/google/perfetto) | parser/analysis references وأداة متخصصة | لا تحميل تقارير الهاتف إلى خدمات خارجية، ولا parser دون ZIP limits/privacy | مرشحان للتجارب المعزولة |
| Backup files | [chen3feng/android-backup](https://github.com/chen3feng/android-backup) | incremental sync، include/exclude، snapshots | auto-select لكل الأجهزة، وsize/mtime-only comparison، وتنزيل المستبعد قبل حذفه | مرجع خوارزمي؛ نحتاج إعادة تصميم |
| Legacy .ab | [Android Backup Extractor](https://github.com/nelenkov/android-backup-extractor) | import/unpack legacy .ab | لا ينشئ backup حديثاً ولا يتجاوز Android 12 restrictions | ميزة استيراد اختيارية P2 |
| MCP/ADB | [us-all/android-mcp-server](https://github.com/us-all/android-mcp-server)، [iksnerd/adb-mcp](https://github.com/iksnerd/adb-mcp) | نماذج أدوات وschemas وADB integration | لا نقل generic shell أو auto-targeting؛ كل tool يحتاج policy وapproval واختبار injection | مراجع تصميم ومراجعة أمان |
| Windows PnP/Drivers | Windows PnP/SetupAPI/Configuration Manager وPnPUtil، ومصادر Microsoft/Google/OEM | APIs الرسمية للتشخيص والمطابقة | لا تثبيت/إزالة driver تلقائياً دون exact Hardware ID وpreview/approval/rollback | تصميم في 04؛ يحتاج PoC Windows |
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
| Host Core | .NET 10 | Rust/Tauri 2 core | ADB USB/Wireless، process cancellation، Windows PnP/WPD، memory، packaging، testability |
| GUI | WinUI 3/WPF | Tauri Web UI | offline startup، responsiveness، accessibility، multi-window/device dashboards، update model |
| ADB | AdvancedSharpAdbClient | Google adb.exe عبر Process Adapter | pairing/connect، multi-device isolation، shell/exec-out، sync streaming، cancellation، recoverability |
| MTP | Windows WPD COM/API wrapper | ADB-only first ثم WPD provider | enumeration، Unicode، large files، Android API/OEM differences |
| Local persistence | SQLite | JSON/flat files | transactions، schema migration، audit retention، corruption recovery |
| MCP | .NET MCP SDK أو TS SDK عبر STDIO | MCP host منفصل يتصل بـApplication API محلية | Codex handshake، tool schemas، user ACL، approval flow، process lifecycle |
| Dependency delivery | bundled pinned tools | managed install/update of signed official tools | offline-first، integrity/hash، rollback، Windows Defender/SmartScreen behavior |

### PoC المطلوب قبل تثبيت Stack

- Windows 10 وWindows 11 على x64 كبداية اختبار؛ ARM64 قرار مستقل لاحقاً.
- اكتشاف جهازين ADB USB مع اختيار serial صريح.
- Wireless pairing/connect/disconnect/reconnect.
- قراءة getprop/df/package list مع timeout وcancellation.
- push/pull streaming لملفات صغيرة وكبيرة وأسماء Unicode.
- Device discovery من Windows PnP، ثم تجربة MTP/WPD منفصلة.
- محاكاة device disconnect أثناء upload/install session.
- MCP STDIO tool read-only يقرأ device info من نفس Application Service، لا من مسار موازٍ.
- قياس cold start، idle memory، package size، وoffline behavior.
- نفس الاختبارات على تنفيذ .NET وTauri/Rust قبل قرار نهائي؛ لا نقارن benchmark واحداً فقط.

## 7. خطة ترتيب البناء بعد موافقة تنفيذ مستقلة

| المرحلة | الناتج | شروط الخروج |
|---|---|---|
| 0 — Source Revalidation | تثبيت commits النهائية، فروق المصدر، الاعتماديات، build instructions، security review | لا توجد فجوة حرجة مجهولة في المرشح المختار |
| 1 — Host Core PoC | Device Registry، explicit DeviceHandle، ADB discovery، typed command execution | USB/Wireless/multi-device/cancel/disconnect tests |
| 2 — Shared Contracts | Device/Capability/Operation/Result/Audit schemas | عقود موحدة ومختبرة ومصدّرة لـGUI وMCP |
| 3 — Read-only Device Intelligence | System info، mode detection، software inventory، storage/partition metadata | corpus fixtures وAPI 24+ tests وsource provenance |
| 4 — Application Management & Installer | app inventory، format adapters، package inspection، APK/APKS ثم APKM/XAPK | format corpus، signature/split checks، device install verification |
| 5 — File & Backup | ADB file manager، MTP provider، file backup/restore | path safety، hash verification، cancellation/recovery |
| 6 — Diagnostics | Logcat، Bugreport، dumpsys، Perfetto integration | privacy/ZIP limits، streaming/retention tests |
| 7 — Network & Drivers | DNS preview/apply/verify/restore؛ Windows driver diagnosis/repair | جهاز/Windows test matrix، rollback، official source verification |
| 8 — MCP | tools/resources فوق Application Services | Codex integration، explicit target، approval، injection/security tests |
| 9 — Hardening | Offline packaging، migrations، update/rollback، performance، accessibility | CI + runtime matrix + documented release gates |

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

## 9. قرارات المنتج التي نثبتها ولا نعيد فتحها في هذه الخطة

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

## 10. ما لم يتم إثباته بعد

- لم يتم بناء PoC .NET أو Tauri للمقارنة.
- لم يتم بناء أو تشغيل أي مرشح ضمن بيئة Windows 10/11 في هذه الجولة.
- لم يتم توصيل هاتف حقيقي أو اختبار USB/Wireless/MTP/Fastboot.
- لم يتم تشغيل تثبيت APK/APKS/APKM/XAPK على جهاز حقيقي.
- لم يتم تنفيذ Backup/Restore أو DNS change أو Driver repair.
- لم يتم ربط MCP بـCodex.
- لم يتم اعتماد أي مشروع أو dependency للدمج النهائي.

**الحالة:** خطة إعادة الاستخدام والمعمارية وترتيب البناء موثقة. الخطوة التالية البحثية هي Source Revalidation للمرشحين الأعلى أولوية ثم PoC مقارنة Host Core، قبل أي قرار Stack أو بدء تنفيذ. هذه الوثيقة لا تمنح موافقة تنفيذ على البرنامج أو الجهاز.
