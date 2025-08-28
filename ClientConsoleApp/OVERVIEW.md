# 🏭 Smart Factory Monitoring System - Complete Solution

## 📁 Project Structure

```
/workspace/
├── 🌐 Main Web Application (ASP.NET Core)
│   ├── Controllers/
│   │   ├── SensorController.cs      # API endpoints for sensor data
│   │   └── TestController.cs        # Testing and simulation endpoints
│   ├── Services/
│   │   └── SensorDataService.cs     # Data processing and analysis
│   ├── Models/
│   │   └── SensorData.cs           # Data models and configurations
│   ├── Hubs/
│   │   └── SensorHub.cs            # SignalR real-time communication
│   ├── Pages/
│   │   ├── Index.cshtml            # Main dashboard page
│   │   └── Index.cshtml.cs         # Page model
│   ├── wwwroot/css/
│   │   ├── site.css                # Base styles
│   │   └── dashboard.css           # Dashboard-specific styles
│   ├── Program.cs                  # Application startup
│   ├── AshrafiMadar.csproj        # Project configuration
│   ├── README.md                   # Web app documentation
│   ├── test-api.sh                # Shell script for API testing
│   └── test_api.py                # Python script for API testing
│
└── 📡 Client Console Application
    └── ClientConsoleApp/SmartFactoryClient/
        ├── Services/
        │   ├── SerialPortService.cs         # COM port communication
        │   ├── ApiClientService.cs          # HTTP API client
        │   ├── DataParserService.cs         # Data parsing and validation
        │   └── SmartFactoryClientService.cs # Main application service
        ├── Models/
        │   └── SensorData.cs               # Data models and configuration
        ├── Program.cs                      # Console app entry point
        ├── appsettings.json               # Configuration file
        ├── SmartFactoryClient.csproj      # Project file
        ├── README.md                      # Client app documentation
        ├── run.bat                        # Windows launcher
        ├── run.sh                         # Linux/Mac launcher
        └── demo.py                        # Demo simulation script
```

## 🚀 Quick Start Guide

### 1. Start the Web Application (Main Server)

```bash
# Navigate to main project
cd /workspace

# Run the web application
export PATH="$PATH:/home/ubuntu/.dotnet"
dotnet run --urls="http://0.0.0.0:5000"
```

**Web Dashboard**: http://localhost:5000

### 2. Start the Client Application

```bash
# Navigate to client project
cd /workspace/ClientConsoleApp/SmartFactoryClient

# Run the client (will auto-detect COM ports or use simulation mode)
dotnet run

# OR use the launcher scripts:
./run.sh          # Linux/Mac
# or
run.bat           # Windows
```

### 3. Test with Demo Data

```bash
# Run the demo simulation (sends test data to API)
python3 demo.py

# OR test API directly
./test-api.sh
```

## 📡 Data Flow Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│     Proteus     │───▶│  Client Console │───▶│   Web Server    │
│   (COM Port)    │    │   Application   │    │   (ASP.NET)     │
│                 │    │                 │    │                 │
│  • Sensor Data  │    │  • COM Reader   │    │  • API Endpoints│
│  • Real Values  │    │  • Data Parser  │    │  • Data Analysis│
│  • Status Info  │    │  • HTTP Client  │    │  • SignalR Hub  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                                        │
                                                        ▼
                       ┌─────────────────┐    ┌─────────────────┐
                       │   Web Browser   │◀───│   Dashboard     │
                       │                 │    │                 │
                       │  • Real-time UI │    │  • Charts       │
                       │  • Responsive   │    │  • Warnings     │
                       │  • Persian RTL  │    │  • Factory Map  │
                       └─────────────────┘    └─────────────────┘
