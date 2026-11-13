# مصفوفة تغطية البحث والوظائف

> **الغرض:** توضيح ما الذي غطته وثائق المستودع فعلياً، وما الذي ورد كإشارة أو مرشح فقط، وما الذي يحتاج بحثاً إضافياً قبل اعتماد تصميم أو مشروع جاهز. هذه مصفوفة بحث وليست إعلاناً عن وظائف منفذة.

**آخر مراجعة:** 13 نوفمبر 2026  
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
| معلومات النظام والبناء والحماية والعتاد | [06](06-android-system-information.md)؛ [01](01-connectivity-protocols.md)؛ [02](02-tool-landscape-and-market-research.md) | بحث جيد — غير مختبر | corpus من أجهزة API 24+، وتدقيق parsers على مخرجات متعددة، واختبارات Runtime/Hardware | الوثيقة المستقلة موجودة |
| معلومات السوفتوير والفريموير وملفات التحديث | [06](06-android-system-information.md) لمعلومات build المثبتة؛ [07](07-firmware-software-inventory.md) لـOTA metadata وpayload manifest وAVB؛ [01](01-connectivity-protocols.md) و[02](02-tool-landscape-and-market-research.md) لمراجع إضافية | بحث جيد جزئياً — غير مختبر | توسيع صيغ OTA legacy وfactory archives خارج payload.bin، وبناء corpus حزم متعددة؛ لا تفليش ولا كتابة أقسام | الوثيقة المستقلة موجودة |
| الأقسام ومعلومات التخزين | [06](06-android-system-information.md) لمساحات filesystem وRAM؛ [08](08-storage-partition-information.md) لـphysical/logical/dynamic partitions وAOSP lpdump/lpunpack | بحث جيد جزئياً — غير مختبر | اختبار parser على super.img/raw/sparse وقراءة Fastboot variables على أجهزة متعددة؛ لا raw partition dump أو write | الوثيقة المستقلة موجودة |
| إدارة التطبيقات والجرد والفلاتر | [03 §16–17](03-app-management-and-network.md)؛ تدقيق Droidsmith وAppManager وUAD-ng في [02 §20.17–20.21 و20.26 و20.29](02-tool-landscape-and-market-research.md) | بحث جيد — غير مختبر | حسم نموذج بيانات موحد للتطبيقات، المستخدمين، تواريخ التثبيت/التحديث، installer source، الحالة، ABI، الصلاحيات، ومسارات التصدير؛ اختبار الفروقات بين Android/OEM | لا؛ الوثيقة الحالية مناسبة |
| تثبيت APK وSplit APK وAPKS وAPKM وXAPK وOBB وAAB | [03 §18](03-app-management-and-network.md)؛ [02 §20.30–20.54](02-tool-landscape-and-market-research.md) | بحث موسع جيد — غير مختبر | إعداد corpus حقيقي للملفات المتنوعة، وتثبيت/تحقق على أجهزة مستهدفة؛ لا تُعتبر مراجعة parser أو CI دليلاً على نجاح التثبيت الفعلي | لا؛ استكمال الاختبارات في 02/03 |
| البحث والتنزيل من APKPure وF-Droid وAPKMirror | [02 §20.55](02-tool-landscape-and-market-research.md) | بحث أولي جيد | تدقيق كل Provider Adapter من حيث البحث والإصدارات والvariants وروابط التنزيل والـredirects والـhash وقيود المصدر، مع اختبار تنزيلات عينات؛ لا تجاوز CAPTCHA/Cloudflare | لا؛ استكمال 02 |
| استعراض الملفات ونقلها وإدارتها | [09](09-device-file-management.md)؛ تدقيق ADB-Explorer وAdbFileManager وWinDroidFS وJSleim/adb-file-explorer؛ مراجع Microsoft WPD | بحث جيد — غير مختبر | اختبار Runtime/Hardware، استكمال MTP provider عملياً، والتحقق من نقل ملفات كبيرة ومتعددة الأجهزة | الوثيقة المستقلة موجودة |
| **Logcat وسجلات النظام — محرك التحليل** | [10](10-android-diagnostics-and-logs.md) + **[21](21-TRANSPLANT-LOGCAT.md)**: كود منقول (parser 6 صيغ، فهرس mmap، فلترة SIMD، crash extract) مع 10 ملفات اختبار منقولة | **كود منقول حرفياً — لم يُبنَ بعد عندنا** | تشغيل `scripts/verify-transplant.sh` على جهاز المستخدم؛ ثم قياس على ملفات كبيرة | **نعم: [21](21-TRANSPLANT-LOGCAT.md)** |
| Logcat المباشر (streaming من الجهاز) | [10](10-android-diagnostics-and-logs.md)؛ `LogcatStream.cs` غير منقول | تصميم — يحتاج جهازاً | نقل `LogcatStream` + `FakeAdbSocket` بعد توفر `AdvancedSharpAdbClient` | ضمن 10 و21 |
| Bugreport وdumpsys وdumpstate والتقارير | [10](10-android-diagnostics-and-logs.md)؛ مراجع AOSP وAndroid Studio وPerfetto، ومراجعة AdbGui bugreport command | بحث جيد — غير مختبر | اختبارات ZIP corpus من إصدارات/OEM مختلفة، اختبار cancellation/progress، وفحص خصوصية/سلامة الأرشيف | الوثيقة المستقلة موجودة |
| العمليات والخدمات والذاكرة والبطارية والتشخيص | [10](10-android-diagnostics-and-logs.md)؛ مراجع ADB وLogCater/AdbGui | جزئي | توسيع كتالوج أوامر القراءة واختبار اختلاف مخرجات ps/dumpsys حسب API/OEM؛ لا إيقاف عمليات أو خدمات تلقائياً | ضمن 10 |
| النسخ الاحتياطي والاستعادة | [11](11-backup-restore-boundaries.md)؛ Android Auto Backup/adb backup limits؛ مراجعة chen3feng/android-backup وOpen Android Backup وAndroid Backup Extractor وJigar Tools | بحث جيد — غير مختبر | اختبار backup/restore فعلياً على Android API 24+، واختبار manifest/hash/incremental/conflict handling | الوثيقة المستقلة موجودة |
| DNS وPrivate DNS وحجب الإعلانات | [03 §17.1 و17.3–17.4](03-app-management-and-network.md)؛ [02 §20.22](02-tool-landscape-and-market-research.md) | بحث جيد — غير مختبر | تثبيت مصفوفة اختلافات Android 7+ وOEM وWi-Fi/mobile؛ اختبار Preview/Apply/Verify/Restore لاحقاً؛ توثيق متطلبات Local VPN منفصلة عن ADB DNS | لا؛ الموجود مناسب |
| تعريفات Windows وتشخيصها وإصلاحها | [04](04-windows-driver-diagnostics.md)؛ [02 §20.23–20.25](02-tool-landscape-and-market-research.md) | بحث جيد — غير مختبر | استكمال Hardware-ID matrix واختبار Windows 10/11 وrollback؛ الالتزام بالمصادر الرسمية فقط | لا؛ الموجود مناسب |
| MCP وربط Codex بطبقة خدمات البرنامج | [02 §20.1–20.20](02-tool-landscape-and-market-research.md)؛ [15](15-source-revalidation-mcp-codex.md)؛ [RESEARCH-METHODOLOGY](RESEARCH-METHODOLOGY.md) | بحث معماري وتدقيق مرشحين جيد — غير مختبر | بناء الخادم فوق Application Services واختباره مع Codex؛ لا write tools في V1 | لا؛ استكمال 02 و15 |
| **العقود المشتركة (Device/Capability/Operation/Result/Plan/Audit)** | [19](19-SHARED-CONTRACTS.md) و[`contracts/schemas`](../contracts/schemas/) | **مواصفة منفذة ومُتحقَّق منها** (45 فحصاً، 10 حالات سلبية) — لا كود C# بعد | توليد الأنواع في .NET + `Contracts.Tests` ترفض نفس الحالات السلبية | **نعم: [19](19-SHARED-CONTRACTS.md)** |
| **الأمان وسجل العمليات والموافقات** | [Threat Model](../contracts/THREAT-MODEL.md) و[18 §5](18-ENGINEERING-DECISIONS.md) و[19 §3.3](19-SHARED-CONTRACTS.md) | **threat model مُصمَّم بـ13 مسار هجوم؛ ضابطان مُنفَّذان فقط** | كتابة اختبارات T1/T2/T3/T7/T11 بعد بناء الـcorpus؛ T8 مغطى عبر العقود | **نعم: Threat Model** |
| **Test Corpus وfixtures** | [`contracts/corpus`](../contracts/corpus/README.md) | **مواصفة كاملة، لا fixtures مولَّدة** | توليد الفئتين أ وC بسكربت (لا تحتاج جهازاً) | **نعم: corpus README** |
| **قرار الستاك وأرقام التشغيل** | [17](17-adr-0001-stack-decision.md) و[18](18-ENGINEERING-DECISIONS.md) | **محسوم** (ما عدا تغطية الأجهزة) | قياس الأرقام على جهاز حقيقي عند توفره | **نعم: 17 و18** |

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

