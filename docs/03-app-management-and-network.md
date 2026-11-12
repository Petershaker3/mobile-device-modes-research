# إدارة تطبيقات Android والشبكة عبر ADB

> يغطي إدارة التطبيقات عبر USB/Wireless ADB، APK/APKS وABI، DNS Ad Blocking، وبيانات تواريخ التثبيت والتحديث ومصدر التطبيق.

## 16. محور إدارة التطبيقات عبر ADB — USB وWireless

**نطاق المحور:** إدارة التطبيقات المثبتة على أجهزة Android من Windows عبر ADB القياسي، سواء اتصال USB أو اتصال TCP/IP/Wireless Debugging. يشمل الاكتشاف والبحث والتفاصيل والتثبيت والإزالة والتعطيل والاستعادة وفحص توافق ملفات APK. لا يشمل تجاوز حماية الجهاز أو إزالة حسابات أو تنفيذ عمليات صيانة/تفليش.

### 16.1 المتطلبات الوظيفية التي سنقيسها

| الوظيفة | السلوك المطلوب | ملاحظات التحقق |
|---|---|---|
| اكتشاف الأجهزة | عرض كل أجهزة ADB المتصلة، USB وWi-Fi، مع حالة الاتصال والتفويض | تمييز unauthorized وoffline عن الجهاز المتاح |
| تعدد الأجهزة | اختيار Serial صريح لكل عملية | منع تنفيذ أمر على جهاز غير مقصود |
| قائمة التطبيقات | عرض اسم الحزمة، اسم التطبيق إن أمكن، System/User، Enabled/Disabled، الإصدار والحجم/المسار عند توفره | لا نفترض أن كل البيانات متاحة دون صلاحيات إضافية |
| البحث والتصفية | بحث فوري بالاسم وPackage ID، وفلاتر System/User/Enabled/Disabled/Uninstalled | دعم اختيار متعدد وعمليات Batch |
| تفاصيل التطبيق | Package ID، Version name/code، min/target SDK عند إمكان استخراجها، مسار APK، Installer، UID، حالة التثبيت | تمييز البيانات المستخرجة من الجهاز عن بيانات تحليل ملف APK |
| التثبيت | APK مفرد، إعادة تثبيت/تحديث، تثبيت عدة تطبيقات | عرض نتيجة كل ملف على حدة |
| Split APK | تثبيت مجموعة base + config/feature splits متوافقة | لا تعامل ملف APKS كأنه APK مفرد |
| الإزالة | Uninstall لتطبيق المستخدم، وإزالة تطبيق النظام للمستخدم الحالي عند دعم النظام | إزالة تطبيق النظام للمستخدم لا تعني حذف ملفه من قسم النظام |
| التعطيل والاستعادة | Disable/Enable أو Uninstall for user/Restore بحسب إصدار Android | عرض تحذير قبل تعطيل حزمة نظام |
| النسخ الاحتياطي | سحب APK أو مجموعة split APKs من الجهاز | لا تخلط نسخ APK مع نسخ بيانات التطبيق الخاصة |
| Wireless ADB | اتصال عبر Pairing/Connect على Android 11+، ومسار TCP/IP للأجهزة الأقدم عند دعمه | حفظ حالة الاقتران بأمان وعدم افتراض بقاء عنوان IP ثابتاً |
| سجل العمليات | تسجيل الأمر والنتيجة والجهاز والوقت | إخفاء أي بيانات تعريفية غير لازمة |

### 16.2 أوامر ADB القياسية التي ينبغي أن تدعمها الأداة

الأوامر التالية مرجع لتقييم التنفيذ، وليست دعوة لتنفيذها تلقائياً على أي جهاز:

- اكتشاف الأجهزة: `adb devices -l`، مع استخدام `adb -s <serial>` لتحديد الجهاز في كل عملية.
- عرض الحزم: `adb shell pm list packages` مع المرشحات `-3` لتطبيقات الطرف الثالث، و`-s` لتطبيقات النظام، و`-d` للمعطلة، و`-e` للمفعلة، و`-i` لعرض مصدر التثبيت.
- تفاصيل الحزمة: `adb shell dumpsys package <package>`، و`adb shell pm path <package>` لمسارات ملفات APK.
- تثبيت APK: `adb install <file.apk>`، والتحديث مع `-r` عند الحاجة.
- تثبيت Split APKs: `adb install-multiple <base.apk> <split.apk>...`.
- إلغاء التثبيت: `adb uninstall <package>`، أو أوامر Package Manager مع تحديد المستخدم عند الحاجة.
- Wireless Debugging: استخدام `adb pair <ip>:<pairing-port>` ثم `adb connect <ip>:<port>` على الأجهزة التي تدعم آلية الاقتران الحديثة. لا تخلط منفذ الاقتران بمنفذ الاتصال.