```

## 🎯 Sensor Data Format

The system expects data from Proteus in this exact format:

```
Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE
Furnace_Temp:892.00,Env_Humid:0.00,Light_Level:210,Gas_Methane:0,Gas_CO:61,Machine_Sound:512,Tank_Pressure:72,Main_Current:123,Engine_Vibe:0,Input_Voltage:0,Conveyor_Dist:0,Water_Leak:512,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:90
```

### Data Structure:
- **Line 1**: System status header
- **Line 2**: Comma-separated sensor values in `Key:Value` format

### Supported Sensors (16 total):
1. **Furnace_Temp** - Furnace temperature (°C)
2. **Env_Humid** - Environmental humidity (%)
3. **Light_Level** - Light level (lux)
4. **Gas_Methane** - Methane gas level (ppm)
5. **Gas_CO** - Carbon monoxide level (ppm)
6. **Machine_Sound** - Machine sound level (dB)
7. **Tank_Pressure** - Tank pressure (PSI)
8. **Main_Current** - Main electrical current (A)
9. **Engine_Vibe** - Engine vibration (Hz)
10. **Input_Voltage** - Input voltage (V)
11. **Conveyor_Dist** - Conveyor distance sensor (cm)
12. **Water_Leak** - Water leak sensor level
13. **Flame_Status** - Flame detection (0/1)
14. **Gate_Status** - Gate status (0=closed, 1=open)
15. **E_Stop_Button** - Emergency stop button (0/1)
16. **Coolant_Valve** - Coolant valve position (%)

## 🚨 Warning System

### 💡 Warning LED (Non-Critical)
- Gate open
- Water leak > 400
- High current > 150A
- Engine vibration > 50Hz
- Abnormal sound > 600dB

### 🚨 Alarm System (Critical)
- Fire detected (Flame_Status = 1)
- Gas leak (Methane > 300 ppm)
- CO poisoning (CO > 100 ppm)
- Pressure danger (Pressure > 90 PSI)
- Overheating (Temperature > 1000°C)
- Emergency stop activated

## 🌐 API Endpoints

### Main Endpoints:
- `POST /api/sensor/raw` - Receive raw sensor data
- `POST /api/sensor/data` - Receive JSON sensor data
- `GET /api/sensor/current` - Get current sensor values
- `GET /api/sensor/locations` - Get sensor information

### Test Endpoints:
- `POST /api/test/simulate` - Send normal test data
- `POST /api/test/simulate-warning` - Send warning test data
- `POST /api/test/simulate-emergency` - Send emergency test data

## 🎨 Dashboard Features

### Real-time Visualization:
- **📊 Interactive Charts**: Line, bar, doughnut, and radar charts
- **🏗️ Factory Layout**: Visual representation of sensor locations
- **🚨 Warning Lights**: Animated warning and alarm indicators
- **📱 Responsive Design**: Works on mobile, tablet, and desktop
- **🌐 Persian Support**: RTL layout with Vazirmatn font

### Visual Elements:
- **Color-coded sensors** based on safety thresholds
- **Animated warning lights** with realistic blinking effects
- **Gradient backgrounds** and modern glass-morphism design
- **Real-time updates** without page refresh using SignalR

## 🔧 Configuration

### Client Application (`appsettings.json`):
```json
{
  "SmartFactory": {
    "ApiBaseUrl": "http://localhost:5000/api",
    "RetryAttempts": 3
  },
  "SerialPort": {
    "PortName": "COM1",
    "BaudRate": 9600
  }
}
```

### COM Port Setup for Proteus:
1. In Proteus, add COMPIM components
2. Set COMPIM1 as transmitter (connects to your circuit)
3. Set COMPIM2 as receiver (connects to PC)
4. Configure baud rate to 9600
5. Use the port name shown in Device Manager

## 🧪 Testing Scenarios

### 1. Normal Operation Test:
```bash
# Send normal sensor data
curl -X POST "http://localhost:5000/api/sensor/raw" \
  -H "Content-Type: text/plain" \
  -d "Smart Factory Monitoring System - AUTOMATIC MODE - ONLINE
Furnace_Temp:850.00,Env_Humid:45.00,Light_Level:200,Gas_Methane:15,Gas_CO:50,Machine_Sound:500,Tank_Pressure:70,Main_Current:120,Engine_Vibe:20,Input_Voltage:230,Conveyor_Dist:150,Water_Leak:100,Flame_Status:0,Gate_Status:0,E_Stop_Button:0,Coolant_Valve:80"
```

### 2. Warning Condition Test:
```bash
# Trigger warning lights
curl -X POST "http://localhost:5000/api/test/simulate-warning"
```

### 3. Emergency Condition Test:
```bash
# Trigger emergency alarms
curl -X POST "http://localhost:5000/api/test/simulate-emergency"
```

## 📊 Performance Metrics

The system tracks:
- **Data Reception Rate**: Packets received per minute
- **API Success Rate**: Successful API calls percentage
- **Error Count**: Failed operations
- **Response Time**: Average API response time
- **Connection Status**: Serial port and API connectivity

## 🔍 Troubleshooting

### Common Issues:

1. **COM Port Not Found**:
   - Check Proteus COMPIM configuration
   - Verify port name in Device Manager
   - Client will auto-detect available ports

2. **API Connection Failed**:
   - Ensure web server is running on port 5000
   - Check firewall settings
   - Verify API URL in configuration

3. **Data Format Errors**:
   - Verify Proteus sends data in exact expected format
   - Check for missing sensors in data string
   - Enable debug logging to see raw data

4. **Dashboard Not Updating**:
   - Check browser console for JavaScript errors
   - Verify SignalR connection
   - Refresh the page

## 🎓 Educational Value

This project demonstrates:
- **Real-time IoT communication** between hardware simulation and web applications
- **Industrial monitoring systems** with safety protocols
- **Modern web technologies** (ASP.NET Core, SignalR, responsive design)
- **Serial communication protocols** and data parsing
- **API design** and HTTP client implementation
- **Error handling** and retry mechanisms
- **Persian/RTL web development**

## 📝 Future Enhancements

Potential improvements:
- **Database integration** for historical data storage
- **User authentication** and role-based access
- **Email/SMS alerts** for critical conditions
- **Mobile app** for remote monitoring
- **Machine learning** for predictive maintenance
- **Export functionality** for reports and analytics

---

## 🏆 Project Summary

This Smart Factory Monitoring System provides a complete solution for industrial IoT monitoring with:

✅ **Real-time data collection** from Proteus simulation  
✅ **Professional web dashboard** with Persian language support  
✅ **Robust communication** between hardware and software  
✅ **Safety monitoring** with warning and alarm systems  
✅ **Modern UI/UX** with responsive design  
✅ **Comprehensive testing** and simulation tools  
✅ **Production-ready** architecture and error handling  

Perfect for thesis demonstration and real-world industrial applications! 🎓🏭