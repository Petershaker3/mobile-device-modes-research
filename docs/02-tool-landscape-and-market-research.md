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


---

## 19. مبدأ إعادة استخدام مشاريع GitHub قبل بناء بديل (Reuse Before Rebuild)

### 19.1 القرار الهندسي

لا نبدأ تنفيذ وظيفة من الصفر قبل البحث المنظم عن مشروع أو مكتبة أو أداة مفتوحة المصدر تنفذ الوظيفة المطلوبة بالفعل. الهدف هو تقليل تكرار العمل والاستفادة من التنفيذات التي نضجت، مع الاحتفاظ بالتحكم في تجربة المستخدم والمعمارية النهائية.

هذا المبدأ لا يعني دمج أي مشروع لمجرد أنه موجود على GitHub، ولا يعني نسخ مستودع كامل داخل البرنامج. نختار أصغر وحدة قابلة لإعادة الاستخدام تحقق الحاجة، ونحافظ على حدود واضحة بين البرنامج والمكوّن الخارجي.

### 19.2 ترتيب خيارات إعادة الاستخدام

يُقيّم كل احتياج بالترتيب التالي، مع اختيار المستوى الأنسب:

1. **استخدام أداة أو مكتبة مستقرة كما هي:** تشغيل CLI أو استدعاء SDK/API موثق، إذا كانت تغطي الوظيفة المطلوبة وتسمح بالتكامل المنضبط.
2. **استخدام مكوّن مستقل:** دمج مكتبة أو حزمة محددة بدلاً من إعادة بناء الوظيفة.
3. **Adapter / Wrapper:** بناء طبقة ربط داخل برنامجنا حول أداة موجودة، لتوحيد الإدخال والإخراج والأخطاء والإصدارات دون تعديل جوهرها.
4. **Fork محدود:** عند الحاجة إلى إصلاح أو توسيع مشروع قائم لا يقبل التغيير المطلوب upstream؛ يجب توثيق نقطة الانفصال والتغييرات ومتابعة الأصل.
5. **Vendor / Embed:** تضمين جزء محدد من المصدر عند وجود مبرر معماري واضح، مع تحديد الملفات المنقولة ومصدرها وإصدارها.
6. **تنفيذ مستقل:** الخيار الأخير عندما لا يوجد حل مناسب أو تكون كلفة/مخاطر دمج الموجود أعلى من بناء بديل.

### 19.3 أمثلة أولية تستحق الفحص — وليست قرارات دمج

