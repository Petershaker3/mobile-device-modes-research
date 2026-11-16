# TODO: تطبيق سطح المكتب على الهاتف الفعلي

## قراءة معلومات الجهاز عبر الأوضاع

- [x] بحث ADB وRecovery sideload وFastboot/Fastbootd وWPD/MTP ومسارات Samsung/Xiaomi/Huawei، وتثبيت حدود كل وضع ومصادره في [24](24-DEVICE-MODE-READ-INFO.md).
- [x] بدء توسيع قراءة Android عبر ADB بقائمة خصائص ثابتة، مع إبقاء serial/IMEI/IMSI وعناوين MAC خارج التقرير، وجمع الخصائص في جلسة shell واحدة لتقليل فتح الجلسات.
- [x] تحقق أولي: اختبارات parser وخدمة القراءة نجحت 4/4؛ بناء WPF Release نجح بـ0 تحذيرات و0 أخطاء. الهاتف المحدد عبر ADB أعاد سجلات الخصائص الخمسة عشر؛ سُجلت أسماء الحقول وحالاتها فقط دون القيم أو serial.
- [ ] تحقق بصري لتمرير لوحة المعلومات والتأكد من أن كل الحقول ظاهرة؛ أداة Computer Use أعادت `Native apps: ... native pipe is unavailable` مرتين، لذلك لم يُدّعَ فحص الشاشة أو النقر.
- [x] أضف Fastboot executable discovery وقراءة getvar فردية allowlisted، واختبر تصنيف Fastbootd عبر `is-userspace` مع إبقاء NotReported حالة صحيحة. مجموعة الاختبارات كاملة: 562/562. بناء WPF Release نجح بـ0 أخطاء، مع تحذيرين `CS9191` سابقين في `AdbManager.cs:364,394` خارج تغيير Fastboot.
- [ ] اختبر التبويب على هاتف موضوع يدوياً في Fastboot. لا ينفذ التطبيق إعادة تشغيل أو انتقال وضع تلقائياً، والهاتف الموثق متصل حالياً عبر ADB اللاسلكي.
- [ ] افحص التبويب بصرياً وانقر أزراره عند توفر أداة Windows UI Automation؛ اختبار XAML والبناء لا يثبت ظهور العناصر أو عملها بصرياً.
- [x] أظهر WPD `FriendlyName` و`Manufacturer` و`Description` من worker القائم ضمن قسم مستقل عن حقائق Android، من دون serial أو PnP ID.
- [ ] أعد smoke metadata-only على الهاتف عبر USB عندما يظهر جهاز WPD؛ آخر تشغيل آمن أعاد 0 جهازاً و0 حقول متاحة ولم يقرأ التخزين أو الملفات.
- [x] أعد بحث GitHub لأدوات Huawei Fastboot وثبّت SHA والترخيص ونطاق الأوامر؛ الأدوات المتخصصة التي راجعتها إما GPL-3.0 أو بلا ترخيص/اختبارات، لذلك لم أنقل منها كوداً أو أوامر OEM.
- [x] أضف profile منفصلاً وتجريبياً لـHuawei Fastboot يضيف `getvar rescue_version` و`getvar vendorcountry` واستعلام `oem get-product-model` الثابت إلى القيم العامة. لا يوجد `getvar all` أو مدخل OEM عام. اختبارات parser تغطي نموذج رد Huawei.
- [ ] اختبر profile Huawei على موديل ورقم بناء وEMUI/HarmonyOS وCUST معروفين؛ لا يوجد جهاز محدد حالياً، لذلك لا تدّعِ توافقاً ميدانياً.
- [ ] أضف واجهة لحالات البروتوكول وRecovery sideload، من دون أوامر تغيير وضع أو OEM غير موثقة.
- [ ] أعد تقييم كل محول Samsung/Xiaomi/Huawei من مصدره وترخيصه واختباراته قبل دمجه.
- [ ] بعد اختبار موديل Huawei موثق، قرر هل يلزم توسيع profile. لا تضف أوامر OEM أو قيماً تعريفية دون دليل موديل محدد واختبارات fixtures وهاتف.

آخر تحديث: 16 نوفمبر 2026

## تحديث قراءة Fastboot

أضيف تبويب Fastboot مستقل مع profile صريح لـAndroid وآخر تجريبي لـHuawei. Android يسأل عن `is-userspace` و`product` و`current-slot` و`unlocked`. Huawei يسأل عن الأربع نفسها ويضيف `rescue_version` و`vendorcountry` كـ`getvar` فرديين، مع استعلام OEM ثابت واحد `get-product-model` لقراءة رمز الموديل. لا `getvar all` ولا مدخل OEM عام أو أوامر انتقال/كتابة، ولا `rescue_phoneinfo` أو serial/IMEI/PSID. استندنا لأمثلة أدوات GitHub في [25](25-FASTBOOT-SOURCE-REVALIDATION.md)، ولم ننقل كوداً من المشاريع بلا ترخيص معلن. `dotnet test src/DeviceToolkit.Tests/DeviceToolkit.Tests.csproj --no-restore` اجتاز 572 اختباراً، 0 فشل و0 تخطٍ. بناء Release للحل نجح بـ0 أخطاء و3 تحذيرات سابقة في `AdbManager.cs` و`FilterEngineTests.cs`. لا يوجد جهاز Huawei محدد حالياً، وأمر `fastboot devices` المحلي السابق لم يجد جهازاً، والفحص البصري بالنقر ما زال مفتوحاً.

