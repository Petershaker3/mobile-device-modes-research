# Android Storage & Partition Information Research

> **الغرض:** قراءة معلومات التخزين والأقسام على جهاز Android أو من ملفات firmware محلية، مع التمييز بين المساحة المستخدمة وpartition layout وdynamic partitions. هذه الوثيقة لا تتضمن قراءة raw partition من الهاتف ولا كتابة/تعديل الأقسام.

**آخر مراجعة:** 12 نوفمبر 2026  
**المنصة:** Windows 10/11 وAndroid 7.0 (API 24) فأحدث.  
**القيود:** لا بحث في Qualcomm أو MediaTek أو بروتوكولاتهما أو خرائط أقسامهما؛ لا root افتراضياً؛ لا تفليش أو erase أو format أو تعديل metadata.

## 1. الخلاصة التنفيذية

معلومات التخزين ليست رقماً واحداً. نحتاج إلى فصل أربع طبقات:

1. **Filesystem Usage:** المساحة الكلية والمستخدمة والمتاحة لكل mount point، من df أو واجهة مناسبة.
2. **Mounted Volumes:** نقاط التركيب وأنواع filesystem والـoptions والأقراص القابلة للإزالة/المعتمدة.
3. **Physical Partition Inventory:** الأقسام الفيزيائية التي يمكن اكتشافها بأمان، وأحجامها ووجود slots عند دعم الجهاز.
4. **Logical/Dynamic Partition Metadata:** محتوى super.img ومجموعات الأقسام والـextents والـslots من ملفات محلية أو من مصدر يمكن قراءته بصلاحية مصرح بها.

**القرار:** لا نستخدم df لإنتاج partition map، ولا نستخدم lpdump لإنتاج filesystem free space. كل مصدر يجيب عن سؤال مختلف.

