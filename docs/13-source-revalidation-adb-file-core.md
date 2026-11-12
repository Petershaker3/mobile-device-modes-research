# Source Revalidation — ADB Core, File Transfer & Logcat

> **الغرض:** إعادة فحص ثلاثة مرشحين أساسيين عند commits مثبتة: AdvancedSharpAdbClient وADB-Explorer وAdbGui. هذه مراجعة مصدر محددة وليست تشغيل Build/Tests أو تجربة أجهزة.

**تاريخ المراجعة:** 12 نوفمبر 2026  
**النطاق:** Windows 10/11، Android 7/API 24+، ADB USB/Wireless.  
**الحالة:** Source Revalidation — لا Build محلي، لا Runtime، لا Hardware.

## 1. ملخص القرار

| المكوّن | النتيجة | ما يمكن إعادة استخدامه | ما يمنع الاعتماد المباشر |
|---|---|---|---|
| AdvancedSharpAdbClient | أقوى مرشح لطبقة ADB typed في .NET | DeviceData، SyncService، PackageManager، progress/cancellation | raw shell argument construction، remote basename collisions، cleanup غير مضمون عند الفشل، install session لا يظهر له abandon path |
| ADB-Explorer | أقوى مرجع لتصميم File Transfer Engine | streaming pipe، backpressure، transfer queue، progress throttling، sync-v2 handling | المشروع WPF كامل، ويستخدم fork محلياً من AdvancedSharpAdbClient؛ Hardware integration tests تتطلب جهازين وتكتب ملفات كبيرة |
| AdbGui | مرجع غني لـADB parsers وLogcat controller والاختبارات | parsers وfixtures، ring buffer، filters، bounded publishing، device-serial-scoped stream | Kotlin/Compose Desktop؛ generic shell runner موجود؛ بعض أزرار Logcat تمسح سجلات الجهاز أو تغير logd |

**القرار الحالي:** AdvancedSharpAdbClient يبقى المرشح الأول لاختبار Host Core .NET، وADB-Explorer مصدر مرجعي لمكونات النقل التي يمكن فصلها، وAdbGui مصدر لاستخلاص parsers/fixtures/Logcat design. لا نعتمد أياً منها كما هو ولا ننسخ generic shell أو GUI كاملة.

## 2. AdvancedSharpAdbClient — commit محدد

المستودع:
https://github.com/SharpAdb/AdvancedSharpAdbClient/tree/cd46a145ac4a9f05ec3b21a7d2b10882411d93d7