## تحديث بيانات WPD/MTP

تستخدم الواجهة `WpdDeviceInfo` الموجود من worker الحالي، وتعرض الاسم والشركة والوصف ضمن قسم Windows metadata مستقل عن Android. أضيف خيار `--metadata-only` إلى smoke القائم ليتحقق من الحقول دون فتح storage أو تعداد ملفات أو طباعة قيم تعريفية حساسة. نتيجة آخر تشغيل: 0 جهاز WPD، 0 اسم، 0 شركة، 0 وصف؛ لم يقرأ ملفات أو مسارات. الهاتف كان ظاهراً عبر ADB في تعداد منفصل، لكنه لم يظهر في WPD أو Fastboot، لذلك اختبار الحقول الفعلية ينتظر اتصال USB بوضع نقل الملفات أو دخول Fastboot يدوياً.

## متابعة الواجهة: مخطط استخدام أولي

- [x] استبدال رؤوس التبويبات بقائمة عمل ثابتة يسار النافذة، ومساحة العمل في الوسط، واختصارات على اليمين.
- [x] إبقاء اختيار الهاتف مشتركاً أعلى النافذة، وربط الاختصارات بأوامر Refresh وRead info وLogcat والانتقال إلى Wireless ADB.
- [x] بناء نسخة Release بعد التخطيط: 0 تحذيرات و0 أخطاء، وفحص `git diff --check` نظيف.
- [x] فتح نسخة Release الجديدة، والعملية استجابت.
- [x] إصلاح زر `Load app inventory` الذي تمدد بعرض مساحة العمل بسبب `DockPanel.LastChildFill`؛ أصبح زر تحديث ثانوياً بعرض محدود. أضيف عنوان `Latest action` لشريط الحالة.
- [x] بناء التعديل في `artifacts/wireframe-fixed/Release/net10.0-windows/` مع إبقاء النافذة القديمة مفتوحة: 0 تحذيرات و0 أخطاء.
- [ ] إعادة فتح النسخة المبنية حديثاً ومراجعة التخطيط بصرياً؛ نسخة Release المفتوحة كانت تقفل DLL، وأداة التحكم بواجهة Windows غير متاحة.
- [ ] فحص النافذة بالنقر على الأقسام والأزرار. لم يتم الفحص: `cua.getState()` لم يعثر على تطبيقات، وأعاد `Computer Use native pipe is unavailable: failed to connect native pipe: The system cannot find the file specified`.
- [ ] بعد توفر أداة التحكم، افحص الأقسام الستة، وحالة عدم وجود جهاز، واختصارات الإلغاء والاتصال، ثم راجع MTP من الواجهة قبل اعتبار التخطيط معتمداً.

**نتيجة محاولة فحص إضافية:** بعد طلب المستخدم تشغيل النسخة المعدلة وفحصها، بقيت عمليتا WPF مستجيبتين. فشل `cua.getState()` و`cua.listWindows()` بالخطأ نفسه الخاص بمضيف التحكم. فحص XAML يؤكد تعريف ستة أقسام وأزرارها، لكنه لا يثبت ظهورها في الشاشة؛ لا تُسجّل الخطوة كنجاح بصري.

الواجهة الحالية ترتيب وظيفي أولي مستند إلى رسم Excalidraw، وليست مرحلة التصميم النهائي. لم تُشغّل الاختبارات في متابعة التخطيط هذه.

## المسار الحالي: فحص واجهة WPF

**التقدم السابق في Wireless ADB:** أصلح انتظار خدمة Connect بعد نجاح Pair: بدل خمس محاولات ضمن 2.5 ثانية، ينتظر حتى 20 ثانية مع فحص كل 500 ms وإلغاء مضبوط بالمهلة. بعد أن أُدخل الرمز في PasswordBox، نجح زر التطبيق على هاتف SM-N976N: ظهرت خدمة TLS Connect، وأكدت الواجهة أن الهاتف اتصل وأصبح محدداً. سجل AuditStore بدء ونجاح `device.wireless_pair` ثم نجاح `device.discover`. لم يُحفظ الرمز. اجتازت المجموعة الكاملة `dotnet test DeviceToolkit.slnx -c Release --no-restore` عدد 548 اختباراً، 0 فشل و0 تخطٍ، ونجح بناء WPF بـ0 تحذيرات و0 أخطاء. أُعيد بناء WPD وworker بلا تحذيرات أو أخطاء؛ fake-worker smoke أثبت بقاء عملية واحدة للجلسة، نقل مسار عربي وتنزيل 16 MiB، وقتل العملية عند timeout والإلغاء. آخر اختبار USB وجد جهاز WPD واحداً، وفتح جلسة read-only وعدّ 74 ملفاً في `Download` ضمن مهلة 8 ثوانٍ؛ لم تُستثر مهلة فعلية على الهاتف. الفحص البصري ما زال غير مكتمل.

