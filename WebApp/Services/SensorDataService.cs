using AshrafiMadar.Models;
using System.Globalization;

namespace AshrafiMadar.Services
{
    public class SensorDataService
    {
        private SensorData _currentData = new SensorData();
        private readonly List<SensorLocation> _sensorLocations;

        public SensorDataService()
        {
            _sensorLocations = InitializeSensorLocations();
        }

        public SensorData ParseSensorString(string sensorString)
        {
            try
            {
                // Remove the header part
                var dataStart = sensorString.IndexOf("Furnace_Temp:");
                if (dataStart == -1) return _currentData;

                var dataString = sensorString.Substring(dataStart);
                var pairs = dataString.Split(',');

                var sensorData = new SensorData();

                foreach (var pair in pairs)
                {
                    var keyValue = pair.Split(':');
                    if (keyValue.Length != 2) continue;

                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();

                    switch (key)
                    {
                        case "Furnace_Temp":
                            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double furnaceTemp))
                                sensorData.Furnace_Temp = furnaceTemp;
                            break;
                        case "Env_Humid":
                            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double envHumid))
                                sensorData.Env_Humid = envHumid;
                            break;
                        case "Light_Level":
                            if (int.TryParse(value, out int lightLevel))
                                sensorData.Light_Level = lightLevel;
                            break;
                        case "Gas_Methane":
                            if (int.TryParse(value, out int gasMethane))
                                sensorData.Gas_Methane = gasMethane;
                            break;
                        case "Gas_CO":
                            if (int.TryParse(value, out int gasCO))
                                sensorData.Gas_CO = gasCO;
                            break;
                        case "Machine_Sound":
                            if (int.TryParse(value, out int machineSound))
                                sensorData.Machine_Sound = machineSound;
                            break;
                        case "Tank_Pressure":
                            if (int.TryParse(value, out int tankPressure))
                                sensorData.Tank_Pressure = tankPressure;
                            break;
                        case "Main_Current":
                            if (int.TryParse(value, out int mainCurrent))
                                sensorData.Main_Current = mainCurrent;
                            break;
                        case "Engine_Vibe":
                            if (int.TryParse(value, out int engineVibe))
                                sensorData.Engine_Vibe = engineVibe;
                            break;
                        case "Input_Voltage":
                            if (int.TryParse(value, out int inputVoltage))
                                sensorData.Input_Voltage = inputVoltage;
                            break;
                        case "Conveyor_Dist":
                            if (int.TryParse(value, out int conveyorDist))
                                sensorData.Conveyor_Dist = conveyorDist;
                            break;
                        case "Water_Leak":
                            if (int.TryParse(value, out int waterLeak))
                                sensorData.Water_Leak = waterLeak;
                            break;
                        case "Flame_Status":
                            if (int.TryParse(value, out int flameStatus))
                                sensorData.Flame_Status = flameStatus;
                            break;
                        case "Gate_Status":
                            if (int.TryParse(value, out int gateStatus))
                                sensorData.Gate_Status = gateStatus;
                            break;
                        case "E_Stop_Button":
                            if (int.TryParse(value, out int eStopButton))
                                sensorData.E_Stop_Button = eStopButton;
                            break;
                        case "Coolant_Valve":
                            if (int.TryParse(value, out int coolantValve))
                                sensorData.Coolant_Valve = coolantValve;
                            break;
                    }
                }

                // Analyze warnings and alarms
                AnalyzeWarningsAndAlarms(sensorData);
                
                _currentData = sensorData;
                return sensorData;
            }
            catch (Exception)
            {
                return _currentData;
            }
        }

        private void AnalyzeWarningsAndAlarms(SensorData data)
        {
            data.ActiveWarnings.Clear();
            data.ActiveAlarms.Clear();

            // Critical alarms (Alarm_System = true)
            if (data.Flame_Status > 0)
            {
                data.ActiveAlarms.Add("🔥 آتش‌سوزی تشخیص داده شد!");
                data.Alarm_System = true;
            }

            if (data.Gas_Methane > 700)
            {
                data.ActiveAlarms.Add("⚠️ نشت گاز متان خطرناک!");
                data.Alarm_System = true;
            }

            if (data.Gas_CO > 650)
            {
                data.ActiveAlarms.Add("☠️ سطح مونوکسید کربن بحرانی!");
                data.Alarm_System = true;
            }

            if (data.Tank_Pressure > 800)
            {
                data.ActiveAlarms.Add("💥 فشار مخزن در حد خطر!");
                data.Alarm_System = true;
            }

            if (data.Furnace_Temp > 600)
            {
                data.ActiveAlarms.Add("🌡️ دمای کوره بیش از حد مجاز!");
                data.Alarm_System = true;
            }

            if (data.E_Stop_Button > 0)
            {
                data.ActiveAlarms.Add("🛑 دکمه توقف اضطراری فعال!");
                data.Alarm_System = true;
            }

            // Non-critical warnings (Warning_LED = true)
            if (data.Gate_Status > 0)
            {
                data.ActiveWarnings.Add("🚪 درب ورودی اصلی باز است");
                data.Warning_LED = true;
            }

            if (data.Water_Leak > 600)
            {
                data.ActiveWarnings.Add("💧 نشت آب در تاسیسات");
                data.Warning_LED = true;
            }

            if (data.Main_Current > 150)
            {
                data.ActiveWarnings.Add("⚡ جریان مصرفی بالا");
                data.Warning_LED = true;
            }

            if (data.Engine_Vibe > 50)
            {
                data.ActiveWarnings.Add("📳 لرزش موتور غیرعادی");
                data.Warning_LED = true;
            }

            if (data.Machine_Sound > 600)
            {
                data.ActiveWarnings.Add("🔊 صدای غیرعادی از خط تولید");
                data.Warning_LED = true;
            }
        }

        public List<SensorLocation> GetSensorLocations()
        {
            return _sensorLocations;
        }

        public SensorData GetCurrentData()
        {
            return _currentData;
        }

        private List<SensorLocation> InitializeSensorLocations()
        {
            return new List<SensorLocation>
            {
                new SensorLocation { Key = "Furnace_Temp", Name = "دمای کوره", Area = "اتاق کوره", Description = "نظارت بر دمای اصلی کوره صنعتی", Unit = "°C", MaxSafe = 1000, Category = "Temperature", Icon = "🌡️", Color = "#FF6B6B" },
                new SensorLocation { Key = "Env_Humid", Name = "رطوبت محیط", Area = "محیط عمومی", Description = "نظارت بر رطوبت کلی محیط کارخانه", Unit = "%", MaxSafe = 80, Category = "Environment", Icon = "💧", Color = "#4ECDC4" },
                new SensorLocation { Key = "Light_Level", Name = "سطح نور", Area = "محیط عمومی", Description = "کنترل هوشمند روشنایی عمومی", Unit = "lux", MinSafe = 100, Category = "Environment", Icon = "💡", Color = "#FFE66D" },
                new SensorLocation { Key = "Gas_Methane", Name = "گاز متان", Area = "انبار مواد شیمیایی", Description = "تشخیص نشت گازهای قابل اشتعال", Unit = "ppm", MaxSafe = 300, Category = "Safety", Icon = "⚠️", Color = "#FF8E53" },
                new SensorLocation { Key = "Gas_CO", Name = "مونوکسید کربن", Area = "اتاق ژنراتور", Description = "تشخیص نشت گاز سمی CO", Unit = "ppm", MaxSafe = 100, Category = "Safety", Icon = "☠️", Color = "#A8E6CF" },
                new SensorLocation { Key = "Machine_Sound", Name = "صدای ماشین‌آلات", Area = "خط تولید", Description = "تشخیص صداهای ناهنجار", Unit = "dB", MaxSafe = 600, Category = "Production", Icon = "🔊", Color = "#DDA0DD" },
                new SensorLocation { Key = "Tank_Pressure", Name = "فشار مخزن", Area = "مخزن تحت فشار", Description = "نظارت بر فشار داخل مخازن", Unit = "PSI", MaxSafe = 90, Category = "Safety", Icon = "💥", Color = "#98D8C8" },
                new SensorLocation { Key = "Main_Current", Name = "جریان اصلی", Area = "تابلو برق اصلی", Description = "نظارت بر جریان مصرفی کل", Unit = "A", MaxSafe = 150, Category = "Electrical", Icon = "⚡", Color = "#F7DC6F" },
                new SensorLocation { Key = "Engine_Vibe", Name = "لرزش موتور", Area = "موتور اصلی", Description = "نظارت بر لرزش موتور", Unit = "Hz", MaxSafe = 50, Category = "Mechanical", Icon = "📳", Color = "#BB8FCE" },
                new SensorLocation { Key = "Input_Voltage", Name = "ولتاژ ورودی", Area = "ورودی برق", Description = "نظارت بر ولتاژ برق ورودی", Unit = "V", MinSafe = 220, MaxSafe = 240, Category = "Electrical", Icon = "🔌", Color = "#85C1E9" },
                new SensorLocation { Key = "Conveyor_Dist", Name = "فاصله نوار نقاله", Area = "نوار نقاله", Description = "تشخیص وجود محصول", Unit = "cm", Category = "Production", Icon = "📦", Color = "#F8C471" },
                new SensorLocation { Key = "Water_Leak", Name = "نشت آب", Area = "تاسیسات", Description = "تشخیص نشتی آب", Unit = "level", MaxSafe = 400, Category = "Safety", Icon = "💧", Color = "#7FB3D3" },
                new SensorLocation { Key = "Flame_Status", Name = "تشخیص آتش", Area = "انبار مواد اولیه", Description = "تشخیص سریع آتش‌سوزی", Unit = "status", MaxSafe = 0, Category = "Safety", Icon = "🔥", Color = "#EC7063" },
                new SensorLocation { Key = "Gate_Status", Name = "وضعیت درب", Area = "درب ورودی اصلی", Description = "نظارت بر درب ورودی", Unit = "status", Category = "Security", Icon = "🚪", Color = "#82E0AA" },
                new SensorLocation { Key = "E_Stop_Button", Name = "توقف اضطراری", Area = "تابلو برق اصلی", Description = "دکمه توقف اضطراری", Unit = "status", Category = "Safety", Icon = "🛑", Color = "#F1948A" },
                new SensorLocation { Key = "Coolant_Valve", Name = "شیر خنک‌کننده", Area = "شیر خنک‌کننده کوره", Description = "کنترل خودکار شیر آب", Unit = "%", Category = "Control", Icon = "🌊", Color = "#76D7C4" }
            };
        }
    }
}