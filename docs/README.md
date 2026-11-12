# دليل الوثائق

هذه الصفحة هي فهرس التفاصيل الفنية. ابدأ من [README الرئيسي](../README.md) لمعرفة هدف المستودع ونطاقه وحالة البحث.

## الوثائق

1. [مرجع الاتصال وقراءة المعلومات والتحويل بين الأوضاع](01-connectivity-protocols.md) — طبقات USB وADB وFastboot وMTP، أوضاع القراءة، انتقالات الأوضاع، وبنية المحولات.
2. [دراسة الأدوات الجاهزة والسوق وتجارب المستخدمين](02-tool-landscape-and-market-research.md) — أدوات GitHub والأدوات التجارية، مراجعة الأدلة، تجارب المجتمع، ومقارنة المرشحين، بما في ذلك تحليل حاويات APK/APKS/APKM/XAPK وأمن ZIP وتوقيع APK.
3. [إدارة التطبيقات والشبكة عبر ADB](03-app-management-and-network.md) — إدارة التطبيقات، APK/APKS، ABI، DNS، تواريخ التثبيت والتحديث، ومصدر التثبيت.
4. [تشخيص وإصلاح تعريفات Android على Windows](04-windows-driver-diagnostics.md) — تشخيص التعريفات، المصادر الرسمية، الإصلاح الآمن، التراجع، واختبارات القبول.
5. [منهجية البحث وضبط جودة الأدلة](RESEARCH-METHODOLOGY.md) — درجات الثقة، حالات التحقق، قواعد التوثيق، وإعادة الاستخدام الآمن للمشاريع وتكامل MCP.

## محاور هندسية مضافة

- **Reuse Before Rebuild:** موثق في القسم 19 من [دراسة الأدوات الجاهزة](02-tool-landscape-and-market-research.md#19-مبدأ-إعادة-استخدام-مشاريع-github-قبل-بناء-بديل-reuse-before-rebuild).
- **AI/MCP Integration:** موثق في القسم 20 من [دراسة الأدوات الجاهزة](02-tool-landscape-and-market-research.md#20-إتاحة-وظائف-البرنامج-لوكلاء-الذكاء-الاصطناعي-عبر-mcp).
- **APK/APKS/APKM/XAPK containers:** موثق في الأقسام 20.30–20.32 من [دراسة الأدوات الجاهزة](02-tool-landscape-and-market-research.md#2030-تدقيق-google-bundletool-اختيار-split-apk-المتوافق-وتثبيت-apks-12-نوفمبر-2026).
- **ZIP/APK security:** موثق في القسم 20.33 من [دراسة الأدوات الجاهزة](02-tool-landscape-and-market-research.md#2033-تدقيق-أمان-zipapk-zip-slip-وzip-bomb-والروابط-الرمزية-والتحقق-من-التوقيع-12-نوفمبر-2026).
- **Android Manifest/APK parsers:** موثق في القسم 20.34 من [دراسة الأدوات الجاهزة](02-tool-landscape-and-market-research.md#2034-محللات-android-manifest-وapk-الجاهزة-مقارنة-axml-parser-وapk-parser-وandroid-sdk-tools-12-نوفمبر-2026).
- **Android ABI/ELF inspection:** موثق في القسم 20.35 من [دراسة الأدوات الجاهزة](02-tool-landscape-and-market-research.md#2035-فحص-abi-وelf-وnative-libraries-داخل-apk-12-نوفمبر-2026).
- **Native library dependencies (DT_NEEDED/SONAME):** القسم 20.36 محفوظ كتشخيص متقدم مؤجل، وليس شرطاً لمسار تثبيت V1.
- **APK installation priority and unified engine:** القرار ومصفوفة الصيغ وتدقيق المشاريع موثقة في الأقسام 20.37–20.41 من [دراسة الأدوات الجاهزة](02-tool-landscape-and-market-research.md#2037-إعادة-ضبط-الأولوية-التثبيت-واسع-التغطية-هو-هدف-v1-12-نوفمبر-2026) و[إدارة التطبيقات والشبكة](03-app-management-and-network.md#18-محور-التثبيت-واسع-الصيغ-قرار-الأولوية-ومحرك-apk-الموحد).

## مراجع تكامل MCP

- [Codex MCP setup](https://developers.openai.com/codex/mcp) — إعداد خوادم STDIO وStreamable HTTP.
- [MCP specification](https://modelcontextprotocol.io/specification/latest) — المعيار الأساسي ومكونات الأدوات والموارد والمطالبات.
- [MCP TypeScript SDK](https://github.com/modelcontextprotocol/typescript-sdk) — SDK رسمي مرشح لخادم TypeScript.

## قواعد القراءة

- المتطلبات المقترحة ليست دليلاً على أن أداة جاهزة تدعمها.
- وجود وظيفة في الكود أو صفحة تسويقية لا يثبت عملها على كل الأجهزة.
- لا يُسجل الاختبار على أنه ناجح إلا مع ذكر الجهاز وإصدار Android وWindows والأداة وإصدارها وخطوات الاختبار والنتيجة.
- راجع حالة كل محور في نهايته قبل اتخاذ قرار تنفيذ.
