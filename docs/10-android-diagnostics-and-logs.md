# Android Diagnostics, Logcat & Bugreport Research

> **الغرض:** بحث منظومة التشخيص والسجلات من Windows عبر ADB، ومراجعة المشاريع الجاهزة التي يمكن اقتباس مكوناتها، مع فصل جمع السجلات عن تحليلها ومشاركتها. هذه وثيقة بحث وتصميم؛ لم تُنفذ عمليات جمع على هاتف فعلي في هذه الجولة.

**آخر مراجعة:** 12 نوفمبر 2026  
**المنصة المستهدفة:** Windows 10/11 وAndroid 7.0 (API 24) فأحدث.  
**القيود:** قراءة فقط افتراضياً؛ لا بحث في Qualcomm أو MediaTek أو التراخيص؛ لا استخراج بيانات شخصية أو حسابات أو رسائل أو سجلات مكالمات.

## 1. الخلاصة التنفيذية

نحتاج إلى محور تشخيص مستقل داخل المنصة، وليس مجرد نافذة Logcat. يتكون من:

1. **Live Logcat Viewer:** عرض مباشر لسجلات الجهاز مع اختيار الجهاز والـbuffer والفلاتر.
2. **Log Recorder:** حفظ السجلات الأصلية إلى ملفات مع rotation وحدود مساحة ومدة احتفاظ.
3. **Log File Analyzer:** فتح ملفات كبيرة، البحث والتصفية والتصدير دون تحميل الملف كله في الذاكرة.
4. **Bugreport Capture:** جمع Android bugreport ZIP عبر ADB مع تقدم ومهلة وإلغاء آمن.
5. **Bugreport Inspector:** فهرسة محتويات ZIP وعرض أقسام dumpstate/dumpsys/logcat وملفات ANR/crash المتاحة.
6. **Targeted Diagnostics:** تشغيل أوامر قراءة محددة مثل dumpsys battery/meminfo وps/service list عند طلب المستخدم.
7. **Report & Redaction:** تقرير محلي قابل للتصدير، مع معاينة الحقول الحساسة قبل المشاركة.

