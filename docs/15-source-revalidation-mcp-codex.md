# Source Revalidation — Android MCP Servers & Codex Integration

> **الغرض:** إعادة فحص مرشحي MCP اللذين سبق إدراجهما في البحث، وتحديد ما يمكن إعادة استخدامه في ربط Codex بالبرنامج دون كشف generic shell أو السماح للـAI بتجاوز سياسات الموافقة.

**تاريخ المراجعة:** 12 نوفمبر 2026  
**المستودع المستهدف:** mobile-device-modes-research  
**الحالة:** Source Revalidation — لا MCP server تم بناؤه أو ربطه بـCodex.

## 1. الخلاصة التنفيذية

| المرشح | نقاط القوة | ما يمنع الاعتماد المباشر | قرارنا |
|---|---|---|---|
| [iksnerd/adb-mcp](https://github.com/iksnerd/adb-mcp/tree/6b12c977ee576091fe34553c0dcf9cfa2258d377) | Go + official MCP Go SDK، typed ADB client، exec.CommandContext، device resolution، shell quoting tests، tool annotations، graceful shutdown | serial اختياري مع single-device auto-resolution، safety annotations/instructions ليست approval enforcement، مساحة tools واسعة وتتضمن تغييرات كثيرة، لا PR CI في workflow الذي تمت مراجعته | أفضل مرجع لبنية Go MCP/ADB وtool contracts، لا ننسخه كمنتج |
| [us-all/android-mcp-server](https://github.com/us-all/android-mcp-server/tree/bacd6eac34273e7bbaef666954ee399bdf1de1e3) | TypeScript MCP SDK، stdio، 76 tool، categories، resources/prompts، execFile، timeouts، separate write/shell gates، input validators | write gate global environment flag وليس approval لكل عملية؛ serial اختياري/auto؛ generic shell متاح عند تفعيل flag؛ بعض أوامر shell تتطلب مراجعة كل input | مرجع لتقسيم الأدوات والـschemas والـdiscovery، لا نفعّل write/shell flags في المنتج |

**القرار:** MCP يجب أن يكون واجهة فوق Application Services نفسها، وليس ADB client موازياً. نبدأ Read-only tools فقط، ونفرض DeviceHandle صريحاً في كل tool يستهدف جهازاً. لا نعتمد MCP annotations أو system prompt كبديل لبوابة موافقة داخلية.

## 2. iksnerd/adb-mcp — مراجعة Go MCP

المستودع:
https://github.com/iksnerd/adb-mcp/tree/6b12c977ee576091fe34553c0dcf9cfa2258d377

- Commit: 6b12c977ee576091fe34553c0dcf9cfa2258d377
- يستخدم official Go MCP SDK v1.6.1.
- [main.go](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/cmd/adb-mcp/main.go)
- [ADB runner](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/internal/adb/adb.go)
- [Device resolution](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/internal/adb/devices.go)
- [Tool registration](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/internal/tools/register.go)
- [Safety annotations](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/internal/tools/annotations.go)
- [Annotation tests](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/internal/tools/annotations_test.go)
- [Shell quoting tests](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/internal/adb/shellquote_test.go)
- [Session defaults](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/internal/tools/session.go)
- [CI/release workflow](https://github.com/iksnerd/adb-mcp/blob/6b12c977ee576091fe34553c0dcf9cfa2258d377/.github/workflows/release.yml)

### 2.1 ما ثبت في الكود

- يستخدم stdio transport، مناسباً من حيث الشكل لتشغيل MCP server محلياً من Codex.
- كل tool handler رفيع؛ يresolve target ثم ينادي Client method ويعيد نتيجة منظمة.
- ADB runner يستخدم exec.CommandContext مع context؛ لا يبني command line واحدة عبر shell على Windows.
- Client مربوط بـserial محدد بعد ResolveSerial.
- ResolveSerial:
  - يستخدم serial الممرر إذا كان موجوداً.
  - إذا لم يُمرر serial، يختار الجهاز الوحيد في state=device.
  - إذا تعددت الأجهزة الجاهزة، يرجع خطأ يطلب serial.
- توجد shell quoting tests لمدخلات تحتوي spaces وquotes و$ و; و&.
- Safety annotations تميز observe/change/destroy/network وتختبر أن كل tool له Title وannotations، وأن مجموعة الأدوات destructive مثبتة بالاختبار.
- عند إغلاق stdio/session، main ينظف عمليات Gradle وبعض capture sessions ويحاول استعادة بعض حالة الجهاز المعروفة سابقاً ضمن timeout.
- هناك مجموعة كبيرة من unit tests للموديلات والـADB tools والـannotations والـcancellation.

### 2.2 القيود بالنسبة لمنتجنا

1. serial اختياري؛ single-device auto-resolution مفيد للأدوات العامة لكنه يخالف عقدنا: كل device-targeted MCP tool يجب أن يطلب device_id/serial صريحاً.
2. MCP ToolAnnotations هي safety hints للعميل؛ لا تنفذ بنفسها موافقة المستخدم ولا تمنع الاستدعاء. لا نعتبر DestructiveHint بوابة حماية.
3. Instructions المرسلة أثناء initialize هي إرشادات للنموذج وليست enforcement؛ يمكن للنموذج أو MCP client ألا يطبقها كما نتوقع.
4. مجموعة الأدوات تشمل tap/swipe/input وdevice lock وuninstall وpush/pull وGradle/build/test. هذا أوسع من Read-only MCP V1.
5. بعض عمليات pull/push تكتب إلى وجهة محلية/بعيدة وقد تستبدل ملفاً موجوداً؛ annotations توضح ذلك، لكن منتجنا يحتاج preview وconflict policy داخل Operation Engine.
6. لا توجد PR CI workflow في شجرة المصدر التي تمت مراجعتها؛ workflow الظاهر يعمل عند tag release، ويشغل go test -race ./... أثناء مسار الإصدار. لا نساوي ذلك باختبارات PR مستمرة.
7. يضم المشروع Android accessibility bridge لتوسيع UI automation؛ لا نحتاج bridge أو صلاحياته في MCP الأساسي.

**الحكم:** أفضل مرجع لبنية Go MCP وtyped ADB client وcontext cancellation وshell quoting tests. لا نعتمد annotations أو instructions كـauthorization، ولا ننسخ كل الأدوات.

## 3. us-all/android-mcp-server — مراجعة TypeScript MCP

المستودع:
https://github.com/us-all/android-mcp-server/tree/bacd6eac34273e7bbaef666954ee399bdf1de1e3

- Commit: bacd6eac34273e7bbaef666954ee399bdf1de1e3
- [README](https://github.com/us-all/android-mcp-server/blob/bacd6eac34273e7bbaef666954ee399bdf1de1e3/README.md)
- [Server registration](https://github.com/us-all/android-mcp-server/blob/bacd6eac34273e7bbaef666954ee399bdf1de1e3/src/index.ts)
- [ADB process adapter](https://github.com/us-all/android-mcp-server/blob/bacd6eac34273e7bbaef666954ee399bdf1de1e3/src/adb.ts)
- [Tool utilities/validation](https://github.com/us-all/android-mcp-server/blob/bacd6eac34273e7bbaef666954ee399bdf1de1e3/src/tools/utils.ts)
- [App tools](https://github.com/us-all/android-mcp-server/blob/bacd6eac34273e7bbaef666954ee399bdf1de1e3/src/tools/apps.ts)
- [File tools](https://github.com/us-all/android-mcp-server/blob/bacd6eac34273e7bbaef666954ee399bdf1de1e3/src/tools/files.ts)
- [Shell tool](https://github.com/us-all/android-mcp-server/blob/bacd6eac34273e7bbaef666954ee399bdf1de1e3/src/tools/shell.ts)

### 3.1 ما ثبت في الكود

- يستخدم MCP TypeScript SDK وstdio افتراضياً، مع Streamable HTTP option منفصل.
- يقسم الأدوات إلى categories: device/apps/ui/logcat/emulator/files/system/debug/shell.
- يحتوي resources وprompts وmeta search-tools tool.
- يستخدم execFile مع argument array بدلاً من shell host string.
- يوجد timeout افتراضي للأوامر.
- لديه validators لـpackage names وpermissions وcomponent names وdevice paths، وshellEscape لمعالجة metacharacters.
- يفصل ANDROID_MCP_ALLOW_WRITE عن ANDROID_MCP_ALLOW_SHELL؛ كلاهما false افتراضياً.
- يسمح بتحديد serial عبر ANDROID_SERIAL أو serial parameter، وإلا يستخدم auto/single-device behavior.
- write tools تستدعي assertWriteAllowed، وgeneric execute-shell يستدعي assertShellAllowed.

### 3.2 القيود بالنسبة لمنتجنا

1. ANDROID_MCP_ALLOW_WRITE=true يفتح مجموعة write tools كلها كإعداد عام للعملية؛ لا يطلب approval منفصلاً لكل install/uninstall/push/delete أو target/path.
2. serial اختياري في schemas كثيرة؛ يجب ألا ننقل auto target إلى منتجنا.
3. generic shell موجود ويصبح متاحاً عند ANDROID_MCP_ALLOW_SHELL=true؛ لا نفعّله ولا نعرضه.
4. بعض عمليات الكتابة تستخدم shell commands مبنية من parameters؛ يجب مراجعة validation لكل tool على حدة، ولا يكفي وجود shellEscape helper في المشروع.
5. install-app يقبل APK واحداً؛ لا يوفر pipeline متكاملاً لصيغ APKM/XAPK/APKS أو split selection/signature verification.
6. file delete/push لا يقدمون وحدهم preview/restore/reconciliation model المطلوب.
7. HTTP transport وMCP_HTTP_SKIP_AUTH خيارات لا نحتاجها في V1؛ نبدأ local stdio فقط.

**الحكم:** مرجع جيد لتصميم categories وtool discovery وtyped schemas وprocess wrapper وvalidation utilities. لا نعتمد global write flag كبديل عن approval engine.

## 4. عقد MCP الذي نحتاجه

### 4.1 Read-only V1

أدوات مقترحة:
- devices_list — قائمة الأجهزة والحالة فقط.
- device_get_info(device_id) — معلومات الجهاز.
- device_get_capabilities(device_id) — القدرات مع evidence/source.
- device_get_modes(device_id) — الوضع الحالي والانتقالات المدعومة.
- device_get_software_inventory(device_id) — build/system/software metadata.
- apps_list(device_id, filters) — جرد التطبيقات.
- app_get_details(device_id, package_id) — تفاصيل تطبيق.
- files_list(device_id, path) — list/stat فقط.
- diagnostics_logcat_start(device_id, filters) — بدء قراءة logcat مع bounded stream.
- diagnostics_logcat_stop(session_id) — إيقاف session المحلية.
- diagnostics_bugreport_import(local_path) — تحليل تقرير محلي دون رفعه.
- package_inspect(local_path) — تحليل APK/Bundle محلياً دون تثبيت.
- operation_get_status(operation_id) — قراءة حالة عملية.
- evidence_get(artifact_id) — استرجاع تقرير محلي منقح.

### 4.2 أدوات لا تُفتح في V1

- execute_shell / run arbitrary command.
- install/uninstall/clear data/grant/revoke.
- push/delete/move/rename على الهاتف.
- DNS changes.
- driver install/repair/remove.
- bootloader/reboot/flash/erase.
- UI tap/swipe/text injection.
- upload أو HTTP server بعيد.

يمكن إضافة أدوات الكتابة لاحقاً بعد وجود Preview + Approval + Operation Engine، وليس بمجرد إضافة tool schema.

### 4.3 Explicit targeting

كل tool يستهدف جهازاً يأخذ device_id إلزامياً، لا serial اختياري ولا "الجهاز الوحيد تلقائياً". Application Service يحل device_id إلى DeviceHandle حديث ويتحقق من transport/state/capability قبل تنفيذ أي شيء.

### 4.4 Approval protocol للكتابة المستقبلية

1. MCP tool يطلب Plan فقط ويعيد operation preview.
2. Application Service ينشئ operation_id وhash للـplan وtarget device والآثار.
3. واجهة البرنامج تعرض target والملفات/الإعدادات والتغييرات.
4. المستخدم يوافق من واجهة موثوقة؛ تصدر one-time approval token مربوطاً بـoperation_id وplan hash وdevice_id.
5. MCP execute tool لا يقبل نص موافقة عام؛ يقبل token صالحاً مرتبطاً بالعملية نفسها.
6. أي تعديل في target أو flags أو paths أو package invalidates token ويحتاج preview جديداً.
7. Approval tokens قصيرة العمر، single-use، لا تظهر في logs ولا ترجع إلى model context أكثر من اللازم.
8. عند timeout/disconnect تتحول العملية إلى OutcomeUnknown/NeedsReconciliation، ولا يعاد تنفيذها تلقائياً.

## 5. ربط Codex بالتطبيق

**الاتجاه المفضل:** Codex يشغل MCP server محلياً عبر stdio. MCP server يستدعي Application Services المشتركة؛ لا ينشئ ADB implementation موازياً.

هناك مساران يحتاجان PoC:
- **In-process/shared library:** MCP server يعمل كعملية مستقلة لكنه يستدعي نفس Core packages/services.
- **Local host IPC:** MCP stdio process يتصل بـWindows background host عبر named pipe ACL مقيد للمستخدم الحالي.

لا نستخدم TCP localhost في V1 ما دام stdio أو named pipe يكفي. وإذا أُضيف HTTP مستقبلاً فيحتاج auth، bind localhost فقط، CSRF/DNS rebinding protections، rate limits، وعدم قبول أي remote client افتراضياً.

## 6. اختبارات القبول الأمنية والوظيفية

| الاختبار | المتوقع |
|---|---|
| MCP request بلا device_id | رفض واضح؛ لا auto-target |
| device_id غير متصل/unauthorized | Unsupported/DeviceUnavailable؛ لا fallback لجهاز آخر |
| أكثر من جهاز | كل طلب مرتبط بجهاز واحد محدد |
| read-only tool | لا يرسل أي command يغير حالة الجهاز |
| write tool غير مصرح | PermissionDenied قبل أي command |
| generic shell request | tool غير مسجل أصلاً أو مرفوض دائماً |
| package/path injection | validation يرفض الإدخال قبل بناء الأمر |
| user approval token مفقود | لا تنفيذ |
| token لعملية أو target مختلف | رفض |
| plan تغير بعد approval | invalidation وإعادة preview |
| client disconnect | cancellation/reconciliation حسب العملية |
| MCP server exit | إغلاق streams والعمليات المؤقتة التي يملكها؛ لا عمليات ADB detached غير مسجلة |
| sensitive output | serials/tokens/personal paths redacted حسب السياسة |
| Codex stdio handshake | initialize/list tools/call/read resources tests |
| concurrency | لا يتشارك عمليتان target أو temp path بطريقة متعارضة |

## 7. القرار

- نستخدم Go MCP SDK أو TypeScript MCP SDK وفق Host Core PoC؛ لا نختار اللغة قبل قرار Host/Core.
- أفضل patterns من adb-mcp: typed client per serial، exec.CommandContext، shell quoting tests، annotation completeness tests، cleanup lifecycle.
- أفضل patterns من us-all: tool categories، schema trimming/search-tools، separate read/write/shell policy layers، validation helpers.
- نرفض auto-device selection في كلتا الحالتين.
- نرفض generic shell كأداة في منتجنا.
- نرفض الاعتماد على MCP annotations أو prompt instructions وحدها لفرض الموافقة.
- لا نفتح write tools في أول نسخة MCP.

## 8. ما لم يتم إثباته

- لم نبن MCP server خاصاً بالمشروع.
- لم نربط Codex به.
- لم نشغل Go/TypeScript test suites في هذه الجولة.
- لم نختبر named pipe ACL أو stdio lifecycle في Windows.
- لم نختبر approval token flow أو adversarial tool calls.
- لم يتم تشغيل أي command على هاتف عبر هذه المراجعة.

**الحالة:** Source Revalidation مكتمل للمرشحين عند commits أعلاه. adb-mcp هو المرجع الأقوى لبنية Go/typed ADB/context lifecycle، وus-all مرجع قوي لتقسيم أدوات TypeScript/validation. كلاهما يحتاج إعادة تصميم targeting/approval قبل أي اعتماد؛ لا يوجد Codex/Runtime/Hardware evidence.