**مصدر مرجعي:** توثيق Android الرسمي لـADB يشرح install وinstall-multiple وPackage Manager، كما يشرح مرجع ADB man page خيارات التثبيت المتعددة.  
[Android Debug Bridge](https://developer.android.com/tools/adb) — [ADB command reference](https://android.googlesource.com/platform/packages/modules/adb/+/refs/heads/main/docs/user/adb.1.md).

### 16.3 فحص معمارية الجهاز وتوافق APK

يجب أن تعرض الأداة قدرات الجهاز الفعلية، لا أن تخمّنها من اسم المعالج أو الشركة:

| الخاصية | مصدر الاستعلام/الفحص | الاستخدام |
|---|---|---|
| Primary ABI | `ro.product.cpu.abi` | المعمارية الأساسية للنظام |
| قائمة ABI المدعومة | `ro.product.cpu.abilist` | قائمة المعماريات التي يستطيع النظام تشغيلها |
| 32-bit ABIs | `ro.product.cpu.abilist32` | دعم تشغيل مكتبات 32-bit إن كانت الخاصية متاحة |
| 64-bit ABIs | `ro.product.cpu.abilist64` | دعم تشغيل مكتبات 64-bit إن كانت الخاصية متاحة |
| Android API level | `ro.build.version.sdk` | مقارنة minSdkVersion مع مستوى النظام |
| Android release | `ro.build.version.release` | عرض نسخة Android للمستخدم |
| Screen density / locale | مواصفات الجهاز أو bundletool device spec | التحقق من split APKs الخاصة بالكثافة واللغة |

المعمارية ليست مجرد x86 مقابل ARM؛ من القيم الشائعة:
- `arm64-v8a`
- `armeabi-v7a`
- `armeabi` (قديم)
- `x86_64`
- `x86`

**منطق فحص APK المقترح:**
1. تحليل AndroidManifest لاستخراج package name وversion وminSdkVersion وtargetSdkVersion.
2. فحص مجلد `lib/` داخل APK لاكتشاف المكتبات الأصلية لكل ABI.
3. إذا لم توجد مكتبات native، فلا ترفض الملف بسبب عدم وجود ABI مطابق؛ قد يكون التطبيق Java/Kotlin ولا يحتوي على native libraries.
4. إذا احتوى الملف على مكتبات native، قارن ABI الموجودة بقائمة ABI التي أعلنها الجهاز، مع إظهار ما إذا كان هناك ABI مطابق أو لا.
5. لا تعتبر تطابق ABI ضماناً للتثبيت أو التشغيل؛ قد توجد قيود SDK أو توقيع أو split dependencies أو متطلبات عتاد/خدمات.
6. افحص APKs داخل ملفات APKS/ZIP وحدد base وconfiguration وfeature splits. لا تثبّت مجموعة غير مكتملة أو غير متوافقة على أنها حزمة مكتملة.

**مرجع Android NDK:** النظام يعلن ABI الأساسية والثانوية، ويختار Package Manager المكتبات المناسبة عند التثبيت.  
[Android ABI guide](https://developer.android.com/ndk/guides/abis).

### 16.4 APK مقابل APKS وSplit APKs

- **APK:** ملف تثبيت مفرد؛ يمكن تثبيته عبر `adb install` إذا كان متوافقاً.
- **Split APKs:** عدة ملفات APK تشكل تطبيقاً واحداً، مثل base وsplit_config.arm64_v8a وsplit_config.en وsplit_config.xxhdpi. تثبيتها يدوياً يتطلب المجموعة الصحيحة معاً باستخدام `adb install-multiple`.
- **APKS:** أرشيف APK Set ينتجه عادة bundletool؛ ليس ملف APK عادياً. استخدم bundletool لاختيار الأجزاء الموافقة للجهاز وتثبيتها، أو استخرج مجموعة splits المتوافقة ثم مررها إلى آلية التثبيت المناسبة.
- **XAPK/APKM أو أرشيفات مشابهة:** لا تفترض أنها صيغة موحدة؛ يلزم التعرف على محتويات الأرشيف والتحقق من ملفات APK والبيانات المصاحبة قبل التثبيت.

**الأداة المرجعية:** Google bundletool يستطيع استخراج مواصفات الجهاز، وإنشاء APK Set مخصص لجهاز متصل، واستخراج APKs المناسبة، وتثبيت APKS مع اختيار الأجزاء المتوافقة.  
[Android bundletool documentation](https://developer.android.com/tools/bundletool) — [Google bundletool repository](https://github.com/google/bundletool).

### 16.5 مقارنة أدوات إدارة التطبيقات التي أضيفت إلى البحث

#### A. ADB AppControl — أقوى مرشح Windows جاهز لهذا المحور

**ما يعلنه المطور:**
- مدير تطبيقات يعرض التطبيقات ويسمح بالبحث داخل القوائم.
- Disable/Enable وUninstall وRestore لتطبيقات النظام وفق القيود التي يفرضها Android.
- تثبيت APK وAPKS، وتثبيت عدة تطبيقات وعمليات جماعية.
- حفظ APK وملفات Split من التطبيقات المثبتة.
- إدارة التطبيقات والاتصال عبر Wi-Fi في نسخته/منتجه المدعوم.
- أدوات إضافية مثل Shell وLogcat وإدارة الأذونات.

**تفاصيل مهمة من وثائق سير العمل:**
- البحث يتم داخل القائمة المختارة (All/System وغيرها).
- إلغاء تثبيت تطبيق نظام دون Root غالباً يزيله من قائمة التطبيقات المثبتة للمستخدم ولا يحذف ملفه الأصلي من قسم النظام.
- يمكن استعادة تطبيق نظام سبق إلغاء تثبيته للمستخدم على Android 8+ وفق آلية الأداة.
- حفظ APK لا يحفظ بيانات التطبيق الخاصة.

**الحكم:** مرشح أول للاختبار العملي في Windows؛ يغطي جزءاً كبيراً من المطلوب مباشرة، بما في ذلك APK وAPKS والبحث والعمليات الجماعية. يجب اختبار Wireless في نسخة PC تحديداً، وفصل ما هو متاح مجاناً عما يتطلب Extended/PRO.

المراجع: [الموقع الرسمي](https://www.adbappcontrol.com/en/) — [PC Getting Started](https://adbappcontrol.com/en/docs/pc/) — [Workflow and app operations](https://www.adbappcontrol.com/en/docs/pc/workflow/).

#### B. Universal Android Debloater Next Generation (UAD-ng) — مرشح مفتوح المصدر

**المثبت من المشروع:**
- واجهة متعددة الأنظمة مكتوبة بـRust وتستخدم ADB لأجهزة غير مروّتة.
- قائمة حزم قابلة للبحث الفوري، مع أسماء/وصف الحزم وقوائم توصيات لإزالة التطبيقات غير المرغوبة.
- Uninstall/Disable وRestore/Enable لحزم النظام.
- دعم عدة أجهزة وعدة مستخدمين، وسجل محلي للعمليات.
- دليل الإعداد يشرح الاتصال عبر USB أو Wireless ADB.

**حدود النطاق:** الأداة تركز على Debloating وإدارة حزم النظام؛ لا يوجد في الدليل الذي تمت مراجعته ما يكفي لاعتبارها مدير APK/APKS متكاملاً أو محللاً لمعمارية APK. لا نمنحها هذه القدرات دون دليل أو اختبار.

**تحذير:** إزالة حزم OEM الحساسة قد تسبب فقدان وظائف أو مشاكل حسابات؛ توصيات قوائم Debloat ليست ضماناً بأن الحزمة غير مهمة لكل جهاز. يجب عرض اسم الحزمة ووظيفتها ودرجة المخاطرة قبل أي إجراء.

المراجع: [GitHub repository](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation) — [Getting started / wireless connection](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation/wiki/Getting-started).

#### C. AYA — مدير ADB عام مفتوح المصدر

- تطبيق Desktop متعدد المنصات، وتوجد حزم Windows x64 معلنة.
- يذكر المشروع Application Manager وFile Explorer وProcess Monitor وLogcat وInteractive Shell وScreen Mirroring.
- لا يوجد في الوصف المختصر الذي تمت مراجعته إثبات كافٍ لدعم APKS أو فحص ABI أو Wireless pairing workflow؛ تبقى هذه نقاط تحقق.
- مناسب كمرشح لإدارة التطبيقات ضمن لوحة أدوات أوسع، وليس مثبتاً حتى الآن كبديل كامل لـADB AppControl.

المصدر: [liriliri/aya](https://github.com/liriliri/aya).

#### D. ADBridge — واجهة Windows محمولة حديثة

- المشروع يعلن اكتشاف الأجهزة عبر USB وWireless مع تتبع مباشر.
- إدارة التطبيقات: عرض القائمة، إلغاء التثبيت، مسح البيانات، وتثبيت APK.
- File Explorer وLogcat وScrcpy.
- لا يعلن دعم APKS أو Split APKs أو تحليل ABI في الوصف المفحوص.
- مشروع مرشح حديث يحتاج تدقيقاً إضافياً للإصدارات والنشاط والاختبارات قبل رفعه لمستوى الأدوات الناضجة.

المصدر: [rosalesKevin/adbridge](https://github.com/rosalesKevin/adbridge).

### 16.6 مقارنة أولية

| الأداة | USB ADB | Wireless ADB | بحث التطبيقات | Disable/Uninstall | APK | APKS/Splits | ABI compatibility analysis |
|---|---|---|---|---|---|---|---|
| ADB AppControl | معلن | معلن/يحتاج اختبار نسخة PC | معلن | معلن | معلن | APKS معلن | غير مثبت |
| UAD-ng | معلن | موثق في دليل الإعداد | معلن | معلن | غير مثبت كمدير تثبيت | غير مثبت | غير مثبت |
| AYA | ADB أساسي | غير مثبت من الوصف | مدير تطبيقات معلن | تفاصيل العمليات تحتاج اختبار | يحتاج اختبار | غير مثبت | غير مثبت |
| ADBridge | معلن | معلن | قائمة تطبيقات؛ بحث متقدم يحتاج اختبار | Uninstall معلن | معلن | غير مثبت | غير مثبت |
| Android Platform Tools + bundletool | نعم | عبر اتصال ADB قائم | أوامر Package Manager | نعم عبر أوامر ADB/PM | نعم | نعم عبر install-multiple/bundletool | مواصفات الجهاز واختيار splits؛ ليس واجهة GUI متكاملة |

**مفتاح التصنيف:** "معلن" يعني أن المشروع أو المطور يذكر الميزة؛ لا يعني أنها اختُبرت على أجهزتنا. "غير مثبت" يعني لم يظهر دليل كافٍ في المصادر المفحوصة، وليس حكماً باستحالة دعمها.

### 16.7 ضوابط الأمان والسلوك

1. يجب أن يكون لكل عملية جهاز محدد بالـSerial، خصوصاً عند اتصال أكثر من هاتف.
2. افصل إزالة تطبيق المستخدم عن إزالة/تعطيل تطبيق نظام، واعرض نطاق العملية بوضوح.
3. لا تنفذ حذفاً جماعياً لقوائم Debloat افتراضياً؛ يلزم تحديد الحزم ومراجعة أثرها.
4. اعرض تأكيداً مستقلاً قبل Disable أو Uninstall لتطبيق System.
5. سجّل نتيجة كل تطبيق في عمليات Batch؛ لا تعرض العملية كلها كناجحة إذا فشل عنصر واحد.
6. لا تدّعِ أن Uninstall لتطبيق نظام دون Root يمحو ملفه من قسم النظام.
7. لا تخزّن مفاتيح ADB الخاصة أو بيانات الاقتران في Logs، ولا تعرضها للمستخدم.
8. Wireless ADB يمنح الكمبيوتر المفوّض قدرة تنفيذ أوامر ADB؛ يجب إظهار الجهاز المقترن وإتاحة فصل/إلغاء التفويض.
9. افحص الملفات محلياً قبل التثبيت: سلامة ZIP، Manifest، Package ID، Version، ABI، minSdk، والتوقيع عند إمكان ذلك.
10. لا تتجاوز تحذيرات Android أو متطلبات التوقيع أو Downgrade أو صلاحيات النظام بصمت.

### 16.8 خطة اختبار المحور

| الاختبار | ADB AppControl | UAD-ng | AYA | ADBridge | ADB + bundletool |
|---|---|---|---|---|---|
| اكتشاف USB ADB | مطلوب | مطلوب | مطلوب | مطلوب | مطلوب |
| اتصال Wireless Pairing | مطلوب | مطلوب | مطلوب | مطلوب | مطلوب |
| عرض System/User apps | مطلوب | مطلوب | مطلوب | مطلوب | عبر Package Manager |
| بحث بالاسم وPackage ID | مطلوب | مطلوب | مطلوب | مطلوب | عبر أوامر/واجهة اختبار |
| تفاصيل الإصدار والمسار | مطلوب | مطلوب | مطلوب | مطلوب | مطلوب |
| تثبيت APK | مطلوب | غير مثبت | مطلوب | مطلوب | مطلوب |
| تثبيت APKS/Split APKs | مطلوب | غير مثبت | غير مثبت | غير مثبت | مطلوب |
| Uninstall تطبيق مستخدم | مطلوب | مطلوب | مطلوب | مطلوب | مطلوب |
| Disable/Restore system app | مطلوب | مطلوب | يحتاج اختبار | يحتاج اختبار | مطلوب بحسب إصدار Android والصلاحيات |
| تحليل ABI للجهاز والملف | مطلوب | غير مثبت | غير مثبت | غير مثبت | مطلوب كاختبار منفصل |
| حفظ APK وSplit APKs | مطلوب | غير مثبت | يحتاج اختبار | غير مثبت | مطلوب |
| سجل عمليات متعدد الأجهزة | مطلوب | مطلوب | يحتاج اختبار | يحتاج اختبار | مطلوب |

**بيانات الاختبار المطلوبة:** موديل الهاتف، إصدار Android/API، Primary/Secondary ABIs، طريقة الاتصال، نسخة Platform Tools، اسم التطبيق وPackage ID، حالة System/User، نوع ملف التثبيت، محتويات splits، نتيجة العملية ورسالة الخطأ. لا تحفظ IMEI أو بيانات شخصية في سجل المقارنة.

### 16.9 الاستنتاج الأولي لهذا المحور

1. **ADB AppControl** هو أقرب برنامج جاهز متكامل لمتطلبات إدارة التطبيقات على Windows وفق الميزات المعلنة، ويستحق أولوية الاختبار الأولى.
2. **UAD-ng** هو المرشح المفتوح المصدر الأقوى لإدارة/تنظيف حزم النظام والبحث عنها، لكنه ليس بديلاً مثبتاً لمدير تثبيت APKS.
3. **AYA** و**ADBridge** يستحقان الاختبار كواجهات ADB عامة؛ دعم APKS وتحليل ABI لم يثبت بعد.
4. **ADB + bundletool** هو خط الأساس المرجعي الموثوق لتثبيت APK Sets/Splits واختيار ما يلائم مواصفات الجهاز، ويمكن أن يكمل أي واجهة GUI لا تدعم APKS.
5. لا توجد حتى الآن أداة ثبت أنها تجمع واجهة إدارة التطبيقات والبحث، Wireless ADB، APKS، وفحص توافق ABI بشكل شامل. هذه نقطة اختبار وليست استنتاجاً نهائياً.

### 16.10 حالة المحور

- [x] تحديد متطلبات إدارة التطبيقات عبر USB وWireless ADB.
- [x] توثيق أوامر ADB/Package Manager الأساسية.
- [x] توثيق ABI وفرق APK عن APKS/Split APKs.
- [x] إضافة ADB AppControl وUAD-ng وAYA وADBridge للمقارنة.
- [ ] فحص إصدارات Windows الحالية وميزات النسخ المجانية/المدفوعة لكل أداة.
- [ ] تدقيق كود AYA وADBridge وUAD-ng للوظائف غير الواضحة.
- [ ] اختبار APK وAPKS وSplit APKs على جهاز ARM64 فعلي.
- [ ] اختبار ملفات x86/x86_64 وarmeabi-v7a على أجهزة ذات ABI مختلفة، وتسجيل نتيجة Package Manager الفعلية.
- [ ] اختبار Wireless Pairing على Android 11+، والاتصال TCP/IP للأجهزة الأقدم عند دعمها.
- [ ] اختبار Disable/Restore/Uninstall مع التفريق بين تطبيقات المستخدم والنظام.


---

## 17. توسيع إدارة التطبيقات: DNS Ad Blocking وفلاتر تاريخ التثبيت

### 17.1 إعداد DNS مخصص لحجب الإعلانات عبر ADB

يجب أن تتعامل الأداة مع DNS كميزة مستقلة في Device Network Management، مع عرض نوع الإعداد ونطاق تأثيره وإمكانية استرجاع الإعداد السابق.

#### المسار A — Android Private DNS (DNS-over-TLS)

- Android 9 / API 28 وما بعده يدعم Private DNS باستخدام DNS-over-TLS (DoT).
- هذا المسار يعتمد على **اسم مضيف DNS provider** (hostname)، مثل اسم مزوّد يدعم DoT، وليس مجرد عنوان IP لخادم DNS عادي.
- يمكن للأداة قراءة الحالة الحالية ومحاولة ضبطها عبر ADB باستخدام مفاتيح Settings.Global:
  - `private_dns_mode`: القيم المتوقعة `off` أو `opportunistic` أو `hostname`.
  - `private_dns_specifier`: اسم المضيف الخاص بالمزوّد عند اختيار `hostname`.
- أوامر ADB المقترحة كمسار فحص/تجربة على جهاز مصرح به:
  - قراءة: `adb shell settings get global private_dns_mode`
  - قراءة المضيف: `adb shell settings get global private_dns_specifier`
  - ضبط وضع hostname: `adb shell settings put global private_dns_specifier <provider-hostname>` ثم `adb shell settings put global private_dns_mode hostname`
  - العودة إلى Automatic: `adb shell settings put global private_dns_mode opportunistic` ثم إزالة/مسح قيمة المضيف عند الحاجة.
- لا تفترض نجاح الكتابة لمجرد أن الأمر أعاد exit code صفراً؛ اقرأ القيم مجدداً، ثم اختبر حل أسماء DNS فعلياً، وتحقق من حالة Private DNS في النظام.
- قد تمنع بعض إصدارات Android أو تخصيصات الشركات أو سياسات الإدارة بعض التغييرات. واجهة DevicePolicyManager الرسمية تتيح ضبط Private DNS للـDevice Owner؛ وهذا ليس دليلاً على أن كل جهاز عادي يسمح لأي تطبيق بتغيير الإعداد.
- يجب اختبار هذا المسار منفصلاً على Android 9 و10+، وعلى أجهزة Samsung وغيرها، لأن تطبيقات VPN وبعض أوضاع الشبكة قد تتداخل مع مسار DNS.

**تنبيه توافق:** Private DNS هنا يعني DoT hostname. لا تضع عنوان IP مثل `1.2.3.4` في خانة hostname وتتوقع أن يعمل كـDNS عادي. كما أن DoT لا يساوي DNS-over-HTTPS (DoH)، ولا يضمن وحده حجب الإعلانات؛ يجب أن يكون المزود نفسه يقدم قوائم حجب.

#### المسار B — DNS عادي لكل شبكة Wi-Fi

- إعداد DNS التقليدي عبر DHCP أو إعدادات IP ثابتة يكون عادة مرتبطاً بكل شبكة Wi-Fi، وليس إعداداً عاماً واحداً لجميع الشبكات.
- لا يوجد أمر ADB قياسي موحد ومضمون لتغيير DNS لكل شبكات Wi-Fi المحفوظة عبر جميع إصدارات Android والشركات.
- لا تعتمد على تعديل قواعد بيانات Wi-Fi الخاصة بالنظام أو أوامر netd الداخلية باعتبارها واجهة مستقرة؛ قد تتطلب صلاحيات أعلى وقد تتغير بين الإصدارات.
- إذا كان المطلوب DNS بعنوان IP محلي أو خادم داخل الشبكة، يجب تحديد هل المطلوب شبكة Wi-Fi واحدة أم جميع الشبكات، ثم اختبار واجهة الجهاز الفعلية.

#### المسار C — تطبيق DNS Ad Block / Local VPN

- بعض حلول الحجب تعمل عبر VPN محلي على الهاتف وتحوّل استعلامات DNS أو حركة الشبكة عبر محرك تصفية.
- يمكن لأداة Windows تثبيت APK عبر ADB وفتح شاشة التطبيق، لكن تفعيل VPN عادة يتطلب موافقة/تأكيداً من المستخدم على الهاتف، ولا ينبغي افتراض إمكانية تشغيله بصمت عبر ADB على جهاز عادي غير مروّت.
- لا تخلط بين تطبيق DNS يستخدم VPN محلياً وبين Private DNS المدمج في Android؛ فهما مساران مختلفان وقد يتعارضان أو يغيّر أحدهما سلوك الآخر.
- لا بد من توضيح ما إذا كان الحجب DNS-only أم يتضمن فلترة أوسع، وهل يعمل مع Wi-Fi وبيانات الهاتف، وهل يتطلب خدمة/اشتراكاً أو اتصالاً خارجياً.

#### واجهة DNS المقترحة في برنامج Windows

| الحقل/الوظيفة | المطلوب |
|---|---|
| Current DNS mode | Off / Automatic / Custom Private DNS / VPN-based DNS / Unknown |
| Provider hostname | اسم DoT hostname، وليس IP |
| Presets | قائمة مزودين موثوقين مع توثيق هل لديهم Ad Blocking أم لا |
| Custom provider | إدخال hostname مخصص والتحقق من الصيغة |
| Apply / Restore | حفظ الحالة السابقة، تطبيق التغيير، وإمكانية الرجوع |
| Verify | إعادة قراءة الإعداد + اختبار DNS resolution + إظهار فشل TLS/الاتصال إن أمكن |
| Device scope | إظهار Serial وUSB/Wireless قبل التنفيذ |
| Compatibility | إظهار Android API level وقيود OEM/Device Owner/VPN |
| Safety | تحذير من احتمال فقدان حل DNS أو الاتصال إذا كان المضيف غير صحيح |

**المراجع:**
- [Google Public DNS — Android Private DNS setup](https://developers.google.com/speed/public-dns/docs/using)
- [Android Help — Manage Private DNS](https://support.google.com/android/answer/9654714)
- [Android DevicePolicyManager — Private DNS APIs and Device Owner restriction](https://developer.android.com/reference/android/app/admin/DevicePolicyManager)
- [AOSP ConnectivitySettingsUtils — accepted Private DNS modes](https://android.googlesource.com/platform/frameworks/libs/net/+/8ef52686483cdff0040d22753bb516e99a96a88d/common/framework/com/android/net/module/util/ConnectivitySettingsUtils.java)

### 17.2 تاريخ التثبيت وآخر تحديث ومصدر التطبيق

يجب أن تكون بيانات الزمن جزءاً أساسياً من Application Manager، لا مجرد عمود اختياري مخفي.

#### البيانات المراد جمعها لكل Package

| الحقل | المعنى | مصدر محتمل |
|---|---|---|
| First install time | تاريخ أول تثبيت مسجل للحزمة الحالية | `dumpsys package <package>` |
| Last update time | تاريخ آخر تحديث للحزمة | `dumpsys package <package>` |
| Installer package | التطبيق/المتجر الذي سجل Package Manager أنه ثبّت الحزمة | `pm list packages -i` أو تفاصيل dumpsys |
| Package name | المعرف الفريد للحزمة | `pm list packages` |
| Version name/code | نسخة التطبيق | `dumpsys package` أو بيانات APK |
| System/User | هل الحزمة System أو تطبيق مستخدم | Package flags وPackage Manager |
| Enabled state | Enabled / Disabled / Disabled-user وغيرها | حالة User package في dumpsys |
| Install location/path | مسار base/split APK | `pm path <package>` |
| UID | معرف التطبيق في النظام | `pm list packages -U` أو dumpsys |
| User/Profile | المستخدم أو ملف العمل الذي ثبتت له الحزمة | مخرجات Package Manager حسب دعم النظام |

**مصدر الوقت:** مخرجات Package Manager/Dumpsys قد تعرض `firstInstallTime` و`lastUpdateTime`. يختلف شكل وتنسيق المخرجات بين إصدارات Android؛ في بعض الإصدارات الحديثة قد تظهر حقول الوقت ضمن قسم المستخدم، ولذلك يجب بناء Parser يتحمل اختلاف التنسيق ولا يعتمد على Regex واحد ثابت لكل الأجهزة.

**قيد تاريخي:** هذه التواريخ تصف حالة الحزمة الحالية. بعد إزالة التطبيق بالكامل قد تختفي بياناته من قائمة الحزم؛ لا نعد المستخدم بإمكانية استرجاع تاريخ كل التطبيقات التي أزيلت سابقاً دون مصدر سجل مستقل.

#### الفلاتر والفرز المطلوبة

- Installed date: قبل/بعد/بين تاريخين، وآخر 24 ساعة/7 أيام/30 يوماً/90 يوماً/سنة.
- Last updated: الفلاتر الزمنية نفسها مستقلة عن تاريخ أول تثبيت.
- Installer source: Google Play، Galaxy Store، Package Installer، ADB/Shell، متجر OEM، Unknown.
- App type: System / User / Updated System App.
- State: Enabled / Disabled / Suspended / Hidden إن ظهرت من المصدر.
- ABI: Native arm64-v8a / armeabi-v7a / x86_64 / x86 / No native libraries / Unknown.
- Android requirement: minSdk/targetSdk إذا أمكن استخراجها من APK المثبت أو ملف APK محفوظ.
- Package name / App label: بحث نصي يدعم المطابقة الجزئية.
- Version: فرز حسب versionName أو versionCode، مع عدم مقارنة versionName نصياً كأنه رقم دائماً.
- Size: حجم base/split APKs إن أمكن قياس الملفات، مع توضيح أن هذا ليس حجم بيانات التطبيق.
- Permissions: فلاتر للأذونات الحساسة عند توافر بيانات موثوقة؛ لا نستنتج الإذن الممنوح من مجرد وجوده في Manifest.
- Multi-select: اختيار نتائج الفلتر لتنفيذ Export APK أو فتح التفاصيل أو إجراءات أخرى؛ لا تجعل Disable/Uninstall الجماعي إجراءً تلقائياً.

#### دقة مصدر التثبيت

مصدر التثبيت ليس دائماً دليلاً قطعياً على مصدر التنزيل الأصلي:
- قد يظهر `com.android.vending` أو متجر الشركة.
- قد يظهر Package Installer عند تثبيت APK يدوياً.
- قد يظهر `com.android.shell` أو قيمة مرتبطة بـADB عند التثبيت من الكمبيوتر.
- قد يكون الحقل فارغاً أو Unknown بسبب إصدار Android أو OEM أو طريقة تثبيت قديمة.
- يجب عرض القيمة الخام بجانب اسم المصدر المصنف، وعدم اختراع اسم متجر إذا لم توجد معلومة.

**المراجع:**
- [Android package inventory and timestamps — forensic reference](https://www.androidforensics.org/en/artifacts/installed-packages)
- [dumpsys package fields and version-dependent timestamps](https://bytescope.dev/en/web-adb/commands/dumpsys-package)

### 17.3 تحديث مقارنة الأدوات بالنسبة إلى DNS والتواريخ

| الأداة/المسار | DNS Private / Ad Block | First/Last Install Dates | Installer Source | ملاحظة |
|---|---|---|---|---|
| ADB AppControl | لا يوجد إثبات كافٍ حتى الآن لميزة إدارة Private DNS | يحتاج فحص واجهة وإصدار | يحتاج فحص | لا ننسب الميزة للأداة قبل اختبارها |
| UAD-ng | ليس مدير DNS وفق النطاق المفحوص | بيانات التثبيت الزمنية غير مثبتة كفلاتر GUI | غير مثبت | تركيزه Debloat |
| AYA | غير مثبت | يحتاج تدقيق الكود | يحتاج تدقيق الكود | يمكن أن يستفيد من بيانات ADB المتاحة |
| ADBridge | غير مثبت | غير مثبت | غير مثبت | يحتاج تدقيق الكود |
| ADB + dumpsys/pm | أوامر Settings.Global قد تسمح بفحص/محاولة ضبط Private DNS حسب الجهاز والسياسات | متاح عبر Package Manager/Dumpsys | متاح عند إعلانه من Package Manager | خط أساس تقني وليس واجهة مستخدم جاهزة |
| Android Device Owner / DPC | API رسمي لإدارة Private DNS ضمن شروط Device Owner | APIs إدارية حسب سياسة الجهاز | حسب Package Manager | مسار إدارة مؤسسية وليس مطلوباً لبرنامج ADB عادي |

**قرار:** لا نرفض أداة لمجرد أنها لا تحتوي DNS؛ يمكن أن تكون ميزة DNS طبقة مستقلة في البرنامج باستخدام ADB أو DPC/تطبيق مرافق، مع احترام قيود النظام.

### 17.4 حالات الاختبار الجديدة

| الاختبار | خطوات التحقق | نتيجة النجاح |
|---|---|---|
| قراءة Private DNS | قراءة mode وspecifier قبل أي تغيير | عرض الحالة الفعلية بدقة |
| تعيين DoT hostname | تجربة مزود صحيح على جهاز اختبار مصرح به | قراءة الإعداد مجدداً ونجاح DNS/TLS |
| مزود غير صالح | hostname غير صحيح أو لا يقدم DoT | إظهار فشل التحقق وعدم تسجيل نجاح |
| Restore | حفظ الحالة الأصلية ثم استعادتها | عودة الإعداد الأصلي والتحقق منه |
| VPN conflict | مقارنة Private DNS مع DNS/VPN app نشط | توضيح أي مسار هو الفعال وعدم ادعاء حجب شامل |
| First install date | استخراج timestamp لتطبيق قديم وآخر حديث | تحويل صحيح للوقت والمنطقة الزمنية مع إظهار المصدر |
| Last update date | مقارنة تطبيق تم تحديثه بتطبيق لم يتغير | فصل تاريخ التحديث عن تاريخ التثبيت |
| Installer source | تطبيق مثبت من Play وآخر عبر ADB | عرض المصدر كما أعلنه Package Manager |
| Unknown source | تطبيق لا يعلن installer | عرض Unknown وعدم تخمين المصدر |
| Date filters | نطاق تاريخي وحدود شاملة/غير شاملة واضحة | نتائج صحيحة قابلة للفرز والتصدير |
| Cross-version parser | أجهزة Android بإصدارات مختلفة | استخراج الحقول أو تسجيل Unsupported/NotReported دون انهيار |

### 17.5 حالة التوسعة

- [x] إضافة مسارات Private DNS وDNS العادي وVPN-based DNS كأنواع مختلفة.
- [x] توثيق قيود DoT hostname مقابل DNS IP عادي.
- [x] إضافة First install time وLast update time وInstaller source.
- [x] إضافة فلاتر تاريخ التثبيت والتحديث ومصدر التثبيت وABI وحالة التطبيق.
- [x] إضافة حالات اختبار التحقق والاسترجاع وتعارض VPN.
- [ ] تدقيق دعم ADB AppControl وAYA وADBridge لهذه الحقول والفلاتر في الإصدارات الحالية.
- [ ] اختبار Private DNS عبر ADB على Android 12 والجهاز Samsung SM-N976N، مع حفظ الإعداد الأصلي وعدم تغيير الجهاز دون إذن منفصل.
- [ ] اختبار التواريخ ومصدر التثبيت على أجهزة Android متعددة الإصدارات.


---

## 16.7 قرار الأولوية: محرك تثبيت APK واسع الصيغ

**تم اعتماد هذا المحور كأولوية V1:** القدرة على تثبيت APK وSplit APK وAPKS وAPKM وXAPK، بما في ذلك OBB عند وجوده، أهم حالياً من التوسع في تحليل ELF أو dependencies native المتقدمة.

### ترتيب الدعم

| الأولوية | الصيغة/الحالة | مسار التنفيذ |
|---|---|---|
| P0 | APK مفرد | adb install |
| P0 | base + split APKs | فحص المجموعة ثم adb install-multiple |
| P0 | APKS | Google bundletool لاختيار splits المتوافقة وتثبيتها |
| P1 | APKM غير المشفر | parser/adapter بعد التحقق من metadata وmanifest |
| P1 | XAPK | parser/adapter، APK set ثم OBB plan منفصل |
| P1 | عدة APKs يختارها المستخدم | لا تجمعها إلا بعد إثبات أنها package/split set واحد |
| P2 | AAB | مسار تحويل عبر bundletool؛ ليس APK جاهزاً للتثبيت |

### قاعدة التوافق الضرورية فقط لمسار التثبيت

نحتفظ بفحص AndroidManifest وpackage/version وminSdk وABI list وdensity/locales وsplit identity. نؤجل DT_NEEDED وSONAME وsymbol versions وdependency graph إلى التشخيص المتقدم؛ هذه ليست شروطاً لازمة لتثبيت معظم ملفات APK.

### سياسة التثبيت

- Serial صريح لكل عملية.
- لا منح تلقائي لكل الأذونات، ولا downgrade أو test-only إلا بطلب صريح.
- لا انتحال installer source باستخدام -i.
- التحقق من package/version بعد التثبيت.
- OBB يعالج كعملية مستقلة بعد نجاح تثبيت APK ومطابقة package، مع معاينة وموافقة.
- لا نعتبر استخراج الأرشيف أو نجاح ADB push دليلاً على نجاح التثبيت.

### حالات الاختبار الإلزامية

APK صالح/تالف؛ split كامل/ناقص/مختلط؛ package/version mismatch؛ ABI غير متوافق؛ density/language variants؛ APKS مع TOC وstandalones؛ APKM/XAPK غير مشفرين ومجهولي البنية؛ XAPK مع OBB؛ توقيع مختلف؛ downgrade؛ عدة أجهزة؛ انقطاع الاتصال؛ إلغاء العملية؛ مساحة غير كافية.

**الحالة الحالية:** القرار والمعمارية ومعايير القبول موثقة. لا يوجد تنفيذ في البرنامج ولا اختبار فعلي على هاتف حتى الآن. التفاصيل وتدقيق المشاريع المرجعية في [دراسة الأدوات](02-tool-landscape-and-market-research.md#2037-إعادة-ضبط-الأولوية-التثبيت-واسع-التغطية-هو-هدف-v1-12-نوفمبر-2026).
