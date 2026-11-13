# ADR-0001 — قرار Stack التنفيذي: .NET 10 + WPF

> **الحالة:** Accepted (تم اعتماده)
> **التاريخ:** 13 نوفمبر 2026
> **يُغلق:** القرارات المفتوحة في [00 §9.2](00-PRODUCT-BASELINE.md) البنود المتعلقة بـ Host/UI/ADB، وبند `Host Core` و`GUI` و`ADB` في جدول قرارات [12 §6](12-REUSE-AND-IMPLEMENTATION-PLAN.md).
> **لا يُغلق:** تغطية Android، مهلات العمليات، سياسة الإصدارات، نشر الاعتماديات، نطاق الكتابة — تبقى في [18](18-ENGINEERING-DECISIONS.md).

## 1. السياق

كان القرار مؤجلاً في الوثائق انتظاراً لـPoC مقارنة بين .NET 10 وTauri 2/Rust. هذا ADR يثبّت القرار على أساس **دليل مصدر موجود فعلياً** اعتباراً من 13 نوفمبر 2026، ويحوّل شرط "إجراء PoC قبل الاختيار" إلى شرط "إثبات القرار داخل PoC التنفيذ" — أي أن PoC لم يعد يقارن لغتين، بل يقيس تنفيذ النواة نفسها.

**الحجج الحاسمة (مرتبة بالأثر):**

1. **حاجة Windows أصلية لا تُخفَّض بديلاً.** المنتج يحتاج SetupAPI/Configuration Manager/PnP، وWPD/COM، وWinUSB، وDevice Manager problem codes، و`pnputil`. في .NET هذه استدعاءات مباشرة عبر CsWin32/P/Invoke داخل نفس الـprocess. في Tauri/Rust تكتب نفس الـFFI يدوياً ثم تعيد تغليفه عبر IPC إلى واجهة Web. تكلفة إضافية بلا مقابل وظيفي.
2. **ADB عبر TCP loopback إلى adb server (port 5037) وليس فقط `adb.exe`.** مكتبة C# جاهزة (AdvancedSharpAdbClient) تُدير الاتصال والـsync service والـinstall sessions. الوصول المباشر للـsocket يعطينا تحكماً حقيقياً في المهلة والإلغاء على مستوى القناة، وهو ما نحتاجه لسياسة `OutcomeUnknown`.
3. **طبقة ADB/الملفات/الـlogcat الموجودة فعلاً بنفس الستاك.** `daitouniao/logcat-viewer` (Commit `a9856162…`) مبني على **net10.0-windows** ويستخدم **AdvancedSharpAdbClient 3.6.16**. هذا يعني أن أنماط التنفيذ (device monitor، self-heal للـserver، مهلة على أوامر الـroot، memory-mapped log index، cancellation) قابلة للاستخراج والقياس على نفس runtime — وليست ترجمة من Kotlin/Rust.
4. **قابلية الاختبار بلا جهاز.** قرار المستخدم: لا يوجد جهاز اختبار الآن. .NET يسمح بفصل الـADB client عن I/O عبر seams (injecها المصدر المرجعي نفسه: `Func<bool> statusProbe`, `Func<IAdbSocket> socketFactory`)، فتُختبر كل طبقة الخدمات بـ848+ نمط اختبار معروف بلا USB.

**البدائل المرفوضة وأسبابها:**

| البديل | سبب الرفض |
|---|---|
| Tauri 2 + Rust | يحتاج FFI لكل Windows API + IPC للـUI؛ لا يضيف قيمة وظيفية في نطاقنا. مسار WPD والـdrivers يصبح أغلى. |
| Node.js/Electron | Native modules لهشاشة إضافية، لا وصول مباشر لـWPD/PnP، حجم runtime أكبر، وطبقة ADB أضعف. |
| WinUI 3 | لا قيمة إضافية تبرر التكلفة مقابل WPF لنمط "أداة فني كثيفة الجداول والنوافذ المتعددة". |

## 2. القرار

| البند | القرار |
|---|---|
| Runtime | **.NET 10** (LTS-style، `net10.0-windows`) |
| GUI | **WPF** على `net10.0-windows`، MVVM، بلا dependency على Windows App SDK |
| ADB | **AdvancedSharpAdbClient 3.6.16+** عبر adapter داخلي، مع `adb.exe` كمسار احتياطي/capability إضافي |
| Windows Native | **CsWin32** (`Microsoft.Windows.CsWin32`) لتوليد P/Invoke، بلا `DllImport` يدوي في كود المنتج |
| Persistence | **SQLite** (عبر `Microsoft.Data.Sqlite`) — مع قرار schema في [18](18-ENGINEERING-DECISIONS.md) |
| MCP | **.NET MCP SDK** يعمل داخل نفس الـprocess كـخدمة، STDIO transport، فوق Application Services |
| الاختبارات | **xUnit** لكل الخدمات؛ fixtures على القرص؛ لا اختبار يتطلب جهازاً في الـCI الافتراضي |
| Target RIDs | `win-x64` أولاً؛ `win-arm64` قرار مؤجل [18](18-ENGINEERING-DECISIONS.md) |

