# 25 - إعادة تقييم أدوات Fastboot من GitHub

**تاريخ التحقق:** 14 نوفمبر 2026. تمت مراجعة فرع المصدر الحالي، commit ثابت، الترخيص المعلن، وبوابات GitHub الظاهرة. نتائج CI لا تثبت اختبار هاتف بعينه.

## قرار التنفيذ

استخدمنا Android Platform Tools المثبتة محلياً، ولم نضف أداة Fastboot موازية أو حزمة خارجية. أضفنا محولاً صغيراً داخل التطبيق لاستدعاء `fastboot.exe` مباشرة. يحد هذا الاختيار نطاق الكود ويترك تحديث أداة البروتوكول للمستخدم أو تثبيت Android SDK.

نمط التشغيل الذي استرشدنا به من ATA-GUI هو تشغيل العملية خارج واجهة المستخدم، تمرير الوسائط كقائمة، متابعة مخرجي stdout وstderr معاً، فرض مهلة، وإيقاف شجرة العملية عند الإلغاء. هذا تكييف تصميمي مستقل بلغة C#، ولم تُنسخ ملفات أو مقاطع كود من المصدر. لم ننسخ مسار Device Info لأن المصدر يستعمل `getvar all` ويحتفظ بقائمة تتضمن `serialno` و`token`؛ وهو أوسع من سياسة هذا المشروع. راجع [flash.rs](https://github.com/msartore/ATA-GUI/blob/fa1b97f6a6ba4ff13edae982f78efc195621e262/src-tauri/src/flash.rs#L24-L29)، [device.rs](https://github.com/msartore/ATA-GUI/blob/fa1b97f6a6ba4ff13edae982f78efc195621e262/src-tauri/src/device.rs#L465-L505)، و[tools.rs](https://github.com/msartore/ATA-GUI/blob/fa1b97f6a6ba4ff13edae982f78efc195621e262/src-tauri/src/tools.rs#L134-L218).

## المرشحون

