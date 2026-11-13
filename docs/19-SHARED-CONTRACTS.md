# العقود المشتركة — مواصفة قابلة للتنفيذ

> هذه ليست وصفاً نثرياً للعقود؛ هي **مواصفة منفذة وقابلة للتحقق**. الملفات الفعلية في [`contracts/`](../contracts/)، والتحقق يعمل الآن على أي نظام بلا .NET:
>
> ```bash
> python contracts/tools/validate_contracts.py
> # contracts: 45 checks over 7 schemas, 11 examples, 10 negative cases
> # OK: all contracts hold
> ```
>
> **ما يثبته هذا الملف:** العقود مُعرّفة، متسقة، وترفض الحالات الخطأ (10 حالات سلبية).
> **ما لا يثبته:** لم يُنفَّذ أي كود C# يستهلكها، ولم تُختبر ضد جهاز.

**التاريخ:** 13 نوفمبر 2026 — **يعتمد على:** [ADR-0001](17-adr-0001-stack-decision.md)، [18](18-ENGINEERING-DECISIONS.md)

---

## 1. لماذا العقود قبل الكود

الوثيقة [12 §4.2](12-REUSE-AND-IMPLEMENTATION-PLAN.md) كتبت DeviceHandle وOperation وCapability وResult **نثراً**. النثر لا يمنع خطأً برمجياً، ولا يسمح لـGUI وMCP بالتشارك فعلياً. هنا تُحوَّل إلى JSON Schema، فتصبح:

- **قابلة للفحص آلياً** — أي payload يُخالف العقد يُرفض في الاختبار.
- **قابلة للاستخدام من الـC#** — تُولَّد منها أنواع عبر `NJsonSchema`/`JsonSchema.Net.Generation` أو تُكتب يدوياً مع اختبار توافق.
- **قابلة للنقل لـMCP** — نفس الـschema تصبح `inputSchema` للأداة.
- **حاوية للسياسة** — "لا كتابة بدون موافقة" صارت شرطاً داخل الـschema، لا نصيحة في وثيقة.

## 2. خريطة الملفات

| الملف | يغطي | القاعدة التي يفرضها |
|---|---|---|
| [`_common.schema.json`](../contracts/schemas/_common.schema.json) | تعريفات مشتركة: deviceId، transportKind، problemCode، operationState، impactLevel، approvalToken | مجموعة `problemCode` مغلقة؛ لا نصوص خطأ حرة |
| [`device-handle.schema.json`](../contracts/schemas/device-handle.schema.json) | هوية الجهاز وقناته وقدراته | `deviceId` بنمط `dev_[0-9a-f]{16}` — **السيريال العتادي لا يمكن أن يكون deviceId** |
| [`capability.schema.json`](../contracts/schemas/capability.schema.json) | قدرة واحدة على جهاز واحد وقناة واحدة | `Unsupported`/`PermissionDenied` **تفرض** وجود `reason` |
| [`operation.schema.json`](../contracts/schemas/operation.schema.json) | دورة حياة الوحدة الواحدة من العمل | DeviceWrite **تفرض** `planId` + `approvalTokenId`؛ الحالة النهائية تفرض `result` + `endedAt` |
| [`result.schema.json`](../contracts/schemas/result.schema.json) | نتيجة خطوة/عملية | `ok=true` ⇔ `problemCode=OK`؛ `OutcomeUnknown` تفرض `reconcile` وتحرّم `recoverability=Retryable` |
| [`plan.schema.json`](../contracts/schemas/plan.schema.json) | خطة منفَّذة قبل التشغيل | الأوامر `argv[]` منفصلة — **لا سلسلة shell واحدة**؛ خطة الكتابة تفرض خطوة تحقق |
| [`audit-event.schema.json`](../contracts/schemas/audit-event.schema.json) | سجل إلحاقي فقط | `detail.redacted` تفرض `true`؛ `previousEventHash` سلسلة hash |

