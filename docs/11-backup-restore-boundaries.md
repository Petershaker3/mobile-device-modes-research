# Android Backup & Restore Boundaries Research

> **الغرض:** تحديد ما يمكن نسخه احتياطياً واستعادته من Android بصورة موثوقة، وفصل الملفات العامة عن APKs وبيانات التطبيقات الخاصة ونسخ النظام. هذه وثيقة بحث وتصميم؛ لم يتم إنشاء backup أو restore على جهاز حقيقي.

**آخر مراجعة:** 12 نوفمبر 2026  
**المنصة:** Windows 10/11 وAndroid 7.0 (API 24) فأحدث.  
**السياسة:** لا root افتراضياً، لا cloud upload تلقائي، لا ادعاء بوجود full-device backup، ولا restore لبيانات تطبيق خاصة دون مسار نظام موثق.

## 1. الخلاصة التنفيذية

لا يوجد مسار ADB عام يضمن نسخ كل بيانات كل التطبيقات الخاصة واستعادتها على كل إصدارات Android. لذلك نقسم Backup إلى scopes مستقلة:

1. **User Files Backup:** الصور والفيديوهات والمستندات والتنزيلات والمجلدات التي يسمح ADB/MTP بقراءتها.
2. **APK Package Export:** تصدير base APK وsplit APKs والبيانات الوصفية للتطبيق؛ لا يشمل بياناته الخاصة.
3. **App Private Data Backup:** غير مضمون عبر ADB عام. يعتمد على Android Backup Service/Auto Backup أو التطبيق نفسه أو صلاحيات root/debuggable.
4. **System/Firmware Backup:** نسخ صور الأقسام والـsystem image يحتاج صلاحيات/أدوات خاصة؛ خارج نطاق النسخ الافتراضي.
5. **Device Snapshot:** تقرير معلومات الجهاز والتطبيقات والإعدادات المتاحة والسجلات، وليس نسخة قابلة لإعادة الجهاز كما كان.
6. **Legacy ADB Backup Import:** فتح ملفات .ab القديمة عند الحاجة؛ لا نعتمد adb backup لإنشاء نسخ جديدة عامة.

**الأولوية العملية:** نسخ ملفات المستخدم مع manifest وintegrity verification وincremental snapshots، ثم restore آمن للملفات. تصدير APKs يبقى ضمن محور التطبيقات في [03](03-app-management-and-network.md). لا نعد بنسخ app private data أو النظام كاملاً.

## 2. حدود Android الرسمية

### 2.1 Auto Backup for Apps

وثائق Android Developers تذكر أن Auto Backup:
- يعمل للتطبيقات التي تستهدف وتعمل على Android 6.0/API 23 فأحدث.
- ينسخ بيانات التطبيق وفق قواعد التطبيق وmanifest؛ يمكن للتطبيق تعطيله عبر allowBackup.
- الحد الافتراضي يصل إلى 25 MB من backup data لكل app user.
- يخزن نسخ Google Drive تحت حساب المستخدم.
- تكون البيانات end-to-end encrypted على أجهزة Android 9 فأحدث باستخدام PIN/pattern/password للجهاز.
- يدعم cloud backup وdevice-to-device transfer، مع اختلاف قواعد include/exclude حسب Android/targetSdk.

المراجع:
- https://developer.android.com/identity/data/autobackup
- https://developer.android.com/about/versions/12/behavior-changes-12

**ما يعنيه ذلك لبرنامج Windows:** Auto Backup خدمة يديرها Android والتطبيق والحساب/النقل؛ ليست API عامة تتيح لبرنامجنا تنزيل نسخة كل تطبيق من Google Drive أو سحب مجلد بياناته الخاص عبر ADB.

### 2.2 adb backup بعد Android 12

Android 12 غيّر السلوك الافتراضي لأمر adb backup:
- بالنسبة للتطبيقات التي تستهدف API 31 فأعلى، تُستثنى بيانات التطبيق من البيانات المصدّرة.
- يمكن للتطبيقات في سياق الاختبار/التطوير أن تسمح بتصدير بياناتها عبر android:debuggable=true.
- هذا الخيار لا يجعل التطبيق production غير القابل للتصحيح قابلاً للنسخ العام.