| المشروع عند commit المراجع | الدليل | النتيجة |
|---|---|---|
| [msartore/ATA-GUI](https://github.com/msartore/ATA-GUI/tree/fa1b97f6a6ba4ff13edae982f78efc195621e262)، `fa1b97f6a6ba4ff13edae982f78efc195621e262`، Apache-2.0 | مستودع Tauri/Rust نشط؛ GitHub أظهر 11 check مكتملة ناجحة. أداة عامة جيدة، لكن Device Info يستخدم `getvar all` وحقولاً تتضمن `serialno` و`token`. | أفضل مرجع لأنماط إدارة العملية والمهلة فقط. لا نقل مباشر لمسار القراءة أو parser، ولا حاجة لإضافة المشروع كتبع. |
| [Tentex1/ADBFastbootGUI](https://github.com/Tentex1/ADBFastbootGUI/tree/ff38470f525ec7d272c6a83eade8c26ae7a2e7b1)، `ff38470f525ec7d272c6a83eade8c26ae7a2e7b1`، MIT | WPF و.NET Framework 4.8. GitHub لم يعرض check runs. فحص `FastbootService.cs` وجد `WaitForExit` بلا مهلة أو إلغاء، تمرير arguments كسلسلة واحدة، وعدم قراءة stderr. لم يظهر مجلد اختبارات في الشجرة المراجعة. | لم ننسخ الخدمة؛ أسلوب تشغيل العملية غير مناسب لمتطلبات الإلغاء والحدود الحالية. |
| [kooiamsung/DroidSpark](https://github.com/kooiamsung/DroidSpark/tree/9868b89607f6a107c2965685ad80d450849e937e)، `9868b89607f6a107c2965685ad80d450849e937e`، GPL-3.0 | تطبيق WPF أوسع يتضمن flash/erase/unlock وأوامر عامة. GitHub لم يعرض check runs ولم تظهر اختبارات في شجرة المصدر المراجعة. | لم نضفه أو ننسخ منه بسبب ترخيصه ونطاق الكتابة الواسع وغياب دليل الاختبارات في المصدر المراجع. |

حالة بيانات GitHub قابلة للتغير؛ القيم أعلاه لقطة API عند تاريخ المراجعة، أما الروابط إلى الشيفرة فتثبت commit المراجع. لا تعني نتيجة check خضراء أن مسار الهاتف اختُبر عملياً.

## Huawei Fastboot مسار منفصل

ملاحظة المستخدم صحيحة: Huawei Fastboot ليس مجرد تسمية ثانية لمسار AOSP العام. هدف الأمان الرسمي لهاتف P40 مع EMUI 11 يذكر أن Fastboot مبني على Google Android Fastboot مع تحسينات Huawei، ويصف XLoader الخاص بـHuawei قبل Fastboot. هذا يثبت وجود طبقة Huawei في ذلك المنتج/الإصدار، ولا يثبت أن كل أوامرها ثابتة بين الأجهزة.

تعليمات Huawei الرسمية لـHiSuite تجعل إصلاح النظام مشروطاً بظهور الموديل في قائمة الدعم، وتوضح أن دخول Fastboot يختلف لبعض الموديلات. لذلك يلزم ربط كل امتداد Huawei بالموديل والـCUST والمنطقة وإصدار النظام وطريقة الدخول، وعدم الاستنتاج من VID/PID أو `product` وحدهما. المصادر: [P40 EMUI 11 Security Target](https://www.commoncriteriaportal.org/files/epfiles/st_huawei_p40_emui11_v1.11.pdf)، [Huawei system repair guidance](https://consumer.huawei.com/cn/support/content/zh-cn16005208/).

راجعنا أيضاً أدوات Huawei Fastboot على GitHub:

| المشروع عند commit المراجع | الدليل | قرار إعادة الاستخدام |
|---|---|---|
| [ljlVink/haucet](https://github.com/ljlVink/haucet/tree/f6579ffc387e11b24b7db09aa5aacce9371bb251)، `f6579ffc387e11b24b7db09aa5aacce9371bb251`، GPL-3.0 | تحديث 9 أكتوبر 2026. واجهة Huawei/HarmonyOS متخصصة، لكنها تجمع `getvar` مع `oem`, `flash`, `reboot`, قراءة memory/raw storage، وHuawei private `ultraflash`. يستخدم USB عبر `nusb` مباشرة. لم أجد suite اختبارات في شجرة المصدر عند هذا commit؛ توجد ملفات إصدار. | أقوى مرجع يثبت أن Huawei لها امتدادات خاصة، لكنه غير مناسب كتبع أو مصدر نقل للمشروع بسبب GPL-3.0 ونطاق العمليات المختلط. لا ننقل raw memory أو ultraflash. |
| [R0rt1z2/huawei-playground](https://github.com/R0rt1z2/huawei-playground/tree/b6e2202bb252b5f6eac2a99e8fc979bef2f19f90)، `b6e2202bb252b5f6eac2a99e8fc979bef2f19f90`، GPL-3.0 | أدوات بحثية لصيغ Huawei؛ `partdumper.py` يستخدم OEM commands ويصف دعمه بأنه تجريبي ومحدود ببعض أجهزة MediaTek. المصدر لا يقدم adapter عام لقراءة المعلومات، ولم تظهر suite اختبارات للمسار. | مرجع لتحديد تنوع أجهزة Huawei، لا نعتمد أوامر التفريغ أو ننقل الملفات. |
| [HikariCalyx/python-fastboot](https://github.com/HikariCalyx/python-fastboot/tree/d26bcfbc0013a24ce144c61ea929b8b0641eb5f4)، `d26bcfbc0013a24ce144c61ea929b8b0641eb5f4`، Apache-2.0 | Python/libusb implementation وtests عامة، وآلية `BrandCommand` لأوامر OEM. آخر push ظاهر 13 يوليو 2025؛ المراجعة لم تجد profile أو اختبارات Huawei موثقة، ويشرح README اتصالاً مباشراً و`oem` عامة. | لا نضيف Python/libusb إلى WPF، ولا ننفذ OEM command عامة. لا يسد حاجة Huawei دون profile/fixtures خاصة. |
| [BenutzerEinsZweiDrei/huawei-fastboot-commands](https://github.com/BenutzerEinsZweiDrei/huawei-fastboot-commands/blob/58cbe29ed7d22b21eaf5568a40e6f50dafaff497/README.md)، `58cbe29ed7d22b21eaf5568a40e6f50dafaff497`، لا ترخيص معلن | قائمة أوامر تجمع معلومات مع flash/unlock/FRP وخدمة؛ لا برنامج ولا tests ظاهرة. | مرجع بحث للأسماء فقط، لا مصدر تنفيذ أو دليل أمان. |
| [FrgfA4ftfzTdfyyyr5f6tESD3/huawei_kirin_toolkit](https://github.com/FrgfA4ftfzTdfyyyr5f6tESD3/huawei_kirin_toolkit/tree/c43ea5c670f25580b2703a51d26cf06d8eaeff2b)، `c43ea5c670f25580b2703a51d26cf06d8eaeff2b`، لا ملف ترخيص معلن | README يصف Device Read Info بأنه parsing لـ`fastboot getvar all` مع ADB hardware identification. المستودع يجمع ذلك مع unlock/flash/FRP/repair، ولم يظهر ملف اختبارات عند SHA المراجع. | استفدنا من تقسيم Fastboot facts عن خصائص ADB، ولم نستخدم `getvar all` لأنه dump واسع قد يتضمن serial أو بيانات تعريفية، ولم ننقل أي كود بسبب غياب الترخيص والنطاق المختلط. |
| [kayger44/MEDIATAB-T5-FRP-BYPASS-VERSION.8.0.0](https://github.com/kayger44/MEDIATAB-T5-FRP-BYPASS-VERSION.8.0.0)، صفحة README حالية، لا ترخيص معلن | يعرض مثال خرج فعلي لـ`fastboot oem get-product-model` مثل `(bootloader) AGS2-L09`، لكنه مستودع تعليمات bypass/FRP ويعرض كذلك أوامر حساسة مثل `get-psid`. | استعرنا فقط اسم أمر قراءة الموديل وصيغة الرد كـfixture معزول؛ لا نقل كود. الأمر مثبت على مثال Huawei واحد فقط وليس ضمان توافق عام. |
| [Uotan-Dev/FirmwareKit.Comm.Fastboot](https://github.com/Uotan-Dev/FirmwareKit.Comm.Fastboot/tree/9e3740bad6e82d42c0eb1769f360df7640bc2953)، `9e3740bad6e82d42c0eb1769f360df7640bc2953`، MIT | مكتبة .NET حديثة نسبياً، CI وملفات اختبار transport/`getvar`؛ سجل commit يذكر 251 اختباراً منها 36 متخطاة. ملف تعريف Huawei يذكر VID `0x12D1` لكن الاختبارات/القواعد لا تثبت أوامر Huawei الخاصة. | مرجع إعادة استخدام جيد لأنماط transport والاختبار فقط؛ لم نضف dependency لأنه يعيد تنفيذ Fastboot ويشمل flash/unlock، وهدفنا الحالي هو CLI الحالي وقائمة قراءة ضيقة. نعيد تقييمه لاحقاً إن احتجنا USB discovery مستقل. |

لم يُنسخ كود من أي من هذه المشاريع. لا تُفعّل أوامر OEM عامة؛ الاستثناء المقيد أدناه هو استعلام واحد ثابت للموديل مستوحى من أمثلة Huawei.

**التنفيذ الحالي، 16 نوفمبر 2026:** أضيف اختيار profile صريح بين `Android Fastboot` و`Huawei Fastboot (experimental read-only)`. Huawei يقرأ الأربع العامة، و`getvar rescue_version` و`getvar vendorcountry`، ثم أمراً واحداً ثابتاً هو `oem get-product-model`. يقتصر parser على token موديل محدود الأحرف/الطول ويعرضه كـ`oem.get-product-model`. أسماء أوامر/شكل الرد مستوحاة من أمثلة GitHub أعلاه؛ لم يُنسخ كود. ملفا قراءة `getvar all` الكامل غير مستخدمين. لا ترخيص لأحد مراجع الأوامر، لذلك لا يُعاد استخدام كود منها؛ المستودع المرخص MIT راجعناه كمرجع transport/tests فقط ولم نضف اعتماداً واسعاً. هذه الأوامر لا تضمن توافق كل موديل؛ الرفض والغياب يظهران `PermissionDenied` أو `NotReported` ويستمر باقي الملف.

لا ينفذ الملف `getvar all` أو أي `oem` عام، ولا يستعلم `rescue_phoneinfo` أو serial/IMEI/PSID، ولا يغير الوضع أو يكتب إلى الهاتف. الاستثناء هو أمر قراءة موديل واحد ثابت. اختيار Huawei يدوي وليس اكتشافاً تلقائياً للمصنع ولا تأكيد توافق؛ يجب التحقق لاحقاً على موديل وإصدار موثقين. لا يوجد جهاز Huawei محدد حالياً، لذا لا ندعي اختباراً فعلياً أو دعم إصلاح.

## حدود التنفيذ الناتج

- اكتشاف `fastboot.exe` من `ANDROID_HOME` و`ANDROID_SDK_ROOT` ومجلد SDK المعتاد و`PATH`، دون تنزيل أدوات أو تعديل PATH.
- تعداد `fastboot devices` وعرض قائمة اختيار. لا ينتقي التطبيق جهازاً تلقائياً.
- إرسال الاستعلام مع `-s <serial>` بعد التحقق من شكل serial المستخرج من التعداد.
- الاستعلام المتسلسل عن الأربع العامة (`is-userspace`, `product`, `current-slot`, `unlocked`)؛ ويضيف ملف Huawei وحده `rescue_version` و`vendorcountry` ثم `oem get-product-model` الثابت. لا يوجد `getvar all` أو OEM عام أو انتقال وضع أو كتابة.
- قراءة stdout وstderr معاً، و15 ثانية لكل عملية، وإلغاء يوقف شجرة العملية، وحد 64 KiB للمخرج.
- تصنيف Fastbootd فقط إذا كانت `is-userspace=yes`، وBootloader Fastboot فقط مع `no`. الغياب ينتج `NotReported` ووضعاً غير مصنف.
- يسجل `AuditStore` التعداد والقراءة تحت عمليتي `device.discover` و`device.read_info`. لا يضع serial أو مخرجات Fastboot في ملخص التدقيق.

## تحقق الخطوة

تغطي اختبارات `FastbootOutputParserTests` التعداد وصيغ stdout/stderr والقيم الغائبة ورفض المورد وتصنيف Fastbootd، وتثبت الفصل بين allowlist العام وHuawei، وقبول متغيري Huawei المحددين فقط، وتحليل صيغة موديل مثل `(bootloader) AGS2-L09`، ورفض الأوامر والحقول التعريفية الأخرى. fixtures تثبت parser فقط ولا تمثل اختبار توافق firmware أو جهاز Huawei. نتيجة التشغيل والبناء تسجل في [`TODO.md`](TODO.md) و[`22-HANDOFF.md`](22-HANDOFF.md).

لم ينفذ اختبار Huawei فعلي: لا يوجد جهاز Huawei محدد الآن. عند توفر جهاز، سجّل الموديل ورقم البناء وإصدار EMUI/HarmonyOS والمنطقة/CUST وحالة الظهور في `fastboot devices`، ثم تحقق أن القيم غير الحساسة تعمل أو تظهر `NotReported`. لا تطلب من التطبيق نقل الهاتف إلى Fastboot تلقائياً.

## Follow-up: Huawei read-info sample, 16 November 2026

The user supplied a PPAH/PPA sample from another utility. GitHub command references and a Huawei read-info script corroborate fixed queries for `get-product-model`, `get-build-number`, `oeminforead-ANDROID_VERSION`, `get_hwnff_ver`, `get_key_version`, `lock-state info`, and `getvar vendorcountry` / `rescue_version`. The version-specific `BASE_VERSION`, `CUSTOM_VERSION`, and `PRELOAD_VERSION` names are corroborated by [Huawei Fastboot examples](https://4pda.to/forum/index.php?showtopic=932930&st=1120) and [Huawei support logs on GitHub](https://github.com/ProfessorJTJ/HISuite-Proxy/issues/199). Sources: [Huawei Fastboot command list](https://github.com/BenutzerEinsZweiDrei/huawei-fastboot-commands/blob/58cbe29ed7d22b21eaf5568a40e6f50dafaff497/README.md) and [Huawei read-info script](https://gitee.com/mouzei/HUAWEIFastbootReadInfo/blob/master/run.bat). These references mix safe reads with risky commands, so no script or code is reused; only an explicit read allowlist is used.

The Huawei profile now reads those fixed version/model/lock fields, including base, custom, and preload versions, in addition to the existing safe `getvar` allowlist. Unsupported OEM reads are recorded as `NotReported` so one unsupported query does not discard earlier successful values. It deliberately omits PSID, broad `oeminforead-hotainfo`, raw dongle data, flashing, unlock, and mode transitions.

### Hardware confirmation, 16 November 2026

A Huawei device appeared in `fastboot devices`. Using explicit `-s` targeting, the fixed read queries returned the same device and firmware family as the user's external report: product model `PPA-LX2`, equipment model `PPAH-L22` derived from the build-version prefix, build `10.1.1.280(C185E8R2P1)`, base `PPAH-LGRP2-OVS 10.1.1.280`, custom `PPAH-L22-CUST 10.1.1.8(C185)`, preload `PPAH-L22-PRELOAD 10.1.1.1(C185R2)`, Android `10`, HWNFF `Ver3.0`, key `huawei_key_v1`, vendor/country `hw/meafnaf`, rescue `rescue0.7`, and both FB and USER lock states `LOCKED`. The common Android `getvar` fields returned `Command not allowed`, which the UI should report as unavailable/permission denied while retaining the Huawei OEM results. The serial was not recorded.

The executable command checks and parser tests covered all values above. Focused parser tests passed 36/36, and the desktop Release build passed with 0 warnings and 0 errors. UIA/Computer Use could not connect to the Windows host, so button-click and on-screen rendering were not verified in this run. Dongle information and the external utility's version/elapsed time are not handset Fastboot facts and remain excluded.