| المشروع | ما يمكن الاستفادة منه | حدود الاستنتاج الحالي |
|---|---|---|
| [ATA-GUI](https://github.com/msartore/ATA-GUI) | مرجع لتجربة واجهة ADB/Fastboot وتدفق إدارة الجهاز | لا يعني اعتماد واجهته أو معماريته كاملة |
| [UAD-ng](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation) | مرجع لوظائف اكتشاف الحزم وإدارة/تعطيل تطبيقات Android | لا يُعامل كمدير شامل لـAPK/APKS أو DNS |
| [bundletool](https://github.com/google/bundletool) | إعادة استخدام الأدوات الرسمية للتعامل مع Android App Bundles وAPK Sets واختيار splits | يجب تحديد ما إذا كان التكامل عبر CLI أو مكتبة/مكوّن، والتحقق من سلوك الإصدار المستخدم |
| [scrcpy](https://github.com/Genymobile/scrcpy) | مكوّن جاهز لعرض شاشة Android والتحكم بها عبر USB أو TCP/IP | يُدمج كعملية/مكوّن منفصل عند الحاجة، لا يعاد تنفيذ بث الشاشة داخلياً دون سبب |
| [Android Platform Tools](https://developer.android.com/tools/releases/platform-tools) | الاعتماد على ADB/Fastboot الرسميين بدلاً من إعادة تنفيذ البروتوكولات | يلزم تثبيت/تحديث إصدار معروف والتحقق من الملفات ومخرجات الأدوات |

هذه قائمة مرشحين أولية مستندة إلى وصف المشاريع ومصادرها العامة؛ لم يتم بعد اعتماد إصدار أو اختبار دمج أي مشروع في تطبيق نهائي.

### 19.4 بطاقة تقييم أي مشروع قبل اعتماده

- الوظيفة المطلوبة بالضبط، وما الذي لا يغطيه المشروع.
- آخر إصدار مستقر وCommit SHA المراد تثبيته؛ لا تعتمد على فرع متحرك مثل `main` وقت البناء.
- حالة الصيانة: نشاط commits وreleases وissues، مع عدم اعتبار عدد النجوم دليلاً على الجودة.
- بنية المشروع ونقاط التكامل وواجهاته العامة.
- متطلبات التشغيل والحجم والأداء وتوافق Windows.
- الاعتماديات الخارجية، تنزيلات وقت التشغيل، الاتصالات الشبكية، والخدمات التي قد تتوقف.
- سلوك الأخطاء، السجلات، دعم تعدد الأجهزة، وإمكانية تحديد Serial صراحة.
- اختبارات المشروع upstream، ثم اختباراتنا المستقلة في بيئة معزولة.
- طريقة تحديث المكوّن أو استبداله دون كسر بقية النظام.
- مصدر كل ميزة منقولة أو مدمجة، والملفات/الإصدار/Commit المستخدم.

### 19.5 قواعد الدمج المعماري

- لا تربط منطق الواجهة مباشرة بمخرجات CLI غير مستقرة؛ استخدم Adapter يحولها إلى نماذج بيانات داخلية typed.
- لا تسمح لمكوّن خارجي بالتحكم في قاعدة البيانات أو إعدادات البرنامج الأساسية دون واجهة محددة.
- اعزل العمليات الخارجية بمهلة زمنية، وإلغاء، وتسجيل منقح، ومعالجة exit codes وstderr.
- ثبّت الإصدارات والـ hashes للملفات التنفيذية التي يتم توزيعها، ولا تنزّل أحدث نسخة عشوائياً عند كل تشغيل.
- افصل بيانات المكوّن الخارجي عن نموذج البيانات الداخلي، واكتب اختبارات عقد (Contract Tests) على واجهة الـAdapter.
- احتفظ بإمكانية استبدال المكوّن بمكوّن آخر أو تنفيذ داخلي دون إعادة تصميم كل الوحدات.
- لا تنسخ مستودعاً كاملاً إذا كان المطلوب وظيفة صغيرة يمكن استدعاؤها من خلال API أو CLI.

### 19.6 حالة المحور

- [x] اعتماد مبدأ البحث وإعادة الاستخدام قبل التنفيذ المستقل.
- [x] وضع مستويات إعادة الاستخدام وبطاقة تقييم موحدة.
- [x] تحديد مشاريع أولية للفحص حسب الوظائف.
- [ ] فحص الإصدارات الحالية والكود والاعتماديات لكل مرشح.
- [ ] تحديد المكونات التي ستُستخدم مباشرة أو عبر Adapter أو Fork.
- [ ] إثبات البناء والتشغيل والاختبارات بعد الدمج في بيئة Windows.
- [ ] توثيق المكوّنات المعتمدة وإصداراتها وحدودها وخطة تحديثها.

**الحالة:** سياسة هندسية مع قائمة مرشحين؛ لا يوجد اعتماد دمج أو اختبار تكامل حتى الآن.

---

## 20. إتاحة وظائف البرنامج لوكلاء الذكاء الاصطناعي عبر MCP

### 20.1 الهدف وسيناريو الاستخدام

يجب تصميم البرنامج بحيث يمكن استخدامه بطريقتين متكاملتين:
- واجهة Windows التقليدية التي يتحكم فيها المستخدم.
- واجهة أدوات قياسية لوكلاء الذكاء الاصطناعي عبر **Model Context Protocol (MCP)**.

مثال الاستخدام المستهدف: يفتح المستخدم Codex ويطلب منه: «اتصل ببرنامج إدارة الهاتف، اعرض الأجهزة المتصلة، ثم اعرض التطبيقات المثبتة على الجهاز المحدد ورتبها حسب تاريخ التثبيت». يقوم Codex باكتشاف أدوات MCP المناسبة واستدعائها، ويعرض النتائج للمستخدم. ويمكنه تنفيذ مهام أخرى فقط ضمن الصلاحيات التي يسمح بها البرنامج.

MCP معيار لتبادل السياق واستدعاء الأدوات بين العميل والخادم؛ هو طبقة تكامل وليس نموذج ذكاء اصطناعي، ولا يمنح البرنامج اتصالاً سحرياً أو يتجاوز قيود Android وADB وWindows.

### 20.2 المعمارية المقترحة

**القرار المعماري المبدئي:** بناء MCP Server محلي تابع للبرنامج، لكنه منفصل عن واجهة المستخدم وعن منطق الوظائف الأساسي.

```text
Codex / MCP Client
        |
        | MCP (STDIO مبدئياً)
        v
Local MCP Server
        |
        v
Application Service Layer
        |
        +--> Device Discovery / ADB / Fastboot
        +--> App Inventory & Package Management
        +--> DNS Management
        +--> Windows Driver Diagnostics
        +--> Reports / Operation History
        |
        v
Connected Android Device / Windows APIs
```

- كل وظيفة أساسية يجب أن تكون متاحة أولاً من خلال Service Layer داخل البرنامج.
- واجهة GUI وMCP تستدعيان نفس الخدمات؛ لا ننشئ منطقاً منفصلاً لكل واجهة.
- MCP Server لا يتعامل مع قاعدة البيانات أو USB مباشرة إذا كانت الخدمة الداخلية تملك هذه المسؤولية.
- يبدأ التصميم بخادم محلي عبر STDIO لأنه مناسب لربط Codex بعملية محلية دون فتح منفذ شبكة.
- يمكن إضافة Streamable HTTP لاحقاً عند وجود حاجة حقيقية لعدة عملاء أو اتصال بعيد، مع مصادقة وتحديد واجهة الاستماع والصلاحيات.
- لا نعرض خدمة HTTP على الشبكة المحلية أو الإنترنت افتراضياً.

### 20.3 مجموعة الأدوات (MCP Tools) المقترحة

| المجال | أمثلة أدوات MCP | مستوى المخاطر الافتراضي |
|---|---|---|
| الأجهزة | `list_devices`, `get_device_info`, `get_connection_state` | قراءة فقط |
| التطبيقات | `list_installed_apps`, `get_app_details`, `filter_apps_by_install_date`, `get_app_install_source` | قراءة فقط |
| ملفات التطبيقات | `inspect_apk`, `analyze_apk_compatibility`, `export_apk` | قراءة/تصدير مع اختيار المسار |
| إدارة التطبيقات | `install_apk`, `install_apks`, `enable_app`, `disable_app`, `uninstall_app` | يتطلب موافقة |
| DNS | `get_dns_config`, `preview_dns_change`, `apply_dns_config`, `restore_dns_config`, `verify_dns` | القراءة فقط افتراضياً؛ التغيير بموافقة |
| التعريفات | `scan_windows_drivers`, `get_driver_diagnosis`, `find_official_driver`, `preview_driver_repair`, `apply_driver_repair` | الفحص قراءة فقط؛ التثبيت/الإزالة بموافقة |
| التقارير | `get_last_operation`, `export_diagnostic_report` | قراءة/تصدير مضبوط |

هذه أسماء وظيفية مقترحة وليست عقد API نهائياً. يجب أن يكون لكل أداة وصف واضح، ومدخلات محددة، ومخرجات منظمة، وأخطاء قابلة للفهم.

### 20.4 الأمان والصلاحيات — شرط أساسي

- **Read-only افتراضياً:** اكتشاف الأجهزة وقراءة المعلومات والقوائم والتقارير لا تغير حالة الهاتف.- **موافقة منفصلة للكتابة:** التثبيت والحذف والتعطيل والتفعيل وتغيير DNS وإصلاح التعريفات لا تنفذ لمجرد أن النموذج طلبها.- **Preview ثم Apply:** العمليات الحساسة تعرض الجهاز المستهدف (Serial/اسم مفهوم)، والقيمة الحالية والجديدة، والأثر المتوقع، ثم تتطلب موافقة المستخدم.
- **لا تنفيذ Shell عام افتراضياً:** لا توفر أداة مثل `execute_arbitrary_shell` كواجهة مفتوحة للنموذج. استخدم أدوات محددة ذات مدخلات متحققة وقوائم سماح.
- **تحديد الجهاز صراحة:** كل عملية تستهدف جهازاً محدداً بمعرف جلسة/Serial؛ لا تعتمد على اختيار أول جهاز متصل عند وجود أكثر من جهاز.
- **تأكيد إضافي للعمليات المدمرة:** إلغاء تثبيت تطبيق أو إزالة تعريف أو تغيير إعداد شبكة يحتاج تأكيداً واضحاً، مع منع العمليات الجماعية الخطرة افتراضياً.
- **أقل صلاحية:** لا تمنح MCP صلاحيات Windows Administrator إلا عند تنفيذ وظيفة تتطلبها فعلاً، ولا تشغّل الخادم كله بصلاحيات مرتفعة.
- **حماية الأسرار والخصوصية:** لا تُرجع IMEI أو Serial أو بيانات شخصية أو رموز اعتماد إلا عند الحاجة وبطلب واضح؛ لا تسجل Tokens أو محتوى حساس في logs.
- **مصدر الثقة:** لا تقبل أوامر MCP من خادم بعيد غير موثوق، ولا تعرض منفذ HTTP دون مصادقة.
- **سجل تدقيق:** يسجل اسم الأداة والوقت والجهاز والعملية والنتيجة والموافقة، مع تنقيح البيانات الحساسة.

### 20.5 موارد وسياقات يمكن إتاحتها للوكيل

إلى جانب Tools، يمكن للخادم تقديم:
- **Resources:** حالة الأجهزة المتصلة، ملخص قدرات الجهاز، سجل آخر عملية، وقواعد التوافق الموثقة.
- **Prompts:** قوالب جاهزة مثل «شخّص لماذا ADB لا يرى الجهاز»، «راجع التطبيقات المثبتة حديثاً»، و«أنشئ تقرير تعريفات Windows».
- **Instructions:** تعليمات ثابتة للوكيل توضح أن القراءة مسموحة، وأن التغيير يتطلب موافقة، وأنه يجب تحديد Serial، وألا يستخدم أوامر Shell غير مقيدة.

يجب أن تبقى البيانات المعروضة للوكيل محدودة بالمهمة؛ لا نرسل كل سجلات الهاتف أو بياناته تلقائياً.

### 20.6 مشاريع MCP قائمة تستحق إعادة الاستخدام أو المقارنة

| المشروع | نطاقه المعلن | كيف نستفيد منه |
|---|---|---|
| [iksnerd/adb-mcp](https://github.com/iksnerd/adb-mcp) | خادم MCP لأجهزة Android عبر ADB، ويعلن أدوات للأجهزة وواجهة المستخدم والتطبيقات والسجلات والملفات، مع دعم Codex مذكور في README | مرشح قوي لفحصه كخادم قائم أو مرجع لتصميم أدوات ADB، مع التحقق من Windows وتوافق الوظائف المطلوبة |
| [us-all/android-mcp-server](https://github.com/us-all/android-mcp-server) | خادم MCP لـADB يعلن أدوات تشخيص وإدارة، ووضع قراءة افتراضي وحواجز منفصلة للكتابة وShell، وSTDIO/Streamable HTTP | مرجع مهم لتصميم الصلاحيات وفصل القراءة عن الكتابة؛ لا يعني اعتماد الكود قبل مراجعته |
| [qalvinahmad/android-mcp](https://github.com/qalvinahmad/android-mcp) | خادم TypeScript لـADB مع اكتشاف USB/Wireless وإدارة التطبيقات وواجهة الجهاز | مرشح للمقارنة مع خادمنا أو أخذ أفكار محددة منه |

المعلومات أعلاه مأخوذة من صفحات المستودعات العامة وREADME وقت إعداد هذه الإضافة. لم يتم تدقيق الكود أو بناء المشاريع أو اختبارها على Windows/أجهزة فعلية، ولذلك تظل **Claimed / Needs audit** وليست مكونات معتمدة.

### 20.7 خطة تحقق MCP

1. تشغيل MCP Server محلياً عبر STDIO وربطه بنسخة Codex المستخدمة.
2. التأكد أن Codex يكتشف الأدوات ويعرض وصفها ومدخلاتها بصورة صحيحة.
3. اختبار قراءة قائمة الأجهزة ومعلومات جهاز واحد، مع أكثر من جهاز متصل للتحقق من منع الالتباس.
4. اختبار قائمة التطبيقات وفلاتر First Install Time وLast Update Time وInstaller Source.
5. اختبار أن جميع أدوات القراءة لا تغير حالة الهاتف.
6. استدعاء أداة كتابة تجريبية والتأكد من رفضها قبل الموافقة.
7. اختبار Preview/Approve/Apply/Verify لعملية واحدة مصرح بها على جهاز اختبار.
8. اختبار فصل الجهاز أو توقف ADB أثناء الاستدعاء، والتأكد من إرجاع خطأ واضح دون الإبلاغ عن نجاح زائف.
9. اختبار إغلاق واجهة البرنامج: تحديد هل يتوقف MCP Server أم يعمل كخدمة محلية مستقلة، وتوثيق القرار.
10. اختبار المصادقة وسلوك الشبكة إذا أضيف Streamable HTTP مستقبلاً.

### 20.8 حالة المحور

- [x] تحديد سيناريو استخدام البرنامج من Codex ووكلاء MCP.
- [x] تحديد فصل MCP Server عن Service Layer والواجهة.
- [x] وضع قائمة أولية للأدوات والموارد والصلاحيات.
- [x] تحديد مشاريع MCP قائمة للفحص وإعادة الاستخدام.
- [ ] تدقيق مشاريع MCP المرشحة وتوافقها مع Windows ونطاق الوظائف.
- [ ] اعتماد عقد الأدوات النهائي ومخططات المدخلات والمخرجات.
- [ ] تنفيذ خادم محلي وربطه بـCodex.
- [ ] اختبار الموافقات وتعدد الأجهزة وفشل الاتصال والسجلات.
- [ ] اعتماد دعم HTTP فقط إذا ظهرت حاجة عملية ومراجعة أمنية مستقلة.

**الحالة:** متطلبات ومعمارية بحثية أولية؛ لم يتم بناء MCP Server أو ربط Codex أو تنفيذ أي عملية على هاتف فعلي بعد.


### 20.9 تدقيق المرشحين بناءً على README والمصادر الحالية (12 نوفمبر 2026)

تمت مراجعة صفحات README الحالية للمشاريع التالية. هذه مراجعة وثائق/مصدر علني وليست مراجعة أمنية للكود كاملاً أو اختبار تشغيل.

| المشروع | ما يظهر من README الحالي | الملاءمة لهذا البرنامج | القرار البحثي |
|---|---|---|---|
| [us-all/android-mcp-server](https://github.com/us-all/android-mcp-server) | يعلن 76 أداة تقريباً في القراءة والتشخيص وإدارة التطبيقات وواجهة المستخدم، ويدعم STDIO وStreamable HTTP. وضع القراءة فقط هو الافتراضي؛ الكتابة وShell لهما بوابتان منفصلتان عبر متغيرات بيئة. | أقرب مرشح لفحص تصميم الصلاحيات وأدوات Package Manager والتشخيص. يحتاج تدقيق Windows، تفاصيل تثبيت/إزالة التطبيقات، سلوك DNS، وحماية أدوات الكتابة قبل أي اعتماد. | **الأولوية الأولى للتدقيق الأمني والوظيفي**، وليس الدمج التلقائي. |
| [iksnerd/adb-mcp](https://github.com/iksnerd/adb-mcp) | يعلن 81 أداة، ويذكر دعم Codex عبر Plugin، وبناء Go، وإصدارات Windows amd64/arm64، وأدوات ADB للأجهزة والتفاعل والملفات والتطبيقات والسجلات. | تكامل Codex وWindows مذكوران صراحة، ويحتوي على خبرة تنفيذية جيدة في ADB. لكنه واسع جداً ويتضمن أدوات تفاعلية وتغييرات جهاز لا نحتاج إتاحتها كلها. | **الأولوية الثانية**؛ انتقاء أدوات القراءة/التشخيص أو الاستفادة من تصميمه بعد مراجعة الكود والأدوات الحساسة. |
| [qalvinahmad/android-mcp](https://github.com/qalvinahmad/android-mcp) | يعلن 26 أداة تقريباً عبر TypeScript/Node، USB وWireless ADB، إدارة تطبيقات، لقطات شاشة، UI automation وLogcat. اختيار الجهاز الافتراضي قد يعتمد على أول جهاز فعلي متصل عند عدم تحديده. | بسيط نسبياً ويدعم Windows وفق مسار اكتشاف Android SDK، لكنه يضم أوامر تغيير مباشرة، ويحتاج فحص سياسة اختيار الجهاز والموافقات. | **مرشح للمقارنة المعمارية**، وليس خياراً افتراضياً بسبب مخاطر الاختيار التلقائي وعمليات الكتابة. |
| [modelcontextprotocol/typescript-sdk](https://github.com/modelcontextprotocol/typescript-sdk) | SDK الرسمي لـMCP في TypeScript؛ يوفر Server/Client وTools/Resources/Prompts وSTDIO وStreamable HTTP. README الحالي يوضح خط v2 وحزم `@modelcontextprotocol/server` و`@modelcontextprotocol/client`. | أساس مناسب لبناء MCP Server خاص بالبرنامج إذا كان TypeScript/Node هو اختيار التنفيذ. لا يوفر منطق ADB أو إدارة الهاتف؛ هو بروتوكول وSDK فقط. | **مرشح أساس تقني** إذا استقر قرار TypeScript؛ تثبيت نسخة محددة ومراجعة دليل الترقية قبل التنفيذ. |
| [modelcontextprotocol/python-sdk](https://github.com/modelcontextprotocol/python-sdk) | SDK الرسمي لـMCP في Python، ويدعم الخوادم والعملاء وSTDIO وStreamable HTTP. | بديل تقني إذا كان جزء الخدمة أو التكامل مبنياً على Python؛ لا يبرر إدخال Runtime إضافي وحده. | **بديل للمقارنة** وليس قراراً بالتعدد اللغوي. |

**ملاحظة تدقيق مهمة:** أرقام الأدوات وأوصاف الخصائص أعلاه هي ما يعلنه README وقت المراجعة وقد تتغير. لا نعتبر عدد الأدوات مقياس جودة. كما أن وجود خيار Read-only افتراضي في مشروع خارجي لا يغني عن بناء سياسة صلاحيات داخل برنامجنا نفسه.

**مراجع هذه المراجعة:**
- [us-all README](https://github.com/us-all/android-mcp-server/blob/main/README.md)
- [adb-mcp README](https://github.com/iksnerd/adb-mcp/blob/main/README.md)
- [android-mcp README](https://github.com/qalvinahmad/android-mcp/blob/main/README.md)
- [MCP TypeScript SDK](https://github.com/modelcontextprotocol/typescript-sdk)
- [MCP Python SDK](https://github.com/modelcontextprotocol/python-sdk)

### 20.10 توصيل Codex بخادم البرنامج المحلي

Codex يدعم خوادم MCP عبر STDIO أو Streamable HTTP. وتوضح وثائقه أن إعدادات MCP مشتركة بين Codex CLI وإضافة IDE وتطبيق ChatGPT desktop/Codex بحسب طريقة الاستخدام والإصدار. في الاستخدام المحلي على جهاز Windows الذي يتصل بالهاتف، يكون STDIO هو نقطة البداية الأبسط: يشغّل Codex عملية MCP المحلية ويتواصل معها عبر stdin/stdout.

#### مثال إعداد عبر Codex CLI

بعد تثبيت البرنامج وخادم MCP محلياً، يكون شكل التسجيل المقترح:

```powershell
codex mcp add kemet-device -- C:\Program Files\Kemet Device Manager\mcp-server.exe
codex mcp list
```

المثال يوضح الشكل فقط؛ المسار التنفيذي واسم الخادم النهائيان لم يتم تحديدهما بعد. يجب استخدام مسار فعلي موجود على جهاز المستخدم.

#### مثال إعداد مباشر في config.toml

المسار الافتراضي المذكور في وثائق Codex هو `~/.codex/config.toml`:

```toml
[mcp_servers.kemet_device]
command = "C:\\Program Files\\Kemet Device Manager\\mcp-server.exe"
startup_timeout_sec = 10
tool_timeout_sec = 60
default_tools_approval_mode = "writes"
```

يجب التحقق من صياغة المسارات في TOML ومن خيارات النسخة المستخدمة وقت التنفيذ. يمكن استخدام `enabled_tools` لتقليل الأدوات المعروضة، و`disabled_tools` لاستبعاد أدوات بعينها، و`tools.<tool>.approval_mode` لتحديد موافقة أداة محددة.

**قرار الصلاحيات المقترح في Codex:**
- اجعل أدوات القراءة فقط متاحة دون موافقة متكررة، بعد اختبار أنها لا تغير حالة الجهاز.
- اجعل أدوات الكتابة تتطلب موافقة، باستخدام وضع `writes` أو موافقة خاصة لكل أداة.
- لا تعتمد على إعداد Codex وحده؛ يجب أن يرفض MCP Server نفسه عمليات الكتابة غير المصرح بها.
- لا تستخدم Shell عام أو أداة تنفيذ أوامر حرة في الخادم الأساسي.
- لا تضع أسراراً أو Tokens داخل ملف إعدادات المشروع أو مستودع Git.

**مراجع Codex الرسمية:**
- [Codex MCP setup and transports](https://developers.openai.com/codex/mcp)
- [Codex MCP configuration reference](https://developers.openai.com/codex/config-reference)
- [OpenAI MCP connections guide](https://developers.openai.com/api/docs/guides/agents-api/tools/mcp)

### 20.11 اعتبارات أمن MCP التي يجب أن تدخل في التصميم

- STDIO: لا تطبع أي رسائل تشغيل أو Debug إلى stdout؛ فهو قناة JSON-RPC. استخدم stderr للسجلات.
- Streamable HTTP: إذا أضيف لاحقاً، اربط الخادم محلياً على `127.0.0.1` افتراضياً، وتحقق من Origin، وفعّل المصادقة المناسبة. لا تستخدم `0.0.0.0` كقيمة افتراضية.
- لا تفترض أن المصادقة وحدها تمنع إساءة الاستخدام؛ يجب تطبيق صلاحيات على مستوى الأداة والعملية والجهاز.
- لا تسمح بتمرير اسم حزمة أو مسار ملف أو وسيطة Shell دون تحقق وتقييد.
- يجب أن تكون كل أداة ذات أثر جانبي واضحة في وصفها ومخططها، وأن تُرجع حالة العملية والنتيجة الفعلية لا مجرد نجاح إرسال الأمر.
- لا تسمح للوكيل باختيار أول جهاز تلقائياً عند تعدد الأجهزة؛ اطلب تحديد Serial أو استخدم جلسة جهاز مثبتة بوضوح.
- ضع حدوداً للوقت وحجم المخرجات وعدد العمليات المتزامنة، مع دعم الإلغاء والتعامل مع انقطاع USB/Wireless.
- لا تعرض IMEI أو أرقاماً تسلسلية أو بيانات شخصية للوكيل إلا عند الحاجة الصريحة للمهمة.
- افصل عمليات التشخيص عن عمليات التغيير، وسجل من استدعى الأداة وعلى أي جهاز وهل تمت الموافقة وما النتيجة.
- اعتبر محتوى الهاتف وسجلاته وبيانات التطبيقات مدخلات غير موثوقة؛ لا تنفذ تعليمات تظهر داخل أسماء التطبيقات أو Logcat أو ملفات الجهاز كأنها تعليمات للمساعد.

**ملاحظة أمنية:** ظهرت تقارير أمنية منشورة عن ثغرات في أدوات تحكم Android عبر MCP بسبب تمرير مدخلات المستخدم إلى أوامر ADB shell دون escaping كافٍ. لذلك يجب مراجعة بناء الأوامر والتحقق من المدخلات واستخدام واجهات محددة بدلاً من تجميع shell strings. مثال مرجعي: [Microsoft UFO security advisory GHSA-5cjx-4375-4877](https://github.com/microsoft/UFO/security/advisories/GHSA-5cjx-4375-4877).

### 20.12 قرار التنفيذ المرحلي المقترح

1. لا ندمج خادم Android MCP جاهزاً بالكامل كواجهة البرنامج الرسمية قبل تدقيق الكود والصلاحيات.
2. نعيد استخدام SDK الرسمي لـMCP، ونستفيد من المشاريع المرشحة في الوظائف والأفكار التي اجتازت التدقيق.
3. نبني MCP Server رفيعاً حول Service Layer الخاصة بالبرنامج، بحيث تستدعي GUI وMCP الخدمات نفسها.
4. نبدأ بأدوات قراءة قليلة: `list_devices`, `get_device_info`, `list_installed_apps`, `get_app_details`, `get_dns_config`, `scan_windows_drivers`.
5. نختبر Codex على Windows مع جهاز اختبار واحد ثم جهازين، ونثبت أن اختيار الجهاز صريح.
6. نضيف الكتابة بعد إكمال نموذج Preview/Approval/Apply/Verify واختبارات الأمان.
7. لا نضيف HTTP إلا إذا احتجنا عميلاً بعيداً أو أكثر من عميل محلي، وبعد مراجعة تهديدات مستقلة.

**الحالة بعد التدقيق:** أصبح لدينا مرشح أول لفحص الحماية والتشخيص (us-all)، ومرشح قوي لتكامل Codex/Windows (iksnerd)، وSDK رسمي للبناء. لم يتم بعد تشغيل أي خادم MCP أو ربط Codex أو اختبار جهاز فعلي.


### 20.13 تدقيق أولي للكود الفعلي للمشروعات الثلاثة (12 نوفمبر 2026)

**نطاق هذه الجولة:** قراءة ملفات المصدر والاختبارات وملفات CI من الفرع `main` لكل مشروع. لم يتم تنزيل المشاريع أو تثبيتها أو تشغيل اختبارات CI محلياً، ولم يتم الاتصال بأي هاتف. لذلك فالنتائج أدناه **Code-inspected** وليست Runtime-verified أو Security-audit كاملة.

#### ملخص النتائج

| المشروع | دليل من الكود | ملاحظة أمن/سلوك | التقييم بعد قراءة المصدر |
|---|---|---|---|
| [us-all/android-mcp-server](https://github.com/us-all/android-mcp-server) | يستخدم `child_process.execFile` مع مصفوفة arguments وtimeout افتراضي 30 ثانية؛ أدوات التطبيقات تفصل القراءة عن الكتابة عبر `assertWriteAllowed()`؛ أداة Shell الحرة لها بوابة منفصلة `assertShellAllowed()`. | بوابات الكتابة وShell مفروضة داخل handlers وليست مجرد annotations. لكن تفعيل `ANDROID_MCP_ALLOW_SHELL=true` يفتح تنفيذ أمر Shell حر؛ يجب ألا يكون ذلك جزءاً من الإعداد الافتراضي للبرنامج. دوال `adbShell` تستقبل command string واحدة، وبالتالي يجب تدقيق كل موضع يبنيها لضمان escaping/validation؛ بعض مواضع Package Manager تتحقق من package name وبعضها يستخدم shell escaping. | **أفضل مرجع مبدئي لفصل الصلاحيات**. لا يُدمج كما هو قبل مراجعة جميع handlers، واختبار Windows، وتحديد مجموعة الأدوات التي نحتاجها فقط. |
| [iksnerd/adb-mcp](https://github.com/iksnerd/adb-mcp) | يستخدم `exec.CommandContext` وتمرير arguments كقائمة منفصلة؛ لديه طبقة ADB مستقلة عن MCP، وطبقة Tools رفيعة تستدعي Client. `ResolveSerial` يختار تلقائياً فقط عند وجود جهاز جاهز واحد؛ عند تعدد الأجهزة يطلب Serial صراحة. | هذا أقوى سلوك من ناحية منع اختيار جهاز عشوائي. توجد اختبارات مخصصة لـshell quoting ومدخلات عدائية. لكن الأدوات التي تنفذ Install/Uninstall وعمليات تفاعلية تحتاج مراجعة سياسة موافقة مركزية قبل إعادة استخدامها؛ وجود MCP annotations أو موافقات العميل وحدها لا يساوي حارس صلاحيات داخل الخدمة. | **أقوى مرجع معماري ومرشح لإعادة استخدام منطق ADB**، خصوصاً فصل Client عن MCP واختيار الجهاز. لا ننقل كل الأدوات الواسعة. |
| [qalvinahmad/android-mcp](https://github.com/qalvinahmad/android-mcp) | يستخدم `execFileSync` و`spawn` مع arguments منفصلة، ويدعم اكتشاف adb على Windows عبر `ANDROID_HOME` و`LOCALAPPDATA`. | عند عدم تحديد الجهاز: يستخدم `ANDROID_MCP_DEVICE`، ثم الجهاز الوحيد، ثم أول جهاز فعلي متصل، ثم أول جهاز متاح. هذا قد يوجه المهمة إلى هاتف غير مقصود عند وجود أكثر من جهاز. أدوات Install/Uninstall لا يظهر في handlers التي تمت مراجعتها حارس كتابة server-side؛ وannotations مثل `destructiveHint` هي metadata وليست enforcement. | **مفيد كمرجع صغير لاكتشاف ADB وعمليات UI**، لكنه غير مناسب كأساس آمن دون تغيير اختيار الجهاز وإضافة authorization داخل الخادم. |

#### تفاصيل تقنية مثبتة من الملفات المقروءة

**أولاً: us-all/android-mcp-server**
- `src/adb.ts`: يستخدم `execFile` لا `exec`، ويمرر ADB arguments كمصفوفة؛ هذا يقلل خطر تفسير مدخلات المضيف كأمر shell على Windows، لكنه لا يلغي مخاطر Android shell عند استخدام `adb shell`.
- `src/config.ts`: قيم السماح للكتابة وShell تكون false ما لم تضبط متغيرات البيئة صراحة.
- `src/tools/utils.ts`: يحتوي `assertWriteAllowed` و`assertShellAllowed`، إلى جانب validators و`shellEscape`.
- `src/tools/apps.ts`: يعرض قراءة Package list وPackage details، ويستخرج `firstInstallTime` و`lastUpdateTime` من `dumpsys package`. Install/Uninstall وLaunch/Stop تمر عبر write gate.
- `src/tools/shell.ts`: ينفذ الأمر المرسل كما هو بعد write-independent Shell gate؛ يجب إبقاء هذا النوع من الأدوات معطلاً في منتجنا.
- `src/tools/device.ts`: يدعم تمرير Serial في استدعاءات معلومات الجهاز، لكن بعض أوامر الاتصال/الفصل تستهدف ADB server دون Serial لأنها عمليات اتصال.
- `tests/apps.test.ts` و`tests/adb.test.ts` و`tests/shell.test.ts` موجودة. اختبار Shell الظاهر يختبر schema وقبول/رفض مدخلات، وليس إثباتاً شاملاً لمقاومة command injection.
- `.github/workflows/ci.yml`: CI الحالي يستخدم Ubuntu وNode 22 ويشغل build/tests، مع تثبيت ADB لبعض فحوص CI. هذا لا يثبت توافق Windows.

**ثانياً: iksnerd/adb-mcp**
- `internal/adb/adb.go`: ينفذ ADB عبر `exec.CommandContext` مع `CommandEnv` ويدعم Context للإلغاء والمهلة عبر السياق.
- `internal/adb/devices.go`: `ResolveSerial` يرفض حالة عدم وجود جهاز جاهز، ويرفض تعدد الأجهزة دون Serial صريح.
- `internal/tools/helpers.go`: كل Handler يحصل على Client مرتبط بجهاز محدد قبل تنفيذ الوظيفة؛ هذا فصل جيد بين اختيار الجهاز والتنفيذ.
- `internal/tools/register.go`: كتالوج MCP يسجل handlers رفيعة تصف متى تستخدم الأداة ومحاذيرها؛ منطق ADB يبقى في `internal/adb`.
- `internal/adb/packages.go`: يدعم ListPackages وInstallApp وUninstallApp وقراءة تفاصيل التطبيق/حالة التطبيق. بعض معلومات install/update time تستخرج من dumpsys ضمن AppState، لكن هذا لا يعني أن جميع بيانات الفرز المطلوبة متاحة في أداة قائمة التطبيقات الحالية.
- `internal/adb/shellquote_test.go`: توجد اختبارات لقيم تحتوي مسافات و& و; وapostrophes و$، وتتحقق من escaping في أوامر Shell الخاصة بالتطبيقات والروابط والقفل. هذا دليل اختبار وحدات على بناء الوسائط، لا اختبار اختراق شامل.
- وجود ملفات اختبارات كثيرة في `internal/adb` و`internal/tools` مؤشر على تغطية وحدات واسعة؛ لم يتم تشغيلها في هذه المراجعة.
- `go.mod` يثبت استخدام Go 1.26 وMCP Go SDK v1.6.1. ملفات Windows-specific وملفات Release workflow موجودة في الشجرة؛ يجب مراجعة الـworkflow ومخرجات الإصدار فعلياً قبل اعتماد حزمة Windows.

**ثالثاً: qalvinahmad/android-mcp**
- `src/adb.ts`: يستعمل `execFileSync` و`spawn` بوسائط منفصلة، ويدعم Windows paths. لديه auto-connect عبر mDNS عند عدم ظهور أجهزة في القائمة.
- `resolveDevice()`: الاختيار التلقائي يفضل أول جهاز فعلي على المحاكي عندما يوجد أكثر من جهاز؛ هذا قرار غير آمن لتطبيق إدارة أجهزة، لأن «أول جهاز» لا يعني الجهاز الذي قصده المستخدم.
- `src/server.ts`: يضع MCP annotations مثل `readOnlyHint` و`destructiveHint`، لكن الأدوات التي فحصناها Install/Uninstall لا تستدعي حارس موافقة أو policy gate داخل handler. لا ينبغي اعتبار metadata بديلاً عن enforcement.
- `android_list_apps` يعرض التطبيقات التي لها Launcher activity، وليس جرداً شاملاً لكل الحزم المثبتة؛ لذلك لا يفي وحده بمتطلب System/User package inventory.
- Install/Uninstall يستعملان arguments منفصلة، وUninstall يتحقق من صيغة Package name. لم يظهر في الملفات التي راجعناها مساراً عاماً لاستخراج install time/update time أو فلاتر تاريخ التثبيت.
- لم يظهر في شجرة الملفات المعروضة مجلد اختبارات أو CI workflow؛ لا نستنتج من ذلك عدم وجود اختبارات خارج الشجرة، لكن لا يوجد دليل اختبارات ظاهر في هذه المراجعة.

#### ما الذي نعيد استخدامه فعلياً؟

1. **من iksnerd:** تصميم طبقة ADB Client المستقلة عن MCP، وآلية Context/cancellation، وسياسة `ResolveSerial` التي ترفض الاختيار التلقائي عند تعدد الأجهزة، واختبارات shell-argument quoting كمرجع.
2. **من us-all:** نموذج server-side write gate وShell gate، وفصل إعدادات السماح، وطرق استخراج package metadata ووقت التثبيت/التحديث، مع تجنب فتح arbitrary shell.
3. **من qalvinahmad:** أفكار اكتشاف مسار ADB على Windows وmDNS wireless discovery، بعد تعديل اختيار الجهاز ليكون صريحاً وعدم السماح باختيار أول هاتف تلقائياً.
4. **لا ننسخ أي MCP Server بالكامل الآن.** الأفضل بناء خدمة ADB داخل برنامجنا، ثم إضافة MCP Adapter رفيع يستدعي نفس Service Layer التي تستخدمها الواجهة الرسومية.

#### فجوات لم تُحسم بعد
- مراجعة كل عمليات الكتابة في iksnerd والتحقق هل يوجد authorization موحد داخل الخادم أم اعتماد على MCP client/annotations/تكوين المستخدم.
- مراجعة بقية handlers في us-all، خصوصاً الملفات والـsystem settings وUI interaction، بحثاً عن shell interpolation أو أدوات ذات آثار جانبية لا تمر عبر write gate.
- فحص ملفات Release workflow وWindows packaging للمشروعين الأول والثاني، لا الاكتفاء بوجود ملفات Windows في المستودع.
- التحقق من إمكان جرد جميع الحزم مع system/user/disabled/installer/install time/update time، وليس Launcher apps فقط.
- مقارنة التغطية الفعلية لفلاتر التطبيقات، Split APK/APKS، تصدير APK، enable/disable، وWireless Pairing.
- لا يوجد حتى الآن تشغيل محلي أو تحقق على Windows أو هاتف حقيقي.

**حالة هذه الجولة:** [x] قراءة ملفات مصدر أساسية للمشروعات الثلاثة. [x] فحص مبدئي لاختيار الجهاز وبناء أوامر ADB. [x] تحديد أدلة الاختبارات الموجودة. [ ] تدقيق كامل لكل الأدوات الحساسة. [ ] تشغيل اختبارات أو بناء Windows. [ ] اعتماد أي كود لإعادة الاستخدام.


### 20.14 نتائج تدقيق الصلاحيات ومسارات بناء Shell (12 نوفمبر 2026)

هذه الجولة وسّعت الفحص من الملفات الأساسية إلى تعريفات الأمان، اختبارات annotations، ومواضع استخدام `adbShell`. ما زالت مراجعة كود ساكنة؛ لم يتم تنفيذ payloads أو اختبار استغلال.

#### 20.14.1 iksnerd/adb-mcp — تصنيف الأثر ليس حارس تنفيذ

**ما ثبت من الكود:**
- `internal/tools/annotations.go` يعرّف كل أداة كـobserve أو change أو destroy، ثم يحول التصنيف إلى MCP safety hints.
- التعليق داخل الملف يشرح صراحة أن هذه hints يستخدمها العميل ليقرر ما يمكن اعتماده تلقائياً وما يستدعي سؤال المستخدم.
- `internal/tools/annotations_test.go` يثبت أن كل أداة لها title وhints، وأن مجموعة الأدوات المدمرة محددة بالأسماء.
- `internal/tools/annotations_invariants_test.go` يختبر ألا توصف الأدوات ذات مدخلات التغيير بأنها read-only، وأن تمرير task/args حرة يصنف كـdestructive.
- في `internal/tools/apps.go`، handlers الخاصة بـ`installApp` و`uninstallApp` تتحقق من المدخلات وتحدد الجهاز ثم تستدعي ADB مباشرة؛ لم يظهر في هذه المسارات فحص authorization داخلي قبل التنفيذ.

**الاستنتاج الدقيق:** المشروع لديه نظام تصنيف أثر واختبارات اتساق جيدة، وهذا مفيد لتصميم واجهة MCP. لكنه لا يثبت وحده وجود حظر server-side لعمليات التغيير. لا نعتمد على annotations كطبقة أمان؛ يجب أن يكون لدى برنامجنا Permission/Approval Service داخل طبقة الخدمة، ويجب أن ترفض الخدمة العملية حتى لو استدعاها MCP client مباشرة.

#### 20.14.2 us-all/android-mcp-server — نقاط Shell تحتاج مراجعة/إصلاح قبل الاقتباس

**مسارات سليمة أو أفضل نسبياً:**
- `src/tools/apps.ts` يتحقق من Package name في `getPackageInfo` قبل إدخاله إلى `dumpsys package`.
- عمليات الكتابة التي تمت مراجعتها تستدعي `assertWriteAllowed()`، وملفات أخرى مثل `files.ts` تستخدم `validateDevicePath` و`shellEscape`.
- `src/tools/shell.ts` يضع arbitrary shell خلف `assertShellAllowed()`، لكنه يظل خطراً كبيراً إذا تم تمكينه.

**نقاط تستحق التحقق الأمني العاجل:**
- `src/tools/debug.ts`: `getMemInfo` يبني `dumpsys meminfo ${params.packageName}`، و`getGfxInfo` يبني `dumpsys gfxinfo ${params.packageName} framestats`. الـschemas تقبل packageName كسلسلة نصية، ولم يظهر في handlers المعروضة استدعاء `validatePackageName` أو `shellEscape` قبل interpolation.
- `src/resources.ts`: Resource template الخاص بأنشطة التطبيق يأخذ packageName من URI ثم يبني `dumpsys package ${packageName}`؛ لم يظهر في المقطع الذي تمت مراجعته تحقق مماثل قبل تمرير الأمر.
- هذه المواضع تستخدم `adbShell` الذي يمرر command string واحدة إلى `adb shell`. لذلك يوجد خطر محتمل لحقن أوامر داخل Android shell إذا وصلت محارف خاصة عبر المدخلات. لا نسجل ذلك كاستغلال مؤكد قبل اختبار معزول ومراجعة سلوك shell على Android، لكن لا يجوز اقتباس هذه الأجزاء كما هي.
- `tests/shell.test.ts` يختبر schema لأداة Shell الحرة، و`tests/utils.test.ts` يختبر بعض validators وescaping. لم يظهر في الاختبارات التي تمت مراجعتها اختبار عدائي مباشر يغطي packageName في `getMemInfo` أو `getGfxInfo` أو URI resource.
- `CONTRIBUTING.md` يطلب من المساهمين استخدام `assertWriteAllowed()` لكل أداة تغير حالة الجهاز و`assertShellAllowed()` للأوامر الحرة؛ هذه قاعدة مساهمة جيدة، لكنها لا تمنع أخطاء في handlers بعينها.

**القرار:** لا ننسخ `debug.ts` أو Resource template المذكورين قبل إصلاح التحقق والاختبارات. الأفضل ألا نبني Shell command من مدخل خارجي أصلاً إذا أمكن تمرير arguments منفصلة؛ وإذا كانت Android shell تفرض command string، نستخدم validators ضيقة وescaping موحداً ونضيف اختبارات hostile-input لكل handler.

#### 20.14.3 متطلبات أمان نضيفها إلى تصميم برنامجنا

1. كل عملية تغيّر حالة الهاتف تمر عبر Permission Service داخل طبقة الخدمة، لا عبر MCP annotations فقط.
2. الموافقة تكون مرتبطة بعملية محددة وجهاز محدد ومدخلات محددة، وتنتهي صلاحيتها بعد التنفيذ أو التعديل.
3. لا يُسمح باستدعاء install/uninstall/disable/enable/DNS change أو driver repair عبر مسار داخلي بديل يتجاوز الموافقة.
4. لا نسمح بـarbitrary shell كأداة عامة في النسخة الأساسية.
5. كل مدخل يدخل Android shell يجب أن يمر بتحقق خاص بنوعه، مع رفض افتراضي للمحارف غير المسموحة.
6. نضيف اختبارات injection على كل handler/resource يستقبل package name أو path أو URL أو اسم نشاط أو قيمة إعداد.
7. نختبر مباشرة أن write tools تُرفض عند عدم وجود Approval Grant حتى لو تم استدعاؤها عبر MCP transport أو GUI أو خدمة داخلية.
8. تبقى annotations وصفاً للعميل وواجهةً مساعدة، لا حدّاً أمنياً.

**حالة هذه الجولة:** [x] مراجعة نظام hints واختباراته في iksnerd. [x] مراجعة مسارات Shell الإضافية في us-all. [x] تحديد نقاط interpolation تستحق الإصلاح/اختبار injection. [ ] تنفيذ اختبارات استغلال معزولة. [ ] مراجعة بقية ملفات system/UI/resources. [ ] اعتماد أي كود خارجي.


### 20.15 تدقيق إضافي: أدوات النظام والملفات وواجهة الجهاز وتوزيعات Windows (12 نوفمبر 2026)

#### 20.15.1 us-all/android-mcp-server — تغطية الوظائف

**الوظائف المقروءة في المصدر:**
- System: قراءة البطارية والشبكة والإعدادات، تغيير Settings، تغيير حجم/كثافة الشاشة، إبقاء الشاشة مضاءة، Wi-Fi/mobile data، الاتجاه، قفل/محاولة فتح الشاشة، ADB forward/reverse.
- Files: list/pull/push/delete، مع مسارات ADB مباشرة لبعض العمليات ومسارات Android shell للبعض الآخر.
- UI: screenshot وUI hierarchy وtap/long press/swipe/drag/type/key events وتسجيل الشاشة.
- عمليات تغيير الإعدادات وWi-Fi وmobile data وعمليات UI التي راجعتها تستدعي `assertWriteAllowed()`. وعمليات push/delete تستدعيه أيضاً. هذا أقوى من مجرد MCP annotations، لكنه ما زال Boolean global gate وليس موافقة دقيقة مرتبطة بجهاز/عملية/مدخلات.

**ملاحظات أمنية محددة من الكود:**

1. **Key event — validator مستورد لكنه غير مستعمل في handler:**
   - `src/tools/ui.ts` يستورد `validateKeycode`، لكن `pressKey()` يبني `input keyevent ${keycode}` مباشرة دون استدعاء validator.
   - `pressKeySchema` يقبل أي string.
   - النتيجة: مدخل key غير موثوق يصل إلى Android shell command string. هذه ثغرة محتملة لبناء أمر غير مقصود/حقن shell، وليست استغلالاً تم تنفيذه.
   - لا يظهر اختبار مباشر لـ`pressKey` ضمن مقطع `tests/ui.test.ts` الذي تمت مراجعته.
   - **الإجراء المقترح:** استدعاء `validateKeycode(keycode)` بعد التطبيع، وتقييد schema، وإضافة اختبارات رفض لمحارف shell والـnewline والـwhitespace.

2. **Screen recording — اسم الملف وحجم الفيديو:**
   - `screenRecordStart()` يركب `fileName` و`size` في command string مثل `screenrecord --size ... /sdcard/...`.
   - schema يقبل `fileName` و`size` كسلاسل دون نمط تحقق ظاهر في الجزء المقروء.
   - **الإجراء المقترح:** السماح باسم ملف بسيط فقط دون slash أو shell metacharacters، والتحقق من `size` بصيغة رقمية صارمة مثل WIDTHxHEIGHT، وبناء مسار آمن.

3. **أوامر Shell أخرى:**
   - `inputText` يستخدم `shellEscape`، و`unlockDevice` يهرب PIN، و`changeSetting` يتحقق من key/value ثم يهرب القيمة.
   - tap/swipe/longPress تستخدم قيماً محولة إلى أرقام من Zod، ما يقلل مساحة حقن النص، لكن يلزم تحديد حدود منطقية للأحداث.
   - `getNetworkInfo` يعتمد على `head` و`grep` داخل Android shell؛ يلزم التحقق من توافق هذه الأدوات مع إصدارات Android المستهدفة، أو استخدام parsing في المضيف لتجنب الاعتماد على shell utilities.

4. **Files:**
   - `listFiles` و`deleteFile` يستعملان `validateDevicePath` و`shellEscape`.
   - `pullFile` و`pushFile` يمران المسارات كوسائط منفصلة إلى ADB، وهو أفضل من إدخالها داخل shell command.
   - `pushFile` و`deleteFile` محميتان بـwrite gate؛ `pullFile` قراءة/نسخ إلى المضيف ولا يظهر لها gate، لكنها تحتاج موافقة/سياسة خصوصية منفصلة في برنامجنا لأن سحب ملفات الهاتف قد يكشف بيانات شخصية.
   - يجب أن نفرق في منتجنا بين «قراءة بيانات تشخيصية» و«تصدير ملفات من الهاتف»؛ كلاهما قراءة من منظور الجهاز لكن أثر الخصوصية مختلف.

5. **Settings / Forwarding:**
   - عمليات تغيير Settings وForward/Reverse/Remove forwarding محمية بـwrite gate.
   - مخطط المنافذ يستخدم coercion إلى number، لكن لا يظهر في الجزء المقروء حد 1–65535 أو شرط integer لكل المنافذ؛ ينبغي فرض حدود رقمية صريحة قبل التنفيذ.
   - `removeForward` دون hostPort يحذف كل forward وreverse mappings؛ هذا سلوك واسع الأثر ويجب أن يتحول في برنامجنا إلى Preview واضح مع موافقة مستقلة، لا زر حذف عادي.

**قرار us-all بعد هذه الجولة:** لا نقتبس UI أو system handlers كما هي. نحتفظ بفكرة التقسيم الوظيفي، ونستفيد من validators وwrite gate بعد تدقيقها، لكن نضيف authorization أدق، device binding، validation لكل مدخل، واختبارات hostile-input تغطي كل handler يبني Android shell command.

#### 20.15.2 Windows packaging وبيئة التشغيل

| المشروع | ما يظهر في ملفات البناء/التوزيع | النتيجة |
|---|---|---|
| us-all/android-mcp-server | مشروع Node/TypeScript ينشر npm package وDocker image. CI والبناء والنشر في الملفات التي راجعناها يعمل على Ubuntu؛ يوجد اختبار إضافي لـAlpine/musl، ولا يظهر Windows runner أو Windows-specific release binary في workflow المقروء. | يمكن أن يعمل نظرياً على Windows عبر Node.js وADB إذا كانت dependencies متوافقة، لكن **Windows runtime غير مثبت في CI الذي تمت مراجعته**. لا يوجد دليل كافٍ على حزمة Windows جاهزة مستقلة. |
| iksnerd/adb-mcp | Release workflow يبني Go binaries لـWindows amd64 وarm64، ويضعها في ZIP، كما يبني Linux/macOS؛ يرفق checksums. الاختبارات `go test -race ./...` و`go vet ./...` تظهر في release gate على Ubuntu، ثم يتم cross-compile لـWindows. | **أفضل توزيع Windows جاهز بين المرشحين الثلاثة**. الـcross-compile دليل بناء موجه لـWindows في CI، لكنه ليس اختبار تشغيل فعلي على Windows؛ نحتاج Windows-native smoke test قبل الاعتماد النهائي. |
| qalvinahmad/android-mcp | حزمة npm تعتمد Node.js >=18، وتحتاج platform-tools/ADB. README يذكر مسار Android SDK على Windows، وطريقة تشغيل Node على ملف dist. لم يظهر في شجرة الملفات التي تمت مراجعتها Windows binary أو Release workflow أو CI workflow. | تشغيل محتمل على Windows بتثبيت Node وADB، لكن لا يوجد دليل CI/Release ظاهر يثبت Windows runtime. |

**ملاحظة مهمة:** وجود ملف `detach_windows.go` أو إعداد Windows path أو cross-compilation لا يعادل اختباراً على Windows حقيقي. وكذلك نجاح Go cross-build لا يثبت أن ADB path discovery أو process lifecycle أو STDIO transport تعمل سليماً على Windows.

#### 20.15.3 ترتيب المرشحين بعد مراجعة هذه الجولة

1. **iksnerd/adb-mcp — مرجع أول لطبقة ADB وتوزيع Windows:** أفضل دليل على Windows amd64/arm64 artifacts، وفصل طبقة ADB، واختيار جهاز صريح عند التعدد. لكن نحتاج طبقة موافقات خاصة بنا واختبار Windows فعلي.
2. **us-all/android-mcp-server — مرجع تقسيم أدوات وصلاحيات:** وظائف System/Files/UI غنية، لكن ظهرت نقاط إدخال غير محققة في `pressKey` و`screenRecordStart` تستلزم إصلاحاً واختبارات قبل اقتباسها. Windows غير مثبت في CI الذي تمت مراجعته.
3. **qalvinahmad/android-mcp — مرجع خفيف لاكتشاف ADB/Wi-Fi:** مناسب للمقارنة المعمارية، لكن الاختيار التلقائي للجهاز وغياب دليل CI/Release ظاهر يجعلان اعتماده أقل أولوية.

#### 20.15.4 بوابة قبول أي كود مقتبس

قبل نسخ أي مكون إلى مشروعنا، يجب أن ينجح في:
- اختبار Windows-native على Windows 10/11 (amd64)؛ arm64 إذا قررنا دعمه.
- اختبار اتصال جهاز واحد وجهازين مع رفض الاستهداف الضمني عند تعدد الأجهزة.
- اختبارات hostile-input لكل حقل يصل إلى Android shell، بما فيها keycode وfilename وscreen size وpackage/path.
- اختبارات تثبت رفض كل عملية كتابة دون Approval Grant داخلي.
- فصل عمليات سحب الملفات عن القراءة التشخيصية في الأذونات.
- timeout/cancellation وسلوك انقطاع USB/Wireless ADB.
- اختبار STDIO يثبت أن stdout لا يحتوي إلا رسائل MCP وأن السجلات تذهب إلى stderr.
- توثيق إصدار/Commit SHA للمكون المقتبس وإمكانية استبداله دون ربط بقية النظام به.

**حالة الجولة:** [x] مراجعة إضافية لملفات System/Files/UI في us-all. [x] تحديد مسارات إدخال تحتاج اختبارات/إصلاح. [x] مراجعة workflow التوزيع والنشر للمشروعات الثلاثة. [ ] Windows-native execution. [ ] اختبارات injection معزولة. [ ] اعتماد كود خارجي.


### 20.16 تدقيق دورة حياة MCP وADB وSTDIO (12 نوفمبر 2026)

#### iksnerd/adb-mcp — نقاط قوة تشغيلية ظهرت في المصدر

- `cmd/adb-mcp/main.go` يشغل MCP عبر `mcp.StdioTransport`، ويتعامل مع إغلاق stdin/EOF كإغلاق طبيعي، ويمرر SIGINT/SIGTERM إلى Context للإلغاء.
- تسجيل الخادم لا يكتفي بإنشاء الأدوات؛ عند الإغلاق يستدعي `cleanup()` لإيقاف عمليات Gradle الجارية، وإيقاف جلسات التقاط logcat/screen recording، ومحاولة استعادة بعض تغييرات حالة الجهاز التي سجلها الخادم.
- أخطاء الاستعادة تذهب عبر Go logger، الذي يستخدم stderr افتراضياً، بينما STDIO transport مخصص لبروتوكول MCP. هذا فصل جيد يجب الحفاظ عليه.
- `internal/adb/adb.go` يوفر Runner interface ويحقن تنفيذ ADB؛ الاختبارات تستطيع تمرير Fake Runner دون تشغيل ADB أو توصيل هاتف. هذا يجعل اختبار command construction وtimeouts أسهل.
- `exec.CommandContext` ينفذ ADB بوسائط منفصلة ويتيح الإلغاء عند انتهاء Context.
- يوجد ملف Windows-specific `detach_windows.go`، ويجعل detach no-op على Windows لأن المحاكي يبدأ دون controlling session بحسب تعليق المشروع. هذه معالجة خاصة بالمنصة، لكنها ليست بديلاً عن اختبار lifecycle على Windows.

**تحفظ:** cleanup يحاول استعادة Overrides المعروفة فقط، ولا يضمن استعادة كل حالة يمكن أن تتغير خارج سجل الخادم. لذلك لا نصمم في منتجنا عمليات طويلة أو تغييرات دائمة على افتراض أن إغلاق MCP سيعيد الهاتف دائماً إلى حالته السابقة.

#### us-all/android-mcp-server — طبقة تنفيذ المضيف

- `src/adb.ts` يستخدم Node `execFile` مع arguments array وtimeout افتراضي 30 ثانية؛ لا يستخدم `exec` لتشغيل أمر المضيف كسطر Shell. هذا يقلل مخاطر حقن أوامر Windows host.
- لكن `adbShell(command)` يرسل command string واحدة بعد `adb shell`؛ لذلك يظل بناء Android shell command مسؤولية كل handler، ولا يحل `execFile` مشكلة الحقن داخل Android shell.
- `src/config.ts` يجعل `allowWrite` و`allowShell` false افتراضياً، ويختبر وجود ADB عند بدء التشغيل.
- `src/index.ts` يستخدم MCP server ويغلف handlers عبر `wrapToolHandler` أو `wrapImageToolHandler`. يجب تدقيق wrappers/runtime عند اعتماد المشروع للتأكد من أن stdout لا يتلقى رسائل logging غير بروتوكولية.
- لم نثبت بعد سلوك الإغلاق وإلغاء العمليات طويلة الأمد أو استعادة الحالة؛ يلزم اختبار عملي قبل النقل.

#### أثر ذلك على تصميم برنامجنا

1. استخدم Runner/Process Adapter موحداً يدعم arguments منفصلة وtimeouts وcancellation وcaptured stdout/stderr.
2. اجعل MCP transport وحده يملك stdout؛ كل logs/diagnostics تذهب إلى stderr أو ملف سجل منقح.
3. اربط كل عملية طويلة بـCancellationToken/Context؛ عند قطع اتصال Codex يجب أن نحدد هل نوقف العملية أم نسمح لها بالاكتمال، حسب نوعها.
4. سجل التغييرات القابلة للاستعادة قبل تنفيذها، وحدد بوضوح العمليات التي لا يمكن rollback لها.
5. لا تجعل cleanup الآلي يخفي فشل الاستعادة؛ اعرض تقريراً للمستخدم بما تم استعادته وما بقي مطبقاً.
6. اختبر إغلاق العميل، قتل العملية، انقطاع ADB، وفشل الاستعادة على Windows فعلياً.

**حالة الجولة:** [x] مراجعة دورة حياة MCP في iksnerd. [x] مراجعة ADB process adapter في المشروعين. [x] توثيق فصل STDIO/logging المطلوب. [ ] اختبار Windows-native. [ ] اختبار انقطاع العميل أثناء عملية طويلة. [ ] اختبار الاستعادة بعد crash أو force-kill.


### 20.17 تدقيق إدارة التطبيقات وWireless ADB وتكامل Codex في iksnerd/adb-mcp (12 نوفمبر 2026)

هذه مراجعة Source-level فقط؛ لم يتم تشغيل المشروع أو توصيل هاتف أو تنفيذ أوامر تغيير.

#### 20.17.1 إدارة التطبيقات والتواريخ

الملفات المقروءة:
- `internal/tools/apps.go` — SHA `e55a316d3e36015fdd2e2d4eaf50ade595bdfd0f`
- `internal/adb/packages.go` — SHA `72d2cb9f033003ab83a13914f989a932200ca4f5`
- `internal/adb/packages_test.go` — SHA `0206622d201de030880cc59ce73c2ec90d1292c9`
- `internal/adb/permissions.go` — SHA `5b963a6301163a0bb57e620c247f66e6a506b4cf`
- `internal/tools/register.go` — SHA `66013675991ea743e9f3f9dba2b6c9de8c215a39`
- `internal/tools/annotations.go` — SHA `c29e4f85a4ec6d817e112e79f34fdbc4e3cfca07`

**المثبت في المصدر:**
- `list_packages` ينفذ `pm list packages` ويطبق substring filter على package names.
- `get_app_details` يعرض version name/code وlauncher activity.
- `app_state` يعرض runtime state ويستخرج first install time وlast update time من `dumpsys package` لتطبيق محدد.
- `packages_test.go` يتضمن `TestParseInstallTimes` للتحقق من تحليل التاريخين من عينة dumpsys.
- توجد أدوات install/uninstall، launch/stop، clear app data، grant/revoke permission، وpush/pull files.

**الفجوات بالنسبة لاحتياجنا:**
- لا يظهر في `list_packages` جرد غني يصنف System/User أو Enabled/Disabled أو installer أو UID أو paths أو تاريخ التثبيت.
- وجود parser للتاريخين في app_state لا يعني وجود bulk inventory أو filters/sorting حسب تاريخ التثبيت لكل التطبيقات.
- `get_app_details` لا يعرض installer أو install/update timestamps في بنية التفاصيل التي راجعتها.
- لا يظهر في الكتالوج workflow متخصص يجمع كل مسارات APK المثبتة ويصدر Split APK set؛ `pull_file` نقل عام لملف، وليس Export Installed App.
- `get_app_details(apk_path)` يقرأ metadata من APK محلي عبر aapt2؛ لا يستخرج APK من الهاتف.
- لا يظهر دعم متكامل لـ`adb install-multiple` أو APKS/bundletool في مسار install الذي راجعته.

**قرار التصميم:** نحتاج Inventory Service يجمع metadata على مستوى الحزمة، مع حقول unknown/unsupported واضحة، ثم فلاتر وفرز محلي. نفصل `inspect_apk` و`export_installed_app` و`install_apk_set`. التصدير يجمع جميع split paths، يعرض Preview وعدد الملفات والحجم ومكان الحفظ، ويطلب موافقة منفصلة لأنه ينقل بيانات من الهاتف إلى الكمبيوتر. لا يشمل بيانات التطبيق الخاصة.

#### 20.17.2 صلاحيات التطبيقات: annotations ليست Authorization

- `grantPermission` و`revokePermission` في `internal/adb/permissions.go` يستدعيان `pm grant` و`pm revoke`.
- handlers في `internal/tools/apps.go` تتحقق من عدم فراغ package/permission ثم تحل الجهاز وتنفذ العملية؛ لم يظهر Approval Service أو authorization gate داخلي في المسارات المقروءة.
- `annotations.go` يصنف grant كـchange وrevoke كـdestroy ويحول التصنيف إلى MCP safety hints. هذه metadata للعميل وليست منعاً server-side.
- الملاحظة تنطبق كذلك على install/uninstall/clear data وغيرها من عمليات التغيير.

**القرار:** لا ننقل هذه handlers كما هي. نحتاج Permission Service داخل طبقة الخدمة، وموافقة مرتبطة بـSerial + العملية + Package + permission/arguments المحددة. تنتهي الموافقة عند تغير أي مدخل، ولا يجوز أن يتجاوز MCP أو GUI هذه الطبقة.

#### 20.17.3 Wireless ADB

- `internal/adb/devices.go` — SHA `0030ae0b279da7872e734a31c6012a3e8e57f0df` — يحتوي `ConnectWireless`.
- الدالة تدعم `adb pair` باستخدام pairing address/code، ثم `adb connect` باستخدام connect host:port؛ وتسمح بفصل عنوان الاقتران عن عنوان الاتصال لأن المنفذين قد يختلفان.
- `docs/TOOLS.md` يوثق `connect_wireless`.
- `ResolveSerial` يرفض اختيار أول جهاز عشوائياً عند تعدد الأجهزة ويطلب Serial صريحاً.

**مرجع جيد لإعادة الاستخدام الوظيفي**، مع متطلبات إضافية عندنا: عدم تسجيل pairing code، Preview للجهاز والعنوان، اختيار Serial صريح بعد الاتصال، Disconnect لا يزيل اتصالات أخرى دون موافقة، واختبارات تغير IP وفشل الاقتران وانقطاع الشبكة على Windows.

#### 20.17.4 Codex Plugin

الملفات المقروءة:
- `plugin/adb-mcp-codex/.codex-plugin/plugin.json` — SHA `b7478af8fef770f65c0eb068195d034694f061ba`
- `plugin/adb-mcp-codex/.mcp.json` — SHA `d95bc4be20e59212430a6dde98b22a35092858b3`

المشروع يضم Codex plugin باسم `adb-mcp`، ويعلن Read/Write capabilities، ويحدد MCP server محلياً بالأمر `adb-mcp` دون arguments، ويشير إلى Skills ضمن الإضافة. هذا دليل على وجود مسار تكامل Codex منظم يجمع plugin metadata وMCP configuration وتعليمات الوكيل.

لكن هذا لا يثبت نجاح التثبيت والتشغيل على كل إصدارات Codex/Windows، ولا يجعل Read/Write capability أو safety hints سياسة موافقات server-side. لا ننقل كتالوج أدواته الواسع كما هو؛ نحتاج MCP tools مخصصة لإدارة الأجهزة والتطبيقات وDNS والتعريفات، وكلها تستدعي Service Layer نفسها المستخدمة من GUI.

#### 20.17.5 Windows CI مقابل Windows runtime

ملف `.github/workflows/release.yml` — SHA `ed1927aa1ae0fb9d4570bbf5396cd4aa47c869aa`:
- يعمل عند دفع version tag، وليس push/PR CI مستمراً.
- الاختبارات و`go vet` تعمل على `ubuntu-24.04`.
- ينفذ `go test -race ./...` ثم يبني Windows amd64 وarm64 binaries من Ubuntu باستخدام GOOS/GOARCH ويضع ملفات Windows داخل ZIP مع checksums.
- لا يظهر Windows runner أو تشغيل اختبارات على Windows.

**الاستنتاج:** Windows artifacts وcross-compilation مثبتان في workflow؛ Windows-native runtime testing غير مثبت. قبل اعتماد المشروع نحتاج Windows 10/11 job يشغل tests وsmoke tests، ويتحقق من ADB discovery، STDIO stdout/stderr، Codex plugin launch، multi-device targeting، cancellation وانقطاع ADB.

#### 20.17.6 قرار إعادة الاستخدام بعد الجولة

| الوظيفة | القرار |
|---|---|
| ADB runner وarguments منفصلة | مرجع معماري قوي؛ اختباره على Windows ما زال مطلوباً |
| اختيار Serial ورفض الاختيار العشوائي | اعتماد السلوك كمطلب تصميم |
| Wireless Pair/Connect | اعتماد الفكرة بعد إخفاء pairing code وإضافة Preview/Disconnect |
| Package inventory | `list_packages` غير كافٍ؛ نحتاج Inventory metadata أوسع |
| Install/update timestamps | إعادة استخدام فكرة parser مع اختبارات Android versions؛ ليس bulk filtering جاهزاً |
| Grant/Revoke permission | لا ننقل handlers قبل server-side authorization |
| Export APK/Split APK | لا يوجد workflow متخصص مثبت؛ نبحث عن مكون مستقل أو نبني Adapter حول ADB/bundletool |
| Codex plugin | مرجع لتجربة الربط والتوزيع، لا مرجع جاهز لسياسة الأمان |
| Windows | cross-build موجود؛ Windows runtime test غير مثبت |

**حالة الجولة:** [x] مراجعة إدارة التطبيقات والصلاحيات. [x] مراجعة parser واختبار تواريخ التثبيت والتحديث. [x] مراجعة حدود تصدير APK. [x] مراجعة Wireless Pair/Connect. [x] مراجعة Codex plugin. [x] مراجعة Windows release workflow. [ ] Windows-native tests. [ ] اختبار Codex على Windows. [ ] اختبار USB/Wireless فعلي. [ ] اعتماد أي كود مقتبس.

 
### 20.18 تدقيق Droidsmith لإدارة التطبيقات وتصدير Split APK (12 نوفمبر 2026)

المستودع: https://github.com/SysAdminDoc/Droidsmith  
الفرع: main — آخر commit ظاهر: `c46341525239adb887cf65322fae905cbc189757` بتاريخ 30 يوليو 2026.  
README يصفه كتطبيق Desktop مبني على Tauri لـWindows/macOS/Linux، ونسخة المصدر المعلنة 0.9.12. الملاحظة في README عن آخر binary منشور أقدم من المصدر؛ لذلك لا نخلط بين حالة Source وRelease artifacts.

هذه مراجعة للكود والـCI فقط؛ لم يتم بناء التطبيق أو تشغيله أو اختباره مع هاتف.

#### 20.18.1 جرد التطبيقات

الملف `src-tauri/src/adb/packages.rs` — SHA `634437ff3a5cf7bef6457d64cfb252a5a71136cb`.

Droidsmith يجمع:
- `pm list packages -e` و`-d` لتمييز Enabled/Disabled ودمج النتيجة.
- `-f` لمسار APK، و`-U` لـUID، و`-i` لمصدر التثبيت.
- `--user N` لتحديد Android user.
- `-u` لاكتشاف archived/retained packages.
- fallback عند رفض بعض نسخ Android/OEM للخيارات الإضافية.
- فلاتر All/User/System/Enabled/Disabled/Archived/Retained.

بنية AppPackage تضم package وenabled وsystem وapk_path وuid وinstaller وarchived وretained. وتوجد اختبارات parsing ودمج القوائم وfallback والفلاتر.

**ملاحظة دقة:** حقل system في parser يستند إلى مسار APK. تحديث تطبيق نظام قد يجعل مساره الحالي في `/data/app`، وبالتالي قد يخطئ هذا الـheuristic في تصنيفه. الكود يستخدم في مسار آخر `pm list packages -s` لتقييم قابلية الاستعادة. لمنتجنا يجب فصل المسار عن System classification واستخدام دليل PackageManager الأقوى أو إظهار الحالة غير محسومة عند تعارض الإشارات.

**الفجوة:** AppPackage لا يحتوي firstInstallTime أو lastUpdateTime، ولا توجد فلاتر تاريخ في نموذج الجرد الذي تمت مراجعته. نحتاج enrichment مستقل لهذه الحقول ثم date filters محلية.

#### 20.18.2 أسماء التطبيقات والأيقونات

- `src-tauri/src/apk_metadata.rs` — SHA `b0c9ccb2deba5bbbd873f5b607727e05cc34c2ca`
- `src-tauri/src/commands/packages.rs` — SHA `47a5fab70785a5ecb807f487d0bef6068a9a7e54`

النمط المستخدم: جلب package inventory أولاً، ثم طلب label/icon بشكل lazy عند اقتراب صف التطبيق من الشاشة. يقرأ `pm path`، ويتحقق من حجم/وقت APK، ويسحب ملفاً مؤقتاً بحد حجم وحد للتوازي، ويستخدم Cache مربوطاً بالجهاز والـtransport generation وAndroid user والحزمة وهوية APK. الملف المؤقت يُحذف بعد الاستخدام.

هذا نمط جيد لتسريع الجرد وتقليل عمليات السحب، لكنه لا يوفر install/update dates.

#### 20.18.3 تصدير APK وSplit APKs

الملف `src-tauri/src/backup.rs` — SHA `02e932240fd7fd64ea79c3c581d52c68ca429ff4`.

يوجد workflow متخصص:
1. Preflight يقرأ `pm path --user N <package>` ويجمع كل المسارات.
2. يتحقق من سلامة المسارات، ويرفض القائمة الفارغة أو أكثر من 64 جزءاً.
3. يسحب كل base/split APK على حدة باستخدام ADB arguments منفصلة.
4. ينشئ ZIP يحتوي كل الأجزاء و`manifest.json`.
5. يسجل لكل artifact الاسم والدور والحجم وSHA-256.
6. يضيف device/build identity hashes بدلاً من serial أو fingerprint الخام.
7. يستخدم staging وcommit ذري للملف النهائي، ويحذف مجلد العمل المؤقت.
8. يدعم تقدم العملية وإلغاءها عبر Operations.

**النتيجة:** هذا أقوى مرجع مباشر وجدناه لوظيفة Export Installed App/Split APK. وهو APK export فقط، وليس نسخة كاملة من بيانات التطبيق الخاصة. مسار Android backup القديم منفصل ومقيد بأدلة الأهلية، ولا يضمن الاستعادة.

#### 20.18.4 Wireless ADB

- `src-tauri/src/adb/wireless.rs` — SHA `5fec47cc2c4ea235a83ec899ac5cc925c28e0789`
- `src-tauri/src/commands/wireless.rs` — SHA `0fdabb3f7fd291c5d7e79ed593f7787b06e3487e`
يدعم mDNS service discovery والتمييز بين Pairing وConnect، و`adb pair` و`adb connect`، والتحقق من endpoint والـport وpairing code، وتصنيف TLS wireless مقابل legacy TCP، وتشخيص بعض مشاكل VPN/mDNS. لا يمنح TLS provenance لمجرد إدخال IP يدوياً.
مرجع قوي لتصميم الاتصال اللاسلكي، مع ضرورة حماية pairing code من السجلات وإضافة Disconnect وإدارة واضحة للاتصالات.

#### 20.18.5 حدود الأمان والمعمارية

- `AdbTransport` يفصل النقل عن الخدمات، مع MockTransport للاختبارات دون جهاز.
- DeviceTarget يربط Serial وtransport identity/generation لتقليل خطر استمرار عملية على اتصال تغيّر.
- package actions تمر عبر planned actions وjournal intent/outcome.
- عمليات install/export تستخدم native one-shot path grants بدلاً من قبول أي مسار من الواجهة.
- توجد عمليات طويلة مع تقدم وإلغاء وحدود للوقت/المخرجات.

هذا قريب من المعمارية المطلوبة، لكن لا نعتبره بديلاً تلقائياً عن Permission Service موحدة عند إضافة MCP. يجب تدقيق كل action والـpath grant قبل اقتباس أي كود.

#### 20.18.6 Windows CI

الملف `.github/workflows/ci.yml` — SHA `757832dbd7ff4e33dc14a559eb86d5a753ba2175`.

يوجد native matrix على Ubuntu وWindows وmacOS. على Windows runner ينفذ workflow `cargo check` و`cargo clippy` و`cargo test` لكل targets/features. هذا دليل أقوى من cross-compilation من Linux فقط، لكنه لا يثبت اختباراً مع هاتف USB/Wireless ولا تكامل Codex/MCP.

لا توجد ملفات MCP في شجرة Droidsmith التي راجعتها؛ بالتالي سيكون MCP adapter إضافة مستقلة فوق Service Layer، وليس جزءاً جاهزاً منه.

#### 20.18.7 مقارنة ADB-Root-Toolbox

المستودع `Bingblop/ADB-Root-Toolbox` هو تطبيق يعمل على هاتف Android نفسه، وليس Windows desktop host manager. لذلك يصلح مرجعاً لبعض أفكار UX وإدارة التطبيقات فقط.

- `SdmPackagesAndroid.java` — SHA `fdd505155347cac200ef3b4724b654b65e0eeb06`: يستعمل Android PackageManager لجرد التطبيقات وقراءة label/system/enabled/UID/version مع caching.
- `ApkScan.java` — SHA `8c37dbefc9cf0c64500c8a3c3a26e58752b98f9`: يبحث عن ملفات APK/APKS/APKM/XAPK الموجودة على تخزين الهاتف بحدود للعمق والوقت والعدد.
- README يوضح أنه يعمل على الهاتف نفسه؛ اكتشاف ملفات APK المخزنة ليس هو استخراج APK مثبت من PackageManager.

#### 20.18.8 قرار إعادة الاستخدام

| الوظيفة | النتيجة | القرار |
|---|---|---|
| Windows desktop + ADB | Droidsmith/Tauri | مرشح مرجعي أول لطبقة الخدمات |
| App inventory | enabled/system heuristic/path/UID/installer/user/archive | أقوى نموذج مرشح؛ نراجع دقة System classification |
| App labels/icons | lazy metadata pull/cache | اعتماد النمط |
| Install/update dates | غير موجودة في AppPackage | إضافة مستقلة |
| Export installed APK | base + splits + manifest + hashes | أفضل مرجع مباشر؛ Adapter أو اقتباس محدود بعد تدقيق الأمان |
| Wireless ADB | mDNS + Pair/Connect + TLS/legacy provenance | مرجع قوي |
| Windows CI | native Windows runner + Rust tests | دليل جيد على CI، وليس hardware testing |
| MCP/Codex | غير موجود في المستودع | بناء MCP adapter منفصل فوق Service Layer |
| ADB-Root-Toolbox | تطبيق Android-side | مرجع UX فقط، ليس مكون Windows |

**التوصية المؤقتة:** Droidsmith هو المرشح المرجعي الأول حالياً لإدارة ADB والتطبيقات وتصدير Split APKs على Windows. لا أوصي بنسخ المستودع كاملاً. نراجع security boundary وpath grants وpackage export تفصيلياً، ثم نحدد وحدات يمكن اقتباسها أو تغليفها خلف Adapter. آخر commit ظاهر يعود إلى يوليو 2026، لذا نحتاج إعادة تقييم نشاط الصيانة قبل اعتماد طويل الأجل.

**حالة الجولة:** [x] package inventory والفلاتر. [x] lazy APK metadata. [x] تصدير base/split APK وmanifest/hashes. [x] Wireless ADB/mDNS. [x] Windows native CI matrix. [x] تصنيف ADB-Root-Toolbox كتطبيق Android-side. [ ] تدقيق كامل لـauthorization/path grants. [ ] تشغيل Windows build/tests محلياً. [ ] اختبار USB/Wireless فعلي. [ ] تصميم MCP adapter. [ ] اعتماد اقتباس كود.


### 20.19 مراجعة MCP في Droidsmith 0.9.19 وتصحيح مرجع الفرع (12 نوفمبر 2026)

**تصحيح مهم للقسم 20.18:** المستودع الافتراضي الحالي هو `master` وليس `main`. المراجعة السابقة لملف `.github/workflows/ci.yml` كانت على الفرع `main`، وهو ليس الفرع الافتراضي الحالي. الفرع الافتراضي `master` عند SHA `a56496cc06a88ebf689b3c2f0fd79de12c605285` (14 سبتمبر 2026)، ولا يحتوي شجرته على `.github/workflows/`. كما يسجل CHANGELOG إزالة hosted build workflow. لذلك لا يصح اعتبار Windows native CI matrix المذكور في 20.18 دليلاً على CI الحالي للمنتج؛ هو دليل من فرع غير افتراضي. يجب اعتبار Windows runtime CI الحالي **غير مثبت** إلى أن يظهر workflow نشط أو تشغيل موثق على Windows.

المراجعة الحالية تمت على الفرع الافتراضي `master`، ونسخة المنتج `0.9.19` بحسب `package.json` و`src-tauri/Cargo.toml`. آخر commit ظاهر على الفرع: `a56496cc06a88ebf689b3c2f0fd79de12c605285` بتاريخ 14 سبتمبر 2026.

#### 20.19.1 MCP موجود فعلاً في الفرع الافتراضي

الملفات:
- `src-tauri/src/mcp.rs` — blob SHA `acd2c36f76a021a81143d72cffdf2d8bbb80d09b`
- `src-tauri/src/bin/droidsmith_mcp.rs` — blob SHA `52fb4d87989e139495500fa6ab3dffe26547fcdd`
- `src-tauri/tests/mcp_smoke.rs` — blob SHA `9cc571692d000fd68f35aed38c83466a99485c7d`
- `src-tauri/Cargo.toml` — blob SHA `bc1bc4a1a16b4a2d5db277bd89dcb546e3c2d8af`
- `CHANGELOG.md` — blob SHA `86e15a92a8fb91f522bb36ae6d49cbe7e7e9d36a`

يعلن CHANGELOG 0.9.19 بتاريخ 5 سبتمبر 2026 عن `droidsmith-mcp` كواجهة MCP محلية عبر STDIO. صفحة README الحالية تصف نفس السلوك، وتقول إنه لا يفتح HTTP listener، وإن أدوات التغيير تحتاج `confirmed: true`.

**التصميم الفعلي في المصدر:**
- MCP server مكتوب Rust كـbinary مستقل `droidsmith-mcp`.
- ينفذ JSON-RPC/MCP عبر UTF-8 JSON line على STDIN/STDOUT.
- لا يفتح listener؛ transport محلي STDIO فقط.
- يترك STDOUT حصرياً لرسائل البروتوكول، وأخطاء تشغيل binary تذهب إلى STDERR.
- يمرر التنفيذ إلى `droidsmith-cli` الشقيق بدلاً من استدعاء ADB من MCP مباشرة.
- يحدد CLI المجاور تلقائياً، مع override اختياري عبر متغير البيئة `DROIDSMITH_CLI`.
- يستخدم arguments منفصلة مع `Command::args`، لا shell command string.
- يعلن MCP protocol version `2025-06-18`.

هذا يثبت وجود MCP في الفرع الافتراضي، ويصحح المعلومة السابقة التي قالت إنه غير موجود.

#### 20.19.2 الأدوات التسع الموجودة — نطاق MCP أضيق من التطبيق

`tools/list` يسجل الأدوات التالية:
1. `devices_list` — اكتشاف الأجهزة.
2. `packages_list` — جرد الحزم لجهاز ومستخدم Android محددين، مع filters.
3. `profile_plan` — إنشاء خطة Profile دون تنفيذ.
4. `baseline_inspect` — مقارنة recovery baseline بالحالة الحية.
5. `fleet_report_read` — قراءة تقرير محلي منقح.
6. `pack_plan` — خطة debloat pack دون تنفيذ.
7. `profile_apply` — تطبيق Profile.
8. `baseline_apply` — تطبيق خطة استعادة/إعادة تطبيق baseline.
9. `pack_apply` — تطبيق debloat pack.

أدوات القراءة تحمل `readOnlyHint=true`، وأدوات التغيير تحمل `readOnlyHint=false` و`destructiveHint=true`.

**ما لا يقدمه MCP الحالي:** لا توجد أدوات مستقلة مباشرة لـinstall/uninstall/enable/disable/permission changes، ولا تصدير APK، ولا DNS، ولا Windows driver repair، ولا Wireless Pair/Disconnect، ولا logcat أو file operations. هذه الوظائف موجودة في التطبيق بدرجات مختلفة، لكنها ليست ضمن كتالوج MCP التسع الذي راجعناه.

إذن MCP في Droidsmith ليس تحكماً كاملاً بكل واجهات البرنامج؛ هو واجهة أتمتة headless لعدد محدد من workflow profiles/baselines/packs والتقارير.

#### 20.19.3 تدقيق confirmed=true — حارس موجود لكنه ليس موافقة بشرية مربوطة بالخطة

في `mcp.rs`:
- `profile_apply` و`baseline_apply` و`pack_apply` ترفض التنفيذ إذا لم يكن `confirmed` قيمة Boolean true.
- توجد اختبارات وحدات لكل الأدوات الثلاث تتأكد من الرفض قبل تشغيل CLI عند غياب التأكيد.
- يوجد Smoke test يشغل MCP binary، ويتأكد من أن stdout يحوي JSON-RPC lines فقط، وأن طلب `profile_apply` دون confirmation يرجع `confirmation_required`.
- CLI يعيد تطبيق validation/planner/journal الخاصة به، وbaseline apply يعيد حساب diff مع الحالة الحية ولا ينفذ خطة قديمة مباشرة.

**الحد الأمني الدقيق:** `confirmed` مجرد Boolean يرسله مستدعي MCP. لا يظهر في MCP adapter إصدار Approval Token من الخادم بعد عرض plan محدد، ولا ربط confirmation بـhash الخطة أو Serial + Android user + package/action set + transport generation، ولا expiry/one-shot approval. وبالتالي يمكن لعميل MCP أن يرسل `confirmed:true` في أول استدعاء دون أن يكون الخادم قد تحقق بنفسه أن الإنسان راجع نتيجة plan السابقة. وصف الأداة يقول "after reviewing its plan"، لكن هذا الشرط procedural/contractual وليس إثبات موافقة بشرية داخل الخادم.

هذا لا يعني أن التنفيذ بلا حواجز: CLI يعيد validation وفحوص التوافق، ويحتفظ بالـjournal، وتوجد acknowledgements منفصلة لبعض الحالات مثل unsafe tier/compatibility. لكنه لا يجعل Boolean وحده إثبات موافقة بشرية دقيقة.

**المطلب عندنا:** لا نعتمد `confirmed:true` وحدها. نحتاج workflow:
1. MCP read-only preview يرجع plan + digest.
2. خدمة الصلاحيات تنشئ Approval Token قصيرة العمر، أحادية الاستخدام، مرتبطة بجهاز/عملية/مدخلات/خطة محددة.
3. واجهة المستخدم (GUI أو MCP client approval surface) تعرض الملخص الدقيق وتطلب موافقة صريحة.
4. apply يقبل token لا Boolean، ويتحقق من digest وحالة الجهاز والـtransport generation ثم يستهلكه.
5. أي اختلاف في الخطة أو الجهاز يبطل الموافقة ويعيد Preview.

#### 20.19.4 حدود التشغيل والـLifecycle

في `run_cli`:
- `Command::new(...).args(...)` مع stdin null وstdout/stderr piped.
- استدعاء `.output()` متزامن ينتظر اكتمال العملية.
- لا يظهر timeout أو cancellation token في MCP adapter نفسه.
- لا توجد في هذا المسار إدارة MCP request cancellation لتنفيذ child CLI؛ إشعار `notifications/cancelled` يتم تجاهله دون إيقاف العملية التي بدأت.
- لا يوجد حد واضح لزمن انتظار child CLI في الدالة التي تمت مراجعتها.

هذا فرق مهم عن طبقة Operations في التطبيق، التي تملك cancellation/timeouts لعملياتها. MCP adapter قد يبقى منتظراً إذا علق CLI؛ يجب إضافة bounded process runner وربط MCP cancellation بإيقاف child process قبل اعتباره مناسباً للمهام الطويلة.

#### 20.19.5 الاختبارات وحدود الدليل

المثبت:
- Unit tests لتهيئة MCP وأشكال JSON-RPC/الأدوات.
- Unit tests لرفض أدوات التغيير دون confirmed.
- Unit tests لرفض selectors الغامضة أو غير الصحيحة.
- Integration smoke test لتشغيل binary والتحقق من STDOUT protocol-only ورفض apply دون confirmation.

غير المثبت من الملفات التي راجعناها:
- Windows-native CI workflow على الفرع الافتراضي؛ لا يوجد `.github/workflows` في شجرته.
- اختبار Codex الفعلي لتثبيت/تشغيل plugin؛ المشروع يقدم MCP executable لكن لا يظهر Codex plugin package خاص به.
- اختبار إلغاء child process من MCP.
- اختبارات موافقة بشرية مرتبطة بخطة ذات digest.
- اختبار hardware مع USB/Wireless device في هذا التدقيق.

#### 20.19.6 القرار بالنسبة لبرنامجنا

| البند | Droidsmith 0.9.19 | قرارنا |
|---|---|---|
| MCP local STDIO | موجود ومخصص | مرجع مناسب للنقل المحلي |
| MCP over HTTP | غير موجود، ولا حاجة له في البداية | نبدأ STDIO مثل هذا التصميم |
| فصل MCP عن ADB | MCP يستدعي CLI، والـCLI يستعمل طبقة الخدمات | نستخدم MCP adapter فوق Service Layer؛ لا نكرر ADB |
| Read-only tools | devices/packages/plans/reports | نمط مناسب |
| Mutation tools | profile/baseline/pack apply | نقتبس نمط التخطيط، لا Boolean confirmation وحدها |
| Server-side human approval | لا يوجد Approval Token مربوط بالخطة في الكود المراجع | نبنيه كشرط إلزامي |
| Child process timeout/cancel | غير ظاهر في MCP runner | نضيفه قبل الإنتاج |
| MCP tools coverage | 9 أدوات فقط | نوسعها لاحقاً حسب نطاقنا: Apps, DNS, Drivers, APK export, Wireless |
| Codex | MCP executable يمكن تسجيله في Codex كـSTDIO server | يحتاج إعداد Codex واختبار فعلي؛ لا plugin جاهز مثبت |
| Windows CI | لا workflow على الفرع الافتراضي | نطلب Windows 10/11 CI + runtime smoke |

#### 20.19.7 النتيجة المحدثة

Droidsmith يظل أقوى مرجع وظيفي وجدناه حتى الآن في Windows/ADB وإدارة التطبيقات وتصدير Split APK وWireless ADB. وأصبح أيضاً مرجعاً عملياً لتصميم MCP محلي عبر STDIO، خصوصاً فصل MCP adapter عن CLI/service boundary.

لكن لا ننسخ MCP adapter كما هو: نحتاج إضافة Approval Token حقيقية مرتبطة بالـplan، timeout/cancellation للـchild process، وتوسيع tool surface لتشمل الوظائف المطلوبة لدينا. كما يجب بناء Windows CI خاص بنا لأن وجود Windows binaries أو اختبارات فرع قديم لا يثبت CI الحالي على الفرع الافتراضي.

**حالة الجولة:** [x] تحديد الفرع الافتراضي وتصحيح تقييم CI السابق. [x] مراجعة MCP source وbinary. [x] حصر الأدوات التسع. [x] مراجعة confirmation gate واختباراته. [x] مراجعة CLI delegation وprocess handling. [ ] اختبار Codex فعلياً. [ ] Windows CI/runtime validation. [ ] تصميم Approval Token. [ ] إضافة timeout/cancellation. [ ] اعتماد أي كود مقتبس.


### 20.20 تدقيق قابلية فصل وإعادة استخدام مكونات Droidsmith (12 نوفمبر 2026)

المصدر: الفرع الافتراضي `master` في `SysAdminDoc/Droidsmith`، نسخة المصدر `0.9.19`. هذه مراجعة ملفات المصدر؛ لم يتم بناء المشروع أو تشغيله أو توصيل هاتف.

#### 20.20.1 بنية Rust: مكتبة واحدة متعددة الـbinaries، وليست workspace مفصولاً

الملفات:
- `src-tauri/src/lib.rs` — SHA `c3c5e23111ab8254afbc9786742b9a8c88244d32`
- `src-tauri/Cargo.toml` — SHA `bc1bc4a1a16b4a2d5db277bd89dcb546e3c2d8af`
- `src-tauri/src/commands.rs` — SHA `962d266a3b69b3f9fa74f363b915302cf516beb9`

**المثبت في المصدر:**
- يوجد Rust library باسم `droidsmith_lib` بنوعي crate `cdylib` و`rlib`.
- GUI وCLI وMCP وpack/schema/bindings/upgrade helper binaries كلها ضمن package واحدة.
- CLI وMCP يربطان نفس المكتبة.
- `adb`, `journal`, `operations`, `profile`, `packs`, `recovery_baseline` وبعض الوحدات الأخرى public، بينما وحدات مثل `backup`, `host_path`, `fs_util`, `process_capture` و`commands` private.
- لا يظهر Cargo workspace أو crates مستقلة لـADB core أو app-management service أو MCP protocol.

**النتيجة:** يوجد فصل منطقي جيد نسبياً داخل Rust library، لكن لا يوجد فصل حزم/اعتماديات يسمح بأخذ ADB core وحده دون سحب package Droidsmith واعتماديات Tauri. حتى `operations.rs` يستورد `tauri::ipc::Channel`، و`lib.rs` يستورد Tauri runtime. لذلك لا نعتبر هذه modules مكتبات مستقلة قابلة للـvendoring مباشرة.

#### 20.20.2 طبقة ADB: أفضل مرشح للاستخراج إلى Core مستقل

الملفات:
- `src-tauri/src/adb/transport.rs` — SHA `c8d4e055d3eca3a529f59307ee6b97968c2cd8a5`
- `src-tauri/src/adb/packages.rs` — SHA `c326da55f257e6ee6ffe96fca64d266320fcbb3d`
- `src-tauri/src/adb/device.rs` — SHA `14e2503a5e527b5fe28099ce93ec355ac1bc2468`
- `src-tauri/src/adb/wireless.rs` — SHA `5fec47cc2c4ea235a83ec899ac5cc925c28e0789`

نقاط تستحق إعادة الاستخدام:
- `AdbTransport` abstraction مع `ShellTransport` و`MockTransport` للاختبارات.
- تمرير arguments منفصلة إلى ADB وعدم تركيب command string من مدخلات المستخدم.
- `DeviceTarget` يربط Serial ببصمة build وconnection generation؛ و`validate_device_target` يعيد اكتشاف الجهاز ويتحقق من عدم تغيّر الهدف قبل العملية.
- جرد التطبيقات يدعم Android user المحدد، enabled/disabled، system/user، APK path، UID، installer، archived/retained.
- فلاتر package manager الإضافية تُعامل كقدرات يتم probe لها من الجهاز، وليس افتراضها من API level فقط.
- Wireless ADB يفصل pairing عن connect، ويميز TLS mDNS provenance عن legacy TCP.
- اختبارات parser وMockTransport وfake adb توفر أساساً جيداً لعقود الاختبار.

**قرار:** لا ننسخ مجلد `adb` بالكامل قبل فصل الاعتماديات. نستخدمه كمرجع تصميم، ونستخرج تدريجياً Core خاصاً بنا بواجهات transport وdevice target وpackage inventory وwireless discovery، مع اختبارات مستقلة. نحتفظ بواجهة adapter مع ADB Platform Tools بدلاً من ربط منطق الأعمال بواجهة Tauri.

#### 20.20.3 إدارة التطبيقات: جرد غني، لكن لا يوجد تاريخ تثبيت/تحديث في نموذج الجرد

`AppPackage` في `adb/packages.rs` يحتوي package/enabled/system/apk_path/uid/installer/archived/retained. لا يحتوي `firstInstallTime` أو `lastUpdateTime`.

`apk_metadata.rs` يقوم بإثراء label/icon وبيانات APK عبر سحب lazy محدود/cache؛ هذا ليس تاريخ تثبيت التطبيق. و`preflight_package_backup` يقرأ بيانات package لأهلية legacy backup، وليس تاريخ التثبيت.

**أثر ذلك على متطلباتنا:** لا نعيد استخدام جرد Droidsmith وحده لتوفير فلتر تاريخ التثبيت أو آخر تحديث. نحتاج enrichment service منفصلة لكل package، بمصدر Android موثوق (مثل PackageManager/dumpsys parsing بعد التحقق من اختلافات الإصدارات/OEM)، وتخزين الحقول كـoptional مع حالة evidence/unsupported. يجب ألا نخترع تاريخاً من وقت APK أو وقت النسخ إلى الجهاز. كما يجب اختبار دقة التواريخ على أجهزة فعلية قبل اعتبارها خاصية مدعومة.

#### 20.20.4 تصدير APK/Split APK: مرشح قوي لإعادة التنفيذ/الاقتباس الانتقائي

الملفات:
- `src-tauri/src/backup.rs` — SHA `02e932240fd7fd64ea79c3c581d52c68ca429ff4`
- `src-tauri/src/fs_util.rs` — SHA `1ffa0c18dc3ea5267979b174524f7035274a5893`
- `src-tauri/src/commands/system.rs` — SHA `c6abc5b632ff5169a20aac879ef654505e9d4794`

السلوك الذي تمت مراجعته:
- preflight يقرأ `pm path --user` ويجمع كل base/split APK paths، ويرفض المسارات غير المطلقة أو غير المنتهية بـAPK، والقائمة الفارغة، وأكثر من 64 جزءاً.
- التصدير يعيد preflight داخل أمر التصدير نفسه، فلا يعتمد على نتيجة preflight قديمة أرسلها renderer.
- كل split يسحب منفصلاً، ثم يتحقق من artifact، ويسجل الاسم والحجم وSHA-256 في manifest.
- ZIP يكتب عبر staging file بجوار الهدف، ثم flush/sync وatomic replace، وDrop يحذف staging عند الفشل.
- صلاحية مسار الحفظ تأتي من native dialog one-shot grant purpose محدد؛ أمر التصدير يستهلك grant قبل التنفيذ، وبعد نجاح الإنتاج يسجل الملف كـproduced artifact.
- target يعاد التحقق منه عبر privileged transport، ويُستخدم operation runner مع timeout وcancel/progress لكل مرحلة.

**حدود يجب الاحتفاظ بها في تصميمنا:**
- APK export ليس backup لبيانات التطبيق الخاصة.
- Android package paths والـsplit naming وقيود OEM تحتاج اختبارات أجهزة.
- فحص header للـAPK/ZIP وحده ليس تحققاً كاملاً من سلامة محتوى ZIP أو توقيع APK؛ manifest hashes تثبت سلامة الملفات بعد التصدير عند إعادة حسابها، ولا تثبت هوية الناشر.
- لا نعرض العملية كـنسخ بيانات أو استعادة تطبيق كاملة.

**قرار:** هذا أقوى مرشح وظيفي في Droidsmith لميزة Export Installed APK/Split APK. نعيد استخدام التصميم والخطوات مع اختباراتنا، ولا ننقل `backup.rs` كما هو قبل فصل `operations` و`fs_util` عن Tauri.

#### 20.20.5 Path Grants والملفات: نمط أمني جيد لكنه مرتبط حالياً بواجهة Tauri

`host_path.rs` — SHA `27965dce0ff3a8cb84cbc63f964ee84b22d60522`.

الخصائص المؤكدة:
- grant له purpose محدد، TTL خمس دقائق، وحد أقصى 64 grant نشط.
- grant يستهلك مرة واحدة؛ لا يقبل purpose مختلفاً.
- read path يُcanonicalize ويتحقق أنه ملف عادي.
- write path يتحقق من absolute normalized path، ويرفض symlink/directory، ويcanonicalize parent.
- reveal/open لا يعتمد على مسار renderer وحده؛ يجب أن يكون artifact أنتجه التطبيق، ثم يعاد التحقق من canonical identity لمنع symlink retargeting.
- أسماء الملفات المقترحة تتحقق من reserved Windows names ومكونات المسار.

**حدود المراجعة:** لم نثبت مقاومة كل حالات TOCTOU بين canonicalization والفتح/الكتابة، ولا تعامل جميع أنظمة الملفات/صلاحيات Windows؛ لا نعلن أن المسار محصّن بالكامل من كل race. كما أن grants الحالية تعتمد على native dialog وTauri state؛ MCP headless لا يمكنه افتراض وجود dialog grant.

**قرار:** نقتبس مفهوم purpose-scoped one-shot grants، لكن في برنامجنا نحتاج فصل File Authorization Service عن Tauri dialog. بالنسبة لـMCP، نستخدم مسارات app-owned أو file handles/IDs مع consent واضح، ولا نقبل arbitrary host paths من agent مباشرة.

#### 20.20.6 Operations: قوية داخل GUI لكن ليست مستقلة عن Tauri

`operations.rs` — SHA `c68f8de860315dbe57614c5b719948b8bee9dc65`.

يوفر operation IDs، cancellation registry، progress events، output limits، timeouts، وقتل process tree. الاختبار `tests/fake_tool_contract.rs` — SHA `013cfd4810967fcba10318f0367d0d204b4afcc8` يختبر argv preservation، stdout/stderr limits، cancellation وقتل descendant tree، target drift، disk failure.

لكن الملف يستورد `tauri::ipc::Channel`، لذلك ليس مستقلاً عن Tauri حالياً. والأهم أن MCP الحالي لا يستخدم هذه الطبقة؛ هو يشغل CLI child process باستخدام `.output()` المتزامن، ولذلك لا يرث تلقائياً cancellation/progress/timeout.

**قرار:** نستخرج Process Runner/Operation Registry كـRust core مستقل لا يعرف Tauri. GUI يربط events إلى Tauri Channel، وMCP يربطها إلى progress notifications/cancellation، وCLI يستخدم نفس core. هذه خطوة أعلى أولوية من نسخ MCP adapter.

#### 20.20.7 CLI/MCP: فصل مفيد لكن يحتاج عقد تشغيل أكثر اكتمالاً

- CLI binary يستخدم `droidsmith_lib`، وMCP binary ينادي `mcp::run_stdio()`.
- MCP يستدعي sibling CLI، ويستخدم argument arrays، وهذا يحد من shell injection على مستوى تشغيل child process.
- CLI يدعم `devices`, `packages`, `pack plan/apply`, `profile run`, `baseline export/inspect/apply`، مع JSON وdry-run وبعض fleet workflows.
- MCP يعرّض تسع أدوات فقط كما وثق القسم 20.19، ولا يقدم كل أوامر CLI أو وظائف GUI.
- MCP لا يستخدم Tauri native path grants؛ لذلك لا ينبغي فتح أدوات تصدير ملفات أو استيرادها عبر مسارات حرة من MCP.

**قرار:** نستخدم CLI كمرجع لعقود JSON والـexit codes والـdry-run/revalidation، لكن معمارية برنامجنا الأفضل هي MCP وGUI وCLI كـadapters فوق Service Layer واحدة، وليس MCP → CLI → Service Layer كطبقة process إضافية. يمكن الإبقاء على CLI كمنتج مستقل/اختبار تكامل، لكن لا نجعل استدعاء process بين adapterين هو الطريق الداخلي الوحيد.

#### 20.20.8 مصفوفة قرار إعادة الاستخدام

| المكوّن | النتيجة | القرار المقترح |
|---|---|---|
| ADB transport + MockTransport | تصميم واختبارات جيدة، داخل package Tauri-coupled | استخراج Core مستقل ثم adapter |
| DeviceTarget + target revalidation | نمط أمان أساسي | إعادة تصميم/تنفيذ مطابق مع tests |
| Package inventory | غني، لا يحتوي install/update timestamps | اقتباس المفاهيم، إضافة enrichment مستقل |
| Wireless ADB | mDNS/pair/connect/provenance | adapter فوق Platform Tools مع تحقق Windows/device |
| APK/Split export | مسار متكامل ومراجع بالمصدر | مرشح قوي للاستخراج بعد فك Tauri coupling |
| Path grants | one-shot scoped authorization | إعادة تصميم ليتناسب GUI وMCP |
| Operations runner | timeout/cancel/output caps/process tree | استخراج Core مستقل أولوية عالية |
| MCP STDIO | minimal local adapter | إعادة كتابة/اقتباس انتقائي، لا نسخ كامل |
| CLI JSON workflows | dry-run/validation/journal/fleet | مرجع للعقود، وليس process dependency إلزامي |
| App install/update timestamps | غير موجودة في AppPackage | تطوير خاص بنا مع evidence + device tests |

#### 20.20.9 القرار المعماري النهائي المبدئي

لا نوصي بعمل fork كامل لـDroidsmith كأساس لبرنامجنا، ولا بنسخ مستودعه بالكامل. نوصي بـ**Selective extraction / clean-room adaptation guided by source audit**:

1. تصميم حزم داخلية مستقلة: `device-core`, `process-core`, `package-service`, `artifact-service`, `permission-service`, `mcp-adapter`, `desktop-adapter`.
2. فصل أي Tauri type أو Channel أو AppHandle من الخدمات الأساسية.
3. اعتماد `DeviceTarget` immutable مع generation/build identity revalidation قبل كل mutation.
4. كل تغيير يمر عبر Plan → Approval Token → Apply → Verify → Journal؛ لا Boolean confirmation منفرد.
5. مشاركة Process Runner بين GUI/CLI/MCP، مع timeout/cancellation/output limits.
6. استخدام Native file picker في GUI، وfile IDs/grants مقيدة في MCP؛ لا arbitrary paths.
7. إضافة install/update timestamps كـenrichment مستقل، مع Unknown/Unavailable بدلاً من التخمين.
8. بناء Windows CI واختبارات fake ADB/contract tests، ثم اختبار فعلي على أجهزة Android محددة قبل إعلان الدعم.

**الحالة:** [x] مراجعة Rust crate boundaries. [x] مراجعة ADB transport وpackage inventory. [x] مراجعة split APK export وpath grants. [x] مراجعة operation cancellation tests. [x] مقارنة MCP/CLI boundaries. [ ] إثبات build محلي أو CI على Windows الحالي. [ ] اختبارات على أجهزة حقيقية. [ ] استخراج core تجريبي في مستودعنا (لم يبدأ). [ ] اعتماد مكونات نهائياً.


### 20.21 مراجعة مشاريع إضافية لتواريخ التطبيقات وجرد الحزم (12 نوفمبر 2026)

هذه الجولة ركزت على كود يقرأ حقول Package Manager، لا على أسماء الأدوات أو صور واجهاتها. لم يتم بناء أي مشروع أو تشغيله أو توصيل هاتف.

#### 20.21.1 REA — مرجع parsing لتفاصيل حزمة واحدة، وليس مدير هاتف

المستودع: [morluto/rea](https://github.com/morluto/rea)

الملفات التي تمت مراجعتها:
- `src/adb/AdbShellOutput.ts` — blob SHA `bca37332b21b0cb8f4ef42a929ec8a260787a72a`
- `src/adb/AdbProvider.ts` — blob SHA `50677b3e1ab417544c1cf9ae66bd2c4473241eb4`

**المثبت في المصدر:**
- يوجد parser مخصص لـ`dumpsys package PACKAGE` باسم `parseDumpsysPackageOutput`.
- يستخرج `versionName`, `versionCode`, `firstInstallTime`, `lastUpdateTime`, `installerPackageName`, `userId`, `pkgFlags` وعدد requested permissions.
- التنفيذ يمرر arguments منفصلة إلى shell runner: `["dumpsys", "package", package]`، ويحدد timeout قدره 60 ثانية وحداً أقصى للمخرجات 8 MiB.
- parser يحتفظ بقيمة التاريخ كاملة بعد trim، بما فيها التاريخ والوقت، ويتجاهل الحقول غير المعروفة بدلاً من تخمينها.
- يوجد اختبار parser يتضمن firstInstallTime وlastUpdateTime وinstallerPackageName.
- العملية هي `inspect_adb_package` لحزمة واحدة يحددها الطلب، وليست bulk enrichment لكل نتائج الجرد.

**الحدود:**
- لا يقدم هذا الجزء وحده جرداً شاملاً مع فرز/فلترة كل التطبيقات حسب تاريخ التثبيت.
- صيغة `dumpsys` تختلف بين إصدارات Android وواجهات OEM؛ وجود parser واختبار fixture لا يثبت توافقه مع كل الأجهزة.
- لم نثبت هنا أن جميع الحقول متاحة لكل Android user أو لكل package state.

**القرار:** REA مرجع جيد لكتابة parser محدود ذي حقول معروفة واختبارات fixtures، لكنه ليس أساساً لمدير تطبيقات Windows. نقتبس الفكرة ونبني طبقة enrichment مستقلة: inventory أولاً، ثم تفاصيل timestamps بشكل محدود/متوازٍ مع cache وtimeouts، مع حالة لكل حقل (Available / Unsupported / ParseFailed / NotInstalledForUser) وعدم إخفاء النقص.

#### 20.21.2 WSA Suite — نموذج جرد وتفاصيل، لكن نطاقه Windows Subsystem for Android

المستودع: [mjcr-dev/wsa-suite](https://github.com/mjcr-dev/wsa-suite)

الملف:
- `src-tauri/src/adb/packages.rs` — blob SHA `40d88ba4584b22b2afffcf1e70ef7a69f2744c7c`

**المثبت في المصدر:**
- جرد التطبيقات يستخدم `pm list packages -f` ويجمع disabled packages وlaunchable apps.
- يؤجل label/version details إلى اختيار التطبيق بدلاً من تشغيل `dumpsys` لكل package في الجرد.
- `PackageDetail` يتضمن `first_install` و`last_update`، وتُقرأ التفاصيل من `dumpsys package PACKAGE`.
- توجد اختبارات parsing لحقول dumpsys، لكن الاختبار الظاهر لتاريخ التثبيت يثبت استخراج الحقل إلى حد محدود ولا يغطي النص الكامل للتاريخ والوقت.

**ملاحظة تدقيق مهمة:** helper المسمى `field()` يقف عند أول whitespace. وتاريخ Android المعتاد يأتي مثل `2026-01-02 03:04:05`؛ لذلك helper الحالي يعيد جزء التاريخ فقط ولا يحفظ الوقت. لا ننسخ هذا parser كما هو لتخزين timestamp كامل.

**حدود الملاءمة:**
- WSA Suite مخصص لإدارة Android subsystem على Windows 11، وليس مديراً عاماً لهواتف Android عبر USB/Wireless.
- أسلوب lazy details مفيد لتقليل عدد أوامر ADB وزمن الجرد.
- تصنيف system/user اعتماداً على مسار APK وحده قد يخطئ في حالات OEM أو تحديثات التطبيقات؛ لا نجعله حقيقة وحيدة دون مصدر تصنيف أقوى.

**القرار:** نقتبس نمط lazy enrichment، لا المشروع كقاعدة تشغيل لهواتف حقيقية. ويجب أن يختبر parser التاريخ الكامل، timezone assumptions، اختلافات dumpsys، والحزم متعددة المستخدمين.

#### 20.21.3 خلاصة المقارنة لتواريخ التطبيقات

| المصدر | جرد شامل | تاريخ التثبيت/التحديث | ملاحظات |
|---|---|---|---|
| Droidsmith 0.9.19 | نعم | غير موجود في AppPackage | يحتاج enrichment مستقل |
| REA | ليس في مسار الفحص الذي راجعناه | موجود لتفاصيل package مفردة | parser يحافظ على النص الكامل؛ لا يثبت bulk support |
| WSA Suite | نعم | موجود في PackageDetail | helper الحالي يقطع الوقت عند whitespace؛ نطاق WSA فقط |
| Android App Manager | مدير تطبيقات يعمل على Android نفسه | لم نثبت من هذا البحث تكاملاً مع ADB host أو bulk export للتواريخ | مرجع UX/وظائف على الجهاز، وليس Windows host core |

**قرار التصميم المبدئي:** نموذج التطبيق يجب أن يفصل بين:
- حقول inventory السريعة: package, user/profile, enabled, system classification + evidence, installer, path, UID.
- حقول التفاصيل المؤجلة: label/icon, version, firstInstallTime, lastUpdateTime, permissions, APK paths.
- provenance لكل حقل: command/API، Android user، وقت القراءة، parser version، وحالة التوفر.
- الفلترة بالتاريخ تتم محلياً على قيم parsed موحدة، ولا نطلب `dumpsys` لكل التطبيقات دفعة واحدة دون قياس.

---

### 20.22 تصميم DNS مخصص للأجهزة عبر ADB (12 نوفمبر 2026)

#### 20.22.1 التمييز بين Private DNS وDNS IP وLocal VPN

هذه ثلاثة مسارات مختلفة ولا يجوز تقديمها للمستخدم كأنها إعداد واحد:

1. **Android Private DNS / DNS-over-TLS:** إعداد عالمي على مستوى الجهاز، يستخدم hostname لمزود DNS يدعم DoT، وليس عنوان IP عادي. Android يدعم Private DNS ابتداءً من Android 9. توثيق Google يوضح إعداد hostname، ومصدر AOSP يعرّف القيم `off`, `opportunistic`, `hostname` ومفتاح `private_dns_specifier`. [Google Public DNS Android setup](https://developers.google.com/speed/public-dns/docs/using) · [AOSP ConnectivitySettingsUtils](https://android.googlesource.com/platform/frameworks/libs/net/+/8ef52686483cdff0040d22753bb516e99a96a88d/common/framework/com/android/net/module/util/ConnectivitySettingsUtils.java)
2. **DNS IP لكل Wi-Fi network:** ليس هو نفسه Private DNS hostname؛ ولا يوجد في هذا التدقيق إثبات لأمر ADB موحد يضبط DNS لكل شبكة Wi-Fi على كل إصدارات Android/OEM. يجب أن يبقى خارج وعود الدعم العامة حتى يتم اختبار طريقة خاصة بكل نطاق/جهاز.
3. **Ad-blocking عبر Local VPN:** تطبيق blocker ينشئ VPN محلياً ويعترض DNS/traffic وفق قدراته؛ يحتاج تثبيت التطبيق وموافقة المستخدم على VPN، وقد يتعارض مع VPN آخر. هذا مسار مختلف عن Private DNS ولا يمكن تشغيله سراً من ADB.

#### 20.22.2 ما يمكن بناؤه في الإصدار الأول

- قراءة الحالة الحالية عبر ADB، بما في ذلك `private_dns_mode` و`private_dns_specifier`، مع حفظ القيم الخام وعدم افتراض أن غياب المفتاح يعني دائماً وضعاً معيناً على كل build.
- عرض خيارات Private DNS المدعومة: Off، Automatic/Opportunistic، Provider hostname.
- إدخال hostname فقط عند اختيار Provider hostname؛ التحقق من صيغة hostname وعدم قبول URL أو IP في هذا الحقل.
- Preview قبل التغيير يوضح الجهاز المحدد، القيم الحالية، القيم المقترحة، واحتمال فقدان حل أسماء النطاقات إذا كان المزود غير متاح.
- حفظ snapshot للقيم السابقة قبل أي كتابة، ثم Apply بعد موافقة صريحة، ثم قراءة الإعداد مجدداً وVerify.
- Restore يعيد القيم السابقة بدقة، ولا يفترض أن الوضع السابق كان Off.
- بعد التغيير، إجراء تحقق اتصال/حل أسماء مستقل قدر الإمكان؛ نجاح أمر الكتابة وحده لا يثبت أن DNS يعمل.
- عند وجود VPN أو ملف إدارة جهاز أو قيود OEM، إظهار أن النتيجة قد تختلف وعدم الادعاء بأن الإعداد يسيطر على كل مسارات الشبكة.

**ملاحظة صلاحيات:** توثيق Android `DevicePolicyManager` يبين أن واجهات إدارة Private DNS هذه مقيدة بامتياز Device Owner/حالات إدارة مؤسسية، وليست API عامة لأي تطبيق عادي. لذلك لا نبني GUI على افتراض توفر DPM control. ADB shell settings طريق منفصل يحتاج تحققاً فعلياً على إصدارات وأجهزة محددة. [Android DevicePolicyManager Private DNS](https://developer.android.com/reference/android/app/admin/DevicePolicyManager)

#### 20.22.3 DNS Ad-blocking: اختيار المزود والتحقق

- يمكن للمستخدم إدخال hostname لمزود DNS filtering يدعم DoT، أو اختيار preset بعد توثيق مزود الخدمة وhostname من مصدره الرسمي.
- لا نضع عنوان IP في خانة Private DNS hostname.
- لا نعد بأن Private DNS يمنع كل الإعلانات داخل التطبيقات؛ الحجب يعتمد على قوائم المزود ونوع الطلبات، وبعض الإعلانات/المحتوى تأتي من نفس نطاقات الخدمة.
- لا ننشئ قائمة DNS عشوائية ولا نضيف مزوداً دون مصدر رسمي واختبار.
- لا نغير إعداد DNS تلقائياً عند توصيل الجهاز؛ كل تغيير يمر Preview → Approval → Apply → Verify، مع Restore.

**قرار:** Private DNS hostname هو المسار الأول المرشح لأنه إعداد نظامي ولا يتطلب تثبيت Local VPN app، لكنه يظل feature مشروطة بالتوافق والتحقق. DNS لكل Wi-Fi وLocal VPN يظلان مسارين منفصلين لاحقاً، لا جزءاً من ادعاء V1.

---

### 20.23 Windows Driver Diagnostics: الاعتماد على SetupAPI وConfiguration Manager (12 نوفمبر 2026)

#### 20.23.1 واجهات Windows الرسمية المقترحة

بدلاً من تحليل Registry عشوائياً أو الاعتماد على أسماء الأجهزة الظاهرة فقط، توثيق Microsoft يوصي بتعداد أجهزة Plug and Play عبر SetupAPI أو Configuration Manager:

- `SetupDiGetClassDevs` لإنشاء Device Information Set، ثم `SetupDiEnumDeviceInfo` لتعداد الأجهزة.
- `SetupDiGetDeviceInstanceId` للحصول على Device Instance ID.
- `SetupDiGetDeviceRegistryProperty(SPDRP_HARDWAREID / SPDRP_COMPATIBLEIDS)` لقراءة Hardware IDs وCompatible IDs.
- `SetupDiGetDeviceProperty` أو `CM_Get_DevNode_Property` لقراءة Unified Device Properties.
- `CM_Get_DevNode_Status` أو `DEVPKEY_Device_ProblemCode` لمعرفة حالة الجهاز ورقم المشكلة.

Microsoft تحذر من تعداد الأجهزة بقراءة Registry مباشرة؛ لأن PnP manager يحتفظ بمعلومات الحضور/الأجهزة الوهمية والفلترة. [Enumerating Installed Devices](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/enumerating-installed-devices) · [Device Information Sets](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/device-information-sets)

#### 20.23.2 حقول التشخيص التي يمكن جمعها

| الحقل | مصدر Windows المقترح | لماذا نحتاجه |
|---|---|---|
| Device Instance ID | SetupDiGetDeviceInstanceId | تمييز نسخة الجهاز على هذا الكمبيوتر |
| Hardware IDs / Compatible IDs | SPDRP_HARDWAREID / SPDRP_COMPATIBLEIDS | مطابقة driver package مع الجهاز |
| Present / Phantom | DIGCF_PRESENT أو CM_GETIDLIST_FILTER_PRESENT | التفريق بين جهاز موصول وآثار جهاز قديم |
| Problem Code | DEVPKEY_Device_ProblemCode أو CM_Get_DevNode_Status | تشخيص حالة Device Manager |
| Problem Status | DEVPKEY_Device_ProblemStatus | NTSTATUS إضافي عند توفره |
| Driver INF | DEVPKEY_Device_DriverInfPath | معرفة INF المنشور الذي ربط الجهاز |
| Driver Provider | DEVPKEY_Device_DriverProvider | الشركة المعلنة في INF |
| Driver Version / Date | DEVPKEY_Device_DriverVersion / DriverDate | مقارنة النسخة والتاريخ |
| Driver Description | DEVPKEY_Device_DriverDesc | وصف تعريف الجهاز |
| Device Install Date | DEVPKEY_Device_InstallDate | آخر وقت تثبيت/تحديث instance، وليس تاريخ إصدار التعريف |
| First Install Date | DEVPKEY_Device_FirstInstallDate | أول تثبيت للـdevice instance، لا يتغير مع كل تحديث |

المراجع الرسمية: [SetupDiGetDeviceProperty](https://learn.microsoft.com/en-us/windows/win32/api/setupapi/nf-setupapi-setupdigetdevicepropertyw) · [Driver INF path](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-driverinfpath) · [Driver version](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-driverversion) · [Driver provider](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-driverprovider) · [Driver date](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-driverdate) · [Problem code](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-problemcode) · [Problem status](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-problemstatus) · [Install date](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-installdate) · [First install date](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/devpkey-device-firstinstalldate)

**فرق مهم في التواريخ:** DriverDate هي تاريخ التعريف وفق INF DriverVer؛ InstallDate هو آخر تثبيت للـdevice instance وقد يتغير مع تحديث التعريف؛ FirstInstallDate هو أول تثبيت للـinstance. لا نعرض أحدها تحت اسم الآخر.

#### 20.23.3 حدود الإصلاح الآمن

- مرحلة Scan تبقى Read-only وتجمع الأدلة فقط.
- التشخيص يصنف: لا يوجد driver binding، Problem Code، driver موجود لكن الجهاز غير حاضر، Hardware ID غير معروف، أو حالة لا يمكن تفسيرها.
- البحث عن تعريف يطابق Hardware ID يجب أن يقتصر على Windows Update أو صفحة الشركة المصنّعة/المصدر الرسمي؛ لا نوصي بمواقع تجميع التعريفات.
- تثبيت/تحديث/rollback/reinstall عملية منفصلة تحتاج Preview وموافقة صريحة، وتوضح INF والـProvider والنسخة والـHardware IDs المستهدفة.
- لا نحذف Driver Store package على نطاق واسع ولا ننفذ `pnputil /delete-driver` تلقائياً.
- `SetupDiInstallDevice` عملية تثبيت فعلية وتتطلب Administrators؛ توثيق Microsoft يوضح أن تثبيت driver node قد يسجل الخدمات والملفات وواجهات الجهاز، ولذلك لا تعامل كإجراء تشخيص بسيط. [SetupDiInstallDevice](https://learn.microsoft.com/en-us/windows/win32/api/setupapi/nf-setupapi-setupdiinstalldevice)
- لا نعتبر أن ظهور ADB Interface أو MTP وحده يعني أن Android authorization أو ADB protocol يعمل؛ هذه طبقات مستقلة كما أوضح القسم 18.

#### 20.23.4 القرار المعماري

نضيف طبقة Windows مستقلة:
- `windows-device-enumerator`: SetupAPI/Configuration Manager enumeration.
- `windows-driver-inspector`: properties وproblem codes وINF metadata.
- `driver-source-resolver`: ربط Hardware IDs بمصادر رسمية فقط.
- `driver-repair-planner`: خطة تغييرات قابلة للمراجعة، دون تنفيذ تلقائي.
- `driver-repair-executor`: privileged boundary منفصل، يعمل فقط بعد approval token، ويسجل before/after وreboot requirement.

يمكن تنفيذ wrapper Rust حول Windows APIs أو استخدام crate موثوق بعد تدقيق واجهاته؛ لا نعتمد الآن على crate بعينه قبل مراجعة صيانته وتغطيته واختباراته. لا نستخدم Registry scraping كبديل أساسي للواجهات الرسمية.

**الحالة:** تصميم مبني على Microsoft API documentation ومراجعة مصدرية للأدوات السابقة؛ لم يتم كتابة Windows API code أو تثبيت/إزالة أي تعريف أو إجراء اختبار على Windows في هذه الجولة.


### 20.24 تدقيق أدوات Windows الجاهزة لإدارة التعريفات (12 نوفمبر 2026)

هذا القسم يفرق بين أداة تصلح كمرجع تشخيص، وأداة إدارة Driver Store، وأداة تستبدل USB driver binding. لا يعني إدراج أداة أننا سنضمّنها أو نشغّلها تلقائياً.

#### 20.24.1 PnPUtil — الخيار الأول للتشخيص والتشغيل المحدود

توثيق Microsoft يصف PnPUtil كأداة موجودة ضمن Windows، ويوصي بها لإدارة حزم التعريفات بدلاً من DevCon. أوامر مهمة للنطاق:

- `pnputil /enum-devices /connected /deviceids /properties /drivers`: جرد الأجهزة المتصلة مع Hardware/Compatible IDs والخصائص والتعريفات المطابقة، وفق إصدار Windows.
- `pnputil /enum-devices /problem`: العثور على الأجهزة التي لديها Problem Code.
- `pnputil /enum-drivers`: عرض حزم الطرف الثالث في Driver Store.
- `pnputil /enum-drivers /files`: إظهار ملفات الحزم، على إصدارات Windows الداعمة.
- `pnputil /add-driver <file.inf> /install`: إضافة حزمة وتثبيتها على الأجهزة المطابقة؛ عملية تغيير وليست فحصاً.
- `pnputil /export-driver <oem#.inf> <directory>`: تصدير حزمة للتراجع/النسخ الاحتياطي.

**تنبيه توافق:** خيارات PnPUtil أضيفت تدريجياً؛ فمثلاً `/enum-devices` متاح من Windows 10 1903، وخيارات `/deviceids` و`/properties` تتطلب إصدارات أحدث من Windows 11، و`/enum-devicetree` من Windows 11 23H2. يجب اكتشاف إصدار Windows/القدرات أو تجربة help، وعدم افتراض توفر كل switch.

**تنبيه parsing:** لا نعتمد على النص الافتراضي المترجم/المحلي. توثيق Microsoft يوصي بمخرجات XML أو CSV عبر `/format` و`/output-file` حيث تكون متاحة، لأن تنسيق النص يتغير حسب لغة Windows. نستخدم XML/CSV المدعوم ونحتفظ بمسار fallback موثق للإصدارات الأقدم.

المراجع: [PnPUtil command syntax](https://learn.microsoft.com/en-us/windows-hardware/drivers/devtest/pnputil-command-syntax) · [Troubleshooting device and driver installations](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/troubleshooting-device-and-driver-installations) · [Create a driver inventory](https://learn.microsoft.com/en-us/windows-hardware/drivers/driversecurity/create-a-driver-inventory) · [DevCon Update replacement](https://learn.microsoft.com/en-us/windows-hardware/drivers/devtest/devcon-update)

#### 20.24.2 Driver Store Explorer (RAPR) — مرجع إدارة متقدم، وليس محرك إصلاح تلقائي

المستودع: [lostindark/DriverStoreExplorer](https://github.com/lostindark/DriverStoreExplorer)

مراجعة README على الفرع الافتراضي `master`:
- يعلن دعم عرض حزم الطرف الثالث وبيانات الحجم/النسخة/التاريخ.
- يعلن Add/Install/Delete/Export، ووجود backends متعددة: Native Windows API وDISM وPnPUtil.
- يعلن ربط الحزم بالأجهزة، والتمييز بين الأجهزة المتصلة وغير المتصلة، وعمليات batch.
- README يحذر صراحة من أن حذف تعريف غير مناسب قد يسبب تعطل وظائف الجهاز أو يمنع Windows من الإقلاع.
- يوضح أن تحديد الحزم غير المستخدمة Best effort، وأن وجود صفر أجهزة مرتبطة لا يثبت أن حذف الحزمة آمن؛ قد تكون الحزمة لازمة لخدمة أو برنامج أو جهاز غير حاضر.

**القرار:** RAPR مرجع ممتاز لتجربة المستخدم، جرد Driver Store، التصدير، وسير عمل مراجعة الحزم. لا ندمج واجهة RAPR أو ميزة تنظيف Driver Store في برنامجنا، ولا نجعل حذف التعريفات جزءاً من "إصلاح تعريف هاتف". نستخدم PnPUtil/SetupAPI لقراءة البيانات اللازمة، ونحصر التغيير في تعريف محدد ثبت ارتباطه بجهاز Android مستهدف.

لم يتم تشغيل RAPR أو مراجعة جميع ملفات تنفيذه؛ ما سبق مراجعة README/وصف المشروع وليس إثباتاً لاختبار runtime.

#### 20.24.3 libwdi / Zadig — غير مناسب كمسار Android ADB/MTP عام
المستودع: [pbatard/libwdi](https://github.com/pbatard/libwdi)

README يصف مكتبة لإنشاء INF/Catalog وتثبيت WinUSB أو libusb0 أو libusbK أو USB Serial أو تعريف مخصص، مع UAC elevation. دليل Zadig يوضح أن هدفها تثبيت تعريف متوافق مع libusb لجهاز USB، وأن استبدال التعريف الموجود قد يمنع النظام أو التطبيقات الأخرى من الوصول للجهاز؛ ويذكر أمثلة مثل HID mouse وUSB hubs وComposite Parent.
**القرار:** لا نستخدم Zadig/libwdi لإصلاح Android ADB أو MTP. استبدال تعريف واجهة Android بتعريف WinUSB/libusb قد يكسر MTP أو ADB أو واجهات composite الأخرى، كما أن أداة عامة لا تعرف أي interface يجب الحفاظ عليه. نحتفظ بها كمرجع منفصل فقط إذا ظهر مستقبلاً احتياج USB raw محدد ومثبت، وبعد تحديد VID/PID/MI والـinterface المستهدف واختبار عدم تعطيل بقية الواجهات.

مصدر: [libwdi README](https://github.com/pbatard/libwdi) · [Zadig user guide](https://github.com/pbatard/libwdi/wiki/Zadig)

#### 20.24.4 مسارات تعريفات Android الرسمية

- Google USB Driver مخصص لأجهزة Google؛ Google توضح أن تعريفات بقية الشركات تأتي من الشركة المصنعة نفسها.
- Android Developers تحتفظ بجدول روابط OEM USB drivers، ومن ضمنه Samsung.
- لا نعدل `android_winusb.inf` أو نضيف Hardware IDs يدوياً كمسار افتراضي؛ Google تحذر من أن تعديل INF قد ينتج تحذيرات أمان عند التثبيت/التحديث.
- لا ننزل تعريفاً من مواقع تجميع مجهولة ولا نعتبر توقيع الملف وحده دليلاً على أنه التعريف الصحيح لهذا الجهاز.

المراجع: [Google USB Driver](https://developer.android.com/studio/run/win-usb) · [OEM USB drivers](https://developer.android.com/studio/run/oem-usb)

#### 20.24.5 قرار الإصدار الأول

1. Scan: قراءة الأجهزة وحالاتها وHardware IDs وProblem Codes.
2. Match: تحديد واجهة Android المحتملة، مع الاحتفاظ بأكثر من واجهة composite وعدم اختزال الجهاز إلى اسم تسويقي.
3. Source: إظهار رابط الشركة الرسمية أو Google فقط.
4. Preview: إظهار INF/Provider/Version/Hardware IDs والتغييرات المتوقعة.
5. Backup: تصدير التعريف الحالي عند الإمكان قبل التغيير.
6. Apply: تثبيت INF محدد فقط بعد موافقة صريحة ورفع صلاحية مؤقت.
7. Verify: إعادة تعداد الجهاز ومقارنة binding والحالة وADB/MTP بشكل منفصل.
8. Rollback: إظهار مسار استعادة موثق؛ لا حذف تلقائي لحزم Driver Store.

---

### 20.25 اختيار مكتبات Rust لعزل Windows Driver Core (12 نوفمبر 2026)

#### 20.25.1 الخيارات التي تستحق المقارنة

**A. `windows` crate (Microsoft windows-rs)**
- إسقاط Rust واسع ومكتوب الأنواع لواجهات Win32 وCOM وWinRT.
- مناسب عندما نحتاج API surface غني مع تحويلات Rust وResult/typed handles.
- يجب تحديد Cargo features بدقة بدلاً من تفعيل كل الواجهات.
- قد يضيف dependency weight وتغيرات إصدار؛ نضعه داخل Windows-specific crate ولا نكشف أنواعه في الـcore العام.

**B. `windows-sys` crate**
- bindings منخفضة المستوى وأقرب إلى FFI الخام، وتناسب wrappers رفيعة حول SetupAPI/CfgMgr32.
- واجهات مثل `SetupDiGetClassDevsW`, `SetupDiEnumDeviceInfo`, `SetupDiEnumDriverInfoW`, `CM_Get_DevNode_Status` متاحة ضمن namespace `Win32::Devices::DeviceAndDriverInstallation` في docs.rs الحالية.
- الدوال قد تكون unsafe وتتطلب الالتزام الدقيق بأحجام structs، المؤشرات، UTF-16، وGetLastError/CONFIGRET conventions.
- مناسب إذا كان الفريق مستعداً لبناء safe wrapper صغير ومدقق حول كل API.

**C. `windows-bindgen`**
- خيار لتوليد مجموعة bindings خاصة ومحدودة عندما نحتاج APIs معينة داخل مكتبة قابلة لإعادة الاستخدام ولا نريد الاعتماد على umbrella projection كاملاً.
- لا نبدأ به قبل أن يثبت أن `windows` أو `windows-sys` لا يقدمان الحدود المطلوبة.

المراجع: [Microsoft windows-rs crate selection](https://github.com/microsoft/windows-rs/blob/master/docs/crates/windows.md) · [Rust for Windows](https://learn.microsoft.com/en-us/windows/dev-environment/rust/rust-for-windows) · [windows-sys SetupAPI bindings](https://docs.rs/windows-sys/latest/windows_sys/Win32/Devices/DeviceAndDriverInstallation/index.html)

#### 20.25.2 التوصية المبدئية

ابدأ بـ`windows-sys` داخل crate مستقل باسم مثل `windows-device-core` إذا كانت الحاجة محصورة في SetupAPI/CfgMgr32 وPnP properties. السبب: نحتاج حدوداً صغيرة ومباشرة مع Windows، وليس إسقاط كل Windows API في core.

لكن القرار ليس نهائياً قبل proof-of-concept يقارن:
- سهولة إدارة handles وUTF-16 والأخطاء.
- حجم dependency graph ووقت البناء.
- سهولة اختبار safe wrapper.
- التوافق مع MSRV وTauri/Rust toolchain الحالي.
- توافر APIs المطلوبة على Windows 10/11 المستهدف.

إذا أصبح التعامل مع COM/WinRT أو واجهات أعلى مستوى ضرورياً، نعيد تقييم `windows` crate. لا نخلط أنواع `windows` و`windows-sys` عبر حدود الخدمات؛ نعيد فقط DTOs مستقلة مثل `DeviceInstance`, `HardwareIdSet`, `DriverBinding`, `ProblemCode`.

#### 20.25.3 حدود الحزمة المقترحة

```text
driver-domain
  ├── DeviceInstance / HardwareIds / DriverBinding DTOs
  ├── ProblemCode mapping
  └── DriverRepairPlan / VerificationResult

windows-device-core
  ├── SetupAPI enumeration
  ├── Configuration Manager status/properties
  ├── PnPUtil capability/version adapter
  └── Windows-specific error translation

driver-source-catalog
  ├── OEM official source records
  └── Google USB Driver source record

driver-repair-service
  ├── match → preview → approval → apply → verify
  └── rollback evidence and operation journal

tauri-adapter / mcp-adapter / cli-adapter
  └── all call the same service layer
```

لا يخرج أي `HANDLE`, `HDEVINFO`, raw pointer، Windows struct، أو `windows-sys` type من `windows-device-core`. الـcore العام لا يعتمد على Windows APIs، ويمكن اختبار mapping وrepair planning فيه على كل الأنظمة.

#### 20.25.4 الاختبارات المطلوبة قبل اعتماد المكتبة

- Unit tests لقراءة MULTI_SZ Hardware IDs وUTF-16 وأخطاء buffer resizing.
- Fake PnP provider لاختبار connected/disconnected/problem/no-driver/multiple-interface.
- Contract tests لنسخ مخرجات PnPUtil XML/CSV عبر إصدارات Windows المدعومة ولغات مختلفة.
- Windows CI على Windows 10/11 حيث تتاح runners المناسبة، مع فصل نجاح compile عن hardware validation.
- اختبار جهاز Android فعلي لكل عائلة OEM مستهدفة قبل إعلان توافق الإصلاح.
- اختبار rollback وإثبات عدم تعطيل MTP أو ADB أو واجهات composite المجاورة.

**الحالة:** مراجعة توثيق Microsoft وRust bindings ومشاريع RAPR/libwdi؛ لم يتم تثبيت crate أو كتابة PoC أو تشغيل أداة تعريفات. التوصية `windows-sys` مبدئية فقط.


### 20.26 تدقيق UAD-ng: جرد التطبيقات وإدارة حالة الحزم (12 نوفمبر 2026)

المستودع: [Universal-Debloater-Alliance/universal-android-debloater-next-generation](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation)

**نطاق التدقيق:** مراجعة README ومصدر Rust في commit الظاهر في روابط المصدر أدناه؛ لم يتم بناء التطبيق أو تشغيله أو توصيل هاتف. هذا مشروع Debloater موجه أساساً لتصنيف حزم النظام وتغيير حالتها، وليس مدير تطبيقات شامل لكل عمليات APK.

#### 20.26.1 جرد الحزم والحالة

الملفات التي تمت مراجعتها:
- `crates/uad-core/src/utils.rs` — blob SHA `3569162d7ffa96cfa38ae57354c4eb8b9bc28a07`.
- `crates/uad-core/src/adb.rs` — blob SHA `319974dd11cd388d734135358c8019af604bddc8`.
- `crates/uad-core/src/sync.rs` — blob SHA `f90b6eeb575fd3c31541706ed5b738572c94710b`.
- `crates/uad-gui/src/views/list.rs` — blob SHA `a9792dd7cd2ae26e596b66898860c4e95451b9cb`.

ما يفعله المصدر:
- `fetch_packages` يستعلم عن الحزم باستخدام `pm list packages` مع قوائم منفصلة تشمل غير المثبتة، والمفعلة فقط، والمعطلة فقط، مع دعم Android user.
- يبني حالة الحزمة Enabled / Disabled / Uninstalled، ويربط الحزمة بوصف وتصنيف UAD إن وجد.
- واجهة القائمة فيها بحث، تحديد متعدد، وتصفية حسب حالة الحزمة وتصنيف/قائمة التوصيات.
- `CorePackage` يحتوي الاسم والوصف والتصنيف والحالة؛ لا يحتوي firstInstallTime أو lastUpdateTime أو installer أو APK path أو UID.
- أوامر CLI تشمل devices/list/info/disable/enable/uninstall، مع تحديد device serial وAndroid user وdry-run لتغيير الحالة.
- `Phone` يحتفظ بـADB serial وmodel وAndroid SDK وusers.

#### 20.26.2 مخاطر لا تسمح بإعادة استخدام الأوامر كما هي

1. **اختيار الجهاز الافتراضي:** `crates/uad-cli/src/device.rs` يسمح باستخدام أول جهاز عند عدم تحديد serial، ويكتفي بتحذير عند وجود أكثر من جهاز. هذا غير مناسب لبرنامجنا؛ كل عملية يجب أن ترتبط بـDeviceTarget/serial صريح، ويرفض التنفيذ عند تعدد الأجهزة دون اختيار.
2. **أثر مسح البيانات:** `apply_pkg_state_commands` في `sync.rs` قد يضيف `pm clear` ضمن بعض مسارات Disable/Uninstall القديمة/المتوافقة. لا يجوز نقل هذه السلوكيات إلى مدير التطبيقات العام؛ Disable يجب ألا يمسح بيانات المستخدم تلقائياً.
3. **أوامر shell كنص:** `run_adb_shell_action` يمرر action string إلى remote shell. توجد حماية PackageId في request builder، لكن لا نعتبر واجهة raw shell مناسبة كـAPI عامة أو MCP tool. نحتاج typed commands وvalidation عند حدود التنفيذ.
4. **جرد غير ذري:** `fetch_packages` ينفذ استعلامات منفصلة للحزم الكلية والمفعلة والمعطلة؛ قد تتغير حالة حزمة بين الاستعلامات. نحتاج reconcile + verification أو inventory snapshot واضح المصدر/الوقت، لا الادعاء بأنه atomic.
5. **فقد معلومات التطبيقات:** الجرد لا يحقق متطلباتنا المتعلقة بتاريخ التثبيت/آخر تحديث ومصدر التثبيت ونسخة التطبيق وABI وAPK export. هذه تحتاج خدمات إثراء منفصلة.

#### 20.26.3 ما يمكن اقتباسه كمرجع تصميم

- تمثيل حالة الحزمة بصورة مستقلة عن تصنيفها في قائمة توصيات.
- فلاتر مستقلة للحالة، وقائمة التصنيف، ومستوى التوصية.
- دعم Android multi-user وعدم افتراض user 0 في كل الحالات.
- قائمة توصيات الحزم مع توضيح مستوى المخاطرة؛ لكن لا نعرض التوصية على أنها ضمان أمان، ويجب أن تكون قابلة للتحديث ومؤرخة ومفسرة.
- نمط CLI يوضح أوامر الجهاز والجرد وتغيير الحالة، ويمكن الاستفادة منه عند تصميم واجهة التشغيل الآلي، لا نسخ سياسة اختيار أول جهاز أو shell executor.

**القرار:** UAD-ng مرجع مفيد لجرد حالة الحزم وتصنيف debloat، لكنه ليس أساساً كافياً لمدير التطبيقات. نعيد تصميم Package Inventory/State Service مستقلاً، ونستفيد من نماذج البيانات والفلاتر فقط. لا ننسخ أوامر إزالة/تعطيل UAD كما هي، ولا نسمح بمسح بيانات التطبيق كأثر جانبي.

روابط المصدر عند commit `9fcad87bf78c52c4270504bc8880139d95a27a29`: [utils.rs](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation/blob/9fcad87bf78c52c4270504bc8880139d95a27a29/crates/uad-core/src/utils.rs) · [adb.rs](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation/blob/9fcad87bf78c52c4270504bc8880139d95a27a29/crates/uad-core/src/adb.rs) · [sync.rs](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation/blob/9fcad87bf78c52c4270504bc8880139d95a27a29/crates/uad-core/src/sync.rs) · [CLI main.rs](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation/blob/9fcad87bf78c52c4270504bc8880139d95a27a29/crates/uad-cli/src/main.rs) · [device.rs](https://github.com/Universal-Debloater-Alliance/universal-android-debloater-next-generation/blob/9fcad87bf78c52c4270504bc8880139d95a27a29/crates/uad-cli/src/device.rs)

### 20.27 تدقيق ADB Explorer من Alex4SSB: طبقة ADB وWireless والعمليات (12 نوفمبر 2026)

المستودع: [Alex4SSB/ADB-Explorer](https://github.com/Alex4SSB/ADB-Explorer) — تطبيق Windows مبني بـWPF. تمت مراجعة README وملفات ADB service في commit `ce9331b4a3fa3f76b8d43c47d4a514d0099adb56`. لم يتم build/run أو اختبار جهاز.

#### 20.27.1 ما يقدمه فعلياً

- واجهة Windows لإدارة ملفات Android ونقلها وتحريرها عبر ADB.
- يعتمد على AdvancedSharpAdbClient إلى جانب تشغيل ADB CLI في مسارات أخرى.
- اكتشاف الأجهزة وحالاتها، مع polling لقائمة الأجهزة.
- يميز اتصالات Remote ويحل serial الفعلي من transport ID عند الحاجة.
- يدعم Pair/Connect/Disconnect لأجهزة الشبكة، ويستخدم CancellationToken في عمليات ADB.
- يتتبع عمليات ADB النشطة ويمكنه إلغاء العمليات وقتل العملية التابعة.
- README يوفر قائمة بإصدارات Android Platform Tools الرسمية وروابط تنزيلها وchecksums.

#### 20.27.2 مراجعة تنفيذ العمليات

الملفات:
- `ADB Explorer/Services/ADB/ADBService.cs` — blob SHA `8dbda136fbdfd49ef6539a45b9820d6611ba3d10`.
- `ADB Explorer/Services/ADB/AdbService.Devices.cs` — blob SHA `84d9a386485e89670ce3b626784d020eea4dc733`.

ملاحظات:
- `ExecuteCommand` يشغل stdout/stderr بالتوازي، وينتظر العملية مع CancellationToken، ويسجل العمليات النشطة ويقتل العملية عند الإلغاء.
- `ExecuteDeviceAdbCommand` يربط الأمر بـdevice/transport محدد.
- دعم wireless يشمل pair/connect/disconnect، ويستخدم transport routing حتى لا يرسل الأمر إلى transport مختلف عن الاتصال المقصود.
- يوجد دعم Root/Unroot ومسارات Magisk؛ هذه وظائف متقدمة وخطيرة وليست جزءاً من متطلبات V1. لا نعرضها ولا نعيد استخدامها افتراضياً.
- `StartCommandProcess` يبني `ProcessStartInfo.Arguments` من تجميع النصوص عبر `string.Join`، وليس من `ArgumentList` المنفصل. هذا يستلزم تدقيق escaping لكل arguments ومسارات Windows قبل إعادة استخدامه؛ الأفضل في مشروعنا تمرير arguments كقائمة منفصلة حيثما أمكن.
- وجود CancellationToken لا يكفي وحده لإثبات أن كل عمليات النقل/الأوامر آمنة عند الإلغاء؛ نحتاج contract tests للتأكد من إيقاف process tree وتنظيف الملفات الجزئية.

#### 20.27.3 حدود الملاءمة

- المشروع ليس مدير تطبيقات مخصصاً: البحث المصدرّي لم يظهر خدمة جرد تطبيقات شاملة أو timestamps أو install/uninstall workflow متكاملاً.
- لا نستخدمه كبديل عن UAD-ng في تصنيف الحزم أو عن REA في parsing تواريخ التثبيت.
- لا ندمج WPF UI في تطبيقنا إن كان الاختيار المعماري النهائي Tauri/Rust؛ يمكن إعادة استخدام التصميم كسلوك مرجعي، أو اقتباس خوارزمية مستقلة بعد فصلها عن WPF/AdvancedSharpAdbClient.
- نحتاج تدقيقاً مستقلاً لتعامل المشروع مع مسارات الملفات، صلاحيات root، أوامر shell، وتزامن عمليات ADB قبل أخذ أي كود تنفيذي.

**القرار:** ADB Explorer مرجع مفيد لطبقة إدارة جلسات ADB، اختيار/توجيه الجهاز، Wireless Pair/Connect/Disconnect، وإلغاء العمليات. لا نعتمده كمدير تطبيقات أو ننسخ ADBService بالكامل؛ نستخرج متطلبات وعقود سلوكية وننفذ Service Layer مستقلة مع process runner موحد وآمن.

روابط المصدر: [ADBService.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB%20Explorer/Services/ADB/ADBService.cs) · [AdbService.Devices.cs](https://github.com/Alex4SSB/ADB-Explorer/blob/ce9331b4a3fa3f76b8d43c47d4a514d0099adb56/ADB%20Explorer/Services/ADB/AdbService.Devices.cs) · [README](https://github.com/Alex4SSB/ADB-Explorer)

### 20.28 التمييز بين مشاريع ADB Explorer وADB AppControl (12 نوفمبر 2026)

#### 20.28.1 أسماء متشابهة ومشاريع مختلفة

وجدنا أكثر من مشروع باسم ADB Explorer:
- [Alex4SSB/ADB-Explorer](https://github.com/Alex4SSB/ADB-Explorer): Windows/WPF، مشروع أكبر، ومرجع لـADB process/device/wireless.
- [gregko/AdbExplorer](https://github.com/gregko/AdbExplorer): مشروع Windows/.NET 8 صغير يركز على dual-pane file manager ونقل الملفات وفتح أكثر من نافذة. README يذكر USB debugging كمسار الإعداد؛ لا نخلط ذلك مع إثبات دعم wireless أو إدارة التطبيقات.
- [JSleim/adb-file-explorer](https://github.com/JSleim/adb-file-explorer): وصف المشروع يعلن دعم USB/Wi-Fi، تعدد الأجهزة، نقل الملفات، تثبيت APK وتشغيل مهام خلفية قابلة للإلغاء. تعذر جلب README من GitHub connector في هذه الجولة؛ لذلك لا نسجل مراجعة source-level أو اعتماداً قبل إعادة التحقق من المستودع والملفات.

#### 20.28.2 ADB AppControl — مرجع وظيفي وليس مصدراً قابلاً للدمج حالياً

- موقع المنتج يعلن إدارة التطبيقات، تعطيل/إزالة التطبيقات، تثبيت APK/APKS، إدارة بعض الأذونات، batch operations، وأدوات إضافية.
- مستودع [CyberCatDev/ADBAppControl-PC-releases](https://github.com/CyberCatDev/ADBAppControl-PC-releases) يصف نفسه صراحة بأنه مستودع توزيع الإصدارات فقط.
- لم يظهر في المستودع الذي تمت مراجعته مصدر تطبيق Windows نفسه؛ لذلك لا يمكن اعتباره مصدراً جاهزاً لاقتباس الكود.
- نستخدم المنتج كمرجع لتجربة المستخدم وتغطية الوظائف فقط، ونبحث عن تنفيذ كل وظيفة في مشاريع مصدرية منفصلة.

المراجع: [ADB AppControl PC](https://adbappcontrol.com/en/pc/) · [Get Started](https://adb.cybercatsoft.com/en/docs/pc/) · [PC releases repository](https://github.com/CyberCatDev/ADBAppControl-PC-releases)

#### 20.28.3 خلاصة إعادة الاستخدام الحالية

| الوظيفة | أفضل مرجع تمت مراجعته حتى الآن | القرار |
|---|---|---|
| جرد حالة الحزم وتصنيف Debloat | UAD-ng | اقتباس نماذج/فلاتر بعد إزالة آثار UAD الجانبية |
| Device selection وADB process lifecycle | ADB Explorer (Alex4SSB) | مرجع معماري، لا نسخ WPF service مباشرة |
| Wireless Pair/Connect/Disconnect | ADB Explorer (Alex4SSB) + Droidsmith | توحيدها في خدمة Wireless مستقلة مع pairing code سري |
| Install/Uninstall/Enable/Disable عام | Droidsmith + UAD-ng + تدقيق مستقل | لا يوجد مرشح واحد يحقق كل متطلباتنا بأمان |
| تواريخ التثبيت والتحديث | REA parser (20.21) | إثراء lazy per package؛ لا نستنتج التواريخ من ملفات APK |
| Export APK/Split APK | Droidsmith (20.18/20.20) | المرجع الأقوى حالياً |
| تجربة إدارة التطبيقات التجارية | ADB AppControl | مرجع UX/feature set فقط؛ لا source code متاح في repo المراجع |

**القرار العام:** لا نختار مشروعاً واحداً وننسخه كاملاً. الأفضل بناء Service Layer مشتركة مع adapters صغيرة، واقتباس مكونات محددة فقط بعد اختبارها وعزلها. الأولوية التالية: مراجعة AppManager (MuntashirAkon) لمعرفة ما إذا كانت هناك مكونات PackageManager/metadata قابلة للفصل، ثم تدقيق مشاريع تثبيت APK/APKS وbundletool end-to-end. لا يوجد أي اختبار runtime أو جهاز فعلي في هذه الجولة.


### 20.29 تدقيق MuntashirAkon/AppManager: جرد الحزم والبيانات والتثبيت (12 نوفمبر 2026)

المستودع: [MuntashirAkon/AppManager](https://github.com/MuntashirAkon/AppManager)

**تحديد طبيعة المشروع:** App Manager تطبيق Android يعمل على الهاتف نفسه، ويستفيد من Android PackageManager وواجهات نظام داخلية، مع أوضاع صلاحيات إضافية عبر ADB/root. ليس برنامج Windows host يتصل بالهاتف مباشرة، ولا هو مكتبة مستقلة جاهزة يمكن وضعها في تطبيق Tauri. مراجعة المصدر تمت على commit `dc4154f4c04399602895f31f9390bb66b25ea8a0`؛ لم يتم بناء التطبيق أو تشغيله أو اختبار هاتف.

#### 20.29.1 جرد التطبيقات وتعدد المستخدمين

الملفات:
- `app/src/main/java/io/github/muntashirakon/AppManager/utils/PackageUtils.java` — blob SHA `0d14691bdd141fc3cefdf2eaca487865378ef260`.
- `app/src/main/java/io/github/muntashirakon/AppManager/compat/PackageManagerCompat.java` — blob SHA `67f2923a8795775320983eb0463598f4b476197d`.
- `app/src/main/java/io/github/muntashirakon/AppManager/filters/FilterableAppInfo.java` — blob SHA `23b9ade63d8979684582de0ea0f7e7d5970c09fc`.

ملاحظات من المصدر:
- `PackageUtils.getAllPackages` يجمع PackageInfo عبر Android users/profiles التي تسمح صلاحيات التطبيق بقراءتها، ويمكن قصره على المستخدم الحالي.
- `PackageManagerCompat.getInstalledPackages` لديه fallback عند اختلاف عدد الحزم المرجعة عن مرجع الجرد؛ يعيد طلب معلومات الحزم واحدة واحدة، ويتوقف عند interruption، ويضيف تأخيراً دورياً لتقليل ضغط Binder.
- `FilterableAppInfo` يحتفظ ببيانات package/user/UID وlabel/icon/version/firstInstallTime/lastUpdateTime/targetSdk/minSdk، ويحسب بيانات إضافية عند الحاجة.
- بعض التفاصيل مثل مكونات التطبيق، permissions، app-ops، usage وstorage ليست كلها ضمن جرد خفيف؛ تحميلها يحتاج طلبات إضافية وصلاحيات مختلفة.

**الاستفادة:** نمط جرد سريع ثم إثراء التفاصيل lazy، واعتبار Android user جزءاً من هوية التطبيق، وفصل معلومات الحزمة عن معلومات الاستخدام/الأذونات. لا ننقل Android PackageManager APIs إلى Windows؛ نبحث عن حقول ADB المكافئة ونحدد لكل حقل مصدره وإمكانية قراءته.

#### 20.29.2 تواريخ التثبيت ومصدر التثبيت والتصدير المنظم

الملفات:
- `app/src/main/java/io/github/muntashirakon/AppManager/apk/list/ListExporter.java` — blob SHA `78db8bc5fff51a6dc7d22275fe49c98ca80cb837`.
- `app/src/main/java/io/github/muntashirakon/AppManager/db/entity/App.java` — blob SHA `6af023e5baeeb2b997fe23a35aa93fc28829a0ae`.

المصدر يثبت أن النموذج يمكن أن يحمل:
- `firstInstallTime` و`lastUpdateTime` من PackageInfo.
- `installerPackageName` من PackageManagerCompat، مع محاولة إظهار اسم التطبيق المثبت.
- package name/label/versionCode/versionName/minSdk/targetSdk.
- SHA-256 لشهادة التوقيع.
- وجود split APKs عبر `ApplicationInfo.splitSourceDirs` في كيان App.
- تصدير قوائم التطبيقات بصيغ CSV وJSON وXML وMarkdown، متضمنة بيانات التواريخ والمثبت والتوقيع.

**تحفظ مهم:** هذه التواريخ مقدمة من Android PackageManager؛ لا تثبت تاريخ تنزيل ملف APK الأصلي أو تاريخ تثبيت سابق إذا أعيد تثبيت التطبيق. كما أن installer package name لا يثبت وحده أن المصدر الفعلي موثوق؛ ينبغي عدم تسميته Verified Publisher أو Trusted Source.

**الاستفادة:** نعتمد نفس حقول الجرد والتصدير في برنامج Windows، لكن نعرض حالات للحقول: Available / Unsupported / PermissionDenied / ParseFailed / Unknown، ولا نستبدل القيم المفقودة بتاريخ الملف أو تاريخ أول ظهور في قاعدة بياناتنا.

#### 20.29.3 تثبيت APK وSplit APK عبر PackageInstaller

الملفات:
- `app/src/main/java/io/github/muntashirakon/AppManager/apk/installer/PackageInstallerCompat.java` — blob SHA `687709863b0c30bdc3be4a38447bfe3eb835403f`.
- `app/src/main/java/io/github/muntashirakon/AppManager/apk/installer/PackageInstallerViewModel.java` — blob SHA `7fbc11b9dd29ad2777adb260c564d543e01c6612`.

مراجعة المصدر تظهر:
- تثبيت مبني على Android PackageInstaller sessions.
- تمرير ملف ApkFile مع مجموعة split IDs مختارة.
- Session/result callbacks مع operation ID وsession ID لمطابقة النتيجة بالعملية الصحيحة.
- حالات فشل منفصلة، وwatchdog للعمليات الطويلة، وإشعارات تقدم/نتيجة.
- يتعامل مع اختيار المستخدم/تأكيدات النظام، وتوافق المستخدمين وملفات OBB وخيارات installer.
- يوجد تنسيق لإعادة المحاولة في بعض حالات ROM/vendor.

**ما لا ننقله:** PackageInstaller API وواجهات PendingIntent/BroadcastReceiver والـpermissions مرتبطة بتطبيق Android وسياق النظام. لا يمكن استدعاؤها مباشرة من برنامج Windows. نستخدمها كمرجع لسير العمل والحالات والـresult model فقط.

**المقابل عندنا:** ADB host commands:
- APK مفرد: `adb -s <serial> install <apk>`.
- عدة splits: `adb -s <serial> install-multiple <base.apk> <split.apk>...`.
- APKS archive: تحليل/استخراج مجموعة APKs باستخدام bundletool أو parser موثوق، ثم التحقق من مجموعة splits المتوافقة قبل install-multiple.
- APKM/XAPK: لا نفترض أنها نفس صيغة APKS؛ نحتاج parser/adapter لكل container، ونفحص OBB عند XAPK، ثم نعرض Preview للملفات التي ستثبت.

كل عملية تثبيت يجب أن تربط serial صريحاً، وتتحقق من package/version/signer/ABI/minSdk قبل التنفيذ، وتعرض نتيجة ADB/system installer كما وردت. لا نضيف تعطيل تحقق APK أو downgrade أو تجاوز حماية النظام كخيارات افتراضية.

#### 20.29.4 الأذونات والتوقيعات والمكونات

المصدر يقرأ تفاصيل أعمق من مجرد قائمة التطبيقات:
- requested permissions وpermission flags.
- signing certificates وSHA-256.
- activities/services/receivers/providers.
- app-ops وبيانات استخدام/تخزين عند توافر الصلاحيات.
- معلومات التطبيق لكل Android user/profile.

لكن هذه الوظائف تعتمد على PackageManager APIs وصلاحيات ADB/root في بعض الحالات، ولا يمكن ضمان استرجاع جميع البيانات عن طريق ADB shell لكل إصدار Android/OEM.

**القرار:** نضع permission/component inspection ضمن مستوى Advanced Details، ونفصل:
1. Requested permissions: ما يطلبه Manifest.
2. Granted runtime permissions: ما تم منحه فعلياً، إذا أمكن قراءته.
3. AppOps: حالة إضافية مستقلة، لا نخلطها مع granted permissions.
4. Signing certificate digest: بصمة شهادة، وليست شهادة ثقة أو تحققاً من الناشر.
5. Components: بيانات وصفية، لا تعني أن المكون قابل للتشغيل أو متاح في كل user.

#### 20.29.5 ADB mode الداخلي وحدود إعادة الاستخدام

يحتوي المشروع على ADB mode وخادم محلي مساعد يعمل على الهاتف، ويستخدم ADB connection لإطلاق/الاتصال بالخادم ثم تبادل رسائل معه. هذا ليس MCP Server ولا host-side ADB manager. أجزاء من هذا التصميم قد تتطلب تشغيل كود على الهاتف وصلاحيات خاصة، ولذلك لا نعتبره حلاً عاماً لجرد تطبيقات Windows.

**القرار:** لا ندمج App Manager كتطبيق فرعي ولا ننقل طبقة Android UI/PackageManager. نستخدمه كمرجع قوي لحقول الجرد، التعامل مع تعدد المستخدمين، البيانات الوصفية، التصدير، حالات التثبيت، والتحقق من النتائج. وننفذ الجانب الخاص بنا من خلال ADB host services مع capability detection واختبارات على Android/OEM فعلي.

روابط المصدر عند commit `dc4154f4c04399602895f31f9390bb66b25ea8a0`: [PackageUtils.java](https://github.com/MuntashirAkon/AppManager/blob/dc4154f4c04399602895f31f9390bb66b25ea8a0/app/src/main/java/io/github/muntashirakon/AppManager/utils/PackageUtils.java) · [PackageManagerCompat.java](https://github.com/MuntashirAkon/AppManager/blob/dc4154f4c04399602895f31f9390bb66b25ea8a0/app/src/main/java/io/github/muntashirakon/AppManager/compat/PackageManagerCompat.java) · [FilterableAppInfo.java](https://github.com/MuntashirAkon/AppManager/blob/dc4154f4c04399602895f31f9390bb66b25ea8a0/app/src/main/java/io/github/muntashirakon/AppManager/filters/FilterableAppInfo.java) · [ListExporter.java](https://github.com/MuntashirAkon/AppManager/blob/dc4154f4c04399602895f31f9390bb66b25ea8a0/app/src/main/java/io/github/muntashirakon/AppManager/apk/list/ListExporter.java) · [PackageInstallerCompat.java](https://github.com/MuntashirAkon/AppManager/blob/dc4154f4c04399602895f31f9390bb66b25ea8a0/app/src/main/java/io/github/muntashirakon/AppManager/apk/installer/PackageInstallerCompat.java) · [PackageInstallerViewModel.java](https://github.com/MuntashirAkon/AppManager/blob/dc4154f4c04399602895f31f9390bb66b25ea8a0/app/src/main/java/io/github/muntashirakon/AppManager/apk/installer/PackageInstallerViewModel.java)

**الحالة:** مراجعة مصدرية لحقول الجرد والتصفية والتصدير والتثبيت؛ لم يتم بناء App Manager أو تشغيله أو تثبيته على هاتف، ولم يتم تنفيذ تثبيت APK/APKS أو تغيير أذونات/حالة أي تطبيق.


### 20.30 تدقيق Google bundletool: اختيار Split APK المتوافق وتثبيت APKS (12 نوفمبر 2026)

المستودع: [google/bundletool](https://github.com/google/bundletool)

**الهدف:** التحقق من أفضل طريقة جاهزة للتعامل مع Android App Bundle وملفات APK Set بصيغة `.apks`، واختيار splits الصحيحة حسب خصائص الهاتف بدلاً من تثبيت كل APK داخل الأرشيف عشوائياً.

المراجعة المصدرية تمت على commit `586a43a450712a1067f3d92cf7574dee68226302`. لم يتم تنزيل الأداة أو تشغيلها أو تثبيت حزمة على جهاز.

#### 20.30.1 الوظائف المثبتة من المصدر

الملفات:
- `src/main/java/com/android/tools/build/bundletool/commands/InstallApksCommand.java` — blob SHA `506f941095802f170a10f7b23f6c27f0e88cb54b`.
- `src/main/java/com/android/tools/build/bundletool/commands/ExtractApksCommand.java` — blob SHA `d7e45e06ae39e73887e2bec1b10a37e6a972e4c3`.
- `src/main/java/com/android/tools/build/bundletool/commands/GetDeviceSpecCommand.java` — blob SHA `5f7b7ecfcc58569d11ce8966c3b045e73b22810c`.
- `src/main/java/com/android/tools/build/bundletool/device/DeviceAnalyzer.java` — blob SHA `fea52526c6bd6beae2fc6c9d316ec71758f65fbc`.
- `src/main/java/com/android/tools/build/bundletool/device/Device.java` — blob SHA `06e0248e7b3c646a38663ca153280d2d4a0fd283`.
- `src/main/java/com/android/tools/build/bundletool/device/AdbRunner.java` — blob SHA `3c33d2b0b3d8b8d467c25b91d9db3cafeb8bd3f3`.

ما يقدمه:
- استخراج APKs المناسبة من APK Set اعتماداً على DeviceSpec.
- قراءة مواصفات الجهاز عبر ADB: SDK/feature level، ABI list، density، locales، features، GL extensions، وبعض خصائص الجهاز.
- إخراج Device Spec إلى JSON وإعادة استخدامه لاحقاً.
- تثبيت APKS على الجهاز المتصل، واختيار ملفات APK المطلوبة قبل إرسالها.
- دعم اختيار device serial، modules، وبعض خيارات التثبيت.
- دعم local testing modules وملفات إضافية مرتبطة بها.

#### 20.30.2 اختيار الجهاز ومنع الالتباس

- `DeviceAnalyzer` يرفض الجهاز غير الموجود، ويفصل حالة UNAUTHORIZED عن حالة غير ONLINE.
- عند عدم تمرير device ID، يسمح باختيار الجهاز الوحيد فقط، ويرفض تعدد الأجهزة برسالة تطلب `--device-id`.
- `AdbRunner` يعيد التحقق من عدد الأجهزة المطابقة ويرفض عدم وجود جهاز أو وجود أكثر من جهاز.
- `InstallApksCommand` يأخذ `--device-id` ويمرر التثبيت إلى الجهاز المحدد.
- يوجد timeout قابل للتخصيص، والافتراضي في Device هو عشر دقائق.

**قرارنا الأكثر تشدداً:** واجهتنا لا تعتمد على implicit single-device selection حتى لو كانت أداة bundletool تسمح بذلك؛ كل عملية تغيير يجب أن تحمل serial صريحاً من DeviceTarget، مع إعادة التحقق من هوية الاتصال قبل التنفيذ.

#### 20.30.3 Device Spec وABI

DeviceAnalyzer يجمع أكثر من ABI، وليس قيمة ABI واحدة فقط، ويستخدم fallback بين ActivityManager query وخصائص الجهاز عند عدم توفر القائمة الأولى. كما يقرأ density وSDK وlocales، ويتحقق من وجود القيم الأساسية قبل إنشاء المواصفات.

هذا يعالج مباشرة حاجة البرنامج لمعرفة:
- ABI الأولوية وقائمة ABIs المدعومة.
- SDK/API level.
- Screen density.
- Locales.
- خصائص أخرى قد تؤثر على targeting.

**تنبيه:** هذه معلومات توافق حزمة وليست إثباتاً أن كل native library داخل APK سليمة أو أن التطبيق سيعمل فعلياً. نعرض توافقاً متوقعاً مع أسباب واضحة، ونفصل ABI المعلن عن اختبار runtime.

#### 20.30.4 اختيار APKs من APKS

- `ExtractApksCommand` يقرأ metadata/TOC من أرشيف APK Set، ويستخدم ApkMatcher لاختيار ملفات APK المتوافقة مع DeviceSpec.
- يمكن تحديد modules، مع حساب dependencies؛ ويختلف اختيار install-time عن on-demand modules.
- يكتب metadata اختيارية عن الملفات المستخرجة، مثل module name وdelivery type.
- لا يتعامل مع الأرشيف على أنه مجموعة ملفات APK عشوائية؛ الاختيار مبني على targeting الموصوف في metadata.

**القرار:** لا ننفذ APKS extractor خاصاً بنا قبل تجربة bundletool. نستخدمه كأداة خارجية رسمية/مرجعية لعملية APKS، ونبني adapter يتحقق من:
1. ملف الإدخال وصيغته.
2. DeviceTarget والـserial.
3. مواصفات الجهاز ووقت جمعها.
4. قائمة APKs الناتجة وأحجامها وhashes.
5. package/version/signing consistency قبل التثبيت.
6. ملخص Preview يوضح ما سيتم تثبيته.
7. نتيجة التثبيت والتحقق اللاحق من PackageManager/ADB.

#### 20.30.5 حدود الصيغة

- bundletool يتعامل مع Android App Bundle وAPK Set `.apks` الذي تنتجه أدواته/صيغة Bundletool.
- لا نفترض أنه parser عام لصيغ `.apkm` أو `.xapk`؛ هذه تحتاج format-specific adapters ومراجعة metadata وOBB والـsplits.
- لا نعيد تسمية APKM/XAPK إلى APKS أو نمررها إلى bundletool لمجرد أنها ZIP.
- لا نستخدم `build-apks` لبناء APK Set من ملف APK مفرد؛ هذا مسار مختلف عن تثبيت تطبيق موجود.

#### 20.30.6 خيارات التثبيت والآثار الجانبية

في InstallApksCommand:
- `allow-downgrade` الافتراضي false.
- `allow-test-only` الافتراضي false.
- `grant-runtime-permissions` الافتراضي false.
- timeout الافتراضي عشر دقائق.
- يمكن تحديد modules وdevice targeting إضافي.

نحافظ على هذه القيم المحافظة في واجهتنا. أي خيار يسبب downgrade أو grant permissions أو test-only يجب ألا يظهر كخيار افتراضي، ويحتاج تفسيراً وموافقة مستقلة إذا أضيف لاحقاً.

#### 20.30.7 الاختبارات وحدود الدليل

المصدر يحتوي `InstallApksCommandTest` — blob SHA `4d689326a55b21ecacda31ecfaa6c73a7384381d` — ويستخدم FakeAdbServer/FakeDevice وأرشيفات APKS اختبارية، ويغطي إعدادات device ID وخيارات install ومطابقة APK sets. هذا دليل على وجود contract/unit tests للمنطق، وليس اختباراً على هاتف حقيقي أو Windows runtime لدينا.

**القرار النهائي:** bundletool مرشح أساسي للاستخدام المباشر كأداة خارجية في APKS، وليس لإعادة كتابة منطق split selection. نغلفه بـ`apks-service` مع version pinning، process runner موحد، timeouts/cancellation، serial إلزامي، artifact staging، Preview، ثم Verify. نحتاج لاحقاً اختبار Windows فعلي على أجهزة مختلفة تشمل ABI متعددة، لكن لا نعلن نجاح ذلك قبل التنفيذ.

المراجع: [bundletool README](https://github.com/google/bundletool) · [InstallApksCommand.java](https://github.com/google/bundletool/blob/586a43a450712a1067f3d92cf7574dee68226302/src/main/java/com/android/tools/build/bundletool/commands/InstallApksCommand.java) · [ExtractApksCommand.java](https://github.com/google/bundletool/blob/586a43a450712a1067f3d92cf7574dee68226302/src/main/java/com/android/tools/build/bundletool/commands/ExtractApksCommand.java) · [GetDeviceSpecCommand.java](https://github.com/google/bundletool/blob/586a43a450712a1067f3d92cf7574dee68226302/src/main/java/com/android/tools/build/bundletool/commands/GetDeviceSpecCommand.java) · [DeviceAnalyzer.java](https://github.com/google/bundletool/blob/586a43a450712a1067f3d92cf7574dee68226302/src/main/java/com/android/tools/build/bundletool/device/DeviceAnalyzer.java) · [Android Developers bundletool guide](https://developer.android.com/tools/bundletool)

**الحالة:** مراجعة README ومصدر الأوامر والجهاز والاختبارات؛ لم يتم تشغيل bundletool أو تثبيت APKS أو اختبار اتصال Android فعلي.

### 20.31 تدقيق SAI: تحليل APKM/XAPK واختيار Split APKs (12 نوفمبر 2026)

المستودع: [Aefyr/SAI](https://github.com/Aefyr/SAI) — الفرع `master`، commit `55505d231b1390e824d1cc0c8f4fa35fd4677105`.

**حدود الدليل:** مراجعة مصدر Java وبيانات Releases. لم يتم بناء التطبيق أو تشغيله أو تثبيت أي ملف. SAI تطبيق Android وليس برنامج Windows host. وصف README نفسه يذكر أن التطوير محدود/متقطع؛ لذلك لا نعتمد عليه كمنتج نشط مضمون أو كحزمة جاهزة لبرنامج Windows.

#### 20.31.1 دعم APKM

الملفات:
- `installerx/resolver/appmeta/apkm/ApkmAppMetaExtractor.java` — blob SHA `4668f9ce9baec5bf3f4a95c5de290d2f6f3cc861`.
- `installerx/resolver/appmeta/DefaultAppMetaExtractor.java` — blob SHA `34560f99fb849be38beb04b3fa64813675f12b90`.

ما يثبته المصدر:
- يتعرف على امتداد `.apkm` ويفتحه كـZIP archive source.
- يبحث عن `info.json` في جذر الأرشيف.
- يقرأ `apkm_version)، ويعالج الإصدار 5 تحديداً في الكود المراجع.
- يستخرج `pname` و`app_name` و`release_version` و`versioncode` كبيانات وصفية.
- يقرأ `icon.png` اختيارياً.
- إذا لم يتعرف على metadata، ينتقل إلى BruteAppMetaExtractor؛ لكن لا نعتبر fallback ضماناً لدعم كل تنويعات APKM.

بيانات Release الخاصة بـSAI تذكر أن دعم APKM أضيف للملفات غير المشفرة فقط. لا نبني في نطاقنا آلية فك تشفير أو تجاوز حماية. الملف غير المدعوم/المشفّر يجب أن يظهر كصيغة غير قابلة للتحليل مع سبب واضح، لا أن يمر إلى تثبيت عشوائي.

#### 20.31.2 دعم XAPK

الملفات:
- `installerx/resolver/appmeta/xapk/XapkAppMetaExtractor.java` — blob SHA `8a2f8a2b0b7b45fb4b3bc376c99e92674525fdae`.
- `installerx/resolver/meta/impl/DefaultSplitApkSourceMetaResolver.java` — blob SHA `beab25108fc5a375d820acb41c711699448f1441`.
- `installerx/resolver/urimess/impl/DefaultUriMessResolver.java` — blob SHA `c8a4fae1da7cc5ea2cdf73d1bb5e85655a4e2350`.
- `installerx/postprocessing/DeviceInfoAwarePostprocessor.java` — blob SHA `ae612b4abb2bd1dd6ec900720d87396c7e1deb6a`.

ما يثبته المصدر:
- يتعرف على `.xapk` كأرشيف ZIP، ويقرأ `manifest.json` للبيانات الوصفية: `package_name`, `name`, `version_name`, `version_code`.
- يوجد تعليق TODO صريح بخصوص دعم إصدارات مختلفة من XAPK manifest؛ فلا نعتبر parser شاملاً لكل نسخ metadata.
- محلل المجموعة لا يثق بأسماء الملفات وحدها؛ يقرأ Android binary manifest من كل APK داخلي ويستخرج package/version/split metadata.
- يرفض غياب APK أو base APK، ويرفض وجود أكثر من base، ويرفض اختلاف package name أو versionCode بين أجزاء المجموعة.
- يصنف base APK وfeature splits وABI config وlocale config وdensity config وunknown splits.
- Postprocessor يختار split مناسباً من مجموعات ABI واللغة والكثافة وفق بيانات الجهاز/الإعدادات المتاحة في SAI. منطق الاختيار خاص ببيئة Android داخل التطبيق؛ نعيد استخدام قواعد التصنيف كمرجع، ولا ننقل Android APIs إلى Windows.
- عند العثور على ملفات OBB في XAPK، يسجل Notice بأن SAI لا يستخرج OBB تلقائياً؛ توجد رسالة واجهة تطلب من المستخدم التعامل معها يدوياً.

**النتيجة:** SAI يوفر نموذجاً مفيداً لفحص اتساق APK set واختيار split parts، لكنه لا يقدم مسار OBB كاملاً، ولا parser متعدد إصدارات XAPK metadata، ولا Windows host integration.

#### 20.31.3 حدود الاختبارات والملاءمة

- ملفات المصدر التي تمت مراجعتها تثبت implementation paths، لكنها لا تثبت Windows runtime.
- لم يظهر في نطاق الملفات التي راجعناها اختبار شامل لعينة APKM/XAPK على إصدارات metadata متعددة.
- SAI يستخدم Android Context/Resources/Build/Locale APIs، لذلك لا يصلح اقتباسه كما هو داخل Rust/Windows.
- يمكن استخراج قواعد domain مستقلة: base/split validation، package/version consistency، split classification، ABI/locale/density targeting، ونتائج التحذير/الفشل.
- لا ننسخ آلية التثبيت Android PackageInstaller أو طبقة UI أو Android-specific postprocessor.

**القرار:** SAI مرجع source-level قوي لسلوك Split APK، وليس مكوّناً جاهزاً ندمجه. الأولوية هي فصل parser domain محايد عن النظام، ثم استخدام ADB + `install-multiple` في طبقة التنفيذ.

روابط المصدر: [ApkmAppMetaExtractor.java](https://github.com/Aefyr/SAI/blob/55505d231b1390e824d1cc0c8f4fa35fd4677105/app/src/main/java/com/aefyr/sai/installerx/resolver/appmeta/apkm/ApkmAppMetaExtractor.java) · [XapkAppMetaExtractor.java](https://github.com/Aefyr/SAI/blob/55505d231b1390e824d1cc0c8f4fa35fd4677105/app/src/main/java/com/aefyr/sai/installerx/resolver/appmeta/xapk/XapkAppMetaExtractor.java) · [DefaultSplitApkSourceMetaResolver.java](https://github.com/Aefyr/SAI/blob/55505d231b1390e824d1cc0c8f4fa35fd4677105/app/src/main/java/com/aefyr/sai/installerx/resolver/meta/impl/DefaultSplitApkSourceMetaResolver.java) · [DeviceInfoAwarePostprocessor.java](https://github.com/Aefyr/SAI/blob/55505d231b1390e824d1cc0c8f4fa35fd4677105/app/src/main/java/com/aefyr/sai/installerx/postprocessing/DeviceInfoAwarePostprocessor.java)

### 20.32 مراجعة مصادر XAPK إضافية: parser مستقل، أدوات إنشاء، وحدود إعادة الاستخدام (12 نوفمبر 2026)

#### 20.32.1 sebdraven/drakkar — Rust XAPK parser للتحليل

المستودع: [sebdraven/drakkar](https://github.com/sebdraven/drakkar) — commit `1ef884c79a03cfc21e5c4e1170b19c7507be3d08`.

الملف:
- `src/parser/xapk.rs` — blob SHA `a53af44fe33deedc355f546cf22d06c6b98da2aa`.

ما يثبته المصدر:
- Rust module يتعرف على XAPK اعتماداً على محتوى ZIP وليس امتداد الملف وحده.
- يتأكد من عدم وجود AndroidManifest.xml في جذر الأرشيف، ووجود APK entries في الجذر، ثم يفحص APKs الداخلية.
- يحدد base من APK الذي لا يحمل split attribute في Android binary manifest؛ ويعامل غياب base أو تعدد bases كخطأ صريح.
- يحتفظ بالـbase والـsplits في الذاكرة، ويحسب SHA-256 للأرشيف والـbase وكل split.
- يسجل APK entries غير القابلة للقراءة كـanomalies بدلاً من إسقاط كل المعلومات بصمت.
- يحدد split الذي يحتوي native libraries عبر وجود `lib/**.so`، ويصدر container report مع أسماء الملفات وhashes وsplit IDs.

**حدود الاستخدام:**
- المشروع موجه لتحليل APKs، وليس مدير تثبيت Android.
- التعليق في المصدر يوضح أن manifest.json والملفات الإضافية لا تتم قراءتها في هذا المسار؛ لذلك لا يستخرج package metadata من XAPK manifest ولا يدير OBB.
- اختيار base قائم على split attribute، وليس على قواعد XAPK manifest أو DeviceSpec.
- لا يختار ABI/locale/density split المطلوب للتثبيت، ولا ينفذ install-multiple.
- توجد اختبارات داخلية مرتبطة بوحدة XAPK/manifest في المشروع، لكن لم يتم تشغيلها في هذه الجولة، ولم نثبت Windows CI/runtime أو اختبار جهاز.

**القرار:** مرشح قوي لدراسة Rust implementation محايد نسبياً للمنصة: content-based detection، in-memory parsing، hashes، anomaly reporting، واختيار base من binary manifest. لا نقتبس module كاملاً قبل تدقيق حدود الذاكرة/أحجام الأرشيف وعمليات ZIP في بقية المشروع، ولا نستخدمه وحده لتجهيز install set.

رابط المصدر: [xapk.rs](https://github.com/sebdraven/drakkar/blob/1ef884c79a03cfc21e5c4e1170b19c7507be3d08/src/parser/xapk.rs)

#### 20.32.2 knightfall-cs/xapk-tool — أداة إنشاء XAPK وليست parser لتثبيت ملفات موجودة

المستودع: [knightfall-cs/xapk-tool](https://github.com/knightfall-cs/xapk-tool) — commit `33b9b9a2ea64bf3c01e96efb3eb45b508e6ed6af`.

الملف:
- `xapktool.py` — blob SHA `91a954621aff4ab21d8fdbc2e233192fa5371ae6`.

المصدر يعلن أداة Python لإنشاء XAPK من مجلد APK/split APK وOBB، ويستخدم Androguard لقراءة بيانات APK. في الكود توجد checks مفيدة قبل إنشاء bundle:
- اتساق package name وversionCode عبر ملفات APK.
- مقارنة SHA-256 fingerprints لشهادات التوقيع عند توفرها.
- محاولة تحديد base وsplit من manifest split attribute وأسماء الملفات، ورفض الحالة الملتبسة.
- التحقق من صيغة OBB القياسية `main|patch.<version>.<package>.obb` ومطابقة package/version، مع تحذير/تجاوز صريح إذا طلب المستخدم ذلك.
- manifest generator يضع split IDs ومعلومات التطبيق داخل XAPK.

**حدود الاستخدام:** هذه أداة إنشاء/تغليف XAPK من مجلد، وليست أداة فتح XAPK وارد وتحليل ما يجب تثبيته. تصلح كمرجع validation وtest-fixture generation فقط. لم يتم تشغيلها أو توليد ملف تجريبي.

**القرار:** لا نضيفها كاعتماد runtime في برنامجنا حالياً. يمكن لاحقاً استخدامها كمرجع لتصميم اختبارات XAPK synthetic تشمل base/splits وOBB وmismatched package/version/signature، بعد مراجعة fixtures مستقلة.
#### 20.32.3 AppManagerNG — تصنيف أولي لأنواع الحاويات

المستودع: [SysAdminDoc/AppManagerNG](https://github.com/SysAdminDoc/AppManagerNG) — commit `959f11b0d238ec37139d479e1461977509007cf0`.

الملف:
- `ApkBundleHeaderParser.java` — blob SHA `9ac3ac6cbf178f188e34020d616c049b339a01c8`.

المصدر يقدم Header Parser يصنف APK/APKS/APKM/XAPK بالبحث عن `manifest.json`, `info.json`, `toc.pb`/ `BundleConfig.pb`, `base.apk`, split APK names وOBB paths. ويحتفظ flags مثل hasBaseApk وsplitApkCount وhasObbData.

**تحفظات مصدرية:**
- هذا تصنيف heuristic لأسماء entries، وليس فحصاً لصحة محتوى APK أو Android manifest أو package/version/signature.
- parser يجمع أسماء ZIP entries عبر ZipInputStream، ويضع MAX_ZIP_ENTRIES=50,000؛ لكن عند بلوغ الحد يتوقف عن الجمع بدلاً من إرجاع خطأ واضح بأن الأرشيف تجاوز الحد. لا ننسخ هذا السلوك كما هو.
- المصدر يذكر أنه JVM-clean وقابل لاختبار JVM، لكن المستودع نفسه Android app؛ لم نثبت Windows packaging أو تشغيله على Windows.
- قد تتغير نتيجة التصنيف إذا كان الأرشيف يستخدم مجلدات داخلية أو أسماء غير canonical؛ لذلك لا نستخدم اسم entry واحد كدليل نهائي على نوع الملف.

**القرار:** نستخدمه كمرجع لفكرة Header Detection ومصفوفة أنواع الحاويات فقط. في تنفيذنا نحتاج قراءة ZIP central directory بطريقة تتحقق من العدد والحدود وترفض تجاوزها، وتستخدم أكثر من إشارة لتصنيف الصيغة، ثم تحقق APK payload فعلياً.

#### 20.32.4 insomn14/Malimite-Core — XAPK merge لأغراض التحليل فقط

المستودع: [insomn14/Malimite-Core](https://github.com/insomn14/Malimite-Core) — commit `52fac3aeed38c23cb508b3d62ea3f794650b9392`.

الملف:
- `XapkBundle.java` — blob SHA `28bbb880ad49f4eeae3ce4bd51cab38a47466e5a`.

المصدر يوضح أن دمج base + split APKs ينتج APK موحداً لأغراض التحليل فقط، ويصرح أن الناتج **ليس ملفاً قابلاً للتثبيت**. من ضوابط التنفيذ:
- حدود معلنة لعدد APKs وعدد entries وإجمالي bytes.
- فحص أسماء المسارات ورفض absolute paths و`..` وbackslashes وبعض الحالات غير الآمنة.
- اختيار base باستخدام manifest split_apks عند توفره، ثم fallback لقواعد أسماء/manifest.
- OBB والملفات الإضافية لا تُقرأ في مسار الدمج.
- يستخدم APK parser لفحص split manifest، ويحتفظ بترتيب deterministic للـsplits.

**القرار:** لا نستخدم الدمج في مسار التثبيت؛ دمج split resources/manifests بهذه الطريقة لا ينتج بالضرورة حزمة Android صالحة للتثبيت. نستفيد فقط من حدود الحجم/العدد، safe-entry validation، ورفض base ambiguity كأفكار لاختبارات parser.

#### 20.32.5 خلاصة مرشحي APKM/XAPK

| المصدر | الوظيفة التي أثبتها المصدر | الملاءمة لبرنامج Windows | القرار |
|---|---|---|---|
| Aefyr/SAI | APKM v5 metadata، XAPK manifest metadata، APK binary manifest parsing، split classification وtargeting | Android-side؛ OBB غير مدعوم تلقائياً وXAPK manifest versions عليها TODO | مرجع أساسي للسلوك، لا دمج مباشر |
| sebdraven/drakkar | Rust XAPK detection، base/split parsing، hashes وanomalies | أقرب إلى Rust/host-neutral؛ ليس installer ولا يقرأ OBB/manifest.json في المسار | مرشح لاستخراج/إعادة تصميم parser core بعد تدقيق أمني واختبارات |
| knightfall-cs/xapk-tool | إنشاء XAPK والتحقق من اتساق APK/OBB قبل التغليف | Python CLI قابل للتشغيل على Windows وفق README، لكنه ليس inbound parser | مرجع validation وtest fixtures فقط |
| SysAdminDoc/AppManagerNG | تصنيف Header أولي لـAPK/APKS/APKM/XAPK | JVM-clean method، لكن heuristic محدود وAndroid project | مرجع classifier لا أكثر |
| insomn14/Malimite-Core | XAPK split merge لأغراض التحليل | Java core؛ الناتج غير قابل للتثبيت وOBB غير مقروء | استبعاد من install pipeline؛ مرجع حدود/مسارات آمنة |

#### 20.32.6 تصميم Parser موحد مقترح دون خلط الصيغ

الاختيار الصحيح ليس «ZIP واحد لكل الامتدادات»، بل طبقة كشف ثم adapters حسب الصيغة:

1. **Container detector:** يتحقق من magic/signature وبنية ZIP، ثم يصنف APKS/APKM/XAPK بناءً على metadata + entry layout، مع الاحتفاظ بالامتداد كإشارة غير حاسمة.
2. **APKS adapter:** يمرر `.apks` إلى Google bundletool؛ لا يعيد تنفيذ TOC/targeting.
3. **APKM adapter:** يفحص `info.json` ونسخة metadata المدعومة، ثم يتحقق من كل APK داخلياً. إذا كان metadata version غير معروف أو الملف غير قابل للقراءة، يتوقف مع سبب واضح.
4. **XAPK adapter:** يقرأ `manifest.json` إن وجد، ويحدد base/split entries، ثم يراجع Android binary manifest لكل APK؛ إذا غاب manifest أو تعارض مع محتوى APK، لا يعتمد على metadata وحدها.
5. **OBB inventory:** يكتشف ملفات OBB ويعرض اسمها وحجمها وpackage/version المستنتجين من الاسم والمحتوى المتاح. لا ينسخها أو يضعها على الهاتف تلقائياً قبل تحديد وجهة Android/obb وموافقة المستخدم.
6. **APK validation:** package name/versionCode consistency، وجود base واحد فقط، split IDs، minSdk/targetSdk، ABI/native libraries، locale/density، وsigner certificate digest consistency. اختلاف التوقيع أو تعارض package/version يجعل النتيجة Blocked أو Requires Review؛ لا نتجاهله بصمت.
7. **Device compatibility:** يجمع SDK/ABIs/density/locales من الجهاز المحدد، ثم ينتج install set واضحاً مع أسباب اختيار/استبعاد كل split.
8. **Preview:** يعرض الحاوية المكتشفة، base، كل split، OBB، الأحجام، hashes، signer digests، توافق الجهاز، والتحذيرات قبل أي كتابة.
9. **Apply:** يمرر APK set المختار فقط إلى `adb -s <serial> install-multiple` بعد approval. OBB deployment مسار منفصل بموافقة واضحة، ثم Verify.

**قواعد أمان ZIP إلزامية:** حدود entries/total uncompressed bytes/individual entry size/metadata size، رفض path traversal وabsolute paths، منع symlink surprises، منع duplicate entry names أو تسجيلها كخطأ، فحص compression ratio، timeout/cancellation، وعدم استخراج الملفات إلى مسار يختاره agent بحرية. لا نعتمد على declared ZIP sizes وحدها؛ نعدّ bytes الفعلية أثناء القراءة.

**القرار الحالي:** نستخدم bundletool مباشرة لـAPKS، وSAI مرجعاً رئيسياً لفهم APKM/XAPK split semantics، وDrakkar مرشحاً لطبقة Rust XAPK parser بعد فحص أمني أوسع. لا يوجد حتى الآن parser Windows مثبت أو اختبار على هاتف، ولا نعتمد أي مشروع كمكوّن production قبل PoC واختبارات fixtures متعددة المصادر.

**حالة الجولة:** [x] مراجعة SAI APKM/XAPK source paths. [x] مراجعة Rust XAPK parser. [x] مراجعة XAPK creation/validation tool. [x] مراجعة header classifier وmerge-only parser. [x] توثيق الفروق بين format adapters. [ ] مراجعة كاملة لحدود ZIP في المرشحين. [ ] بناء parser PoC على Windows. [ ] اختبارات fixtures حقيقية من مصادر مختلفة. [ ] اختبار DeviceSpec/install-multiple على أجهزة فعلية. [ ] اعتماد أي كود production.

### 20.33 تدقيق أمان ZIP/APK: Zip Slip وZip Bomb والروابط الرمزية والتحقق من التوقيع (12 نوفمبر 2026)

**نوع المراجعة:** مراجعة مصدرية لكود مكتبات ZIP وDrakkar، ومراجعة وثائق Android وOWASP. لم يتم تشغيل fuzzing أو توليد أرشيفات هجومية أو بناء PoC. هذه متطلبات تصميم واختبار، وليست إثباتاً بأن البرنامج الحالي محمي.

#### 20.33.1 zip-rs/zip2 — مكوّن Rust مرشح لقراءة الأرشيف

المستودع: [zip-rs/zip2](https://github.com/zip-rs/zip2) — commit `6cdc6f35d99c8c0598a04a183c512b3b91044137`.

الملفات التي تمت مراجعتها:
- `src/read/zipfile.rs` — blob SHA `785d47235e323c9dd3bc4b461a1d2603cf48589f`.
- `src/read/extract.rs` — blob SHA `4186058ce0cea1b3f5bfdb0bd68ba581d5331763`.
- `examples/extract.rs` — blob SHA `3223aa63bbfbb9bb7f7fabcf2f446303c4950a26`.
- `CHANGELOG.md` — blob SHA `7ab4f8e687dab6150efc3cc07761e26b15d5ac12`.

ما يوفره المصدر:
- `ZipFile::enclosed_name()` يرفض أسماء المسارات التي تحتوي NULL أو مساراً مطلقاً أو تخرج من مجلد الاستخراج. توثيق المكتبة يحذر صراحةً من استخدام `name()` مباشرة عند استخراج الملفات.
- يوفر `is_symlink()` و`is_dir()` لتمييز أنواع entries.
- `ExtractOptions::with_size_limit()` يحدد حداً لحجم البيانات المفكوك المعلن، و`enforce_header_consistency(true)` يطلب التحقق من اتساق local headers مع central directory.
- خيارات التعامل مع symlinks متعددة؛ منها `NoExtract`، ويجب اختيار الرفض/عدم الاستخراج في برنامجنا بدلاً من اتباع الروابط.
- المصدر يضع سقفاً داخلياً قدره 64 MiB للـpreallocation، ويوثق إصلاحاً لحالة symlink ذات حجم معلن ضخم كانت قد تسبب OOM.

**تحفظات مهمة:**
- `ExtractOptions::default()` يبدأ بـ`size_limit: None` و`enforce_header_consistency: false`. لا يجوز الاعتماد على defaults؛ يجب ضبط الخيارات صراحةً.
- `with_size_limit` يفحص الحجم الإجمالي المعلن للأرشيف عند مسار extraction، وليس بديلاً عن عدّ bytes الفعلية أثناء القراءة أو حدود مستقلة لكل entry.
- `enclosed_name` يحمي مسار الملف، لكنه لا يقرر إن كان APK صالحاً أو إن كان محتوى الأرشيف آمناً من استنزاف الموارد.
- سقف preallocation الداخلي ليس سقفاً لإجمالي البيانات التي يمكن فك ضغطها.

**القرار:** `zip-rs/zip2` هو المرشح الأول لمكوّن ZIP reader في Rust PoC، مع حدودنا الخاصة للـentries والأحجام الفعلية والوقت، ورفض symlinks، والتحقق من headers. لا نستخدم وظيفة extract العامة مباشرة كخط تثبيت؛ نقرأ entries المطلوبة ونكتبها إلى staging بأسماء مولدة داخلياً.

#### 20.33.2 Drakkar — لماذا لا نقتبس قارئه المتسامح في مسار التثبيت

المستودع: [sebdraven/drakkar](https://github.com/sebdraven/drakkar) — commit `1ef884c79a03cfc21e5c4e1170b19c7507be3d08`.

الملفات:
- `src/parser/xapk.rs` — blob SHA `a53af44fe33deedc355f546cf22d06c6b98da2aa`.
- `src/parser/apkzip.rs` — blob SHA `4437da7962dfa041fbe0a45cc54d6c5a453de998`.
- `src/parser/mod.rs` — blob SHA `45515d6ed6a16d0122c2f77a7f68932487d51215`.

نتائج مراجعة المصدر:
- المسار الاسمي في `xapk.rs` يخصص `Vec::with_capacity(entry.size() as usize)` قبل قراءة APK الداخلي. الحجم يأتي من metadata الأرشيف، وهو مدخل غير موثوق؛ لا ننسخ هذا النمط. يجب القراءة في chunks مع حد فعلي، أو استخدام bounded writer.
- إذا رفضت مكتبة ZIP الأرشيف أو entry، يحاول Drakkar الرجوع إلى `TolerantZip`.
- قارئ `apkzip.rs` يعلن صراحةً أنه يتجاهل CRC وعلامة التشفير، ولا يعتمد طريقة الضغط المعلنة؛ يجرب deflate ثم stored، ويقرأ بعض الحقول اعتماداً على تقديرات/بدائل عند فساد البيانات.
- يوجد حد 512 MiB لحجم APK الكامل الذي يقرأه قارئه المتسامح من القرص، لكنه لا يعالج وحده مشكلة التخصيص المسبق في المسار الاسمي، ولا يثبت وجود حد صارم لكل entry أو إجمالي bytes بعد فك الضغط.
- المصدر يصف هذا القارئ بأنه tolerant لمعالجة APKs معدلة/مشوهة لأغراض التحليل؛ وهذا هدف مختلف عن قبول ملفات للتثبيت.

**القرار:** لا نستخدم `TolerantZip` ولا fallback يتجاوز فشل parser القياسي في مسار APKM/XAPK installer. إذا رفض parser القياسي archive أو entry بسبب header/CRC/encryption/compression inconsistency، تكون النتيجة Blocked أو Requires Review مع سبب محدد. يظل Drakkar مرجعاً لخوارزمية اكتشاف base/split وhashes فقط.

#### 20.33.3 ضوابط ZIP الإلزامية في برنامجنا

1. **كشف المحتوى أولاً:** الامتداد مجرد إشارة. نتحقق من ZIP structure وmetadata لكل صيغة، ولا نثق في `manifest.json` أو `info.json` وحدهما.
2. **Entry inventory قبل الاستخراج:** نقرأ central directory وننشئ قائمة metadata فقط، ثم نتحقق من العدد والأسماء والتكرارات والأنواع والأحجام قبل قراءة payload.
3. **حدود موارد متعددة:** سقف لعدد entries، وحجم الأرشيف المضغوط، وإجمالي الحجم المفكوك المعلن، وحجم entry الواحد، وحجم metadata/JSON، وإجمالي bytes الفعلية التي خرجت من decompressor، ومدة العملية والذاكرة المؤقتة. القيم الرقمية تضبط بعد جمع corpus من ملفات مشروعة، ولا تُخترع من غير قياس.
4. **التحقق من الحجم الفعلي:** لا نثق في `uncompressed_size` أو compression ratio المعلن وحدهما؛ نعدّ كل chunk فعلياً ونوقف القراءة فور تجاوز أي حد. Compression ratio إنذار إضافي، وليس آلية الحماية الوحيدة.
5. **Zip Slip:** نرفض absolute paths و`..` وNULL وWindows drive/UNC paths وأسماء المسارات غير الصالحة. لا نستخدم entry name كمسار كتابة مباشرة.
6. **Symlinks وspecial files:** نرفض symlink وأي entry ليست regular file أو directory مسموحاً به. لا ننشئ hard links أو reparse points من بيانات الأرشيف.
7. **Duplicate names:** نرفض duplicate raw names، وكذلك الأسماء التي تصبح متطابقة بعد normalization/case-folding على Windows. لا نسمح باستبدال entry بآخر لاحقاً.
8. **Header consistency:** نتحقق من local file header مقابل central directory، ومن offsets/bounds/overlaps، ونرفض الأرشيفات المتناقضة بدلاً من التخمين.
9. **CRC/integrity:** نتحقق من CRC لكل entry بعد فك الضغط، ونرفض CRC mismatch. لا نستخدم قارئاً يتجاهل CRC أو encryption flags في مسار التثبيت.
10. **التخزين المرحلي:** لا نفك الأرشيف إلى مجلد يختاره المستخدم أو agent. نستخدم staging directory خاصاً بالتطبيق، محدود الصلاحيات، واسم ملف مولداً من index/hash، مع تنظيف مضمون عند الفشل والإلغاء.
11. **الملفات المطلوبة فقط:** لا نفك كل محتويات XAPK/APKM بشكل أعمى. نقرأ metadata وAPK entries المطلوبة، ونعالج OBB كنوع منفصل.
12. **الاستمرارية والإلغاء:** timeouts، cancellation، disk quota، memory budget، وتنظيف partial files. لا تبدأ عمليات ADB أثناء التحليل.
13. **لا silent fallback:** فشل parser أو signature check لا يؤدي إلى قارئ أكثر تساهلاً ثم تثبيت الناتج. يمكن حفظ تقرير تشخيصي read-only، لكن لا نرفع مستوى الثقة في الملف.

#### 20.33.4 توقيع APK وهوية المجموعة

المرجع الأساسي هو أداة Android الرسمية `apksigner` من Android SDK Build Tools، وليس مجرد قراءة ملفات `META-INF` أو مقارنة hashes غير موثوقة.

مسار التحقق المقترح لكل APK داخلي:
- تشغيل `apksigner verify --verbose --print-certs -Werr <apk>` كعملية محددة المعاملات، دون shell command string.
- تسجيل نتيجة التحقق، schemes التي تم التحقق منها، وعدد signers وبصمة شهادة SHA-256 لكل signer.
- فشل التحقق أو warnings التي نقرر معاملتها كخطأ تمنع الانتقال إلى Preview قابل للتثبيت.
- التحقق من اتساق هوية التوقيع عبر base وكل split. لا نكتفي بمقارنة SHA-256 نصياً دون فهم certificate rotation/signing lineage؛ نحتاج دعم lineage وفق أدوات Android الرسمية عند وجود تدوير مفاتيح.
- metadata داخل XAPK/APKM ليست دليلاً على هوية الناشر؛ لا نعتمد اسم التطبيق أو package name المعلن قبل مطابقته مع Android manifest داخل APK.
- لا نعتبر نجاح `apksigner` وحده إثباتاً أن التطبيق آمن أو موثوق المصدر؛ هو يثبت صحة التوقيع وسلامة المحتوى وفق scheme، وليس سمعة المطور.
- يبقى Android Package Manager على الجهاز هو الحكم النهائي عند التثبيت؛ تحقق الكمبيوتر مرحلة preflight وليس ضماناً لقبول الجهاز.

وثائق Android توضح أن v2/v3 تحقق من محتوى APK بالكامل وتمنع تعديل المحتوى المحمي دون كشفه، وأن `apksigner` يعرض schemes والشهادات وبصماتها. لا نعيد تنفيذ التحقق cryptographic بأنفسنا ما دامت أداة Android الرسمية تؤدي ذلك.

#### 20.33.5 اختبارات أمان مطلوبة قبل اعتماد parser

| الحالة | السلوك المتوقع |
|---|---|
| ZIP Slip: `../outside.apk` أو absolute path | رفض entry، وعدم إنشاء أي ملف خارج staging |
| Windows path: drive/UNC/backslash traversal | رفض |
| symlink أو special entry | رفض دون اتباع الرابط |
| duplicate entry names أو collision بعد case-folding | رفض |
| ZIP bomb / declared size صغير لكن output الفعلي ضخم | إيقاف عند حد bytes الفعلي |
| entry count أو JSON metadata ضخم | رفض قبل استهلاك موارد غير محدودة |
| CRC mismatch | رفض |
| local header وcentral directory مختلفان | رفض |
| overlapping offsets أو entry data خارج حدود الملف | رفض |
| encrypted entry أو compression method غير مدعوم | رفض مع سبب واضح |
| ملف APK داخلي تالف أو binary manifest غير قابل للقراءة | لا يمر إلى install set |
| base مفقود أو أكثر من base | رفض المجموعة |
| package/version/signing identity غير متسقة بين splits | Blocked/Requires Review |
| إلغاء أثناء فك الضغط | إيقاف العملية وتنظيف staging |
| امتلاء القرص أو timeout | فشل آمن دون تنفيذ ADB |

يضاف fuzzing لـZIP central directory وlocal headers وAndroid binary manifest، واختبارات regression ثابتة لكل حالة، واختبار على Windows CI. لا تُعتبر اختبارات parser وحدها بديلاً عن اختبار التثبيت على أجهزة Android فعلية.

#### 20.33.6 القرار الهندسي

- **مرشح ZIP:** `zip-rs/zip2` مع إعدادات صريحة وحدود موارد إضافية.
- **مرفوض من install path:** Drakkar tolerant ZIP fallback؛ بسبب تجاهل CRC/encryption flags وقبول بيانات مشوهة، إضافة إلى تخصيصات مبنية على أحجام معلنة.
- **توقيع APK:** Android `apksigner` كأداة تحقق خارجية مثبتة الإصدار، مع تشغيل آمن وقراءة structured output.
- **Extraction:** لا يوجد general-purpose extraction إلى مسارات archive. نقرأ entries المطلوبة إلى staging بأسماء داخلية مولدة.
- **OBB:** يبقى منفصلاً عن APK install set، مع preview وموافقة صريحة قبل النقل.
- **الحالة:** توصية معمارية source-backed، وليست اعتماداً نهائياً لمكتبة أو إثباتاً أمنياً. لا PoC، لا fuzzing، لا تشغيل APK، ولا اختبار هاتف في هذه الجولة.

المراجع:
- [zip-rs/zip2 source](https://github.com/zip-rs/zip2/tree/6cdc6f35d99c8c0598a04a183c512b3b91044137)
- [Drakkar XAPK parser](https://github.com/sebdraven/drakkar/blob/1ef884c79a03cfc21e5c4e1170b19c7507be3d08/src/parser/xapk.rs)
- [Drakkar tolerant ZIP reader](https://github.com/sebdraven/drakkar/blob/1ef884c79a03cfc21e5c4e1170b19c7507be3d08/src/parser/apkzip.rs)
- [Android APK Signature Scheme v2](https://source.android.com/docs/security/features/apksigning/v2)
- [Android APK Signature Scheme v3](https://source.android.com/docs/security/features/apksigning/v3)
- [Android apksigner](https://developer.android.com/tools/apksigner)
- [OWASP ZIP Slip, symlink and archive bomb testing](https://wstg.owasp.org/stable/4-Web_Application_Security_Testing/10-Business_Logic_Testing/09-Test_Upload_of_Malicious_Files/)


### 20.34 محللات Android Manifest وAPK الجاهزة: مقارنة AXML Parser وAPK Parser وAndroid SDK tools (12 نوفمبر 2026)

**نوع المراجعة:** مراجعة source-level لمشاريع Androguard ومراجعة وثائق Android الرسمية. لا يوجد build أو تشغيل CLI أو Windows runtime test أو اختبار APK من ملفات المستخدم في هذه الجولة.

#### 20.34.1 Android SDK — apkanalyzer وaapt2

الأدوات الرسمية في Android SDK تظل مرجعاً عملياً قوياً لتحليل APK على جهاز Windows بعد تثبيت Android SDK Build Tools/Command-line tools.

- apkanalyzer manifest print <apk> يعرض Manifest بصيغة XML.
- apkanalyzer manifest application-id وversion-name وversion-code وmin-sdk وtarget-sdk تستخرج حقولاً محددة.
- apkanalyzer files list يعرض محتويات APK، وfiles cat يقرأ entry من الأرشيف.
- aapt2 dump badging وaapt2 dump permissions خياران عمليان لاستخراج معلومات تعريف التطبيق والصلاحيات.
- هذه الأدوات تتعامل مع APK داخلي؛ لا نفترض أنها تحلل APKM/XAPK/APKS كحاويات خارجية. طبقة archive adapter الآمنة تسبقها.
- لا نعتبر إخراج CLI وحده دليلاً على صحة التوقيع؛ نستخدم apksigner verify منفصلاً.

**القرار:** نجعل Android SDK tools خياراً مرجعياً/adapter أولياً في Windows PoC، خصوصاً لمقارنة النتائج والتحقق المتبادل. نثبت نسخة Build Tools ونفحص exit code وstderr وtimeouts ونحلل المخرجات إلى DTOs typed بدلاً من عرض نص خام فقط. لا يوجد تأكيد تشغيل Windows حتى الآن.

#### 20.34.2 androguard/axml-parser — محلل AXML/ARSC بلغة Rust

المستودع: [androguard/axml-parser](https://github.com/androguard/axml-parser) — default branch commit المراجع 93d008a7ce337fb9bafb708bc8ce93ecdc25f80b.

الملفات:
- src/axml/parser.rs — blob SHA 079701dbe0e30e813212a0e0df4cb035e9fd87ff.
- src/stringblock.rs — blob SHA 1cc20576e4a4e5eaecb605d86b1cb772e4c70cb5.
- src/header.rs — blob SHA f00ecec7eb995b55c832e57a465c2fbc03d90d82.
- src/lib.rs — blob SHA 42e7179c3977524f3d05671f2e1f56b77ebefe60.

ما يقدمه:
- Pure Rust library مع CLI باسمَي axml وarsc، وPython bindings اختيارية.
- يفك Android binary XML، ويقرأ package/version/permissions، كما يحلل resources.arsc ويحل resource IDs.
- API مباشر: AXMLPrinter وARSCParser، ويمكن دمجه دون تشغيل عملية خارجية.
- README يذكر fixtures واختبارات عبر cargo test، لكن لم نشغّلها.
- هذا parser لطبقة AXML/ARSC وليس APK archive parser كاملاً؛ يحتاج قارئ ZIP آمن لاستخراج AndroidManifest.xml وresources.arsc من APK.

**نتائج مراجعة حدود الإدخال:** وجدنا مواضع تستحق PoC أمني قبل الاعتماد:
- StringBlock::parse يستخدم Vec::with_capacity(string_count) وVec::with_capacity(style_count) اعتماداً على أعداد من الملف، ويحسب أحجاماً من header_size - strings_offset وheader_size - styles_offset دون ظهور تحقق شامل من ترتيب هذه offsets وحدود chunk قبل التخصيص والقراءة.
- AXMLParser::do_next يقرأ attribute_count من البيانات ويضيف عناصر إلى vector، كما أن بعض مسارات القراءة تستخدم default values عند فشل القراءة؛ نحتاج التحقق من أن كل chunk والـattribute array محصوران داخل حدود chunk والملف.
- هذه ملاحظات مراجعة كود وليست إثبات استغلال. لكنها تمنع اعتماده production مباشرة قبل إضافة limits وفحوص bounds واختبارات malformed-input/fuzzing.

**القرار:** مرشح جيد كـRust AXML/ARSC component في PoC بعد تدقيق وإصلاح حدود الإدخال. لا نمرر إليه bytes غير محدودة من APK غير موثوق، ولا نعتبره بديلاً عن ZIP reader أو apksigner.

#### 20.34.3 androguard/apk-parser — مكتبة APK/APKM كاملة (Rust + Python)

المستودع: [androguard/apk-parser](https://github.com/androguard/apk-parser) — default branch commit المراجع 95ec9b00b4e2e30d16ac98c09868e5d6b58d3fdb.

الملفات:
- README.md — blob SHA 5bf6c110de7b8cc6fc5dec6039405bb03e41380b.
- apkparser-rs/README.md — blob SHA d7c8f7af5c4d435f4afb137da51e13410742ce57.
- apkparser-rs/src/lib.rs — blob SHA b69347330f0622d372c432fc8fff428450e79b83.
- apkparser-rs/src/apkm.rs — blob SHA 6720cd960a82de1193cdf52910fb25867f36dbca.
- apkparser-rs/src/zip.rs — blob SHA 65a2398f774f89f563bb77bfca1c7a9c672576c0.
- apkparser-rs/src/main.rs — blob SHA 16e05fba268b2a48ad80d1e4d5791703f58aba5c.

ما يقدمه:
- Rust library وCLI، وPython facade.
- APK metadata: package, versionCode/versionName, minSdk/targetSdk, requested/declared permissions، entries وDEX names.
- قراءة بعض معلومات التوقيع v1/v2/v3 وبصمات الشهادات.
- يدعم APKM تحديداً: base/split enumeration وunwrap؛ لا يعني ذلك دعماً عاماً لكل XAPK/APKS/APKM variants.
- يوفر tests وsample APKs بحسب README؛ لم يتم تشغيلها.

**تحفظ أمني حاسم:** README يصف ZIP reader بأنه lenient ويتسامح مع Extra Field TLV غير السليم وطرق ضغط معدلة لأغراض تحليل ملفات مشوهة. مراجعة zip.rs تؤكد وجود fallback من deflate إلى stored، وتسمح ببعض حالات local/central mismatch مع تسجيلها، وتستخدم capped output. هذا مفيد في malware forensics لكنه لا يطابق سياسة القبول الصارمة التي وضعناها لمسار التثبيت.

كما أن ApkmArchive::extract_apks_to يكتب entries إلى مجلد عبر safe_filename لتقليل مخاطر path traversal، لكننا لا نعتبر هذا وحده إثباتاً لاستيفاء حدود ZIP الأمنية لدينا: يلزم رفض duplicate/case-collision، والتحقق من CRC/header consistency، وحدود عدد entries والـmetadata والـactual output، وstaging آمن. كما أن الاستخراج المباشر ليس مطلوباً أصلاً في مسارنا؛ نفضل قراءة entries المطلوبة إلى staging داخلي مضبوط.

**القرار:** لا نعتمد apk-parser الحالي مباشرةً في install pipeline بسبب سياسة ZIP المتسامحة. يمكن الاستفادة من نماذج DTO وحقول Manifest وAPKM classification كمرجع، أو إجراء PoC منفصل لتحليل read-only مع مدخلات محدودة. لا ننسخ ZIP fallback أو extraction إلى الإنتاج دون مراجعة/تعديل.

#### 20.34.4 Androguard 5 — طبقة تحليل أعلى من APK parser

المستودع: [androguard/androguard](https://github.com/androguard/androguard) — commit 581f7a3251545a97f66ee92b674dd810ee380a00.

الملفات:
- pyproject.toml — blob SHA 71f9f2eb56a9227bd614c060173c92b079791d5c.
- androguard/application.py — blob SHA 7513843697bf8fe9c6a3a8f90833047d307a2cc2.
- androguard/cli/main.py — blob SHA 0b77cd03045334e85bbfbae2b25a6da843ee6a28.

المشروع يعرض Application API وCLI، ويعتمد على apkparser-ag وdexparser-ag وAXML parser. يمكنه قراءة Manifest وDEX وتحليل strings/classes/methods، كما يتيح أوامر مثل --getmanifest وملخص APK. لكنه أوسع من احتياجنا الأساسي لاستخراج package/version/SDK/permissions/ABI، ويضيف تبعيات Python/Rust.

**القرار:** ليس dependency افتراضياً للبرنامج. نحتفظ به كأداة تحليل متقدمة اختيارية أو مرجع بحثي، بينما نبدأ في V1 بمكوّن أخف: Android SDK CLI adapter أو Rust AXML parser بعد hardening، مع ZIP layer الصارم المستقل.

#### 20.34.5 ما الذي نحتاجه فعلياً من محلل APK في برنامجنا؟

نستخرج ونطبع إلى نموذج موحد:
- Package ID، versionName، versionCode.
- minSdkVersion، targetSdkVersion، maxSdkVersion إن وُجد.
- requested permissions وdeclared permissions بشكل منفصل.
- Activities/services/receivers/providers عند الحاجة، وليس بالضرورة في شاشة القائمة الأساسية.
- native libraries paths وABI inventory من lib/<abi>/*.so، مع التحقق من ELF headers عند الحاجة.
- split name/configuration metadata عند وجود split APK.
- SDK/ABI compatibility estimate مع DeviceSpec؛ النتيجة Preview وليست ضماناً للتثبيت.
- مصدر الحقل: manifest أو archive metadata أو SDK tool output؛ لا نخلط metadata الخارجية مع الحقيقة الموجودة داخل APK.
- أخطاء parsing والتحذيرات وحالة التحقق، مع الاحتفاظ بالفرق بين Missing وInvalid وUnsupported.

#### 20.34.6 القرار المقترح وخطة PoC

1. **Baseline مرجعي:** Android SDK apkanalyzer وaapt2 لاستخراج Manifest والحقول المطلوبة من APK على Windows.
2. **Rust in-process candidate:** androguard/axml-parser بعد إصلاح/اختبار حدود chunk وstring pool وattribute counts.
3. **Not approved for install path:** androguard/apk-parser الحالي بسبب lenient ZIP behavior؛ لا نستخدمه لتجاوز ZIP security policy.
4. **Optional advanced analyzer:** Androguard 5 للتحليل المتقدم، وليس شرطاً في V1.
5. **Signature:** apksigner مستقل ومثبت الإصدار؛ لا نستبدله بفحص signature metadata داخل parser.
6. **Container adapters:** APKS عبر bundletool؛ APKM/XAPK عبر adapters منفصلة؛ كل APK داخلي يمر عبر نفس Manifest/ABI/signature validation.
7. **Cross-check fixtures:** نمرر نفس APK corpus عبر apkanalyzer وaapt2 وAXML parser ونقارن الحقول؛ أي اختلاف يسجل ولا نخفيه.
8. **Security gate:** malformed AXML, oversized string/style counts, invalid offsets, truncated chunks, huge attribute counts, malformed UTF-8/UTF-16, nested archive limits، fuzzing وWindows CI.
9. **Performance gate:** قياس bulk APK metadata extraction، ذاكرة كل ملف، ووقت التحليل قبل تحديد parallelism أو caching.
10. **Approval gate:** لا نختار dependency production ولا ندمج كوداً قبل PoC على Windows ونتائج الاختبارات.

**حالة الجولة:** [x] مراجعة Android SDK apkanalyzer docs. [x] مراجعة Androguard axml-parser source. [x] مراجعة apk-parser Rust/Python APIs وAPKM extraction. [x] مراجعة Androguard 5 dependency/API. [ ] تشغيل Android SDK tools على Windows. [ ] تشغيل cargo tests للمكتبات. [ ] إضافة AXML malformed fixtures/fuzzing. [ ] PoC ومقارنة النتائج. [ ] اختبار corpus APK/APKM/XAPK/APKS. [ ] اعتماد production.

**خلاصة الاختيار:** لا نكتب Manifest parser جديداً الآن. نبدأ بمقارنة Android SDK tools كـbaseline، ونختبر axml-parser كخيار Rust داخلي بعد hardening. نرفض إدخال lenient ZIP reader إلى مسار التثبيت، ونفصل تحليل Manifest عن قراءة الأرشيف والتحقق من التوقيع.

المراجع:
- [Android apkanalyzer documentation](https://developer.android.com/tools/apkanalyzer)
- [Android AAPT2 documentation](https://developer.android.com/tools/aapt2)
- [androguard/axml-parser source](https://github.com/androguard/axml-parser/tree/93d008a7ce337fb9bafb708bc8ce93ecdc25f80b)
- [androguard/apk-parser source](https://github.com/androguard/apk-parser/tree/95ec9b00b4e2e30d16ac98c09868e5d6b58d3fdb)
- [androguard/androguard source](https://github.com/androguard/androguard/tree/581f7a3251545a97f66ee92b674dd810ee380a00)


### 20.35 فحص ABI وELF وNative Libraries داخل APK (12 نوفمبر 2026)

**نوع المراجعة:** توثيق Android ABI الرسمي + source-level review لمكتبات Rust ELF. لم يتم بناء أو تشغيل أي parser، ولم يتم فحص APK فعلي أو الاتصال بهاتف في هذه الجولة.

#### 20.35.1 ما الذي يجب أن يكتشفه البرنامج؟

نحتاج فصل ثلاث طبقات وعدم دمجها في نتيجة واحدة:

1. **Device ABI capability:** قائمة ABI التي يعلنها Android للجهاز، بالترتيب الذي يعيده النظام، مع ABI أساسية وثانوية إن توفرت.
2. **APK native payload:** مجلدات lib/<abi>/ وملفات .so الموجودة فعلياً داخل كل APK أو split APK.
3. **ELF identity:** معمارية كل ملف .so الفعلية من ELF header، لا من اسم المجلد فقط.

Android NDK يوضح أن ABI تشمل instruction set وendianness وcalling conventions وformat للملفات التنفيذية والمكتبات، وأن Android يستخدم ELF. ABI المعروفة حالياً في NDK تشمل armeabi-v7a وarm64-v8a وx86 وx86_64. مدير الحزم يختار أكثر machine code ملاءمة من المعماريات التي يدعمها الجهاز، وقد يعرض الجهاز primary وsecondary ABIs.

المسار المعتاد للمكتبات داخل APK:
- lib/armeabi-v7a/libname.so
- lib/arm64-v8a/libname.so
- lib/x86/libname.so
- lib/x86_64/libname.so

لا نستنتج أن مجلد lib/arm64-v8a يحتوي ملف AArch64 صحيحاً قبل فحص ELF. ولا نفترض أن غياب مجلد ABI يعني فشل التطبيق كله؛ قد لا يحتوي التطبيق على Native code أصلاً، أو قد ينزل split آخر أو يستخدم طريقة تحميل مختلفة.

#### 20.35.2 جمع ABI من الجهاز عبر ADB

لـ read-only inventory:
- adb -s <serial> shell getprop ro.product.cpu.abilist
- adb -s <serial> shell getprop ro.product.cpu.abi
- adb -s <serial> shell getprop ro.product.cpu.abilist32
- adb -s <serial> shell getprop ro.product.cpu.abilist64

يجب اعتبار ro.product.cpu.abilist قائمة القدرات التي يعرضها build للجهاز، مع حفظ القيم الخام وترتيبها. الخصائص المفردة أو القديمة قد لا تكون متاحة أو قد تختلف حسب Android/OEM؛ لا نستخدم قيمة واحدة كحقيقة كاملة.

لا نضيف قواعد تخمينية مثل: «الجهاز ARM64 إذن يقبل كل ARM32». نعتمد على قائمة ABI التي يعلنها Android، ونترك Package Manager صاحب القرار النهائي. لا نستنتج دعم ترجمة ARM على x86 إلا إذا ظهر في قائمة ABI التي يعرضها النظام أو في دليل موثوق خاص بالمنصة.

#### 20.35.3 مرشح Rust الأول: m4b/goblin

المستودع: [m4b/goblin](https://github.com/m4b/goblin) — آخر commit تمت مراجعته: cc6beec889d74680d8f77626f508cb9d949dfc23 بتاريخ 2026-10-26.

ملفات تمت مراجعتها:
- README.md — blob SHA 4205fa1107189daa62dc81b00fcbfd5f8561bcdd.
- Cargo.toml — blob SHA 9088bb5ce233b4ca95ea8443952400c4ef94304e.
- src/elf/header.rs — blob SHA 2c05e8ffc6fc98f117c427a27c1737bb521b5eea.
- src/elf/dynamic.rs — blob SHA 67e960b5df3d8a7d9d58811016aaed59303e3a02.
- fuzz/fuzz_targets/parse_elf.rs — blob SHA fc7228d2991893a01f303898112d3675991b5708.

ما يقدمه:
- ELF32 وELF64، parsing endian-aware، وواجهة موحدة Elf::parse.
- Header exposes e_machine وe_ident ومنها class/endianness، وحقول ELF الأخرى.
- constants للمعماريات، ويمكن بناء mapping بين EM_ARM وEM_AARCH64 وEM_386 وEM_X86_64 وغيرها وبين ABI المعروفة.
- Dynamic table يتضمن DT_NEEDED وDT_SONAME وغيرهما، وهو مفيد لإظهار shared-library dependencies كبيانات تشخيصية.
- README يذكر fuzzing targets لـELF، وملف parse_elf.rs يمر على section headers وdynamic relocations وnote iterators.
- commit المراجع يتضمن تغييراً حديثاً يتحقق من حدود PT_DYNAMIC قبل التخصيص في dynamic::from_fd، وهو مؤشر مفيد على صيانة parser لهذه الفئة من المدخلات.

**تحفظات وحدود:**
- وجود fuzz target أو إصلاح bounds لا يثبت خلو المكتبة من كل parser bugs؛ نحتاج fuzzing خاصاً بنا على corpus من APK native libraries.
- لا نسمح بتحميل أو تنفيذ .so؛ نقرأ metadata فقط.
- لا نقرأ كل sections/symbols/dependencies إلا عند طلب التفاصيل، لتقليل تكلفة تحليل ملفات كثيرة.
- لا نعطي ثقة في ELF لمجرد نجاح parse؛ نتحقق من magic/class/endianness/e_machine/type وbounds وfile size، ونتعامل مع malformed أو unsupported بوضوح.
- DT_NEEDED لا يثبت أن المكتبات التابعة موجودة على الجهاز أو متوافقة معه؛ هو مؤشر تشخيصي فقط.

**القرار:** goblin هو المرشح الأول لمكتبة ELF داخل Rust PoC. لا يوجد اعتماد production أو تشغيل Windows حتى الآن.

#### 20.35.4 مرشح بديل: gimli-rs/object

المستودع: [gimli-rs/object](https://github.com/gimli-rs/object) — default branch source تمت مراجعته عند commit 119f48ace38d1cab8e5653489acde1452da8384b.

الملفات:
- README.md — blob SHA 2054d608916a1291c782a5fe6bc5f0ee8f2571c6.
- src/read/elf/file.rs — blob SHA aa75f22bac75af093602e74a151a60bc87409414.
- Cargo.toml — blob SHA 610646eecc37fde9ebcc4b4c52410975d7354afc.

المكتبة توفر unified read API لملفات ELF وPE وMach-O وغيرها، وتعرض architecture() لتحديد المعمارية. لكن README يصرح أن المكتبة موجهة إلى trusted inputs، وأنها لا تركز على تخفيف DoS الناتج عن هياكل متداخلة أو متداخلة بشكل مكثف.

**القرار:** ليست الخيار الأول لتحليل APKs غير موثوقة. يمكن إعادة تقييمها لأدوات developer/trusted artifact inspection أو كمرجع API، لكن لا ندخلها في مسار فحص ملفات المستخدم قبل إثبات حدود الموارد.

#### 20.35.5 mapping مبدئي من ELF إلى Android ABI

نستخدم e_machine + ELF class، مع فحوص إضافية حسب ABI:

| Android ABI | ELF class | e_machine المتوقع |
|---|---:|---:|
| armeabi-v7a | ELF32 | EM_ARM (40) |
| arm64-v8a | ELF64 | EM_AARCH64 (183) |
| x86 | ELF32 | EM_386 (3) |
| x86_64 | ELF64 | EM_X86_64 (62) |

هذا mapping للتحقق من المعمارية الأساسية وليس تحققاً كاملاً من ABI. في ARM نحتاج لاحقاً فحص e_flags وخصائص ABI ذات الصلة عند الحاجة، لأن تطابق EM_ARM وحده لا يثبت كل خصائص تعليمات/Calling convention المطلوبة. Android little-endian؛ الملف الذي يعلن big-endian لا ينبغي اعتباره Android ABI صالحاً لهذه الحالات القياسية.

#### 20.35.6 تحليل التوافق دون ادعاء ضمان التثبيت

لكل APK أو split:
1. اكتشاف entries lib/<abi>/*.so دون استخراج عام للأرشيف.
2. تمرير entry إلى ZIP streaming reader محدود وآمن.
3. تحليل ELF header بواسطة Goblin.
4. مقارنة اسم ABI directory مع e_machine وELF class وendianness.
5. تسجيل mismatch كتحذير/خطأ حسب سياسة التحقق، وعدم إعادة تسمية الملف أو إصلاحه تلقائياً.
6. تجميع ABI المتوفرة فعلياً في كل APK/split.
7. مقارنة ABI package مع ro.product.cpu.abilist للجهاز المحدد.
8. إظهار أفضل ABI match وفق ترتيب الجهاز، مع ذكر إن كانت النتيجة مستندة إلى ABI list أو إلى مجرد تطابق معماري.
9. إذا كانت الحزمة split set، نستخدم DeviceSpec وbundletool لتحديد splits المناسبة؛ لا نعامل كل split على أنه APK مستقل مكتمل.
10. النتيجة النهائية: Compatible / No matching ABI / No native libraries detected / Invalid ELF / ABI-folder mismatch / Incomplete split set / Unknown. ونوضح أن Android Package Manager هو الاختبار النهائي.

لا نستخدم adb install --abi كخيار تلقائي. يمكن عرضه في أدوات التشخيص المتقدمة فقط، بعد أن يختار المستخدم ABI مدعومة فعلياً من الجهاز، مع Preview وموافقة منفصلة.

#### 20.35.7 نموذج البيانات المقترح

- DeviceAbiProfile: serial, raw_abilist, primary_abi, abi32_list, abi64_list, captured_at, source.
- NativeLibraryEntry: apk_id, split_name, archive_path, declared_abi_dir, file_size, sha256.
- ElfMetadata: class, endianness, e_type, e_machine, machine_name, soname, needed_libraries, parse_status.
- AbiValidation: folder_abi, elf_abi, folder_matches_elf, device_supports_abi, selected_candidate, warnings.
- ApkCompatibilityReport: package_id, version, apk_set_id, native_abi_candidates, device_abi_profile_id, outcome, evidence, limitations.

يجب أن تكون الحقول Unknown أو NotPresent عند غيابها، لا نستبدلها باستنتاجات. ولا نحتفظ بملفات .so بعد انتهاء الفحص إلا إذا طلب المستخدم تصديرها صراحة.

#### 20.35.8 خطة PoC والاختبارات

- Fixtures ELF32 ARM, ELF64 AArch64, ELF32 x86, ELF64 x86_64.
- ملف ELF صحيح في مجلد ABI خاطئ.
- ELF header truncated، bad magic، unknown class، unknown endianness، unknown machine.
- big-endian ELF، wrong ELF type، oversized program/section table offsets/counts.
- native libraries مفقودة أو موجودة في split منفصل.
- APK بدون مجلد lib.
- APK يحتوي عدة ABIs، وAPK يحتوي ABI واحدة فقط.
- ملفات .so غير ELF رغم امتدادها.
- archive entry كبيرة أو compressed bomb؛ يجب أن تطبق حدود ZIP السابقة قبل تسليم bytes إلى Goblin.
- اختلاف بين device ABI list وملفات الحزمة.
- اختبارات Windows CI، fuzzing لـELF headers/program headers/dynamic table، قياس وقت وذاكرة تحليل دفعات APK.

**حالة الجولة:** [x] مراجعة Android NDK ABI documentation. [x] مراجعة Goblin README/Cargo/ELF header/dynamic parser/fuzz target. [x] مراجعة object crate API والتحذير الأمني. [ ] PoC Rust. [ ] Windows build/test. [ ] ELF fuzzing خاص بالمشروع. [ ] APK corpus cross-check. [ ] device ABI capture على أجهزة فعلية. [ ] اعتماد production.

**الخلاصة:** نوصي بـgoblin كأول مرشح ELF parser، مع ZIP streaming reader الآمن الموجود في قرار 20.33، وجمع ABI من الجهاز عبر ADB read-only. object ليس الخيار الأول لملفات APK غير موثوقة بسبب تحذير upstream من DoS. لا نكتب ELF parser جديداً.

المراجع:
- [Android NDK ABI guide](https://developer.android.com/ndk/guides/abis)
- [Goblin repository at reviewed commit](https://github.com/m4b/goblin/tree/cc6beec889d74680d8f77626f508cb9d949dfc23)
- [Goblin ELF parser source](https://github.com/m4b/goblin/blob/cc6beec889d74680d8f77626f508cb9d949dfc23/src/elf/header.rs)
- [Goblin ELF fuzz target](https://github.com/m4b/goblin/blob/cc6beec889d74680d8f77626f508cb9d949dfc23/fuzz/fuzz_targets/parse_elf.rs)
- [object repository](https://github.com/gimli-rs/object)
- [Android ELF machine identifiers](https://chromium.googlesource.com/android_ndk/+/refs/heads/unmodified/sysroot/usr/include/linux/elf-em.h)


### 20.36 فحص اعتماديات Native Libraries وDT_NEEDED وSONAME (12 نوفمبر 2026)

**نوع المراجعة:** مراجعة وثائق Android الرسمية ومصدر Bionic linker، ومراجعة source/API لأدوات ELF الجاهزة. لا يوجد تشغيل للأدوات أو تحليل APK فعلي في هذه الجولة.

#### 20.36.1 المشكلة التي نريد كشفها

فحص ABI وحده لا يكفي. قد يحتوي APK على ملفات ELF صحيحة للمعمارية المطلوبة، لكن هذه الملفات تعتمد على مكتبات مشتركة أخرى لا يمكن تحميلها في بيئة التطبيق، أو تسجل أسماء/مسارات اعتماديات غير صحيحة.

نحتاج استخراج العناصر التالية لكل ملف .so:
- `DT_NEEDED`: أسماء المكتبات المطلوبة مباشرة.
- `DT_SONAME`: الاسم الذي يعرّف المكتبة نفسها عند الربط الديناميكي.
- `DT_RPATH` و`DT_RUNPATH` عند وجودها، كبيانات تشخيصية لا كمسارات نثق بها.
- Dynamic symbols المستوردة/المصدرة، وsymbol version requirements عند الحاجة للتحقيق في اختلاف API أو الرموز.
- ELF machine/class/endianness وABI directory من القسم 20.35.

لا نخلط بين dependency المباشرة وبين شجرة الاعتماديات الكاملة؛ المكتبة المطلوبة قد تحتاج بدورها مكتبات أخرى.

#### 20.36.2 الفرق بين وجود DT_NEEDED وإمكانية التحميل الفعلية

وجود اسم في `DT_NEEDED` لا يحدد وحده أين سيجد Android المكتبة، ولا يثبت أنها متاحة في سياق التطبيق.

Android Bionic يستخدم linker namespaces ذات search paths وpermitted paths وروابط بين namespaces، وقد تكون namespace معزولة. لذلك لا يصح أن نقارن كل `DT_NEEDED` بقائمة ملفات موجودة في APK فقط، أو بقائمة مكتبات Windows/Linux على جهاز الفحص. يجب مراعاة:
- مكتبات التطبيق الموجودة ضمن APK/split set وبـABI المطابقة.
- مكتبات Android العامة المتاحة للإصدار المستهدف والجهاز/صورة النظام.
- قيود namespace ومسارات البحث والـ public libraries.
- اختلافات OEM/API level، وأي مكتبات خاصة أو غير عامة لا يمكن افتراض إتاحتها للتطبيق.

توضح وثائق Bionic أن المكتبات المطلوبة قد تفشل في التحميل إذا كانت خارج search/permitted paths ولم تكن متاحة عبر namespace مرتبطة. كما أن مسارات البحث وترتيبها جزء من إعداد namespace وليس استنتاجاً من اسم dependency وحده.

**نتيجة مهمة:** لا نصدر حكم `Missing dependency = install failure`. قد ينجح تثبيت APK بينما يظهر الخطأ فقط عند تحميل native library أو استدعاء المسار البرمجي الذي يحتاجها. تقريرنا يصف المخاطر المحتملة ولا يحاكي linker runtime.

#### 20.36.3 DT_NEEDED وSONAME ومسارات البناء

وثائق Android linker changes توثق حالات خاطئة يضع فيها toolchain مساراً مطلقاً من جهاز البناء داخل `DT_NEEDED`. وتشير إلى أن سلوك التعامل مع الاسم/المسار تغير ابتداءً من API 23؛ قد تفشل المسارات المطلقة القادمة من Windows build host بدلاً من أن يعاملها linker كاسم مكتبة فقط.

كما توثق الوثائق أهمية وجود SONAME صحيح للمكتبات المشتركة، وأن غيابه قد يؤدي لاستخدام اسم الملف كبديل، مع احتمال تعارضات namespace أو تحميل مكتبة غير المقصودة.

قواعد الفحص المقترحة:
1. إذا احتوى `DT_NEEDED` على مسار مطلق أو drive letter أو مسار build host، ارفع تحذيراً عالي الأهمية.
2. قارن `DT_NEEDED` بالأسماء الفعلية وSONAMEs للمكتبات الموجودة داخل نفس APK/split set، وليس بأسماء الملفات فقط.
3. اكشف duplicate SONAME داخل ABI نفسها، وسجل كل مسار ومصدر؛ لا تختَر نسخة تلقائياً.
4. إذا غاب SONAME من shared object، أظهر Missing SONAME كتحذير يحتاج تفسيراً وفق API level وسياق التحميل.
5. لا تعدّل ELF أو تعيد كتابة dynamic entries تلقائياً.
6. لا تعتبر وجود SONAME مطابق دليلاً على تطابق ABI أو الرموز أو سلامة المكتبة.

#### 20.36.4 الأدوات الجاهزة — Android NDK llvm-readelf

**المرشح الأول كأداة cross-check على Windows:** `llvm-readelf.exe` المرفق مع Android NDK.

وثائق Android Developers تعرض مسار Windows صريحاً للأداة داخل:
`<SDK>/ndk/<version>/toolchains/llvm/prebuilt/windows-x86_64/bin/llvm-readelf.exe`

هذا يؤكد وجود binary Windows ضمن NDK، ويجعل الأداة مناسبة كمرجع مستقل للتحقق من نتائج parser الداخلي. أوامر فحص مقترحة في PoC:
- `llvm-readelf.exe -d <file.so>` لعرض dynamic section، بما فيها NEEDED/SONAME/RUNPATH إن وجدت.
- `llvm-readelf.exe --dyn-syms <file.so>` لمراجعة dynamic symbols.
- `llvm-readelf.exe --version-info <file.so>` لمراجعة symbol version information.
- `llvm-readelf.exe -h <file.so>` لمقارنة ELF header وmachine/class.
- `llvm-readelf.exe -l <file.so>` عند الحاجة لمراجعة program headers.

هذه أوامر مقترحة للتحقق وليست نتيجة تشغيل في هذه الجولة. يجب تثبيت NDK version واستخدام Process runner بوسائط منفصلة وtimeout/cancel، وعدم بناء command shell string من اسم الملف.

**القرار:** نستخدم NDK llvm-readelf كـgolden cross-check في PoC وضمن أدوات التشخيص المتقدمة، وليس بالضرورة كاعتماد runtime دائم إذا أثبت Goblin كفايته.

#### 20.36.5 Goblin — استمرار المرشح الداخلي

المستودع: [m4b/goblin](https://github.com/m4b/goblin) — commit المراجع: `cc6beec889d74680d8f77626f508cb9d949dfc23`.

مراجعة `src/elf/dynamic.rs` تؤكد تعريف dynamic tags ومنها:
- `DT_NEEDED`
- `DT_SONAME`
- `DT_RPATH`
- `DT_RUNPATH`
- `DT_STRTAB` و`DT_STRSZ`
- `DT_VERNEED` و`DT_VERNEEDNUM`
- `DT_VERSYM` و`DT_VERDEF` و`DT_VERDEFNUM`

آخر commit تمت مراجعته يعالج التحقق من حدود `PT_DYNAMIC` قبل التخصيص في `dynamic::from_fd)، ويضيف اختباراً بملف fixture. هذا تحسين محدد مهم لكنه لا يغني عن اختباراتنا للـ string table offsets والـ dynamic entry counts والـ malformed ELF.

**القرار:** نحتفظ بـGoblin كمرشح Rust أول. يجب أن يثبت PoC استخراج أسماء NEEDED وSONAME والـ symbol-version metadata بدقة، ومقارنتها بنتائج llvm-readelf. لا نعلن أن التكامل مكتمل قبل ذلك.

#### 20.36.6 LIEF — بديل أوسع للفحص والتحليل

المستودع: [lief-project/LIEF](https://github.com/lief-project/LIEF) — آخر commit تمت مراجعته: `a3c9ca3a36a3b1ad6d099c8e3318175c175918fb` بتاريخ 8 نوفمبر 2026.

مراجعة Python ELF reader example توضح عرض dynamic entries، وبشكل صريح `SONAME` و`NEEDED` و`RUNPATH` و`RPATH`، إضافة إلى dynamic symbols وimported symbols وsymbol versions. ويوفر LIEF طبقة تحليل أوسع من مجرد قراءة ELF header.

لكن إدخاله يضيف مكوّناً أكبر من حاجة V1، ويجب تقييم حزم Windows وطريقة تضمينه وحجم الاعتماديات وسلوك parser مع ملفات APK غير موثوقة قبل اعتماده. وجود مثال API لا يثبت سلامة parser لكل مدخلات مشوهة.

**القرار:** LIEF بديل/أداة cross-check اختيارية إذا احتجنا تحليلاً أوسع أو API جاهزة، وليس اعتماداً موازياً افتراضياً إلى جانب Goblin. لا نحتاج الاثنين في مسار الإنتاج دون سبب مقاس.

#### 20.36.7 بناء Dependency Graph داخل APK

بعد قراءة جميع ملفات .so في ABI واحدة:
1. إنشاء عقدة لكل ملف native library.
2. تسجيل اسم archive path وABI directory وELF machine وSONAME.
3. ربط كل `DT_NEEDED` بالعقدة الداخلية التي تطابق SONAME، إن وجدت.
4. إذا لم توجد عقدة داخل الحزمة، تصنيف dependency كـExternal/Unresolved؛ لا نصفها بأنها Missing نهائياً قبل فحص Android runtime catalog.
5. كشف cycles، duplicate SONAME، self-dependency، absolute path، وdependency name غير المعتاد.
6. تنفيذ التحليل منفصلاً لكل ABI ولكل split set؛ لا نخلط مكتبة arm64 مع x86.
7. عدم دمج نتائج ملفات APK مختلفة أو نسخ قديمة من التطبيق دون تحديد APK set/version.

نموذج بيانات مبدئي:
- `NativeDependency`: source_library_id, needed_name, resolution_kind, resolved_library_id, evidence, confidence.
- `NativeLibraryMetadata`: archive_path, abi, soname, needed[], runpath[], imported_symbols[], version_requirements[], parse_status.
- `NativeDependencyGraph`: package_id, version_code, abi, libraries[], edges[], unresolved_external[], duplicate_sonames[], warnings[].
- `AndroidRuntimeLibraryProfile`: device/build fingerprint, Android API, ABI, public library evidence, source, captured_at.

لا نستخدم أسماء ملفات النظام من جهاز Windows host كمرجع. أي Runtime profile يجب أن يأتي من Android system image أو جهاز Android محدد، مع تسجيل build/API ومصدر البيانات، ودون افتراض أن جهازاً واحداً يمثل كل أجهزة Android.

#### 20.36.8 فحص توافق الرموز وAPI level

حتى لو حُلّت أسماء `DT_NEEDED`، قد تفشل المكتبة بسبب symbol غير متاح أو symbol version غير متوافق. لذلك:
- نقرأ imported dynamic symbols وsymbol version requirements عند الحاجة.
- نربطها ببيانات Android API level وNDK/public API evidence موثوقة.
- نميز بين API level الذي يستهدفه APK وبين API level الذي بنيت عليه المكتبة إذا توفر دليل موثوق؛ لا نستنتج أحدهما من الآخر.
- نعرض نتيجة Symbol/API analysis كـPotential mismatch أو Unknown إذا لم تتوفر قاعدة مرجعية دقيقة.
- لا ندّعي محاكاة linker أو نجاح التحميل دون تجربة runtime على بيئة Android مطابقة.

#### 20.36.9 الاختبارات المطلوبة قبل الاعتماد

- ملفات ELF سليمة بها dependency واحدة ومتعددة.
- NEEDED يطابق SONAME داخل نفس APK، ويطابق اسم الملف فقط مع SONAME مختلف.
- غياب SONAME، وduplicate SONAME داخل ABI واحدة.
- NEEDED بمسار مطلق أو Windows drive path أو separators غير متوقعة.
- مكتبة تعتمد على مكتبة داخل split آخر.
- اعتماد خارجي موجود في Android public library profile، وآخر غير موجود في العينة.
- symbol imports وversion requirements مع اختلافات API.
- Dynamic string table offset خارج الملف أو strings غير منتهية بـNUL.
- dynamic table بلا DT_NULL أو بعدد entries مبالغ فيه.
- corrupted ELF وtruncated file وoversized metadata.
- مقارنة Goblin مع llvm-readelf؛ وLIEF عند استخدامه كـcross-check.
- fuzzing للـdynamic table/string table/version tables، وWindows CI، وقياس الأداء والذاكرة.
- عدم تحميل أو تنفيذ أي .so في الاختبارات؛ التحليل static فقط.

#### 20.36.10 القرار الحالي وحالة التحقق

1. **Goblin:** المرشح الداخلي الأول، مع PoC مطلوب لاستخراج NEEDED/SONAME/symbol versions.
2. **Android NDK llvm-readelf:** أداة Windows جاهزة للتحقق المستقل ومقارنة النتائج.
3. **LIEF:** خيار أوسع احتياطي/تحليلي، لا يضاف افتراضياً قبل قياس الحاجة.
4. **Bionic linker docs/source:** المرجع لتفسير namespace والبحث والإتاحة؛ لا نستنسخ linker ولا نحاكيه بالكامل.
5. **Runtime compatibility:** لا يمكن الجزم به من ELF metadata وحدها؛ يتطلب Android runtime evidence أو اختباراً فعلياً على build مطابق.

**حالة الجولة:** [x] مراجعة Bionic linker changes حول NEEDED/SONAME وAPI 23. [x] مراجعة linker namespace configuration/source. [x] مراجعة Goblin dynamic tags وcommit bounds fix. [x] مراجعة LIEF dynamic-entry reader example. [x] التحقق من وجود llvm-readelf.exe ضمن مسار NDK Windows في وثائق Android. [ ] تشغيل llvm-readelf على Windows. [ ] PoC Goblin dynamic dependency extraction. [ ] مقارنة Goblin وllvm-readelf وLIEF. [ ] بناء Android runtime library catalog موثق. [ ] symbol/API compatibility tests. [ ] fuzzing وWindows CI. [ ] اعتماد production.

المراجع:
- [Android linker changes for NDK developers](https://android.googlesource.com/platform/bionic/+/master/android-changes-for-ndk-developers.md)
- [Bionic linker namespace configuration format](https://android.googlesource.com/platform/bionic/+/master/linker/ld.config.format.md)
- [Bionic linker namespace implementation](https://android.googlesource.com/platform/bionic/+/master/linker/linker_namespaces.h)
- [Android Developers — 16 KB page size verification (Windows llvm-readelf path)](https://developer.android.com/guide/practices/page-sizes)
- [Goblin dynamic ELF tags at reviewed commit](https://github.com/m4b/goblin/blob/cc6beec889d74680d8f77626f508cb9d949dfc23/src/elf/dynamic.rs)
- [Goblin reviewed bounds-validation commit](https://github.com/m4b/goblin/commit/cc6beec889d74680d8f77626f508cb9d949dfc23)
- [LIEF ELF reader example](https://github.com/lief-project/LIEF/blob/main/api/python/examples/elf_reader.py)
- [LIEF reviewed latest commit](https://github.com/lief-project/LIEF/commit/a3c9ca3a36a3b1ad6d099c8e3318175c175918fb)

### 20.37 إعادة ضبط الأولوية: التثبيت واسع التغطية هو هدف V1 (12 نوفمبر 2026)

**قرار معتمد من مالك المنتج:** الأولوية القصوى هي تمكين المستخدم من تثبيت أكبر عدد عملي من صيغ وحالات حزم Android الشائعة على جهاز محدد. لا نواصل التوسع في تحليل ELF وDT_NEEDED وSONAME وsymbol/API compatibility كشرط أساسي لمسار التثبيت.

#### 20.37.1 ما الذي نعتبره نجاحاً؟

لا يكفي أن يتعرف البرنامج على امتداد الملف أو أن يستخرج ملفات APK. النجاح يمر بالمراحل التالية:

1. التعرف على الصيغة الفعلية من المحتوى، وليس الامتداد وحده.
2. قراءة بيانات الحزمة وملفاتها الداخلية دون تنفيذ محتوى الأرشيف.
3. التأكد من أن مجموعة APK تنتمي إلى package/version متسقين، وأن base والـsplits المطلوبة موجودة.
4. تحديد مواصفات الجهاز المستهدف: Serial صريح، Android API، ABI list، density، locales، وحالة الاتصال.
5. اختيار الملفات التي تطابق الجهاز، أو بيان سبب عدم القدرة على الاختيار.
6. عرض ملخص قبل التنفيذ: التطبيق المتوقع، الإصدار، عدد الملفات، OBB إن وجد، وأي تحذيرات.
7. تنفيذ التثبيت بأداة Android المناسبة، مع تقدم وإلغاء حيث تسمح العملية.
8. قراءة النتيجة النهائية والتحقق من ظهور الحزمة والإصدار على الجهاز.
9. فصل نجاح تثبيت الحزمة عن نجاح تشغيل التطبيق؛ لا نعد بإمكانية التشغيل لمجرد نجاح التثبيت.

#### 20.37.2 ترتيب صيغ V1

| الأولوية | المدخل | المعالجة المطلوبة | المحرك المرجعي | معيار القبول |
|---|---|---|---|---|
| P0 | APK مفرد | فحص manifest والتوقيع والتوافق الأساسي ثم التثبيت | Android Platform-Tools adb install | تثبيت/تحديث حزمة واحدة على Serial محدد والتحقق من package/version |
| P0 | ملفات APK منفصلة تمثل base + splits | تجميع وفحص الاتساق واختيار splits ثم التثبيت كمجموعة واحدة | adb install-multiple أو PackageInstaller sessions | رفض المجموعة الناقصة أو المختلطة؛ نجاح المجموعة المتسقة |
| P0 | APKS (Bundletool APK Set) | قراءة TOC/device targeting واختيار مجموعة الجهاز | Google bundletool install-apks أو extract ثم install-multiple | اختيار splits المطابقة للجهاز وعدم تثبيت standalone مع splits في نفس المجموعة |
| P1 | APKM غير المشفر | قراءة metadata/manifest، واستخراج APKs بأسماء staging مولدة، ثم التحقق والتجميع | Adapter مبني على format parser مدقق؛ SAI مرجع تنفيذي | دعم عينات APKM معروفة، ورفض encrypted/unknown variants بوضوح |
| P1 | XAPK | قراءة manifest.json عند وجوده، مطابقة package/version مع APK manifests، تصنيف APKs وOBB | Adapter خاص بـXAPK؛ parser مرجعي من SAI/ApkInstaller | تثبيت APK set المتوافق، ومعالجة OBB منفصلة بموافقة وتحقق |
| P1 | APKs متعددة مختارة من المستخدم | تجميعها في package set واحد فقط بعد فحص package/version/split identity | نفس split pipeline | لا يتم تمرير ملفات من تطبيقات مختلفة إلى install-multiple |
| P2 | AAB | تحويل bundle إلى APK Set عبر bundletool مع إعدادات توقيع واضحة | Google bundletool | لا يُعرض كملف APK جاهز؛ يطلب signing config عند الحاجة ويشرح أثر إعادة التوقيع |
| P2 | صيغ/امتدادات أخرى أو حاويات معدلة | كشف النوع وبيان عدم الدعم أو طلب عينة اختبار | Adapter منفصل فقط بعد إثبات الحاجة | لا يوجد fallback أعمى يفك كل شيء ويثبت كل APK داخله |

هذه الترتيبات تصف أولوية التطوير، ولا تعني أن كل الصيغ تم اختبارها فعلياً.

#### 20.37.3 مصفوفة الحالات التي يجب تغطيتها

- APK عادي صالح، APK تالف، وملف غير APK أُعيدت تسميته إلى .apk.
- APK جديد، تحديث لنفس package، إصدار أقدم، وتوقيع مختلف عن النسخة المثبتة.
- Split set مكتمل، split مفقود، duplicate split، base مفقود، package mismatch، version mismatch.
- ABI واحد، عدة ABI، ABI غير مدعوم، تطبيق بلا native libraries.
- density وlocale متطابقان، بديل مناسب، split غير متاح، split عام، وfeature split.
- APKS يحتوي toc.pb وsplits/ وstandalones/ وasset slices؛ يجب اتباع metadata/targeting ولا نخلط standalone مع split install set.
- APKM غير مشفر، APKM مشفر أو variant غير معروف، metadata ناقصة أو متناقضة.
- XAPK مع APK فقط، APK splits، OBB واحد أو أكثر، manifest مفقود، package mismatch، OBB path غير متوقع.
- جهاز واحد، عدة أجهزة، unauthorized/offline، انقطاع USB/Wi-Fi أثناء النقل، مساحة غير كافية.
- إلغاء قبل التثبيت، إلغاء أثناء التحضير، وفشل أثناء التثبيت؛ يجب توضيح ما إذا كانت العملية قد وصلت إلى مرحلة لا يمكن إيقافها بأمان.
- تثبيت عدة تطبيقات مستقلة: لا نستخدم install-multiple لها؛ يمكن تقييم install-multi-package كعملية ذرية مستقلة عند الحاجة.

#### 20.37.4 قواعد ADB التي نعتمدها

توثيق Android الرسمي يفرق بين:
- adb install: ملف APK مفرد.
- adb install-multiple: ملفات APK متعددة لحزمة واحدة (base + splits).
- adb install-multi-package: أكثر من package في عملية ذرية واحدة، وليس بديلاً عن split install.

خيارات يجب ألا تُفعّل تلقائياً:
- -t: السماح بملفات test-only، لا يُستخدم إلا بطلب صريح.
- -d: downgrade، ولا يمرر افتراضياً؛ يخضع لقيود Android.
- -g: منح كل runtime permissions، لا يُستخدم افتراضياً.
- -p: partial install، ليس مسار التثبيت الكامل المعتاد.
- -i: يحدد installer attribution؛ لا نستخدمه لانتحال متجر أو مصدر تثبيت آخر.

**القاعدة:** أقل مجموعة خيارات لازمة، مع عرض أي خيار استثنائي للمستخدم قبل التنفيذ. لا نستخدم أمر shell عام عندما يتوفر ADB command typed ومحدد.

#### 20.37.5 ما نؤجله من ELF

- فحص DT_NEEDED/SONAME وsymbol versions وdependency graph ليس شرطاً لتثبيت APK.
- نحتفظ بفحص ABI الأساسي وminSdk والـmanifest والـsplit targeting لأنها تؤثر مباشرة في اختيار الحزمة أو توقع فشل التثبيت.
- فحص ELF المتقدم يبقى أداة تشخيص اختيارية بعد V1، عندما نحتاج تفسير تطبيق ثبت بنجاح لكنه يفشل عند تحميل مكتبة native.
- لا نحذف البحث السابق؛ نعيد تصنيفه من blocker إلى advanced diagnostics.

**المراجع الرسمية:** [ADB install commands](https://developer.android.com/tools/adb) — [ADB command-line reference](https://android.googlesource.com/platform/packages/modules/adb/+/refs/heads/main/docs/user/adb.1.md) — [bundletool install-apks and device targeting](https://developer.android.com/tools/bundletool).

### 20.38 تدقيق مرشحي Windows الجاهزين للتثبيت واسع الصيغ

#### 20.38.1 havokentity/android-application-installer — أقرب مرشح معماري لواجهة Windows

المستودع: [havokentity/android-application-installer](https://github.com/havokentity/android-application-installer)  
الملفات التي تمت مراجعتها: README، docs/architecture.md، src-tauri/src/adb.rs، src/api.ts، ومقتطفات أوامر Tauri.  
Commit source الذي ظهر في روابط مراجعة الكود: 11acb70c5a8886a1f0e985217a635321d681bc49.

**ما يثبته README/المصدر:**
- Desktop app بـTauri 2 + Rust + React/TypeScript.
- حزم Windows x64 معلنة، وGitHub Actions لبناء الإصدارات.
- يدير ADB وbundletool وJava محلياً، ويدعم APK install وAAB build/install.
- يملك typed IPC بين الواجهة وRust، progress events، cancellation tokens، اختيار الجهاز، وتتبّع اتصال Wireless ADB.
- الكود يتضمن مساراً خاصاً لـWindows لنقل APK ثم pm install يذكر المشروع أنه workaround لمشكلة EOF، ومسار ADB install للأنظمة الأخرى.
- يوجد فصل بين ADB commands وcommand runner وpackage metadata وtool management.

**ما لا يثبته المصدر المفحوص:**
- لا يعلن دعم APKS كملف إدخال مباشر من الواجهة، ولا APKM/XAPK/OBB.
- لا يوجد دليل في الملفات المفحوصة على parser شامل لملفات APK containers.
- README يثبت ادعاء ميزات المطور، وليس نجاحها على أجهزة متعددة في بيئتنا.

**نقاط مراجعة قبل إعادة الاستخدام:**
1. نعيد استخدام أنماط typed service/IPC وprogress/cancel/device selection بعد تدقيقها، لا ننسخ التطبيق كاملاً.
2. مسار Windows اليدوي adb push ثم pm install يحتاج اختباراً مستقلاً قبل اعتماده؛ لا نعممه على split/APKS.
3. نتحقق من نتيجة التثبيت فعلياً، لأن نجاح النقل أو عدم ظهور كلمة Failure وحدهما ليسا أفضل معيار وحيد.
4. لا نسمح بمسار يسمح بتجاوز سياسة downgrade أو test-only افتراضياً.
5. نراجع إدارة الملفات المؤقتة والتزامن عند تثبيت ملفين لهما الاسم نفسه.

**التصنيف:** مرشح قوي لإعادة استخدام بنية Windows/ADB وإدارة الأدوات، وليس محركاً جاهزاً لتغطية APKM/XAPK.

#### 20.38.2 zechuan-start/adb-gui — مرجع إدارة الجهاز والتطبيقات

المستودع: [zechuan-start/adb-gui](https://github.com/zechuan-start/adb-gui).

README يعلن Tauri 2 + React، Windows x64، ADB مضمّن، USB/Wi-Fi، قائمة تطبيقات تعرض الإصدار وتاريخ التثبيت والحجم، وعمليات lifecycle، بالإضافة إلى أدوات ADB أخرى. لم يظهر في README دليل على APKS/APKM/XAPK أو OBB أو parser split متكامل.

**التصنيف:** مرجع قوي لواجهة ADB العامة واكتشاف الجهاز وإدارة التطبيقات، وليس محرك تثبيت متعدد الصيغ. لا نكرر هذه الوظائف إذا استطعنا إعادة استخدام خدمة أو نمط مناسب بعد مراجعة المصدر.

#### 20.38.3 chihafuyu/App-Bundle-Installer — سكربت Windows يغطي صيغاً متعددة لكنه غير صالح للدمج كما هو

المستودع: [chihafuyu/App-Bundle-Installer](https://github.com/chihafuyu/App-Bundle-Installer).  
الملف المفحوص: App-Bundle-Installer.ps1.

**ما يعلنه وينفذه المصدر:**
- PowerShell على Windows، يبحث عن .apkm و.apks و.xapk و.zip و.apk+.
- يفك الأرشيف، يجمع كل ملفات APK التي يعثر عليها، ثم يمررها إلى adb install-multiple.
- يحاول نقل مجلد Android/obb إلى /sdcard/Android/obb/.
- يتضمن حدوداً أولية لعدد entries والحجم الإجمالي المعلن، واختيار Serial عند تعدد الأجهزة.

**ملاحظات حرجة من مراجعة الكود:**
1. يطلب اختيار Fake Installer Source ويضيف adb install-multiple -i <installerSource>؛ هذا انتحال لبيانات مصدر التثبيت ولا يدخل في تصميمنا.
2. وصف README بأن ذلك يتجاوز APK signature verification غير مثبت من الكود المعروض؛ خيار -i يحدد installer attribution، وليس دليلاً على تعطيل تحقق توقيع APK. لا نعتمد هذا الادعاء.
3. يجمع جميع ملفات APK داخل الأرشيف دون بناء package/split set موثوق أو التأكد من تطابق package/version، ما قد يخلط ملفات غير متجانسة.
4. وجود base.apk شرط وحيد تقريباً في مسار الفحص؛ لا يثبت أن كل split مطلوب موجود أو أن المجموعة مطابقة للجهاز.
5. يفحص الحجم المعلن وعدد entries فقط؛ لا يظهر حد فعلي للبايتات المفكوكة أثناء streaming، ولا معالجة كاملة للتصادمات والمسارات والروابط الرمزية.
6. ينفذ ExtractToDirectory على archive غير موثوق دون إظهار طبقة staging آمنة مفصلة أو سياسة path/duplicate/case collision.
7. مسار OBB يبدأ بعد محاولة التثبيت دون اشتراط نجاحها، ولا يظهر تحقق كافٍ من مطابقة package name داخل manifest مع مجلد OBB قبل النقل.
8. يستخدم kill-server في نهاية العملية، وهذا قد يعطل جلسات ADB أخرى يديرها المستخدم أو أدوات أخرى.
9. يعرض نجاح التثبيت عند exit code ووجود Success في النص؛ يلزم التحقق من package/version على الجهاز بعد العملية.

**التصنيف:** مرجع لاستخراج متطلبات Windows/OBB وحالات الاستخدام فقط. **مرفوض للدمج المباشر**؛ يمكن اقتباس أفكار محددة بعد إعادة تصميم آمنة. لا نستخدم installer spoofing أو تعطيل التحقق أو استخراج الأرشيف بهذه الصورة.

### 20.39 مرجع Android-side لتغطية APKM/XAPK وSplit APKs

#### 20.39.1 CarlosSuarezFernandez/ApkInstaller

المستودع: [CarlosSuarezFernandez/ApkInstaller](https://github.com/CarlosSuarezFernandez/ApkInstaller).  
Commit المصدر المراجع: cb49fcd7bf1308ab71dc2e7dd548c6745d612bc6.

README يعلن دعم:
- APK
- APKS بصيغ SAI/bundletool
- XAPK من APKPure
- APKM غير المشفر
- ملفات APK منفصلة base + splits
- اختيار splits حسب ABI/density/language
- PackageInstaller API على Android

**مراجعة المصدر:**
- ArchiveExtractor.kt: يتعرف على APK من وجود AndroidManifest.xml، ويستخرج APK entries إلى أسماء staging مولدة لا تعتمد على مسارات archive. يدعم container extraction لـAPKS/XAPK/APKM غير المشفر.
- ManifestReader.kt: يقرأ package/split/version/configForSplit/isFeatureSplit من Binary AndroidManifest.
- SplitClassifier.kt: يصنف ABI/density/language/feature splits.
- SplitSelector.kt: يختار ABI حسب ترتيب ABI للجهاز، وكثافة قريبة، واللغات المتوافقة.
- SessionInstaller.kt: يستخدم Android PackageInstaller sessions ويكتب APK files ثم commit، مع progress وabandon عند exception.
- ApkParsingTest.kt: fixtures حقيقية موصوفة بأنها ناتجة من aapt2 link --split، وتغطي قراءة manifest وAPKS وXAPK واختيار ABI/density/language ورفض package/version mismatch وغياب base.

**قيود مهمة عند نقل الفكرة إلى Windows host:**
1. المشروع Android app؛ PackageInstaller API تعمل على الجهاز المضيف Android، ولا يمكن نقل هذا installer كما هو إلى برنامج Windows.
2. ArchiveExtractor.selectEntries يتعامل مع splits/ وasset-slices/ وstandalones/ بقاعدة أولية؛ لا نعتبرها بديلاً كاملاً عن قراءة Bundletool TOC واختيار DeviceSpec.
3. استخراج APK يستخدم copyTo دون حدود حجم فعلية/عدد entries أو وقت/مساحة staging ظاهرة في هذا الملف؛ نحتاج تطبيق سياسة archive security الموجودة بالقسم 20.33.
4. SplitSelector مرجع مفيد، لكن يجب تدقيق حالات default language، locales المتعددة، SDK-targeted splits، feature dependencies، وasset delivery قبل النقل.
5. الاختبارات الوحدوية لا تثبت نجاح التثبيت على Windows/ADB أو كل صيغ APKM/XAPK.

**التصنيف:** مرجع source-level ممتاز لبناء parser/selector واختبارات fixtures، وليس مكوّناً قابلاً لإعادة الاستخدام مباشرة في Windows. نفضل إعادة استخدام خوارزميات/أفكار محددة بعد التحقق، مع bundletool كمرجع APKS رسمي.

#### 20.39.2 Aefyr/SAI

المستودع: [Aefyr/SAI](https://github.com/Aefyr/SAI).

README يوضح أن SAI مثبت Android لحزم Split APKs، ويدعم طرق تثبيت Rooted وRootless، ويصرح المطور أن المشروع شبه متوقف مع احتمال إصلاحات محدودة. سبق تدقيق parser الخاص بـAPKM/XAPK في القسم 20.31.

**التصنيف:** مرجع تاريخي/تنفيذي لتنسيقات APKM وXAPK وسلوك split installation. لا نعتمد عليه كخدمة Windows جاهزة أو كخيار وحيد للمستقبل.

### 20.40 المعمارية المستهدفة لمحرك التثبيت الموحد

لا نبني parser واحداً يحاول تفسير كل صيغة؛ نستخدم dispatcher يوجه كل نوع إلى adapter متخصص، ثم يوحد الناتج في نموذج Package Install Plan.

```text
User / GUI / MCP
       |
       v
Install Request (explicit device serial)
       |
       v
Format Detection + File Intake
       |
       +--> Plain APK Adapter
       +--> Loose Split Set Adapter
       +--> APKS Adapter (bundletool)
       +--> APKM Adapter
       +--> XAPK Adapter ----> OBB Plan (separate approval)
       +--> AAB Adapter (later / optional)
       |
       v
Normalized Package Install Plan
       |
       +--> package/version consistency
       +--> signature verification evidence
       +--> device ABI/API/density/locale match
       +--> selected APKs + OBB metadata
       +--> warnings / unsupported reasons
       |
       v
Preview + explicit approval
       |
       v
ADB Install Engine (typed args, serial-bound)
       |
       +--> adb install
       +--> adb install-multiple
       +--> bundletool install-apks
       |
       v
Verify package/version on the same device
       |
       v
Operation result + redacted report + MCP result
```

#### 20.40.1 نموذج بيانات موحد

- InstallInput: source paths, detected format, file hashes, intake timestamp.
- DeviceTarget: exact serial, transport, API level, ABI list, density, locales, online state.
- PackageIdentity: package name, version code/name, signing certificate digest evidence, base/split identity.
- ApkArtifact: staging ID, original archive entry, size, hash, manifest metadata, split role, ABI/density/locale qualifier.
- ObbArtifact: package directory, file names/sizes/hashes, expected destination, provenance from container manifest.
- InstallPlan: package identity, selected APK artifacts, skipped variants/reasons, OBB plan, flags, warnings, estimated bytes.
- InstallResult: command path, operation state, raw exit status, normalized failure reason, installed package/version read-back, OBB copy state.

#### 20.40.2 قواعد التنفيذ

- تحديد Serial صريح لكل عملية، وعدم الاعتماد على اختيار ADB الضمني.
- لا shell command string؛ كل process invocation arguments منفصلة.
- لا -i لانتحال مصدر التثبيت، ولا -g أو -d أو -t تلقائياً.
- لا install-multiple قبل التحقق من package/version/base/split consistency.
- لا OBB push قبل نجاح تثبيت APK والتحقق من package mapping، ويحتاج معاينة وموافقة منفصلة.
- staging directory خاص باسم عشوائي، وحذف آمن بعد انتهاء العملية، مع الاحتفاظ بالتقرير فقط.
- حدود archive الفعلية تشمل عدد entries، أحجام الملفات المفكوكة فعلياً، الحجم الكلي، نسبة الضغط، الوقت، الذاكرة، ومساحة القرص.
- منع path traversal وabsolute/drive/UNC paths والروابط الرمزية والتكرار والتصادمات.
- التوقيع: التحقق من سلامة شهادة التوقيع ومقارنة split set؛ لا ندعي أن التوقيع يثبت أن التطبيق آمن أو موثوق.
- لا نعيد توقيع APKs الموردة ولا نعدل محتواها تلقائياً.
- التحديث بنفس package يتطلب توافق signing certificate؛ اختلاف التوقيع يعرض خطأ واضحاً ولا يقترح إزالة التطبيق تلقائياً.
- إلغاء العملية يوقف التحضير/النقل عندما يكون ذلك ممكناً؛ بعد commit قد لا يمكن التراجع عن تثبيت Package Manager الجاري.
- نتيجة التثبيت لا تساوي نجاح تشغيل التطبيق. يمكن إضافة Launch smoke check لاحقاً وبطلب منفصل.

#### 20.40.3 MCP

تُعرض نفس خدمة InstallPlan عبر MCP، وليس مسار تثبيت موازياً:
- inspect_install_input — تحليل الملفات فقط، read-only.
- preview_install — إنشاء الخطة واختيار الجهاز وعرض المخاطر.
- install_package — تنفيذ الخطة بعد موافقة واضحة ومربوطة بنفس plan/device/hash.
- get_install_operation — متابعة التقدم والنتيجة.
- verify_installed_package — قراءة package/version بعد التنفيذ.
- preview_obb_copy وapply_obb_copy — فصل بيانات OBB عن تثبيت APK.

يجب أن تكون الموافقة مرتبطة ببصمات الملفات والجهاز والخطة؛ أي تغيير في الملف أو Serial أو selected splits يبطل الموافقة السابقة.

#### 20.40.4 شروط قبول V1

1. تثبيت APK عادي وتحديث متوافق.
2. تثبيت split set صحيح، ورفض مجموعة ناقصة أو مختلطة.
3. APKS عبر bundletool مع اختيار الجهاز الصحيح.
4. APKM/XAPK غير المشفرين للعينات المحددة في corpus الاختبار، مع إظهار الصيغ غير المدعومة بوضوح.
5. توافق ABI/API الأساسي، واختيار density/locale عند الحاجة.
6. XAPK OBB: اكتشاف ومطابقة ومعاينة ونقل منفصل بعد موافقة.
7. عدة أجهزة: لا تحدث أي كتابة دون Serial محدد.
8. أخطاء التوقيع والـdowngrade والمساحة والاتصال تظهر برسالة قابلة للتصرف.
9. التحقق النهائي من package/version على الجهاز.
10. اختبارات Windows automated للـparser/security، ثم اختبارات ADB على أجهزة حقيقية موثقة؛ لا نخلط النوعين.

**حالة التحقق:** مراجعة وثائق ADB وbundletool رسمياً؛ مراجعة README ومقتطفات source لمشاريع المرشحين المذكورة؛ مراجعة unit tests في ApkInstaller. لم يتم تشغيل build أو test suite لأي مشروع خارجي، ولم يتم تثبيت APK على هاتف في هذه الجولة.