المصدر الرسمي:
https://developer.android.com/about/versions/12/behavior-changes-12#adb-backup-restrictions

**النتيجة:** لا نستخدم adb backup كمسار P0 لنسخ بيانات التطبيقات، ولا نعرض "App Data Backup: Success" إلا إذا كان هناك دليل فعلي من مسار موثوق.

### 2.3 Device-to-device transfer

Android يدعم نقل البيانات بين الأجهزة ضمن setup/restore flows، لكن العملية يديرها النظام و/OEM، وقد تختلف تفاصيلها بين الشركات والإصدارات. لا نفترض وجود أمر ADB عام يحول D2D إلى backup archive محلي يمكن لبرنامجنا استعادته لاحقاً.

### 2.4 ما الذي يتيحه ADB عملياً؟

- يمكن نقل الملفات التي يسمح shell user بقراءتها عبر adb pull.
- يمكن إعادة الملفات إلى مسار يملك shell user حق الكتابة إليه عبر adb push.
- لا يضمن ذلك الوصول إلى /data/data أو قواعد بيانات التطبيقات الخاصة.
- run-as متاح فقط لتطبيقات debuggable، وليس طريقاً عاماً إلى بيانات كل تطبيق.
- لا نستخدم su/root افتراضياً ولا نتجاوز سياسات الجهاز.

مرجع ADB:
https://developer.android.com/tools/adb

## 3. أنواع النسخ التي يجب أن تظهر في الواجهة

| النوع | ما الذي يتضمنه؟ | قابل للاستعادة؟ | الحالة |
|---|---|---|---|
| Shared Files Backup | مجلدات المستخدم التي يمكن قراءتها مثل DCIM/Download/Documents/Pictures | نعم، إلى مسار يحدده المستخدم | مدعوم مبدئياً عبر 09 |
| APK Export | base/split APKs وpackage metadata | تثبيت APK ممكن؛ لا يعيد بيانات التطبيق | محور 03 |
| App Private Data | databases/shared preferences/files داخل sandbox | ليس عاماً؛ يتطلب system backup/app-specific/root/debuggable | غير مضمون، لا وعد |
| Contacts Export | vCard أو صيغة أخرى عبر app/permission مخصص | ممكن فقط عبر مسار منفصل وصلاحية مناسبة | خارج default scope |
| SMS/Call Logs | بيانات شخصية عالية الحساسية | تحتاج permissions/system role/restore support منفصل | خارج default scope |
| Settings Snapshot | قيم إعدادات عامة متاحة للقراءة | لا تعني restore كامل للنظام | تقرير فقط |
| Firmware/Partition Images | صور أقسام وبيانات boot | لا restore تلقائي؛ لا كتابة ضمن هذا النطاق | فحص محلي فقط في 07/08 |
| Diagnostic Snapshot | معلومات الجهاز والتطبيقات والسجلات | غير قابل لإعادة حالة الجهاز | تقرير |

**تمييز مهم:** تصدير APK لا ينسخ app data. ونسخ /sdcard لا ينسخ قواعد بيانات التطبيقات الخاصة. ونسخ ملف firmware لا يساوي backup للنظام الحالي.

## 4. تدقيق مشاريع GitHub الجاهزة

### 4.1 chen3feng/android-backup — incremental shared-file backup

المستودع: https://github.com/chen3feng/android-backup  
الفرع: master. تمت مراجعة README وbackup.py وadbsync/adb.py وadbsync/local_fs.py وbackup.bat.

ما يقدمه:
- Python utility لنسخ مجلدات Android المحددة إلى الكمبيوتر عبر ADB.
- USB وWireless ADB.
- دعم عدة أجهزة متصلة مع ملفات إعداد لكل serial.
- include/exclude paths.
- incremental sync يعتمد على size وmtime.
- multi-version snapshots مع hardlinks للملفات غير المتغيرة عند دعم نظام الملفات.
- Windows batch entrypoint موجود.