## 3. نتيجة القرار على المرشحين: إعادة ترتيب Source Revalidation

القرار يغيّر أولوية المرشحين، لأن معيار "قابلية الاقتباس" أصبح "قابلية الاقتباس **داخل .NET 10**". التفاصيل الكاملة في [20 §1](20-source-revalidation-round2.md).

| المرشح | الترتيب السابق | الترتيب بعد ADR-0001 | السبب |
|---|---|---|---|
| AdvancedSharpAdbClient | مرشح Host Core | **مرشح معتمد (Core)** | .NET، نفس runtime، مستخدم في مشروعين مُراجَعين |
| daitouniao/logcat-viewer | "مرجع أداء" | **مرشح أول للاقتباس** | .NET 10 + WinForms + AdvancedSharpAdbClient 3.6.16 + 852 اختبار + Apache-2.0 |
| Paving-Base/APK-Installer | "مرجع UI/ADB جزئي" | **مرشح ثانٍ** | .NET 10 + AAPT metadata model نافع؛ لكن WinUI ومسار WSA يُستبعدان |
| ADB-Explorer | مرشح ملفات | يبقى **patterns only** | WPF/.NET لكنه fork قديم؛ أنماط SyncPipe فقط |
| AdbFileManager | مرجع Unicode | **patterns only** | Kotlin/Compose — لا اقتباس كود |
| WinDroidFS | Virtual Drive اختياري | **مرجع تصميم فقط + GPL حاجز** | Rust + **GPL-3.0** → لا اقتباس كود في منتجنا |
| AdbGui | مرشح Logcat | **patterns only** | Kotlin/Compose |

## 4. أثر الترخيص (قيد تنفيذي جديد)

هذا أول ADR يسجل الترخيص كقيد، لأن حاجز الترخيص يمكن أن يُبطل قراراً تقنياً بعد كتابة الكود:

| المشروع | الترخيص | الأثر على منتجنا |
|---|---|---|
| AdvancedSharpAdbClient | Apache-2.0 | اقتباس/اعتماد مباشر مسموح مع إشعارات |
| daitouniao/logcat-viewer | Apache-2.0 | اقتباس كود مسموح مع NOTICE؛ يجب إدراج `THIRD-PARTY-NOTICES` |
| APK-Installer | MIT (تحقق في [20 §2](20-source-revalidation-round2.md)) | اقتباس مسموح |
| ADB-Explorer | MIT | أنماط فقط |
| WinDroidFS | **GPL-3.0** | **لا اقتباس كود.** يُستخدم كمرجع تصميم موثق فقط |
| LogCater | راجع [20](20-source-revalidation-round2.md) | لا اقتباس قبل تثبيت الترخيص |

## 5. ما لا يعنيه هذا القرار

- **ليس دليلاً على نجاح runtime.** لم يُبنَ أي مشروع، ولم تُشغَّل اختبارات، ولم يُوصَل جهاز. حالة الدليل تبقى "Source revalidated / Runtime not verified".
- **لا يفتح الكتابة على الجهاز.** سياسة Read-only الافتراضية كما هي.
- **لا يختار نطاق الكتابة للسوفتوير** ولا يفتح تفليشاً — يبقى قراراً مستقلاً في [00 §9.2](00-PRODUCT-BASELINE.md).
- **لا يحسم تغطية Android** ولا سياسة نشر الاعتماديات؛ ينتقلان إلى [18](18-ENGINEERING-DECISIONS.md).

## 6. أثر على الوثائق الأخرى

| الملف | التحديث المطلوب |
|---|---|
| [00](00-PRODUCT-BASELINE.md) §9.2 | تحويل سطر "معمارية Windows" و"أدوات التشغيل جزئياً" إلى محسوم مع إحالة لهذا ADR |
| [12](12-REUSE-AND-IMPLEMENTATION-PLAN.md) §1 و§6 | إغلاق صفوف Host Core وGUI وADB من "قرار مفتوح" إلى "محسوم بـADR-0001" |
| [05](05-RESEARCH-COVERAGE-MATRIX.md) | إضافة صفوف العقود والـThreat Model والـcorpus كحالة "مواصفة منفذة" بعد [19](19-SHARED-CONTRACTS.md) |
| [20](20-source-revalidation-round2.md) | الجولة الثانية الكاملة للمرشحين الستة |

**آخر تحديث:** 13 نوفمبر 2026.
