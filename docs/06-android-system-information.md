# Android System & Device Information Research

> **الغرض:** تحديد المعلومات التي ينبغي قراءتها من جهاز Android عبر ADB، ومصادر كل معلومة وحدودها حسب API والصلاحيات، ثم تدقيق مشاريع GitHub الجاهزة لتقليل إعادة التنفيذ. هذه وثيقة بحث وتصميم، وليست كوداً منفذاً أو إثباتاً لتوافق كل هاتف.

**آخر مراجعة:** 12 نوفمبر 2026  
**النطاق:** Windows 10/11 host، Android 7.0 (API 24) فأحدث، قراءة فقط.  
**مستبعد:** أي بحث في قراءة Qualcomm أو MediaTek أو بروتوكولاتهما، وأي مقارنة للتراخيص.  
**حالة الاختبار:** لم تُجرَ تجارب Hardware في هذه الجولة.

## 1. الخلاصة التنفيذية

نحتاج إلى خدمة مستقلة باسم منطقي **Device Information Service** داخل المنصة، تستقبل Device ID/Serial صريحاً وتنتج تقريراً منظماً عن الجهاز. الأفضل أن تعتمد على ADB من Windows، لأن هذا ينسجم مع المنتج الحالي ولا يتطلب تثبيت تطبيق مرافق على الهاتف.

القرار المبدئي:

1. استخدام **adb -s <serial>** لكل أمر دون اختيار أول جهاز تلقائياً.
2. استخدام قائمة خصائص **getprop** محددة ومراجعة، بدلاً من سحب كل الخصائص دون تصنيف أو حدود.
3. جمع المعلومات من عدة مصادر: **getprop**، أوامر ADB القياسية، **dumpsys** المحدد، وملفات القراءة العامة مثل **/proc/meminfo** عند توفرها.
4. حفظ القيمة الخام والمفسرة ومصدرها ووقت جمعها وحالة القراءة؛ لا نعرض **Unknown** كأنه قيمة حقيقية ولا نملأ المفقود بالتخمين.
5. فصل بيانات التعريف الثابتة نسبياً عن المقاييس المتغيرة مثل البطارية والذاكرة والحرارة.
6. استخدام مشروع Windows جاهز كمرجع لأوامر ADB والـparsers والاختبارات، والاستفادة من مشاريع Android-side لفهم حقول النظام فقط؛ لا ننقل تطبيق Android كاملًا إلى برنامج Windows.
7. عدم جمع IMEI أو IMSI أو الرقم التسلسلي العتادي أو عناوين MAC ضمن التقرير الافتراضي. Serial الخاص بجلسة ADB هو معرّف توجيه الجلسة وليس تصريحاً بعرض معرّفات الهاتف الحساسة.