نقاط يمكن اقتباسها:
- تنظيم source directories كـroot/source pairs.
- فصل ADB process wrapper عن local filesystem snapshot manager.
- hardlink-based snapshots لتقليل المساحة عندما يدعم filesystem ذلك.
- exclude patterns.
- دعم device-specific configuration.

حدود وملاحظات المصدر:
- backup.py ينفذ backup لكل الأجهزة المكتشفة تلقائياً؛ لا يناسب سياسة اختيار جهاز واحد صراحةً قبل العملية.
- incremental comparison يعتمد size وmtime (مع tolerance) لا SHA-256؛ قد يفوّت تغييرات نادرة إذا تغير المحتوى مع بقاء الحجم/mtime متقاربين.
- في مسار pull_dir، ينزل مجلداً ثم يزيل excluded files محلياً؛ قد يؤدي ذلك إلى تنزيل ملفات مستبعدة مؤقتاً قبل حذفها من النسخة المحلية.
- multi-version snapshot يعتمد hardlinks؛ على FAT32/exFAT قد يتحول إلى copy ويستهلك مساحة أكبر.
- README يعلن أن Windows مدعوم، لكن لم يتم اختبار التشغيل على Windows 10/11 في هذه الجولة.
- لم تتم مراجعة كل parser/path edge cases ولم يتم تشغيل المشروع.

**الحكم:** مرجع جيد لخوارزمية incremental shared-file snapshots، مع تعديل explicit device selection، hash verification، filtering قبل النقل، وWindows path tests. ليس محركاً معتمداً كما هو.

