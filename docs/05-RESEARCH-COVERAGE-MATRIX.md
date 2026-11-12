# مصفوفة تغطية البحث والوظائف

> **الغرض:** توضيح ما الذي غطته وثائق المستودع فعلياً، وما الذي ورد كإشارة أو مرشح فقط، وما الذي يحتاج بحثاً إضافياً قبل اعتماد تصميم أو مشروع جاهز. هذه مصفوفة بحث وليست إعلاناً عن وظائف منفذة.

**آخر مراجعة:** 12 نوفمبر 2026  
**المنصة المستهدفة:** Windows 10/11 وAndroid 7.0 (API 24) فأحدث.  
**قيود النطاق:** لا بحث في التراخيص أو أوضاع/بروتوكولات Qualcomm وMediaTek. لا اختبارات على أجهزة فعلية في هذه المرحلة.

## 1. طريقة قراءة الحالة

| الحالة | معناها |
|---|---|
| **بحث جيد — غير مختبر** | توجد متطلبات ومراجع ومقارنة أو تدقيق كود مناسب، لكن لم يثبت التشغيل على أجهزتنا. |
| **جزئي** | توجد معلومات ذات صلة، لكنها لا تغطي الوظيفة من البداية للنهاية أو لا تحسم المرشحين والقيود. |
| **إشارة فقط** | ذُكرت الميزة في README أو جدول أدوات أو ملاحظة، دون تدقيق كافٍ للكود أو الاختبارات. |
| **غير مغطى** | لم يظهر بحث كافٍ في الوثائق التي تمت مراجعتها. لا يعني ذلك أن الميزة غير ممكنة. |
| **خارج النطاق** | مستبعد بقرار صريح، ولا ينبغي فتح بحثه عرضاً. |

**مستويات الدليل:** وصف المشروع، مراجعة المصدر، اختبارات المشروع/CI، تجربة Runtime، تجربة Hardware. لا يجوز القفز من مستوى إلى آخر. كل مرشح في الجدول التالي يظل مرشحاً حتى تتم مراجعة المصدر والنسخة والاعتماديات والاختبارات وفق منهجية المستودع.

## 2. مصفوفة تغطية محاور المنتج