- [x] تشغيل اتصال ADB اللاسلكي مع الهاتف الفعلي والتأكد من ظهوره Online.
- [x] اختبار `adb pair` برمز أُدخل عبر البطاقة الآمنة. أرجع ADB نجاح الاقتران.
- [x] تحديد أن الهاتف كان يعلن خدمة الاقتران وحدها قبل الاقتران، ثم تحقق الاقتران عبر المنفذ المكتشف.
- [x] التجربة الأولى لزر `Pair and connect` داخل WPF وصلت إلى نجاح Pair لكنها لم تجد خدمة Connect ضمن مهلة المحاولات القصيرة؛ رُفعت المهلة إلى 20 ثانية.
- [x] إعادة اختبار الزر بعد الإصلاح على الهاتف: نجح Pair وConnect، أكد AuditStore نجاح العملية، وأفادت الواجهة بأن الهاتف أصبح محدداً.
- [ ] فحص واجهة التطبيق بصرياً. نجح الزر وأظهرت واجهة WPF رسالة نجاح، لكن مضيف CUA تعذر تهيئته، لذلك لم ألتقط صورة للشاشة.
- [x] إعادة الاختبارات بعد تحديث المسار: `dotnet test DeviceToolkit.slnx -c Release --no-restore` طبع 518 ناجحاً، 0 فشل، 0 تخطٍ.
- [x] بناء التطبيق إلى `artifacts/wireless-final/`: `dotnet build ... -c Release --no-restore` طبع 0 تحذيرات و0 أخطاء.

### آخر فحص متابعة في 14 نوفمبر 2026

ظهر الهاتف عبر ADB اللاسلكي كـ`SM-N976N`، Android 12/API 31. شغّلت `tests/fixtures/diagnostic-smoke/` على serial صريح: نجحت قراءة facts، والتقط Logcat مدة 30.0 ثانية وقرأ 1,299 سطراً، بلا crash events أو groups. لم يُطبع محتوى Logcat. سجل SQLite أظهر بدء ونجاح `device.discover` و`device.read_info` و`diagnostics.logcat_capture`؛ لا يظهر serial أو المحتوى في الملخصات.

اختُبر زر `Pair and connect` في WPF: أُدخل الرمز مباشرةً إلى حقل PasswordBox دون طباعته أو تسجيله. أكد ADB نجاح Pair، لكن خدمة Connect لم تظهر عبر mDNS أثناء محاولات التطبيق؛ سجل `AuditStore` `ApprovalGranted` و`OperationStarted` ثم `OperationFailed` لعملية `device.wireless_pair`. بقي اتصال ADB السابق Online، ولم ينفذ التطبيق `ConnectAsync`. لا يثبت هذا نجاح تدفق الزر كاملاً.

لم أغيّر إعدادات Wireless debugging. آخر مجموعة اختبارات كاملة قبل تغييرات التدقيق: 545 ناجحاً، 0 فشل، 0 تخطٍ؛ مدقق العقود مرّ بـ48 فحصاً و13 حالة سلبية، وبناء WPF النهائي مرّ بـ0 تحذيرات و0 أخطاء. الخطوة التالية: تشخيص سبب غياب خدمة Connect بعد نجاح Pair وإعادة الاختبار، ثم إدخال WPD في الواجهة وربطه بالتدقيق.

### ما تم في آخر متابعة

اقترن الهاتف `SM_N976N` بنجاح على `192.168.1.4:45919` عبر `adb pair`، وأظهر `adb devices -l` اتصالاً لاسلكياً بحالة `device`. هذا يثبت اقتران ADB على الهاتف، لكنه لا يثبت أن زر التطبيق أو واجهته نفّذا المسار. تعذر بدء أتمتة واجهة Windows برسالة `Computer Use native pipe is unavailable`، لذلك بقي فحص الواجهة واختبار الزر معلقين.

أعيدت الاختبارات بعد التحديث: 518 ناجحاً، 0 فشل، 0 تخطٍ. أُعيد بناء الواجهة إلى مجلد مستقل: 0 تحذيرات، 0 أخطاء. لم يُختبر زر التطبيق بعد.

فُحص المتصفح بناءً على طلب المستخدم؛ لم يكن هناك تطبيق ويب أو عنوان محلي، والصفحة الفارغة لم تعرض واجهة WPF. أُغلقت علامة التبويب بعد الفحص.

كفحص قراءة مباشر للهاتف (وليس عبر واجهة التطبيق)، أكدت ADB أن الجهاز Online وقرأ: `SM-N976N`، Samsung، Android 12، API 31، `arm64-v8a`. أعاد فحص logcat الحديث سطرين فقط دون طباعة محتواهما، ولم تطابق أي منهما علامات crash أو ANR. هذا خط أساس لحظي، وليس اختبار التقاط التطبيق لمدة 30 ثانية.

أُنشئ smoke runner يعيد استخدام `DeviceWorkflow` الحالي في `tests/fixtures/diagnostic-smoke/`، ويشترط serial محدداً دون اختيار تلقائي. شُغّل على `192.168.1.4:35443`: قرأ Model `SM-N976N`، Manufacturer `samsung`، Android `12`، API `31`، ABI `arm64-v8a`. اكتمل التقاط وتحليل 30 ثانية خلال `30.0` ثانية: `3977` سطراً، `0` crash events، `0` crash groups. لم يطبع السجل، ويزيل `DeviceWorkflow` الملف المؤقت بعد التحليل.

