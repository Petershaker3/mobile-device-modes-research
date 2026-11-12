# دراسة الأدوات الجاهزة والسوق وتجارب المستخدمين

> يجمع هذا الملف الأدوات مفتوحة المصدر والتجارية، مراجعة الكود، أدلة السوق والمجتمع، ومصفوفة الاختبار المقارن.

## 8. مرحلة البحث عن برامج جاهزة (Ready-Made Software Discovery)

**تغيير هدف المرحلة:** الأولوية الآن ليست جمع مكتبات لبناء برنامج جديد، بل العثور على برامج Windows جاهزة تنفذ قراءة معلومات الهاتف واكتشاف أوضاع الاتصال والتحويل بينها، أو تقدم وظائف أوسع. هذه القائمة نتائج بحث أولية وليست اعتماداً نهائياً أو توصية شراء.

### 8.1 أدوات مفتوحة المصدر على GitHub

| الأداة | النطاق المعلن | مدى قربها من الهدف | ما يحتاج التحقق |
|---|---|---|---|
| [ATA-GUI](https://github.com/msartore/ATA-GUI) | Windows/Linux/macOS؛ ADB وFastboot وSideload وLogcat وScrcpy وإدارة التطبيقات وقراءة معلومات الجهاز في System/Fastboot؛ مبنية حالياً على Tauri 2 + Rust + React | مرشح قوي كواجهة عامة مستقرة نسبياً، وليس حلاً مثبتاً لكل أوضاع الشركات | فحص الإصدارات والـ Releases، آلية تنزيل Platform Tools، تعدد الأجهزة، وغياب Samsung Download/Odin وXiaomi Mi Assistant من قائمة الميزات المعلنة |
| [VioletToolBox](https://github.com/Smart-Paocai/VioletToolBox) | Windows/.NET 8؛ يعلن نحو 110 وظائف تشمل ADB/Fastboot وScrcpy وقراءة/كتابة/استخراج أقسام، وعمليات OPLUS وRecovery/TWRP وأدوات Firmware | من أوسع المشاريع المفتوحة التي ظهرت في البحث، ويستحق أولوية عالية للفحص | توجد وظائف EDL/Qualcomm ضمن المنتج؛ تُستبعد من تقييمنا. يجب اختبار الوظائف المتبقية، ومراجعة Release package والاعتماد على الخدمات السحابية ومصادر ROM |
| [Android Multitool Universal](https://github.com/auratech0/android-multitool-universal) | Python/Tkinter؛ قراءة getprop وFastboot getvar، نسخ احتياطي، إدارة التطبيقات، Logcat، Flash وWireless ADB | يغطي وظائف ADB/Fastboot العامة في واجهة واحدة | مشروع صغير؛ التحقق من آخر تحديث، جودة الكود، توافق Windows، وإمكانية التعامل مع أكثر من جهاز |
| [ADB GUI](https://github.com/RookieJay/AdbGui) | Windows-first؛ USB/Wireless ADB، اكتشاف الأجهزة، معلومات النظام، إدارة التطبيقات، تقارير تشخيصية وScreenshots | مناسب كأداة قراءة وتشخيص ADB جاهزة، لكنه ليس أداة أوضاع خدمة متعددة | المشروع يعلن أن بعض الوظائف المتقدمة مؤجلة؛ التحقق من آخر Release وتشغيل النسخة |
| [Android Toolbox GUI](https://github.com/fatalvirus76/Android_Toolbox_GUI) | واجهة تجمع ADB وFastboot وOdin/Heimdall وScrcpy وأدوات تفليش أخرى | تغطية واسعة من حيث جمع الأدوات | المشروع يضم مكونات خارج نطاقنا، ويجب التحقق من Windows support الفعلي ومن أن كل أداة مدمجة/مرفقة وليست مجرد أوامر أو مسارات خارجية |
| [QADB](https://github.com/ludoven/QADB) | ADB GUI متعددة الأنظمة؛ معلومات الجهاز، إدارة التطبيقات، Logcat، Terminal، ملفات ومرايا شاشة | قوي كأداة ADB عامة للمطورين والفنيين | لا يعلن دعم Download Mode أو MTP أو بروتوكولات الشركات؛ ليس مرشحاً وحيداً |
| [ADBFastbootGUI](https://github.com/Tentex1/ADBFastbootGUI) | واجهة ADB/Fastboot مع معلومات الجهاز وملفات وApps وScrcpy وSideload | مرشح إضافي لوظائف ADB/Fastboot | فحص النضج، الإصدارات، وحالة الصيانة والتوافق |

**ملاحظة على حجم المجتمع:** عدد Stars/Forks مؤشر اهتمام فقط، وليس دليلاً على سلامة البرنامج أو نجاح وظائفه. مثال: VioletToolBox ظهر في صفحة GitHub بحوالي 801 Star و73 Fork وقت البحث؛ لكن يلزم فحص سجل الإصدارات والـ Issues وتجربة البرنامج قبل اعتباره جاهزاً للاستخدام.

### 8.2 أدوات تجارية / مغلقة المصدر تستحق المقارنة

هذه الأدوات قد تكون أقرب لما يستخدمه فني الصيانة فعلياً من مشاريع GitHub الصغيرة. إدراجها هنا لأجل تقييم الوظائف والتجارب، وليس لتقييم التراخيص أو الأسعار.

| الأداة | الخصائص ذات الصلة بنطاقنا | الملاحظات الأولية |
|---|---|---|
| [SamFw Tool](https://samfw.com/blog/SamFwTool) | Samsung MTP Read Info، ADB Read Info، Samsung Download Mode، معلومات/إجراءات Xiaomi Sideload، وعمليات Android عامة | مرشح مهم جداً لأن سجل التحديثات يذكر صراحة قراءة معلومات MTP، وقراءة معلومات Samsung، وXiaomi Sideload. يجب فصل الوظائف التي ما زالت موجودة في الإصدار الحالي عن سجل الميزات التاريخي، والتحقق من كل وضع على حدة |
| [Chimera Tool](https://www.chimeratool.com/) | أداة صيانة متعددة العلامات؛ Device Info في Download Mode لسامسونج، وعمليات ADB/USB وأوضاع أخرى حسب الجهاز | لديها قسم دعم نشط على GSMHosting وتقارير مستخدمين فعلية؛ توجد أيضاً شكاوى موثقة عن عدم اكتشاف بعض الأجهزة وأخطاء USB وتغييرات سياسات الاستخدام |
| [UnlockTool](https://www.unlocktool.net/) | أداة Windows متعددة العلامات؛ Read Info وعمليات Download Mode لبعض أجهزة Samsung، وعمليات ADB/Fastboot وأوضاع خدمة أخرى حسب الموديل | سجل تحديث نوفمبر 2026 يثبت استمرار التطوير؛ يجب استخراج ما يتعلق بقراءة المعلومات واكتشاف الأوضاع فقط، دون التوسع في أوضاع Qualcomm/MediaTek |
| [Android Multi Tool (AMT)](https://forum.gsmhosting.com/vbb/f1141/) | منصة صيانة متعددة العلامات؛ منتدى دعم مخصص وتقارير نجاح وفشل من المستخدمين | المنتدى يعرض تحديثات وتقارير تشغيل ومشاكل Server Offline وعمليات فاشلة؛ يستحق فحصاً منفصلاً للخصائص العامة وموثوقية الاتصال |
| [Android Root Tool (ART)](https://forum.gsmhosting.com/vbb/f1134/) | سجل التحديثات يذكر Xiaomi Mi Assistant Sideload وقراءة Device Info، إلى جانب ADB وإدارة التطبيقات وأدوات Android | مرشح مهم لمسار Xiaomi Mi Assistant تحديداً؛ يجب التحقق مما إذا كانت قراءة المعلومات مستقلة عن عمليات الصيانة الأخرى |
| [DFT Pro](https://forum.gsmhosting.com/vbb/f1092/) | أداة متعددة العلامات وتذكر إصلاحات ADB ومهام قراءة/صيانة حسب التحديثات | الدليل المتاح حالياً لا يكفي لإثبات أنها أفضل أداة لاكتشاف الأوضاع؛ نحتاج تجميع تجارب المستخدمين حسب الموديل |

### 8.3 استنتاج أولي من بحث البرامج الجاهزة

- **SamFw Tool** هو أقرب مرشح ظهر حتى الآن لتغطية مجموعة الأوضاع التي تهمنا في منتج واحد: Samsung MTP/ADB/Download Mode وXiaomi Sideload، إضافة إلى Android ADB.
- **Chimera Tool وUnlockTool وAMT** مهمون لأنهم مستخدمون في سوق الصيانة ولديهم مجتمعات دعم وتقارير حقيقية، لكن يجب فصل دعم قراءة المعلومات عن وظائف الإصلاح والتفليش.
- **VioletToolBox** هو مرشح GitHub الأوسع من حيث وظائف Android/Firmware العامة، لكن بعض مساراته خارج النطاق الحالي، ولا يوجد دليل حتى الآن أنه بديل كامل لأدوات Samsung Download Mode وXiaomi Mi Assistant.
- **ATA-GUI** مرشح مناسب جداً لواجهة ADB/Fastboot حديثة، لكنه لا يغطي وحده كل بروتوكولات الشركات المطلوبة.
- لا يوجد حتى الآن دليل كافٍ على وجود مشروع GitHub مفتوح المصدر واحد يجمع بشكل مؤكد: ADB + Fastboot/Fastbootd + MTP/WPD + Samsung AT/Download Mode + Xiaomi Mi Assistant + انتقالات الأوضاع عبر الشركات.

---

## 9. بحث آراء وتجارب المستخدمين (Community & Field Feedback)

**مصادر البحث الأولية:** GSMHosting، Martview، Reddit، صفحات الدعم الرسمية وسجلات تحديثات الأدوات. يجب التعامل مع منشورات فرق الأدوات على أنها ادعاءات/إعلانات منتج، ومع منشورات المستخدمين على أنها تجارب فردية لا تمثل بالضرورة كل الأجهزة.

### 9.1 GSMHosting — نتائج وتجارب موثقة

1. **Chimera Tool — وجود ميزة قراءة فعلية في Download Mode:** في موضوع Samsung Exynos، نشر مستخدم تقرير Device Info لجهاز Galaxy M34 5G SM-M346B2 يتضمن Model وCSC وAP/BL/CP وIMEI وSerial وAndroid version وFRP status وUSB mode. هذا دليل عملي على أن الأداة تستطيع استخراج مجموعة معلومات مهمة من بعض أجهزة Samsung في سياق Download Mode/USB، وليس مجرد إعلان ميزة. في نفس التقرير فشلت عملية أخرى رغم نجاح Device Info؛ أي إن نجاح القراءة لا يعني نجاح كل عملية صيانة.  
   المصدر: [GSMHosting — Samsung Exynos Download Mode thread](https://forum.gsmhosting.com/vbb/f898/10-11-2026-extended-samsung-exynos-remove-frp-solution-download-mode-2819592/index194.html).

2. **Chimera Tool — مشاكل التوافق والاتصال:** في نقاش تحديث 2024، أبلغ مستخدمون عن USB communication timeout، وعدم تعرف الأداة على بعض أجهزة Xiaomi/CMF في وضع معين، ومشاكل في السجل والواجهة. هذه شكاوى مرتبطة بإصدار وسياق محددين وليست حكماً بأن الإصدارات الحالية تعاني منها، لكنها تؤكد ضرورة اختبار كل وضع/موديل وعدم الاكتفاء بجدول الدعم.  
   المصدر: [GSMHosting — Chimera update feedback](https://forum.gsmhosting.com/vbb/f898/06-11-2024-new-chimera-new-mtk-support-3346127/).

3. **Android Multi Tool (AMT) — مشاكل الخدمات والاعتمادية:** في منتدى الدعم ظهرت بلاغات عن Server Offline، وعدم القدرة على فتح/الاتصال بالخادم، وشكاوى من خصم Credits حتى عند فشل القراءة أو التفليش، إضافة إلى انتقاد ضعف معلومات ساعات عمل الخادم والدعم. هذه نقطة مهمة لأن الأداة قد تكون قوية وظيفياً لكن الاعتماد على خادم خارجي قد يعطل بعض العمليات.  
   المصدر: [GSMHosting — AMT official support](https://forum.gsmhosting.com/vbb/f1141/welcome-android-multi-tool-amt-official-support-section-3192481-print/).

4. **تعدد الأدوات في الاستخدام اليومي:** في سؤال فني عن أفضل أداة لمهام FRP، ذكر صاحب السؤال أنه يستخدم UnlockTool وOctoplus وChimera معاً، ومع ذلك لا يستطيع حل كل الأجهزة؛ الردود اقترحت أدوات إضافية مثل Android Multi Tool وDFT Pro. رغم أن السؤال متعلق بـFRP وليس Read Info تحديداً، فهو مؤشر على أن الفنيين لا يعتمدون عادة على برنامج واحد لكل الموديلات والمهام.  
   المصدر: [GSMHosting — FRP, best tools?](https://forum.gsmhosting.com/vbb/f606/frp-best-tools-3409533/).

5. **أهمية سجل الجهاز والتوافق الدقيق:** نقاشات Chimera تتضمن طلبات دعم موديلات محددة وتقارير نجاح وفشل مفصلة مع إصدار الأداة وWork ID. لذلك ينبغي في المقارنة النهائية حفظ الموديل الكامل والـ firmware/build وإصدار الأداة والوضع ونتيجة القراءة، وليس الاكتفاء بعبارة "يدعم Samsung".

### 9.2 Martview ومجتمعات أخرى

- ظهر نقاش مقارنة بين UnlockTool وChimera وOctoplus وPandora وCheetah، وركز على سهولة الاستخدام والتغطية والسعر وتواتر التحديث والدعم. المشاركون وصفوا UnlockTool بأنه قوي في تغطية بعض العلامات الصينية وقيمة مقابل السعر، لكنه أكثر تقنية للمبتدئ. هذا رأي مشارك/مقارنة تجارية، وليس اختباراً مستقلاً لميزة Read Info.  
  المصدر: [Martview — UnlockTool vs Other GSM Tools](https://www.martview-forum.com/threads/unlocktool-vs-other-gsm-tools-%E2%80%93-2026-full-video-comparison.170894/).

### 9.3 Reddit ومراجعات السوق

- نقاش Reddit حول أدوات Android يذكر Chimera وSamFW في سياق حلول FRP، ويشير إلى أن طريقة الحصول على تفعيل UnlockTool تمر عبر موزعين/Resellers، مع تجربة إيجابية فردية مع أحدهم. لا يقدم النقاش دليلاً فنياً على جودة Read Info، لكنه مفيد في فهم تجربة المستخدم مع التفعيل والتوزيع.  
  المصدر: [Reddit — Best FRP tool for androids](https://www.reddit.com/r/FRPtools/comments/1m0h1o1/best_frp_tool_for_androids_paid_or_free/).

### 9.4 ما الذي تعنيه هذه التجارب للقرار؟

1. يجب تقييم **Device Detection** و**Read Info** و**Mode Switching** كوظائف منفصلة؛ لا تعتبر نجاح واحدة إثباتاً للأخريين.
2. يجب تسجيل الاعتماد على الإنترنت والخوادم والـ Credits كعامل اعتمادية، حتى لو لم يكن السعر أو الترخيص ضمن معايير الاستبعاد.
3. يجب التفريق بين بيانات يقرأها البرنامج محلياً عبر USB وبين معلومات يجلبها من Server أو قاعدة بيانات.
4. يجب إعطاء وزن أكبر لتقارير الفنيين التي تتضمن موديل الجهاز وإصدار النظام ونسخة الأداة والـ logs، وأقل للمنشورات الترويجية العامة.
5. لا نستنتج أن الأداة غير صالحة بسبب شكوى واحدة، ولا أنها ممتازة بسبب تقرير نجاح واحد. نحتاج نمطاً متكرراً عبر عدة تقارير وموديلات.

---

## 10. المرحلة التالية من البحث

- [ ] فحص مستودعات GitHub المرشحة من الداخل: README، Releases، آخر Commit، Issues، بنية الكود، الاعتماد على أدوات خارجية أو خدمات Server.
- [ ] تنزيل/تشغيل البرامج المفتوحة المصدر في بيئة Windows معزولة عند توفر ملفات Releases، وتسجيل ما أمكن اختباره دون توصيل هاتف أو تنفيذ عمليات كتابة.
- [ ] فحص SamFw Tool وChimera وUnlockTool وAMT وART وDFT Pro من مصادرها الرسمية/منتديات الدعم، والتركيز على قراءة المعلومات واكتشاف الأوضاع والتحويل بينها.
- [ ] جمع عينة أكبر من تجارب GSMHosting وMartview وReddit ومجموعات/قنوات عامة يمكن الوصول إليها، مع فصل الإعلان عن تجربة المستخدم.
- [ ] إنشاء جدول مقارنة نهائي يميز: مؤكد من الكود، معلن من المطور، شوهد في تجربة مستخدم، يحتاج اختباراً عملياً.
- [ ] عدم بدء بناء أداة جديدة قبل تحديد أفضل برنامج جاهز، وما الذي ينقصه تحديداً.

**حالة هذه المرحلة:** بحث استكشافي أولي عن البرامج الجاهزة وآراء المستخدمين. لم يتم اعتماد أداة نهائية، ولم يتم اختبار أي أداة على أجهزة فعلية ضمن هذه المرحلة.


---

## 11. تدقيق داخلي أعمق لمرشحي GitHub (حتى 12 نوفمبر 2026)

### 11.1 ATA-GUI

**التصنيف:** أفضل مرشح مفتوح المصدر لواجهة ADB/Fastboot/Recovery العامة، وليس أداة خدمة شاملة.

ما تأكد من README وFEATURES وARCHITECTURE:
- قراءة بيانات System وFastboot: manufacturer/model/board/device وAndroid version وCPU ABI وIP ووسيلة النقل والمستخدم النشط.
- ADB وFastboot وADB sideload وRecovery OTA sideload واستخراج partition images.
- إدارة APK، Logcat/GetEvent، Wireless ADB/QR pairing، Scrcpy، Terminal.
- دمج اتصالات USB والشبكة في صف واحد لكل هاتف باستخدام ro.serialno.
- لا يعلن دعم MTP/WPD أو Samsung AT Modem أو Samsung Odin/Download Mode أو Xiaomi Mi Assistant كواجهات مستقلة.

الإصدارات:
- v4.0.0 صدر في 1 نوفمبر 2026، وv4.0.2 ظهر كـPre-release في 4 نوفمبر.
- v4.0 إعادة كتابة كاملة باستخدام Tauri 2 + Rust + React. فرع legacy يحتوي مصدر 3.x WinForms، و3.x لم يعد يتلقى إصلاحات بحسب صفحة الإصدار.
- صفحة Releases تميز v4.0.2 التجريبي عن v4.0.0 Latest، وتوضح أن مستخدمي 3.x لا تتم ترقيتهم تلقائياً.
- Platform Tools وScrcpy يتم تنزيلهما عند أول تشغيل. README يذكر عدم وجود حساب أو Telemetry.
- المشروع يذكر 42 اختباراً في سجل التغييرات الذي تمت مراجعته، لكن لم يتم اختبار أجهزة فعلية في هذه المراجعة.

**الحكم:** ممتاز كواجهة عامة لـADB/Fastboot/Recovery، لكنه لا يغطي البروتوكولات الخاصة المطلوبة. يجب اختبار 4.0.0 المستقر منفصلاً عن 4.0.2 التجريبي.

المراجع: [README](https://github.com/msartore/ATA-GUI), [Releases](https://github.com/msartore/ATA-GUI/releases), [FEATURES](https://github.com/msartore/ATA-GUI/blob/main/docs/FEATURES.md), [ARCHITECTURE](https://github.com/msartore/ATA-GUI/blob/main/docs/ARCHITECTURE.md).

### 11.2 VioletToolBox

**التصنيف:** أوسع مرشح GitHub مفتوح المصدر ظهر من حيث وظائف Android/OPLUS/Xiaomi، لكنه ليس بديلاً مثبتاً لأدوات Download Mode المتخصصة.

الخصائص المعلنة:
- Windows 10/11، واجهة WPF/.NET، نحو 110 وظائف.
- اكتشاف حالة الجهاز، Wireless Debugging، Scrcpy.
- ADB/Fastboot، قراءة معلومات النظام، قراءة جداول الأقسام، وعمليات نسخ/قراءة/كتابة/مسح الأقسام.
- Fastbootd وTWRP وXiaomi Fastboot وOPPO/OnePlus/Realme workflows وOTA/Payload وإدارة التطبيقات والملفات والنسخ الاحتياطي.
- المشروع يضم أيضاً Qualcomm EDL؛ هذا الجزء خارج نطاق تقييمنا ولا يدخل في نقاط المقارنة.
- README لا يقدم دليلاً كافياً على Samsung Download Mode/Odin-Loke أو Samsung AT Modem أو Xiaomi Mi Assistant Sideload كمسارات مستقلة. لا نستنتج ذلك من وجود Xiaomi Fastboot أو تغيير وضع نقل الملفات.

حالة الصيانة والمشكلات:
- أحدث Commit تمت مراجعته بتاريخ 11 نوفمبر 2026، وشمل دمج إصلاح UTF-8 وإضافة ترجمة يابانية.
- Issue #8 عن ظهور نصوص ADB/Fastboot الصينية كرموز غير مفهومة أُغلق في 11 نوفمبر 2026، ودمج الإصلاح في Commit [8f8094f](https://github.com/Smart-Paocai/VioletToolBox/commit/8f8094fee8ce2a7f11a9f9148352668db7c386a2).
- Issue #4 ما زال مفتوحاً عند الفحص في 12 نوفمبر 2026: مستخدم Xiaomi Pad 7 Ultra/XRING O1 أبلغ أن وظيفة Backup Baseband فشلت بسبب hard-coded partition path وهو /dev/block/bootdevice/by-name، بينما المسار الصحيح على الجهاز /dev/block/by-name. كما أن معالجة الخطأ لم تلتقط عبارة No such file or directory. المشكلة تخص وظيفة النسخ الاحتياطي المشار إليها، ولا تثبت فشل كل وظائف البرنامج.
- المشروع يطلب .NET 8 أو أحدث. انتهى دعم .NET 8 رسمياً في 10 نوفمبر 2026. يجب التأكد من أن البناء/التشغيل يستهدف Runtime مدعوماً مثل .NET 10 أو أن التطبيق self-contained ومبني بطريقة لا تعتمد على Runtime غير مدعوم. انتهاء دعم Runtime لا يعني أن البرنامج توقف عن العمل، لكنه عامل صيانة وأمان.

**الحكم:** يستحق تجربة Windows مبكرة بعد مراجعة Release الفعلي، مع تدقيق عمليات الأقسام ومصادر تنزيل الملفات ومتطلبات الشبكة ونسخة .NET المستهدفة. لا نعتبره حلاً شاملاً حتى نثبت دعم Samsung Download/AT وXiaomi Mi Assistant.

المراجع: [Repository](https://github.com/Smart-Paocai/VioletToolBox), [English feature list](https://github.com/Smart-Paocai/VioletToolBox/blob/main/docs/README.en.md), [Issue #4](https://github.com/Smart-Paocai/VioletToolBox/issues/4), [Issue #8](https://github.com/Smart-Paocai/VioletToolBox/issues/8), [Microsoft .NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy).

### 11.3 QADB

- الإصدار الموثق في README هو v2.0.7 بتاريخ 29 مايو 2026.
- يدعم Windows وmacOS، ويحتوي ADB مدمجاً، واكتشاف أجهزة USB/Network ADB، ومعلومات الجهاز، وإدارة التطبيقات، Logcat، Terminal، Screenshots/Recording وCommand Center.
- Fastboot وMTP وSamsung Download Mode وXiaomi Mi Assistant ليست ضمن قائمة الوظائف المعلنة.
- توجد Issues عن فشل تهيئة ADB، وعدم اكتشاف ADB الافتراضي، ومشاكل تشغيل Windows 11؛ وهي بلاغات تحتاج قراءة تفاصيل الحلول قبل تقدير أثرها.

**الحكم:** أداة ADB مساعدة وليست منافساً مباشراً لأداة متعددة الأوضاع.

المراجع: [QADB](https://github.com/ludoven/QADB), [Issues](https://github.com/ludoven/QADB/issues), [Releases](https://github.com/ludoven/QADB/releases).

### 11.4 ADB GUI من RookieJay

- README يصف v1 بأنه مكتمل، لكن Shell وLogcat وFile push/pull وScrcpy وعمليات النظام المتقدمة مؤجلة.
- الوظائف الحالية: USB/Wireless ADB، تعدد الأجهزة، معلومات model/version/SDK/ABI/serial/resolution، تقارير TXT، Screenshots وإدارة التطبيقات.
- يتطلب JDK 21 وADB على PATH؛ لا يشحن ADB في v1.
- لا يعلن واجهات Fastboot/MTP/Download Mode.

**الحكم:** مرجع لتصميم شاشة معلومات وتشخيص ADB فقط.

المصدر: [AdbGui](https://github.com/RookieJay/AdbGui).

### 11.5 Android Multitool Universal

- تطبيق Python/Tkinter يعمل على Windows/Linux/macOS ويستخدم Android Platform Tools الموجودة مسبقاً على PATH.
- يعلن Device Info من getprop وfastboot getvar، Reboot إلى system/recovery/bootloader/sideload، Wireless ADB، Logcat، نسخ احتياطي، إدارة تطبيقات وعمليات Flash.
- لا يعلن اكتشاف MTP/WPD أو Samsung Download Mode أو Xiaomi Mi Assistant.
- المستودع صغير ولا يقدم README حزمة Windows Release جاهزة موثقة؛ التشغيل يتطلب Python وPlatform Tools.

**الحكم:** نموذج أولي مفيد لواجهة ADB/Fastboot، وليس مرشحاً جاهزاً رئيسياً لفني صيانة.

المصدر: [Android Multitool Universal](https://github.com/auratech0/android-multitool-universal).

### 11.6 ADBFastbootGUI وAndroid Toolbox GUI

**ADBFastbootGUI**
- تطبيق Windows WPF يستهدف .NET Framework 4.8.
- ADB وFastboot وScrcpy ومعلومات الجهاز وApps وFiles وReboot وSideload وPartition management.
- يتطلب USB debugging في ADB وتعريفات OEM؛ لا توجد وظائف MTP/WPD أو Download Mode مستقلة موثقة.
- Sideload هنا يعني OTA sideload القياسي، وليس Xiaomi Mi Assistant.

المصدر: [ADBFastbootGUI](https://github.com/Tentex1/ADBFastbootGUI).

**Android Toolbox GUI**
- يجمع ADB/Fastboot وOdin/Heimdall وScrcpy وواجهات لأدوات Flash أخرى.
- README يصف Odin4 بأنه أداة Linux، ويعتمد على برامج خارجية مثل Platform Tools وHeimdall وScrcpy؛ لذلك يلزم التحقق من قابلية تشغيل كل مكون على Windows قبل اعتباره برنامج Windows متكاملاً.
- وجود Odin/Heimdall ضمن القائمة لا يثبت قراءة معلومات Download Mode؛ الوصف يركز أساساً على التفليش وإدارة PIT.

المصدر: [Android Toolbox GUI](https://github.com/fatalvirus76/Android_Toolbox_GUI).

### 11.7 الترتيب المؤقت لمرشحي GitHub

| الترتيب | المشروع | ما يستحق التجربة | سبب عدم اعتباره حلاً كاملاً |
|---:|---|---|---|
| 1 | VioletToolBox | أوسع وظائف Android/OPLUS/Xiaomi وقراءة معلومات وأقسام | لا دليل على Samsung Download/AT أو Mi Assistant؛ Issue #4 مفتوح؛ .NET 8 EOL |
| 2 | ATA-GUI 4.x | واجهة حديثة لـADB/Fastboot/Recovery وWireless ADB | لا MTP/WPD ولا بروتوكولات Samsung/Xiaomi الخاصة؛ 4.0.2 Pre-release |
| 3 | Android Toolbox GUI | تجميع أدوات متعددة مع Odin/Heimdall | تكامل الأدوات وWindows support يحتاجان اختباراً |
| 4 | QADB | ADB متكامل نسبياً مع ADB مدمج | لا Fastboot أو أوضاع الشركات |
| 5 | ADBFastbootGUI | وظائف Windows ADB/Fastboot | لا بروتوكولات Download الخاصة |
| 6 | Android Multitool Universal | نموذج Python سريع للقراءة والتحكم العام | لا حزمة جاهزة أو دعم أوضاع خاصة مثبت |
| 7 | RookieJay/AdbGui | معلومات وتقارير ADB | v1 محدود والميزات المتقدمة مؤجلة |

---

## 12. تدقيق أدوات السوق وتجارب المستخدمين — تحديث 12 نوفمبر 2026

### 12.1 SamFw Tool 6.2

الميزات التي يمكن إثباتها من سجل المطور:
- Samsung MTP: Read Info، وإضافات لاحقة لإظهار Android version وFRP status ثم IMEI2 وSecurity Patch Level.
- Samsung ADB: قراءة معلومات وبيانات البطارية وإدارة التطبيقات.
- Samsung Download Mode: يوجد دعم للخروج من Download Mode وإصلاح بعض حالات Soft Brick، وعمليات Download Mode أخرى في سجل الإصدارات؛ لكن لا نخلط ذلك مع إثبات واجهة عامة لقراءة كل بيانات الجهاز من Download Mode.
- Xiaomi Sideload: سجل التطوير يذكر Read offline MiCloud information، وليس بالضرورة Read Info عاماً لكل بيانات الهاتف.
- Android ADB: معلومات البطارية وإدارة التطبيقات وعمليات النظام.
- الصفحة تجمع تاريخ إصدارات طويل، لذلك يلزم اختبار الإصدار 6.2 نفسه وعدم افتراض أن كل ميزة تاريخية ما زالت متاحة كما هي.

**اعتماد الشبكة:** سجل المطور يذكر أن الأداة أصبحت تتطلب الإنترنت منذ إصدار 4.0 لأسباب حماية الأداة. يجب اختبار ما يعمل Offline وما يتطلب Server/API.

**الأمان:** صفحة المطور تحذر من أن بعض منتجات الحماية قد تصنف الأداة بسبب مكونات فك القفل والأتمتة والتغليف، وتوصي بعدم تشغيلها إذا لم يثق المستخدم بها، وتقترح Windows Sandbox أو VM لعزلها. ظهرت أيضاً اتهامات فردية على Reddit بوجود Malware/سرقة بيانات؛ هذه ادعاءات مستخدمين وليست نتيجة تحليل جنائي أو إثباتاً مستقلاً. فحص سمعة الرابط عبر Malwarebytes أعاد verdict = unknown، وليس safe ولا malicious، وكانت الصفحة غير قابلة للوصول من الفاحص. لذلك لا أستنتج سلامة ملف EXE من اسم الموقع أو Hash منشور وحده.

**قرار الاختبار:** لا تثبّت الأداة على جهاز العمل الأساسي في المرحلة الأولى. استخدم Windows Sandbox/VM نظيفة، ولا تدخل حسابات أو كلمات مرور أو محافظ أو بيانات شخصية، واحتفظ بنسخة من ملف التثبيت وSHA-256 بعد التنزيل من المصدر الرسمي. لا تستخدم MD5 المنشور كدليل كافٍ على سلامة الملف.

المراجع: [SamFw Tool 6.2 and changelog](https://samfw.com/blog/samfwtool), [SamFw official Telegram](https://t.me/s/samfwfrpchannel), [Reddit user reports](https://www.reddit.com/r/FRPtools/comments/1hc0h74/is_the_samfw_tool_49_legit/).

### 12.2 Chimera Tool — GSMHosting / Martview

- منتدى Chimera Tool الرسمي على GSMHosting نشط للغاية؛ صفحة القسم أظهرت أكثر من 1,000 صفحة نقاش، وموضوع Samsung Exynos Download Mode له آلاف الردود والمشاهدات.
- توجد تقارير Device Info في Download Mode، وطلبات دعم موديلات محددة، وردود من فريق Chimera تطلب تشغيل Device Info في Download Mode قبل إعادة محاولة إجراء آخر.
- تجربة Martview على Samsung A556B أظهرت أن SamFw استطاع عرض بيانات الهاتف في MTP، بينما Chimera Auto-detect عرض Unknown Samsung phone في Modem Mode واضطر المستخدم لاختيار الجهاز يدوياً. رد فريق Chimera أوضح أن Test Point على Android 16 محجوب حالياً في السياق المذكور.
- هذه المقارنة توضح أن الأداة قد تنجح في قراءة MTP بينما تفشل أداة أخرى في التعرف التلقائي على نفس الهاتف/واجهة USB. لا يوجد دعم Samsung موحد؛ الواجهة والموديل وإصدار النظام مهمة.
- تقارير Download Mode تثبت وجود إجراءات Device Info على عائلات Samsung معينة، لكنها لا تعني نجاح كل إجراء FRP/Repair أو دعم كل موديل.

المراجع: [GSMHosting Chimera forum](https://forum.gsmhosting.com/vbb/f898/), [GSMHosting Exynos Download Mode thread](https://forum.gsmhosting.com/vbb/f898/10-11-2026-extended-samsung-exynos-remove-frp-solution-download-mode-2819592-print/index194.html), [Martview A556B experience](https://www.martview-forum.com/threads/samsung-a546b-broken-test-point-pad.157431/).

### 12.3 UnlockTool

- موضوع تحديثات UnlockTool على GSMHosting يذكر إصدار 2026.10.14.0 و2026.10.15.0، مع وظائف Samsung Exynos في Odin Download Mode دون تفعيل ADB، وقائمة عائلات Exynos المدعومة.
- الموضوع ينبه إلى أن التوافق يختلف حسب الموديل والـ chipset والـ firmware/security patch.
- هذا دليل على وجود مسار Download Mode متخصص لدى الأداة، لكنه في سياق FRP/خدمات محددة وليس دليلاً تلقائياً على Read Info عامة لكل الشركات أو كل أوضاع الاتصال.
- يجب فحص تحديثات UnlockTool بحسب الوظيفة: Device Info / Read Info / Port Detection / Reboot / Mode Switching، وعدم عدّ كل إضافة FRP كميزة قراءة معلومات.

المصدر: [GSMHosting — UnlockTool updates](https://forum.gsmhosting.com/vbb/f162/unlock-tool-updates-3452654/).

### 12.4 آراء المستخدمين على Reddit

- في نقاش عن الأدوات المدفوعة، وصف مستخدمون SamFw بأنه خيار مجاني شائع، وذكروا استخدام UnlockTool وChimera وOctoplus مع اختلاف التغطية حسب العلامة والمهمة.
- ظهر رأي آخر يصف عدم وجود أداة واحدة تغطي كل الحالات، وأن المحلات الأكبر قد تحتفظ بأكثر من اشتراك.
- هذه آراء مجتمعية محدودة العدد وليست إحصاءً للسوق، لكنها تتسق مع تقارير GSMHosting التي تظهر أن دعم الموديل والإصدار أهم من اسم الأداة وحده.

المصدر: [Reddit — Paid FRP Tools?](https://www.reddit.com/r/FRPhelpdesk/comments/1vfkg54/paid_frp_tools/).

### 12.5 استنتاج محدث

1. SamFw يبدو واسعاً في Samsung MTP وADB، وله مسار Xiaomi Sideload ومهام Download Mode، لكن الإنترنت والتحقق الأمني شرطان قبل اعتماده.
2. Chimera وUnlockTool لديهما أدلة أقوى على مسارات Samsung Download Mode المتخصصة، لكن التوافق مقيد بالموديل والـ firmware، وبعض تقارير Auto-detection تظهر اختلافاً بين الأدوات.
3. VioletToolBox هو الأوسع من مرشحي GitHub في الوظائف العامة/OPLUS/Xiaomi، لكنه لا يثبت تغطية Samsung Download Mode أو Xiaomi Mi Assistant.
4. ATA-GUI هو أفضل مرشح واجهة عامة منظمة لـADB/Fastboot/Recovery، وليس أداة خدمة شاملة.
5. لا يوجد حتى الآن دليل كافٍ على برنامج واحد جاهز يجمع كل ما نريده مع إثبات عملي لكل وضع؛ قد تكون النتيجة الواقعية استخدام برنامج عام مع أداة أو أدوات متخصصة.

---

## 13. خطة الاختبار المقارن للأدوات الجاهزة

لا يتم تشغيل وظائف الكتابة أو التفليش أو إزالة الحماية أثناء اختبار Read Info. نبدأ بالاستكشاف والقراءة فقط.

| الاختبار | ATA-GUI | VioletToolBox | SamFw | Chimera | UnlockTool |
|---|---|---|---|---|---|
| اكتشاف ADB USB | مطلوب | مطلوب | مطلوب | مطلوب | مطلوب |
| قراءة بيانات ADB | مطلوب | مطلوب | مطلوب | مطلوب | مطلوب |
| Fastboot / Fastbootd | مطلوب | مطلوب | حسب دعم الإصدار | حسب الموديل | حسب الموديل |
| MTP/WPD generic | غير معلن | غير واضح | ليس هدفه العام | حسب الواجهة | حسب الواجهة |
| Samsung MTP Read Info | غير معلن | غير معلن | معلن | تجارب مستخدمين | يحتاج دليل |
| Samsung Download Mode Device Info | غير معلن | غير معلن | يحتاج تحديد الوظيفة/الموديل | معلن/تقارير | دعم Download Mode مثبت لوظائف معينة |
| Samsung AT Modem | غير معلن | غير معلن | بعض سجل AT/USB؛ يحتاج اختبار | حسب الموديل | حسب الموديل |
| Xiaomi Mi Assistant Sideload Read Info | غير معلن | غير معلن | MiCloud offline info فقط وفق السجل | يحتاج دليل | يحتاج دليل |
| Mode switching | ADB/Fastboot/Recovery | ADB/Fastboot/Recovery وبعض إجراءات الشركة | عدة أوضاع حسب الموديل | حسب الموديل | حسب الموديل |
| يعمل Offline بالكامل | تنزيل الأدوات أول مرة؛ يحتاج اختبار | يعتمد على الوظيفة/ROM center | لا؛ يتطلب الإنترنت لبعض الوظائف | يحتاج اختبار | يحتاج اختبار |

**سجل نتيجة كل تجربة:** اسم الأداة والإصدار، موديل الهاتف الكامل، build/security patch، وضع الهاتف، VID/PID، واجهات Windows/COM، الحقول التي ظهرت، زمن القراءة، هل احتاجت إنترنت، والرد الخام بعد إخفاء IMEI/SN وأي بيانات شخصية.

## 14. حالة البحث بعد هذه الجولة

- [x] فحص README والخصائص الداخلية لـATA-GUI وVioletToolBox وQADB وAdbGui وAndroid Multitool Universal وADBFastbootGUI وAndroid Toolbox GUI.
- [x] مراجعة إصدارات وCommits حديثة لـATA-GUI وVioletToolBox.
- [x] مراجعة Issues فعلية في VioletToolBox، وتوثيق ما أُغلق وما بقي مفتوحاً.
- [x] تصحيح تصنيف SamFw: Xiaomi Sideload Read offline MiCloud information لا يعني Read Info عاماً لكل الجهاز.
- [x] إضافة مراجعة أعمق لتجارب GSMHosting وMartview وReddit.
- [x] تسجيل مخاطر الاتصال بالإنترنت وملاحظات أمان SamFw دون اعتبار اتهامات Reddit إثباتاً.
- [ ] مراجعة المزيد من أدوات السوق: Octoplus، Pandora، DFT Pro، Android Utility Tool، EFT Pro، مع حصر ما يخص Read Info/Mode Detection/Mode Switching.
- [ ] اختبار إصدارات البرامج الجاهزة في Windows Sandbox أو VM.
- [ ] اختبار عملي على أجهزة حقيقية في أوضاع MTP/ADB/Fastboot/Recovery/Samsung Download/Xiaomi Mi Assistant.
- [ ] إعداد مقارنة نهائية وتحديد أفضل أداة أو مجموعة أدوات جاهزة.

**الحكم الحالي:** VioletToolBox وATA-GUI هما أبرز مرشحي GitHub، بينما SamFw وChimera وUnlockTool هي أهم المرشحين التجاريين للقراءة متعددة الأوضاع. لا توجد أداة معتمدة نهائياً قبل استكمال اختبار النسخ الفعلية على أجهزة حقيقية.


---

## 15. توسيع مقارنة الأدوات التجارية: Octoplus وEFT Pro وPandora

### 15.1 Octoplus Samsung Tool

**الحالة الحالية:** مرشح تجاري قوي جداً لقراءة معلومات Samsung والعمل في Download Mode.

- الموقع الرسمي أعلن Octoplus Samsung Tool v5.1.7 بتاريخ 3 نوفمبر 2026.
- سجل الإصدار يذكر تحسين اكتشاف أجهزة ADB وإعادة الاتصال بعد إعادة تشغيل الهاتف، وتحسين نقل البيانات عبر ADB، ودعم مستويات أمان حتى سبتمبر 2026.
- سجل GSMHosting يذكر في إصدارات 2026 عمليات Samsung Exynos عبر Odin/Download Mode، مع اختلاف التغطية حسب المعالج والـ firmware، وبعض العائلات كانت موسومة بأنها غير مختبرة وقت الإعلان.
- توجد سجلات مستخدمين فعلية لعملية Read Info تعرض Model وFirmware وCSC وSerial وAndroid version وSecurity Patch وبيانات أخرى. في بعض السجلات يتم الاتصال من ADB بعد تشغيل الجهاز، لذلك لا ننسب كل البيانات إلى Download Mode مباشرة.
- سجل قديم في تحديثات Octoplus يذكر إضافة Read Info عبر Download Mode؛ يجب اختبار النسخة الحالية 5.1.7 وتحديد هل هي قراءة مباشرة أم مسار متعدد المراحل.
- المنتدى الرسمي نشط، لكن نجاح Read Info لا يعني نجاح FRP أو Repair على نفس الموديل.

**الحكم:** يضاف إلى أعلى قائمة التجربة مع Chimera وUnlockTool عند التركيز على Samsung، مع اختبار مستقل لثلاث حالات: MTP/ADB، Download Mode Device Info، وعودة الجهاز بعد Reboot.

المراجع: [Official Octoplus news](https://octoplusbox.com/news/), [v5.1.7 release notes](https://octopusbox.com/por/news/details/1104), [GSMHosting Octoplus Samsung forum](https://forum.gsmhosting.com/vbb/f684/), [2026 update thread](https://forum.gsmhosting.com/vbb/f684/octoplus-box-samsung-updates-1608250-print/index17.html), [user Read Info log](https://forum.gsmhosting.com/vbb/f684/a055f-bit-17-fail-bypass-3455671/).

### 15.2 EFT Pro Dongle

**الحالة الحالية:** مرشح متعدد الأوضاع يستحق الفحص، لأن قائمة الميزات الرسمية تفصل صراحة بين MTP وADB وFastboot وSamsung COM وOdin Download Mode.

قائمة الميزات الرسمية تعرض:
- General MTP: Read Info (MTP Only).
- Fastboot Service: Read Info Full.
- ADB: Read Info.
- Samsung Service Normal Mode (COM Port): Read Information وReboot to Download Mode.
- Samsung Service Odin Mode (Download Mode): Read Information وFix Soft Bricked.
- Samsung ADB Mode: Read Information.
- فئات أخرى للشركات والمعالجات موجودة في المنتج، لكنها خارج نطاق هذا البحث ولا تدخل في تقييمنا.

**الحكم:** من أقرب الأدوات المعلنة لمتطلب تعدد قنوات قراءة المعلومات، لأن الموقع يسرد قنوات قراءة منفصلة حسب الوضع. لكن قائمة الخصائص لا تكفي لإثبات أن كل وضع يعمل على كل موديل؛ نحتاج إثباتات موديل-بموديل من GSMHosting وتشغيل نسخة فعلية. كما يجب اختبار ما إذا كان MTP Read Info عاماً أم مقيداً بأجهزة/واجهات معينة.

المراجع: [EFT Pro official feature catalog](https://www.eftprodongle.com/features), [GSMHosting EFT Pro update discussion](https://forum.gsmhosting.com/vbb/f1014/eft-pro-dongle-update-v3-4-0-repair-imei-for-generic-qcom-devices-2881419/).

### 15.3 Pandora Tool

- تحديث Pandora 4.9 المنشور في 2023 يذكر Read Info/IMEI/Partitions لموديلات Samsung SPD وبعض أجهزة vivo، ضمن مجموعة وظائف صيانة واسعة.
- ظهرت في GSMHosting بلاغات أخطاء عند Read Info على بعض الأجهزة، ورد دعم Z3X بطلب إيقاف Read Info وإعادة التجربة في حالة معينة؛ هذا يثبت وجود اختلافات أو أخطاء مرتبطة بموديل/إصدار، ولا يكفي للحكم على المنتج كله.
- لم يظهر في هذه الجولة دليل حديث كافٍ على أحدث إصدار Pandora أو تغطية Read Info الحالية للأوضاع المطلوبة؛ لذلك لا أرفعه إلى نفس أولوية Octoplus/EFT/Chimera حتى نثبت نشاط التطوير الحالي.

المراجع: [Pandora 4.9 announcement](https://forum.gsmhosting.com/vbb/f1110/pandora-tool-4-9-update-new-devices-new-exclusive-functions-added-3145931/), [GSMHosting Read Info error report](https://forum.gsmhosting.com/vbb/f1110/erro-pandora-answered-3308425/).

### 15.4 ترتيب المرشحين التجاريين بعد التوسعة

| الأولوية | الأداة | الدليل الأقوى الحالي | ما يحتاج تحقق |
|---:|---|---|---|
| 1 | EFT Pro | قائمة رسمية تفصل Read Info عبر MTP وADB وFastboot وSamsung COM وOdin Download Mode | دعم الموديلات الفعلي، العمل دون Server، ونسخة البرنامج الحالية |
| 2 | Octoplus Samsung | إصدار 5.1.7 حديث، دعم Download Mode لعائلات Exynos، وسجلات Read Info حقيقية | فصل قراءة Download Mode المباشرة عن قراءة ADB بعد إعادة التشغيل |
| 3 | Chimera | مجتمع GSMHosting نشط وتقارير Device Info في Download Mode | مشاكل Auto-detection وتوافق الموديل/firmware |
| 4 | UnlockTool | تقارير حديثة عن Download Mode لخدمات Samsung Exynos | إثبات Read Info مستقلة لكل وضع، وليس فقط إجراءات FRP |
| 5 | SamFw | Samsung MTP Read Info وADB، ومعلومات MiCloud عبر Xiaomi Sideload | مخاطر الأمان، الاعتماد على الإنترنت، وتحديد الحقول الفعلية لكل وضع |
| 6 | Pandora | تاريخ وظائف Read Info/IMEI/Partitions لموديلات محددة | تحديث حديث وتوافق حالي غير مثبت |

**ملاحظة:** هذا ترتيب لأولوية الفحص، وليس حكماً نهائياً على الجودة أو الأفضلية التجارية. الأدلة المعلنة من المطور لا تساوي تجربة مستقلة، وسجل النجاح لموديل واحد لا يثبت دعم العائلة كلها.

### 15.5 ما الذي تغير في الاستنتاج؟

أصبح لدينا الآن مرشحان تجاريان إضافيان قد يكونان أقرب من SamFw إلى فكرة قراءة المعلومات عبر عدة أوضاع:
- EFT Pro بسبب وجود Read Info منفصل لـMTP وADB وFastboot وSamsung COM وOdin Download Mode في كتالوج الخصائص.
- Octoplus بسبب دعم Samsung Download Mode وتحديثات حديثة وسجلات Device Info من أجهزة حقيقية.

لذلك لن نحصر اختبار الأدوات التجارية في SamFw وChimera وUnlockTool. يجب أن تكون EFT Pro وOctoplus في نفس دفعة الاختبار الأولى.


---
