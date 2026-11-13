# Android Device File Management & Transfer Research

> **الغرض:** تحديد طبقات استعراض الملفات ونقلها بين Windows وAndroid، ومراجعة المشاريع الجاهزة على مستوى المصدر والاختبارات، مع الفصل بين ADB وMTP/WPD والقرص الافتراضي. هذه وثيقة بحث وتصميم؛ لم تُنفذ عمليات نقل على هاتف فعلي في هذه الجولة.

**آخر مراجعة:** 12 نوفمبر 2026  
**المنصة المستهدفة:** Windows 10/11 وAndroid 7.0 (API 24) فأحدث.  
**القيود:** لا بحث في Qualcomm أو MediaTek أو التراخيص. لا root افتراضياً، ولا وصول إلى بيانات التطبيقات الخاصة دون صلاحية موثقة وموافقة صريحة.

## 1. الخلاصة التنفيذية

لا يوجد سبب لحصر إدارة الملفات في بروتوكول واحد. نحتاج إلى طبقة File Management موحدة فوق Providers مستقلة:

1. **ADB File Provider:** استعراض ونقل وإدارة الملفات عبر ADB USB/Wireless، مع دعم مسارات Android العامة التي يسمح بها shell user.
2. **MTP/WPD Provider:** استعراض الملفات التي يعرضها الهاتف عبر MTP عندما لا تكون جلسة ADB متاحة أو عندما تكون واجهة MTP أنسب.
3. **Virtual Drive Provider (اختياري):** تركيب تخزين الهاتف كقرص Windows من خلال تطبيق مرافق وWinFsp، مثل WinDroidFS.
4. **Local File Provider:** إدارة ملفات Windows وتحديد الوجهة المحلية.
5. **Transfer Engine:** queue، progress، cancel، retry، staging، partial cleanup، size/hash verification، وحالات واضحة.
6. **File Metadata Service:** النوع والحجم وتاريخ التعديل والصلاحيات وsymlink عند توفرها، مع عدم اختلاق معلومات الإنشاء.