اكتمل الاختبار بعد موافقة المستخدم عبر `tests/fixtures/file-transfer-smoke/` مع استهداف serial `192.168.1.4:35443` صراحةً. رفع `8602` بايت إلى `/sdcard/Download/DeviceToolkitInstallTest-smoke.apk`، ونُزّل الملف للتحقق، وطابق SHA-256 للمصدر والنسخة: `8616B71B2DD880D3E0E64F5BF403942A4F581B687A35E50E9FA31457BFE17416`. حذف المشغل الملف من الهاتف ثم تحقق من اختفائه. حُذف ملف التنزيل المحلي المؤقت أيضاً.

## بعد إغلاق المسار الحالي

- [x] اختبار قراءة معلومات الهاتف عبر `DeviceWorkflow` مع serial صريح.
- [x] اختبار التقاط logcat المحدود وتحليله على الهاتف عبر `DeviceWorkflow` لمدة 30 ثانية.
- [x] اختبار رفع وتنزيل ملف اختبار محدد، والتحقق من المسار والحجم وSHA-256 وعدم الاستبدال، ثم حذف النسخة التجريبية والتأكد من اختفائها.
- [x] مراجعة بقية محاور V1 مقابل المرشحين وقرارات إعادة الاستخدام في وثائق البحث؛ المحاور المؤجلة وحدود المرشحين موثقة أدناه وفي `docs/12-reuse-and-implementation-plan.md`.
- [x] مراجعة مرشحي تشخيص تعريفات Windows وإثبات مسار جرد read-only على المضيف الحالي.
- [x] بناء خدمة PnP read-only عبر CsWin32 وSetupAPI/Configuration Manager، تعرض اسم الجهاز وInstance ID وHardware IDs وstatus flags وProblem Code.
- [x] ربط الخدمة بتبويب `Windows drivers` مع حالات البدء والتحميل والنتيجة الفارغة والخطأ وإعادة المحاولة.
- [x] تشغيل smoke read-only لخدمة PnP ومقارنة قائمة الأجهزة مع PnPUtil وWMI.
- [ ] تشغيل التطبيق وتحديث الجرد من التبويب وفحص حالات الشاشة بصرياً. أُعيدت محاولة أداة `computer-use` بعد قراءة تعليماتها، لكنها فشلت من `sky.list_apps` بالخطأ `Computer Use native pipe is unavailable`. لا يمكن اعتماد فحص UI Automation أو المتصفح كبديل لتطبيق WPF الأصلي.
- [x] مراجعة مرشحي Private DNS: لا يوجد مكوّن جاهز مثبت يغطيه؛ استخدم ADB read-only الموجود لقراءة Settings.Global بعد parser typed، ولا تنسخ تطبيقات الإدارة أو تستخدم مسار Device Owner.
- [x] إضافة قراءة Private DNS read-only إلى `DeviceWorkflow` ولوحة معلومات الجهاز، مع fallback إلى `private_dns_default_mode` واختبارات parsing لقيم off/opportunistic/hostname/unknown.
- [x] التحقق من نشاط DNS-over-TLS على الهاتف عبر حالة LinkProperties لشبكة Wi-Fi النشطة وطلب DNS ناجح، دون تغيير الإعداد أو تسجيل اسم المزود.
- [x] تحديث هذا الملف و`22-HANDOFF.md` بنتائج مقاسة فقط بعد كل خطوة.
- [x] إعادة التحقق من مسار MTP/WPD عبر وثائق Microsoft الرسمية وتحديد شروط بدء provider واختبار الهاتف USB أدناه.
- [x] عند توصيل الهاتف، التحقق read-only من تعداد WPD وظهور `Internal storage/Download`.
- [x] إعادة التحقق من خدمة WPD الجديدة على هاتف حاضر عبر USB: ظهر `Peter's Note10+` في PnP بحالة `OK`، وأعاد smoke runner جهازاً واحداً مع اسم ودي ومصنّع متاحين. لم يطبع runner القيم أو PnP ID، ولم يقرأ محتوى الهاتف.
- [x] نقل ملف الاختبار ذهاباً وإياباً عبر WPD والتحقق من سلامته ثم حذفه من الهاتف: نُقل `8602` بايت إلى `Internal storage/Download/DeviceToolkitInstallTest-wpd-smoke.apk`، وأُعيد تنزيله وطابقت SHA-256 للأصل والنسخة `8616B71B2DD880D3E0E64F5BF403942A4F581B687A35E50E9FA31457BFE17416`. حُذف الملف المحدد عبر ADB USB الصريح `R3CMA016SEJ`، والتحقق اللاحق أكد غيابه. لم يُثبت أي تطبيق. النسخ المحلية المؤقتة ما زالت موجودة لأن أمر تنظيفها رُفض قبل التشغيل برسالة `Rejected by policy`.
- [x] مواءمة مراجعة MCP مع قرار .NET 10، وإعادة فحص SDK الرسمي عند tag `v2.4.0` وتوثيق revision والرخصة وframework/tests وحدود المراجعة.
- [x] نقل قراءة معلومات الجهاز وجرد التطبيقات ونسخة التطبيق من `DeviceWorkflow` إلى `ReadOnlyDeviceService` داخل Core، مع إبقاء WPF مفوضاً إلى الخدمة نفسها.
- [ ] استخرج هوية جهاز محلية `device_id` وربطها بهوية ADB صريحة، وحدد bootstrap/runtime لـADB في MCP host قبل تسجيل أي أداة؛ لا تمرر serial كـ`device_id` ولا تشغّل generic shell.

