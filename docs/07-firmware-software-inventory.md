# Android Firmware & Software Package Inventory Research

> **الغرض:** تنظيم قراءة معلومات السوفتوير المثبت على الجهاز وفحص حزم تحديث Android وبياناتها وصور الأقسام على الكمبيوتر، مع إعادة استخدام أدوات AOSP ومشاريع GitHub الجاهزة. هذه الوثيقة لا تضيف أي مسار تفليش أو كتابة إلى الجهاز.

**آخر مراجعة:** 12 نوفمبر 2026  
**النطاق:** Windows 10/11، Android 7.0 (API 24) فأحدث؛ فحص محلي Read-only.  
**المستبعد:** البحث في Qualcomm/MediaTek، بروتوكولاتهما أو صيغ firmware المرتبطة بهما؛ مقارنة التراخيص؛ تفليش أو sideload أو تحديث جهاز حقيقي.

## 1. الخلاصة التنفيذية

نفصل محور السوفتوير والفريموير إلى أربع طبقات لا ينبغي دمجها:

1. **Installed Build Inventory:** قراءة إصدار Android وbuild fingerprint وSDK/security patch وproduct/device وABI وحقول النظام من الجهاز. معظم الحقول الأساسية موثقة في [06](06-android-system-information.md).
2. **Installed Software Inventory:** جرد التطبيقات والحزم والإصدارات والمصادر والتواريخ. هذا موجود في [03](03-app-management-and-network.md) ولا نعيد بناءه هنا.
3. **Offline OTA Package Inspection:** فحص ملف تحديث محلي: نوعه، metadata، الجهاز المستهدف، build pre/postconditions، hashes، payload manifest، partition names/sizes، وعمليات التحديث.
4. **Offline Image Metadata Inspection:** فحص vbmeta/AVB وmetadata داخل صور النظام دون تركيبها أو كتابتها إلى جهاز.