**أفضل مرجع رسمي:** AOSP partition_tools وliblp، خصوصاً lpdump JSON وlpunpack.  
**أفضل مرشح Windows أولي:** [lpunpack_cyth](https://github.com/ahsanihlwn/lpunpack_cyth) بسبب وجود Windows executable وخيار --info JSON، مع قيود عدم دعم slot selection وعدم وجود اختبارات ظاهرة.  
**أفضل مرشح Go لتوسعة داخلية:** [lucky-arch](https://github.com/soe1hom-arch/lucky-arch) لقائمة partitions دون extraction ودعم sparse super images، لكنه لا يعرض Windows CI أو اختبارات واضحة في الملفات التي راجعناها.  
**مرشح extraction مع Windows CI:** [lpunpack-go](https://github.com/soe1hom-arch/lpunpack-go)، لكنه raw-only ولا يقدم list-only CLI حالياً.

## 2. التمييز بين مستويات التخزين

| المستوى | ما الذي يصفه؟ | مصدر مناسب |
|---|---|---|
| User-visible storage | مساحة الصور والتنزيلات والملفات المشتركة | /storage/emulated/0 وMTP/WPD وADB file provider |
| Mounted filesystem | المساحة المتاحة/المستخدمة على mount point | df، /proc/mounts، statfs |
| Physical partition | قسم في GPT أو block device | Fastboot read-only getvar، /proc/partitions، /dev/block/by-name عند السماح |
| Logical partition | قسم افتراضي مكوّن من extents داخل super | lpdump/lpunpack من super.img |
| Dynamic partition group | مجموعة تحدد سقف الحجم لأقسام logical | LP metadata |
| A/B slot metadata | نسخة metadata أو suffixes للـslots | lpdump slot metadata وخصائص boot |
| Virtual A/B snapshot | حالة تحديث مؤقتة وCOW snapshot | أدوات/بيانات snapshot الموثقة؛ لا تعاملها كقسم دائم |

### 2.1 Filesystem Usage

أوامر قراءة مبدئية:

~~~powershell
adb -s <serial> shell df -k
adb -s <serial> shell mount
adb -s <serial> shell cat /proc/mounts
adb -s <serial> shell cat /proc/partitions
adb -s <serial> shell sm list-disks
adb -s <serial> shell sm list-volumes all
adb -s <serial> shell dumpsys diskstats
~~~

كل أمر best-effort؛ قد يكون غير متاح أو يختلف تنسيقه حسب Android/OEM. لا نعرض فشل أمر واحد كفشل التقرير كله.

**مهم:** /storage/emulated/0 غالباً مساحة emulated مبنية فوق data، وليست partition مستقلة. كما أن df يعرض mounted filesystems، وليس كل block partitions.

### 2.2 Physical Partition Inventory

مصادر ممكنة، حسب الوضع والصلاحيات:

- Fastboot read-only variables مثل current-slot وhas-slot وpartition-size:name وis-logical:name عند دعمها.
- /proc/partitions عند إمكانية القراءة.
- /dev/block/by-name عند إمكانية الوصول؛ لا نحاول رفع الصلاحيات عند PermissionDenied.
- خصائص boot مثل ro.boot.slot_suffix وro.boot.super_partition كإشارات مساعدة وليست خريطة كاملة.

لا ننفذ fastboot getvar all افتراضياً؛ إذا احتجنا قراءة مجموعة متغيرات نستخدم allowlist، وننقح serials أو identifiers من التقرير.

لا نستخدم أوامر الكتابة مثل flash/erase/format/set_active/unlock أو أي أمر OEM في محور القراءة.

### 2.3 Dynamic Partitions

AOSP يشرح أن super partition تحتوي metadata تسجل أسماء logical partitions ونطاقات blocks الخاصة بها. عند الإقلاع يقرأ النظام metadata ويُنشئ virtual block devices للأقسام المنطقية. Android 10+ قدم dynamic partitions للأجهزة الجديدة، مع وجود أجهزة أقدم تم retrofit لها.

مصدر AOSP:
https://source.android.com/docs/core/ota/dynamic_partitions/implement

نحتاج من metadata:
- Metadata version وslot count.
- Partition names/attributes/size.
- Groups وmaximum sizes.
- Extents وblock device mapping.
- Slot-suffixed flags.
- Header flags مثل virtual A/B عند وجودها.
- Checksums وصحة metadata.

لا نفترض أن كل جهاز Android 10+ يملك super partition، ولا أن كل جهاز Android 9 أو أقدم لا يملك dynamic partitions؛ اكتشف من البيانات الفعلية.

### 2.4 A/B وVirtual A/B

- A/B devices قد تملك slot metadata متعددة، وقد تختلف طريقة suffixing للأقسام.
- في dynamic partition devices، super نفسه قد يحتوي metadata slots؛ لا تخلط metadata slot number مع active boot slot.
- Virtual A/B يستخدم snapshots وCOW أثناء التحديث؛ لا تعرض snapshot المؤقت على أنه partition دائم.
- أسماء الأقسام ووجود suffix _a/_b تختلف حسب layout؛ لا نضيف suffix من عندنا.

مرجع AOSP:
https://source.android.com/docs/core/ota

## 3. أوامر القراءة وحدودها

| المصدر | ما يعرضه | ما لا يثبته |
|---|---|---|
| df -k | mounted filesystem size/used/available | partition map أو الحجم الخام للقرص |
| mount /proc/mounts | mount points وfilesystem/options | كل الأقسام غير المركبة |
| /proc/partitions | block devices/partition sizes عند السماح | logical partition metadata بالكامل |
| /dev/block/by-name | aliases لأقسام معروفة عند السماح | أن كل alias قابل للقراءة أو أن الاسم قياسي |
| fastboot getvar partition-size | حجم partition إذا كان bootloader يدعم المتغير | الحجم المتاح داخل filesystem |
| fastboot getvar is-logical | هل الاسم logical إذا دعم الجهاز | جميع partitions أو groups |
| lpdump super.img | metadata logical partitions/groups/extents/slots | free space داخل filesystem |
| lpunpack super.img | استخراج صور logical partitions محلياً | توافق الصور مع جهاز معين أو صلاحيتها للتفليش |
| avbtool info_image | AVB metadata من vbmeta/image | صحة كل الملفات المرتبطة أو موثوقية المفتاح دون trust anchor |

كل أمر له status مستقل: Success / Unsupported / PermissionDenied / Timeout / ParseError / DeviceDisconnected.

## 4. تدقيق مشاريع GitHub/AOSP الجاهزة

### 4.1 AOSP system/extras/partition_tools — المرجع الأساسي

المصدر الرسمي:
https://android.googlesource.com/platform/system/extras/+/master/partition_tools/

الملفات الرئيسية:
- lpdump.cc
- lpdump_host.cc
- lpdump_target.cc
- lpunpack.cc
- lpmake.cc
- lpflash.cc
- README.md

ما يهمنا:
- lpdump يعرض logical partition metadata، ويدعم JSON في المصدر الحالي، ويدعم تحديد slot أو عرض كل slots.
- lpdump يمكنه قراءة image محلية، كما يملك مساراً لقراءة block device عندما تكون الصلاحيات متاحة.
- lpunpack يقرأ super image محلياً ويستخرج صور logical partitions.
- liblp هو المرجع الأساسي لبنية metadata.

**ما لا ندمجه في المنتج:**
- lpmake لإنشاء/إعادة بناء super images.
- lpflash لكتابة صورة إلى block device.
- lpadd لتعديل super image.
هذه الأدوات ليست مطلوبة لقراءة البيانات، ولن تكون ضمن واجهة المنتج.

### 4.2 itsNileshHere/android-lptools — AOSP port

المستودع: https://github.com/itsNileshHere/android-lptools  
الفرع الذي تمت مراجعته: android-14.

- يضم lpdump/lpunpack/lpmake/lpflash/lpadd.
- README يعلن prebuilt releases.
- bin/ داخل الفرع يحتوي executables بلا امتداد، وworkflow release يعمل على Ubuntu ويؤرشف bin/*؛ لم يظهر أن binaries المرفقة Windows builds.
- lpdump.cc يدعم --json وslot selection وقراءة FILE أو DEVICE.
- المصدر مبني على AOSP مع تعديلات build.

**الحكم:** مرجع مفيد لفهم AOSP toolchain، لكن binaries الموجودة في الفرع لا نعتمدها كحزم Windows جاهزة. نحتاج Windows build موثقاً قبل استخدامها.

### 4.3 ahsanihlwn/lpunpack_cyth — Windows metadata inspector مرشح

المستودع: https://github.com/ahsanihlwn/lpunpack_cyth

- README يعلن Windows executable.
- CLI يدعم --info و--format json، ويمكنه عرض metadata دون استخراج الصور.
- parser مكتوب Cython/Python، ويحتوي sparse image support.
- JSON يتضمن metadata version/size/slot count/partitions/groups/block devices/layout.
- README يذكر أن slot A/B index extraction غير مطبق.
- source يحتوي TODO حول header flag parsing، ولا تظهر suite اختبارات واضحة في شجرة الملفات التي تمت مراجعتها.

**الحكم:** مرشح PoC سريع لـmetadata-only inspection على Windows بعد اختبارات ملفات متعددة؛ ليس parser معتمداً حالياً، ولا نستخدم extraction أو slot selection قبل إغلاق الفجوات.

### 4.4 soe1hom-arch/lpunpack-go — Go extractor

المستودع: https://github.com/soe1hom-arch/lpunpack-go

- Go 1.22+.
- CI يبني/يفحص المشروع على Ubuntu، ويصدر Windows amd64/arm64 binaries عند tags.
- يدعم LP metadata v10.0–v10.2، ويقرأ geometry/metadata checksums.
- README يوضح أنه يتطلب تحويل sparse image إلى raw باستخدام simg2img أولاً.
- CLI الحالي يستخرج كل partitions ولا يقدم list-only mode.
- شجرة الملفات لا تحتوي tests واضحة؛ workflow ينفذ go build وgo vet فقط.

**الحكم:** مرشح لتوسعة Go داخلياً بعد إضافة metadata-only API واختبارات. لا نعتمد extraction كما هو قبل اختبار حدود metadata والـextents على corpus.

### 4.5 soe1hom-arch/lucky-arch — sparse-aware list/extract

المستودع: https://github.com/soe1hom-arch/lucky-arch

- Go project يجمع sparse image reader وLP metadata parser.
- يدعم raw وAndroid sparse super.img دون كتابة raw intermediate كامل.
- يحتوي --list mode يعرض partition names/sizes دون extraction.
- source يعلن دعم LP metadata v10.0–v10.2.
- لم تظهر اختبارات واضحة في شجرة الملفات التي تمت مراجعتها، ولا توجد Windows release workflow موثقة في README.

**الحكم:** مرشح بحثي قوي لأن list-only + sparse support يطابق احتياجنا؛ يجب إضافة tests وفحص cross-compilation إلى Windows قبل أي اعتماد.

### 4.6 Rprop/aosp15_partition_tools

المستودع: https://github.com/Rprop/aosp15_partition_tools

- يقدم prebuilt AOSP partition tools.
- README يذكر Windows x86 وLinux x86_64؛ لم يظهر Windows x64 binary في القائمة.
- مناسب كمرجع لحزم الأدوات المسبقة، لكن لا نعتبره خيار Windows x64 جاهزاً دون اختبار معماري.

## 5. قرار إعادة الاستخدام

| الاحتياج | الخيار | قرار |
|---|---|---|
| Metadata JSON لـsuper.img | AOSP lpdump --json | المرجع المعياري؛ نحتاج Windows build أو adapter |
| Windows metadata PoC | lpunpack_cyth --info --format json | اختباره على corpus، لا اعتماد slot support |
| Go parser يدعم sparse/list | lucky-arch | أفضل مرشح لتطوير read-only metadata mode بعد إضافة tests |
| Go extraction | lpunpack-go | يمكن اقتباس parser بعد فصل metadata API وإضافة list-only |
| استخراج AOSP partitions | lpunpack | offline host-only بعد تأكيد المساحة والوجهة |
| إنشاء/تعديل/كتابة partitions | lpmake/lpadd/lpflash | خارج نطاق المنتج ولا تُعرض للمستخدم |

**الاقتراح العملي:** نبدأ من AOSP liblp/lpdump كمرجع correctness، ونقارن lucky-arch وlpunpack_cyth على ملفات اختبار. لا نكتب LP metadata parser من الصفر إلا إذا فشل كل مرشح في متطلبات Windows أو الاختبار.

## 6. نموذج Storage & Partition Report

| الحقل | الوصف |
|---|---|
| device_id | الجهاز المحدد صراحةً |
| collected_at | وقت القراءة |
| android_api / build_fingerprint | سياق نسخة النظام |
| storage_volumes[] | mount point/type/total/used/available |
| physical_partitions[] | name/size/slot/availability/source |
| logical_partitions[] | name/size/attributes/group/extents |
| partition_groups[] | name/maximum_size/partition_members |
| super_metadata | version/slot_count/header_flags/checksum_status |
| active_slot | قيمة من مصدر موثق أو Unknown |
| source | ADB / Fastboot / local super.img / OTA payload |
| status | Success/Partial/Unsupported/PermissionDenied/ParseError |
| warnings[] | نقص الصلاحية أو metadata غير متاحة أو عدم تطابق |

لا نملأ physical_partitions من أسماء logical partitions ولا نستنتج مساحة filesystem من حجم partition.

## 7. السلامة وحدود التنفيذ

- جميع أوامر الجهاز read-only allowlist.
- لا root أو su أو adb disable-verity.
- لا adb pull لملفات /dev/block.
- لا fastboot fetch أو flash أو erase أو format أو set_active.
- لا lpflash أو lpmake أو lpadd في المنتج.
- لا extraction تلقائي عند فحص super.img.
- extraction المحلي إلى مجلد جديد فقط بعد اختيار المستخدم، مع تقدير مساحة وlimits.
- لا نفك sparse image إلى raw كامل إذا كان list-only أو metadata inspection يكفي.
- لا نركب filesystem image تلقائياً؛ أي mount لاحق يحتاج مساراً منفصلاً ومعزولاً.
- جميع ملفات الصور والـmetadata مدخلات غير موثوقة؛ تحقق من الأحجام والـoffsets والـextents قبل القراءة.

## 8. اختبارات القبول البحثية

| الاختبار | المتوقع |
|---|---|
| لا يوجد super.img | Unsupported/NotFound |
| raw super image صالح | metadata/partitions/groups صحيحة |
| sparse super image | كشف sparse؛ list-only دون raw intermediate إذا كان parser يدعم ذلك |
| metadata slot count أكثر من 1 | عرض slots بتمييز صحيح؛ لا افتراض slot active |
| metadata version غير مدعوم | Unsupported مع version |
| checksum mismatch | Corrupt/IntegrityError |
| extent خارج حجم الملف | رفض parsing/extraction |
| partition table count ضخم/غير منطقي | حدود تمنع memory exhaustion |
| super.img كبير | metadata inspection لا يقرأ/يكتب الصور كاملة |
| df output غير قياسي | ParseError للحقل، لا فشل التقرير كله |
| PermissionDenied لـ/dev/block | حالة واضحة؛ لا محاولة root |
| Android 7–9 | لا افتراض وجود dynamic partitions |
| Android 10+ | اكتشاف dynamic partition من metadata لا من API فقط |
| virtual A/B snapshot | لا عرض COW المؤقت كقسم دائم |
| local extraction | لا كتابة إلى الجهاز ولا overwrite صامت |

## 9. المصادر الرسمية

- [AOSP Dynamic Partitions](https://source.android.com/docs/core/ota/dynamic_partitions/implement)
- [AOSP Dynamic Partition Tools](https://android.googlesource.com/platform/system/extras/+/master/partition_tools/)
- [AOSP lpdump/lpunpack README](https://android.googlesource.com/platform/system/extras/+/master/partition_tools/README.md)
- [AOSP OTA updates](https://source.android.com/docs/core/ota)
- [AOSP legacy A/B dynamic partitions](https://source.android.com/docs/core/ota/dynamic_partitions/ab_legacy)

## 10. ما لم يتم إثباته

- لم يتم تشغيل lpdump أو lpunpack أو أي مرشح Windows في هذه الجولة.
- لم يتم اختبار super.img حقيقي من عدة أجهزة.
- لم يتم قراءة partition map من هاتف حقيقي.
- لم يتم اختبار صلاحيات /dev/block أو Fastboot variables على Android 7+.
- لم يتم استخراج أي صورة partition.
- لم يتم تنفيذ أي كتابة أو تعديل على جهاز.

**الحالة:** البحث ومراجعة المصدر تمت. AOSP liblp/lpdump هو المرجع، وlucky-arch وlpunpack_cyth مرشحان لتجربة metadata-only على Windows بعد بناء corpus واختبارات. لا يوجد اعتماد دمج أو اختبار Hardware.