### Private DNS read-only slice

راجعت مرشحي Private DNS: لا يوجد مشروع جاهز يغطي الوظيفة؛ القرار هو إعادة استخدام ADB الموجود للقراءة فقط. يقرأ AOSP `private_dns_mode` مع fallback إلى `private_dns_default_mode`، و`private_dns_specifier` لاسم المضيف. على Android 12 ظهر mode=`hostname` وspecifier موجود، دون تسجيل قيمته أو تعديل الإعداد.

أضيف `PrivateDnsSettings` إلى Core مع حالات typed وfallback للقيمة الافتراضية، وثلاثة أوامر `settings get global` allowlisted في `DeviceWorkflow.ReadFactsAsync`. تعرض لوحة معلومات الجهاز mode وhostname محلياً. أضيفت اختبارات parsing لقيم off/opportunistic/hostname/unknown. النتيجة: 525 اختباراً ناجحاً، 0 فشل، 0 تخطٍ؛ بناء WPF إلى `artifacts/private-dns-build/` نجح بـ0 تحذيرات و0 أخطاء. smoke test على الهاتف قرأ `Hostname` ووجود المزود من دون طباعة اسمه، ثم سجل 30 ثانية من logcat: 2293 سطراً، 0 crash events، 0 crash groups.

## مراجعة مرشحي تعريفات Windows

توصي Microsoft بتعداد الأجهزة عبر SetupAPI أو Configuration Manager، وعدم تعداد Registry مباشرةً. PnPUtil مفيد كأداة نظام للفحص وsmoke tests، لكن إصدارات الأوامر والحقول تتغير بين Windows؛ لا نعتمد parsing لنصه كعقد المنتج. RAPR v1.0.26 حديث وله CI واختبارات معلنة، لكن رخصته GPL-2.0 ووظيفته تشمل تعديل Driver Store؛ لا ندمجه ولا نستعمل مسارات الحذف. القرار: واجهة المنتج تستعمل SetupAPI/CfgMgr32 عبر bindings Windows المعتمدة، وتستخدم PnPUtil فقط في تجارب المضيف والتحقق اليدوي.

على Windows build `26200` شغّل `pnputil /enum-devices /connected /deviceids` و`pnputil /enum-devices /problem /deviceids` للقراءة فقط، وكلاهما خرج برمز `0`. ظهر `135` جهازاً حاضراً، `0` أجهزة بمشكلة، و`0` معرفات Android USB من قائمة الشركات المفحوصة. الهاتف متصل عبر Wi-Fi، لذا لا توجد واجهة USB فعلية لاختبار الربط.

أضيفت خدمة `PnpDeviceInventory` إلى Core باستخدام CsWin32 `0.3.371`، مع APIs المولدة من `NativeMethods.txt` لاستدعاء SetupAPI وConfiguration Manager. تستخدم `DIGCF_PRESENT | DIGCF_ALLCLASSES`، وتقرأ اسم الجهاز وinstance ID وHardware IDs وstatus flags وProblem Code. لا تكتب إلى الجهاز ولا تقرأ Registry مباشرة. رُبطت بتبويب `Windows drivers` بقراءة يدوية عبر زر، وفرز أجهزة المشكلات أولاً، ورسائل تحميل/فراغ/خطأ وإعادة محاولة. اختير أسلوب `achromatic-ledger` (seed `1082445840`) مع الاحتفاظ بلوحة ألوان المشروع، ووجه الشاشة من لون الخلفية الدافئ القائم.

شُغّل `tests/fixtures/pnp-inventory-smoke` بنجاح: SetupAPI أعاد `135` جهازاً حاضراً، وطابقت القائمة تماماً `135` instance ID من `pnputil /enum-devices /connected /deviceids`، بلا IDs مفقودة أو زائدة. أبلغ Configuration Manager عن جهاز واحد برمز مشكلة `22` مع `DN_HAS_PROBLEM`؛ وأعاد WMI بشكل مستقل جهازاً واحداً برمز `22`. في المقابل أرجع `pnputil /enum-devices /problem /deviceids` صفراً، واستعلام PnPUtil للجهاز صنفه `Disconnected` مع `IsPresent=TRUE` و`HasProblem=FALSE`. هذا اختلاف مرصود بين تقارير APIs، لذا نعرض الرمز كما أبلغته Configuration Manager ولا ندعي تطابق نتيجة PnPUtil. بناء Release للواجهة نجح: 2 تحذير `CS9191` قديمين في `AdbManager.cs`، و0 أخطاء. `git diff --check` نظيف. لم يعمل التطبيق ليتحقق ضغط الزر أو شكل الشاشة؛ أداة Computer Use فشلت برسالة `Computer Use native pipe is unavailable` والمتصفح غير متاح. يبقى فحص الواجهة بصرياً واختبار هاتف USB معلقين.