**أفضل نواة Host-side مرشحة حالياً:** مكونات Logcat في [RookieJay/AdbGui](https://github.com/RookieJay/AdbGui)، بعد تدقيق أمان واختبار وتعديل اختيار الجهاز.  
**مرجع تحليل ملفات ضخمة:** [daitouniao/logcat-viewer](https://github.com/daitouniao/logcat-viewer) أو [laomou/LogFilter](https://github.com/laomou/LogFilter)، مع التحقق من المصدر والنسخة قبل أي اقتباس.  
**مرجع وظائف متكامل وليس مرشح دمج كما هو:** [kefengwei/LogCater](https://github.com/kefengwei/LogCater)، بسبب الاختيار التلقائي للجهاز ومسارات MCP/الأوامر التي تحتاج إعادة تصميم.  
**تحليل Bugreport متقدم:** [Perfetto](https://perfetto.dev/docs/getting-started/other-formats) كأداة مساعدة، وليس شرطاً أن نعيد بناء محلل traces من الصفر.

## 2. أنواع البيانات التي يجب دعمها

| النوع | المصدر | ماذا يقدم؟ | السياسة |
|---|---|---|---|
| Live Logcat | adb logcat | أحداث التطبيق والنظام أثناء التشغيل | جلسة محددة بجهاز ووقت وبuffer واضح |
| Logcat dump | adb logcat -d | محتوى buffers الحالي وقت الطلب | Export منفصل عن العرض المباشر |
| Event buffer | logcat -b events | أحداث النظام المنظمة | خيار متقدم، وليس buffer افتراضياً دائماً |
| Radio buffer | logcat -b radio | رسائل radio/telephony عند توفرها | لا جمع تلقائي؛ يتطلب سبباً واضحاً وتحذير خصوصية |
| Crash buffer | logcat -b crash | سجلات الأعطال المتاحة | ضمن جمع التشخيص عند الطلب |
| Bugreport ZIP | adb bugreport | تجميعة تشخيص من النظام تشمل dumpstate/dumpsys/logcat وأقساماً أخرى | ملف حساس؛ يظل محلياً ولا يشارك تلقائياً |
| Dropbox entries | dumpsys dropbox | ملخص سجلات الحوادث/ANR/WTF المتاحة | عرض metadata أولاً؛ تحميل التفاصيل عند اختيار المستخدم |
| Targeted dumpsys | dumpsys <service> | حالة خدمة نظام محددة | خدمة واحدة في كل طلب؛ لا dumpsys شامل تلقائياً |
| Process snapshot | ps -A / dumpsys meminfo | العمليات وبيانات الذاكرة المتاحة | قراءة snapshot فقط، لا قتل أو تغيير أولوية |
| Service inventory | dumpsys -l / service list | أسماء خدمات النظام وواجهاتها المعلنة | فهرس معلومات، لا يعني أن كل خدمة قابلة للاستعلام بحرية |

مرجع Android الرسمي يوضح أن bugreport يحتوي على بيانات dumpsys وdumpstate وlogcat، وأن logcat يعرض سجلات النظام والتطبيقات.  
- https://source.android.com/docs/core/tests/debug/read-bug-reports
- https://developer.android.com/tools/logcat
- https://developer.android.com/studio/debug/bug-report

## 3. متطلبات Logcat

### 3.1 العرض المباشر

الأمر الأساسي المقترح:

~~~powershell
adb -s <serial> logcat -v threadtime
~~~

أوامر قراءة إضافية عند اختيار buffer:

~~~powershell
adb -s <serial> logcat -b main -v threadtime
adb -s <serial> logcat -b system -v threadtime
adb -s <serial> logcat -b crash -v threadtime
adb -s <serial> logcat -b events -v threadtime
adb -s <serial> logcat -b radio -v threadtime
~~~

يجب ألا نفترض أن كل buffer متاح أو ممتلئ على كل جهاز. يعرض البرنامج فشل/عدم توفر buffer بدلاً من إظهار شاشة فارغة بلا تفسير.

### 3.2 الفلاتر المطلوبة

- Level: Verbose / Debug / Info / Warning / Error / Fatal.
- Tag include/exclude.
- Message text search، مع دعم regex اختياري وحدود على تعقيده.
- PID / TID.
- Package filter عبر resolve PID من الجهاز؛ يجب إعادة حل PID لأن التطبيق قد يعيد التشغيل أو يغير PID.
- Time range.
- Buffer selector.
- Saved filter presets.
- Bookmarks/marks على الأسطر المهمة.
- Copy raw line / Copy selected / Export filtered results.

**لا نحذف الأسطر التي لا ينجح parser في فهمها.** Stack traces والـcontinuation lines وعلامات بداية buffer يجب أن تبقى قابلة للعرض والتصدير حتى لو لم تنتج حقول PID/Tag/Level.

### 3.3 الذاكرة والتسجيل

- واجهة العرض تستخدم ring buffer محدوداً بالعدد/الذاكرة.
- التسجيل إلى القرص مسار منفصل عن ring buffer؛ لا نفقد السجل الأصلي عندما تمتلئ الذاكرة.
- الكتابة تكون streaming مع backpressure، لا نجمع stream كله في RAM.
- ملفات التسجيل تتضمن الجهاز ووقت البدء/التوقف وbuffer والأمر/الإصدار المستخدم.
- تدوير الملفات عند حد حجم قابل للضبط.
- سقف إجمالي للمساحة، وتنظيف حسب retention policy واضحة.
- عند انقطاع الجهاز: يوقف التسجيل أو يعيد الاتصال وفق إعداد صريح؛ لا يخلط جلسات جهازين في ملف واحد.
- لا نسجل pairing codes أو tokens أو بيانات حساسة في logs الخاصة ببرنامج Windows.

### 3.4 Clear ليس عملية عرض عادية

الأمر adb logcat -c يمسح ring buffer على الجهاز. لذلك:
- زر Clear الافتراضي يمسح العرض المحلي فقط.
- مسح buffer على الهاتف إجراء مستقل باسم واضح وتحذير وموافقة.
- لا نشغله تلقائياً عند بدء جلسة أو عند تصدير السجل.

## 4. Bugreport Capture & Inspection

### 4.1 الالتقاط

الأمر الأساسي:

~~~powershell
adb -s <serial> bugreport <destination-directory>
~~~

- عملية طويلة وقد تستغرق عشرات الثواني أو أكثر؛ لا تعاملها كأمر ADB قصير.
- نعرض مراحل: Preparing / Capturing / Pulling / Validating / Complete.
- مهلة قابلة للضبط مع قيمة افتراضية عملية، وCancel يحاول إنهاء عملية المضيف بأمان.
- بعد انتهاء ADB، نتحقق من وجود ZIP فعلي وحجمه وسلامة بنية الأرشيف.
- لا نعلن نجاحاً بناءً على exit code فقط؛ يجب التحقق من الملف.
- نحتفظ باسم الملف الأصلي ونربط التقرير بـDevice Session ID ووقت الالتقاط.

### 4.2 محتويات التقرير

بحسب وثائق Android، ملف bugreport يتضمن بيانات dumpstate وdumpsys وlogcat، وقد يحتوي ZIP على الملف النصي الرئيسي وملفات إضافية. محتوى الملف قد يتضمن stack traces وبيانات تطبيقات وخدمات نظام.

التحليل الأولي المطلوب:

1. قراءة manifest/metadata إن وجدت، مثل version.txt وmain_entry.txt.
2. اكتشاف الملف النصي الرئيسي دون افتراض اسم ثابت واحد.
3. فهرسة الأقسام والعناوين دون تحميل كامل التقرير إلى الذاكرة.
4. إظهار أقسام DUMPSYS وSYSTEM LOG وEVENT LOG وDROPBOX وغيرها عندما تكون موجودة.
5. اكتشاف ملفات ANR/tombstone/trace عند وجودها.
6. حفظ offset/line index لفتح القسم بسرعة.
7. تمييز قسم غير موجود عن قسم فارغ أو غير قابل للتحليل.

لا نفترض أن جميع أجهزة Android أو الشركات تنتج ZIP متطابقاً. يجب أن يتحمل parser اختلاف أسماء الملفات وترتيب الأقسام وتكرارها.

### 4.3 أمان ملفات ZIP

- حدود لحجم ZIP والحجم المفكوك وعدد الملفات وعمق المسارات.
- منع Zip Slip وZip Bomb والروابط الرمزية.
- لا نفك الأرشيف كاملاً إلى مجلد غير محكوم.
- قراءة streaming للأقسام الكبيرة.
- لا نعرض ملفات داخلية حساسة تلقائياً.
- التقرير المحلي لا يرفع إلى أي خدمة خارجية دون طلب صريح.

## 5. تحليل السجلات والـBugreport

### 5.1 مستويات التحليل

| المستوى | الوظيفة | متطلبات |
|---|---|---|
| L0 — Viewer | فتح/بحث/تصفية/تصدير | parser سريع وfile index |
| L1 — Heuristics | تمييز crash/ANR/Fatal/low-memory/reboot/thermal | قواعد قابلة للاختبار مع إظهار السطر المصدر |
| L2 — Correlation | ربط الأحداث الزمنية عبر logcat وdropbox وdumpsys وbattery history | توحيد timestamps ومصادرها |
| L3 — Deep trace | تحليل Perfetto/trace والـsystem performance | أداة متخصصة أو integration مستقل |

لا نعرض استنتاجاً مثل "سبب العطل" كحقيقة إذا كان مجرد pattern match. يجب أن نعرض:
- ما الذي تم اكتشافه.
- السطر/القسم الذي يدعمه.
- نوع القاعدة: exact marker / heuristic / correlated evidence.
- درجة الثقة وحدودها.

### 5.2 Perfetto — إعادة استخدام أداة متخصصة

وثائق Perfetto الرسمية تذكر إمكانية فتح Android bugreport ZIP وتحليل أقسام dumpstate وlogcat وbattery statistics ضمن Trace Processor، بدلاً من كتابة محلل trace جديد. هذا مرشح قوي كتكامل اختياري لميزات الأداء والـtrace، وليس بديلاً عن Logcat Viewer أو bugreport file browser.

- https://perfetto.dev/docs/getting-started/other-formats
- https://perfetto.dev/

### 5.3 Bugreport_analyzer — محلل Python متخصص

- المستودع: https://github.com/VenkateshJallu/Bugreport_analyzer
- README يصف CLI يقبل ملف أو أكثر من bugreport text files، وينشئ تقارير HTML ورسوم PNG للبطارية وANR/crash وLMK والحرارة وwakelocks، مع مقارنة بين أكثر من تقرير.
- تمت مراجعة الملف الرئيسي bugreport_analyzer.py؛ حجمه كبير (script واحد طويل) ويحتوي parsing rules منفصلة لأجزاء البطارية وANR وcrash والحرارة.
- لا نعتبره ZIP ingestion جاهزاً: الواجهة المعلنة تتعامل مع bugreport .txt، وبالتالي يلزم استخراج الملف الرئيسي من ZIP أولاً.
- لم يتم تدقيق كل parser أو إنشاء test corpus في هذه الجولة؛ لا ننقل الملف كاملاً ولا نعتمد استنتاجاته قبل اختبارات على تقارير متعددة من Android 7+.

**الحكم:** مرجع لخوارزميات تحليل محددة، وليس المحرك العام للمنصة.

## 6. تدقيق مشاريع GitHub الجاهزة

### 6.1 RookieJay/AdbGui — أفضل مرشح أولي لمكونات Logcat Core

المستودع: https://github.com/RookieJay/AdbGui  
الفرع الافتراضي وقت المراجعة: master.

تمت مراجعة ملفات المصدر والاختبارات التالية:
- core/.../device/LogcatController.kt
- core/.../device/LogcatRecorder.kt
- core/.../device/LogcatRecordingManager.kt
- core/.../device/LogcatRecordingMaintenance.kt
- core/.../adb/LogcatLineParser.kt
- core/.../adb/CommandRunner.kt
- desktop/.../ui/LogcatViewModel.kt
- desktop/.../ui/LogcatScreen.kt
- اختبارات LogcatController/Recorder/RecordingManager/Maintenance/Parser.

**ما وجدناه في المصدر:**
- Live stream عبر ADB مع تحديد serial.
- ring buffer محدود للعرض، وفلاتر Level/Package/PID/Tag/Text/Regex.
- publish throttling لتقليل تحديث الواجهة تحت تدفق كثيف.
- Manual recording وauto-recording، مع reconnect عند انقطاع stream.
- file rotation وحد إجمالي للمساحة وretention وتنظيف ملفات قديمة.
- Export كامل من logd buffer منفصل عن ring buffer المعروض.
- واجهة Logcat وViewModel وCore tests موجودة فعلياً في شجرة المصدر.
- CommandRunner يحتوي bugreport capture بمهلة 180 ثانية ومحاولة استخراج مسار ZIP من stdout أو destination directory.

**تعارض يجب توثيقه:** README في نفس الفرع يذكر أن Logcat streaming وfile push/pull مؤجلة في قسم v1/Next steps، بينما شجرة المصدر تحتوي Controller/Recorder/ViewModel/Screen واختبارات Logcat. لذلك README لا يعكس كامل حالة المصدر الحالي. لا نعتمد README وحده لإثبات حالة الميزة، ويجب فحص wiring والبناء قبل أي اقتباس.

**ملاحظات تمنع النقل دون مراجعة:**
- توجد دالة fixLogcatDisabled تستخدم setprop ثم stop/start logd؛ هذه عملية تغيير على الجهاز ولا تدخل في الجمع الافتراضي. لا تعرض كإجراء تلقائي.
- bugreport method موجودة في CommandRunner، لكن لم يظهر في شجرة الاختبارات ملف اختبار واضح مخصص لها؛ نحتاج اختباراتنا الخاصة.
- نحتاج مراجعة stream cancellation وtimeouts وoutput limits في مسارات كل من العرض والتسجيل والتصدير قبل النقل.
- لا ننسخ UI؛ نركز على Core وparsers وtests ونوصلها إلى Service Layer الخاص بنا.

**الحكم:** المرشح الأول لاستخراج/اقتباس Logcat Core بعد بوابة مراجعة؛ ليس اعتماداً نهائياً ولا إثباتاً لاختبار جهاز.

### 6.2 daitouniao/logcat-viewer — مرجع Windows للأحجام الكبيرة

المستودع: https://github.com/daitouniao/logcat-viewer  
الفرع الافتراضي وقت المراجعة: master.

README يصف تطبيقين Windows من solution واحدة:
- Log viewer يستخدم memory-mapped files وcolumnar index وvirtualized list لملفات ضخمة.
- Android toolbox منفصل للـlive ADB capture وscreenshots وscreen recording وtwo-pane file transfer وAPK install/uninstall وcommand window.

تمت مراجعة شجرة الملفات وبعض الملفات:
- LogcatStream.cs
- LogParser.cs
- AdbManager.cs
- LogParserTests.cs
- FileBrowserDialog.cs وDeviceFilePane.cs وLocalFilePane.cs
- CommandDialog.cs

نقاط قوة:
- فصل viewer عن toolbox ومكتبة مشتركة.
- parsing لعدة log formats وفهرسة ملفات كبيرة دون تحميلها بالكامل.
- اختبار parser على تنسيقات مختلفة وedge cases.
- device selection component واختبارات مرتبطة به.
- file browser وtransfer tests موجودة في شجرة الاختبارات.

قيود:
- README يذكر أن GitHub mirror قد يتأخر عن مستودع Gitee الأساسي؛ لذلك لا نفترض أن GitHub يعكس أحدث إصدار upstream.
- solution كبيرة ومترابطة مع WinForms وواجهة toolbox؛ اقتباس أجزاء محددة أفضل من نقل التطبيق كاملاً.
- وجود command window يعني ضرورة تدقيق حدود الأوامر قبل أي ربط MCP.
- لم يتم بناء المشروع أو اختبار UI/device integration في هذه الجولة.

**⚠️ تصحيح 13 نوفمبر 2026 — ترقية من "مرجع أداء" إلى أعلى قيمة اقتباس:** المراجعة الفعلية للشجرة كشفت ما لم تسجّله هذه الوثيقة:

- مبني على **`net10.0-windows`** (نفس runtime المعتمد في [ADR-0001](17-adr-0001-stack-decision.md)) لا على Rust كما يوحي ذكره مع LogFilter/Sherlog.
- يستخدم **`AdvancedSharpAdbClient 3.6.16`** — نفس المكتبة المعتمدة كـADB Core.
- حل من 4 مشاريع بينها **`logcat.UiCore`** كمكتبة مشتركة تحمل `Services/AdbManager.cs` (636 سطراً): تعداد أجهزة، `DeviceMonitor`، self-heal لـadb server، push/pull، screenshots، root detection.
- **852 اختباراً، 94.26% تغطية سطور / 88.64% branches** على `Services`+`Models` (أرقام مُعلنة في `docs/TESTING.md`، لم نُشغّلها).
- رخصة **Apache-2.0** مع `THIRD-PARTY-NOTICES.md` و`DISCLAIMER.md`.

**فجوة حرجة مكتشفة:** `AdbManager.Shell(serial, cmd, timeoutSec = 30)` يستقبل مهلة **ولا يستخدمها في جسم الدالة**؛ المهلة المنفَّذة فعلاً تخص `RootProbeTimeoutSec` في دالة أخرى. كما أن `Shell` يأخذ الأمر كنص واحد ويُعيد `string` بلا `problemCode`. لذلك: **يُقتبس النمط والبنية، ولا يُقتبس `Shell` كما هو** — يلزم wrapping بـ`CancellationToken` وإرجاع `Result`.

**الحكم المصحَّح:** **أعلى قيمة اقتباس بين كل المرشحين** (كود C# ناضج بنفس الستاك)، بشرط المراجعة الانتقائية. التفاصيل والاستثناءات في [20 §2.1](20-source-revalidation-round2.md).

### 6.3 kefengwei/LogCater — ميزات كثيرة لكن سياسة اختيار الجهاز غير مناسبة

> **إضافة 13 نوفمبر 2026:** الشجرة تحتوي `src/mcp/` — أي أن LogCater يضم **خادم MCP داخلياً**، وهو ما لم تذكره هذه الوثيقة. يُضاف كمرجع لتصميم MCP فقط، مع نفس التحذير: لا نقل auto-device ولا generic shell. التفاصيل في [20 §2.2](20-source-revalidation-round2.md).

المستودع: https://github.com/kefengwei/LogCater  
الفرع الافتراضي وقت المراجعة: master.

README يعلن Windows C++20 tool يحتوي:
- Live Logcat.
- Dropbox viewer لعرض crash/ANR/WTF entries.
- File browser.
- App info وPID-to-process mapping.
- MCP server عبر loopback/stdio.
- إدارة ملفات وأوامر ADB.

تمت مراجعة:
- src/adb/LogcatReader.cpp
- src/adb/DropboxReader.cpp
- src/ui/FileBrowserPanel.cpp
- src/adb/DeviceManager.cpp
- src/core/AdbProcess.cpp
- src/mcp/McpTools.cpp وMcpTools.h

نقاط يمكن دراستها:
- LogcatReader يفصل stream عن LogBuffer.
- DropboxReader يستخدم dumpsys dropbox list/detail.
- MCP tools مبنية على schemas وJSON responses.
- File browser يوضح تدفق ADB list/read/pull/push.

**موانع إعادة الاستخدام كما هو:**
- DeviceManager ينفذ mDNS discovery ثم auto-connect، ويختار أول جهاز online تلقائياً عند غياب اختيار صالح.
- resolveSerial في MCP قد يستخدم selected device أو الجهاز الوحيد المتصل تلقائياً؛ هذا لا يطابق قاعدة explicit device targeting لدينا.
- FileBrowserPanel يحلل نص ls -la بقواعد نصية؛ يحتاج corpus ومراجعة أسماء الملفات والمسافات وlocale.
- وجود command window/MCP tool surface يستلزم تدقيقاً لكل أداة وحدود أوامرها؛ لا ننسخ generic command execution إلى MCP.
- لم تتم مراجعة كل الاختبارات أو تنفيذ البناء.

**الحكم:** مصدر لأفكار ومكونات منفصلة فقط؛ لا نعتمده كقاعدة MCP أو Device Manager.

### 6.4 LogFilter — مرشح إضافي للـRust viewer

المستودع: https://github.com/laomou/LogFilter  
README يعلن:
- فتح ملفات log كبيرة بذاكرة محدودة وsparse disk index.
- live ADB streaming مع اختيار الجهاز وcommand presets.
- تسجيل دائم إلى ملفات مع preview محدود.
- دعم تنسيقات logcat متعددة وفلاتر PID/TID/Tag/Level/Message.
- حفظ النتائج والـbookmarks.

لم تتم مراجعة المصدر الداخلي أو الاختبارات في هذه الجولة؛ يبقى مرشحاً للمقارنة قبل اتخاذ قرار.

### 6.5 Sherlog — مرجع لتحليل ملفات Logcat فقط

المستودع: https://github.com/A-tafaj/Sherlog

README يصف desktop viewer لملفات Logcat كبيرة، مع parsing/filter/search/export وmulti-file tabs، لكنه لا يعلن live ADB capture في MVP. يصلح كمرجع file parser/index/filter، وليس كطبقة اتصال أو تسجيل مباشر.

## 7. قرار إعادة الاستخدام

| المكون | القرار المبدئي | المصدر الأفضل للبدء |
|---|---|---|
| Live Logcat controller | اقتباس انتقائي بعد مراجعة stream/cancellation | AdbGui LogcatController |
| Logcat parser + tests | اقتباس الفكرة وتوسيع formats/continuation lines | AdbGui + Logcat Viewer + LogFilter |
| Recorder/rotation/retention | مرشح قوي لإعادة استخدام التصميم وبعض Core code | AdbGui LogcatRecorder/RecordingManager/Maintenance |
| Huge-file viewer/index | دراسة تصميم/خوارزميات واختبارات، لا نسخ UI | logcat-viewer / LogFilter / Sherlog |
| Bugreport capture | إعادة استخدام ADB command flow بعد اختبار progress/path/error | AdbGui CommandRunner.bugreport |
| Bugreport ZIP inspection | استخدام ZIP reader آمن + فهرس streaming؛ لا parser خاص بكل OEM من البداية | AOSP format docs + Perfetto integration |
| Crash/ANR heuristics | اقتباس قواعد محددة بعد corpus tests | Bugreport_analyzer + LogCater Dropbox patterns |
| Dropbox viewer | مرجع قراءة list/detail فقط | LogCater DropboxReader |
| MCP diagnostics | بناء tools محددة فوق Service Layer، لا نقل generic shell/auto-device resolution | لا اعتماد مباشر على LogCater MCP |

**القرار:** نبدأ باستخراج مكونات Logcat Core من AdbGui كمرشح، ونستخدم Perfetto كتكامل اختياري لتحليل trace/bugreport. أما Logcat Viewer وLogFilter وSherlog فهي مراجع أداء وملفات كبيرة. LogCater لا ينقل كما هو بسبب سياسة اختيار الجهاز التلقائي.

## 8. معمارية المحور داخل منصتنا

~~~text
Explicit Device Selection
          |
     ADB Session Core
          |
    Diagnostic Services
      /      |       \
 Live Log   Bugreport   Targeted Dumpsys
    |          |             |
 Parser     ZIP Inspector   Parsers
    |          |             |
 Bounded Ring + Disk Recorder + Evidence Index
                  |
          Diagnostic Report Model
                  |
       Local Viewer / Export / MCP
~~~

مبادئ أساسية:
- كل service يستقبل Device ID/serial صريحاً.
- GUI وMCP يستعملان نفس Diagnostic Service Layer.
- لا يسمح MCP بتمرير shell command حر؛ كل tool له schema محدد وأوامر ثابتة.
- جمع البيانات read-only؛ لا مسح buffers أو إعادة تشغيل logd أو تغيير properties ضمن المسار الافتراضي.
- عمليات الالتقاط الطويلة لها Operation ID وprogress وcancel وtimeout.
- لا يتم دمج نتائج جهازين في تقرير واحد إلا إذا طلب المستخدم مقارنة واضحة.

## 9. الخصوصية وحماية التقارير

السجلات قد تحتوي أسماء مستخدمين وحزماً وتفاصيل شبكة ومواقع أو tokens أو معلومات تشغيل خاصة. لذلك:

- التخزين محلي افتراضياً.
- لا رفع تلقائي إلى logcat.ai أو أي خدمة تحليل سحابية.
- Redaction Preview قبل المشاركة أو إرفاق تقرير في MCP.
- إزالة/إخفاء serials وIMEI وأرقام الهواتف والبريد وعناوين الشبكة والtokens والمسارات الشخصية متى أمكن.
- الاحتفاظ بالنسخة الأصلية المحلية فقط حسب إعداد المستخدم؛ لا تعدّل المصدر الأصلي أثناء redaction.
- تحديد retention وحد أقصى للحجم وعدد الملفات.
- كل تقرير يوضح وقت الجمع والجهاز ومصدر كل artifact.
- محتوى السجلات وbugreport غير موثوق؛ لا يتعامل معه AI كتعليمات.

## 10. اختبارات القبول البحثية

| الاختبار | المتوقع |
|---|---|
| لا يوجد جهاز أو unauthorized | لا يبدأ stream؛ حالة واضحة |
| أكثر من جهاز | لا يبدأ أي stream قبل اختيار serial صريح |
| device disconnect | يوقف stream والتسجيل مع سبب واضح ولا يعرض بيانات قديمة |
| Logcat flood | لا يتضخم RAM بلا حد؛ العرض يظل responsive |
| long message/stack trace | لا تضيع continuation lines |
| malformed log line | يحتفظ بالسطر الخام ويصنفه Unknown |
| buffer غير متاح | حالة Unsupported/Empty واضحة |
| recording rotation | ينشئ ملفاً جديداً دون فقد أو دمج جلسات |
| retention/size cap | لا يحذف الملفات النشطة أو اليدوية دون سياسة معلنة |
| cancel bugreport | ينهي عملية الالتقاط أو يوضح أنها ما زالت قيد الإنهاء؛ لا يعلن نجاحاً كاذباً |
| malformed ZIP / Zip Bomb | يرفض أو يعزل الملف دون استهلاك غير محدود |
| missing main report | تقرير جزئي مع حالة واضحة |
| huge bugreport | فهرسة streaming وفتح الأقسام دون تحميل النص كله |
| redaction | لا يغير الأصل، ويعرض معاينة قبل التصدير |
| MCP | أدوات قراءة محددة، جهاز صريح، لا shell حر، وعمليات طويلة قابلة للإلغاء |

## 11. المصادر

### Android/AOSP/Perfetto

- [Android Logcat command-line tool](https://developer.android.com/tools/logcat)
- [Android Debug Bridge](https://developer.android.com/tools/adb)
- [AOSP — Read bug reports](https://source.android.com/docs/core/tests/debug/read-bug-reports)
- [Android Studio — Capture and read bug reports](https://developer.android.com/studio/debug/bug-report)
- [Perfetto — Other trace formats and Android bugreports](https://perfetto.dev/docs/getting-started/other-formats)

### GitHub projects

- [RookieJay/AdbGui README](https://github.com/RookieJay/AdbGui/blob/master/README.md)
- [AdbGui LogcatController.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/main/kotlin/com/adbgui/core/device/LogcatController.kt)
- [AdbGui LogcatRecorder.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/main/kotlin/com/adbgui/core/device/LogcatRecorder.kt)
- [AdbGui LogcatRecordingManager.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/main/kotlin/com/adbgui/core/device/LogcatRecordingManager.kt)
- [AdbGui LogcatRecordingMaintenance.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/main/kotlin/com/adbgui/core/device/LogcatRecordingMaintenance.kt)
- [AdbGui LogcatLineParser.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/main/kotlin/com/adbgui/core/adb/LogcatLineParser.kt)
- [AdbGui CommandRunner.kt](https://github.com/RookieJay/AdbGui/blob/master/core/src/main/kotlin/com/adbgui/core/adb/CommandRunner.kt)
- [AdbGui LogcatController tests](https://github.com/RookieJay/AdbGui/blob/master/core/src/test/kotlin/com/adbgui/core/device/LogcatControllerTest.kt)
- [AdbGui LogcatRecorder tests](https://github.com/RookieJay/AdbGui/blob/master/core/src/test/kotlin/com/adbgui/core/device/LogcatRecorderTest.kt)
- [daitouniao/logcat-viewer README](https://github.com/daitouniao/logcat-viewer/blob/master/README.md)
- [logcat-viewer LogParser.cs](https://github.com/daitouniao/logcat-viewer/blob/master/logcat-viewer/Services/LogParser.cs)
- [logcat-viewer LogcatStream.cs](https://github.com/daitouniao/logcat-viewer/blob/master/logcat-viewer/Services/LogcatStream.cs)
- [logcat-viewer LogParserTests.cs](https://github.com/daitouniao/logcat-viewer/blob/master/tests/logcat.Tests/LogParserTests.cs)
- [laomou/LogFilter](https://github.com/laomou/LogFilter)
- [A-tafaj/Sherlog](https://github.com/A-tafaj/Sherlog)
- [kefengwei/LogCater README](https://github.com/kefengwei/LogCater/blob/master/README.md)
- [LogCater LogcatReader.cpp](https://github.com/kefengwei/LogCater/blob/master/src/adb/LogcatReader.cpp)
- [LogCater DropboxReader.cpp](https://github.com/kefengwei/LogCater/blob/master/src/adb/DropboxReader.cpp)
- [LogCater DeviceManager.cpp](https://github.com/kefengwei/LogCater/blob/master/src/adb/DeviceManager.cpp)
- [LogCater MCP tools](https://github.com/kefengwei/LogCater/blob/master/src/mcp/McpTools.cpp)
- [VenkateshJallu/Bugreport_analyzer](https://github.com/VenkateshJallu/Bugreport_analyzer)

## 12. ما لم يتم إثباته

- لم يتم تشغيل أي مشروع من المرشحين أو بناءه في هذه الجولة.
- لم يتم جمع Logcat أو Bugreport من هاتف حقيقي.
- لم يتم إثبات توافق parser على جميع صيغ logcat أو تقارير OEM.
- لم يتم تدقيق كل أدوات MCP في LogCater أو كل أوامر command window في المشاريع الأخرى.
- لم يتم اعتماد كود للدمج.

**الحالة:** البحث ومراجعة المصدر تمت على المرشحين المحددين؛ لا يوجد اعتماد دمج أو اختبار Hardware. محور إدارة الملفات موثق بالفعل في [09](09-device-file-management.md). الخطوة التالية هي Source Revalidation لمرشحي Logcat/Bugreport، وبناء corpus منقح واختبار ZIP/streaming ضمن خطة [12](12-REUSE-AND-IMPLEMENTATION-PLAN.md)، دون الخلط بين السجلات وصلاحيات root أو بيانات التطبيقات الخاصة.
