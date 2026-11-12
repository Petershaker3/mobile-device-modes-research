# تشخيص وإصلاح تعريفات Android على Windows

> يحدد مصادر التعريفات الموثوقة، تشخيص واجهات USB، مسارات الإصلاح والتراجع، وضوابط الأمان واختبارات القبول.

## 18. تشخيص وإصلاح تعريفات أوضاع اتصال Android على Windows

### 18.1 الهدف والنطاق

إضافة وحدة مستقلة باسم **Windows Android Driver Diagnostics & Repair** لتشخيص تعريفات USB الخاصة بهواتف Android وإصلاحها عند وجود دليل واضح على أن المشكلة من تعريف Windows، مع استخدام Windows Update أو صفحات Google/OEM الرسمية فقط.

هذا المحور خاص بتعريفات Windows لواجهات الاتصال التي يشملها البحث، ومنها:
- Android ADB Interface / Composite ADB Interface.
- Android Bootloader / Fastboot Interface.
- MTP / Portable Devices.
- واجهات Samsung USB وCDC/Modem/Download Mode عند توفر تعريف رسمي مناسب للموديل.
- واجهات USB المركبة (Composite) والواجهات الفرعية التي ينشئها الجهاز.

**حدود مهمة:** ظهور واجهة USB في Device Manager لا يثبت أن بروتوكول الوضع نفسه يعمل، كما أن عدم ظهور ADB قد ينتج عن تعطيل USB debugging أو عدم قبول RSA authorization أو الكابل/المنفذ، وليس بالضرورة تعريفاً معطوباً. لا تُصنّف المشكلة Driver Failure إلا بعد جمع أدلة من Windows وطبقة الاتصال.

### 18.2 مصادر التعريفات المسموح بها

ترتيب الثقة المقترح:
1. Windows Update وOptional driver updates من Microsoft.
2. صفحة Google الرسمية لتعريف Google USB Driver، للأجهزة التي ينطبق عليها.
3. صفحة الدعم/التنزيل الرسمية للشركة المصنّعة للهاتف (OEM)، مع مطابقة الموديل وإصدار Windows ومعمارية النظام.
4. تعريفات USB class المدمجة في Windows؛ لا تطلب من المستخدم تنزيل تعريف Microsoft class driver يدوياً إذا كان النظام يضمّه بالفعل.