إعادة تشغيل الـsmoke: `dotnet run --project tests/fixtures/pnp-inventory-smoke/PnpInventorySmoke.csproj -c Release`. إعادة فحص WMI للرموز فقط: `Get-CimInstance -ClassName Win32_PnPEntity | Where-Object { $_.ConfigManagerErrorCode -ne 0 } | Group-Object ConfigManagerErrorCode | Select-Object Name, Count`. لا تطبع هذه الأوامر معرّفات الأجهزة.

## مراجعة بقية محاور V1 وإعادة الاستخدام

راجعت خطة إعادة الاستخدام وقرارات الهندسة وإعادة التحقق من المصادر في `docs/12` و`docs/13` إلى `docs/20` ووثائق المحاور. القرار هو:

- ADB وملفات Android وLogcat: استمر بإعادة استخدام AdvancedSharpAdbClient و`DeviceWorkflow` ومكوّنات Logcat المنقولة والموثقة؛ لا تضف عميل ADB أو parser موازياً.
- إدارة التطبيقات: استخدم `PackageManager` و`apkanalyzer` المثبتين في التدفق الحالي. bundletool هو المسار القياسي لملفات APKS عند تنفيذ دعمه، وapkfile مجرد مرشح parsing لمجموعات APKM/XAPK/APKS بعد إعادة فحص المصدر وبناء corpus.
- Windows PnP: لا يوجد مشروع جاهز مناسب للدمج؛ SetupAPI/Configuration Manager الرسميان هما المسار المختار والمنفذ للقراءة. RAPR GPL-2.0 لا يدمج، وPnPUtil يبقى أداة مقارنة/تشخيص يدوية بسبب اختلاف سلوكه المرصود عن CM/WMI على هذا المضيف.
- MTP/WPD: لا تنسخ File Manager خارجي. المرشح هو Windows WPD API كـprovider منفصل، ويظل مؤجلاً حتى تجربة تعداد/نقل Unicode على Windows وجهاز اختبار.
- MCP: خوادم `adb-mcp` و`android-mcp-server` مراجع لبنية الأدوات فقط؛ لا تنشئ ADB client موازياً. التنفيذ المستقبلي يكون عبر .NET MCP SDK فوق Application Services، بأدوات قراءة فقط واستهداف جهاز صريح.
- Backup وOTA/partitions ومصادر التطبيقات: لا يوجد مرشح جاهز للدمج كما هو. `chen3feng/android-backup` و`payload-dumper-go` و`lucky-arch` وF-Droid signed index لها أدوار محددة ومخاطر موثقة في المصادر المعاد التحقق منها؛ قبل أي تنفيذ جديد أعد فحص revision والرخصة والصيانة والاختبارات وبناء corpus.

اعتمدت مراجعة المرشحين السابقة على تقارير source-revalidation المثبتة عند SHAs المذكورة في الوثائق. في هذه المتابعة أُعيد فحص .NET MCP SDK `v2.4.0` ونقلت مجموعة استعلامات قراءة إلى `ReadOnlyDeviceService` في Core. لم أضف حزمة MCP أو host بعد: ما زال يلزم device_id محلي لا يكشف serial وتهيئة ADB يملكها host صراحةً. أضيف أيضاً provider WPD read-only فوق MediaDevices 2.0.0، ونتيجة اختبار الهاتف وحدوده أدناه. أما backup/firmware ومصادر التطبيقات فتظل مراحل منفصلة.

بناء الحل بعد استخراج الخدمة: نجح `dotnet build DeviceToolkit.slnx -c Release --no-restore -v:minimal -m:1 -nr:false` مع 0 أخطاء و3 تحذيرات سابقة خارج الملفات المعدلة: `CS9191` اثنان في `src/DeviceToolkit.Core/Adb/AdbManager.cs:362,392`، و`xUnit2029` واحد في `src/DeviceToolkit.Tests/FilterEngineTests.cs:689`. لم تُشغّل مجموعة الاختبارات.

## مسار MTP/WPD التالي

**سجل الاستكشاف الأولي قبل provider:** أُعيدت مراجعة وثائق Microsoft الرسمية: تعداد الأجهزة يبدأ بـ`IPortableDeviceManager::GetDevices`، وتُعاد القائمة عند إنشاء manager، لذلك ينبغي استدعاء `RefreshDeviceList` قبل التعداد المتجدد. معرفات PnP الناتجة هي المعرفات التي تُمرر إلى `IPortableDevice::Open`. بعد فتح الجهاز، يتيح `IPortableDeviceContent` تعداد المحتوى، بينما `IPortableDeviceResources` و`IPortableDeviceDataStream` هما واجهتا موارد/نقل البيانات. بعد توصيل الهاتف ظهر جهاز WPD واحد بحالة PnP `OK`، وأظهر Shell `Internal storage/Download`. كانت هذه الملاحظة قبل اختبار provider، وتحديث نتيجته أدناه.

أُضيف مشروع `src/DeviceToolkit.Wpd/` فوق مرشح `MediaDevices` بإصدار NuGet `2.0.0` (MIT، upstream tag commit `83e66e035dd417078cf40f5e1afbc18d90079acd`). النسخة الحالية من upstream أحدث، لكننا ثبّتنا إصدار الحزمة المنشور الذي راجعنا مصدره. يوفر المشروع اكتشافاً بمعرّف اختيار مؤقت محلي، واتصالاً صريحاً read-only، وتعداداً بمقابض opaque، وواجهة تنزيل تتحقق من الحجم؛ لا توجد فيه عمليات رفع أو إنشاء أو حذف أو إعادة تسمية. بناء تطبيق WPF Release نجح بعد إضافة المشروع: 0 تحذيرات و0 أخطاء. Smoke فعلي على الهاتف اختار `Peter's Note10+` صراحةً، ونجح الاتصال read-only وتعداد جذر WPD: مدخل واحد هو مجلد التخزين. لا يطبع runner رقم الجهاز أو PnP ID.

