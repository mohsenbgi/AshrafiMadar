# 🏭 سیستم نظارت کارخانه هوشمند صنعتی

یک سیستم جامع و پیشرفته برای نظارت بر کارخانه‌های صنعتی با قابلیت نمایش داده‌های لحظه‌ای سنسورها، هشدارها و آلارم‌های ایمنی.

## ✨ ویژگی‌های کلیدی

- **نمایش لحظه‌ای داده‌ها**: دریافت و نمایش داده‌های سنسورها به صورت Real-time
- **رابط کاربری زیبا**: طراحی مدرن و رسپانسیو با پالت رنگی گرم
- **سیستم هشدار هوشمند**: دو سطح هشدار (Warning و Alarm) با نمایش گرافیکی
- **نمودارهای تعاملی**: نمایش داده‌ها در قالب نمودارهای مختلف
- **نقشه کارخانه**: نمایش موقعیت سنسورها در بخش‌های مختلف کارخانه
- **پشتیبانی از زبان فارسی**: طراحی RTL با فونت Vazirmatn

## 🎯 سنسورهای پشتیبانی شده

### بخش‌های مختلف کارخانه:
1. **اتاق کوره**: دمای کوره صنعتی
2. **محیط عمومی**: رطوبت و سطح نور
3. **انبار مواد شیمیایی**: تشخیص گاز متان
4. **اتاق ژنراتور**: تشخیص گاز CO
5. **خط تولید**: صدای ماشین‌آلات
6. **مخزن تحت فشار**: فشار مخازن
7. **تابلو برق اصلی**: جریان و دکمه اضطراری
8. **موتور اصلی**: لرزش موتور
9. **ورودی برق**: ولتاژ ورودی
10. **نوار نقاله**: فاصله‌سنج
11. **تاسیسات**: تشخیص نشت آب
12. **انبار مواد اولیه**: تشخیص آتش
13. **درب ورودی**: وضعیت درب
14. **شیر خنک‌کننده**: کنترل خودکار

## 🚀 نحوه راه‌اندازی

### پیش‌نیازها:
- .NET 9.0 SDK
- Visual Studio یا VS Code

### مراحل اجرا:

1. **انتقال پوشه ClientConsoleApp به خارج از پروژه**

2. **بازیابی پکیج‌ها:**
```bash
dotnet restore
```

3. **اجرای برنامه:**
```bash
dotnet run --urls="http://0.0.0.0:5000"
```

4. **دسترسی به داشبورد:**
   - مرورگر خود را باز کنید
   - به آدرس `https://localhost:5001` بروید

## 📡 API Endpoints

### ارسال داده‌های سنسور:

#### POST `/api/sensor/data`
ارسال داده‌های سنسور به صورت JSON:
```json
{
  "sensorString": "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\nFurnace_Temp:892.00,Env_Humid:45.50,Light_Level:210,Gas_Methane:15,Gas_CO:61,Machine_Sound:512,Tank_Pressure:72,Main_Current:123,Engine_Vibe:25,Input_Voltage:230,Conveyor_Dist:150,Water_Leak:100,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:90"
}
```

#### POST `/api/sensor/raw`
ارسال داده‌های خام سنسور:
```
Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE
Furnace_Temp:892.00,Env_Humid:45.50,Light_Level:210,Gas_Methane:15,Gas_CO:61,Machine_Sound:512,Tank_Pressure:72,Main_Current:123,Engine_Vibe:25,Input_Voltage:230,Conveyor_Dist:150,Water_Leak:100,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:90
```

### دریافت داده‌ها:

#### GET `/api/sensor/current`
دریافت آخرین داده‌های سنسور

#### GET `/api/sensor/locations`
دریافت لیست موقعیت‌های سنسورها

### تست و شبیه‌سازی:

#### POST `/api/test/simulate`
شبیه‌سازی داده‌های عادی

#### POST `/api/test/simulate-warning`
شبیه‌سازی داده‌های هشدار

#### POST `/api/test/simulate-emergency`
شبیه‌سازی داده‌های اضطراری

## 🔧 نحوه استفاده

### ارسال داده از Arduino/ESP:

```cpp
// مثال کد Arduino برای ارسال داده
void sendSensorData() {
    String data = "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE\n";
    data += "Furnace_Temp:" + String(furnaceTemp) + ",";
    data += "Env_Humid:" + String(humidity) + ",";
    data += "Light_Level:" + String(lightLevel) + ",";
    // ... سایر سنسورها
    
    HTTPClient http;
    http.begin("http://your-server-ip:5000/api/sensor/raw");
    http.addHeader("Content-Type", "text/plain");
    int httpResponseCode = http.POST(data);
    http.end();
}
```

### تست از طریق curl:

```bash
curl -X POST "https://localhost:5000/api/sensor/raw" \
  -H "Content-Type: text/plain" \
  -d "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE
Furnace_Temp:892.00,Env_Humid:45.50,Light_Level:210,Gas_Methane:15,Gas_CO:61,Machine_Sound:512,Tank_Pressure:72,Main_Current:123,Engine_Vibe:25,Input_Voltage:230,Conveyor_Dist:150,Water_Leak:100,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:90"
```

## 🚨 سیستم هشدار

### چراغ هشدار عمومی (Warning LED):
- **درب باز**: وقتی درب ورودی باز باشد
- **نشت آب**: سطح آب بالای 400
- **جریان بالا**: جریان مصرفی بیش از 150 آمپر
- **لرزش موتور**: لرزش بیش از 50 هرتز
- **صدای غیرعادی**: صدای بیش از 600 دسی‌بل

### آژیر و چراغ خطر اصلی (Alarm System):
- **آتش‌سوزی**: تشخیص آتش (Flame_Status > 0)
- **نشت گاز متان**: سطح بیش از 300 ppm
- **گاز CO**: سطح بیش از 100 ppm
- **فشار خطرناک**: فشار مخزن بیش از 90 PSI
- **دمای بحرانی**: دمای کوره بیش از 1000 درجه
- **توقف اضطراری**: فعال شدن دکمه E-Stop

## 🎨 طراحی و UI/UX

- **پالت رنگی گرم**: استفاده از رنگ‌های آرام و دوستانه
- **طراحی رسپانسیو**: سازگار با موبایل، تبلت و دسکتاپ
- **انیمیشن‌های زیبا**: جلوه‌های بصری جذاب و نرم
- **نمودارهای تعاملی**: Chart.js برای نمایش داده‌ها
- **فونت فارسی**: Vazirmatn برای بهترین خوانایی

## 🔄 Real-time Communication

سیستم از SignalR استفاده می‌کند تا داده‌ها به صورت لحظه‌ای به کلاینت‌ها ارسال شوند.

## 📱 سازگاری

- **مرورگرهای مدرن**: Chrome, Firefox, Safari, Edge
- **دستگاه‌های موبایل**: iOS و Android
- **تبلت‌ها**: iPad و Android tablets
- **دسکتاپ**: Windows, macOS, Linux

---
**توسعه‌یافته برای پایان‌نامه هوشمندسازی و مانیتورینگ کارخانه صنعتی** 🎓