مصادر مرجعية:
- [Android Developers — Install OEM USB drivers](https://developer.android.com/studio/run/oem-usb): دليل Google وروابط دعم الشركات المصنّعة.
- [Android Developers — Get the Google USB Driver](https://developer.android.com/studio/run/win-usb): تنزيل Google USB Driver أو تثبيته من Android SDK Manager، مع توضيح أن بقية الشركات توفر تعريفاتها.
- [Microsoft Support — Update drivers through Device Manager](https://support.microsoft.com/en-gb/windows/hardware/drivers/update-drivers-through-device-manager-in-windows): تحديث/إعادة تثبيت/استرجاع التعريفات، وتوصية Windows Update وتجنب مواقع التعريفات غير الرسمية.
- [Microsoft Learn — USB device class drivers included in Windows](https://learn.microsoft.com/en-us/windows-hardware/drivers/usbcon/supported-usb-classes): تعريفات فئات USB المضمنة في Windows وتحديثها عبر Windows Update.
- [Microsoft Learn — Using Device Manager to uninstall devices and driver packages](https://learn.microsoft.com/en-us/windows-hardware/drivers/install/using-device-manager-to-uninstall-devices-and-driver-packages): تحذير من حذف Driver Package قد تستخدمه أجهزة أخرى، والتوصية بنقطة استعادة قبل عمليات الإزالة.

لا تُستخدم مواقع Driver Updater العامة أو روابط إعادة رفع ملفات INF/SYS/CAT كمصادر تنزيل. لا تُعدّل ملفات INF أو التواقيع الرقمية ولا تتجاوز Windows driver-signing enforcement.

### 18.3 التشخيص قبل الإصلاح

تجمع الوحدة، دون تغيير النظام:
- Windows edition/build/architecture.
- USB device instance ID وHardware IDs وCompatible IDs.
- Parent/child device tree لتحديد واجهات USB Composite.
- اسم الجهاز والفئة والحالة وProblem Code من Device Manager.
- Driver Provider وVersion وDate وINF Published Name وSigner/Signature status.
- حالة الجهاز: حاضر/غير حاضر، وتمييز الجهاز الحالي عن إدخالات الأجهزة القديمة المخفية.
- ربط الواجهة بالوضع المتوقع: MTP أو ADB أو Bootloader/Fastboot أو Samsung interface، فقط عند وجود Hardware ID أو دليل موثوق.
- مقارنة نتيجة Windows enumeration مع أدوات البروتوكول: adb devices، وfastboot devices، وظهور الجهاز في File Explorer/MTP. لا تعتبر فشل أداة واحدة دليلاً كافياً وحده.

تصنيفات التشخيص:
- Driver missing / no driver assigned.
- Driver error / Device Manager problem code.
- Wrong or incompatible driver binding.
- Driver package present but device absent.
- USB enumeration / cable / port issue suspected.
- Android-side authorization or mode configuration issue suspected.
- Unknown / insufficient evidence.

### 18.4 مسارات الإصلاح

| المسار | الإجراء | شروط التنفيذ |
|---|---|---|
| Windows Update | البحث عن تعريف مناسب، بما فيه Optional driver updates | عرض اسم التعريف والناشر قبل التثبيت |
| OEM official package | تنزيل الحزمة من صفحة الشركة الرسمية | مطابقة Hardware IDs والموديل وإصدار Windows والتحقق من التوقيع |
| Google USB Driver | تثبيت/تحديث تعريف Google عند انطباقه | لا يُعمم على كل هواتف Android |
| Device Manager reinstall | إعادة تثبيت تعريف الجهاز المحدد | حفظ الحالة الحالية، ثم إعادة فحص الجهاز |
| Rollback | الرجوع لإصدار سابق عند ارتباط المشكلة بتحديث حديث | وجود Driver version سابق وخيار Roll Back متاح |
| Rescan/reconnect | إعادة فحص الأجهزة بعد الإصلاح أو فصل/توصيل الهاتف | لا يتطلب حذف Driver Store |

### 18.5 ضوابط الأمان والتراجع

- الوضع الافتراضي Read-only: لا تثبيت ولا حذف ولا تعديل قبل موافقة المستخدم على عملية الإصلاح المحددة.
- عرض الشركة والناشر وإصدار التعريف وHardware IDs ومصدر التنزيل قبل التثبيت.
- التحقق من توقيع الحزمة الرقمية وسلسلة الثقة، وعدم قبول حزمة مجهولة أو غير موقعة.
- إنشاء Restore Point عندما تكون مدعومة، وحفظ تقرير بالحالة السابقة والتعريفات المستهدفة.
- لا تستخدم pnputil /delete-driver على نطاق واسع أو تحذف Driver Packages مشتركة/غير مرتبطة بالجهاز.
- لا تحذف كل تعريفات Android/Samsung دفعة واحدة، ولا تستخدم أدوات تنظيف Driver Store العامة.
- أي إزالة لحزمة تعريف تتطلب إثبات أنها الحزمة الخاطئة، وفحص الأجهزة الأخرى التي تستخدمها، وموافقة صريحة منفصلة.
- بعد الإصلاح أعد قراءة Device Manager وHardware IDs وDriver Provider/Version، ثم اختبر الواجهة المستهدفة فعلياً.
- نجاح تثبيت التعريف لا يعني نجاح ADB/Fastboot/MTP أو دعم وضع الشركة؛ سجّل نتائج كل طبقة بشكل منفصل.
- وفّر تقريراً قبل/بعد وسجل تغييرات وإمكانية Rollback عند توفرها.

### 18.6 واجهة المستخدم المقترحة

| العنصر | الوظيفة |
|---|---|
| Driver Health Scan | فحص الأجهزة والتعريفات دون تغيير |
| Device/Mode Mapping | عرض الهاتف والواجهة والوضع المتوقع |
| Problem & Evidence | رمز المشكلة وHardware IDs وسبب التشخيص |
| Official Source | رابط المصدر الرسمي واسم الناشر |
| Install / Repair | تنفيذ الإجراء الذي وافق عليه المستخدم فقط |
| Rollback | الرجوع للتعريف السابق إذا كان مدعوماً |
| Verify | إعادة الفحص واختبار الواجهة |
| Before/After Report | عرض الفروقات والنتيجة وسجل العمليات |

### 18.7 اختبارات القبول

- جهاز معروف بتعريف صحيح: لا يقترح إصلاحاً غير ضروري.
- جهاز بلا تعريف: يحدد Hardware IDs ويقترح المصدر الرسمي المناسب.
- تعريف خاطئ: يكتشف اختلاف Hardware IDs/الربط قبل الاستبدال.
- تعريف غير موقع أو مصدر غير موثوق: يرفض التثبيت.
- Composite USB: يميز الواجهات الفرعية ولا يستبدل تعريف الأب عشوائياً.
- ADB مع USB debugging مغلق: يصنفها مشكلة إعداد/وضع محتملة، لا Driver Failure تلقائياً.
- ADB unauthorized: يوضح أن المطلوب قبول RSA على الهاتف، لا إعادة تثبيت التعريف تلقائياً.
- MTP يعمل وADB لا يعمل: يشخص كل واجهة على حدة.
- Fastboot يعمل وMTP لا يعمل: لا يغيّر تعريف Fastboot.
- تحديث تعريف سبب المشكلة: يختبر Rollback ويعيد التحقق.
- تعريف مشترك بين أجهزة متعددة: يمنع الحذف الجماعي.
- فصل الهاتف أثناء الإصلاح: يوقف العملية بأمان ويعرض حالة الاستعادة.
- التحقق النهائي: يسجل نتائج Device Manager وADB/Fastboot/MTP كلٌّ على حدة.

### 18.8 حالة المحور

- [x] تحديد مصادر موثوقة من Google وMicrosoft وOEM.
- [x] فصل تشخيص تعريف Windows عن إعدادات Android ومشكلات الكابل/USB.
- [x] تحديد مسارات الإصلاح والتراجع وضوابط Driver Store.
- [x] إضافة اختبارات قبول تمنع الإصلاح الخاطئ.
- [ ] إعداد جدول Hardware IDs ↔ OEM driver ↔ supported Windows versions للموديلات الداخلة في الاختبار.
- [ ] تدقيق صفحات التنزيل الرسمية لكل شركة ضمن الأجهزة المستهدفة والتحقق من استمرار الروابط.
- [ ] تنفيذ تجارب فعلية على Windows مع أجهزة متعددة وأوضاع MTP/ADB/Fastboot/Samsung، وتسجيل النتائج قبل/بعد.
- [ ] تحديد ما إذا كانت أداة GUI جاهزة تغطي هذه الوظائف أم أن الوحدة تحتاج تنفيذاً مستقلاً.

**الحالة الحالية:** هذا تصميم بحثي ومتطلبات موثقة بالمصادر الرسمية، وليس ادعاءً بأن الإصلاح نُفّذ أو اختُبر على جهاز فعلي.