| الوظيفة | الوثائق الحالية والأدلة الموجودة | التغطية | الفجوة الحقيقية / الخطوة التالية | وثيقة مستقلة؟ |
|---|---|---|---|---|
| اكتشاف أجهزة Windows وواجهات USB | [01](01-connectivity-protocols.md)؛ [04](04-windows-driver-diagnostics.md)؛ مراجعات Windows/PnP في [02 §20.23–20.25](02-tool-landscape-and-market-research.md) | بحث جيد — غير مختبر | بناء مصفوفة VID/PID وHardware IDs للأجهزة المستهدفة وربطها بالواجهات والأوضاع؛ اختبار التوصيل والفصل وتعدد الأجهزة لاحقاً | لا؛ وسّع 01 و04 |
| ADB USB وWireless ADB وإدارة الجلسات | [01](01-connectivity-protocols.md)؛ [02 §20.16–20.18 و20.27](02-tool-landscape-and-market-research.md)؛ [03 §16](03-app-management-and-network.md) | بحث جيد — غير مختبر | توحيد اكتشاف الأجهزة والاقتران والاتصال والانقطاع والمهلات وتحديد Serial؛ إعداد مصفوفة توافق حسب Android/Platform Tools | لا؛ استكمال الموجود |
| Fastboot/Fastbootd وMTP/WPD والأوضاع الموثقة | [01](01-connectivity-protocols.md) | بحث جيد جزئياً — غير مختبر | فصل ما يمكن قراءته في كل وضع عن الانتقالات الممكنة؛ وضع مصفوفة دعم حسب الموديل، وعدم اعتبار الانتقال المدعوم عاماً | لا؛ استكمال 01 |
| قراءة هوية الجهاز ومعلوماته الأساسية | [01 §2 و§5](01-connectivity-protocols.md)؛ getprop وFastboot getvar في [02](02-tool-landscape-and-market-research.md) | بحث جيد جزئياً | تصميم Device Information Schema: الحقل، المصدر، طريقة الجمع، صلاحية الوصول، API range، الثقة، وحالات Unsupported/PermissionDenied/NotReported | موجودة الآن في [06](06-android-system-information.md)؛ البحث الأولي موثق |
| معلومات النظام والبناء والحماية والعتاد | [00](00-PRODUCT-BASELINE.md)؛ [01](01-connectivity-protocols.md)؛ مراجع getprop/ABI متفرقة في [02](02-tool-landscape-and-market-research.md) | جزئي | بحث مستقل في Android properties وBuild fields وSDK/API وsecurity patch وABI وdensity وfeatures وkernel/runtime، مع توثيق اختلاف OEM وإخفاء البيانات الحساسة | نعم: 06 |
| معلومات السوفتوير والفريموير وملفات التحديث | [06](06-android-system-information.md) لمعلومات build المثبتة؛ [07](07-firmware-software-inventory.md) لـOTA metadata وpayload manifest وAVB؛ [01](01-connectivity-protocols.md) و[02](02-tool-landscape-and-market-research.md) لمراجع إضافية | بحث جيد جزئياً — غير مختبر | توسيع صيغ OTA legacy وfactory archives خارج payload.bin، وبناء corpus حزم متعددة؛ لا تفليش ولا كتابة أقسام | الوثيقة المستقلة موجودة |
| الأقسام ومعلومات التخزين | [06](06-android-system-information.md) لمساحات filesystem وRAM؛ [08](08-storage-partition-information.md) لـphysical/logical/dynamic partitions وAOSP lpdump/lpunpack | بحث جيد جزئياً — غير مختبر | اختبار parser على super.img/raw/sparse وقراءة Fastboot variables على أجهزة متعددة؛ لا raw partition dump أو write | الوثيقة المستقلة موجودة |
| إدارة التطبيقات والجرد والفلاتر | [03 §16–17](03-app-management-and-network.md)؛ تدقيق Droidsmith وAppManager وUAD-ng في [02 §20.17–20.21 و20.26 و20.29](02-tool-landscape-and-market-research.md) | بحث جيد — غير مختبر | حسم نموذج بيانات موحد للتطبيقات، المستخدمين، تواريخ التثبيت/التحديث، installer source، الحالة، ABI، الصلاحيات، ومسارات التصدير؛ اختبار الفروقات بين Android/OEM | لا؛ الوثيقة الحالية مناسبة |
| تثبيت APK وSplit APK وAPKS وAPKM وXAPK وOBB وAAB | [03 §18](03-app-management-and-network.md)؛ [02 §20.30–20.54](02-tool-landscape-and-market-research.md) | بحث موسع جيد — غير مختبر | إعداد corpus حقيقي للملفات المتنوعة، وتثبيت/تحقق على أجهزة مستهدفة؛ لا تُعتبر مراجعة parser أو CI دليلاً على نجاح التثبيت الفعلي | لا؛ استكمال الاختبارات في 02/03 |
| البحث والتنزيل من APKPure وF-Droid وAPKMirror | [02 §20.55](02-tool-landscape-and-market-research.md) | بحث أولي جيد | تدقيق كل Provider Adapter من حيث البحث والإصدارات والvariants وروابط التنزيل والـredirects والـhash وقيود المصدر، مع اختبار تنزيلات عينات؛ لا تجاوز CAPTCHA/Cloudflare | لا؛ استكمال 02 |
| استعراض الملفات ونقلها وإدارتها | [09](09-device-file-management.md)؛ تدقيق ADB-Explorer وAdbFileManager وWinDroidFS وJSleim/adb-file-explorer؛ مراجع Microsoft WPD | بحث جيد — غير مختبر | اختبار Runtime/Hardware، استكمال MTP provider عملياً، والتحقق من نقل ملفات كبيرة ومتعددة الأجهزة | الوثيقة المستقلة موجودة |
| Logcat وسجلات النظام والأحداث | [10](10-android-diagnostics-and-logs.md)؛ تدقيق AdbGui Logcat Core وLogcat Viewer وLogCater وLogFilter وSherlog | بحث جيد — غير مختبر | تدقيق wiring والبناء والاختبارات وعمليات Runtime؛ مراجعة privacy/retention وحدود stream/cancel على أجهزة فعلية | الوثيقة المستقلة موجودة |
| Bugreport وdumpsys وdumpstate والتقارير | [10](10-android-diagnostics-and-logs.md)؛ مراجع AOSP وAndroid Studio وPerfetto، ومراجعة AdbGui bugreport command | بحث جيد — غير مختبر | اختبارات ZIP corpus من إصدارات/OEM مختلفة، اختبار cancellation/progress، وفحص خصوصية/سلامة الأرشيف | الوثيقة المستقلة موجودة |
| العمليات والخدمات والذاكرة والبطارية والتشخيص | [10](10-android-diagnostics-and-logs.md)؛ مراجع ADB وLogCater/AdbGui | جزئي | توسيع كتالوج أوامر القراءة واختبار اختلاف مخرجات ps/dumpsys حسب API/OEM؛ لا إيقاف عمليات أو خدمات تلقائياً | ضمن 10 |
| النسخ الاحتياطي والاستعادة | [11](11-backup-restore-boundaries.md)؛ Android Auto Backup/adb backup limits؛ مراجعة chen3feng/android-backup وOpen Android Backup وAndroid Backup Extractor وJigar Tools | بحث جيد — غير مختبر | اختبار backup/restore فعلياً على Android API 24+، واختبار manifest/hash/incremental/conflict handling | الوثيقة المستقلة موجودة |
| DNS وPrivate DNS وحجب الإعلانات | [03 §17.1 و17.3–17.4](03-app-management-and-network.md)؛ [02 §20.22](02-tool-landscape-and-market-research.md) | بحث جيد — غير مختبر | تثبيت مصفوفة اختلافات Android 7+ وOEM وWi-Fi/mobile؛ اختبار Preview/Apply/Verify/Restore لاحقاً؛ توثيق متطلبات Local VPN منفصلة عن ADB DNS | لا؛ الموجود مناسب |
| تعريفات Windows وتشخيصها وإصلاحها | [04](04-windows-driver-diagnostics.md)؛ [02 §20.23–20.25](02-tool-landscape-and-market-research.md) | بحث جيد — غير مختبر | استكمال Hardware-ID matrix واختبار Windows 10/11 وrollback؛ الالتزام بالمصادر الرسمية فقط | لا؛ الموجود مناسب |
| MCP وربط Codex بطبقة خدمات البرنامج | [02 §20.1–20.20](02-tool-landscape-and-market-research.md)؛ [RESEARCH-METHODOLOGY](RESEARCH-METHODOLOGY.md) | بحث معماري وتدقيق مرشحين جيد — غير مختبر | تثبيت عقود الأدوات والـschemas وApproval Tokens وtimeouts/cancellation وعمليات الجهاز المحدد؛ اختبار MCP فعلياً مع Codex بعد وجود Service Layer | لا؛ استكمال 02 والمنهجية |
| الأمان وسجل العمليات والموافقات | [00](00-PRODUCT-BASELINE.md)؛ [02 §20.11 و20.14–20.16](02-tool-landscape-and-market-research.md)؛ [RESEARCH-METHODOLOGY](RESEARCH-METHODOLOGY.md) | بحث جيد على مستوى التصميم | تحويل السياسات إلى threat model واختبارات لكل adapter؛ لا اعتبار annotation أو confirmation flag وحده موافقة مخولة | لا؛ تطوير المنهجية |