الاستقصاء كشف تذبذباً بدل سبب واحد ثابت. في محاولة علق فتح جلسة provider أو جلب الجذر حتى مهلة 30 ثانية. في اختبار المكتبة المباشر نجح الاتصال وتعداد 58 مجلداً في `Internal storage`، لكن تعداد الملفات تجاوز 30 ثانية مرة ثم أعاد 333 ملفاً في محاولة أخرى. مسار `GetDirectoryInfoAsync("Internal storage/Download")` أتم حل المسار مع cancellation token، وتعداد 25 مجلداً و480 ملفاً وقراءة أحجامها بإجمالي `9214051094` بايت اكتمل. ملف تعريف MediaDevices يبين أن `MediaDirectoryInfo.EnumerateDirectories/EnumerateFiles` متزامنة بلا cancellation token، بينما `MediaDevice.Cancel()` موثق لإلغاء عملية معلقة. اختبار Cancel أثناء تعداد قيد التنفيذ أعاد القائمة كاملة (333 ملفاً)، ولم يثبت إيقاف تعليق فعلي.

فُصل API الـprovider إلى `ListDirectories` و`ListFiles` كي لا يحجب تعداد ملفات المجلد الأعلى الوصول إلى مجلد فرعي. اختبار فعلي عبر `WpdReadOnlySession` اختار الهاتف صراحةً واتصل read-only، وعدّ جذر WPD (مجلد واحد)، و58 مجلداً تحت `Internal storage`، و25 مجلداً و481 ملفاً تحت `Download` أثناء وجود ملف الاختبار. نزّل الملف المحدد حجمه `8602` بايت وطابقت SHA-256 المصدر `8616B71B2DD880D3E0E64F5BF403942A4F581B687A35E50E9FA31457BFE17416`. بعد الاختبار حُذف الملف المحدد عبر ADB USB الصريح `R3CMA016SEJ` وتأكد غيابه. لم يُثبت APK أو يُشغّل. بقيت نسختان محليتان مؤقتتان من اختبار Windows Shell السابق (staging تحت `artifacts/` وreadback تحت `%TEMP%`) لأن أمر تنظيفهما رُفض تلقائياً قبل التشغيل برسالة `Rejected by policy`؛ لا تعاود التنظيف بطريقة ملتوية.

اختبار التشخيص شغّل تعداد `Internal storage` عبر `Task.Run` على الهاتف. اكتمل في محاولة وأظهر القائمة كاملة بعد استدعاء `Cancel()`، لكنه لم يختبر واجهة WPF ولا يضمن التخلص من خيط عالق؛ لذلك لا نضيف async wrapper يوحي بإلغاء موثوق.

**متابعة WPD في 14 نوفمبر 2026:** بنيت `src/DeviceToolkit.Wpd/` و`DeviceToolkit.Wpd.Worker/` في Release بلا تحذيرات أو أخطاء. أضفت `tests/fixtures/wpd-session-smoke/` مع عامل وهمي يعالج نفس بروتوكول worker: يتأكد من بقاء PID واحد خلال الاتصال وتعداد المجلدات وطلب التنزيل، ويمرر مساراً عربياً، وينزل ملفاً محلياً بحجم 16 MiB ويتحقق من الحجم، ثم يعلّق عملية files حتى يثبت إنهاءها بالمهلة؛ سيناريو الإلغاء يثبت إنهاء العامل أيضاً. الأمر: `dotnet run --project tests/fixtures/wpd-session-smoke/WpdSessionSmoke.csproj -c Release --no-restore`، والنتيجة `WPD fake-worker smoke passed`. هذا يتحقق من lifecycle والبروتوكول فقط، ولا يثبت MediaDevices أو USB/MTP على الهاتف. شغّلت `tests/fixtures/wpd-inventory-smoke/` للقراءة فقط؛ أرجع `WPD devices enumerated: 0`، بينما الهاتف حاضر عبر ADB Wi-Fi. **الخطوة التالية:** وصل الهاتف عبر USB واختر File Transfer/MTP، ثم شغّل `dotnet run --project tests/fixtures/wpd-inventory-smoke/WpdInventorySmoke.csproj -c Release --no-restore`، واتبع باختبار الجلسة البطيئة. لا تنشئ ملفات على الهاتف إلا إذا احتاجها الاختبار.

**تحديث دورة WPD والبوابة (14 نوفمبر 2026):** أُضيف اختبار التخلص من الجلسة أثناء طلب معلّق؛ نجح في إثبات إلغاء الطلب وإنهاء worker. يختبر fixture الآن الجلسة الواحدة، المسار العربي، تنزيل 16 MiB، المهلة، الإلغاء الخارجي، والتخلص من الجلسة. أضيفت استعادة lockfile وتشغيله إلى CI على Windows. ما زال الاختبار على هاتف USB/MTP مطلوباً.

