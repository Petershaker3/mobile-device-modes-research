# Android Device Modes & Windows Toolkit Research

مرجع بحثي لتصميم منصة Windows متكاملة للتعامل مع أجهزة Android: الاتصال وقراءة البيانات وفهم الأوضاع ومعلومات السوفتوير وإدارة التطبيقات والملفات والتشخيص والشبكة وتعريفات Windows، مع إتاحة الوظائف لوكلاء الذكاء الاصطناعي عبر MCP.

> **حالة المستودع:** بحث وتصميم + قرارات محسومة + عقود قابلة للفحص + **كود فعلي مبنيّ ومُختبر** في `src/`: 30 ملف C# منقولاً حرفياً (5,566 سطراً) + 17 ملف اختبار منقول (6,803 سطراً).
>
> **مُثبت بالتنفيذ لا بالادعاء** — [GitHub Actions run 43346149307](https://github.com/Petershaker3/mobile-device-modes-research/actions/runs/43346149307):
>
> | ما أُثبت | النتيجة |
> |---|---|
> | الملفات المنقولة مطابقة لأصلها بايت-ببايت | 48/48 |
> | المكتبة تبني على `net10.0` | ✅ ويندوز + لينكس |
> | الاختبارات تمر | **437/437** ويندوز · 433/437 لينكس (4 فروق POSIX موثّقة) |
> | جدول الترجمة مطابق لأصلهم | 643 مفتاحاً |
> | مفاتيح الترجمة المستخدمة كلها موجودة | 43/43 |
>
> **ما لم يُثبت — وما زال بصراحة على حاله:** لا شيء من هذا يمسّ جهازاً حقيقياً. لا يوجد جهاز، ولا تثبيت APK، ولا اتصال ADB. تلك المسافة يقطعها أول اختبار على جهازك.
>
> **نهج التنفيذ (قرار المستخدم):** نقل الموجود فعلاً على GitHub لا كتابته. التفاصيل في [ADR-0001 §3.1](docs/17-adr-0001-stack-decision.md) وسجل النقل في [ATTRIBUTION.md](ATTRIBUTION.md).
>
> **الستاك محسوم (13 نوفمبر 2026):** .NET 10 + WPF — راجع [ADR-0001](docs/17-adr-0001-stack-decision.md).
>
> **تحقق بنفسك** — بوابتان تعملان بلا .NET SDK، والثالثة تحتاجه:
> ```bash
> scripts/verify-transplant.sh --no-build   # 48 ملفاً متطابقاً + قابلية النقل
> python scripts/check-loc-keys.py           # 43/43 مفتاح ترجمة موجود
> scripts/verify-transplant.sh               # + build + test (يحتاج .NET SDK 10)
> ```
> أو اترك CI يفعلها عنك: كل دفع يشغّل الأربع وظائف تلقائياً.
>
> **جرّب العقود الآن** (لا تحتاج .NET ولا جهازاً):
> ```bash
> pip install 'jsonschema>=4.18'
> python contracts/tools/validate_contracts.py
> # contracts: 45 checks over 7 schemas, 11 examples, 10 negative cases
> # OK: all contracts hold
> ```

## ابدأ من هنا

1. **[خط الأساس ومتطلبات المنتج](docs/00-PRODUCT-BASELINE.md)** — المرجع الموحد لهدف البرنامج، القرارات، الأولويات، المعمارية، رحلة تثبيت التطبيقات، الأمان، وحالة التنفيذ.
2. **[مصفوفة تغطية البحث والوظائف](docs/05-RESEARCH-COVERAGE-MATRIX.md)** — حالة تغطية كل محور، الأدلة الحالية، المرشحين الجاهزين، والفجوات التي يجب استكمالها.
3. **[خطة إعادة الاستخدام والتنفيذ](docs/12-REUSE-AND-IMPLEMENTATION-PLAN.md)** — Source SHAs، المعمارية الموحدة، قرارات Stack المفتوحة، PoC وخطة البناء.
4. **[إعادة التحقق من مصدر ADB والملفات وLogcat](docs/13-source-revalidation-adb-file-core.md)** — مراجعة المصدر المحددة للمرشحين الأساسيين وفجواتهم.
5. **[إعادة التحقق من مثبتات APK والحزم](docs/14-source-revalidation-app-installers.md)** — مراجعة parser/install candidates لكل الصيغ.
6. **[إعادة التحقق من MCP وCodex](docs/15-source-revalidation-mcp-codex.md)** — مراجعة خوادم MCP وتثبيت سياسة آمنة للربط المحلي.
7. **[إعادة التحقق من OTA والتخزين والنسخ الاحتياطي](docs/16-source-revalidation-firmware-storage-backup.md)** — مراجعة parser/backup candidates وحدودها.
8. **[ADR-0001 — قرار الستاك](docs/17-adr-0001-stack-decision.md)** — .NET 10 + WPF محسوم، وأثره على ترتيب المرشحين وحاجز الترخيص.
9. **[القرارات الهندسية التنفيذية](docs/18-ENGINEERING-DECISIONS.md)** — المهلات، الحدود، إصدارات الأدوات، الاحتفاظ بالسجلات، نطاق الكتابة.
10. **[العقود المشتركة](docs/19-SHARED-CONTRACTS.md)** — DeviceHandle/Plan/Operation/Result/Audit كمواصفة منفذة مع مدقّق يعمل.
11. **[الجولة الثانية من مراجعة المصدر](docs/20-source-revalidation-round2.md)** — 6 مشاريع و4 تصحيحات لوثائق قائمة.
12. **[Threat Model](contracts/THREAT-MODEL.md)** — 13 مسار هجوم، وضابطا حماية منفَّذان فقط.
13. **[Test Corpus](contracts/corpus/README.md)** — مواصفة الـfixtures لكل فئة.
14. **[النقل الأول — طبقة Logcat](docs/21-TRANSPLANT-LOGCAT.md)** — 8,530 سطراً منقولة حرفياً وكيف بُنيت بلا لمس الكود.
15. **[ATTRIBUTION](ATTRIBUTION.md)** — سجل كل ملف منقول ومصدره.
14. [فهرس الوثائق](docs/README.md) — خريطة القراءة حسب المجال.
15. [منهجية البحث وضبط الأدلة](docs/RESEARCH-METHODOLOGY.md) — كيف نراجع المشاريع ونفرق بين الادعاء والكود والاختبار.

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
| [13 — ADB Source Revalidation](docs/13-source-revalidation-adb-file-core.md) | مراجعة كود AdvancedSharpAdbClient وADB-Explorer وAdbGui عند SHAs مثبتة، وتوثيق فجوات التثبيت والنقل وLogcat |
| [14 — App Installer Source Revalidation](docs/14-source-revalidation-app-installers.md) | مراجعة bundletool وapkfile وPacMan-APK وxapkInstaller، وفصل صيغ APKS/APKM/XAPK وسياسة التثبيت وأمان الأرشيف |
| [15 — MCP & Codex Source Revalidation](docs/15-source-revalidation-mcp-codex.md) | مراجعة adb-mcp وus-all/android-mcp-server، وتصميم MCP محلي read-only مع explicit targeting وapproval protocol |
| [16 — Firmware, Storage & Backup Source Revalidation](docs/16-source-revalidation-firmware-storage-backup.md) | مراجعة payload-dumper-go وlucky-arch وlpunpack_cyth وchen3feng/android-backup، وتحديد فجوات metadata/sparse/backup |
| [17 — ADR-0001 Stack](docs/17-adr-0001-stack-decision.md) | قرار .NET 10 + WPF، البدائل المرفوضة، وأثر الترخيص |
| [18 — Engineering Decisions](docs/18-ENGINEERING-DECISIONS.md) | المهلات والحدود والإصدارات والاحتفاظ ونطاق الكتابة |
| [19 — Shared Contracts](docs/19-SHARED-CONTRACTS.md) | العقود كمواصفة منفذة + المدقّق + الحالات السلبية |
| [20 — Source Revalidation R2](docs/20-source-revalidation-round2.md) | الجولة الثانية للمرشحين والتصحيحات |
| [contracts/](contracts/) | JSON Schemas، أمثلة، المدقّق، Threat Model، corpus |
| [src/](src/) | الكود: `DeviceToolkit.Core` (منقول) + `DeviceToolkit.Tests` |
| [ATTRIBUTION.md](ATTRIBUTION.md) | سجل النقل: كل ملف ومصدره وSHA وهل عُدّل |
| [scripts/verify-transplant.sh](scripts/verify-transplant.sh) | يثبت أن المنقول مطابق ويبني ويختبر |
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
| الاتصال وRead Info والأوضاع | بحث ومصفوفات؛ قرار الستاك محسوم؛ عقود قابلة للفحص | اختبارات فعلية حسب الموديل والإصدار (يحتاج جهازاً) |
| معلومات النظام والجهاز | وثيقة بحث مستقلة 06 ومراجعة مرشحين | استكمال المصفوفة والاختبارات لاحقاً |
| معلومات build والـOTA والفريموير | وثيقتا بحث 06 و07 ومراجعة payload-dumper-go/AOSP | توسيع صيغ OTA legacy واختبار corpus |
| التخزين والأقسام | وثيقة بحث مستقلة 08 ومراجعة AOSP/liblp وأدوات Go/Cython | اختبار parser على corpus وقراءة الأجهزة لاحقاً |
| Logcat والتشخيص | **مبنيّ ومُختبر في CI**: 437/437 اختباراً على ويندوز (parser + mmap index + فلترة SIMD + crash extract + sidecar cache + file proxy). لا اختبار على جهاز | Bugreport ZIP وRuntime وHardware؛ نقل `LogcatStream` |
| إدارة الملفات والنقل | وثيقة بحث مستقلة 09 ومراجعة مصدر لعدة مشاريع | اختبار Runtime/Hardware وMTP/WPD |
| النسخ الاحتياطي والاستعادة | وثيقة بحث مستقلة 11 وحدود Android ومراجعة مشاريع incremental/legacy | اختبار Backup/Restore فعلياً بعد اعتماد النطاق |
| تثبيت التطبيقات | تصميم محرك موحد وتدقيق صيغ؛ `Plan`+`Approval`+`Verify` مفروضة في العقود | Test Corpus وPoC على Windows وهاتف محدد |
| مصادر التطبيقات | F-Droid/APKPure/APKMirror موثقة مبدئياً | إثبات تدفق التنزيل لكل مصدر |
| إدارة التطبيقات وDNS | متطلبات وخطط اختبار | تدقيق نهائي واختبار فعلي |
| تعريفات Windows | تصميم تشخيص وإصلاح آمن | Hardware ID matrix واختبارات rollback |
| MCP/Codex | معمارية ومراجعات أولية؛ قرار Read-only V1 | بناء الخادم واختباره بعد استقرار Service Layer |

**المرجع الحاكم للأولويات والقرارات:** [docs/00-PRODUCT-BASELINE.md](docs/00-PRODUCT-BASELINE.md).  
**المرجع الحاكم لحالة التغطية والفجوات:** [docs/05-RESEARCH-COVERAGE-MATRIX.md](docs/05-RESEARCH-COVERAGE-MATRIX.md).  
**المرجع الحاكم لإعادة الاستخدام وخطة PoC:** [docs/12-REUSE-AND-IMPLEMENTATION-PLAN.md](docs/12-REUSE-AND-IMPLEMENTATION-PLAN.md).  
**المرجع الحاكم لقرار الستاك:** [docs/17-adr-0001-stack-decision.md](docs/17-adr-0001-stack-decision.md).  
**المرجع الحاكم للعقود والسياسات القابلة للفحص:** [docs/19-SHARED-CONTRACTS.md](docs/19-SHARED-CONTRACTS.md) و[contracts/](contracts/).