## 3. مشاريع وأدوات جاهزة ظهرت في البحث — دون اعتماد تلقائي

هذه قائمة توجيهية للعثور على التفاصيل الموجودة بالفعل في [02](02-tool-landscape-and-market-research.md)، وليست توصية بدمج كل مشروع:

| المجال | المرشحون/المراجع الموجودة في البحث | ما نحتاج التحقق منه قبل إعادة الاستخدام |
|---|---|---|
| واجهة عامة وADB/Fastboot | ATA-GUI، VioletToolBox، QADB، Droidsmith | فصل المكونات، تغطية الأوضاع، Windows runtime، جودة العمليات، وما إذا كانت الوظائف قابلة لإعادة الاستخدام خارج الواجهة |
| ADB وإدارة التطبيقات | Droidsmith، AppManager، UAD-ng، ADB Explorer | جرد الحزم، تعدد المستخدمين، التواريخ، export، wireless، الأمان، وإدارة العمليات |
| تثبيت الصيغ | Google bundletool، SAI، apkfile، xapkInstaller، pacman-apk، Android Application Installer، AdvancedSharpAdbClient | corpus عينات، صحة parser، التوقيع، توافق splits، ZIP safety، واختبار تثبيت فعلي |
| MCP | iksnerd/adb-mcp، us-all/android-mcp-server، Droidsmith MCP | لا نقل Shell handlers دون تدقيق؛ صلاحيات حقيقية، device targeting، cancellation، output limits، وعزل العمليات |
| الملفات | AdbExplorer، QADB، Droidsmith file/path-grant components | نقل الملفات الكبيرة، path grants، عدم قبول arbitrary host paths من MCP، وفصل UI عن core |
| Logcat والتشخيص | ATA-GUI، QADB، Android Multitool Universal، مراجع ADB/MCP في 02 | دعم streaming/filters/export، دورة حياة الجلسة، حفظ السجلات، حدود الصلاحيات والخصوصية |
| تعريفات Windows | PnPUtil، SetupAPI/Configuration Manager، Driver Store Explorer كمرجع | لا تثبيت تلقائي؛ مطابقة Hardware IDs ومصادر Microsoft/Google/OEM والتحقق والتراجع |

## 4. حالة الوثائق المستقلة

تم إنشاء الوثائق الست التي حددتها المراجعة السابقة:

- [06 — Android System Information](06-android-system-information.md)
- [07 — Firmware & Software Inventory](07-firmware-software-inventory.md)
- [08 — Storage & Partition Information](08-storage-partition-information.md)
- [09 — Device File Management](09-device-file-management.md)
- [10 — Android Diagnostics & Logs](10-android-diagnostics-and-logs.md)
- [11 — Backup & Restore Boundaries](11-backup-restore-boundaries.md)