**تحديث WPD USB بعد توصيل الهاتف (14 نوفمبر 2026):** ظهر جهاز WPD واحد، ونجح الـsmoke في فتح جلسة read-only واختيار جذر `Phone` صراحةً كالجذر المعروف الوحيد، ثم عدّ 15 مجلداً و`Download`. ملفات `Download` كانت 0. probe بمهلة 8 ثوانٍ فحص `DCIM` و`Pictures` و`Download` و`Documents` و`Movies` و`Music`؛ كلها فارغة، لذلك لم يحدث timeout فعلي، لكن worker أنهى كل عمليات التعداد والتنظيف بلا عمليات متبقية. لا أنشئ بيانات اختبار على الهاتف تلقائياً. لإثبات مهلة WPD على جهاز فعلي، استخدم مجلداً موجوداً يحوي عدداً كبيراً من الملفات أو وافق على إنشاء بيانات اختبار مؤقتة محددة.

**إعادة اختبار WPD بعد أن وضع المستخدم ملفات (14 نوفمبر 2026):** ظهر جهاز WPD واحد، ونجح الجرد في جلسة read-only. وجد `Download` 74 ملفاً؛ أكملت مجلدات `DCIM` و`Pictures` و`Download` و`Documents` و`Movies` و`Music` تعدادها ضمن مهلة 8 ثوانٍ، لذا لم يحدث timeout فعلي. لم تبق عمليات worker بعد الاختبار. في هذا الفحص لم نقرأ محتوى الملفات أو نعدّلها؛ التنزيل المحلي المحدود جرى في اختبار منفصل موثق أدناه. يظل إثبات إنهاء تعداد بطيء على الهاتف مفتوحاً؛ اختبار timeout/cancellation/disposal نجح على fake-worker.

**متابعة Unicode والتنزيل والإلغاء على WPD (14 نوفمبر 2026):** من metadata فقط، ظهر 65 اسماً يحوي Unicode من 74 ملفاً في `Download`، وكان أكبر حجم معروف 67,588,123 بايت. نُزّل ملف Unicode إلى مجلد محلي مؤقت: 9,971,441 بايت، وطابق الحجم المعلن، ثم حُذف. أُلغي طلب تنزيل الملف الأكبر بعد 250 ms؛ توقف worker، ورفضت الجلسة أي طلب تالٍ، ولم تبق ملفات مؤقتة. فتحت جلسة جديدة بعدها بنجاح وأعادت تعداد الملفات الـ74. لم يحدث أي تعديل على الهاتف؛ أسماء الملفات لم تُطبع في خرج الاختبار. فصل USB الفعلي وإعادة الاتصال التلقائية لم يُختبرا.

**ربط WPD بواجهة سطح المكتب (14 نوفمبر 2026):** أضيف تبويب `MTP storage` منفصل للتعداد والتصفح read-only وتنزيل ملف بعد معاينة المسار والحجم، مع اختيار الجهاز صراحةً وتسجيل قائمة الملفات والتنزيل في `AuditStore` تحت `file.list` و`file.pull`. الإلغاء والمهلة يغلقان الجلسة ويطلبان تحديث الأجهزة قبل إعادة المحاولة. البناء المعزول للواجهة نجح بـ0 تحذيرات و0 أخطاء، واختبارات الحل نجحت بـ548/548. لم يُفحص العرض أو تُنقر عناصره؛ أداة Computer Use أعادت `failed to write kernel assets`، والتطبيق الحالي المفتوح يمنع استبدال exe مباشرة.

## تحقق اتصال DNS-over-TLS

أعيد الفحص مباشرةً على الهاتف المحدد: `private_dns_mode=hostname`، وشبكة Wi-Fi النشطة أظهرت `UsePrivateDns=true` و`ValidatedPrivateDnsAddresses` فيها عنوانان، كما أظهر `ping -c 1 -W 2 example.com` أن اسم النطاق حُلّ إلى عنوان. هذا يتحقق من DNS resolution فقط ولا يزعم وصول ICMP. لم أطبع عنوان الخادم أو اسم المزود ولم أغير أي إعداد. اجتماع الوضع الصارم `hostname`، وعناوين Private DNS المعتمدة على الشبكة، ونجاح حل الاسم يثبت أن DoT كان متاحاً ويُستخدم وقت الفحص. `dumpsys dnsresolver` لم يُظهر log متاحاً لهذا المستخدم، لذا اعتمدت دليل LinkProperties ونتيجة resolver.

## Huawei Fastboot live update, 16 November 2026

A Huawei PPA-LX2 is now present in Fastboot, superseding the earlier note that no Huawei target was connected. The read-only profile now queries fixed product/build/base/custom/preload/Android/HWNFF/key/lock fields plus the existing vendor/country and rescue variables. It derives the equipment model from the build-version prefix. Hardware reads matched the supplied report; common Android getvar fields returned a permission denial while Huawei OEM fields succeeded. Parser tests: 36/36 passed. Desktop Release build: 0 warnings, 0 errors. The running app is open. Computer Use could not connect to the Windows host, so the visible read-button flow still needs a user-side check. Detailed results are in [25](25-FASTBOOT-SOURCE-REVALIDATION.md).
