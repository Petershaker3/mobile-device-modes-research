# دليل برمجة أداة قراءة بيانات الهواتف والتحويل بين الأوضاع (Mobile Device Modes & Read Info Reference)

مرجع تقني شامل لبناء برنامج خدمي لفنيي السوفتوير لقراءة بيانات الهواتف الذكية (Read Info) والتحويل البرمجي بين الأوضاع المختلفة (Mode Switching) عبر كافة الشركات والمعالجات.

## 1. طبقات الاتصال الأساسية في نظام ويندوز

يعتمد أي برنامج سوفتوير متكامل على 4 قنوات اتصال رئيسية للتخاطب مع الهواتف:
1. **طبقة الأوامر القياسية (`ADB & Fastboot`):** عبر مكتبات [Android Platform Tools](https://source.android.com/docs/setup/test/running) للتعامل مع أوضاع التصحيح والبوتلودر والسايدلود.
2. **طبقة المنافذ التسلسلية (`SerialPort / COM`):** لإرسال أوامر `AT Commands` لمودم سامسونج ([galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool)) وحزم بروتوكول `Odin / LOKE` في وضع الداونلود مود ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)).
3. **طبقة أجهزة الوسائط المحمولة (`Windows Portable Devices - WPD API`):** لقراءة بيانات الهاتف في وضع `MTP` القياسي لجميع الشركات بدون الحاجة لتفعيل تصحيح USB ([FC.SDK WPD Transport](https://github.com/SharpAstro/FC.SDK)).
4. **طبقة USB المباشرة (`libusb / WinUSB`):** تُستخدم للتعامل مع واجهات USB الخاصة بالشركات، مثل Xiaomi Mi Assistant. أوضاع Qualcomm EDL وMediaTek BROM/Preloader خارج نطاق هذا البحث والإصدار الحالي.

## 2. قراءة بيانات الأجهزة في كل وضع (Read Info Protocols)

### أ. وضع التصحيح القياسي (ADB Mode)
يعمل عندما يكون الهاتف في الوضع العادي أو الريكفري المخصص مع تفعيل `USB Debugging` ([Android Open Source Project](https://source.android.com/docs/setup/test/running))، وتُقرأ الخصائص عبر تحليل مخرجات `adb shell getprop` ([Stack Overflow](https://stackoverflow.com/questions/59608810/get-imei-via-adb-only-for-old-huawei-models)):
* **الموديل والشركة المصنعة:** `adb shell getprop ro.product.model` و `adb shell getprop ro.product.brand` ([Stack Overflow](https://stackoverflow.com/questions/59608810/get-imei-via-adb-only-for-old-huawei-models)).
* **إصدار أندرويد ومستوى الحماية:** `adb shell getprop ro.build.version.release` و `adb shell getprop ro.build.version.security_patch` ([Stack Overflow](https://stackoverflow.com/questions/59608810/get-imei-via-adb-only-for-old-huawei-models)).
* **المعالج وبنية النظام:** `adb shell getprop ro.board.platform` و `adb shell getprop ro.product.cpu.abi` ([Stack Overflow](https://stackoverflow.com/questions/59608810/get-imei-via-adb-only-for-old-huawei-models)).
* **السيريال ورقم الـ IMEI:** `adb get-serialno` للسيريال، و `adb shell service call iphonesubinfo 1` لاستخراج الـ IMEI في إصدارات أندرويد الحديثة عبر تحليل الرد السداسي عشري ([Stack Overflow](https://stackoverflow.com/questions/59608810/get-imei-via-adb-only-for-old-huawei-models)).
* **حالة البوتلودر والـ FRP:** `adb shell getprop ro.boot.flash.locked` و `adb shell getprop sys.oem_unlock_allowed` ([Android Open Source Project](https://source.android.com/docs/setup/test/running)).

### ب. وضع الفاستبوت العام والشركات (Generic, Xiaomi, Motorola Fastboot)
يتم التواصل مع البوتلودر مباشرة عبر بروتوكول Fastboot ([Android Open Source Project](https://source.android.com/docs/setup/test/running)):
* **قراءة كافة متغيرات البوتلودر (موتورولا، شاومي، أندرويد خام):** `fastboot getvar all` ويعيد الموديل (`product`)، والسيريال (`serialno`)، وحالة الحماية (`secure`)، والسلوت النشط (`current-slot`) ([HardReset Motorola](https://www.hardreset.info/devices/motorola/motorola-moto-x-pure-edition/faq/read-info-fastboot/motorola-fastboot-read-info-bootloader/)، [XDA Motorola Guide](https://xdaforums.com/t/guide-using-fastboot-exe-with-motorola-devices.4042039/)).
* **قراءة حالة البوتلودر ومستوى الحماية في شاومي:** `fastboot getvar unlocked` و `fastboot getvar anti` (رقم حماية Anti-Rollback) ([HardReset Motorola](https://www.hardreset.info/devices/motorola/motorola-moto-x-pure-edition/faq/read-info-fastboot/motorola-fastboot-read-info-bootloader/)).
* **قراءة حالة القفل التفصيلية:** `fastboot oem device-info` لمعرفة حالة `Device unlocked` و `Device critical unlocked` ([Android Open Source Project](https://source.android.com/docs/setup/test/running)).

### ج. وضع فاستبوت هواوي وهونر (Huawei & Honor Fastboot)
تعتمد أجهزة هواوي وهونر على أوامر `OEM` مخصصة داخل البوتلودر ([HardReset Huawei](https://www.hardreset.info/devices/huawei/huawei-mate-30/faq/read-info-fastboot/fastboot-mode-read-info-huawei/)، [XDA Huawei Fastboot](https://xdaforums.com/t/huawei-fastboot-commands.3850123/)):
* **قراءة الموديل:** `fastboot oem get-product-model` ([HardReset Huawei](https://www.hardreset.info/devices/huawei/huawei-mate-30/faq/read-info-fastboot/fastboot-mode-read-info-huawei/)).
* **قراءة رقم الإصدار (Build Number):** `fastboot oem get-build-number` ([HardReset Honor](https://www.hardreset.info/devices/honor/honor-viewpad-6/faq/read-info-fastboot/fastboot-mode-read-info-huawei/)).
* **قراءة السيريال والـ IMEI:** `fastboot oem get-psid` ([HardReset Huawei](https://www.hardreset.info/devices/huawei/huawei-mate-30/faq/read-info-fastboot/fastboot-mode-read-info-huawei/)، [XDA Huawei Codes](https://xdaforums.com/t/guide-get-imei-and-other-useful-codes-from-fastboot.3771336/)).
* **قراءة حالة البوتلودر (Locked / Unlocked):** `fastboot oem get-bootinfo` ([HardReset Huawei](https://www.hardreset.info/devices/huawei/huawei-mate-30/faq/read-info-fastboot/fastboot-mode-read-info-huawei/)).
* **قراءة التوجيه والدولة (Cust & Vendor/Country):** `fastboot getvar vendorcountry` و `fastboot oem oeminforead-CUSTOM_VERSION` ([XDA Huawei Fastboot](https://xdaforums.com/t/huawei-fastboot-commands.3850123/)).

### د. وضع الـ MTP العام ومودم سامسونج (Generic MTP & Samsung MTP)
* **وضع MTP العام (WPD API):** استخدام Windows Portable Devices لاكتشاف الجهاز وقراءة خصائص MTP القياسية التي يتيحها الهاتف. لا يضمن WPD استخراج إصدار Android أو IMEI أو حالة Bootloader؛ هذه البيانات تعتمد على ما يعلنه الجهاز وخدمات الشركة. مشروع FC.SDK مرجع لفهم WPD/PTP وليس SDK عامة لقراءة معلومات هواتف Android.
* **وضع MTP لأجهزة سامسونج (AT Commands):** فتح منفذ `Samsung Mobile USB Modem` بسرعة `115200` وإرسال الأمر `AT+DEVCONINFO\r\n` (مع إرسال `AT+ACTIVATE=0,0,0\r\n` أولاً في الحمايات الحديثة) ([galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool)، [nPhoneKIT Issue #28](https://github.com/nlckysolutions/nPhoneKIT/issues/28)).
* **تحليل رد سامسونج (`+DEVCONINFO`):** يعيد الجهاز البيانات بصيغة `MN(SM-A146U);BASE(...);VER(...);HIDVER(...);PRD(...);IMEI(...);SN(...);UN(...)` لاستخراج الموديل، وإصدار الفيرموير والحماية، والتوجيه `CSC`، والـ `IMEI`، والسيريال ([nPhoneKIT Issue #28](https://github.com/nlckysolutions/nPhoneKIT/issues/28)، [INESC TEC Research](https://repositorio.inesctec.pt/server/api/core/bitstreams/54e039b4-ded3-4cce-9012-c4851faac24b/content)).

### هـ. وضع الداونلود مود لسامسونج (Samsung Download Mode / Odin Protocol)
يتصل البرنامج بمنفذ `Samsung Mobile USB CDC Composite` باستخدام بروتوكول `Odin / LOKE` ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)):
* **قراءة معلومات الجهاز السريعة (`DVIF`):** إرسال الأمر `DVIF` عبر المنفذ التسلسلي للحصول على الموديل، ورمز `CSC`، وإصدار الحماية، والمعرف الفريد `UN` ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)، [Sharp Odin Protocol Gist](https://gist.github.com/bepiy7i8d/17cf7826ffce4239a35b26fefcfac8ae)).
* **تهيئة جلسة Odin وقراءة `PIT`:** إرسال `ODIN` واستقبال `LOKE` (`IsOdin` ثم `LOKE_Initialize`) لقراءة جدول التقسيمات `PIT` أو تفليش الجهاز ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)).

### و. وضع السايدلود لشاومي (Xiaomi Sideload / Mi Assistant)
عند توصيل جهاز شاومي في وضع `Connect with MiAssistant`، يمكن استخدام بروتوكول USB الخاص بوضع Mi Assistant. مشروع MiAssistantTool يفتح واجهة USB مباشرة باستخدام libusb، ثم يستخدم ADB packet framing لبدء جلسة sideload:: وإرسال أوامر Xiaomi الخاصة؛ لا يعتمد هذا المسار على اتصال ADB المحلي عبر المنفذ 5037 ([MiAssistantTool](https://github.com/MiForge/MiAssistantTool/blob/main/miassistant/core.py)).
* **أوامر القراءة التي ظهرت في الكود المفحوص:** getsn:, getregion:, getdevice:, getbranch:, getversion:, getromzone:, getlanguage:, getcodebase:, getcarrier:, getmitoken:, getrecoveryversion:. لا يعني وجود الأمر أن كل موديلات Xiaomi أو إصدارات Recovery تدعمه؛ يجب تسجيل الرد الفعلي لكل جهاز. مرجع التنفيذ: [MiAssistantTool core.py](https://github.com/MiForge/MiAssistantTool/blob/main/miassistant/core.py).


## 3. مصفوفة التحويل بين الأوضاع برمجياً (Mode Switching)

| من الوضع (Source) | إلى الوضع (Target) | الأمر أو الحزمة البرمجية (Command / Packet) | القناة والمصدر المرجعي |
| :--- | :--- | :--- | :--- |
| **Samsung MTP** | **Download Mode** | إرسال `AT+FUS?\r\n` أو `AT+SUDDLMOD=0,0\r\n` | منفذ `Samsung USB Modem` ([Mobile-Diagnostics-Research](https://github.com/fsinnes/Mobile-Diagnostics-Research)، [DFReroot D2](https://github.com/polygraphene/DFReroot/blob/main/D2_Error.md)) |
| **Download Mode** | **Normal Mode** | تهيئة `ODIN` ثم `LOKE_Initialize` ثم `PDAToNormal()` | بروتوكول Odin عبر `COM Port` ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)) |
| **ADB Mode** | **Fastboot / Bootloader** | `adb reboot bootloader` | واجهة `ADB` القياسية ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **ADB Mode** | **Recovery / Sideload** | `adb reboot recovery` أو `adb reboot sideload` | واجهة `ADB` القياسية ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **ADB Mode** | **Samsung Download** | `adb reboot download` | واجهة `ADB` لأجهزة سامسونج ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **Fastboot Mode** | **Normal Mode** | `fastboot reboot` أو `fastboot continue` | واجهة `Fastboot` القياسية ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **Fastboot Mode** | **Fastbootd (Userspace)** | `fastboot reboot fastboot` | واجهة `Fastboot` للأجهزة الحديثة ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **Fastboot Mode** | **Recovery Mode** | `fastboot reboot recovery` | واجهة `Fastboot` القياسية ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **Xiaomi Sideload** | **Normal Mode** | `adb reboot` (أو إرسال `reboot:` عبر مقبس ADB) | واجهة `Mi Assistant Sideload` ([xiaomi_adb_sideload](https://github.com/GautamGreat/xiaomi_adb_sideload/blob/main/main.c)) |

## 4. المكتبات المفتوحة المصدر المقترحة للتنفيذ
* **[SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient):** مكتبة C# متكاملة للتعامل مع وضع الداونلود في أجهزة سامسونج بدون الحاجة لملف `SS_DL.dll`.
* **[galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool):** أداة ومكتبة Python للتعامل مع أوامر `AT+DEVCONINFO` لأجهزة سامسونج عبر منفذ المودم.
* **[xiaomi_adb_sideload](https://github.com/GautamGreat/xiaomi_adb_sideload/blob/main/main.c):** تطبيق عملي بلغة C لبروتوكول شاومي `Mi Assistant Sideload`.
* **[MiAssistantTool](https://github.com/MiForge/MiAssistantTool):** مرجع عملي لبروتوكول Xiaomi Mi Assistant عبر USB ويتضمن دوال قراءة معلومات الجهاز.


---

## 5. نتائج التدقيق الموسع وتصحيحات البحث

> النطاق الحالي: قراءة معلومات الأجهزة والتحويل بين أوضاع Android وSamsung وXiaomi وغيرها. قراءة أوضاع Qualcomm EDL وMediaTek BROM/Preloader مستبعدة من هذا البحث ومن خطة التنفيذ الحالية.

### 5.1 قواعد موثوقية عامة

1. وجود أمر في Fastboot لا يعني أن كل Bootloader ينفذه. أوامر getvar وOEM commands تختلف حسب الجهاز.
2. افصل بين اكتشاف الجهاز وقراءة بياناته وتنفيذ انتقال وضع؛ نجاح USB enumeration لا يثبت نجاح البروتوكول.
3. كل حقل معلومات يجب أن يسجل حالة توفره: Success أو Unsupported أو PermissionDenied أو TimedOut أو NotReported.
4. لا تعرض IMEI أو Serial أو Mi Token كاملاً في السجلات العامة؛ أخفِ البيانات التعريفية الحساسة.
5. لا تنفذ انتقالاً تلقائياً قبل إظهار الوضع الحالي والهدف وتوضيح أن الهاتف سيعيد التشغيل أو يغير وضع USB.

### 5.2 Fastboot وFastbootd

يجب تمثيل الوضعين كحالتين منفصلتين:
- Bootloader Fastboot: واجهة Fastboot المقدمة من Bootloader.
- Fastbootd: واجهة Fastboot تعمل في userspace ضمن Recovery.

يمكن الاستعلام عن fastboot getvar is-userspace؛ القيمة yes تعني Fastbootd وno تعني Bootloader Fastboot عندما يدعم الجهاز هذا المتغير. الأمر fastboot reboot fastboot يطلب الانتقال إلى Fastbootd على الأجهزة الداعمة، ولا يضمن توفره على جميع الأجهزة.

المراجع:
- [AOSP Fastbootd](https://source.android.com/docs/core/architecture/bootloader/fastbootd)
- [AOSP Fastboot implementation](https://android.googlesource.com/platform/system/core/+/master/fastboot/)

### 5.3 Xiaomi Mi Assistant — نتيجة فحص الكود

المشروع [MiAssistantTool](https://github.com/MiForge/MiAssistantTool) يقدم مرجعاً تنفيذياً:
- يكتشف واجهة USB ويفتحها باستخدام PyUSB أو libusb.
- يبدأ جلسة باستخدام ADB packet framing ويتحقق من استجابة sideload::.
- يرسل أوامر Xiaomi الخاصة على قناة USB نفسها؛ ليس اتصالاً بواجهة ADB المحلية على localhost:5037.
- دالة get_info() في الكود المفحوص تحاول قراءة: sn وregion وdevice وbranch وversion وromzone وlanguage وcodebase وcarrier وmitoken وrecoveryversion.
- لا يعني وجود الأمر أن كل موديلات Xiaomi أو كل إصدارات Recovery تستجيب له.

مرجع الكود: [miassistant/core.py](https://github.com/MiForge/MiAssistantTool/blob/main/miassistant/core.py).

**قرار التصميم:** إنشاء XiaomiMiAssistantAdapter مستقل، مع فصل USB transport عن ADB framing وعن أوامر Xiaomi، وإتاحة أوامر القراءة فقط في المرحلة الأولى.

### 5.4 Samsung — Download Mode وAT Modem

هناك مساران مختلفان:

**Samsung AT Modem**
- يعتمد على ظهور واجهة مودم/منفذ COM مناسب، وقد يتغير ذلك حسب الموديل وإعداد USB والتعريف.
- AT+DEVCONINFO موثق في أدوات وتجارب لأجهزة Samsung محددة، لكنه ليس مضموناً لكل أجهزة Galaxy.
- لا تفترض أن AT+ACTIVATE=0,0,0 مطلوب أو آمن على جميع الأجهزة؛ لا ترسله تلقائياً دون إثبات الحاجة والتوافق.

**Samsung Download Mode / Odin-Loke**
- SharpOdinClient يستطيع اكتشاف واجهة Samsung CDC وقراءة معلومات DVIF، ومنها model وCSC وبعض معلومات firmware/identity بحسب الجهاز.
- يمكنه قراءة PIT، لكن وظائف الكتابة والتفليش ليست ضمن نطاق Read Info.
- المشروع يذكر قيوداً على بعض إصدارات Odin والبروتوكولات الأحدث؛ يجب تسجيل Unsupported عند عدم الدعم.

المراجع:
- [SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)
- [galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool)

### 5.5 Huawei وHonor

توجد أوامر Fastboot/OEM منشورة لبعض الأجهزة، منها:
- fastboot oem get-product-model
- fastboot oem get-build-number
- fastboot oem get-bootinfo
- fastboot getvar vendorcountry

هذه أوامر مرتبطة بتنفيذات Bootloader محددة وليست واجهة موحدة لكل Huawei/Honor. يجب حفظ الأمر والرد والموديل الذي اختُبر عليه كل أمر، وعدم افتراض أن كل حقل مثل IMEI أو Serial متاح من هذه الأوامر.

**قرار التصميم:** HuaweiFastbootExtension اختياري فوق محول Fastboot العام، ويُفعّل فقط عند اكتشاف دعم الأمر أو وجود قاعدة توافق موثقة للموديل.

### 5.6 Motorola

المسار الأساسي هو Android ADB/Fastboot القياسي حيث يكون متاحاً. لا يوجد دليل كافٍ على مجموعة أوامر Motorola موحدة لكل الأجيال:
- استخدم fastboot getvar للمتغيرات التي يعلنها Bootloader.
- افصل Fastbootd عن Bootloader Fastboot.
- لا تستنتج إمكانية فتح Bootloader أو التفليش من مجرد ظهور الجهاز في Fastboot.

مرجع بحثي: [swat-rescue-tool](https://github.com/irixaligned/swat-rescue-tool) — مرجع لتدفق العمل وليس دليلاً على توافق شامل.

### 5.7 OPPO وvivo وOnePlus وRealme

لا توجد أدلة كافية لتعميم أوامر الخدمة أو أوضاع Fastboot على كل موديلات هذه الشركات:
- ابدأ بـADB وMTP، ثم Fastboot فقط إذا ظهر الجهاز فعلياً في هذا الوضع.
- أوامر OEM وService/Download modes تسجل كامتدادات خاصة بموديلات محددة.
- لا تعتمد على اسم مستودع أو أداة صيانة تجارية لإثبات وجود بروتوكول حصري للشركة.

مرجع [fastboot-vivo](https://github.com/AndroPlus-org/fastboot-vivo) يشرح بروتوكول Fastboot العام، ولا يثبت بمفرده وجود بروتوكول vivo مستقل.

### 5.8 مصفوفة أوضاع واتصالات مقترحة

| الشركة / العائلة | الوضع | وسيلة الاتصال | ما يمكن محاولة قراءته | حالة الدليل |
|---|---|---|---|---|
| Android عام | ADB | ADB server / USB | getprop وmodel وbuild وABI وserial المتاح | موثق؛ يتطلب ADB authorization |
| Android عام | Bootloader Fastboot | USB Fastboot | getvar المدعوم وproduct وserial وslot إن توفر | موثق؛ المتغيرات تختلف |
| Android عام | Fastbootd | USB Fastboot | getvar المدعوم وحالة userspace | موثق على الأجهزة الداعمة |
| Android عام | Recovery ADB/Sideload | ADB protocol | حالة sideload ومعلومات محدودة حسب Recovery | مشروط بنوع Recovery |
| Android عام | MTP | Windows WPD | خصائص الجهاز القياسية | موثق جزئياً؛ لا يضمن بيانات النظام |
| Samsung | AT Modem | COM / Serial | DEVCONINFO والحقول التي يعيدها المودم | مشروط بالموديل والواجهة |
| Samsung | Download Mode | Samsung CDC / Odin-Loke | DVIF وبعض بيانات firmware/CSC وPIT | موثق للموديلات والبروتوكولات المدعومة |
| Xiaomi | Mi Assistant | USB + libusb + ADB packet framing | device/version/region/branch/serial وغيرها حسب الاستجابة | تنفيذ مرجعي؛ التوافق الفعلي يحتاج اختبار |
| Huawei / Honor | Fastboot OEM | Fastboot USB | model/build/bootinfo/vendorcountry حسب الجهاز | مشروط بالموديل |
| Motorola | Fastboot | Fastboot USB | المتغيرات التي يعلنها Bootloader | واجهة عامة موثقة؛ التغطية حسب الجهاز |
| OPPO / vivo / OnePlus / Realme | ADB/MTP/Fastboot | البروتوكولات القياسية عند توفرها | البيانات التي يسمح بها الوضع | دعم أساسي مشروط؛ امتدادات الشركة غير مثبتة |

### 5.9 مصفوفة انتقالات الأوضاع

| من | إلى | الآلية | الحكم |
|---|---|---|---|
| Android ADB | Bootloader Fastboot | adb reboot bootloader | قياسي عند دعم الجهاز والسماح بالأمر |
| Android ADB | Recovery | adb reboot recovery | قياسي عند دعم الجهاز والسماح بالأمر |
| Android ADB | Sideload | adb reboot sideload أو مسار Recovery المناسب | يختلف حسب Recovery |
| Android ADB | Samsung Download | adb reboot download | خاص بأجهزة Samsung/التنفيذ الداعم |
| Bootloader Fastboot | النظام العادي | fastboot reboot | قياسي |
| Bootloader Fastboot | Fastbootd | fastboot reboot fastboot | للأجهزة الداعمة |
| Fastbootd | النظام العادي | fastboot reboot | للأجهزة الداعمة |
| Fastboot | Recovery | fastboot reboot recovery | مشروط بتنفيذ Bootloader |
| Samsung AT Modem | Download Mode | أوامر AT منشورة مثل AT+SUDDLMOD=0,0 | غير معمم؛ لا يُنفذ إلا بعد التحقق من الموديل |
| Samsung Download Mode | Normal Mode | إعادة تشغيل الجهاز عبر آلية Odin المدعومة | مشروط؛ لا تفترض أن تهيئة جلسة Odin لازمة دائماً |
| Xiaomi Mi Assistant | Normal/Recovery transitions | أوامر Mi Assistant/Recovery الخاصة | لا تعتمد انتقالاً قبل التحقق من الأمر وسلوك الموديل |

**ملاحظة:** أوامر EDL مثل fastboot oem edl وfastboot reboot-edl وadb reboot edl ليست انتقالات Android عامة، وهي خارج نطاق البحث الحالي.

### 5.10 بنية البرنامج المقترحة

1. **USB Discovery:** تعداد USB devices/interfaces، وVID/PID وHardware IDs، وربط COM ports، ومتابعة تغير الاتصال عند إعادة تشغيل الهاتف.
2. **Mode Detection:** اكتشاف ADB/Fastboot/MTP/Download/AT/Mi Assistant. لا تستنتج الوضع من VID/PID وحده عند وجود أكثر من واجهة.
3. **Protocol Adapters:** AdbAdapter وFastbootAdapter وRecoveryAdapter وWpdMtpAdapter وSamsungAtAdapter وSamsungOdinAdapter وXiaomiMiAssistantAdapter وHuaweiFastbootExtension وVendorExtension.
4. **Unified Device Model:** manufacturer وmodel وcodename وserial وandroidVersion وbuildNumber وsecurityPatch وchipset عند توفره وcurrentMode وconnectionInterfaces وbootloaderState وcapabilities وevidence.
5. **Transition Manager:** رسم بياني للانتقالات المدعومة لكل جهاز؛ كل انتقال يحدد prerequisites والنتيجة المتوقعة والمهلة وإمكانية الرجوع. أعد اكتشاف USB بعد الانتقال ولا تفترض ثبات VID/PID أو COM port.

### 5.11 حالات نتائج موحدة

- Success: تم الحصول على قيمة صالحة.
- Unsupported: الجهاز/الوضع لا يدعم الأمر.
- PermissionDenied: التفويض غير كافٍ.
- DeviceDisconnected: الهاتف اختفى أثناء العملية.
- Timeout: لم يصل رد في المهلة.
- MalformedResponse: الرد غير قابل للتحليل.
- NotReported: البروتوكول لم يقدم المعلومة.

### 5.12 خطة تحقق عملية

لكل موديل يجب تسجيل:
- اسم الموديل ورقم الطراز وإصدار Android/Build.
- وضع USB الظاهر في Windows وVID/PID وHardware IDs.
- اسم التعريف وواجهة الاتصال المستخدمة.
- الأوامر والرد الخام بعد إخفاء البيانات الشخصية.
- الحقول الناجحة وغير المدعومة.
- نتيجة الانتقال ووقت اختفاء الجهاز وعودته والوضع بعد إعادة الاتصال.
- إعادة الاختبار بعد فصل الكابل وإعادة التوصيل.

لا يوصف أي دعم بأنه مؤكد إلا بعد اختبار فعلي على جهاز حقيقي. قبل ذلك استخدم: موثق نظرياً، أو تنفيذ مرجعي موجود، أو يحتاج اختبار.

---

## 6. المراجع التي تمت مراجعتها

- [Android Platform Tools / AOSP](https://source.android.com/docs/setup/test/running)
- [AOSP Fastbootd](https://source.android.com/docs/core/architecture/bootloader/fastbootd)
- [Microsoft Windows Portable Devices](https://learn.microsoft.com/en-us/windows/win32/wpd_sdk/getting-started-with-wpd)
- [SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)
- [galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool)
- [MiAssistantTool](https://github.com/MiForge/MiAssistantTool)
- [xiaomi_adb_sideload](https://github.com/GautamGreat/xiaomi_adb_sideload)
- [FC.SDK](https://github.com/SharpAstro/FC.SDK) — مرجع WPD/PTP وليس SDK عامة لهواتف Android.
- [swat-rescue-tool](https://github.com/irixaligned/swat-rescue-tool)
- [fastboot-vivo](https://github.com/AndroPlus-org/fastboot-vivo) — مرجع Fastboot العام.

## 7. حالة البحث

- [x] مراجعة README وتصحيح الادعاءات العامة غير الدقيقة.
- [x] مراجعة تنفيذ Xiaomi Mi Assistant في الكود.
- [x] فصل Samsung AT عن Samsung Odin/Download Mode.
- [x] توضيح الفرق بين Bootloader Fastboot وFastbootd.
- [x] استبعاد قراءة Qualcomm وMediaTek من النطاق الحالي.
- [ ] اختبار عملي على أجهزة فعلية من الشركات المستهدفة.
- [ ] بناء قاعدة توافق حسب الموديل وإصدار النظام.
- [ ] تحديد أوامر الانتقال الآمنة لكل موديل قبل تنفيذها.

**حالة المستند:** مرجع بحث وتصميم أولي مُدقّق جزئياً، وليس شهادة توافق أجهزة أو مواصفة تنفيذ نهائية.