## 5. ترتيب العمل بعد اكتمال الوثائق المستقلة

اكتمل إنشاء الوثائق المستقلة 06–11. لم تعد الجولة التالية إنشاء وثائق أخرى؛ بل تحويل البحث إلى مرشحين مثبتين وخطة PoC موحدة:

| الترتيب | الجولة | الناتج المطلوب | حالة البداية |
|---:|---|---|---|
| 1 | Source Revalidation | مراجعة الفروق عند SHAs المثبتة، وفصل commit المراجع عن HEAD الملتقط | **✅ مكتمل على جولتين** — الجولة 1 في [13](13-source-revalidation-adb-file-core.md)/[14](14-source-revalidation-app-installers.md)/[15](15-source-revalidation-mcp-codex.md)/[16](16-source-revalidation-firmware-storage-backup.md)، والجولة 2 في [20](20-source-revalidation-round2.md) (6 مشاريع، 4 تصحيحات لوثائق قائمة) |
| 2 | قرار Stack | Host/UI/ADB/MTP/MCP/dependency delivery | **✅ محسوم:** [ADR-0001](17-adr-0001-stack-decision.md) — .NET 10 + WPF + AdvancedSharpAdbClient + CsWin32 + SQLite؛ تفاصيل [18](18-ENGINEERING-DECISIONS.md) |
| 3 | Shared Contracts | DeviceHandle وCapability وOperation وResult وPlan وAudit | **✅ مواصفة منفذة:** [19](19-SHARED-CONTRACTS.md) + `contracts/` — 45 فحصاً يعمل، 10 حالات سلبية. كود C# لم يُكتب |
| 4 | Threat Model | مسارات هجوم + ضوابط + اختبار لكل ضابط | **✅ مُصمَّم:** [Threat Model](../contracts/THREAT-MODEL.md) — 13 مساراً؛ **ضابطان فقط مُنفَّذان (T8 باختبار يعمل)** |
| 5 | Test Corpus | fixtures مولَّدة (أ وج) ثم مستوردة (د) | **مواصفة كاملة، لم تُولَّد** — [`contracts/corpus`](../contracts/corpus/README.md). أ وج لا تحتاجان جهازاً → التالي |
| 6 | Logcat Core | محرك التحليل والتشخيص | **✅ كود منقول:** [21](21-TRANSPLANT-LOGCAT.md) — 9 ملفات كود + 10 اختبار. **البناء لم يُتحقق منه هنا** (بيئة بلا .NET) |
| 7 | Host Core PoC | تنفيذ النواة على `win-x64`: ADB USB/Wireless، جهازان، cancellation، disconnect/reconcile، ثم Windows PnP | Not Started — **يحتاج جهاز اختبار** |
| 8 | Runtime Matrix | Windows 10/11 وAndroid API 24+ مع فصل Unit/Parser/Runtime/Hardware | Not Started — **يحتاج جهاز اختبار** |
| 9 | Implementation Approval | موافقة تنفيذ مستقلة قبل أي تغيير في البرنامج أو الجهاز | Required Gate (الجهاز فقط؛ كتابة الكود داخل النطاق المسموح) |