**أفضل مرجع Host-side لإدارة عمليات ADB والملفات:** [Alex4SSB/ADB-Explorer](https://github.com/Alex4SSB/ADB-Explorer)، خصوصاً DirectoryLister وFileSyncOperation وSyncPipe وFileOperationQueue.  
**أفضل مرشح جاهز لتجربة dual-pane وWindows Unicode transfers:** [wkbin/AdbFileManager](https://github.com/wkbin/AdbFileManager)، لكن لا يُنسخ كما هو قبل إصلاح حواجز الحذف وتقدم السحب.  
**أفضل مرشح اختياري للقرص الافتراضي:** [Wilsoon7721/WinDroidFS](https://github.com/Wilsoon7721/WinDroidFS)، بعد معالجة device selection وتحديد هل نريد تطبيق Android مرافقاً.  
**مرشح إضافي متعدد الأنظمة:** [JSleim/adb-file-explorer](https://github.com/JSleim/adb-file-explorer)، فيه cancellation وpartial cleanup، لكن يختار الجهاز الوحيد تلقائياً.

## 2. فصل طرق الوصول

### 2.1 ADB

ADB يتيح نقل ملفات ومجلدات من وإلى الجهاز عبر push/pull. هذا هو المسار الأساسي لإدارة الملفات في برنامجنا لأنه يدعم USB وWireless ADB ويتوافق مع طبقة ADB الموجودة بالفعل.

~~~powershell
adb -s <serial> pull <remote-path> <local-path>
adb -s <serial> push <local-path> <remote-path>
adb -s <serial> shell ls -la <remote-path>
adb -s <serial> shell stat <remote-path>
adb -s <serial> shell mkdir -p <remote-path>
~~~

مرجع Android الرسمي: https://developer.android.com/tools/adb

حدود ADB:
- لا تعني عبارة "أي مسار" أن كل مسار قابل للقراءة أو الكتابة؛ صلاحيات shell وSELinux وسياسة الجهاز تظل حاكمة.
- مجلدات التطبيقات الخاصة مثل /data/data ليست مساحة تصفح عامة. run-as يخص التطبيقات القابلة للتصحيح (debuggable) فقط، وليس طريقاً عاماً إلى بيانات كل التطبيقات.
- لا نستخدم su/root تلقائياً ولا نخفي فشل الصلاحية.
- لا نخلط بين ADB authorization وMTP authorization؛ هما قناتان مختلفتان.

### 2.2 MTP/WPD على Windows

Windows Portable Devices API يوفر واجهات لتعداد الأجهزة ومحتوياتها ونقل الملفات وقراءة metadata:
- IPortableDeviceManager لاكتشاف الأجهزة.
- IPortableDevice / IPortableDeviceContent لفتح الجهاز وتعداد المحتوى.
- IEnumPortableDeviceObjectIDs لتعداد عناصر المجلد.
- IPortableDeviceProperties لقراءة خصائص العناصر.
- IPortableDeviceDataStream / IStream لنقل البيانات.

مراجع Microsoft:
- https://learn.microsoft.com/en-us/windows/win32/windows-portable-devices
- https://learn.microsoft.com/en-us/windows/win32/wpd_sdk/enumerating-devices
- https://learn.microsoft.com/en-us/windows/win32/wpd_sdk/enumerating-content
- https://learn.microsoft.com/en-us/windows/win32/wpd_sdk/client-interfaces
- https://learn.microsoft.com/en-us/samples/microsoft/windows-classic-samples/wpd-sample/

**القرار المعماري:** MTP/WPD Provider مستقل، ولا نحاول تمرير WPD object IDs إلى ADB أو العكس. كل Provider يعيد FileEntry موحداً مع Provider ID ومصدر metadata.

### 2.3 Virtual Drive

WinDroidFS يقدم طبقة مختلفة:
- Android companion app يشارك مساحة التخزين التي يحددها المستخدم.
- Windows daemon يعرضها كقرص افتراضي عبر WinFsp.
- الاتصال قد يكون Direct USB/AOA أو ADB forwarding أو USB tethering أو Wi-Fi.
- ADB forwarding يستخدم مساراً خاصاً للوصول إلى Android/data وAndroid/obb في تصميمه.

هذا ليس بديلاً عن ADB File Provider؛ إنه خيار تكامل مستقل إذا أردنا أن تظهر ملفات الهاتف في File Explorer وكأنها قرص.

## 3. نموذج FileEntry الموحد

كل Provider يجب أن ينتج حقلاً موحداً، مع الحفاظ على البيانات الخاصة بالمصدر:

| الحقل | المعنى |
|---|---|
| provider_id | adb / mtp / virtual_drive / local |
| device_id | معرّف الجهاز المحدد صراحةً، أو null للملفات المحلية |
| path | مسار داخل Provider، وليس host path |
| name | اسم العنصر |
| kind | file / directory / symlink / other / unknown |
| size_bytes | الحجم أو null |
| modified_at | وقت آخر تعديل أو null |
| created_at | null ما لم يوجد مصدر موثوق |
| permissions | بيانات الصلاحيات عند توفرها |
| owner / group | عند توفرها |
| is_hidden | حالة العرض المخفية |
| is_readable / is_writable | نتيجة تحقق فعلية أو unknown |
| status | Success / Partial / PermissionDenied / Unsupported / NotFound / Disconnected / ParseError |
| source_metadata | metadata الخام الخاصة بالـProvider |

لا نخترع created_at من ctime؛ ctime في Unix يعني وقت تغيير metadata وليس تاريخ الإنشاء.

## 4. وظائف إدارة الملفات المطلوبة

### 4.1 التصفح

- مجلدات وملفات مع breadcrumb/path bar.
- Back/Forward/Up/Refresh.
- فرز بالاسم والنوع والحجم وتاريخ التعديل.
- إظهار مخفي/إخفاء.
- Multi-select.
- Search داخل مسار محدد مع حد عمق وCancel.
- معاينة metadata والصلاحيات.
- tabs/bookmarks للمسارات المتكررة.
- Empty/PermissionDenied/Disconnected states منفصلة.
- عدم اتباع symlink تلقائياً أثناء البحث أو التصفح التكراري.

### 4.2 العمليات

| العملية | سياسة |
|---|---|
| Pull / Export | مسار محلي واضح، تحقق الوجهة والمساحة، progress/cancel |
| Push / Import | اختيار الوجهة على الجهاز، حماية الملفات الموجودة، Unicode |
| Copy within device | عملية منفصلة عن pull/push |
| Move | لا يحذف المصدر إلا بعد نجاح الوجهة والتحقق |
| Rename | تحقق من الاسم والمسار والتعارض |
| Create folder/file | تحقق من الاسم والصلاحية |
| Delete | تأكيد صريح، عرض العناصر، منع root/parent traversal، لا rm -rf بلا guard |
| Search | Streaming وCancel وحدود العمق |
| Properties | قراءة metadata فقط |
| Permissions | خارج المسار الافتراضي؛ يتطلب إذناً مستقلاً وتحذيراً |

### 4.3 Transfer Engine

- طابور عمليات، يدعم تشغيل عدة عمليات ضمن حد concurrency قابل للضبط.
- كل عملية تحمل Device ID وProvider ID وSource وDestination وOperation ID.
- progress bytes/total مع unknown إذا لم يتوفر الحجم.
- cancellation يوقف ADB child process أو stream فعلياً.
- partial destination يتم تنظيفه فقط إذا كان البرنامج هو من أنشأه، ولا يحذف نسخة قديمة موجودة قبل العملية.
- upload إلى staging name ثم rename عند نجاح التحقق.
- تنزيل الملفات إلى temp destination ثم atomic move إلى الوجهة النهائية عند نجاح التحقق.
- تحقق size أولاً، وhash اختياري/قابل للتفعيل للملفات المهمة أو الكبيرة.
- resume لا يفعل إلا إذا كان البروتوكول والمسار يدعمان الاستكمال بأمان؛ وإلا يبدأ نقل جديد.
- retries محدودة مع backoff، ولا يعاد تلقائياً تنفيذ delete/move.
- حماية تعارض الاسم: Overwrite / Keep both / Skip / Cancel، مع Preview قبل batch apply.
- سجل عملية محلي لا يحتوي محتوى الملفات.

### 4.4 Windows Unicode and Path Safety

اختبارات إلزامية:
- مسارات محلية فيها مسافات وأقواس وعلامات اقتباس.
- أسماء عربية وصينية ويابانية ورموز Unicode.
- Windows reserved names، trailing dots/spaces، طول المسار.
- أسماء Android تحتوي مسافات وsingle quote وdollar sign وpipe وnewlines.
- عدم تجميع arguments في command string؛ استخدم ProcessStartInfo.ArgumentList أو ProcessBuilder argument list.
- shell quoting منفصل عن Windows argument quoting؛ لا تستخدم دالة quoting واحدة للجهتين.
- symlink and reparse-point handling.
- لا تستخدم host path على أنه remote path أو العكس.

## 5. تدقيق مشاريع GitHub الجاهزة

### 5.1 Alex4SSB/ADB-Explorer — مرشح Host-side قوي

المستودع: https://github.com/Alex4SSB/ADB-Explorer  
الفرع الافتراضي: master. تمت مراجعة ملفات الخدمة والعمليات والاختبارات التالية على المصدر الحالي:
- ADB Explorer/Services/ADB/AdbService.Files.cs
- ADB Explorer/Services/ADB/DirectoryLister.cs
- ADB Explorer/Services/ADB/AdbService.Search.cs
- ADB Explorer/Services/FileOperation/FileSyncOperation.cs
- ADB Explorer/Services/FileOperation/FileTransferOperation.cs
- ADB Explorer/Services/FileOperation/AbstractSyncFileOperation.cs
- ADB Explorer/Services/FileOperation/FileOperationQueue.cs
- ADB Explorer/Services/Adb/SyncPipe.cs
- ADB_Test/StreamingTransferTests.cs

**ما وجدناه:**
- Directory listing streaming مع ConcurrentQueue وCancellationToken، بدلاً من انتظار اكتمال كل القائمة قبل عرضها.
- قراءة file metadata عبر stat، وتفسير Unix file mode/type/permissions.
- Search streaming مع حد depth ودعم include/exclude والبحث في المحتوى والأرشيف.
- FileSyncOperation يستخدم AdvancedSharpAdbClient sync protocol، ويدعم parallel lanes وthrottled UI progress.
- FileOperationQueue يدير Waiting/InProgress/Completed/Failed/Canceled.
- FileTransferOperation يدعم نقل الملفات بين جهازين من خلال SyncPipe بدلاً من تنزيل الملف كاملاً إلى ملف وسيط.
- SyncPipe تستخدم ذاكرة محدودة، backpressure، ويمكنها spill إلى ملف مؤقت مع cleanup.
- الاختبارات تشمل SyncPipe ordering/backpressure/spill/cleanup؛ بعض اختبارات النقل تحتاج جهازين حقيقيين.

**حدود الاقتباس:**
- التطبيق WPF/Windows ومترابط مع ViewModels/Dispatcher/Settings؛ لا ننقل UI إلى منتجنا.
- AdvancedSharpAdbClient dependency يجب عزلها خلف adapter بدلاً من ربط كل Service Layer بها.
- يوجد دعم root وعمليات ملفات متقدمة؛ لا نعرضها افتراضياً.
- يجب تدقيق parser/quoting وsymlink behavior لكل عملية.
- لم يتم build/run أو اختبار الجهاز في هذه الجولة.

**الحكم:** أقوى مرشح لاستخراج File Listing/Transfer Queue/SyncPipe patterns وعمليات محددة إلى Core مستقل. لا نسخ كامل للتطبيق.

روابط المصدر:
- [AdbService.Files.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/master/ADB%20Explorer/Services/ADB/AdbService.Files.cs)
- [DirectoryLister.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/master/ADB%20Explorer/Services/ADB/DirectoryLister.cs)
- [FileSyncOperation.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/master/ADB%20Explorer/Services/FileOperation/FileSyncOperation.cs)
- [FileTransferOperation.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/master/ADB%20Explorer/Services/FileOperation/FileTransferOperation.cs)
- [FileOperationQueue.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/master/ADB%20Explorer/Services/FileOperation/FileOperationQueue.cs)
- [SyncPipe.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/master/ADB%20Explorer/Services/Adb/SyncPipe.cs)
- [StreamingTransferTests.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/master/ADB_Test/StreamingTransferTests.cs)

### 5.2 wkbin/AdbFileManager — dual-pane وUnicode transfers

المستودع: https://github.com/wkbin/AdbFileManager  
الفرع: main. README يعلن Kotlin/Compose Desktop، dual-pane، ADB USB/Wireless، local/remote browsing، drag/drop، batch operations، code editor، terminal، وWindows Unicode workaround. يتطلب Java 25 عند البناء من المصدر؛ MSI المعلن يضم Java runtime.

تمت مراجعة:
- src/main/kotlin/data/remote/adb/AdbFileOperations.kt
- src/main/kotlin/data/remote/adb/AdbImpl.kt
- src/main/kotlin/data/remote/adb/Terminal.kt
- src/main/kotlin/viewmodel/FileManagerViewModel.kt
- src/test/kotlin/data/remote/adb/AdbFileOperationsTest.kt
- RELEASE_NOTES_v3.1.0.md

**نقاط قابلة للاقتباس:**
- تمرير ADB arguments كقائمة منفصلة عبر ProcessBuilder.
- تمرير Device ID إلى adb -s في أوامر shell/push/pull.
- Windows upload workaround: اسم staging ASCII ثم remote rename إلى UTF-8 الأصلي.
- cleanup للملف المحلي المؤقت والـremote staging في finally.
- اختبارات لأسماء Unicode والمسافات والاقتباس وعلامات الدولار، واختبارات لـsearch command construction.
- cancellation عبر coroutine/job وإنهاء ADB child process في Terminal.

**ملاحظات خطيرة تمنع النقل كما هو:**
1. delete(filePath) يستدعي rm -rf على absoluteRemotePath(filePath). absoluteRemotePath("") ينتج "/"، ولا يظهر guard في هذه الدالة يمنع حذف root. يجب إضافة منع على مستوى Service Layer قبل أي استخدام.
2. pullWithProgress يرسل progress 100 قبل await() لعملية pull؛ وهذا قد يعرض اكتمالاً قبل تأكيد نهاية النقل. لا يوجد اختبار ظاهر يغطي هذا المسار.
3. loadFiles يستخدم stat format مفصولاً بعلامة | ويمر على wildcard؛ أسماء الملفات التي تحتوي delimiter/newline تحتاج اختبارات خاصة.
4. loadFiles يستخدم stat -L، أي أن metadata قد تتبع symlink؛ لا نعتمد ذلك كسياسة traversal آمنة.
5. pushPathFromWindows يعالج المجلدات recursively؛ يجب تدقيق symlinks المحلية حتى لا يخرج الرفع عن المجلد الذي اختاره المستخدم.
6. Terminal يسجل command/output إلى stdout؛ لا ننقل هذا السلوك لأنه قد يسجل بيانات ملفات أو مخرجات حساسة.
7. service methods تعتمد currentDeviceProvider بدلاً من تمرير DeviceHandle صريح في العقد؛ نحتاج pinning صريح للجهاز داخل كل عملية.

**الحكم:** مرشح قوي لاستخراج فكرة dual-pane وUnicode staging والاختبارات، لكن يجب إصلاح root deletion guard وpull progress وsymlink/path safety قبل إعادة استخدام أي كود.

روابط المصدر:
- [AdbFileOperations.kt](https://github.com/wkbin/AdbFileManager/blob/main/src/main/kotlin/data/remote/adb/AdbFileOperations.kt)
- [AdbImpl.kt](https://github.com/wkbin/AdbFileManager/blob/main/src/main/kotlin/data/remote/adb/AdbImpl.kt)
- [Terminal.kt](https://github.com/wkbin/AdbFileManager/blob/main/src/main/kotlin/data/remote/adb/Terminal.kt)
- [AdbFileOperationsTest.kt](https://github.com/wkbin/AdbFileManager/blob/main/src/test/kotlin/data/remote/adb/AdbFileOperationsTest.kt)
- [Release notes v3.1.0](https://github.com/wkbin/AdbFileManager/blob/main/RELEASE_NOTES_v3.1.0.md)

### 5.3 Wilsoon7721/WinDroidFS — Virtual Drive اختياري

المستودع: https://github.com/Wilsoon7721/WinDroidFS  
الفرع: main، تحديث المستودع 11 نوفمبر 2026 وقت المراجعة. README يصف Rust Windows daemon + Kotlin Android companion + Protobuf protocol. إعداد Android يحدد minSdk 24 وtargetSdk 36.

تمت مراجعة:
- apps/windows-daemon/src/adb.rs
- apps/windows-daemon/src/adb_files.rs
- apps/windows-daemon/src/winfsp_fs.rs
- apps/android-host/app/src/main/java/dev/wilsoon/windroidfs/link/FileServer.kt
- Framing.kt وAdb.kt
- FileServerTest وFileServerScopeTest في شجرة الاختبارات.

**نقاط قيمة:**
- Windows virtual drive باستخدام WinFsp.
- Android app يحدد storage scope الذي يظهر للكمبيوتر.
- FileServer يتحقق من canonical path للبقاء داخل root المشارك.
- يمكن قفل الوصول حتى تأكيد biometric على الهاتف.
- framing protocol له magic/length وMAX_PAYLOAD.
- ADB provider منفصل لمعالجة Android/data وAndroid/obb عبر sync file transfer.
- Windows 10/11 وAndroid 7+ متوافقان مع نطاقنا المعلن حسب وصف المشروع وminSdk 24.

**قيود مهمة:**
- يحتاج Android companion app؛ ليس ADB-only.
- يحتاج WinFsp لقرص افتراضي محلي؛ يجب تقييم تثبيت driver/الاعتمادية كخيار مستقل.
- مسار ADB في apps/windows-daemon/src/adb.rs يختار أول جهاز authorized من adb devices عبر first_ready/phone؛ لا يطابق explicit device selection لدينا.
- لا نعتبر README claim عن الأداء/التوافق دليلاً على تجربتنا؛ لم يتم تشغيل المشروع أو قياسه.
- الوصول إلى Android/data وAndroid/obb يعتمد على مسار ADB منفصل عن FileServer app-side؛ لا نعمم النتيجة على كل جهاز قبل اختبارها.

**⚠️ حاجز ترخيص (أُضيف 13 نوفمبر 2026):** ملف `apps/windows-daemon/Cargo.toml` ينص على `license = "GPL-3.0-only"`. **GPL-3.0 يمنع نسخ أي كود إلى منتجنا.** لذلك يُصحَّح الحكم السابق: هذا المشروع **مرجع تصميم فقط، لا اقتباس كود**، ويُسجَّل هنا صراحةً حتى لا يُقتبس سهواً لاحقاً. راجع [20 §2.4](20-source-revalidation-round2.md).

**فكرة تُنقل (فكرة لا كود):** الـdaemon يستخدم delay-load لـWinFsp عبر `winfsp_link_delayload()`، ويسقط إلى **WebDAV mount** عند غياب WinFsp. هذا النمط صحيح لمبدأ "لا تجعل تثبيت مكوّن نظام شرطاً لتشغيل الميزة"، ويُنقل كتصميم مستقل: virtual drive اختياري + fallback بلا driver.

**الحكم المصحَّح:** مرجع تصميم فقط (لا اقتباس كود بسبب GPL-3.0). لا نضعه شرطاً لعمل File Manager الأساسي.

روابط المصدر:
- [WinDroidFS README](https://github.com/Wilsoon7721/WinDroidFS/blob/main/README.md)
- [Windows ADB provider](https://github.com/Wilsoon7721/WinDroidFS/blob/main/apps/windows-daemon/src/adb.rs)
- [ADB file provider](https://github.com/Wilsoon7721/WinDroidFS/blob/main/apps/windows-daemon/src/adb_files.rs)
- [WinFsp filesystem](https://github.com/Wilsoon7721/WinDroidFS/blob/main/apps/windows-daemon/src/winfsp_fs.rs)
- [Android FileServer](https://github.com/Wilsoon7721/WinDroidFS/blob/main/apps/android-host/app/src/main/java/dev/wilsoon/windroidfs/link/FileServer.kt)
- [Android Framing](https://github.com/Wilsoon7721/WinDroidFS/blob/main/apps/android-host/app/src/main/java/dev/wilsoon/windroidfs/link/Framing.kt)

### 5.4 JSleim/adb-file-explorer — مرشح Python/PyQt متعدد الأنظمة

المستودع: https://github.com/JSleim/adb-file-explorer  
الفرع: main.

README يعلن:
- USB وWireless ADB.
- dual-pane/multi-device panels وmulti-tabs.
- copy/move/rename/delete/search.
- PC-to-device وdevice-to-PC وdevice-to-device.
- cancellation وbackground tasks وcleanup للملفات الجزئية.

تمت مراجعة handler.py وoperation_control.py:
- uses argument arrays في subprocess.
- remote path quoting.
- OperationControl يدير cancellation events ويغلق process/socket المسجل.
- ينظف partial local/remote destination، ويحاول recovery للموجود سابقاً.

حدود:
- ADBHandler يختار الجهاز تلقائياً عندما يكون جهاز واحد متصلاً ولا يمرر serial صريحاً من المستخدم؛ نحتاج تعديل ذلك.
- بعض المسارات تستخدم shell command strings، وتتطلب تدقيق escaping/path validation.
- لم يتم مراجعة كل handlers أو تشغيل UI.

**الحكم:** مرجع ثانوي لعمليات cancellation وrecovery، لا ننسخ auto-select أو كل التطبيق.

### 5.5 مرشحون آخرون

| المشروع | ما يفيده | القرار |
|---|---|---|
| [Alex4SSB/ADB-Explorer](https://github.com/Alex4SSB/ADB-Explorer) | file listing/search/queue/streaming transfers | المرشح الأساسي للـCore |
| [wkbin/AdbFileManager](https://github.com/wkbin/AdbFileManager) | dual-pane/Unicode upload/Compose UI | مرجع UI وtransfer staging مع إصلاحات |
| [WinDroidFS](https://github.com/Wilsoon7721/WinDroidFS) | virtual drive/Android companion/WinFsp | feature اختياري مستقل |
| [JSleim/adb-file-explorer](https://github.com/JSleim/adb-file-explorer) | Python/PyQt/cancellation | مرجع ثانوي |
| [gregko/AdbExplorer](https://github.com/gregko/AdbExplorer) | WPF dual-pane file manager | مرجع أبسط، README يركز على USB |
| [T0biasCZe/AdbFileManager](https://github.com/T0biasCZe/AdbFileManager) | WinForms file transfer | README يذكر known bug في عرض بعض المجلدات؛ ليس المرشح الأول |
| [GustavLindberg99/AndroidDrive](https://github.com/GustavLindberg99/AndroidDrive) | mounting phone as drive via ADB/Dokan | مرجع virtual filesystem قديم نسبياً؛ WinDroidFS أكثر شمولاً في مسارات الاتصال |

## 6. حدود الوصول والخصوصية

- ADB shell user ليس root؛ لا تعرض ملفات لا يمكن قراءتها فعلياً.
- /data/data لا يظهر كمساحة عامة. لا تستخدم run-as إلا مع package debuggable وبعد التحقق من package identity.
- لا تستخدم root/su لإظهار "كل الملفات" كافتراض افتراضي.
- لا تجمع محتوى الرسائل أو قواعد بيانات التطبيقات أو بيانات حسابات المستخدمين ضمن مدير الملفات.
- الملفات المنقولة غير موثوقة؛ لا تفتح APK أو scripts تلقائياً بعد النقل.
- لا تتبع symlink أو junction/reparse point خارج الجذر الذي حدده المستخدم.
- كل حذف أو overwrite أو chmod أو move يحتاج confirmation وPreview للوجهة.
- لا تسجل محتويات الملفات في logs؛ اكتفِ بالمسارات المنقحة والأحجام والنتيجة.
- Device ID ثابت داخل عملية النقل؛ تغيير الجهاز المحدد في UI لا يغير وجهة عملية بدأت بالفعل.

## 7. المعمارية المقترحة

~~~text
Device Selection (explicit DeviceHandle)
             |
        File Service API
             |
    +--------+----------+
    |        |          |
   ADB      MTP/WPD   Virtual Drive
 Provider   Provider   Adapter (optional)
    |        |          |
    +--------+----------+
             |
      Unified FileEntry
             |
     Transfer Queue Engine
             |
  Progress / Cancel / Retry / Verify
             |
   Local Provider + Report/History
~~~

كل عملية تستقبل DeviceHandle صريحاً وProvider ID. لا يختار أي Provider أول جهاز تلقائياً. عمليات الجهازين تتطلب مصدر ووجهة محددين كل على حدة.

## 8. أولويات المنتج

| الأولوية | الوظائف |
|---|---|
| P0 | ADB list/stat/push/pull، USB/Wireless، dual-pane، multi-select، progress، cancel، Unicode، overwrite policy، create folder، rename، delete محمي، queue/history |
| P1 | MTP/WPD provider، recursive search، file preview، bookmarks، archive browsing، device-to-device transfer، resume عند دعمه |
| P2 | WinDroidFS/WinFsp virtual drive، virtual drive mount/unmount، Android companion storage scopes، advanced permissions/metadata |

## 9. اختبارات القبول

| الاختبار | المتوقع |
|---|---|
| جهازان متصلان | لا عملية دون اختيار الجهاز صراحةً |
| جهاز واحد فقط | لا auto-select؛ يظهر اختيار/تأكيد واضح |
| ADB unauthorized/offline | لا يبدأ النقل |
| مجلد فارغ | يظهر empty folder لا error |
| Permission denied | لا يختفي المسار كأنه غير موجود |
| أسماء عربية/Unicode/مسافات | النقل والعودة يحتفظان بالاسم والبايتات |
| أسماء تحتوي quotes/$/pipe/newline | لا shell injection ولا parsing corruption |
| file كبير | streaming دون RAM غير محدود |
| cancel upload/download | يوقف العملية وينظف partial الجديد فقط |
| destination exists | overwrite/rename/skip/cancel حسب قرار المستخدم |
| نقل جهاز إلى جهاز | لا يحذف المصدر قبل تحقق الهدف |
| symlink | لا traversal غير مقصود |
| path root/../ | رفض delete/write خارج scope |
| Windows protected folder | رسالة واضحة قبل النقل |
| MTP device disconnect | حالة منفصلة عن ADB disconnect |
| ADB + MTP لنفس الهاتف | يظهران كـProviders/Interfaces منفصلة دون دمج خاطئ |

## 10. المصادر الرسمية

- [Android Debug Bridge — Copy files to and from a device](https://developer.android.com/tools/adb)
- [Windows Portable Devices](https://learn.microsoft.com/en-us/windows/win32/windows-portable-devices)
- [WPD content enumeration](https://learn.microsoft.com/en-us/windows/win32/wpd_sdk/enumerating-content)
- [WPD client interfaces](https://learn.microsoft.com/en-us/windows/win32/wpd_sdk/client-interfaces)
- [WPD API sample](https://learn.microsoft.com/en-us/samples/microsoft/windows-classic-samples/wpd-sample/)

## 11. ما لم يتم إثباته

- لم يتم بناء أو تشغيل أي من مشاريع الملفات المذكورة.
- لم يتم نقل ملفات على هاتف حقيقي أو قياس سرعة.
- لم يتم إثبات أن Android/data وAndroid/obb متاحان عبر ADB على كل موديل/API.
- لم يتم اختبار WPD مع كل الشركات أو Windows 10/11 builds.
- لم يتم اختبار آلاف الملفات أو ملفات كبيرة جداً أو أسماء newline على جهاز فعلي.
- لم يتم اعتماد WinDroidFS أو WinFsp أو أي كود للدمج.

**الحالة:** البحث ومراجعة المصدر تمت للمرشحين المحددين؛ لا يوجد اعتماد دمج أو اختبار Hardware. محورا Firmware/Software وStorage/Partitions موثقان بالفعل في [07](07-firmware-software-inventory.md) و[08](08-storage-partition-information.md)، والنسخ الاحتياطي في [11](11-backup-restore-boundaries.md). الخطوة التالية هي Source Revalidation ثم PoC للنقل والـMTP/WPD وفق [12](12-REUSE-AND-IMPLEMENTATION-PLAN.md).