**الأمثلة** في [`contracts/examples/`](../contracts/examples/): 11 مثالاً صحيحاً، منها ثلاثة تخص السيناريوهات الحرجة: عملية قراءة ناجحة، عملية كتابة انتهت بـ`OutcomeUnknown`، وخطة تثبيت APKS.

## 3. القرارات التصميمية الحاكمة

### 3.1 `problemCode` مجموعة مغلقة، والرسالة نص بشري

```jsonc
// ممنوع: callers تفسّر النص
{ "message": "error: device offline" }

// المعتمد
{ "ok": false, "problemCode": "DeviceDisconnected", "recoverability": "RetryAfterReconcile", "message": "…" }
```
السبب: الرسائل تُترجم وتتغير؛ الشيفرة لا. كل branch في GUI/MCP يعتمد على `problemCode` فقط.

### 3.2 `ok` و`exitCode` ليسا نفس الشيء

`exitCode` موجود للتشخيص، و**لا يدخل في الحكم**. حالة `adb install` التي تدخل `Success` في stdout ثم ينقطع الجهاز أثناء الـverification يجب أن تنتهي `ok=false` + `OutcomeUnknown`، وليس `ok=true`.

### 3.3 الموافقة مرتبطة بالخطة بـhash لا بالنية

`approvalToken.scopeDigest` = SHA-256 على JSON القانوني (مفاتيح مرتبة، UTF-8، بلا مسافات) للـ`Plan`. أي تغيير في بايت واحد يُبطل الموافقة. هذا يمنع:
- "وافقت على تثبيت 3MB" ثم يُثبَّت 300MB.
- MCP يعيد استخدام token عملية أخرى.
- TOCTOU بين العرض والتنفيذ.

### 3.4 لا كتابة بلا خطة وتحقّق

شرطان في الـschema (راجع `operation.allOf[0]` و`plan.allOf[0]`): عملية `DeviceWrite` تفرض وجود `planId` و`approvalTokenId`، وخطة الكتابة تفرض خطوة تحقق (`VerifyPackageState`/`VerifyFileState`/`RefreshDeviceState`). أي تنفيذ يخالف هذا يُرفض في الاختبار لا في المراجعة.

### 3.5 `OutcomeUnknown` ليست نجاحاً ولا فشلاً

`OutcomeUnknown` و`NeedsReconciliation` **ليستا نهائيتين**. الـschema:
- تفرض وجود `reconcile` block.
- تحرّم `recoverability=Retryable` (لا إعادة محاولة عمياء).
- تمنع `ok=true`.

هذا هو المبدأ الذي نصّت عليه [00 §9.2](00-PRODUCT-BASELINE.md) ("لا تُعاد العملية تلقائياً إذا كان من الممكن أن تكون قد نُفذت جزئياً")، لكنه أصبح قابلاً للفحص.

### 3.6 السيريال لا يظهر في أي عقد

`deviceId` مشتق (16 hex) وثابت على الجهاز المحلي، و`transportIdentity.redacted` يحمل آخر 4 أحرف فقط مع سياسة تنقيح صريحة. `audit-event` تفرض `detail.redacted=true`. هذا ينفّذ قاعدة [18 §4](18-ENGINEERING-DECISIONS.md) تقنياً، لا كنصيحة.

### 3.7 التحقق مستقل عن الأمر

`result.verification` كائن مستقل: `performed` و`verified` منفصلان عن `exitCode`. والـschema تمنع `ok=true` مع `verified=false`. عملياً: النجاح يعني **قرأنا الحالة المستهدفة**، لا أن الأمر عاد بصفر.

## 4. الحالات السلبية المفروضة (10)

الملف `validate_contracts.py` يرفض هذه الوثائق؛ أي منها لو مرّ يعني أن العقد ضعيف:

| # | الحالة | القاعدة المنتهكة |
|---:|---|---|
| 1 | `ok=true` + `problemCode=Timeout` | النجاح يعني OK |
| 2 | `ok=false` + `problemCode=OK` | الفشل يحمل شيفرة حقيقية |
| 3 | `OutcomeUnknown` بلا `reconcile` | المصالحة إلزامية |
| 4 | `OutcomeUnknown` + `Retryable` | لا إعادة محاولة عمياء |
| 5 | `ok=true` + `verified=false` | النجاح يتطلب حالة مرصودة |
| 6 | DeviceWrite بلا `planId`/`approvalTokenId` | لا كتابة بلا موافقة |
| 7 | حالة نهائية بلا `result`/`endedAt` | لا نجاح بلا دليل |
| 8 | `Unsupported` بلا `reason` | الرسالة قابلة للتنفيذ |
| 9 | `deviceId` = سيريال عتادي | لا هوية حساسة في العقد |
| 10 | خطة كتابة بلا خطوة تحقق | التحقق مصمَّم لا مضاف |

## 5. كيف تُستهلك في .NET (خطة التوليد)

| المسار | الطريقة | اختبار التوافق |
|---|---|---|
| Contract → C# types | كتابة `record` يدوية مطابقة + `JsonSerializerContext` (source-generated) | اختبار `round-trip` لكل مثال في `contracts/examples` |
| C# types → Contract | `JsonSchema.Net.Generation` وقت الاختبار فقط | اختبار يثبت أن الـschema الناتج يقبل الأمثلة ويرفض الحالات السلبية |
| MCP `inputSchema` | نفس ملفات JSON كما هي | اختبار يتحقق أن كل أداة MCP تشير إلى schema موجود |
| قاعدة البيانات | جداول `operations`, `audit_events`, `device_handles` بأعمدة تطابق الحقول | اختبار migration + اختبار أن `audit_events` لا يقبل UPDATE/DELETE |

**قرار:** لا تُولَّد أنواع C# تلقائياً من الـschema وقت البناء، لأن ذلك يربط الكود بأداة توليد ويصعّب تشخيص الأخطاء. بدلاً من ذلك: أنواع مكتوبة يدوياً + اختبار توافق ثنائي الاتجاه. التوليد الآلي يُعاد تقييمه إذا تضاعف عدد العقود.

## 6. الخطوة التالية المباشرة

| # | العمل | الشرط |
|---:|---|---|
| 1 | `Contracts.Tests` في الحل: يقرأ `contracts/examples` ويفحص كل `record` | يجب أن يرفض نفس الحالات السلبية العشرة |
| 2 | `ApprovalService` — إصدار token، حساب `scopeDigest`، التحقق من التطابق والانتهاء | اختبارات: تغيير بايت واحد في الخطة يُبطل الموافقة |
| 3 | `AuditStore` — إلحاقي، سلسلة hash، منع UPDATE/DELETE على مستوى المستودع | اختبار: كسر السلسلة يرفع `AuditIntegrityViolation` |
| 4 | تحديث [12 §4.2](12-REUSE-AND-IMPLEMENTATION-PLAN.md) لتحيل هنا بدلاً من تكرار النثر | بعد دمج الكود |
| 5 | ربط fixtures الفئة هـ ([corpus](../contracts/corpus/README.md)) بالحالات السلبية العشرة كأمثلة ملفات | بعد توليد الفئة هـ |

## 7. التحقق يعمل فعلياً (لا ادعاء)

```bash
$ python contracts/tools/validate_contracts.py
contracts: 45 checks over 7 schemas, 11 examples, 10 negative cases

1 warning(s):
  - _common.schema.json: schema does not set additionalProperties=false at the top level
OK: all contracts hold
```

**وقد أثبت أنه يرفض فعلاً:** تغيير `problemCode` في `result.ok.example.json` من `OK` إلى `Timeout` أنتج:

```
1 violation(s):
  ✗ example result.ok.example.json does not conform to result.schema.json:
    at problemCode: 'OK' was expected
EXIT=1
```

**آخر تحديث:** 13 نوفمبر 2026. **الحالة:** العقود مُنفَّذة كمواصفة ومُتحقَّق منها بمدقّق يعمل ومُختبر ضد حالات سلبية؛ كود C# غير مكتوب بعد.