روابط المصدر:
- [README](https://github.com/chen3feng/android-backup/blob/master/README.md)
- [backup.py](https://github.com/chen3feng/android-backup/blob/master/backup.py)
- [adbsync/adb.py](https://github.com/chen3feng/android-backup/blob/master/adbsync/adb.py)
- [adbsync/local_fs.py](https://github.com/chen3feng/android-backup/blob/master/adbsync/local_fs.py)

### 4.2 mrrfv/open-android-backup — workflow متكامل لكن Windows عبر WSL2

المستودع: https://github.com/mrrfv/open-android-backup  
الفرع: master (المشروع يصرح أن master للتطوير وأن الإصدارات المستقرة عبر Releases/Tags).

README يعلن:
- نسخ التطبيقات كـAPK فقط؛ app private data غير مشمولة بسبب قيود النظام.
- نسخ internal storage.
- companion app لتصدير بيانات إضافية بموافقة/تفاعل المستخدم.
- contacts بصيغة vCard، وSMS CSV، وcall logs نصية؛ بعض هذه البيانات view-only.
- ضغط 7z وتشفير الأرشيف والـmetadata.
- wired/wireless backup.
- Windows workflow يحتاج WSL2، WSLg، Bash، ADB، 7z واعتماديات Linux؛ ليس تطبيق Windows-native مستقل.

تمت مراجعة README وbackup-windows.ps1 وbackup.sh وbackup_func.sh:
- script يوضح scopes ويطلب اختيار فئات النسخ.
- companion app يستخدم لتصدير بعض البيانات إلى مجلد مؤقت على الهاتف ثم سحبها.
- README يصرح صراحةً أن app data غير مشمولة.
- hooks.sh يمكن أن ينفذ كوداً يملكه المستخدم بصلاحيات script؛ المشروع نفسه يحذر أن hooks تشكل خطراً أمنياً إذا لم تتم مراجعتها.

**الحكم:** مرجع مفيد لتصميم scopes وarchive manifest وcompanion export، لكنه ليس قاعدة لبرنامج Windows-native بسبب WSL2 والاعتماديات. لا ننسخ جمع contacts/SMS/call logs إلى default scope، ولا نفعل hooks.

روابط:
- [README](https://github.com/mrrfv/open-android-backup/blob/master/README.md)
- [Windows wrapper](https://github.com/mrrfv/open-android-backup/blob/master/backup-windows.ps1)
- [Backup script](https://github.com/mrrfv/open-android-backup/blob/master/backup.sh)
- [Backup functions](https://github.com/mrrfv/open-android-backup/blob/master/functions/backup_func.sh)
- [Companion app](https://github.com/mrrfv/open-android-backup/tree/master/companion_app)

### 4.3 official-arvind/jigar-tools — Windows-native candidate، غير معتمد

المستودع: https://github.com/official-arvind/jigar-tools  
الفرع: main.

README يعلن PowerShell 5.1+/WinForms، 20-thread ADB transfer، delta sync، tree selection، restore picker، وHTML logs.

تمت مراجعة README وJigarSmartSync.ps1:
- يوجد process runner عبر ProcessStartInfo.
- الأوامر تستخدم ProcessStartInfo.Arguments string، لا ArgumentList؛ يتطلب ذلك تدقيق quoting لكل path/serial.
- script يملك 3-stage fallback؛ المرحلة الثالثة تستخدم su/root وmount مؤقتاً لتجاوز قيود الوصول إلى الملفات.
- script يحاول حذف ADB temp files عند الإلغاء باستخدام pattern /data/local/tmp/jgr_*.
- README يعلن سرعات نقل عالية، لكن هذه claims لم يتم اختبارها.
- استخدام 20 threads وroot fallback لا يناسبان default read-only/no-root architecture.
- لم تتم مراجعة كل مسارات restore أو auto-updater أو بناء البرنامج، ولم يتم تشغيله.

**الحكم:** لا نعتمده كقاعدة Backup/Restore؛ يمكن دراسة TreeView selection UI بعد مراجعة مستقلة، لكن لا ننقل root fallback أو multi-thread transfer assumptions أو أوامر الحذف.

### 4.4 nelenkov/android-backup-extractor — legacy .ab only

المستودع: https://github.com/nelenkov/android-backup-extractor

- أداة Java لفتح وإعادة حزم ملفات Android backup بصيغة .ab القديمة.
- تدعم unpack/pack وصيغ header وإمكانية كلمة مرور للأرشيفات المشفرة.
- لا تنشئ backup من جهاز ولا تتجاوز قيود Android 12.
- صيغة tar داخل .ab حساسة لترتيب الملفات، وREADME يحذر من archives غير متوافقة.

**الحكم:** دعم استيراد legacy .ab اختياري فقط، لا نستخدمها لإنشاء نسخ جديدة ولا كمسار restore عام.

روابط:
- [README](https://github.com/nelenkov/android-backup-extractor/blob/master/README.md)
- [AndroidBackup.java](https://github.com/nelenkov/android-backup-extractor/blob/master/src/org/nick/abe/AndroidBackup.java)

### 4.5 مرشحون إضافيون لم تتم مراجعتهم بالكامل

- [ParadoxEpoch/adb-backup-extract](https://github.com/ParadoxEpoch/adb-backup-extract): أداة Windows لفك ملفات .ab القديمة إلى tar.
- [mrrfv/open-android-backup](https://github.com/mrrfv/open-android-backup): archive/companion workflow.
- [chen3feng/android-backup](https://github.com/chen3feng/android-backup): incremental file sync.
- [Android Backup Extractor](https://github.com/nelenkov/android-backup-extractor): legacy .ab.

لا نضيف أداة إلى المنتج لمجرد أن README يصفها بأنها full backup؛ يجب تحديد scope ومراجعة المصدر واختبار restore.

## 5. Backup Manifest المقترح

كل backup يجب أن يكون self-describing، ويمكن التحقق منه دون الاتصال بالهاتف:

| الحقل | الغرض |
|---|---|
| backup_id | معرف داخلي فريد |
| schema_version | نسخة صيغة manifest |
| created_at | وقت الإنشاء |
| source_device_id | معرّف داخلي منقح؛ لا نخزن hardware serial خاماً في اسم المجلد |
| device_model / build_fingerprint / android_api | سياق المصدر |
| selected_scopes[] | الفئات التي وافق المستخدم على نسخها |
| provider | ADB / MTP / companion |
| entries[] | path/kind/size/mtime/hash/status |
| skipped_entries[] | path/reason مثل PermissionDenied/Excluded/Unsupported |
| app_packages[] | package/version/APK artifact refs إن تم اختيار APK export |
| archive_hash | SHA-256 للأرشيف أو لكل blob |
| encryption | algorithm/key derivation metadata/version؛ لا كلمة مرور خام |
| tool_version | إصدار البرنامج |
| verification_status | Verified / Partial / Failed / NotChecked |

لا نخزن محتوى الملفات في manifest، ولا نخزن كلمات المرور أو مفاتيح التشفير.

## 6. Backup Engine المقترح

~~~text
Explicit Device Selection
          |
      Scope Preview
          |
    Backup Manifest Plan
          |
      File Providers
      /           \
    ADB           MTP
      \           /
       Transfer Queue
             |
     Temp/Staging Store
             |
      Size + SHA-256
             |
      Snapshot Manager
             |
      Backup Archive
             |
     Verify + Report
~~~

### 6.1 قواعد النسخ

- اختيار جهاز واحد صراحةً قبل إنشاء job؛ لا backup لكل الأجهزة تلقائياً.
- اختيار المجلدات مع preview للعدد والحجم التقديري.
- exclusions تطبق قبل نقل البيانات، لا بعد تنزيلها.
- الملفات تتنقل streaming مع حدود concurrency.
- لا تعتمد incremental على size/mtime فقط؛ استخدم hash أو استراتيجية موثقة للتحقق من التغيير.
- إذا استخدمنا hardlinks للسnapshots، يجب التحقق من filesystem support وأن حذف snapshot لا يحذف بيانات snapshot أخرى.
- staging folder لكل job، ثم commit snapshot بعد التحقق.
- عند الإلغاء: يحتفظ البرنامج بسجل incomplete ويزيل الملفات المؤقتة التي أنشأها فقط.
- كل entry يسجل PermissionDenied/Skipped/Unsupported بدلاً من إسقاطه بصمت.
- backup local by default؛ لا cloud sync أو upload تلقائي.
- تشفير النسخة المحلية خيار واضح، وليس سبباً لرفع البيانات لخدمة خارجية.

### 6.2 Restore Engine

- قراءة manifest والتحقق من hash قبل بدء restore.
- عرض الجهاز المستهدف ومقارنته بمصدر النسخة.
- عرض مسارات الملفات التي ستكتب والتعارضات قبل الموافقة.
- لا overwrite افتراضياً؛ الخيارات Skip / Keep both / Overwrite / Cancel.
- staging محلي ثم adb push/MTP copy إلى الوجهة المختارة.
- لا يحذف البرنامج المصدر المحلي بعد restore.
- لا يحذف ملفات إضافية على الهاتف لمجرد أنها غير موجودة في backup.
- تحقق من size/hash بعد النقل عند توفر وسيلة قراءة.
- restore jobs قابلة للإلغاء وتحتفظ بسجل الملفات التي نجحت/فشلت.
- لا تدعي أن restore الملفات يعيد التطبيقات إلى حالتها الداخلية السابقة.

## 7. ما الذي يمكن أن نعد المستخدم به؟

| العبارة | مسموح؟ | السبب |
|---|---|---|
| "نسخ مجلد الصور والتنزيلات والمستندات التي يمكن قراءتها" | نعم، مع تقرير الملفات التي تخطيناها | نطاق واضح وقابل للتحقق |
| "نسخ ملفات المستخدم القابلة للوصول عبر ADB/MTP" | نعم، مع توضيح حدود الصلاحيات | لا يفترض الوصول إلى كل المسارات |
| "تصدير التطبيقات المثبتة كـAPK" | نعم، إذا تم تصدير كل splits المطلوبة والتحقق منها | لا يشمل app data |
| "نسخ بيانات كل التطبيقات" | لا | Android 12+ والـsandbox والسياسات تمنع ضمان ذلك |
| "نسخة كاملة من الهاتف" | لا | لا يوجد full raw image أو private app data |
| "استعادة الجهاز كما كان" | لا | file restore لا يعيد settings/app state/system |
| "نسخ firmware أو super image" | لا في default scope | يتطلب صلاحيات/ملفات خاصة؛ الفحص المحلي موثق في 07/08 فقط |

## 8. أولويات المنتج

| الأولوية | الوظائف |
|---|---|
| P0 | File backup لمجلدات يختارها المستخدم، manifest، SHA-256، exclusions قبل النقل، تقرير partial/errors |
| P1 | Incremental backup، multi-version snapshots، encryption at rest، restore preview/conflict handling، file restore |
| P2 | APK package archive integration، legacy .ab import، companion provider لتصدير scope يختاره المستخدم |
| خارج النطاق الحالي | App private data backup عام، root backup، full system/partition image backup، SMS/call log/contact harvesting، cloud backup integration |

## 9. اختبارات القبول

| الاختبار | المتوقع |
|---|---|
| لا يوجد جهاز | لا يبدأ job |
| أكثر من جهاز | اختيار صريح لجهاز واحد؛ لا auto-backup لكل المتصلين |
| جهاز unauthorized/offline | لا يبدأ pull |
| PermissionDenied | يسجل skipped reason ويكمل باقي الملفات |
| ملف تغير حجمه | يعاد نسخه |
| ملف تغير محتواه مع ثبات الحجم/mtime | hash verification يكتشف التغيير |
| exclude pattern | لا يتم تنزيل الملفات المستبعدة أصلاً |
| فشل منتصف النقل | لا يظهر backup كـVerified؛ staging غير المكتمل واضح |
| cancel | cleanup يقتصر على temp files التي أنشأها job |
| hardlink unsupported | fallback إلى copy مع بيان استهلاك المساحة |
| backup archive corrupted | verification fails |
| encrypted archive wrong password | لا يكتب partial restore |
| restore على موديل مختلف | تحذير قبل النقل؛ لا يمنع استعادة ملفات عامة يوافق عليها المستخدم |
| destination conflict | preview ثم قرار صريح |
| restore cancelled | لا حذف للنسخة الأصلية ولا تنظيف ملفات لم ينشئها البرنامج |
| app APK export | التقرير يوضح أن app private data غير مشمولة |
| Android 12+ adb backup | لا ادعاء بتصدير app data للتطبيقات target API31+ |
| manifest version unsupported | رفض الاستعادة مع رسالة واضحة |

## 10. المصادر الرسمية

- [Android Auto Backup for Apps](https://developer.android.com/identity/data/autobackup)
- [Android 12 behavior changes — ADB backup restriction](https://developer.android.com/about/versions/12/behavior-changes-12)
- [Android Debug Bridge](https://developer.android.com/tools/adb)
- [Android data backup overview](https://developer.android.com/identity/data/backup)
- [OWASP MASTG — Backups](https://mas.owasp.org/MASTG-KNOW-0050/)

## 11. ما لم يتم إثباته

- لم يتم تشغيل أي مشروع Backup/Restore على Windows 10/11 في هذه الجولة.
- لم يتم نسخ أو استعادة ملفات من هاتف حقيقي.
- لم يتم اختبار Auto Backup/adb backup على أجهزة API 24–35.
- لم يتم اختبار incremental backup مع ملفات تتغير دون تغيير الحجم/mtime.
- لم يتم اعتماد archive format أو تشفير معين.
- لم يتم تنفيذ أي restore إلى جهاز.

**الحالة:** البحث ومراجعة حدود Android ومصادر GitHub تمت. التوصية هي Shared Files Backup أولاً، مع APK export منفصل، وعدم الوعد بنسخ بيانات التطبيقات الخاصة أو صورة كاملة للهاتف. لا يوجد اعتماد دمج أو اختبار Hardware.