**أفضل مرشح عملي أولي للـHost-side:** [RookieJay/AdbGui](https://github.com/RookieJay/AdbGui)؛ لأنه Windows-first ويفصل core عن UI، ويحتوي GetpropParser واختباراته، مع بقاء ميزاته المعلنة وحدودها بحاجة إلى التحقق.  
**مرشح مساعد خفيف:** [muk1m0v/device_info](https://github.com/muk1m0v/device_info)؛ مشروع Windows 10/11 قائم على ADB ويعرض ملخصاً وتقارير قراءة فقط، لكنه أقل فصلاً وتوجد ملاحظات كود تمنع نسخه كما هو.  
**مراجع Android-side للحقول:** [ashokvarmamatta/ANTAR](https://github.com/ashokvarmamatta/ANTAR) و[ScoobyDouche/PhoneDiagnosticTool](https://github.com/ScoobyDouche/PhoneDiagnosticTool)، مع اختلاف مهم: PhoneDiagnosticTool يحدد minSdk 26، فلا يغطي Android 7 ضمن هدفنا.

## 2. ما الذي يجب أن يقرأه البرنامج؟

### 2.1 هوية الجهاز والمنتج

| الحقل | مصدر أولي مقترح | ملاحظات وحدود |
|---|---|---|
| Manufacturer / Brand / Model | ro.product.manufacturer, ro.product.brand, ro.product.model | قد تختلف قيم product/vendor/system على بعض الأجهزة؛ احتفظ بالقيمة الأصلية لكل namespace عند الحاجة ولا تفترض أن خاصية واحدة هي الحقيقة المطلقة |
| Product / Device / Board / Hardware | ro.product.name, ro.product.device, ro.product.board, ro.board.platform, ro.hardware | بيانات تعريف من build properties؛ قد تكون عامة أو ناقصة أو OEM-specific |
| Build fingerprint | ro.build.fingerprint | مفيد لتمييز build، لكنه ليس دليلاً تشفيرياً على أصالة الجهاز |
| Build ID / Display / Incremental | ro.build.id, ro.build.display.id, ro.build.version.incremental | حقول وصفية للنسخة |
| Build type / tags | ro.build.type, ro.build.tags | تعرض كقيم خام؛ لا نقرر Root/سلامة الجهاز من هذه العلامات وحدها |
| SKU | ro.boot.hardware.sku أو الخصائص المعروضة من النظام | قد لا تتوفر أو تختلف تسمية الحقل؛ لا نستنتج SKU من اسم الموديل |

### 2.2 Android ونسخة النظام

| الحقل | مصدر أولي | ملاحظات |
|---|---|---|
| Android release | ro.build.version.release | نص للعرض، لا تعتمد عليه وحده للمقارنة الرقمية |
| API / SDK level | ro.build.version.sdk | رقم SDK هو أساس شروط التوافق |
| Codename | ro.build.version.codename | قد يكون REL في الإصدارات النهائية |
| Security patch | ro.build.version.security_patch | تاريخ patch المعلن من النظام؛ لا يثبت أن كل مكونات OEM محدثة |
| Build date | ro.build.date.utc | timestamp build وليس تاريخ آخر تحديث |
| Base OS | ro.build.version.base_os | قد يكون فارغاً |
| Radio / Baseband | gsm.version.baseband أو dumpsys مناسب عند توفره | حقل اختياري؛ لا تخلطه بمعلومات firmware الكاملة |
| Kernel | adb shell uname -a أو cat /proc/version | نص خام؛ لا تفترض أن جميع الأجهزة تسمح بالقراءة نفسها |
| Bootloader version | ro.bootloader | قد يكون فارغاً أو غير ممثل في بعض builds |

### 2.3 ABI وخصائص المنصة

- ro.product.cpu.abilist
- ro.product.cpu.abi
- ro.product.cpu.abilist32
- ro.product.cpu.abilist64
- adb shell pm list features
- adb shell pm list libraries

قائمة ABI مهمة للتوافق مع APK، لكن لا تختزلها في ABI واحدة إذا كانت القائمة متاحة. pm list features يعرض خصائص يعلنها Package Manager؛ لا يثبت أن كل ميزة تعمل عملياً أو أن العتاد سليم.

بعض الحقول لها Android API رسمي مباشر داخل تطبيق Android، مثل Build.VERSION.SDK_INT وBuild.VERSION.SECURITY_PATCH وBuild.SUPPORTED_ABIS. هذه APIs مفيدة كمرجع للمقارنة، لكن برنامج Windows لا يستطيع استدعاء Java API على الهاتف مباشرة؛ يجب أن يحصل على القيمة عبر ADB أو تطبيق companion إن قررنا ذلك مستقبلاً.

### 2.4 الشاشة والعرض

- adb shell wm size
- adb shell wm density
- dumpsys display عند الحاجة وبعد مراجعة اختلاف الصيغ.

نخزن الحجم الفيزيائي/override منفصلين إذا كان الأمر يعرض الاثنين، ونخزن density مستقلة عن resolution. لا نخلط إعدادات العرض الحالية مع دقة لوحة الشاشة الأصلية.

### 2.5 الذاكرة والتخزين والطاقة

| المجال | مصادر القراءة المقترحة | حدود مهمة |
|---|---|---|
| RAM | cat /proc/meminfo, dumpsys meminfo | تمييز MemTotal وMemAvailable عن قيمة "المستخدم" المحسوبة؛ لا تجمع قيماً من تعريفات مختلفة |
| Storage | df -k أو df -h إن دعمها الأمر؛ مسارات محددة مثل /data و/sdcard | أسماء الأعمدة والوحدات ومخرجات df تختلف؛ parser يجب أن يتحقق من header/الأعمدة ولا يفترض صيغة واحدة |
| Battery | dumpsys battery | قراءة المستوى والحالة والحرارة والجهد عند وجودها؛ تفسير الحرارة والوحدات يحتاج تحققاً |
| Uptime | cat /proc/uptime أو dumpsys مناسب | افصل uptime عن وقت التشغيل منذ آخر boot وعن تاريخ build |
| CPU | cat /proc/cpuinfo, getprop، وقراءات sysfs عند توفرها | لا نستنتج اسم SoC تجارياً من hardware string غير موثوق؛ ولا نعتمد على مسارات sysfs خاصة بمورد واحد كحقائق عامة |
| GPU | dumpsys SurfaceFlinger أو مخرجات OpenGL المتاحة عبر تطبيق Android | بيانات GPU من ADB shell ليست موحدة؛ لا نعرضها كحقل مضمون |
| Sensors / Cameras / Codecs | تطبيق Android يستخدم Android framework APIs أو أدوات موثقة خاصة | ليست كلها قابلة للجمع بدقة من ADB shell العام؛ قد تحتاج companion app وصلاحيات منفصلة |

### 2.6 حالة النظام والخصائص

يمكن إضافة مجموعات قراءة منفصلة حسب الحاجة:

- adb shell getprop (بعد تصنيف whitelist keys فقط).
- adb shell pm list features.
- adb shell pm list libraries.
- adb shell service list.
- adb shell dumpsys battery.
- adb shell dumpsys meminfo.
- adb shell dumpsys power عند طلب تقرير تشخيص طاقة.
- adb shell settings get لقيم محددة ومسموح بها فقط.

لا ننفذ dumpsys شامل لكل الخدمات تلقائياً. بعض المخرجات ضخمة أو تحتوي بيانات شخصية/اتصالات/حسابات؛ يجب اختيار الخدمة والغرض وحدود الحجم قبل الجمع.

## 3. معمارية الجمع المقترحة

~~~text
Selected Device (explicit serial)
          |
     ADB Session
          |
   Read-only Command Runner
          |
   +------+----------------------+
   |                             |
Property Reader              Command Adapters
(getprop whitelist)       (wm / pm / df / proc / dumpsys)
   |                             |
   +-------------+---------------+
                 |
          Parsers + Validators
                 |
        Normalized Device Model
                 |
      Provenance / Status / Timestamp
                 |
          Report + Cache
~~~

### 3.1 عقد البيانات لكل حقل

كل قيمة يجب أن تحفظ بهذا المعنى، حتى لو تغير شكل JSON لاحقاً:

| الخاصية | الغرض |
|---|---|
| key | اسم ثابت للحقل، مثل android.sdk |
| value | القيمة المفسرة أو null |
| raw_value | النص الأصلي بعد حدود التنظيف |
| source | الأمر/الخاصية/الواجهة التي أنتجت القيمة |
| collected_at | وقت القراءة |
| status | Success / Empty / Unsupported / PermissionDenied / Timeout / DeviceDisconnected / ParseError |
| confidence | Direct / Derived / Heuristic؛ لا نستخدم High لمجرد أن النص موجود |
| api_min / api_max | نطاق API الموثق إن كان معروفاً |
| notes | ملاحظة اختلاف OEM أو parser أو وحدة القياس |

لا تخلط بين Empty وUnsupported وPermissionDenied. غياب الخاصية ليس دليلاً على أن الميزة غير موجودة.

### 3.2 تقسيم التقرير

1. **Identity:** manufacturer/brand/model/product/device/board/hardware.
2. **Build & Android:** release/API/codename/build ID/fingerprint/security patch/build date.
3. **ABI & Features:** ABI lists وPackageManager features/libraries.
4. **Display:** size/density/refresh عند توفر دليل موثوق.
5. **CPU & Memory:** cpuinfo وmeminfo وsysfs best-effort.
6. **Storage:** volumes وfilesystem usage، مع تمييز storage المشترك عن data/system.
7. **Battery & Uptime:** القيم المتغيرة.
8. **System Capabilities:** boot properties وfeatures وservices التي تمت مراجعتها.
9. **Collection Diagnostics:** الأوامر التي نجحت/فشلت والمهلة وسبب الفشل.

## 4. توافق Android 7+ وحدود API

### 4.1 حقائق رسمية مهمة

- Build.VERSION.SDK_INT موجود منذ API 4، وBuild.VERSION.SECURITY_PATCH منذ API 23؛ لذلك كلاهما يقع ضمن الحد الأدنى API 24. مرجع: [Build.VERSION](https://developer.android.com/reference/android/os/Build.VERSION.html).
- Build.SUPPORTED_ABIS متاح منذ API 21؛ يقع ضمن الحد الأدنى API 24. مرجع: [Build](https://developer.android.com/reference/android/os/Build).
- Build.SOC_MANUFACTURER وBuild.SOC_MODEL أضيفا في API 31؛ يجب عدم افتراض توفرهما على Android 7–11. مرجع: [Build](https://developer.android.com/reference/android/os/Build).
- Android system properties ليست schema موحدة ثابتة. توضح وثائق AOSP أن الخصائص قاموس key/value، وأن اختلاف partitions والترقيات قد يجعل الخصائص غير مستقرة؛ ومن Android 10 توجد sysprop definitions/API للتعامل المنظم بين partitions. مرجع: [Implement system properties as APIs](https://source.android.com/docs/core/architecture/configuration/sysprops-apis).
- بدءاً من Android 10 (API 29)، الوصول إلى المعرّفات غير القابلة لإعادة الضبط مثل IMEI والرقم التسلسلي يخضع لقيود إضافية وأذونات مميزة؛ لا نجعلها جزءاً من تقرير الجهاز الافتراضي. مرجع: [Privacy changes in Android 10](https://developer.android.com/about/versions/10/privacy/changes).

### 4.2 جدول سياسة التوافق

| الحقل/المجموعة | API 24–28 | API 29+ | سياسة البرنامج |
|---|---|---|---|
| model/manufacturer/build properties | غالباً متاحة عبر getprop، مع اختلاف OEM | غالباً متاحة عبر getprop، مع اختلاف OEM | best-effort مع provenance |
| SDK/release/security patch | متاحة | متاحة | حقول أساسية |
| ABI lists | متاحة | متاحة | حقول أساسية |
| SoC API fields الرسمية | غير متاحة قبل API 31 | متاحة API 31+ | API-aware؛ لا fallback تخميني |
| hardware serial/IMEI | قد تعود فارغة أو مقيدة حسب السياق | قيود أشد منذ API 29 | لا تجمعها افتراضياً |
| CPU sysfs | تختلف حسب kernel/OEM | تختلف حسب kernel/OEM | optional probes مع حالات فشل |
| sensors/cameras/codecs | framework APIs تختلف حسب API/عتاد | framework APIs تختلف حسب API/عتاد | companion app فقط إذا لزم |
| storage volumes | تختلف طريقة العرض والصلاحيات | قيود scoped storage تؤثر على تطبيقات Android | ADB shell path منفصل عن تطبيق Android |

**ملاحظة:** هذا الجدول إطار تخطيط لا مصفوفة إثبات لكل property. كل حقل خاص يجب أن يثبت على API levels وموديلات الاختبار.

## 5. تدقيق مشاريع GitHub الجاهزة

تمت مراجعة README وشجرة الملفات وملفات المصدر المذكورة أدناه. لا تعني مراجعة المصدر أن المشروع تم بناؤه أو تشغيله على جهاز فعلي في هذه الجولة.

### 5.1 RookieJay/AdbGui — مرشح Host-side أولي

- المستودع: https://github.com/RookieJay/AdbGui
- الفرع الافتراضي وقت المراجعة: master.
- README يصف تطبيق Windows-first مبني Kotlin Multiplatform، مع فصل :core pure Kotlin/JVM عن :desktop Compose Desktop.
- يعلن لوحة Device Info وExport تقرير نصي يحتوي getprop وwm size/density وmeminfo وcpuinfo وbattery وdisk.
- شجرة المصدر تحتوي:
  - core/.../adb/GetpropParser.kt
  - core/.../device/DeviceRepository.kt
  - core/.../adb/CommandRunner.kt
  - core/.../adb/AdbProcessRunner.kt
  - core/.../adb/DeviceListParsers.kt
  - core/src/test/.../GetpropParserTest.kt
- تمت قراءة GetpropParser.kt واختباره:
  - parser يحول brand/manufacturer/model/Android release/SDK/serial/ABI إلى نموذج DeviceProps.
  - الاختبار يغطي الخصائص الموجودة والناقصة.
  - الملاحظة: بناء regex الحالي يضم property key مباشرة داخل regex دون Regex.escape; أسماء الخصائص التي تحتوي . قد تطابق أي حرف بدل النقطة الحرفية. يجب إصلاح ذلك قبل نقل parser أو اعتماد سلوكه.
  - النموذج الحالي محدود؛ لا يغطي fingerprint/security patch/build fields/feature lists/structured provenance.
- يوجد تعارض في README: قسم v1/Next steps يذكر أن Logcat streaming مؤجل، لكن المصدر الحالي يحتوي LogcatController وLogcatRecorder وLogcatViewModel وLogcatScreen واختبارات مرتبطة. وثيقة التشخيص 10 تسجل هذا التعارض وتفصل ما ثبت وجوده في المصدر عن حالة الإصدار المنشور.
- يذكر README أنه auto-selects أول جهاز online. هذا السلوك لا يناسب سياستنا؛ نعيد استخدام الـcore/parsers فقط بعد إزالة أي اختيار تلقائي وربط كل عملية بجهاز محدد.

**الحكم:** أقوى مرشح مبدئي لاقتباس مكونات ADB/Parser واختبارها داخل Host Core، وليس نسخ التطبيق أو سياسة اختيار الجهاز. لا يوجد اعتماد دمج بعد.

### 5.2 muk1m0v/device_info — مرجع Windows سريع

- المستودع: https://github.com/muk1m0v/device_info
- README يحدد Windows 10/11 وPython 3.10+، وADB، وواجهة محلية على 127.0.0.1.
- app/adb.py يحتوي:
  - run_adb(args, timeout) ويستخدم argument list لا shell=True.
  - get_connected_devices() وauthorized_devices().
  - get_device_info(serial) ويحدد الجهاز صراحةً عبر adb -s serial.
  - قراءات read-only لـgetprop وdumpsys battery و/proc/meminfo وdf وwm size.
- نقاط قوة قابلة للاقتباس: process invocation بقائمة arguments، timeout، فصل helper عن الواجهة، اختيار الجهاز بالـserial، وتطبيع حالة البطارية.
- نقاط تمنع نقله كما هو:
  - التقرير الحالي صغير نسبياً وليس Device Information schema متكاملاً.
  - يوجد تكرار ظاهر في قراءة/تحليل /proc/meminfo داخل get_summary.
  - تحليل df يعتمد على تقسيم نصي مبسط وقد يتأثر باختلاف شكل المخرجات.
  - الاعتماد على تثبيت dependencies تلقائياً عند أول تشغيل يحتاج مراجعة منفصلة قبل إدخاله في برنامج offline-first.
- لم نراجع كل الملفات الداخلية ولا نثبت كل وظائف README من خلال تشغيل فعلي.

**الحكم:** مرجع مفيد لتصميم ADB runner على Windows، وليس قاعدة بيانات أو محرك جمع جاهزاً كاملاً.

### 5.3 ashokvarmamatta/ANTAR — مرجع Android-side واسع

- المستودع: https://github.com/ashokvarmamatta/ANTAR
- الفرع الافتراضي وقت المراجعة: prod.
- README يعلن Android 7.0 / API 24+، ويقسم البيانات إلى Device/System/CPU/Battery/Location/Network/Storage/Display/Sensors/Apps/Camera.
- تمت مراجعة شجرة الملفات وملفات DeviceRepositoryImpl.kt وSystemRepositoryImpl.kt وCpuRepositoryImpl.kt وStorageRepositoryImpl.kt.
- نقاط قيمة:
  - تقسيم واضح نسبياً إلى repositories وdomain models وواجهات العرض.
  - يجمع Build fields وSDK/security patch/kernel/runtime وABI وRAM/storage/display/sensors.
  - بعض القراءات مخزنة مؤقتاً لتجنب عمليات متكررة.
  - الكود يوضح عملياً أن بعض القيم مثل serial وMAC لا تكون متاحة على الأجهزة الحديثة.
- حدود وملاحظات:
  - هذا تطبيق Android يعمل داخل الهاتف، وليس مكتبة Host-side لقراءة جهاز آخر عبر ADB.
  - بعض القراءات تعتمد على /sys أو /proc أو خصائص vendor؛ لا تعتبر paths أو أسماء الخصائص موحدة.
  - توجد أجزاء best-effort أو heuristic؛ لا ننقل أي قيمة مستنتجة على أنها حقيقة عامة.
  - بعض عمليات النظام تستخدم Runtime.exec داخل التطبيق؛ لا ننسخ نمط التنفيذ دون توحيد process runner والمهلات.
- الاستخدام المقترح: مرجع للحقول وAndroid API/repository separation، لا ندمج التطبيق نفسه في Windows.

**الحكم:** مرجع تصميمي جيد لكتالوج الحقول على Android API 24+، وليس أساساً لمحرك ADB الخاص بنا.

### 5.4 ScoobyDouche/PhoneDiagnosticTool — مرجع تشخيص أعمق، لا يغطي الحد الأدنى

- المستودع: https://github.com/ScoobyDouche/PhoneDiagnosticTool
- تمت مراجعة README وDeviceInfoCollector وDeviceInfo models وDeepProbes وDumpsysParsers وGradle config.
- README يعلن حقولاً متقدمة: CPU/GPU/Battery/RAM/Storage/Display/Sensors/Cameras/Codecs/Thermals/Network/Security/System Activity، مع تقارير redacted.
- بنية المصدر تفصل DeviceInfoCollector وDeepProbes وDumpsysParsers وReportExporter ونماذج البيانات والاختبارات.
- نقطة قوة: DeepProbes يستقبل دالة قراءة nodes، ما يجعل parsing قابلاً للاختبار ويتيح إظهار مصدر القراءة العادي أو elevated؛ وتوجد اختبارات parsers منفصلة.
- قيد توافق حاسم: minSdk = 26 في app/build.gradle.kts، لذلك التطبيق الحالي لا يغطي Android 7 / API 24 أو Android 7.1 / API 25. لا نعتمده كمرجع وحيد للحد الأدنى.
- بعض الحقول المتقدمة تعتمد على Shizuku/root أو sysfs غير موحد؛ يجب إظهار مستوى الوصول ومصدر القيمة.
- الاستخدام المقترح: انتقاء نماذج البيانات، parser tests، ومفاهيم source provenance؛ لا نسخ collector كاملاً إلى برنامج Windows.

**الحكم:** مرجع انتقائي للميزات المتقدمة واختبار parsers، وليس مرشحاً مباشراً لنسخة Android 7+.

## 6. قرار إعادة الاستخدام

| المكون | القرار المبدئي | السبب |
|---|---|---|
| ADB command runner | اقتباس/تكييف نمط AdbGui أو device_info بعد تدقيق الاختبارات | تنفيذ host-side واضح مع timeouts وargument arrays |
| Device discovery & explicit serial | الاستفادة من AdbGui/device_info مع تعديل auto-select | منع تنفيذ أمر على جهاز غير مقصود |
| getprop parser | اقتباس الفكرة والاختبارات من AdbGui بعد إصلاح regex وتوسيع النموذج | نقطة بداية صغيرة قابلة للاختبار |
| Device information field catalogue | الاستفادة من ANTAR وPhoneDiagnosticTool كمراجع للمصادر والحقول | تغطية واسعة، لكنهما Android-side |
| Advanced sysfs probes | لا نقل مباشر؛ بحث/اختبار لكل probe على حدة | اختلاف OEM/kernel والصلاحيات |
| Report model/export | اقتباس نموذج البيانات والتقارير من PhoneDiagnosticTool بعد فصل الحقول الحساسة | يدعم JSON/text وredaction في المشروع المرجعي |
| Whole application | لا نسخ كامل لأي مرشح حالياً | اختلاف معمارية المنتج واحتياجاته وأمانه |

**لا يوجد مشروع واحد يغطي متطلباتنا كاملة.** الاختيار الأفضل هو اقتباس مكونات صغيرة من Host-side ADB GUI، مع استخدام تطبيقات Android كمرجع للحقول فقط. هذا يقلل إعادة التنفيذ دون ربط المنتج بواجهة أو بنية تطبيق خارجي.

## 7. نموذج أوامر القراءة الأولي

كل أمر يمر من Command Runner موحد مع -s <serial>، timeout، output cap، cancellation، وحالة نتيجة واضحة. هذه أمثلة بحثية وليست قائمة نهائية:

~~~powershell
adb -s <serial> get-state
adb -s <serial> shell getprop ro.product.manufacturer
adb -s <serial> shell getprop ro.product.brand
adb -s <serial> shell getprop ro.product.model
adb -s <serial> shell getprop ro.product.device
adb -s <serial> shell getprop ro.build.fingerprint
adb -s <serial> shell getprop ro.build.version.release
adb -s <serial> shell getprop ro.build.version.sdk
adb -s <serial> shell getprop ro.build.version.security_patch
adb -s <serial> shell getprop ro.product.cpu.abilist
adb -s <serial> shell wm size
adb -s <serial> shell wm density
adb -s <serial> shell pm list features
adb -s <serial> shell pm list libraries
adb -s <serial> shell cat /proc/meminfo
adb -s <serial> shell dumpsys battery
adb -s <serial> shell df -k
adb -s <serial> shell uname -a
~~~

### 7.1 قواعد التنفيذ

- لا تمرر serial فارغاً أو wildcard.
- لا تجمع getprop كاملاً افتراضياً؛ استخدم whitelist مع نسخة من قائمة الخصائص وتاريخها.
- لا تعتبر exit code صفر وحده دليلاً على أن parser استخرج بيانات صحيحة.
- افحص empty output وunknown وnull وحقولاً ناقصة دون استبدالها بتخمين.
- حدود output والوقت وعدد عمليات dumpsys مطلوبة لتجنب التعليق أو ملفات تقارير ضخمة.
- لا تسجل serial أو بيانات تعريف حساسة في log افتراضي؛ يمكن حفظ معرّف جلسة داخلي غير قابل للعكس.
- كل تقرير قابل للتصدير يجب أن يقدم Redaction Preview قبل مشاركة البيانات.

## 8. اختبارات القبول البحثية

| الاختبار | المتوقع |
|---|---|
| لا يوجد جهاز | حالة NoDevice، لا ينفذ أي أمر shell |
| جهاز unauthorized/offline | الحالة واضحة، ولا تُعرض بيانات قديمة |
| جهازان أو أكثر | كل أمر يستخدم serial الذي اختاره المستخدم صراحةً |
| property غير موجودة | Empty/NotReported، لا قيمة مخمّنة |
| SDK property غير رقمية | ParseError/Unknown مع حفظ raw value |
| اختلاف ترتيب/مسافات getprop | parser يقرأ السطر الصحيح دون regex فضفاض |
| مخرجات df مختلفة | parser يحدد الأعمدة والوحدات أو يعلن ParseError |
| timeout أو disconnect | نتيجة جزئية مع status واضح؛ لا نعرض التقرير مكتملاً |
| API 24/25 | لا استدعاء Build APIs غير المتاحة؛ جمع ADB الأساسي يعمل حيثما تسمح الصلاحيات |
| API 29+ | لا محاولة لجمع IMEI/serial العتادي افتراضياً |
| property vendor مفقودة | الحقول الأساسية تظل قابلة للعرض ولا يفشل التقرير كله |
| تصدير تقرير | قيم المصدر والحالة ووقت الجمع موجودة، والحقول الحساسة منقحة افتراضياً |

## 9. مصادر البحث

### مستودعات GitHub التي تمت مراجعتها

- [RookieJay/AdbGui README](https://github.com/RookieJay/AdbGui/blob/master/README.md)
- [AdbGui GetpropParser.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/main/kotlin/com/adbgui/core/adb/GetpropParser.kt)
- [AdbGui GetpropParserTest.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/test/kotlin/com/adbgui/core/adb/GetpropParserTest.kt)
- [AdbGui DeviceRepository.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/main/kotlin/com/adbgui/core/device/DeviceRepository.kt)
- [muk1m0v/device_info README](https://github.com/muk1m0v/device_info/blob/main/README.md)
- [device_info app/adb.py](https://github.com/muk1m0v/device_info/blob/main/app/adb.py)
- [ANTAR README](https://github.com/ashokvarmamatta/ANTAR/blob/prod/README.md)
- [ANTAR DeviceRepositoryImpl.kt](https://github.com/ashokvarmamatta/ANTAR/blob/prod/app/src/main/java/com/ashes/dev/works/system/core/internals/antar/data/repository/DeviceRepositoryImpl.kt)
- [ANTAR SystemRepositoryImpl.kt](https://github.com/ashokvarmamatta/ANTAR/blob/prod/app/src/main/java/com/ashes/dev/works/system/core/internals/antar/data/repository/SystemRepositoryImpl.kt)
- [ANTAR CpuRepositoryImpl.kt](https://github.com/ashokvarmamatta/ANTAR/blob/prod/app/src/main/java/com/ashes/dev/works/system/core/internals/antar/data/repository/CpuRepositoryImpl.kt)
- [ANTAR StorageRepositoryImpl.kt](https://github.com/ashokvarmamatta/ANTAR/blob/prod/app/src/main/java/com/ashes/dev/works/system/core/internals/antar/data/repository/StorageRepositoryImpl.kt)
- [PhoneDiagnosticTool README](https://github.com/ScoobyDouche/PhoneDiagnosticTool/blob/main/README.md)
- [PhoneDiagnosticTool DeviceInfoCollector.kt](https://github.com/ScoobyDouche/PhoneDiagnosticTool/blob/main/app/src/main/java/com/phonediagnostic/data/DeviceInfoCollector.kt)
- [PhoneDiagnosticTool DeviceInfo.kt](https://github.com/ScoobyDouche/PhoneDiagnosticTool/blob/main/app/src/main/java/com/phonediagnostic/data/DeviceInfo.kt)
- [PhoneDiagnosticTool DeepProbes.kt](https://github.com/ScoobyDouche/PhoneDiagnosticTool/blob/main/app/src/main/java/com/phonediagnostic/data/DeepProbes.kt)
- [PhoneDiagnosticTool DumpsysParsers.kt](https://github.com/ScoobyDouche/PhoneDiagnosticTool/blob/main/app/src/main/java/com/phonediagnostic/data/DumpsysParsers.kt)
- [PhoneDiagnosticTool app/build.gradle.kts](https://github.com/ScoobyDouche/PhoneDiagnosticTool/blob/main/app/build.gradle.kts)

### مراجع Android/AOSP الرسمية

- [Build.VERSION API](https://developer.android.com/reference/android/os/Build.VERSION.html)
- [Build API](https://developer.android.com/reference/android/os/Build)
- [Android 10 privacy changes — non-resettable identifiers](https://developer.android.com/about/versions/10/privacy/changes)
- [AOSP — Implement system properties as APIs](https://source.android.com/docs/core/architecture/configuration/sysprops-apis)
- [Android Debug Bridge](https://developer.android.com/tools/adb)

## 10. ما لم يتم إثباته بعد

- لم يتم تشغيل أي مشروع من المرشحين أو اختباره على Windows 10/11 في هذه الجولة.
- لم يتم جمع تقرير من جهاز Android حقيقي API 24 أو أحدث.
- لم يتم إثبات أن كل property في الجداول متاحة على كل OEM أو كل وضع.
- لم يتم اختبار parsers على corpus من getprop/df/dumpsys مأخوذ من أجهزة متعددة.
- لم يتم اعتماد أي كود للدمج أو النسخ إلى المنتج.

**الحالة:** البحث الأولي ومراجعة المصدر تمت. الخطوة التالية هي توسيع تدقيق Host-side parser/command runner، ثم إنشاء corpus مخرجات من مصادر اختبار مصرح بها، وبعدها الانتقال إلى محور التشخيص والسجلات. لا توجد موافقة تنفيذ مطلوبة لأن هذه الجولة توثيق فقط، ولا تتضمن تعديل هاتف أو بناء المنتج.
