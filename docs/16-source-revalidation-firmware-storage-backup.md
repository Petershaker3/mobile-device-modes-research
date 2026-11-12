# Source Revalidation — OTA Payload, Dynamic Partitions & File Backup

> **الغرض:** مراجعة مصادر parser الأساسية للـOTA/super metadata والنسخ الاحتياطي للملفات عند SHAs مثبتة، وتحديد ما يصلح للفحص المحلي read-only وما يحتاج hardening قبل الاعتماد.

**تاريخ المراجعة:** 12 نوفمبر 2026  
**المنصة:** Windows 10/11 وAndroid 7/API 24 فأحدث.  
**الحالة:** Source Revalidation — لم نشغل الأدوات ولم نقرأ جهازاً حقيقياً.

## 1. الخلاصة التنفيذية

| المحور | المرشح | النتيجة |
|---|---|---|
| OTA payload metadata | [payload-dumper-go](https://github.com/ssut/payload-dumper-go/tree/c3a50ba8d784764c06143c1f82971cd81a77f9d6) | أقوى مرشح: list-only + machine-readable + Go API + Windows builds + parser tests؛ يلزم manifest size limits والتحقق المستقل من OTA metadata signature |
| Dynamic partitions raw/sparse | [lucky-arch](https://github.com/soe1hom-arch/lucky-arch/tree/09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b) | list-only دون استخراج وsparse ReaderAt بلا raw intermediate؛ لا tests، slot 0 فقط، header checksum لا يظهر التحقق منه، ويحتاج bounds hardening |
| Windows super metadata JSON | [lpunpack_cyth](https://github.com/ahsanihlwn/lpunpack_cyth/tree/7ace45e6166d5b3b243c085a92e931d6a2809663) | Windows exe معلن و--info JSON، لكنه يفك sparse إلى raw file حتى في info mode، slot غير مطبق، checksum validation غير ظاهر، وJSON يحذف حقول صفرية |
| Incremental shared-file backup | [chen3feng/android-backup](https://github.com/chen3feng/android-backup/tree/dca2c38c60cc17237fa4aaff0cac06475d02ee66) | مرجع خوارزمية scan/sync/hardlink snapshots وWindows Unicode workaround؛ auto-all-devices وsize/mtime-only وexclude-after-pull، ولا restore أو tests واضحة |

**الاتجاه:** payload-dumper-go هو المرشح الأول لفحص OTA metadata؛ AOSP liblp يظل مرجع صحة، وlucky-arch مرشح list-only يحتاج اختباراً؛ lpunpack_cyth لا نستخدمه في metadata-only flow قبل إصلاح sparse temp/slot/checksum/JSON؛ chen3feng مرجع incremental algorithm وليس backup engine جاهزاً.

## 2. payload-dumper-go — OTA payload parser

المستودع:
https://github.com/ssut/payload-dumper-go/tree/c3a50ba8d784764c06143c1f82971cd81a77f9d6

- Commit: c3a50ba8d784764c06143c1f82971cd81a77f9d6
- [README](https://github.com/ssut/payload-dumper-go/blob/c3a50ba8d784764c06143c1f82971cd81a77f9d6/README.md)
- [CLI root](https://github.com/ssut/payload-dumper-go/blob/c3a50ba8d784764c06143c1f82971cd81a77f9d6/cmd/root.go)
- [Payload model](https://github.com/ssut/payload-dumper-go/blob/c3a50ba8d784764c06143c1f82971cd81a77f9d6/payload/payload.go)
- [Input detection](https://github.com/ssut/payload-dumper-go/blob/c3a50ba8d784764c06143c1f82971cd81a77f9d6/payload/input.go)
- [Extraction engine](https://github.com/ssut/payload-dumper-go/blob/c3a50ba8d784764c06143c1f82971cd81a77f9d6/payload/extract.go)

### ما ثبت في المصدر

- يفتح payload.bin خاماً أو داخل OTA ZIP؛ إذا كان payload.bin مخزناً بلا ضغط يستخدم ReaderAt/SectionReader دون نسخة مؤقتة، وإذا كان مضغوطاً يستخرجه إلى temp file.
- parser يقرأ header وmanifest protobuf ويعرض partitions مع size/hash/old hash/operation count/needs-source/unsupported operations.
- CLI يدعم -list و-machine-readable؛ list mode يطبع أسماء الأقسام وأحجامها ويخرج قبل extraction.
- API يمكن استخدامه مباشرة عبر payload.Open وPartitions دون تحليل النص المطبوع.
- extraction يدعم اختيار partitions، concurrency، incremental payload مع base images، source/output SHA256 verification، وdm-verity hash tree/FEC generation.
- -no-verify و-no-fec opt-outs؛ لا نستخدمهما في مسار extraction الافتراضي. -no-fec ينتج صوراً لا تطابق SHA256 ويصرح README بأنها لا يجب أن تُفلش.
- يدعم Windows releases، مع workflow منفصل لـWindows ARM64.
- توجد unit tests في payload/FEC/bspatch/verity، وreal payload integration tests تعتمد على fixtures/ملفات خارجية بحسب إعداد الاختبار.

### حدود مهمة

1. -list هو المسار المناسب لـmetadata-only؛ لا نشغّل Extract لمجرد قراءة partition inventory.
2. PUFFDIFF وZUCCHINI وLZ4DIFF_* غير مدعومة في النسخة التي راجعناها؛ بعض partitions تفشل مع رسالة واضحة. لا نعلن أن كل OTA payload قابل للاستخراج.
3. manifestBuf يُخصص بحجم ManifestLen المأخوذ من header بعد التحقق من أنه لا يتجاوز حجم الملف؛ لم يظهر cap مستقل أعلى لحجم manifest أو عدد partitions/operations. نضيف limits في adapter أو نراجع upstream قبل تمرير ملفات غير موثوقة.
4. parser يقرأ MetadataSignatureLen ويعرض طوله؛ هذا وحده ليس تحققاً من توقيع OTA cryptographically ولا إثباتاً لمصدر التحديث. نحتاج verifier مستقل/سلسلة ثقة للمصدر.
5. استخراج OTA delta قد يحتاج base images؛ لا نستنتج أن الصورة كاملة إذا كان payload partial/delta.
6. استخراج partitions عملية محلية تكتب ملفات كبيرة؛ تحتاج disk-space preflight وunique output folder وatomic staging. ليست وظيفة جهاز ولا تفليش.

**الحكم:** مرشح أول لـOTA payload metadata وpartition manifest. نبدأ بAPI list-only، ونضيف resource limits وsource provenance، ولا نربطه بأي fastboot/flash path.

## 3. lucky-arch — sparse super metadata/list-only

المستودع:
https://github.com/soe1hom-arch/lucky-arch/tree/09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b

- Commit: 09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b
- [README](https://github.com/soe1hom-arch/lucky-arch/blob/09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b/README.md)
- [main.go](https://github.com/soe1hom-arch/lucky-arch/blob/09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b/main.go)
- [super.go](https://github.com/soe1hom-arch/lucky-arch/blob/09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b/super.go)
- [sparse.go](https://github.com/soe1hom-arch/lucky-arch/blob/09ba1a6b4b2c8d603fcb1d8bfe7cf167c9e0463b/sparse.go)

### ما ثبت في المصدر

- pure-Go binary يقرأ raw أو Android sparse super.img.
- SparseReaderAt يبني chunk index ويقرأ RAW/FILL/DONTCARE دون فك sparse image كاملة إلى raw file.
- -list يقرأ metadata ويعرض partition names/sizes دون extraction.
- يقرأ geometry وLP metadata، ويجرب primary metadata ثم backup metadata.
- يتحقق من geometry SHA256 checksum وtables SHA256 checksum، ويحدد metadata versions المدعومة.
- يتحقق من بعض حدود الجداول وextent ranges وblock device index.
- يدعم v10.0–v10.2 وفق المصدر الحالي.
- README يصرح أن slot 0 فقط هو الذي يُقرأ؛ slot-suffixed names تلحق بها _a.

### فجوات الاعتماد

1. لا توجد test files أو CI workflow في tree عند SHA المراجع؛ لا نعتبر الكود موثوقاً لمجرد نجاح build غير منفذ.
2. parseHeader يقرأ HeaderChecksum لكنه لا يظهر تحققاً من SHA256 الخاص بالـheader؛ geometry وtables checksums يتم التحقق منهما.
3. metadata_max_size وsparse TotalChunks يؤثران على allocations؛ لم يظهر حد أقصى واضح يمنع memory exhaustion من ملف غير موثوق.
4. لا يوجد اختيار slot غير slot 0؛ لا نعرض slot _a على أنه active slot ولا نستنتج slot _b غير مقروء.
5. extent TargetSource يتم التحقق من أنه ضمن block device table، لكن مسار extraction يقرأ من ReaderAt واحد ويستخدم TargetData مباشرة؛ لا يظهر mapping منفصل لمصادر block devices متعددة. نحتاج رفض/دعم صريح للحالات متعددة block devices.
6. list-only لا يثبت أن كل extent يقع ضمن backing image؛ يلزم range validation قبل عرض الحجم على أنه verified.
7. ExtractPartitions ينشئ ملفات output ويستخدم O_TRUNC؛ لا نستخدم extraction في المنتج دون Preview/unique staging وعدم overwrite صامت.
8. لا يوجد اختيار partitions في CLI؛ list-only هو المسار الذي يطابق احتياجنا حالياً.

**الحكم:** أفضل مرشح بحثي للـsparse-aware metadata-only listing، لكن يحتاج tests وhardening وslot/block-device validation قبل اعتماد parser.

## 4. lpunpack_cyth — Windows metadata JSON candidate

المستودع:
https://github.com/ahsanihlwn/lpunpack_cyth/tree/7ace45e6166d5b3b243c085a92e931d6a2809663

- Commit: 7ace45e6166d5b3b243c085a92e931d6a2809663
- [README](https://github.com/ahsanihlwn/lpunpack_cyth/blob/7ace45e6166d5b3b243c085a92e931d6a2809663/README.md)
- [CLI](https://github.com/ahsanihlwn/lpunpack_cyth/blob/7ace45e6166d5b3b243c085a92e931d6a2809663/lpunpack_cli.py)
- [Cython parser](https://github.com/ahsanihlwn/lpunpack_cyth/blob/7ace45e6166d5b3b243c085a92e931d6a2809663/src/lpunpack_src.pyx)

### ما يقدمه

- Windows lpunpack.exe معلن في README.
- --info مع --format json لعرض partitions/groups/block devices/layout دون استخراج ملفات partition.
- يقبل raw وsparse super images.
- يحتوي metadata tables وgeometry وslot count وheader flags fields.

### فجوات مهمة

1. SparseImage.unsparse() ينشئ ملف .unsparse.img خاماً بجانب الصورة، حتى عند طلب --info؛ لذلك metadata-only قد يستهلك مساحة بحجم super raw بالكامل.
2. خيار --slot موجود في CLI لكنه يعلن صراحةً أن التنفيذ غير مطبق، وLpUnpack يضبط _slot_num=None.
3. _read_metadata_header يتحقق من magic لكنه لا يظهر تحققاً من geometry/header/tables checksums؛ لا نعتبر البيانات Integrity Verified.
4. JSON encoder يزيل كل الحقول التي قيمتها صفر، ويحول integers إلى strings؛ قد يحذف flags=0 أو معلومات zero-valued ذات معنى ويجعل schema غير ثابت.
5. build_attribute_string يستخدم elif، فيعرض أول attribute فقط إذا كانت عدة flags مجتمعة.
6. لا تظهر اختبارات automated واضحة في tree الذي تمت مراجعته.
7. --info لا يضمن عدم إنشاء temp raw file في حالة sparse input.

**الحكم:** لا نعتمده كـmetadata-only Windows parser بصورته الحالية. يمكن الاحتفاظ به للمقارنة أو استخدامه على نسخة تجريبية من الملف فقط بعد disk preflight؛ الأفضل تطوير lucky-arch أو بناء AOSP lpdump read-only adapter بعد اختبارات.

## 5. chen3feng/android-backup — incremental shared files

المستودع:
https://github.com/chen3feng/android-backup/tree/dca2c38c60cc17237fa4aaff0cac06475d02ee66

- Commit: dca2c38c60cc17237fa4aaff0cac06475d02ee66
- [README](https://github.com/chen3feng/android-backup/blob/dca2c38c60cc17237fa4aaff0cac06475d02ee66/README.md)
- [backup.py](https://github.com/chen3feng/android-backup/blob/dca2c38c60cc17237fa4aaff0cac06475d02ee66/backup.py)
- [adbsync/adb.py](https://github.com/chen3feng/android-backup/blob/dca2c38c60cc17237fa4aaff0cac06475d02ee66/adbsync/adb.py)
- [adbsync/local_fs.py](https://github.com/chen3feng/android-backup/blob/dca2c38c60cc17237fa4aaff0cac06475d02ee66/adbsync/local_fs.py)

### ما يمكن الاستفادة منه

- scan remote directories باستخدام find للحصول على path/size/mtime.
- incremental sync مع old backup.
- snapshots متعددة الإصدارات باستخدام hardlinks عندما يدعم filesystem ذلك، والنسخ العادي fallback.
- include/exclude patterns.
- ADB commands تبنى كـargument arrays في subprocess.
- Windows workaround لأسماء ملفات Unicode التي قد تتعطل عند adb pull، عبر pull إلى current directory ثم move محلياً.

### ما لا نأخذه كما هو

1. backup.py يكتشف كل الأجهزة المتصلة ثم يمر عليها كلها؛ لا يوجد explicit single target approval.
2. incremental comparison يعتمد size وmtime tolerance ±2 seconds، لا SHA-256؛ يمكن أن يفوّت محتوى تغير مع ثبات الحجم والوقت.
3. عند pull directory غير موجود محلياً، ينفذ adb pull للمجلد كاملاً ثم يزيل excluded files محلياً؛ الملفات المستبعدة قد تنزل مؤقتاً إلى الكمبيوتر قبل حذفها.
4. لا يوجد restore engine إلى الهاتف؛ المشروع ينفذ incremental pull إلى الكمبيوتر.
5. لا توجد automated test suite واضحة في شجرة الملفات التي تمت مراجعتها.
6. لا يظهر timeout/cancellation موحد لأوامر adb.
7. hardlink snapshots تعتمد filesystem؛ fallback إلى copy قد يرفع مساحة النسخ بشكل كبير.
8. أسماء مجلدات backup مستمدة من device name؛ يجب استخدام internal device ID وتجنب تخزين identifiers خامة في أسماء المسارات.
9. إعدادات الأجهزة وinclude paths وexclude rules تحتاج validation/containment قبل أي نقل.

**الحكم:** نقتبس مبادئ scan/sync/hardlink/Windows Unicode workaround، ولا نقتبس backup.py أو adbsync كـengine مباشر. نحتاج engine جديداً فوق File Transfer Adapter المشترك، مع hash verification وexplicit target وpre-transfer exclusions.

## 6. التصميم المقترح بعد إعادة التحقق

### OTA

- Open payload with content-based detection.
- Parse header/manifest only.
- Emit partition inventory: name/size/hash/old size/old hash/needs-source/unsupported operations.
- Mark metadata signature as NotVerified unless a trusted verifier validates it.
- No extraction unless the user selects a local output and confirms disk estimate.

### super.img

- Start with AOSP liblp as correctness oracle.
- Compare lucky-arch list-only against AOSP lpdump JSON on raw and sparse fixtures.
- Add header checksum validation, bounds/resource limits, block-device mapping, slot selection, and tests before adoption.
- Do not use lpunpack_cyth metadata mode on sparse images until it stops creating raw intermediate files.

### Backup

- Device selection is mandatory.
- Exclusions happen before pull.
- Manifest records each file's size/mtime/hash/status.
- Hash verification detects same-size/same-mtime changes.
- Staging + atomic snapshot commit.
- Incremental hardlink only when filesystem supports it; otherwise copy with space estimate.
- No auto-delete, no app-private-data promise, no restore to device until separate preview/approval.

## 7. اختبارات القبول

| المجال | الاختبار | المتوقع |
|---|---|---|
| OTA list | raw payload.bin | partition inventory دون output files |
| OTA ZIP | payload.bin stored/deflated | parsing صحيح؛ deflated temp محدود ومزال |
| malformed payload | manifest length ضخم أو خارج file | رفض قبل allocation كبير |
| delta payload | source images missing | NeedsSource واضح؛ لا full OTA claim |
| unsupported ops | PUFFDIFF/ZUCCHINI/LZ4DIFF | unsupported partition status لا فشل مبهم |
| payload signature | metadata signature present/invalid | NotVerified/Invalid ما لم يتحقق verifier مستقل |
| super raw | valid LP geometry/metadata | تطابق partition inventory مع AOSP |
| super sparse | RAW/FILL/DONTCARE/CRC chunks | list-only دون raw temp file |
| LP corruption | header/tables/geometry checksum mismatch | رفض مع تحديد checksum الفاشل |
| malicious metadata | huge slot count/table size/extent count | bounded parse ورفض آمن |
| multi block devices | extent target_source غير صفر | mapping صحيح أو Unsupported؛ لا قراءة من offset خاطئ |
| multiple slots | metadata slot count >1 | كل slot مميز؛ لا ادعاء active slot من slot 0 |
| backup multi-device | جهازان متصلان | لا يبدأ قبل اختيار جهاز واحد |
| backup exclude | excluded file داخل missing directory | لا ينزل إلى الكمبيوتر مؤقتاً |
| backup same-size change | المحتوى يتغير مع نفس size/mtime | SHA256 يكتشف التغيير |
| backup cancellation | فصل/إلغاء أثناء pull | incomplete state؛ لا snapshot مكتمل |
| Windows Unicode | أسماء عربية/صينية ومسارات طويلة | لا truncation أو overwrite |

## 8. ما لم يتم إثباته

- لم يتم تشغيل payload-dumper-go أو lucky-arch أو lpunpack_cyth أو chen3feng/android-backup.
- لم يتم مقارنة metadata على super.img حقيقي أو corpus متعدد الأجهزة.
- لم يتم اختبار sparse input كبير أو disk pressure على Windows.
- لم يتم نسخ أو استعادة ملفات من هاتف.
- لم يتم اختبار incremental hash behavior أو Windows Unicode workaround.
- لم يتم اعتماد أي مشروع أو dependency للدمج.

**الحالة:** Source Revalidation مكتمل عند SHAs المثبتة. payload-dumper-go مرشح OTA metadata أول، lucky-arch مرشح sparse list-only بعد hardening، lpunpack_cyth غير مناسب حالياً لـmetadata-only sparse، وchen3feng مرجع خوارزمي فقط. لا يوجد Runtime/Hardware evidence أو اعتماد دمج.
