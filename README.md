# Android Device Modes & Windows Toolkit Research

مرجع بحثي لتصميم منصة Windows متكاملة للتعامل مع أجهزة Android: الاتصال وقراءة البيانات وفهم الأوضاع ومعلومات السوفتوير وإدارة التطبيقات والملفات والتشخيص والشبكة وتعريفات Windows، مع إتاحة الوظائف لوكلاء الذكاء الاصطناعي عبر MCP.

> **حالة المستودع:** بحث وتوثيق وتصميم متطلبات. لا يوجد حتى الآن تطبيق منتج مكتمل أو ادعاء باختبار كل الوظائف على أجهزة فعلية.

## ابدأ من هنا

1. **[خط الأساس ومتطلبات المنتج](docs/00-PRODUCT-BASELINE.md)** — المرجع الموحد لهدف البرنامج، القرارات، الأولويات، المعمارية، رحلة تثبيت التطبيقات، الأمان، وحالة التنفيذ.
2. **[مصفوفة تغطية البحث والوظائف](docs/05-RESEARCH-COVERAGE-MATRIX.md)** — حالة تغطية كل محور، الأدلة الحالية، المرشحين الجاهزين، والفجوات التي يجب استكمالها.
3. **[خطة إعادة الاستخدام والتنفيذ](docs/12-REUSE-AND-IMPLEMENTATION-PLAN.md)** — Source SHAs، المعمارية الموحدة، قرارات Stack المفتوحة، PoC وخطة البناء.
4. [فهرس الوثائق](docs/README.md) — خريطة القراءة حسب المجال.
5. [منهجية البحث وضبط الأدلة](docs/RESEARCH-METHODOLOGY.md) — كيف نراجع المشاريع ونفرق بين الادعاء والكود والاختبار.

## خريطة الملفات

| الملف | الغرض |
|---|---|
| [00 — Product Baseline](docs/00-PRODUCT-BASELINE.md) | متطلبات المنتج وقراراته وأولوياته ومعماريته الموحدة |
| [01 — Connectivity](docs/01-connectivity-protocols.md) | الاتصال وRead Info وMode Switching وAdapters |
| [02 — Tool & Market Research](docs/02-tool-landscape-and-market-research.md) | سجل مراجعات GitHub والأدوات التجارية وتجارب المستخدمين ومراجعات parsers/installers ومصادر التنزيل وMCP |
| [03 — App Management & Network](docs/03-app-management-and-network.md) | إدارة التطبيقات، USB/Wireless ADB، DNS، تواريخ التثبيت والتحديث، ومحرك التثبيت واسع الصيغ |
| [04 — Windows Drivers](docs/04-windows-driver-diagnostics.md) | تشخيص تعريفات Windows والإصلاح والتراجع |
| [05 — Research Coverage Matrix](docs/05-RESEARCH-COVERAGE-MATRIX.md) | تغطية الوظائف، حالة الأدلة، المشاريع المرشحة، والفجوات البحثية |
| [06 — Android System Information](docs/06-android-system-information.md) | حقول الجهاز والنظام والبناء والـABI والذاكرة والتخزين، وتدقيق أدوات القراءة الجاهزة |
| [07 — Firmware & Software Inventory](docs/07-firmware-software-inventory.md) | فحص OTA metadata وpayload.bin وpartition manifests وAVB/vbmeta دون تفليش |
| [08 — Storage & Partition Information](docs/08-storage-partition-information.md) | معلومات filesystem usage وphysical/logical/dynamic partitions وsuper metadata وأدوات AOSP للقراءة فقط |
| [09 — Device File Management](docs/09-device-file-management.md) | ADB وMTP/WPD وvirtual drive، dual-pane، نقل الملفات، Unicode، الإلغاء والتحقق، ومراجعة المشاريع الجاهزة |
| [10 — Android Diagnostics & Logs](docs/10-android-diagnostics-and-logs.md) | Logcat والتسجيل وBugreport وdumpsys وDropbox وPerfetto وتحليل السجلات ومراجعة المشاريع الجاهزة |
| [11 — Backup & Restore Boundaries](docs/11-backup-restore-boundaries.md) | أنواع النسخ، حدود adb backup وAuto Backup، incremental snapshots، APK export، واستعادة الملفات دون ادعاء نسخة كاملة |
| [12 — Reuse & Implementation Plan](docs/12-REUSE-AND-IMPLEMENTATION-PLAN.md) | خطة إعادة الاستخدام، source revisions، المعمارية، قرارات Stack، PoC ومراحل البناء |
| [Research Methodology](docs/RESEARCH-METHODOLOGY.md) | الأدلة، الاختبارات، إعادة الاستخدام، أمان MCP وصيانة الوثائق |

## بيئة التشغيل المعتمدة

- Windows 10 وWindows 11.
- Android 7.0 (API 24) فأحدث؛ توفر الوظائف يختلف حسب الإصدار والموديل والصلاحيات.

## نطاق المنتج