- Branch: main
- Commit: cd46a145ac4a9f05ec3b21a7d2b10882411d93d7
- Project: AdvancedSharpAdbClient
- Project file: [AdvancedSharpAdbClient.csproj](https://github.com/SharpAdb/AdvancedSharpAdbClient/blob/cd46a145ac4a9f05ec3b21a7d2b10882411d93d7/AdvancedSharpAdbClient/AdvancedSharpAdbClient.csproj)
- Async package manager: [PackageManager.Async.cs](https://github.com/SharpAdb/AdvancedSharpAdbClient/blob/cd46a145ac4a9f05ec3b21a7d2b10882411d93d7/AdvancedSharpAdbClient/DeviceCommands/PackageManager.Async.cs)
- Device identity: [DeviceData.cs](https://github.com/SharpAdb/AdvancedSharpAdbClient/blob/cd46a145ac4a9f05ec3b21a7d2b10882411d93d7/AdvancedSharpAdbClient/Models/DeviceData.cs)
- Async tests: [PackageManagerTests.Async.cs](https://github.com/SharpAdb/AdvancedSharpAdbClient/blob/cd46a145ac4a9f05ec3b21a7d2b10882411d93d7/AdvancedSharpAdbClient.Tests/DeviceCommands/PackageManagerTests.Async.cs)
- CI: [build-and-test.yml](https://github.com/SharpAdb/AdvancedSharpAdbClient/blob/cd46a145ac4a9f05ec3b21a7d2b10882411d93d7/.github/workflows/build-and-test.yml)

### 2.1 ما ثبت في الكود

1. **Device identity**
   - DeviceData يحتوي Serial وTransportId وState وModel وProduct وUSB/features fields.
   - PackageManager يرتبط بكائن DeviceData محدد، وهذا مناسب لمبدأ explicit target.
   - يجب أن يظل serial/transport identity جزءاً من DeviceHandle عندنا؛ لا نستخدم FirstOrDefault من أمثلة المشروع.

2. **Framework compatibility**
   - csproj يتضمن net8.0 وnet10.0، وعند Windows يتضمن net10.0-windows10.0.17763.0.
   - workflow يختبر Windows/Linux/macOS باستخدام .NET SDK 11.0.x.
   - هذا يجعل .NET 10 Host Core فرضية عملية، لكنه لا يحسم اختيار UI أو معمارية المنتج.

3. **Single APK**
   - InstallPackageAsync يرفع الملف عبر SyncService إلى /data/local/tmp/ ثم ينفذ pm install.
   - يستخدم Path.GetFileName(localFilePath) لإنشاء المسار البعيد؛ ملفان مختلفان لهما basename نفسه قد يتعارضان في عمليات متزامنة على الجهاز نفسه.
   - RemoveRemotePackageAsync ينفذ بعد نجاح مسار التثبيت العادي، لكن لا يوجد finally موحد يضمن cleanup عند exception/cancellation أثناء upload أو install.
   - إلغاء العملية قد يترك الملف المؤقت على الهاتف؛ يحتاج Operation Engine إلى cleanup/reconciliation مستقل عن cancellation token الأصلي.

4. **Split APK install**
   - InstallMultiplePackageAsync يرفع base ثم يرفع split files، ويستخدم WhenAll في رفع splits.
   - InstallMultipleRemotePackageAsync ينشئ install session ثم ينفذ install-write للـbase والـsplits ثم install-commit.
   - مسار remote session الذي تمت مراجعته لا يظهر فيه pm install-abandon داخل catch/finally عند فشل كتابة split أو cancellation بعد إنشاء session.
   - بعض مسارات كتابة session متتابعة؛ رفع الملفات المحلية قد يكون متوازياً، فلا نخلط بين upload concurrency وinstall-write sequencing.
   - GetVersionInfoAsync موجودة كلبنة post-install، لكن caller هو المسؤول عن إعادة قراءة package/version ومقارنة النتيجة؛ المكتبة لا تجعل هذا تحققاً نهائياً تلقائياً.

5. **Command construction / input validation**
   - CreateInstallSessionAsync يجمع packageName وarguments في shell command string.
   - InstallRemotePackageAsync يجمع arguments في command string.
   - WriteInstallSessionAsync يضم session/apkName/path في command string، مع quoting للمسار لا يساوي validation شاملاً لكل input.
   - لا نمرر package names أو flags أو remote paths غير موثوقة. نستخدم typed allowlists وvalidation صارماً، ولا نعرض shell العام عبر Application API أو MCP.

6. **Remote temp lifecycle**
   - SyncPackageToDeviceAsync يستخدم basename فقط داخل /data/local/tmp/.
   - RemoveRemotePackageAsync يستخدم rm على المسار المؤقت.
   - نحتاج remote path مولداً وفريداً لكل Operation ID، وسجل ملكية للملفات المؤقتة، وتنظيفاً محدود المهلة بعد reconnect؛ لا نحذف أي ملف لم تنشئه العملية الحالية.

### 2.2 الاختبارات ودرجة الدليل

- توجد اختبارات Async تستخدم DummyAdbClient وDummySyncService للتحقق من command sequence وprogress ورفع base/splits.
- توجد ملفات APK test assets تشمل base.apk وsplit_config.arm64_v8a.apk وsplit_config.xxhdpi.apk.
- هذا يثبت وجود unit/mock coverage للمسارات المحددة؛ لا يثبت تثبيتاً على هاتف حقيقي أو نجاح USB/Wireless.
- workflow المراجع يبني ويختبر على Windows/Linux/macOS، لكن لم نشغله نحن في هذه الجولة، ولا نعامل وجوده كاختبارنا.

**الحكم:** مرشح جاد لطبقة ADB .NET بعد adapter داخلي وإغلاق فجوات cleanup/session/validation/post-verify. لا نستخدم PackageManager مباشرة من GUI أو MCP.

## 3. ADB-Explorer — commit محدد

المستودع:
https://github.com/Alex4SSB/ADB-Explorer/tree/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56

- Branch: master
- Commit: ce9331b4a3fa3f76b8d43c47d4a514d0099adb56
- [AdbService.Files.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB%20Explorer/Services/ADB/AdbService.Files.cs)
- [DirectoryLister.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB%20Explorer/Services/ADB/DirectoryLister.cs)
- [FileSyncOperation.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB%20Explorer/Services/FileOperation/FileSyncOperation.cs)
- [FileTransferOperation.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB%20Explorer/Services/FileOperation/FileTransferOperation.cs)
- [FileOperationQueue.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB%20Explorer/Services/FileOperation/FileOperationQueue.cs)
- [SyncPipe.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB%20Explorer/Services/Adb/SyncPipe.cs)
- [StreamingTransferTests.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB_Test/StreamingTransferTests.cs)

### 3.1 ما ثبت في الكود

1. **Streaming and bounded memory**
   - SyncPipe يربط pull من جهاز بـpush إلى جهاز آخر أو shell stream دون intermediate file عادي.
   - السعة الافتراضية للذاكرة 256 KiB، ويستخدم backpressure لإيقاف الكاتب عند امتلاء القناة.
   - يمكنه spill إلى temp file عند تفعيل ذلك وتوفر مساحة كافية، مع حد أدنى للمساحة الحرة قدره 5 GiB أو 10% من القرص (الأكبر).
   - spill file يفتح بـFileOptions.DeleteOnClose، ويوجد cleanup عند إغلاق/تفريغ القناة.
   - هذا نمط مفيد لنقل الملفات الكبيرة دون read-all إلى الذاكرة، لكن يجب اختباره على أقراص النظام قليلة المساحة وعلى cancellation/disconnect.

2. **Transfer queue and concurrency**
   - FileOperationQueue يدير قائمة العمليات وحالاتها وتقدمها وحدود العمليات المتزامنة.
   - FileSyncOperation يطبق streaming/SyncPipe ويدير progress callbacks مع throttling حتى لا يضغط على UI thread.
   - FileTransferOperation ينقل الملفات بين جهازين عبر pull/push، مع lanes وتقدم مستقل.
   - Move يحذف المصدر فقط عندما تكون كل الملفات قد وصلت إلى الهدف بنجاح؛ لا يحذف المصدر في حالة نقل جزئي وفق المسار الذي تمت مراجعته.
   - توجد عمليات تنظيف للملف الهدف عند فشل push، لكن يلزم مراجعة كل حالات partial/rename والـtarget overwrite قبل الاقتباس.

3. **File listing**
   - DirectoryLister يستخدم CancellationTokenSource وConcurrentQueue لتحديث نتائج القائمة تدريجياً.
   - هناك تدفق منفصل للبحث في أسماء الملفات والمحتوى وأسماء entries داخل الأرشيف.
   - يجب نقل هذا السلوك إلى Core مستقل عن Dispatcher/WPF، لا نقل ViewModel أو ObservableList إلى طبقة الخدمات.

4. **Windows target**
   - csproj يستهدف net10.0-windows10.0.26100.0، ويدعم x64 وARM64، مع SupportedOSPlatformVersion 10.0.18362.0.
   - المشروع WPF؛ لا ننسخ الواجهة كشرط لاستخدام محرك النقل.
   - csproj يوضح أنه يستخدم DLL محلياً من fork خاص بـAdvancedSharpAdbClient على branch fix/push-short-read. لذلك لا نفترض أن DLL المضمنة مطابقة لحزمة upstream عند SHA أعلاه. يجب مراجعة fork/patch أو استخدام upstream بعد اختبار short-read.

### 3.2 اختبارات Hardware وحدودها

- StreamingTransferTests تتطلب جهازين ADB فعليين أو جهازاً ومحاكياً.
- الاختبارات تستخدم مساراً مخصصاً على الجهازين: /sdcard/adb-explorer-test-stream.
- بعض الاختبارات تنشئ ملفات كبيرة تصل إلى 512 MiB، وتنفذ cleanup بـrm -rf على مسار الاختبار المحدد.
- هذه ليست Unit Tests آمنة للتشغيل على هاتف المستخدم اليومي دون مراجعة وإعداد جهازين مخصصين ومساحة تخزين كافية.
- توجد اختبارات أخرى لمحاكاة shell escaping وSyncPipe وtransfer speed، لكن لم نشغلها.

**الحكم:** أقوى مرجع لاستخراج File Transfer Engine patterns وSyncPipe/queue behavior، مع عزل كامل عن WPF والاعتماديات المحلية. لا نشغل Hardware tests إلا على أجهزة اختبار مخصصة وبموافقة تنفيذ منفصلة.

## 4. AdbGui — commit محدد

المستودع:
https://github.com/RookieJay/AdbGui/tree/4ca23e550b8040041bfeec474b1ac8ba65660c92

- Branch: master
- Commit: 4ca23e550b8040041bfeec474b1ac8ba65660c92
- [LogcatController.kt](https://github.com/RookieJay/AdbGui/blob/4ca23e550b8040041bfeec474b1ac8ba65660c92/core/src/main/kotlin/com/adbgui/core/device/LogcatController.kt)
- [CommandRunner.kt](https://github.com/RookieJay/AdbGui/blob/4ca23e550b8040041bfeec474b1ac8ba65660c92/core/src/main/kotlin/com/adbgui/core/adb/CommandRunner.kt)

### 4.1 ما يمكن الاستفادة منه

- LogcatController يأخذ serial صريحاً عند start؛ لا يحتاج اختيار الجهاز من داخل controller.
- ring buffer افتراضي 10,000 سطر، مع filtered buffer وStateFlow.
- conflated channel لتجميع إشعارات التحديث، وfixed-interval publish throttling لتقليل ضغط UI.
- Mutex يحمي تعديلات ArrayDeque من السباقات.
- فلاتر مستوى السجل، package/PID، tag include/exclude، والنص/Regex.
- شجرة الاختبارات تحتوي parsers وfixtures لمخرجات getprop وdumpsys package وls وlogcat وADB wireless pairing وdevice list.

### 4.2 ما لا نقتبسه كما هو

- المشروع Kotlin/Compose Desktop؛ لا يمكن استيراد Kotlin classes مباشرة إلى .NET Host Core دون runtime/IPC إضافي.
- CommandRunner يحتوي runShellCmd(serial, cmd) كواجهة shell عامة. لا نعرض هذه الواجهة للمنتج أو MCP.
- توجد وظائف تغيّر الجهاز:
  - fixLogcatDisabled يكتب persist.sys.logd.level ويعيد تشغيل logd.
  - clearLogcat يمسح ring الخاص بـlogd على الجهاز، وليس فقط عرض القائمة محلياً.
- لذلك لا ننقل هذه الوظائف إلى Read-only Logcat API. يمكن إعادة تصميم clear المحلي كعملية UI منفصلة، وأي مسح على الجهاز يبقى Device Write بموافقة مستقلة إن أُدرج لاحقاً.

**الحكم:** مرجع قوي لاستخلاص تصميم Logcat bounded stream وparsers والـfixtures، وليس مرشحاً لدمج CommandRunner أو نقل التطبيق كاملاً.

## 5. عقود Adapter المطلوبة بعد هذه المراجعة

### 5.1 ADB Adapter

- Start/ensure ADB server.
- Enumerate devices مع serial/transport id/state.
- Pair/connect/disconnect wireless مع redaction لرموز الاقتران.
- Execute allowlisted command typed arguments مع timeout/cancellation.
- Sync list/stat/pull/push streaming.
- Package install/uninstall/session APIs خلف Application Service فقط.
- لا generic shell method في public contract.

### 5.2 File Transfer Adapter

- Listing وstat وpath kind وpermission metadata مع partial/unsupported states.
- Streaming transfer مع bounded memory/backpressure.
- Unique operation-scoped staging paths.
- Queue/concurrency limits لكل جهاز ولكل host.
- Cancellation حقيقي للقراءة والكتابة، وcleanup token مستقل محدود المهلة.
- لا delete/move إلا بعد Preview/Approval والتحقق من وصول الملفات كاملة.

### 5.3 Logcat Adapter

- Start(serial, filters) وPause/Resume/Stop وExport local.
- Bounded ring buffer وpublish throttling.
- لا clear-device-buffer ولا setprop/restart logd في Read-only contract.
- أي تقرير/تصدير يخضع لتنقيح البيانات الحساسة وسياسة الاحتفاظ.

## 6. اختبارات PoC المشتقة

| المجال | الاختبار | النتيجة المطلوبة |
|---|---|---|
| Device identity | جهازان متصلان وUSB+Wireless لنفس الجهاز | اختيار serial/transport محدد؛ لا auto-select ولا ازدواج مضلل |
| ADB cancellation | إلغاء أثناء connect/shell/pull/push | توقف فعلي، حالة واضحة، لا تسريب موارد |
| APK staging | عمليتان لملفين لهما basename نفسه | remote paths فريدة ولا يحدث overwrite |
| Split session | فشل split بعد install-create | install-abandon أو reconciliation موثق؛ لا جلسة مجهولة صامتة |
| Command validation | package/path/flags تحتوي quotes أو shell metacharacters | رفض input غير صالح؛ لا shell injection |
| Post-install | نجاح pm install-commit | query مستقل للـpackage/version قبل إعلان verified |
| File streaming | ملف كبير أكبر من RAM buffer | streaming/backpressure، memory bounded |
| Disk pressure | temp drive أقل من spill reserve | لا spill غير آمن؛ fallback أو فشل واضح |
| Transfer disconnect | فصل source أو target أثناء النقل | لا Move source deletion؛ target partial cleanup محدود |
| Transfer integrity | hash source/target بعد النقل | تطابق أو فشل واضح؛ لا نجاح من exit code وحده |
| Logcat flood | stream كثيف مع filters | ring cap ثابت، UI publish throttled، لا unbounded memory |
| Logcat device state | clear view مقابل clear device buffer | العملية الافتراضية محلية ولا تمسح logd |
| Hardware integration tests | ADB-Explorer two-device suite | تشغيل فقط على test devices ومسار test مخصص بعد موافقة |

## 7. القرار التالي

1. إجراء PoC .NET 10 يستخدم AdvancedSharpAdbClient خلف ADB Adapter صغير.
2. اختبار ADB-Explorer SyncPipe/transfer patterns منفصلة عن WPF؛ لا نسخ DLL المحلي أو fork دون تدقيق patch.
3. بناء Logcat prototype من عقد read-only، والاستفادة من AdbGui fixtures/logic فقط؛ لا نقل generic shell.
4. عدم بدء أي install/uninstall/push/delete/DNS/driver operation على هاتف المستخدم خلال Source Revalidation.
5. بعد PoC، تحديث [12 — خطة إعادة الاستخدام](12-REUSE-AND-IMPLEMENTATION-PLAN.md) بقرار Host/UI ونتائج الاختبارات الفعلية.

## 8. ما لم يتم إثباته

- لم يتم build أو تشغيل AdvancedSharpAdbClient أو ADB-Explorer أو AdbGui.
- لم يتم تشغيل اختبارات xUnit أو MSTest أو Kotlin.
- لم يتم اختبار ADB USB/Wireless أو نقل ملفات أو تثبيت Split APK على هاتف.
- لم يتم تشغيل اختبارات ADB-Explorer التي تحتاج جهازين؛ ولم يتم تنفيذ أي rm أو نقل 512 MiB.
- لم يتم اعتماد أي مكتبة أو نسخ كود إلى منتج.
- لم يتم اختبار .NET 10 مقابل Tauri/Rust؛ قرار Stack ما زال مفتوحاً.

**الحالة:** Source Revalidation مكتمل لهذه المشاريع الثلاثة عند SHAs أعلاه. تم توثيق المكونات المرشحة والفجوات والاختبارات المطلوبة؛ لا يوجد Runtime/Hardware evidence أو اعتماد دمج.
