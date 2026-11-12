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

- **Read-only افتراضياً:** اكتشاف الأجهزة وقراءة المعلومات والقوائم والتقارير لا تغير حالة الهاتف.
- **موافقة منفصلة للكتابة:** التثبيت والحذف والتعطيل والتفعيل وتغيير DNS وإصلاح التعريفات لا تنفذ لمجرد أن النموذج طلبها.
- **Preview ثم Apply:** العمليات الحساسة تعرض الجهاز المستهدف (Serial/اسم مفهوم)، والقيمة الحالية والجديدة، والأثر المتوقع، ثم تتطلب موافقة المستخدم.
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