- منصة متعددة المحاور وليست أداة تثبيت تطبيقات فقط.
- اكتشاف أجهزة Android وقراءة بياناتها ومعلومات النظام والبناء والعتاد والخصائص المتاحة بحسب وضع الاتصال والدليل.
- إدارة التطبيقات عبر USB وWireless ADB.
- البحث والتنزيل من **APKPure وF-Droid وAPKMirror**، مع اختلاف طريقة التكامل والتحقق لكل مصدر.
- قراءة معلومات السوفتوير والبناء والإصدارات والحزم، وفحص ملفات السوفتوير وبياناتها المتاحة؛ دون افتراض أن التفليش أو الكتابة إلى الأقسام ضمن النطاق.
- فهم أوضاع الاتصال والانتقالات المدعومة، وإدارة الملفات والسجلات وأدوات التشخيص التي يثبت تدقيقها فائدتها.
- تثبيت APK وSplit APKs وAPKS، ثم APKM وXAPK وOBB حسب الأولوية والقدرة على التحقق.
- DNS Ad Blocking، وفلاتر تاريخ التثبيت والتحديث ومصدر التثبيت.
- تشخيص تعريفات Android على Windows من مصادر Microsoft/Google/OEM الرسمية.
- تكامل MCP/Codex عبر طبقة خدمات موحدة وصلاحيات محكومة.

## خارج النطاق الحالي

- قراءة أوضاع Qualcomm وMediaTek وبروتوكولاتها.
- مقارنة تراخيص الأدوات.
- تجاوز حماية المواقع أو CAPTCHA/Cloudflare للحصول على ملفات التطبيقات.
- تنفيذ تغييرات على هاتف أو تعريفات Windows دون موافقة اختبار مستقلة.

## المبادئ الحاكمة

1. **Reuse Before Rebuild:** نبحث عن مشروع جاهز ونراجع مصدره قبل تنفيذ بديل.
2. **Multi-domain platform:** الاتصال وقراءة البيانات والأوضاع والسوفتوير وإدارة التطبيقات والتشخيص محاور أساسية متكاملة؛ التثبيت أولوية داخل محور التطبيقات فقط.
3. **One Service Layer:** الواجهة وMCP يستخدمان الخدمات والسياسات نفسها.
4. **Safe by default:** Read-only افتراضياً؛ Preview → Approval → Apply → Verify للتغييرات.
5. **Explicit device targeting:** لا تنفذ عملية على أول جهاز متصل تلقائياً.
6. **Evidence over claims:** الإعلان أو وجود الكود أو CI لا يثبت Runtime/Hardware.
7. **No hidden install:** تنزيل الملف لا يبدأ تثبيته تلقائياً.

## الحالة العامة

| المحور | التوثيق الحالي | الخطوة التالية |
|---|---|---|
| الاتصال وRead Info والأوضاع | بحث ومصفوفات ومراجعات أولية | اختبارات فعلية حسب الموديل والإصدار |
| معلومات النظام والجهاز | وثيقة بحث مستقلة 06 ومراجعة مرشحين | استكمال المصفوفة والاختبارات لاحقاً |
| معلومات build والـOTA والفريموير | وثيقتا بحث 06 و07 ومراجعة payload-dumper-go/AOSP | توسيع صيغ OTA legacy واختبار corpus |
| التخزين والأقسام | وثيقة بحث مستقلة 08 ومراجعة AOSP/liblp وأدوات Go/Cython | اختبار parser على corpus وقراءة الأجهزة لاحقاً |
| Logcat وBugreport والتشخيص | وثيقة بحث مستقلة 10 ومراجعة مصدر لعدة مشاريع | اختبارات ZIP وRuntime وHardware لاحقاً |
| إدارة الملفات والنقل | وثيقة بحث مستقلة 09 ومراجعة مصدر لعدة مشاريع | اختبار Runtime/Hardware وMTP/WPD |
| النسخ الاحتياطي والاستعادة | وثيقة بحث مستقلة 11 وحدود Android ومراجعة مشاريع incremental/legacy | اختبار Backup/Restore فعلياً بعد اعتماد النطاق |
| تثبيت التطبيقات | تصميم محرك موحد وتدقيق صيغ ومشاريع | Test Corpus وPoC على Windows وهاتف محدد |
| مصادر التطبيقات | F-Droid/APKPure/APKMirror موثقة مبدئياً | إثبات تدفق التنزيل لكل مصدر |
| إدارة التطبيقات وDNS | متطلبات وخطط اختبار | تدقيق نهائي واختبار فعلي |
| تعريفات Windows | تصميم تشخيص وإصلاح آمن | Hardware ID matrix واختبارات rollback |
| MCP/Codex | معمارية ومراجعات أولية | بناء الخادم واختباره بعد استقرار Service Layer |

**المرجع الحاكم للأولويات والقرارات:** [docs/00-PRODUCT-BASELINE.md](docs/00-PRODUCT-BASELINE.md).  
**المرجع الحاكم لحالة التغطية والفجوات:** [docs/05-RESEARCH-COVERAGE-MATRIX.md](docs/05-RESEARCH-COVERAGE-MATRIX.md).  
**المرجع الحاكم لإعادة الاستخدام وخطة PoC:** [docs/12-REUSE-AND-IMPLEMENTATION-PLAN.md](docs/12-REUSE-AND-IMPLEMENTATION-PLAN.md).