لا نحتاج فتح وثائق مستقلة إضافية حالياً. الفجوات المتبقية تسجل داخل هذه الوثائق أو ملفات المجال الحالية، وتتحول إلى عمل جديد فقط إذا ظهر سبب تقني واضح.

## 5. ترتيب العمل البحثي بعد المصفوفة

| الترتيب | الجولة | ناتج البحث المطلوب |
|---:|---|---|
| 1 | تم إنجاز 06 — Android System Information | بحث أولي ومراجعة مصدر Host-side وAndroid-side؛ لا اختبار Hardware ولا اعتماد دمج |
| 2 | تم إنجاز 10 — Android Diagnostics & Logs | بحث ومراجعة مصدر لمرشحي Logcat وBugreport وDropbox وPerfetto؛ لا تشغيل أو اختبار Hardware |
| 3 | تم إنجاز 09 — Device File Management | تدقيق مشاريع ADB-Explorer وAdbFileManager وWinDroidFS وJSleim ومراجع WPD؛ لا اختبار Hardware |
| 4 | تم إنجاز 07 — Firmware & Software Inventory | مراجعة AOSP OTA metadata وupdate_metadata.proto وpayload-dumper-go وAVB؛ لا extraction أو flash |
| 5 | تم إنجاز 08 — Storage & Partition Information | مراجعة AOSP liblp/lpdump وlpunpack-go وlucky-arch وlpunpack_cyth؛ لا اختبار Hardware أو extraction |
| 6 | تم إنجاز 11 — Backup & Restore Boundaries | حدود Android Auto Backup وadb backup، ومراجعة أدوات incremental/legacy/Windows؛ لا اختبار Backup/Restore فعلي |
| 7 | تحديث مصفوفة التغطية | ربط كل نتيجة بمشروع ومصدر وملف/قسم ودليل وحالة تحقق، ثم تحديث 00 وREADME |

## 6. قواعد تحديث هذه المصفوفة

- لا تغيّر «جزئي» إلى «بحث جيد» لمجرد إضافة روابط؛ يجب أن توجد مقارنة تقنية تغطي الوظيفة ومخاطرها وحدودها.
- لا تغيّر «بحث جيد — غير مختبر» إلى «تم التحقق» دون سجل اختبار يحدد Windows build والجهاز والموديل وإصدار Android والأداة والإصدار والخطوات والنتيجة.
- أي مشروع جاهز يضاف يجب أن يمر ببطاقة التقييم في [RESEARCH-METHODOLOGY.md](RESEARCH-METHODOLOGY.md) وبوابات الأمان في [02](02-tool-landscape-and-market-research.md).
- لا ننسخ مشروعاً كاملاً قبل تحديد المكونات القابلة للفصل وإعادة الاستخدام وحدود الاعتماد على GUI أو shell.
- لا نخلط تصدير APK بنسخ بيانات التطبيق، ولا قراءة معلومات الأقسام بالتفليش، ولا قراءة إعداد DNS بتغييره.
- عند إضافة دليل خارجي، سجّل الرابط وتاريخ المراجعة ونوع الدليل وما الذي يثبته وما الذي لا يثبته.
- حافظ على ملف 02 كسجل تدقيق تراكمي، ولا تنقل محتواه بالكامل إلى وثائق المجالات.

## 7. مراجع رسمية تأسيسية أضيفت لهذه المراجعة

هذه المراجع تثبت وجود واجهات ADB العامة فقط؛ ولا تثبت توافق كل أمر أو صلاحية على كل هاتف:

- [Android Debug Bridge (ADB) — Android Developers](https://developer.android.com/tools/adb): تحديد الجهاز، shell، push/pull، wireless debugging، ومرجع أوامر ADB.
- [Read bug reports — Android Open Source Project](https://source.android.com/docs/core/tests/debug/read-bug-reports): يشرح محتويات bugreport بما فيها dumpsys وdumpstate وlogcat.
- [Capture and read bug reports — Android Studio](https://developer.android.com/studio/debug/bug-report): إنشاء تقرير bugreport وقراءة ملف ZIP.
- [Logcat command-line tool — Android Developers](https://developer.android.com/tools/logcat): buffers وfilters وصيغ السجلات وحدود بعض الخيارات والصلاحيات.

**حالة المصفوفة:** جميع الوثائق المستقلة المقترحة 06–11 أُنشئت بعد مراجعة المصادر. لا يوجد ادعاء بتشغيل الأدوات أو اختبار Hardware. لا يوجد ادعاء باختبار Hardware أو اعتماد مشروع للدمج.
