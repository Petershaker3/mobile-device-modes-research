# Android Device Modes & Windows Toolkit Research

مرجع بحثي لتصميم برنامج Windows لإدارة أجهزة Android، قراءة معلوماتها، إدارة تطبيقاتها وتنزيلها وتثبيتها، تشخيص تعريفات Windows، وإتاحة الوظائف لوكلاء الذكاء الاصطناعي عبر MCP.

> **حالة المستودع:** بحث وتوثيق وتصميم متطلبات. لا يوجد حتى الآن تطبيق منتج مكتمل أو ادعاء باختبار كل الوظائف على أجهزة فعلية.

## ابدأ من هنا

1. **[خط الأساس ومتطلبات المنتج](docs/00-PRODUCT-BASELINE.md)** — المرجع الموحد لهدف البرنامج، القرارات، الأولويات، المعمارية، رحلة تثبيت التطبيقات، الأمان، وحالة التنفيذ.
2. [فهرس الوثائق](docs/README.md) — خريطة القراءة حسب المجال.
3. [منهجية البحث وضبط الأدلة](docs/RESEARCH-METHODOLOGY.md) — كيف نراجع المشاريع ونفرق بين الادعاء والكود والاختبار.

## خريطة الملفات

| الملف | الغرض |
|---|---|
| [00 — Product Baseline](docs/00-PRODUCT-BASELINE.md) | متطلبات المنتج وقراراته وأولوياته ومعماريته الموحدة |
| [01 — Connectivity](docs/01-connectivity-protocols.md) | الاتصال وRead Info وMode Switching وAdapters |
| [02 — Tool & Market Research](docs/02-tool-landscape-and-market-research.md) | سجل مراجعات GitHub والأدوات التجارية وتجارب المستخدمين ومراجعات parsers/installers ومصادر التنزيل وMCP |
| [03 — App Management & Network](docs/03-app-management-and-network.md) | إدارة التطبيقات، USB/Wireless ADB، DNS، تواريخ التثبيت والتحديث، ومحرك التثبيت واسع الصيغ |
| [04 — Windows Drivers](docs/04-windows-driver-diagnostics.md) | تشخيص تعريفات Windows والإصلاح والتراجع |
| [Research Methodology](docs/RESEARCH-METHODOLOGY.md) | الأدلة، الاختبارات، إعادة الاستخدام، أمان MCP وصيانة الوثائق |

## نطاق المنتج

- اكتشاف أجهزة Android وقراءة المعلومات بحسب وضع الاتصال والدليل المتاح.
- إدارة التطبيقات عبر USB وWireless ADB.
- البحث والتنزيل من **APKPure وF-Droid وAPKMirror**، مع اختلاف طريقة التكامل والتحقق لكل مصدر.
- تثبيت APK وSplit APKs وAPKS، ثم APKM وXAPK وOBB حسب الأولوية والقدرة على التحقق.
- فحص توافق الجهاز والتوقيع والملفات قبل التثبيت والتحقق بعده.
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
2. **Install-first:** تثبيت التطبيقات بأوسع تنوع عملي هو أولوية محور التطبيقات؛ تحليل ELF المتقدم مؤجل.
3. **One Service Layer:** الواجهة وMCP يستخدمان الخدمات والسياسات نفسها.
4. **Safe by default:** Read-only افتراضياً؛ Preview → Approval → Apply → Verify للتغييرات.
5. **Explicit device targeting:** لا تنفذ عملية على أول جهاز متصل تلقائياً.
6. **Evidence over claims:** الإعلان أو وجود الكود أو CI لا يثبت Runtime/Hardware.
7. **No hidden install:** تنزيل الملف لا يبدأ تثبيته تلقائياً.

## الحالة العامة

| المحور | التوثيق الحالي | الخطوة التالية |
|---|---|---|
| الاتصال وRead Info | بحث ومصفوفات ومراجعات أولية | اختبارات فعلية حسب الموديل |
| تثبيت التطبيقات | تصميم محرك موحد وتدقيق صيغ ومشاريع | Test Corpus وPoC على Windows وهاتف محدد |
| مصادر التطبيقات | F-Droid/APKPure/APKMirror موثقة مبدئياً | إثبات تدفق التنزيل لكل مصدر |
| إدارة التطبيقات وDNS | متطلبات وخطط اختبار | تدقيق نهائي واختبار فعلي |
| تعريفات Windows | تصميم تشخيص وإصلاح آمن | Hardware ID matrix واختبارات rollback |
| MCP/Codex | معمارية ومراجعات أولية | بناء الخادم واختباره بعد استقرار Service Layer |

**المرجع الحاكم للأولويات والقرارات:** [docs/00-PRODUCT-BASELINE.md](docs/00-PRODUCT-BASELINE.md).
