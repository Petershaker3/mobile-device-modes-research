# `contracts/` — العقود والـcorpus القابل للفحص

هذا المجلد هو الجزء الوحيد في المستودع الذي **يُتحقق منه آلياً** اليوم. كل ما فيه يعمل بلا .NET وبلا جهاز.

```
contracts/
├── schemas/          JSON Schema (Draft 2020-12) لكل عقد مشترك
├── examples/         أمثلة صحيحة، منها سيناريوهات الفشل الحرجة
├── tools/
│   ├── validate_contracts.py    يفحص الـschemas والأمثلة و10 حالات سلبية
│   └── make_fixtures.py         يولّد corpus اصطناعي حتمي (فئات أ وC)
├── corpus/           مواصفة الـfixtures + المولَّد (مستثنى من Git)
└── THREAT-MODEL.md   13 مسار هجوم ↔ ضابط ↔ اختبار
```

## التشغيل

```bash
pip install 'jsonschema>=4.18'

python contracts/tools/validate_contracts.py        # العقود: 45 فحصاً
python contracts/tools/make_fixtures.py             # corpus: توليد + تحقق
python contracts/tools/make_fixtures.py --check     # تحقق فقط (exit 1 عند أي انحراف)
```

## ماذا يعني كل نتيجة

| الأمر | النجاح يعني | لا يعني |
|---|---|---|
| `validate_contracts.py` | العقود متسقة، والأمثلة تطابقها، و10 وثائق خطأ **مرفوضة** | أن كود C# يطبّقها |
| `make_fixtures.py` | الـcorpus موجود، بطاقاته صحيحة، وكل أرشيف خبيث يحمل هجومه فعلاً، ولا يوجد ما يشبه IMEI | أن أي parser يقرأها بشكل صحيح |

## الوثائق المرتبطة

- [19 — العقود المشتركة](../docs/19-SHARED-CONTRACTS.md) — الشرح والقرارات التصميمية.
- [Threat Model](THREAT-MODEL.md) — التهديدات ومصفوفة «ضابط ↔ اختبار».
- [corpus/README.md](corpus/README.md) — مواصفة كل fixture وما يثبته وما لا يثبته.
- [18 — القرارات الهندسية](../docs/18-ENGINEERING-DECISIONS.md) — الأرقام التي تفترضها الاختبارات (مهلات، حدود، retention).

## القاعدة الحاكمة

> عقد لا يرفض وثيقة خاطئة ليس عقداً. وfixture لا يكسر شيئاً ليس fixture.
