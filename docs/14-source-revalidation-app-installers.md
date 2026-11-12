# Source Revalidation — APK, APKM, XAPK & APKS Installers

> **الغرض:** إعادة فحص مرشحي تثبيت وتحليل الحزم عند SHAs محددة، مع التركيز على تنوع الصيغ، اختيار splits المناسبة، حماية البيانات، تحديد الجهاز، وأمان فك الأرشيف. هذه مراجعة مصدر وليست تشغيل برنامج أو تثبيتاً على هاتف.

**تاريخ المراجعة:** 12 نوفمبر 2026  
**المنصة:** Windows 10/11 وAndroid 7/API 24 فأحدث.  
**الأولوية:** عالية؛ تثبيت صيغ APK المختلفة أحد الاحتياجات الأساسية.  
**الحالة:** Source Revalidation — لا Build/Runtime/Hardware.

## 1. الخلاصة التنفيذية

لا يوجد parser واحد يفسر كل ملف يحمل امتداد .apks أو .xapk؛ يجب فحص محتويات الأرشيف وتحديد schema الفعلي قبل اختيار parser.

| الصيغة/الوظيفة | المرشح المفضل | قرار الاستخدام |
|---|---|---|
| APK metadata/signing/permissions | [apkfile](https://github.com/david-lev/apkfile) | أقوى مرشح parser/metadata متعدد الصيغ بعد path/resource hardening؛ لا نعتمد installer كلياً |
| APKS القياسي الذي يحتوي toc.pb | [Google bundletool](https://github.com/google/bundletool) | المرجع الأساسي لاختيار DeviceSpec وsplits والتثبيت |
| APKS من SAI v1/v2 | [apkfile](https://github.com/david-lev/apkfile) و[PacMan-APK](https://github.com/nuloperrito/pacman-apk) | parser support مرشح؛ يجب تمييز meta.sai_v1/v2 وعدم افتراض toc.pb |
| APKM | apkfile وPacMan-APK | مقارنة parser metadata وsplits؛ نحتاج corpus من ملفات حقيقية متنوعة |
| XAPK + OBB | apkfile | مرشح metadata/OBB؛ يجب التحقق من path containment ووجهة OBB وصلاحيات Android |
| XAPK fallback formats | PacMan-APK و[xapkInstaller](https://github.com/adhu2018/xapkInstaller) | مراجع fallback فقط؛ لا اعتماد كما هي |
| AAB → APKS | bundletool | استخدام bundletool الرسمي؛ keystore signing flow منفصل وبموافقة صريحة |
| APK install session | AdvancedSharpAdbClient أو adapter داخلي | لا نعتمد session lifecycle كما هو قبل معالجة abandon/cleanup/verification في [13](13-source-revalidation-adb-file-core.md) |

**التوصية:** نستخدم apkfile كمرشح أول لفحص الحزم والـmetadata بعد تقوية حدود الأرشيف، ونستخدم bundletool كمرجع/أداة APKS الرسمية. لا نأخذ installer logic من xapkInstaller أو apkfile كما هو دون إعادة تصميم session/device targeting/cleanup.

## 2. Google bundletool — المرجع القياسي لـAPKS

المستودع:
https://github.com/google/bundletool/tree/586a43a450712a1067f3d92cf7574dee68226302

- Commit: 586a43a450712a1067f3d92cf7574dee68226302
- [InstallApksCommand.java](https://github.com/google/bundletool/blob/586a43a450712a1067f3d92cf7574dee68226302/src/main/java/com/android/tools/build/bundletool/commands/InstallApksCommand.java)
- [DeviceSpecUtils.java](https://github.com/google/bundletool/blob/586a43a450712a1067f3d92cf7574dee68226302/src/main/java/com/android/tools/build/bundletool/device/DeviceSpecUtils.java)

### ما ثبت في المصدر

- InstallApksCommand يقرأ BuildApksResult من archive ويجري validate قبل التثبيت.
- يستخرج DeviceSpec من الجهاز المحدد عبر DeviceAnalyzer؛ يحتوي ذلك على ABI وSDK وdensity وlocales وخصائص targeting أخرى.
- يختار APKS المناسبة من TOC وفق DeviceSpec ثم ينفذ install.
- يدعم --device-id، وallow-downgrade وallow-test-only وgrant-runtime-permissions وtimeout.
- القيم الافتراضية للـallow-downgrade وallow-test-only وgrant-runtime-permissions هي false.
- إذا لم يُحدد device id، يستخدم AdbRunner.run على كل الأجهزة المتصلة؛ لذلك يجب أن يمرر تطبيقنا target serial دائماً ولا يترك اختيار الجهاز لـbundletool.
- يستخدم TempDirectory لاستخراج الملفات المحلية المؤقتة ويغلقه بعد انتهاء التنفيذ.
- يدعم Local Testing لمسارات dynamic feature/asset splits؛ هذه وظيفة مختلفة عن التثبيت العادي ولا نفعّلها تلقائياً.

### حدود الاستخدام

- هذا المسار خاص بـbundletool APKS الذي يحتوي toc.pb وBuildApksResult صالحاً؛ لا يعني أن كل ملف .apks هو bundletool APKS.
- SAI APKS قد يحتوي meta.sai_v1.json أو meta.sai_v2.json بدلاً من toc.pb.
- bundletool CLI أداة Java؛ يجب تحديد Java runtime المرفق/المتوافق وإصدار jar وhash، ثم إدارة stdout/stderr/timeout/cancellation من adapter.
- لا نستخدم --device-id اختيارياً؛ هو إلزامي في منتجنا.
- لا نفعل allow-test-only أو allow-downgrade أو grant-runtime-permissions افتراضياً.
- لا نستخدم AAB signing أو keystore من دون شاشة مستقلة توضح أثر التوقيع ومن يملك المفتاح.

**الحكم:** مرجع رسمي أساسي لملفات bundletool APKS وAAB conversion. نحتاج process adapter آمن أو integration معزول، مع serial صريح وtimeout ومخرجات منظمة.

## 3. apkfile — أقوى مرشح لفحص الحزم والـmetadata

المستودع:
https://github.com/david-lev/apkfile/tree/4b910abae705106cde80a44d607917d038f0c36e

- Commit: 4b910abae705106cde80a44d607917d038f0c36e
- Version المعلنة في pyproject: 1.4.2
- Python: 3.10 فأحدث.
- Dependencies: androguard وcryptography وloguru.
- CI يختبر Python 3.10 و3.14 على Windows، و3.10–3.14 على Linux، و3.10/3.14 على macOS.
- [README](https://github.com/david-lev/apkfile/blob/4b910abae705106cde80a44d607917d038f0c36e/README.md)
- [ApkFile](https://github.com/david-lev/apkfile/blob/4b910abae705106cde80a44d607917d038f0c36e/apkfile/_apk.py)
- [Bundle readers](https://github.com/david-lev/apkfile/blob/4b910abae705106cde80a44d607917d038f0c36e/apkfile/_bundle.py)
- [Install backend](https://github.com/david-lev/apkfile/blob/4b910abae705106cde80a44d607917d038f0c36e/apkfile/install.py)
- [Signing metadata](https://github.com/david-lev/apkfile/blob/4b910abae705106cde80a44d607917d038f0c36e/apkfile/_signing.py)
- [Bundle tests](https://github.com/david-lev/apkfile/blob/4b910abae705106cde80a44d607917d038f0c36e/tests/test_bundle.py)
- [Install tests](https://github.com/david-lev/apkfile/blob/4b910abae705106cde80a44d607917d038f0c36e/tests/test_install.py)

### ما يدعمه المصدر

- APK metadata: package/version/minSdk/targetSdk، permissions، features، libraries، launcher activity، labels، icons، ABIs، density، screen support.
- Signing metadata: schemes V1/V2/V3/V3.1 وشهادات SHA256 وخصائص الشهادة.
- Security metadata: permissions، exported components، provider permissions، deep links، cleartext/network flags.
- Bundle formats: APKM وXAPK وAPKS وAPKV؛ يقرأ base/splits lazily من archive bytes.
- XAPK OBB expansion files.
- APK diff وSHA-256 وsize breakdown وDEX metadata.
- Install compatibility: min SDK، ABI، locale/density split selection.
- Locale logic يتعامل مع persist.sys.locales وpersist.sys.locale وSettings.System وlegacy locale aliases.
- Tests تشمل APKS v1/v2، APKM، XAPK، OBB، broken splits، multi-device behavior، ABI/locale/density selection وcleanup عند عدم التوافق.

### فجوات مهمة في Installer

1. إذا لم يُمرر device_id، يستهدف كل الأجهزة المتصلة بالتوازي. منتجنا يفرض target serial دائماً.
2. subprocess.run لا يحدد timeout أو cancellation في المسار الذي تمت مراجعته.
3. install backend ينشئ remote temp directory عبر mktemp ويزيله في finally، وهذا جيد كنمط cleanup، لكنه لا ينفذ install-abandon واضحاً إذا فشلت كتابة split بعد install-create وقبل install-commit.
4. لا يوجد post-install query مستقل يثبت package/version بعد commit في المسار الذي تمت مراجعته.
5. أسماء split من archive تُستخدم لإنشاء أسماء ملفات داخل temp؛ يجب إثبات path containment وعدم السماح بالـtraversal أو collision.
6. public extract() في APK/Bundle يستخدم zipfile.extractall؛ لا نعرض هذا المسار لأرشيف غير موثوق قبل hardening واختبارات traversal/symlink/resource exhaustion.
7. SigningInfo يعرض schemes والشهادات المسجلة من parser؛ لا نعتبر مجرد استخراج certificate metadata إثباتاً مستقلاً لصحة التوقيع/سلامة كل APK bytes. نحتاج apksigner verify أو verifier موثوقاً منفصلاً، ومقارنة signers بين base/splits.
8. install helper يرفع OBB بعد نجاح تثبيت APK؛ OBB ليس جزءاً من install session، وقد يفشل النقل بعد نجاح التثبيت. يجب أن يعرض التقرير حالة APK وOBB منفصلة ولا يعلن العملية كلها Succeeded إذا فشل OBB.
9. pm install-commit لا يكفي لإثبات نجاح الحالة النهائية؛ يجب query package manager بعد commit.

### ما يمكن إعادة استخدامه

- metadata parser/data model كمرشح sidecar/CLI بعد تقييم تكلفة Python runtime، أو تحويل contracts مع الحفاظ على fixtures.
- split selection logic كمرجع واختبارات regression، خصوصاً تعدد locales وlegacy aliases وdensity fallback.
- parser fixtures وAPKM/XAPK/APKS sample generators.
- signing metadata وsecurity report كطبقة تحليل منفصلة عن قرار التثبيت.

**الحكم:** أفضل مرشح حالي لفحص APK/bundle metadata، لكن Python dependency/runtime وarchive hardening وinstall lifecycle تحتاج PoC. لا نقرر تضمين Python runtime قبل قياس startup/package size وoffline packaging.

## 4. PacMan-APK — مرجع Windows UI وقراءة الصيغ

المستودع:
https://github.com/nuloperrito/pacman-apk/tree/87af1d04541297fdf272813567928a2a6de6e756

- Commit: 87af1d04541297fdf272813567928a2a6de6e756
- Flutter/Dart، Flutter >=3.44.4، Dart >=3.12.2.
- [README](https://github.com/nuloperrito/pacman-apk/blob/87af1d04541297fdf272813567928a2a6de6e756/README.md)
- [APKM reader](https://github.com/nuloperrito/pacman-apk/blob/87af1d04541297fdf272813567928a2a6de6e756/lib/parser/reader_apkm.dart)
- [APKS reader](https://github.com/nuloperrito/pacman-apk/blob/87af1d04541297fdf272813567928a2a6de6e756/lib/parser/reader_apks.dart)
- [XAPK reader](https://github.com/nuloperrito/pacman-apk/blob/87af1d04541297fdf272813567928a2a6de6e756/lib/parser/reader_xapk.dart)
- [ADB/WSA utilities](https://github.com/nuloperrito/pacman-apk/blob/87af1d04541297fdf272813567928a2a6de6e756/lib/utils/adbwsa_utils.dart)

### ما يقدمه

- تطبيق Windows مبني بـFlutter/Fluent UI.
- APK/APKM/APKS/XAPK.
- يدعم WSA وأجهزة ADB USB، ويعرض اختيار الجهاز عند تعدد الأجهزة بحسب README.
- APKS reader يفهم toc.pb ويختار variants حسب ABI، مع fallback لمعالجة ZIP alignment.
- APKM reader يعتمد info.json.
- XAPK reader يعتمد manifest.json، ولديه fallback عندما لا يوجد manifest.
- يتعامل مع XAPK expansion/OBB files.
- يستخدم isolates لمعالجة الحزم بعيداً عن UI.

### ما يمنع اعتماده مباشرة

- الوصف والبنية موجهان أساساً إلى WSA؛ نحتاج التحقق من دعم أجهزة Android العامة على USB/Wireless.
- بعض مسارات APKM/XAPK تبني file path باستخدام اسم entry من archive؛ لم يظهر containment guard واضح في المواضع التي راجعناها. يجب اختبار Zip Slip وabsolute path وduplicate basename.
- فك archive/inner APKs إلى byte arrays قد يستهلك ذاكرة كبيرة؛ لا توجد حدود decompression/entry-count واضحة في المواضع التي تمت مراجعتها.
- APKS reader لديه ABI scoring مخصص يعطي x86_64 أفضلية في سيناريو WSA؛ لا ننقل هذا الترتيب كقاعدة عامة لكل هاتف.
- ADB utilities تعرض shell(command) كنص حر؛ لا نستخدمها في service contract.
- مهلة Future.timeout لا تعني بالضرورة قتل عملية adb.exe؛ يجب التحقق من process termination.
- يوجد embedded adb.exe؛ لا ننسخ binary من المشروع دون تثبيت مصدره ونسخته وhash.

**الحكم:** مرجع مفيد لـWindows UI/WSA workflows وformat parsing edge cases، وليس أساس Host Core أو parser معتمداً قبل security/resource tests.

## 5. xapkInstaller — مرجع fallback لا يعتمد كما هو

المستودع:
https://github.com/adhu2018/xapkInstaller/tree/a88e05386a0fc9751ef7637ad437659bfdd60a43

- Commit: a88e05386a0fc9751ef7637ad437659bfdd60a43
- [README](https://github.com/adhu2018/xapkInstaller/blob/a88e05386a0fc9751ef7637ad437659bfdd60a43/README.md)
- [xapkInstaller.py](https://github.com/adhu2018/xapkInstaller/blob/a88e05386a0fc9751ef7637ad437659bfdd60a43/xapkInstaller.py)
- [Windows test/build workflow](https://github.com/adhu2018/xapkInstaller/blob/a88e05386a0fc9751ef7637ad437659bfdd60a43/.github/workflows/test.yml)

### ما يقدمه

- Python single-file Windows utility يدعم APK/APKM/APKS/XAPK وAAB workflows.
- يقرأ device ABI/ABI list/density/locale/SDK.
- يتعرف على APKM info.json وXAPK manifest.json وbundletool toc.pb وSAI metadata.
- يحتوي install session helpers مع install-abandon عند فشل install-write أو install-commit.
- لديه Windows Actions workflow يشغل lint ويصنع executable؛ لم نشغل workflow بأنفسنا.

### مشاكل لا ننسخها

- تثبيت APK وAPKM يبدأ افتراضياً بـ-rtd؛ أي replace + test-only + downgrade، وهي صلاحيات أوسع من default الآمن المطلوب.
- check() يجمع كل الأجهزة؛ main() يمر عليها. مسار APK/APKM/APKS يرجع من داخل الحلقة عند أول جهاز، بينما XAPK قد يكرر العملية لكل الأجهزة. هذا السلوك غير متسق ولا يطابق اختيار جهاز صريح.
- _push() يستخدم اسم الملف الأصلي داخل /data/local/tmp/؛ collision محتمل لملفات لها basename نفسه.
- install session لا يملك finally موحداً يغطي كل أخطاء push/write/cleanup.
- install_apk يعيد المحاولة، وقد يصل إلى uninstall -k مع backup APK عند بعض حالات الفشل؛ لا ننقل هذا fallback التلقائي، وأي uninstall يحتاج موافقة مستقلة.
- os.chdir() يغير working directory للعملية كلها؛ غير مناسب لخدمة متعددة العمليات.
- XAPK extraction يستخدم shutil.unpack_archive؛ نحتاج حدود حجم/عدد entries ومسار containment.
- APKM/APKS extraction يستخدم ZipFile.extract؛ لا نعتبر ذلك كافياً وحده ضد archive bombs أو duplicate/path cases.

**الحكم:** مرجع حالات fallback/schema وsession abandon، لا نستخدم installer logic أو defaults كما هي.

## 6. توحيد format detection وInstaller Pipeline

### 6.1 اكتشاف الصيغة

الامتداد مجرد hint. بعد فحص file signature وZIP central directory، نفحص markers:

| Marker داخل archive | الصيغة المحتملة | المعالج |
|---|---|---|
| Android APK signing/ZIP + AndroidManifest.xml | APK | APK parser |
| info.json مع base/split APKs | APKM | APKM provider |
| manifest.json مع splitApks/expansions | XAPK | XAPK provider |
| toc.pb + splits/ | Bundletool APKS | bundletool provider |
| meta.sai_v1.json أو meta.sai_v2.json | SAI APKS | SAI provider |
| classes.dex / manifest داخل AAB modules وBundleConfig.pb | AAB | bundletool build-apks |
| ملفات غير معروفة أو markers متعارضة | Unknown/Conflicting | لا تثبيت؛ تقرير unsupported |

لا نغير امتداد الملف ولا نستنتج الصيغة من الاسم فقط.

### 6.2 Pipeline

1. Open as untrusted archive، وفحص magic/header وcentral directory.
2. Enforce limits: max archive size، max entry count، max uncompressed total، max single entry، compression ratio، nested archive depth، time/memory budget.
3. Reject absolute paths، .. traversal، drive prefixes، ADS/alternate data streams، symlinks/hardlinks غير المتوقعة، duplicate normalized paths، case-fold collisions على Windows.
4. Parse metadata دون extraction كامل قدر الإمكان.
5. Identify base APK، splits، ABI/density/language/config splits، OBBs.
6. Parse package/version/minSdk/targetSdk/required features/native ABIs.
7. Verify APK signatures cryptographically باستخدام apksigner أو verifier معتمد؛ compare package identity وsigner set لكل base/split.
8. Build DeviceSpec من target serial المحدد: SDK، ABI list، density، locale list، feature/GL requirements عند الحاجة.
9. Resolve split selection مع explanation لكل included/excluded split.
10. Preview: package، version old→new، size، splits، OBB destinations، flags، warnings، target serial.
11. Require approval للـdevice write؛ لا downgrade/testOnly/grant permissions افتراضياً.
12. Stage remote files/session paths uniquely per operation؛ track session id and remote artifacts.
13. Install via install session، abandon on failure/cancel، cleanup with independent bounded token.
14. Post-install verify package/version/signer where possible.
15. OBB copy is a separate sub-operation with its own verify/status.
16. Report Success / Partial / Failed / OutcomeUnknown / NeedsReconciliation؛ لا نختزل الحالة إلى exit code.

## 7. Default install policy

| الخيار | القيمة الافتراضية | متى يسمح؟ |
|---|---|---|
| Target device | required explicit serial | لا multi-device implicit |
| Replace existing | no extra behavior beyond normal package update semantics | user preview |
| Downgrade | false | explicit user selection and device/package compatibility |
| Test-only | false | explicit user selection |
| Grant runtime permissions | false | explicit user selection; explain scope |
| Install for user | current/default user | explicit user selection |
| Launch after install | false | separate action after verified install |
| OBB copy | only if bundle declares matching OBB | show destination and verify separately |
| Uninstall before retry | never automatic | separate explicit approval; explain app data impact |
| Device cleanup | operation-owned paths only | bounded cleanup/reconciliation |

## 8. Corpus and acceptance tests

### 8.1 Required sample families

- APK: v1/v2/v3/v3.1 signatures، debug/release، invalid/truncated، duplicate signing blocks، native ABIs.
- Split APK: base + ABI + density + language + feature splits، split missing، duplicate split id، package mismatch، signer mismatch.
- APKM: valid info.json، unknown fields، absent base، multiple base candidates، malformed JSON، nested APKs.
- XAPK: APKPure manifest، alternative/fallback manifestless package، expansion files، missing OBB، mismatched package/version.
- APKS: bundletool toc.pb، SAI v1، SAI v2، invalid/missing TOC، ZIP alignment edge case.
- AAB: valid bundle and missing/invalid keystore scenario; no signing without user approval.
- Malicious archives: path traversal, absolute path, duplicate normalized names, ZIP bomb, huge entry count, nested archive, malformed central directory, symlink entry.

### 8.2 Device matrix

- Android API 24, 26, 28, 29, 30, 31, 33, 34, 35+.
- ABI arm64-v8a, armeabi-v7a, x86, x86_64 where test devices/emulators support them.
- Multiple locales including modern/legacy language code aliases.
- Density buckets and device-reported density fallback.
- USB and Wireless ADB, one device and multiple devices.
- Existing package: same version, newer version, older version, different signer, testOnly.

### 8.3 Assertions

- no implicit multi-device install.
- no install when split identity/signature/target mismatch.
- no -d, -t, -g unless user chose them.
- session abandon after failure/cancellation.
- remote staging path unique per operation.
- local archive extraction stays within operation temp root.
- no resource exhaustion on malicious archive fixtures.
- post-install package/version read-back.
- partial OBB failure is reported separately.
- no automatic uninstall/data removal fallback.

## 9. قرار إعادة الاستخدام

1. **bundletool:** استخدامه كمرجع/adapter رسمي لـbundletool APKS وAAB؛ لا تعمم APKS marker على SAI.
2. **apkfile:** أقوى مرشح parser metadata متعدد الصيغ، لكن يحتاج path/resource hardening، verifier منفصل، وقرار Python sidecar مقابل نقل contracts.
3. **PacMan-APK:** مرجع UI/WSA وformat fallback وedge cases، وليس أساس عام.
4. **xapkInstaller:** مرجع session-abandon/fallback schemas فقط؛ لا نستخدم defaults أو multi-device logic.
5. **AdvancedSharpAdbClient:** طبقة ADB candidate؛ install session gaps موثقة في [13](13-source-revalidation-adb-file-core.md).

## 10. ما لم يتم إثباته

- لم نشغل أي parser أو installer في هذه الجولة.
- لم نختبر أي APK/APKM/APKS/XAPK/AAB على جهاز Android.
- لم نتحقق من سلامة التوقيع cryptographically باستخدام apksigner على corpus.
- لم نختبر ZIP bombs/path traversal أو حدود الذاكرة.
- لم نختبر OBB push/restore على Android API 24+.
- لم نقرر تضمين Python/Flutter/Java runtime أو اختيار Host Core.
- لم نعتمد أي كود أو dependency للدمج.

**الحالة:** Source Revalidation مكتمل للمصادر المحددة عند commits أعلاه. apkfile هو مرشح parser أولي، bundletool هو المرجع APKS القياسي، وInstaller Pipeline والسياسات الافتراضية موثقة؛ لا يوجد Runtime/Hardware evidence أو اعتماد دمج.