التفاصيل والمعايير وترتيب المحاور في [12 — خطة إعادة الاستخدام والمعمارية والتنفيذ](12-REUSE-AND-IMPLEMENTATION-PLAN.md)، ونتائج الجولتين في [13](13-source-revalidation-adb-file-core.md) و[14](14-source-revalidation-app-installers.md) و[15](15-source-revalidation-mcp-codex.md) و[16](16-source-revalidation-firmware-storage-backup.md) و[20](20-source-revalidation-round2.md). القرارات التنفيذية في [17](17-adr-0001-stack-decision.md) و[18](18-ENGINEERING-DECISIONS.md).

> **⚠️ اكتشاف مهم من الجولة الثانية (يجب ألا يُفقد):** `daitouniao/logcat-viewer` مبني على **.NET 10 + WinForms + AdvancedSharpAdbClient 3.6.16** مع 852 اختباراً وتغطية 94%، وليس مجرد "مرجع أداء" كما كان موصوفاً. هذا يجعله أعلى قيمة اقتباس بين كل المرشحين — لكن فيه فجوة حرجة: `Shell(serial, cmd, timeoutSec)` **لا يستخدم المهلة**. راجع [20 §2.1](20-source-revalidation-round2.md).

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

**حالة المصفوفة:** جميع الوثائق المستقلة المقترحة 06–11 أُنشئت بعد مراجعة المصادر. لم يتم تشغيل الأدوات أو اختبار Hardware أو اعتماد أي مشروع للدمج.