**أفضل مرشح جاهز:** [ssut/payload-dumper-go](https://github.com/ssut/payload-dumper-go)؛ يوفر Go library لفتح OTA ZIP أو payload.bin، قراءة manifest، قائمة partitions وأحجامها وhashes وعمليات غير مدعومة، مع اختبارات واضحة. يمكننا اقتباسه كـsidecar أو library adapter دون تشغيل extraction افتراضياً.

**أداة AOSP مساعدة:** avbtool لعرض معلومات AVB والتحقق من ملفات الصور؛ لكن وثائق AOSP تحذر من الاعتماد على شكل مخرجات info_image لأنها قد تتغير.

## 2. معلومات السوفتوير المثبتة على الجهاز

### 2.1 الحقول التي نعيد استخدامها من وثيقة 06

- Android release وSDK/API level.
- Build ID وDisplay ID وIncremental version.
- Build fingerprint وproduct/device/brand/manufacturer.
- Security patch level وbuild timestamp.
- ABI list وbootloader version وbaseband عند توفرها.
- build type/tags وverified boot properties عندما تكون متاحة.
- current slot/slot suffix وdynamic partition hints عند وجود دليل موثوق.

مصادر القراءة:
- getprop whitelist.
- pm list features/libraries.
- Fastboot getvar read-only للمتغيرات العامة التي تمت مراجعتها في [01](01-connectivity-protocols.md).
- ملفات metadata محلية إذا كان المستخدم قد وفرها.

**لا نعرض "Firmware version" كحقل واحد مبهم.** نفصل Android build عن vendor build وعن bootloader/baseband وعن OTA package version؛ قد تختلف تواريخها ومصادرها.

### 2.2 المقارنة بين الجهاز وحزمة التحديث

عند توفر OTA package:
- نقارن product/device/codename مع بيانات الجهاز.
- نقارن pre-build/pre-device إذا كانت الحزمة incremental.
- نقارن post-build/post-device مع النسخة التي تستهدفها الحزمة.
- نعرض security patch قبل وبعد.
- نعرض هل الحزمة full أم incremental/partial بحسب metadata وpayload manifest.
- لا نعلن أن الحزمة "متوافقة" اعتماداً على اسم موديل تسويقي وحده.

إذا كانت metadata غير كافية أو متناقضة، تكون النتيجة **Compatibility Unknown** أو **Mismatch**، لا "Compatible" افتراضياً.

## 3. أشكال OTA وملفاتها

### 3.1 Android OTA package

AOSP يوثق أن OTA package قد تحتوي metadata وشروط ما قبل/بعد التحديث، وأن update packages قد تكون full أو incremental، وأن Android يستخدم A/B وnon-A/B mechanisms بحسب الجهاز.

مراجع:
- https://source.android.com/docs/core/ota
- https://source.android.com/docs/core/ota/tools

أهم العناصر التي قد تظهر داخل ZIP:

| العنصر | ما نقرأه |
|---|---|
| META-INF/com/android/metadata | pre/post build، device، fingerprint، SDK level، security patch وغيرها حسب الحزمة |
| payload.bin | حاوية update_engine payload التي تضم manifest وبيانات partition operations |
| payload_properties.txt | FILE_HASH وFILE_SIZE وMETADATA_HASH وMETADATA_SIZE في تطبيقات update_engine الشائعة |
| care_map.pb | metadata متعلقة بمناطق/خرائط care عند وجودها |
| compatibility.zip | ملفات compatibility metadata عند وجودها |
| android-info.txt | معلومات build/device في بعض factory packages |
| *.img | صور أقسام مباشرة في بعض الحزم |
| *.new.dat / *.new.dat.br / *.transfer.list | آثار من صيغ block/file-based OTA القديمة أو غير A/B؛ دعمها يحتاج adapter مستقل |
| boot.img / vendor_boot.img / init_boot.img / vbmeta.img / dtbo.img | أسماء صور قد تظهر حسب إصدار Android والجهاز؛ لا تفترض وجودها جميعاً |

ملاحظة توافق: AOSP يذكر أن file-based OTA packages لأجهزة non-A/B تم إهمالها منذ Android 8.0 لصالح block-based OTA. هذا يعني أن parser واحداً لـpayload.bin لا يغطي كل حزم Android القديمة.

### 3.2 OTA metadata

ملف META-INF/com/android/metadata قد يتضمن حقولاً مثل:

- pre-device / post-device
- pre-build / post-build
- pre-build-incremental / post-build-incremental
- pre-sdk-level / post-sdk-level
- pre-security-patch-level / post-security-patch-level

الحقول الفعلية تختلف حسب نوع الحزمة وأداة البناء. لا نفترض وجود precondition في full OTA، ولا نعامل الحقول المفقودة كـwildcard متوافق.

### 3.3 payload.bin

صيغة update_engine payload تبدأ header، ثم metadata manifest (Protocol Buffers)، ثم metadata signature وبيانات عمليات التحديث. الـmanifest يصف partitions وInstallOperations وextents وhashes وdynamic partition metadata وغيرها.

المصدر الأساسي للـschema:
- https://android.googlesource.com/platform/system/update_engine/+/master/update_metadata.proto
- نسخة schema المضمنة في مشروع payload-dumper-go: https://github.com/ssut/payload-dumper-go/blob/main/update_metadata.proto

البيانات التي نحتاجها من manifest:
- payload major/minor version.
- block size.
- partition name.
- old/new partition size and hash.
- هل العملية تحتاج source image (incremental).
- عمليات التحديث التي تظهر لكل partition.
- unsupported operation types.
- dynamic partition group metadata عندما توجد.

لا نحتاج استخراج صور partitions لكي نعرض هذه المعلومات؛ يمكن قراءة header/manifest أولاً ثم سؤال المستخدم إن كان يريد extraction محلياً.

### 3.4 payload_properties.txt

Android UpdateEngine API يوضح أن header key/value pairs تشمل في تطبيق Google الشائع:
- FILE_HASH
- FILE_SIZE
- METADATA_HASH
- METADATA_SIZE

مرجع المصدر:
https://android.googlesource.com/platform/frameworks/base/+/master/core/java/android/os/UpdateEngine.java

هذه قيم تحقق metadata/payload وليست وحدها إثباتاً لهوية الناشر أو أن الحزمة تخص الهاتف الحالي.

## 4. تدقيق ssut/payload-dumper-go

المستودع: https://github.com/ssut/payload-dumper-go  
الفرع: main. تم فحص README وشجرة الملفات وpayload.go وheader.go وinput.go وextract.go وpayload_test.go وupdate_metadata.proto وGitHub workflows وgoreleaser config.

### 4.1 ما يوفره المصدر

- Go package قابلة للاستخدام كمكتبة: github.com/ssut/payload-dumper-go/payload.
- Open() يفتح payload.bin مباشرة أو يبحث عن payload entry داخل OTA ZIP.
- Header() يقرأ payload version وmanifest/signature lengths.
- Manifest() يعيد DeltaArchiveManifest protobuf.
- Partitions() يعطي أسماء الأقسام وأحجامها وhashes وold hashes وsource requirements والعمليات غير المدعومة.
- IsDelta() يميز payload الذي يحتاج بيانات المصدر.
- Extract() يدعم استخراج partitions المحددة، full/incremental operations المدعومة، ويحسب/يتحقق من SHA-256 للصور النهائية.
- الاختبارات تغطي full payload، selected partitions، unknown partition، delta payload، missing/wrong source images، unsupported operations، hash mismatch، ZIP input، وverity metadata.
- README يعلن prebuilt Windows releases وGo library integration.

### 4.2 الحدود المعروفة

- PUFFDIFF وZUCCHINI وLZ4DIFF_* غير مدعومة حالياً؛ قد تفشل partitions التي تستخدمها مع رسالة واضحة.
- الاستخراج يعتمد على xz system dependency في README، ويفضل SSD للحزم الكبيرة.
- بعض incremental OTA تحتاج base images لإعادة بناء النتيجة.
- extraction قد يحتاج مساحة كبيرة جداً؛ لا تشغله تلقائياً عند مجرد فتح package.
- دعم parser لا يعني أن كل vendor package يحتوي payload.bin.
- README يذكر أن -no-fec يتجاوز FEC generation وبعض التحقق؛ لا نستخدم هذا الخيار في مسار التحقق الافتراضي.

### 4.3 قرار إعادة الاستخدام

**اعتماد مبدئي كمكتبة metadata inspection بعد adapter واختباراتنا.**

مسار الاستخدام المقترح:
1. Open ZIP/payload.
2. Read Header and Manifest.
3. Return JSON structured inventory.
4. List partitions, sizes, hashes, source requirements, unsupported operations.
5. Close input.
6. لا Extract إلا بعد اختيار المستخدم ومساحة تخزين كافية وتحذير أن المخرجات صور firmware محلية وليست تثبيتاً.

لا نحتاج إعادة كتابة protobuf schema أو parser من الصفر.

روابط المصدر:
- [payload.go](https://github.com/ssut/payload-dumper-go/blob/main/payload/payload.go)
- [header.go](https://github.com/ssut/payload-dumper-go/blob/main/payload/header.go)
- [input.go](https://github.com/ssut/payload-dumper-go/blob/main/payload/input.go)
- [extract.go](https://github.com/ssut/payload-dumper-go/blob/main/payload/extract.go)
- [payload tests](https://github.com/ssut/payload-dumper-go/blob/main/payload/payload_test.go)
- [Windows release configuration](https://github.com/ssut/payload-dumper-go/blob/main/.goreleaser.yml)
- [update_metadata.proto](https://github.com/ssut/payload-dumper-go/blob/main/update_metadata.proto)

## 5. AVB / vbmeta image metadata

Android Verified Boot يستخدم vbmeta metadata للتحقق من صور مثل boot وsystem وغيرها. vbmeta يمكن أن يحتوي descriptors وdigests وhashtree data وpublic-key information وrollback indexes.

AOSP يوفر avbtool:
- info_image لعرض معلومات AVB في image.
- verify_image للتحقق من توقيع VBMeta والـhash/hashtree descriptors عندما تتوفر الصور المرتبطة.
- calculate_vbmeta_digest لحساب digest.

مرجع AOSP:
https://android.googlesource.com/platform/external/avb/+/master/README.md

**تحذير مهم من AOSP:** مخرجات avbtool info_image ليست واجهة ثابتة ولا ينبغي الاعتماد على شكلها النصي. لذلك:
- نستخدمها في diagnostics/manual inspection أو نقرأ metadata عبر parser منظم.
- لا نعتمد على regex نصي لمخرجات info_image كعقد API.
- verify_image قد يثبت اتساق الصور مع المفتاح المضمن، لكنه لا يثبت وحده أن المفتاح موثوق لدى الشركة المصنّعة؛ يجب توافر trust anchor متوقع للمقارنة.
- لا نعدل vbmeta ولا ننشئ صوراً معدلة ولا نكتبها إلى الهاتف.

## 6. نموذج Firmware Package Record

| الحقل | الغرض |
|---|---|
| package_id | معرف داخلي ثابت للحزمة |
| source_uri / source_label | المصدر الذي جلب منه المستخدم الحزمة |
| local_path | مسار الملف المحلي |
| container_type | ZIP / payload.bin / image / unknown |
| file_size_bytes | الحجم |
| sha256 | hash للملف الأصلي |
| signature_status | Verified / Failed / NotChecked / Unsupported / Unknown |
| package_type | Full OTA / Incremental OTA / Factory archive / Unknown |
| pre_device / post_device | device/codename pre/post |
| pre_build / post_build | build fingerprints pre/post |
| pre_sdk / post_sdk | SDK/API pre/post |
| pre_security_patch / post_security_patch | patch levels pre/post |
| payload_version | version |
| partitions[] | name, old_size, new_size, hashes, operations, source_required |
| dynamic_partition_groups[] | أسماء المجموعات والحدود والأعضاء إن وجدت |
| inspection_status | Success / Partial / Unsupported / Corrupt / Error |
| warnings[] | أسباب عدم اليقين أو incompatibility |
| inspected_at | وقت الفحص |

نحتفظ بالقيم الخام ونتيجة parsing؛ لا نكتب فوق الأصل ولا نغير اسم package أو صور الأقسام.

## 7. سياسة الفحص والأمان

- كل الحزم تعامل كمدخل غير موثوق.
- لا نشغل scripts أو executables من داخل ZIP.
- لا نفك ZIP إلى مسار غير محكوم؛ نمنع Zip Slip وZip Bomb وduplicate entry ambiguity.
- حدود حجم archive، إجمالي الحجم المفكوك، عدد الملفات، عمق المسارات، ووقت التحليل.
- hash الملف الأصلي قبل parsing وبعده إذا احتاجت العملية إعادة قراءة.
- لا نعلن Signature Verified إلا بعد تحقق cryptographic فعلي بمفتاح موثوق ومحدد.
- نفصل package metadata من signature verification ومن partition hash verification.
- لا نقبل package لموديل مختلف على أنه "غالباً مناسب".
- لا ننفذ adb sideload أو update_engine applyPayload أو fastboot flash أو أي كتابة إلى الأقسام.
- لا نعيد تشغيل الهاتف إلى وضع تحديث كجزء من فحص ملف.
- extraction على الكمبيوتر فقط، إلى مجلد جديد ومحدد، ولا يلمس الهاتف.

## 8. أولويات المنتج

| الأولوية | الوظائف |
|---|---|
| P0 | قراءة build info من الجهاز عبر 06، فحص OTA ZIP metadata، قراءة payload header/manifest، partition list/sizes/hashes، package hash، compatibility report |
| P1 | payload-dumper-go extraction لصور محددة على الكمبيوتر، AVB info/verification، incremental source-image inventory |
| P2 | parsers لصيغ legacy OTA غير payload.bin، فحص factory archives العامة، مقارنة عدة builds وحزم محلية |

لا تتضمن أي أولوية في هذا الجدول تفليشاً أو sideload أو تحديث جهاز.

## 9. اختبارات القبول

| الاختبار | المتوقع |
|---|---|
| ZIP بلا payload.bin ولا metadata | Unknown/Unsupported مع تقرير واضح |
| OTA ZIP مع metadata فقط | عرض metadata دون استخراج |
| payload.bin صالح | عرض version/manifest/partitions/sizes/hashes |
| payload header تالف | Corrupt مع إيقاف التحليل بأمان |
| manifest protobuf غير صالح | ParseError دون crash |
| payload incremental بلا base images | عرض source requirements؛ لا يدعي اكتمال extraction |
| unsupported delta operation | قائمة operation/partition واضحة؛ لا extraction جزئي صامت |
| hash mismatch | فشل integrity verification |
| ZIP path traversal | رفض entry غير الآمن |
| ZIP bomb/huge file | إيقاف ضمن limits |
| package mismatch | Mismatch مع تحديد الحقل الذي لم يتطابق |
| signature غير متحقق | NotChecked، لا Verified |
| vbmeta بلا trust anchor | فصل embedded signature integrity عن OEM authenticity |
| extraction إلى مجلد موجود | رفض أو اختيار مجلد جديد؛ لا overwrite صامت |
| الجهاز غير متصل | فحص الملف المحلي يظل ممكناً ولا يحاول الاتصال بالهاتف |

## 10. المصادر

### AOSP / Android

- [OTA updates](https://source.android.com/docs/core/ota)
- [Build OTA packages and metadata](https://source.android.com/docs/core/ota/tools)
- [UpdateEngine API](https://android.googlesource.com/platform/frameworks/base/+/master/core/java/android/os/UpdateEngine.java)
- [update_metadata.proto](https://android.googlesource.com/platform/system/update_engine/+/master/update_metadata.proto)
- [AVB / avbtool README](https://android.googlesource.com/platform/external/avb/+/master/README.md)
- [AVB version information](https://source.android.com/docs/core/architecture/bootloader/version-info-avb)

### GitHub

- [ssut/payload-dumper-go](https://github.com/ssut/payload-dumper-go)
- [payload package API](https://github.com/ssut/payload-dumper-go/tree/main/payload)
- [payload manifest schema](https://github.com/ssut/payload-dumper-go/blob/main/update_metadata.proto)

## 11. ما لم يتم إثباته

- لم يتم تشغيل payload-dumper-go أو بناءه على Windows في هذه الجولة.
- لم يتم اختبار OTA packages متعددة من أجهزة مختلفة.
- لم يتم التحقق من توقيع حزمة OEM بمفتاح موثوق.
- لم يتم استخراج أي partition image.
- لم يتم تركيب أو تحليل صور ملفات النظام.
- لم يتم تنفيذ sideload أو flash أو أي كتابة إلى جهاز.

**الحالة:** البحث ومراجعة المصدر تمت؛ المرشح الأساسي metadata inspection هو payload-dumper-go، مع avbtool كأداة AOSP مساعدة. لا يوجد اعتماد دمج أو اختبار Hardware أو أي إجراء تحديث للجهاز.
