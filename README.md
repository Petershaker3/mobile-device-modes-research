# دليل برمجة أداة قراءة بيانات الهواتف والتحويل بين الأوضاع (Mobile Device Modes & Read Info Reference)

مرجع تقني شامل لبناء برنامج خدمي لفنيي السوفتوير لقراءة بيانات الهواتف الذكية (Read Info) والتحويل البرمجي بين الأوضاع المختلفة (Mode Switching) عبر كافة الشركات والمعالجات.

## 1. طبقات الاتصال الأساسية في نظام ويندوز

يعتمد أي برنامج سوفتوير متكامل على 4 قنوات اتصال رئيسية للتخاطب مع الهواتف:
1. **طبقة الأوامر القياسية (`ADB & Fastboot`):** عبر مكتبات [Android Platform Tools](https://source.android.com/docs/setup/test/running) للتعامل مع أوضاع التصحيح والبوتلودر والسايدلود.
2. **طبقة المنافذ التسلسلية (`SerialPort / COM`):** لإرسال أوامر `AT Commands` لمودم سامسونج ([galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool)) وحزم بروتوكول `Odin / LOKE` في وضع الداونلود مود ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)).
3. **طبقة أجهزة الوسائط المحمولة (`Windows Portable Devices - WPD API`):** لقراءة بيانات الهاتف في وضع `MTP` القياسي لجميع الشركات بدون الحاجة لتفعيل تصحيح USB ([FC.SDK WPD Transport](https://github.com/SharpAstro/FC.SDK)).
4. **طبقة الاتصال المباشر عبر USB (`libusb / WinUSB`):** للتعامل مع أوضاع الطوارئ للمعالجات مثل `Qualcomm EDL 9008` ([bkerler/edl](https://github.com/bkerler/edl)) و `MediaTek BROM / Preloader` ([bkerler/mtkclient](https://github.com/bkerler/mtkclient)).

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
* **وضع MTP العام لجميع الشركات (WPD API):** استخدام واجهة `Windows Portable Devices` وإرسال الكود `GetDeviceInfo (0x1001)` لاستخراج اسم الشركة، والموديل، ورقم السيريال، وإصدار النظام بدون الحاجة لتفعيل ADB ([FC.SDK WPD Transport](https://github.com/SharpAstro/FC.SDK)).
* **وضع MTP لأجهزة سامسونج (AT Commands):** فتح منفذ `Samsung Mobile USB Modem` بسرعة `115200` وإرسال الأمر `AT+DEVCONINFO\r\n` (مع إرسال `AT+ACTIVATE=0,0,0\r\n` أولاً في الحمايات الحديثة) ([galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool)، [nPhoneKIT Issue #28](https://github.com/nlckysolutions/nPhoneKIT/issues/28)).
* **تحليل رد سامسونج (`+DEVCONINFO`):** يعيد الجهاز البيانات بصيغة `MN(SM-A146U);BASE(...);VER(...);HIDVER(...);PRD(...);IMEI(...);SN(...);UN(...)` لاستخراج الموديل، وإصدار الفيرموير والحماية، والتوجيه `CSC`، والـ `IMEI`، والسيريال ([nPhoneKIT Issue #28](https://github.com/nlckysolutions/nPhoneKIT/issues/28)، [INESC TEC Research](https://repositorio.inesctec.pt/server/api/core/bitstreams/54e039b4-ded3-4cce-9012-c4851faac24b/content)).

### هـ. وضع الداونلود مود لسامسونج (Samsung Download Mode / Odin Protocol)
يتصل البرنامج بمنفذ `Samsung Mobile USB CDC Composite` باستخدام بروتوكول `Odin / LOKE` ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)):
* **قراءة معلومات الجهاز السريعة (`DVIF`):** إرسال الأمر `DVIF` عبر المنفذ التسلسلي للحصول على الموديل، ورمز `CSC`، وإصدار الحماية، والمعرف الفريد `UN` ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)، [Sharp Odin Protocol Gist](https://gist.github.com/bepiy7i8d/17cf7826ffce4239a35b26fefcfac8ae)).
* **تهيئة جلسة Odin وقراءة `PIT`:** إرسال `ODIN` واستقبال `LOKE` (`IsOdin` ثم `LOKE_Initialize`) لقراءة جدول التقسيمات `PIT` أو تفليش الجهاز ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)).

### و. وضع السايدلود لشاومي (Xiaomi Sideload / Mi Assistant)
عند توصيل جهاز شاومي في وضع `Connect with MiAssistant`، يتخاطب البرنامج مع خدمة ADB على المنفذ `5037` بأوامر شاومي الخاصة ([xiaomi_adb_sideload](https://github.com/GautamGreat/xiaomi_adb_sideload/blob/main/main.c)):
* **الأوامر المدعومة:** إرسال `getdeviceinfo:`، و `getversion:`، و `getromzone:`، و `getcodebase:` عبر مقبس ADB لاستخراج الاسم الرمزي للجهاز (Codename)، وإصدار الروم، والمنطقة، وحالة البوتلودر ([xiaomi_adb_sideload](https://github.com/GautamGreat/xiaomi_adb_sideload/blob/main/main.c)).

### ز. أوضاع الطوارئ للمعالجات (Qualcomm EDL 9008 & MediaTek BROM)
* **كوالكوم (EDL 9008):** قراءة `MSM_HW_ID`، و `OEM_PK_HASH`، ورقم السيريال عبر بروتوكول `Sahara` باستخدام مكتبة [bkerler/edl](https://github.com/bkerler/edl).
* **ميدياتك (BROM / Preloader):** قراءة `HW_CODE` وحالة الحماية (`SBC / SLA / DAA`) عبر بروتوكول BROM باستخدام مكتبة [bkerler/mtkclient](https://github.com/bkerler/mtkclient).

## 3. مصفوفة التحويل بين الأوضاع برمجياً (Mode Switching)

| من الوضع (Source) | إلى الوضع (Target) | الأمر أو الحزمة البرمجية (Command / Packet) | القناة والمصدر المرجعي |
| :--- | :--- | :--- | :--- |
| **Samsung MTP** | **Download Mode** | إرسال `AT+FUS?\r\n` أو `AT+SUDDLMOD=0,0\r\n` | منفذ `Samsung USB Modem` ([Mobile-Diagnostics-Research](https://github.com/fsinnes/Mobile-Diagnostics-Research)، [DFReroot D2](https://github.com/polygraphene/DFReroot/blob/main/D2_Error.md)) |
| **Download Mode** | **Normal Mode** | تهيئة `ODIN` ثم `LOKE_Initialize` ثم `PDAToNormal()` | بروتوكول Odin عبر `COM Port` ([SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient)) |
| **ADB Mode** | **Fastboot / Bootloader** | `adb reboot bootloader` | واجهة `ADB` القياسية ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **ADB Mode** | **Recovery / Sideload** | `adb reboot recovery` أو `adb reboot sideload` | واجهة `ADB` القياسية ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **ADB Mode** | **Samsung Download** | `adb reboot download` | واجهة `ADB` لأجهزة سامسونج ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **ADB Mode** | **Qualcomm EDL 9008** | `adb reboot edl` | واجهة `ADB` لأجهزة كوالكوم ([bkerler/mtkclient](https://github.com/wmhcarter2024/bkerler-mtkclient/blob/main/README.md)) |
| **Fastboot Mode** | **Normal Mode** | `fastboot reboot` أو `fastboot continue` | واجهة `Fastboot` القياسية ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **Fastboot Mode** | **Fastbootd (Userspace)** | `fastboot reboot fastboot` | واجهة `Fastboot` للأجهزة الحديثة ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **Fastboot Mode** | **Recovery Mode** | `fastboot reboot recovery` | واجهة `Fastboot` القياسية ([Android Open Source Project](https://source.android.com/docs/setup/test/running)) |
| **Fastboot Mode** | **Qualcomm EDL 9008** | `fastboot oem edl` أو `fastboot reboot-edl` | واجهة `Fastboot` ([Scribd Fastboot Logs](https://www.scribd.com/doc/191196433/HTC-ONE-Fastboot-Getvar-All)) |
| **Xiaomi Sideload** | **Normal Mode** | `adb reboot` (أو إرسال `reboot:` عبر مقبس ADB) | واجهة `Mi Assistant Sideload` ([xiaomi_adb_sideload](https://github.com/GautamGreat/xiaomi_adb_sideload/blob/main/main.c)) |

## 4. المكتبات المفتوحة المصدر المقترحة للتنفيذ
* **[SharpOdinClient](https://github.com/Alephgsm/SharpOdinClient):** مكتبة C# متكاملة للتعامل مع وضع الداونلود في أجهزة سامسونج بدون الحاجة لملف `SS_DL.dll`.
* **[galaxy-at-tool](https://github.com/apeppels/galaxy-at-tool):** أداة ومكتبة Python للتعامل مع أوامر `AT+DEVCONINFO` لأجهزة سامسونج عبر منفذ المودم.
* **[xiaomi_adb_sideload](https://github.com/GautamGreat/xiaomi_adb_sideload/blob/main/main.c):** تطبيق عملي بلغة C لبروتوكول شاومي `Mi Assistant Sideload`.
* **[bkerler/edl](https://github.com/bkerler/edl) & [bkerler/mtkclient](https://github.com/bkerler/mtkclient):** المراجع الأساسية للتعامل مع معالجات كوالكوم وميدياتك في أوضاع البوت روم والطوارئ.
